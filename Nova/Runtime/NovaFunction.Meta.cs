// -----------------------------------------------------------------------

using System.Dynamic;
using System.Linq.Expressions;

// <copyright file="NovaFunction.Meta.cs" Company="Michael Tindal">
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
    public partial class NovaFunction : INovaDynamicMetaObjectProvider {
        private NovaScope _scope;

        #region IScopeExpression implementation

        public void SetScope(NovaScope scope) {
            _scope = scope;
        }

        public NovaScope Scope {
            get { return _scope; }
        }

        #endregion

        public DynamicMetaObject /*!*/ GetMetaObject(Expression /*!*/ parameter) {
            var m = new Meta(parameter, BindingRestrictions.Empty, this);
            m.SetScope(Scope);
            return m;
        }

        internal sealed class Meta : NovaMetaObject<NovaFunction> {
            public Meta(Expression expression, BindingRestrictions restrictions, NovaFunction value)
                : base(expression, restrictions, value) {}
        }
    }
}