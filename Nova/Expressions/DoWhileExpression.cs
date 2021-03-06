﻿// -----------------------------------------------------------------------
// <copyright file="DoWhilecs" Company="Michael Tindal">
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
using Nova.Parser;

namespace Nova.Expressions {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class DoWhileExpression : WhileExpression {
        internal DoWhileExpression(Expression test, Expression body) : base(test, body) {}

        public override Expression Reduce() {
            var whileLabel = Label("<nova_do_while>");
            ParameterExpression whileReturn = null;
            var useReturn = true;
            if (Body.Type == typeof (void)) {
                useReturn = false;
            }
            else {
                whileReturn = Variable(Body.Type, "<nova_do_while_return>");
            }
            var whileTest = Variable(typeof (bool), "<nova_do_while_test>");
            var realBody = new List<Expression> {
                Label(whileLabel),
                Label(NovaParser.RetryTarget),
                useReturn ? Assign(whileReturn, Body) : Body,
                Label(NovaParser.ContinueTarget),
                Assign(whileTest, Boolean(Test)),
                IfThen(whileTest, Goto(whileLabel)),
                Label(NovaParser.BreakTarget)
            };

            if (useReturn) {
                realBody.Add(whileReturn);

                return Block(new[] {
                    whileTest,
                    whileReturn
                }, realBody);
            }

            return Block(new[] {whileTest}, realBody);
        }
    }
}