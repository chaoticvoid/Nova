// -----------------------------------------------------------------------
// <copyright file="IncludeExpression.cs" Company="Michael Tindal">
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
using System.Text;
using Nova.Runtime;

namespace Nova.Expressions {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class IncludeExpression : NovaExpression {
        internal IncludeExpression(List<string> names) {
            Names = names;
        }

        public List<string> Names { get; private set; }

        public override Type Type {
            get { return typeof (object); }
        }

        public override Expression Reduce() {
            return Operation.Include(typeof (object), Constant(Names));
        }

        public override string ToString() {
            var str = new StringBuilder("include ");
            Names.ForEach(name => str.AppendFormat("{0}::", name));
            str.Remove(str.Length - 2, 2);
            return str.ToString();
        }
    }
}