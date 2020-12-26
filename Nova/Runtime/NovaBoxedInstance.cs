// -----------------------------------------------------------------------
// <copyright file="NovaBoxedObject.cs" Company="Michael Tindal">
// Copyright 2011-2013 Michael Tindal
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nova.Builtins;

namespace Nova.Runtime {
    public class NovaBoxedInstance : NovaInstance {
        private static readonly Dictionary<object, NovaBoxedInstance> _boxCache =
            new Dictionary<object, NovaBoxedInstance>();

        internal NovaBoxedInstance(object obj, NovaScope scope, NovaClass @class) : base(@class) {
            BoxedObject = obj;
            BoxedScope = scope;
        }

        protected NovaBoxedInstance(object obj, NovaScope scope) : base(GetBoxClass(obj)) {
            BoxedObject = obj;
            BoxedScope = scope;
        }

        internal object BoxedObject { get; private set; }

        internal NovaScope BoxedScope { get; private set; }

        public static NovaBoxedInstance Box(object obj, NovaScope scope = null) {
            if (obj == null) {
                return null;
            }
            if (_boxCache.ContainsKey(obj)) {
                _boxCache[obj].BoxedScope.MergeWithScope(scope ?? new NovaScope());
                return _boxCache[obj];
            }
            var boxed = new NovaBoxedInstance(obj, scope ?? new NovaScope());
            _boxCache[obj] = boxed;
            if (scope != null) {
                string name;
                var _scope = scope.SearchForObject(obj, out name);
                if (_scope != null) {
                    _scope[name] = boxed;
                }
            }
            return boxed;
        }

        public static NovaBoxedInstance BoxNoCache(object obj, NovaScope scope = null) {
            if (obj == null) {
                return null;
            }
            var boxed = new NovaBoxedInstance(obj, scope ?? new NovaScope());
            if (scope != null) {
                string name;
                var _scope = scope.SearchForObject(obj, out name);
                if (_scope != null) {
                    _scope[name] = boxed;
                }
            }
            return boxed;
        }

        public static dynamic Unbox(NovaBoxedInstance obj) {
            _boxCache.Remove(obj.BoxedObject);
            return obj.BoxedObject;
        }

        private static NovaClass GetBoxClass(object obj) {
            return Nova.Box(obj.GetType());
        }

        // Nova -> .net
        internal static void SyncInstanceVariablesFrom(NovaInstance NovaObject, object obj) {
            var _fields =
                obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
            NovaObject.InstanceVariables.Variables.Keys.ToList().ForEach(key => {
                var _fq = _fields.Where(field => field.Name == key);
                if (_fq.Any())
                {
                    var val = NovaObject.InstanceVariables[key];
                    if (val is NovaNumber)
                    {
                        val = NovaNumber.Convert(val);
                    }
                    _fq.First().SetValue(obj, val);
                }
            });
        }

        // .net -> Nova
        internal static void SyncInstanceVariablesTo(NovaInstance NovaObject, object obj) {
            var _fields =
                obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
            _fields.ForEach(field => NovaObject.InstanceVariables[field.Name] = field.GetValue(obj));
        }
    }
}