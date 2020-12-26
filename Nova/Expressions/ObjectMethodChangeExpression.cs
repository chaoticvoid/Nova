// -----------------------------------------------------------------------
// <copyright file="ObjectMethodChangeExpression.cs" Company="Michael Tindal">
// Copyright 2011-2014 Michael Tindal
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

namespace Nova.Expressions
{
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class ObjectMethodChangeExpression : NovaExpression
    {
        internal ObjectMethodChangeExpression(Expression lvalue, string varName, bool isRemove)
        {
            LValue = lvalue;
            VarName = varName;
            IsRemove = isRemove;
        }

        public Expression LValue { get; private set; }
        public bool IsRemove { get; private set; }

        public string VarName { get; private set; }

        public override Type Type
        {
            get { return typeof(object); }
        }

        // Should not actually reduce, used by the runtime directly
        public override Expression Reduce()
        {
            return Operation.ObjectMethodChange(Type, Constant(LValue), Constant(VarName), Constant(IsRemove), Constant(Scope));
        }

        public override void SetChildrenScopes(NovaScope scope)
        {
            LValue.SetScope(scope);
        }

        public override string ToString()
        {
            return "";
        }
    }
}