// -----------------------------------------------------------------------
// <copyright file="WhileExpression.cs" Company="Michael Tindal">
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
    ///     Represents a while expression for Nova.
    /// </summary>
    public class WhileExpression : NovaExpression {
        internal WhileExpression(Expression test, Expression body) {
            Test = test;
            Body = body;
        }

        public Expression Test { get; private set; }

        public Expression Body { get; private set; }

        public override Type Type {
            get { return Body.Type; }
        }

        public override Expression Reduce() {
            var whileLabel = Label("<nova_while>");
            ParameterExpression whileReturn = null;
            var useReturn = true;
            if (Body.Type == typeof (void)) {
                useReturn = false;
            }
            else {
                whileReturn = Variable(Body.Type, "<nova_while_return>");
            }
            var whileTest = Variable(typeof (bool), "<nova_while_test>");
            var realBody = new List<Expression> {
                Label(whileLabel),
                Label(NovaParser.ContinueTarget),
                Assign(whileTest, Boolean(Test)),
                Label(NovaParser.RetryTarget),
                useReturn
                    ? IfThen(whileTest, Assign(whileReturn, Body))
                    : IfThen(whileTest, Body),
                IfThen(whileTest, Goto(whileLabel)),
                Label(NovaParser.BreakTarget)
            };
            if (useReturn) {
                realBody.Add(Convert(whileReturn, Body.Type));

                return Block(new[] {whileTest, whileReturn}, realBody);
            }

            return Block(new[] {whileTest}, realBody);
        }

        public override void SetChildrenScopes(NovaScope scope) {
            Test.SetScope(scope);
            Body.SetScope(scope);
        }

        public override string ToString() {
            return "";
        }
    }
}