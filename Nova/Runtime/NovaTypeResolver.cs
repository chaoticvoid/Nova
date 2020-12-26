//
//  Copyright 2013  Michael Tindal
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nova.Runtime {
    public static class NovaTypeResolver {
        private static readonly List<string> _includedNamespaces = new List<string>();

        static NovaTypeResolver() {
            _includedNamespaces.Add(""); // So you can specify the namespace yourself
            _includedNamespaces.Add("System");
        }

        public static void Include(string @namespace) {
            _includedNamespaces.Add(@namespace);
        }

        public static Type Resolve(string name) {
            var mq =
                _includedNamespaces.Where(@namespace => Type.GetType(string.Format("{0}.{1}", @namespace, name)) != null);
            if (mq.Any()) {
                return Type.GetType(string.Format("{0}.{1}", mq.First(), name));
            }
            return null;
        }
    }
}