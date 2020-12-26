// -----------------------------------------------------------------------
// <copyright file="AssignmentExpression.cs" Company="Michael Tindal">
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

using System.Linq.Expressions;
using Nova.Runtime;

namespace Nova.Expressions {
    using CS = CompilerServices;

    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class ConditionalAssignmentExpression : AssignmentExpression {
        internal ConditionalAssignmentExpression(LeftHandValueExpression left, Expression right, NovaExpressionType conditionalAssignmentType)
            : base(left, right, ExpressionType.Assign)
        {
            ConditionalAssignmentType = conditionalAssignmentType;
        }

        public NovaExpressionType ConditionalAssignmentType { get; }

        public override Expression Reduce() {
            var rl = Left.Reduce();
            if (rl is VariableExpression) {
                return Operation.ConditionalAssign(Right.Type, Constant(rl), Convert(Right, typeof (object)),
                    Constant(ConditionalAssignmentType), Constant(IsConst), Constant(Scope));
            }
            return
                Convert(
                    ConditionalAccessSet((rl as AccessExpression).Container,
                        (rl as AccessExpression).Arguments, Right, ConditionalAssignmentType), Type);
        }

        public override string ToString()
        {
            var opStr = "||=";
            if (ConditionalAssignmentType == NovaExpressionType.IfNotNullAssign)
            {
                opStr = "&&=";
            }
            return string.Format("{0} {1} {2}", Left, opStr, Right);
        }
    }
}