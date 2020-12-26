// -----------------------------------------------------------------------
// <copyright file="VariableExpression.cs" Company="Michael Tindal">
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
    public class VariableExpression : NovaExpression {
        internal VariableExpression(Expression name) {
            Name = name;
        }

        internal VariableExpression(Symbol sym) {
            HasSym = true;
            Sym = sym;
        }

        public Expression Name { get; private set; }

        public Symbol Sym { get; private set; }

        internal bool HasSym { get; set; }

        public override Type Type {
            get { return typeof (object); }
        }

        public override Expression Reduce() {
            return HasSym
                ? Operation.ResolveSymbol(typeof (object), Constant(Sym), Constant(Scope))
                : Operation.Resolve(typeof (object), Name, Constant(Scope));
        }

        public override void SetChildrenScopes(NovaScope scope) {
            Name.SetScope(scope);
        }

        public override string ToString() {
            return Name.ToString();
        }
    }
}