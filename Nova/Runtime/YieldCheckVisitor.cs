// -----------------------------------------------------------------------
// <copyright file="YieldCheckVisitor.cs" company="Michael Tindal">
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
using Nova.Expressions;

namespace Nova.Runtime {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class YieldCheckVisitor : NovaExpressionVisitor {
        private bool _hasYield;

        /// <summary>
        ///     Initializes a new instance of the {YieldCheckVisitor} class.
        /// </summary>
        public YieldCheckVisitor() {
            _hasYield = false;
        }

        /// <summary>
        ///     Gets the paramter variables this visitor collects from the tree.
        /// </summary>
        public bool HasYield {
            get { return _hasYield; }
        }

        protected override Expression VisitYield(YieldExpression node) {
            _hasYield = true;
            return node;
        }
    }
}