// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nova.Expressions;
using Nova.Parser;
using BlockExpression = Nova.Expressions.BlockExpression;

// <copyright file="NovaPartialFunction.cs" Company="Michael Tindal">
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

namespace Nova.Runtime {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public partial class NovaNativeFunction : NovaFunction {
        private static readonly Dictionary<MethodBase, List<FunctionArgument>> ArgumentCache =
            new Dictionary<MethodBase, List<FunctionArgument>>();

        public NovaNativeFunction(Type target, MethodBase method)
            : base(
                GetExportName(method) ?? (method.IsConstructor ? "new" : method.Name), GenerateArguments(method),
                GenerateBody(target, method), new NovaScope()) {
            Func<MethodBase, bool> chkDoNotExportMethod = t => {
                var a = t.GetCustomAttributes(typeof (NovaDoNotExportAttribute), false).FirstOrDefault();
                return a != null;
            };
            if (chkDoNotExportMethod(method)) {
                Name = "<__doNotExport>";
                return;
            }
            Target = target;
            Method = method;
            NumberOfArguments = method.GetParameters().Count();
        }

        public Type Target { get; private set; }

        public MethodBase Method { get; private set; }

        public int NumberOfArguments { get; private set; }

        public static List<FunctionArgument> GenerateArguments(MethodBase method) {
            if (ArgumentCache.ContainsKey(method)) {
                return ArgumentCache[method];
            }
            var args = new List<FunctionArgument>();
            method.GetParameters().ToList().ForEach(p => {
                var arg = new FunctionArgument(p.Name);
                if (p.GetCustomAttributes(typeof (ParamArrayAttribute), false).Any()) {
                    arg.IsVarArg = true;
                }
                if (p.DefaultValue != null && p.DefaultValue.GetType() != typeof (DBNull)) {
                    arg.HasDefault = true;
                    arg.DefaultValue = Expression.Constant(p.DefaultValue);
                }
                args.Add(arg);
            });
            ArgumentCache[method] = args;
            return args;
        }

        public static BlockExpression GenerateBody(Type type, MethodBase method) {
            var body = new List<Expression>();
            body.Add(
                NovaExpression.Invoke(
                    Expression.Constant(method.IsConstructor ? typeof (NovaInstance) : type, typeof (Type)),
                    Expression.Constant(method, typeof (MethodBase)), ArgumentCache[method]));
            body.Add(Expression.Label(NovaParser.ReturnTarget, Expression.Constant(null, typeof (object))));
            return NovaExpression.NovaBlock(body.ToArray());
        }

        public static string GetExportName(MethodBase t) {
            var a = t.GetCustomAttributes(typeof (NovaExportAttribute), false).FirstOrDefault();
            return a != null ? ((NovaExportAttribute) a).Name : null;
        }

        public override string ToString() {
            return string.Format("[NovaNativeFunction: TargetType={0}, Scope={1}]", Target, Scope);
        }
    }
}