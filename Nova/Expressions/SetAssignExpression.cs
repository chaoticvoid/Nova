// -----------------------------------------------------------------------
// <copyright file="NovaBinaryExpression.cs" company="Michael Tindal">
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
    public class SetAssignExpression : NovaExpression {
        internal SetAssignExpression(Expression left, Expression right, ExpressionType type) {
            Left = left;
            Right = right;
            SetNodeType = type;
        }

        /// <summary>
        ///     The left side of the comparison.
        /// </summary>
        public Expression Left { get; private set; }

        /// <summary>
        ///     The right side of the comparison.
        /// </summary>
        public Expression Right { get; private set; }

        protected ExpressionType SetNodeType { get; private set; }

        public override Type Type {
            get { return Right.Type; }
        }

        /// <summary>
        ///     Reduces this expression to base DLR variables.
        /// </summary>
        /// <returns></returns>
        public override Expression Reduce() {
            var expr = new AssignmentExpression(Left as LeftHandValueExpression, Right, SetNodeType);

            expr.SetScope(Scope.ParentScope);
            expr.Right.SetScope(Scope);

            return expr;
        }

        /// <summary>
        ///     Converts this expression to a string representing its contents.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() {
            return "set " + (new AssignmentExpression(Left as LeftHandValueExpression, Right, SetNodeType));
        }

        /// <summary>
        ///     Visits the children of this expression.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        /// <returns>A new expression with the modified children.</returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor) {
            Left = visitor.Visit(Left);
            Right = visitor.Visit(Right);
            return new SetAssignExpression(Left, Right, SetNodeType);
        }

        public override void SetChildrenScopes(NovaScope scope) {
            Left.SetScope(scope);
            Right.SetScope(scope);
        }
    }
}