// -----------------------------------------------------------------------
// <copyright file="InstanceReferenceExpression.cs" Company="Michael Tindal">
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
    ///     TODO: Update summary.
    /// </summary>
    public class InstanceReferenceExpression : VariableExpression {
        public InstanceReferenceExpression(Expression lvalue, Expression key) : base(Constant("")) {
            LValue = lvalue;
            Key = key;
        }

        public Expression LValue { get; private set; }

        public Expression Key { get; private set; }

        public override Type Type {
            get { return typeof (InstanceReference); }
        }

        public override string ToString() {
            return string.Format("{0}.{1}", LValue, Key);
        }

        public override Expression Reduce() {
            return Operation.InstanceRef(typeof (InstanceReference), Constant(LValue), Key);
        }

        public override void SetChildrenScopes(NovaScope scope) {
            LValue.SetScope(scope);
            Key.SetScope(scope);
        }
    }
}