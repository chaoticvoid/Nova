// -----------------------------------------------------------------------
// <copyright file="InvokeExpression.cs" Company="Michael Tindal">
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
using System.Linq.Expressions;
using Nova.Runtime;

namespace Nova.Expressions {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class InvokeExpression : NovaExpression {
        internal InvokeExpression(Expression type, Expression method, List<FunctionArgument> arguments) {
            TargetType = type;
            Method = method;
            Arguments = arguments;
        }

        public Expression TargetType { get; private set; }

        public Expression Method { get; private set; }

        public List<FunctionArgument> Arguments { get; private set; }

        public override Type Type {
            get { return typeof (object); }
        }

        public override Expression Reduce() {
            var args = new List<FunctionArgument>();
            Arguments.ForEach(arg => args.Add(new FunctionArgument(arg.Name, Variable(Constant(arg.Name)))));
            return Operation.Invoke(typeof (object), TargetType, Method, Constant(args), Constant(Scope));
        }

        public override void SetChildrenScopes(NovaScope scope) {
            TargetType.SetScope(scope);
            Method.SetScope(scope);
        }

        public override string ToString() {
            return string.Format("[InvokeExpression: TargetType={0}, Method={1}, Arguments={2}, Type={3}]", TargetType,
                Method, Arguments, Type);
        }
    }
}