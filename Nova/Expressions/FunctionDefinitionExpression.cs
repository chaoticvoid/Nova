// -----------------------------------------------------------------------
// <copyright file="FunctionDefinitionExpression.cs" Company="Michael Tindal">
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
using Nova.Parser;
using Nova.Runtime;

namespace Nova.Expressions {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class FunctionDefinitionExpression : NovaExpression {
        internal FunctionDefinitionExpression(string name, List<FunctionArgument> arguments, Expression body) {
            Name = name;
            Arguments = arguments;
            Body = body;
        }

        public string Name { get; private set; }

        public List<FunctionArgument> Arguments { get; private set; }

        public Expression Body { get; private set; }

        public override Type Type {
            get { return typeof (NovaFunction); }
        }

        public override Expression Reduce() {
            var ci = 0;
            Arguments.ForEach(arg => arg.Index = ci++);
            var realBody = new List<Expression>(((BlockExpression) Body).Body);
            if (Name == "new") {
                realBody.Add(Return(new List<FunctionArgument> {new FunctionArgument(null, Variable(Constant("self")))}));
            }
            realBody.Add(Label(NovaParser.ReturnTarget, Constant(null, typeof (object))));

            return Operation.Define(typeof (NovaFunction), Constant(Name),
                Constant(Arguments),
                Constant(NovaParser.CreateBlock(realBody)),
                Constant(Scope));
        }

        public override void SetChildrenScopes(NovaScope scope) {
            Body.SetScope(scope);
        }

        public override string ToString() {
            return string.Format("[FunctionDefinitionExpression: Name={0}, Arguments={1}, Block={2}, Type={3}]", Name,
                Arguments, Body, Type);
        }
    }
}