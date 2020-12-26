// -----------------------------------------------------------------------
// <copyright file="NovaClass.Meta.cs" Company="Michael Tindal">
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

using System.Dynamic;
using System.Linq.Expressions;

namespace Nova.Runtime {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public partial class NovaInterface : INovaDynamicMetaObjectProvider {
        private NovaScope _scope;

        #region IScopeExpression implementation

        public new void SetScope(NovaScope scope) {
            _scope = scope;
        }

        public new NovaScope Scope {
            get { return _scope; }
        }

        #endregion

        public new DynamicMetaObject GetMetaObject(Expression /*!*/ parameter) {
            var m = new Meta(parameter, BindingRestrictions.Empty, this);
            m.SetScope(Scope);
            return m;
        }

        internal new sealed class Meta : NovaMetaObject<NovaInterface> {
            public Meta(Expression expression, BindingRestrictions restrictions, NovaInterface value)
                : base(expression, restrictions, value) {}

            public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args) {
                return new DynamicMetaObject(Expression.Constant(null),
                    BindingRestrictions.GetExpressionRestriction(Expression.Constant(true)));
            }

            public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args) {
                return new DynamicMetaObject(Expression.Constant(null),
                    BindingRestrictions.GetExpressionRestriction(Expression.Constant(true)));
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder,
                params DynamicMetaObject[] args) {
                return new DynamicMetaObject(Expression.Constant(null),
                    BindingRestrictions.GetExpressionRestriction(Expression.Constant(true)));
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder) {
                return new DynamicMetaObject(Expression.Constant(null),
                    BindingRestrictions.GetExpressionRestriction(Expression.Constant(true)));
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value) {
                return new DynamicMetaObject(Expression.Constant(null),
                    BindingRestrictions.GetExpressionRestriction(Expression.Constant(true)));
            }
        }
    }
}