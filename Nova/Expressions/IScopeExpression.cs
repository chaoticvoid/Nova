// -----------------------------------------------------------------------
// <copyright file="IScopeExpression.cs" Company="Michael Tindal">
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
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public interface IScopeExpression {
        NovaScope Scope { get; }

        void SetScope(NovaScope scope);
    }

    public static class ScopeExpressionExtension {
        public static void SetScope(this Expression expr, NovaScope scope) {
            expr.SetParentScopeIfBlock(scope);
        }

        private static void SetParentScope(this Expression expr, NovaScope scope)
        {
            if (!(expr is BlockExpression block)) return;
            if (block.Scope == null)
            {
                block.Scope = new NovaScope();
            }
            block.Scope.ParentScope = scope;
        }

        private static void SetParentScopeIfBlock(this Expression expr, NovaScope scope) {
            if (expr is BlockExpression) {
                expr.SetParentScope(scope);
            }
            else if (expr is IScopeExpression) {
                (expr as IScopeExpression).SetScope(scope);
            }
        }
    }
}