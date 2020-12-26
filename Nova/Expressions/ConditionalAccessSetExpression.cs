// -----------------------------------------------------------------------
// <copyright file="AccessSetExpression.cs" Company="Michael Tindal">
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
using System.Linq.Expressions;
using Nova.Runtime;

namespace Nova.Expressions {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class ConditionalAccessSetExpression : AccessSetExpression {
        internal ConditionalAccessSetExpression(Expression container, List<FunctionArgument> arguments, Expression value,
            NovaExpressionType conditionalAssignmentType)
            : base(container, arguments, value, ExpressionType.Assign)
        {
            ConditionalAssignmentType = conditionalAssignmentType;
        }

        public NovaExpressionType ConditionalAssignmentType { get; }

        public override Expression Reduce() {
            return Operation.ConditionalAccessSet(Type, Container, Constant(Arguments), Convert(Value, typeof (object)),
                Constant(ConditionalAssignmentType), Constant(Scope));
        }
    }
}