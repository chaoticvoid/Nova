// -----------------------------------------------------------------------
// <copyright file="NovaClass.cs" Company="Michael Tindal">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Nova.Expressions;
using Nova.Parser;

namespace Nova.Runtime {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public partial class NovaClass {
        #region Properties

        internal static readonly Dictionary<Type, NovaClass> TypeCache = new Dictionary<Type, NovaClass>();

        public Dictionary<string, NovaMethodTable> ClassMethods { get; private set; }

        public Dictionary<string, NovaMethodTable> InstanceMethods { get; private set; }

        public List<string> UndefinedMethods { get; private set; }

        public List<string> RemovedMethods { get; private set; }

        public NovaScope Context { get; internal set; }

        public string Name { get; internal set; }

        public NovaClass Parent { get; internal set; }

        #endregion

        public NovaClass(string name, NovaClass parent, List<NovaFunction> classMethods,
            List<NovaFunction> instanceMethods) {
            Name = name;
            ClassMethods = new Dictionary<string, NovaMethodTable>();
            classMethods.ForEach(func => AddMethod(ClassMethods, func));
            if (!ClassMethods.ContainsKey("new")) {
                AddMethod(ClassMethods, new NovaFunction("new", new List<FunctionArgument>(),
                    NovaExpression.NovaBlock(
                        NovaExpression.Return(new List<FunctionArgument> {
                            new FunctionArgument(null, NovaExpression.Variable(Expression.Constant("self")))
                        }),
                        Expression.Label(NovaParser.ReturnTarget, Expression.Constant(null, typeof (object)))),
                    new NovaScope()));
            }
            InstanceMethods = new Dictionary<string, NovaMethodTable>();
            instanceMethods.ForEach(func => AddMethod(InstanceMethods, func));
            UndefinedMethods = new List<string>();
            RemovedMethods = new List<string>();
            Context = new NovaScope();
            Parent = parent;
        }

        internal NovaClass() {
            ClassMethods = new Dictionary<string, NovaMethodTable>();
            InstanceMethods = new Dictionary<string, NovaMethodTable>();
            UndefinedMethods = new List<string>();
            RemovedMethods = new List<string>();
            Context = new NovaScope();
        }

        public static NovaClass BoxClass(Type type) {
            Func<Type, bool> chkDoNotExport = t => {
                var a = t.GetCustomAttributes(typeof (NovaDoNotExportAttribute), false).FirstOrDefault();
                return a != null;
            };
            if (chkDoNotExport(type)) {
                return null;
            }

            if (TypeCache.ContainsKey(type)) {
                return TypeCache[type];
            }

            var @class = type.IsInterface ? new NovaInterface() : (NovaClass) new NovaBoxedClass();
            type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .ToList().
                ForEach(method => AddMethod(@class.ClassMethods, new NovaNativeFunction(type, method)));
            type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .ToList().
                ForEach(method => AddMethod(@class.InstanceMethods, new NovaNativeFunction(type, method)));
            type.GetConstructors()
                .ToList().
                ForEach(ctor => AddMethod(@class.ClassMethods, new NovaNativeFunction(type, ctor)));

            Func<Type, NovaClass> genBaseType = t => {
                if (t.BaseType != null) {
                    return TypeCache.ContainsKey(t.BaseType) ? TypeCache[t.BaseType] : BoxClass(t.BaseType);
                }
                return null;
            };

            Func<Type, string> getExportName = t => {
                var a = t.GetCustomAttributes(typeof (NovaExportAttribute), false).FirstOrDefault();
                return a != null ? ((NovaExportAttribute) a).Name : null;
            };

            if (@class is NovaBoxedClass) {
                ((NovaBoxedClass) @class).BoxedType = type;
            }

            @class.Name = getExportName(type) ?? type.Name;
            @class.Parent = genBaseType(type);
            TypeCache[type] = @class;
            return @class;
        }

        public static void AddMethod(IDictionary<string, NovaMethodTable> dict, NovaFunction func) {
            if (func.Name == "<__doNotExport>") {
                return;
            }
            if (!dict.ContainsKey(func.Name)) {
                dict[func.Name] = new NovaMethodTable(func.Name);
            }


            dict[func.Name].AddFunction(func);
        }

        public void Merge(NovaClass klass) {
            foreach (var key in klass.ClassMethods.Keys) {
                NovaMethodTable table;
                if (ClassMethods.ContainsKey(key)) {
                    table = ClassMethods[key];
                }
                else {
                    table = new NovaMethodTable(key);
                }
                foreach (var func in klass.ClassMethods[key].Functions) {
                    table.AddFunction(func);
                }
            }

            foreach (var key in klass.InstanceMethods.Keys) {
                NovaMethodTable table;
                if (InstanceMethods.ContainsKey(key)) {
                    table = InstanceMethods[key];
                }
                else {
                    table = new NovaMethodTable(key);
                }
                foreach (var func in klass.InstanceMethods[key].Functions) {
                    table.AddFunction(func);
                }
            }

            Context.MergeWithScope(klass.Context);
        }

        public override string ToString() {
            var builder = new StringBuilder("NovaClass: ");
            builder.Append(Name);
            builder.AppendLine(":");
            builder.AppendLine("  Class Methods:");
            foreach (var func in ClassMethods) {
                builder.AppendFormat("    {0}", func);
                builder.AppendLine();
            }
            builder.AppendLine("  Instance Methods:");
            foreach (var func in InstanceMethods) {
                builder.AppendFormat("    {0}", func);
                builder.AppendLine();
            }
            if (Parent != null) {
                builder.AppendLine("Parent: ");
                builder.Append(Parent);
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}