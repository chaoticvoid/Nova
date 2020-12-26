// -----------------------------------------------------------------------
// <copyright file="Forcs" Company="Michael Tindal">
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
using Nova.Parser;
using Nova.Runtime;

namespace Nova.Expressions {
    /// <summary>
    ///     Represents a standard for loop in Nova.
    /// </summary>
    public class ForExpression : NovaExpression {
        internal ForExpression(Expression init, Expression test, Expression step, Expression body) {
            Init = init;
            Test = test;
            Step = step;
            Body = body;
        }

        public Expression Init { get; private set; }

        public Expression Test { get; private set; }

        public Expression Step { get; private set; }

        public Expression Body { get; private set; }

        public override Type Type {
            get { return Body.Type; }
        }

        public override Expression Reduce() {
            var forLabel = Label("<nova_for>");
            VariableExpression forReturn = null;
            LeftHandValueExpression forReturnLh = null;
            var useReturn = true;
            if (Body.Type == typeof (void)) {
                useReturn = false;
            }
            else {
                forReturn = Variable(Constant("<nova_for_return>"));
                forReturnLh = LeftHandValue(forReturn);
                forReturn.Scope = ((NovaExpression) Body).Scope;
                forReturnLh.Scope = ((NovaExpression) Body).Scope;
            }
            var forTest = Variable(Constant("<nova_for_test>"));
            var forTestLh = LeftHandValue(forTest);
            forTest.Scope = ((NovaExpression) Body).Scope;
            forTestLh.Scope = ((NovaExpression) Body).Scope;
            var realBody = new List<Expression> {
                Init,
                Label(forLabel),
            };
            var testAssign = Assign(forTestLh, Test);
            realBody.Add(Label(NovaParser.RetryTarget));
            testAssign.Scope = (Body as NovaExpression).Scope;
            realBody.Add(testAssign);
            IfExpression testIf;
            if (useReturn) {
                var returnAssign = Assign(forReturnLh, Body);
                returnAssign.Scope = (Body as NovaExpression).Scope;
                testIf = IfThen(forTest, returnAssign);
            }
            else {
                testIf = IfThen(forTest, Body);
            }
            testIf.Scope = ((NovaExpression) Body).Scope;
            realBody.Add(testIf);
            realBody.Add(Label(NovaParser.ContinueTarget));
            realBody.Add(Step);
            realBody.Add(IfThen(forTest, Goto(forLabel)));
            realBody.Add(Label(NovaParser.BreakTarget));
            if (useReturn) {
                realBody.Add(forReturn);
            }

            var block = new BlockExpression(realBody) {Scope = (Body as NovaExpression).Scope};
            return Convert(block, Type);
        }

        public override void SetChildrenScopes(NovaScope scope) {
            Body.SetScope(scope);
            Init.SetScope(((NovaExpression) Body).Scope);
            Test.SetScope(((NovaExpression) Body).Scope);
            Step.SetScope(((NovaExpression) Body).Scope);
        }

        public override string ToString() {
            var str = new StringBuilder("for (");
            str.AppendFormat("{0}; ", Init);
            str.AppendFormat("{0}; ", Test);
            str.AppendFormat("{0})", Step);
            str.Append(Body);
            return str.ToString();
        }
    }
}