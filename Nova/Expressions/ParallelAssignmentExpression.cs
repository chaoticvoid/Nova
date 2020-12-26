// -----------------------------------------------------------------------
// <copyright file="ParallelAssignmentExpression.cs" Company="Michael Tindal">
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
    using CS = CompilerServices;

    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class ParallelAssignmentExpression : NovaExpression {
        internal ParallelAssignmentExpression(List<ParallelAssignmentInfo> lvalues, List<ParallelAssignmentInfo> rvalues) {
            LeftHandValues = lvalues;
            RightHandValues = rvalues;
        }

        public List<ParallelAssignmentInfo> LeftHandValues { get; private set; }
        public List<ParallelAssignmentInfo> RightHandValues { get; private set; }

        public override Type Type {
            get { return typeof (object); }
        }

        public override Expression Reduce() {
            return Operation.ParallelAssign(typeof (object), Constant(LeftHandValues), Constant(RightHandValues),
                Constant(Scope));
        }

        public override void SetChildrenScopes(NovaScope scope) {
            LeftHandValues.ForEach(value => {
                if (value.Value is NovaExpression) {
                    value.Value.SetScope(scope);
                }
            });
            RightHandValues.ForEach(value => {
                if (value.Value is NovaExpression) {
                    value.Value.SetScope(scope);
                }
            });
        }

        public override string ToString() {
            return "";
        }

        /// <summary>
        ///     Used to represent lvalues and rvalues in a parallel assignment statement.
        /// </summary>
        public struct ParallelAssignmentInfo {
            public bool IsWildcard;
            public dynamic Value;
        }
    }
}