// -----------------------------------------------------------------------
// <copyright file="UnaryExpression.cs" Company="Michael Tindal">
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
using System.Linq.Expressions;
using Nova.Runtime;

namespace Nova.Expressions {
    /// <summary>
    ///     Represents a unary operator expression in Nova.
    /// </summary>
    public class UnaryExpression : NovaExpression {
        internal UnaryExpression(Expression expr, ExpressionType type) {
            Expression = expr;
            UnaryNodeType = type;
        }

        public Expression Expression { get; private set; }

        public ExpressionType UnaryNodeType { get; private set; }

        public override Type Type {
            get { return UnaryNodeType == ExpressionType.Not ? typeof (bool) : typeof (object); }
        }

        private static Expression Convert<T>(Expression e) {
            return Convert(e, typeof (T));
        }

        public override Expression Reduce() {
            return Operation.Unary(Type, Convert<object>(Expression), Constant(UnaryNodeType), Constant(Scope));
        }

        public override void SetChildrenScopes(NovaScope scope) {
            Expression.SetScope(scope);
        }

        public override string ToString() {
            switch (UnaryNodeType) {
                case ExpressionType.OnesComplement:
                    return string.Format("~{0}", Expression);
                case ExpressionType.Not:
                    return string.Format("!{0}", Expression);
                default:
                    return MakeUnary(UnaryNodeType, Expression, Type).ToString();
            }
        }
    }
}