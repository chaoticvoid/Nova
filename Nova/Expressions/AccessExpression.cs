// -----------------------------------------------------------------------
// <copyright file="AccessExpression.cs" Company="Michael Tindal">
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
using System.Text;
using Nova.Runtime;

namespace Nova.Expressions {
    /// <summary>
    ///     Represents an index style access, until [] and []= method calls are implemented.
    /// </summary>
    public class AccessExpression : NovaExpression {
        internal AccessExpression(Expression container, List<FunctionArgument> arguments) {
            Container = container;
            Arguments = arguments;
        }

        /// <summary>
        ///     Gets the container used in this expression.
        /// </summary>
        /// <value>The container.</value>
        public Expression Container { get; private set; }

        /// <summary>
        ///     Gets the arguments used for this expression.
        /// </summary>
        /// <value>The arguments.</value>
        public List<FunctionArgument> Arguments { get; private set; }

        /// <summary>
        ///     The type returned by this expression.
        /// </summary>
        /// <value>The type.</value>
        public override Type Type {
            get { return typeof (object); }
        }

        /// <summary>
        ///     Reduces this expression into a dynamic expression using <c><see cref="Nova.Runtime.Operation" />.Access</c>
        /// </summary>
        /// <returns>The reduced expression</returns>
        public override Expression Reduce() {
            return Operation.Access(typeof (object), Container, Constant(Arguments), Constant(Scope));
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents the current <see cref="Nova.Expressions.AccessExpression" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="Nova.Expressions.AccessExpression" />.</returns>
        public override string ToString() {
            var str = new StringBuilder();
            str.AppendFormat("{0}[", Container);
            Arguments.ForEach(arg => str.AppendFormat("{0},", arg));

            str.Remove(str.Length - 1, 1);
            str.Append("]");
            return str.ToString();
        }

        /// <summary>
        ///     Called by SetScope to set the children scope on expressions. Should be overridden to tell the runtime which
        ///     children should have scopes set.
        /// </summary>
        /// <param name="scope"></param>
        public override void SetChildrenScopes(NovaScope scope) {
            Container.SetScope(scope);
            foreach (var arg in Arguments) {
                arg.Value.SetScope(scope);
            }
        }
    }
}