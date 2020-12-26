// -----------------------------------------------------------------------
// <copyright file="FunctionCallExpression.cs" Company="Michael Tindal">
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
    public class FunctionCallExpression : NovaExpression {
        internal FunctionCallExpression(Expression func, List<FunctionArgument> arguments, NovaExpressionType pipeType) {
            Function = func;
            Arguments = arguments;
            PipeType = pipeType;
        }

        internal FunctionCallExpression(Expression func, List<FunctionArgument> arguments)
            : this(func, arguments, NovaExpressionType.Empty) {
        }

        internal FunctionCallExpression(Expression func, List<FunctionArgument> arguments, bool isUnaryOp,
            bool isPostfix) : this(func, arguments) {
            IsOp = isUnaryOp;
            IsPostfix = isPostfix;
        }

        public NovaExpressionType PipeType { get; private set; }

        public Expression Function { get; private set; }

        public bool IsPostfix { get; private set; }

        public bool IsOp { get; private set; }

        public List<FunctionArgument> Arguments { get; private set; }

        public override Type Type {
            get { return typeof (object); }
        }

        public override Expression Reduce() {
            if (Function is VariableExpression && ((VariableExpression) Function).Name is InstanceReferenceExpression) {
                return Operation.Call(typeof (object), ((VariableExpression) Function).Name, Constant(Arguments),
                    Constant(Scope), Constant(PipeType), Constant(IsOp), Constant(IsPostfix));
            }
            return Operation.Call(typeof (object), Function, Constant(Arguments), Constant(Scope), Constant(PipeType), Constant(IsOp), Constant(IsPostfix));
        }

        public override void SetChildrenScopes(NovaScope scope) {
            Function.SetScope(scope);
        }

        public override string ToString() {
            return string.Format("[FunctionCallExpression Function: {0} Arguments: {1}", Function, Arguments);
        }
    }
}