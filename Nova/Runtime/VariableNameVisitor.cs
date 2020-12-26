// -----------------------------------------------------------------------
// <copyright file="VariableNameVisitor.cs" company="Michael Tindal">
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
using Nova.Expressions;

namespace Nova.Runtime {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class VariableNameVisitor : NovaExpressionVisitor {
        private readonly List<string> _variableNames = new List<string>();

        /// <summary>
        ///     Initializes a new instance of the {CollectParameterVisitor} class.
        /// </summary>
        public VariableNameVisitor() {
            _variableNames = new List<string>();
        }

        /// <summary>
        ///     Gets the paramter variables this visitor collects from the tree.
        /// </summary>
        public List<string> VariableNames {
            get { return _variableNames; }
        }

        protected override Expression VisitVariable(VariableExpression node) {
            var name = CompilerServices.CreateLambdaForExpression(node.Name)();
            if (name is string) {
                _variableNames.Add(name);
            }
            return node;
        }
    }
}