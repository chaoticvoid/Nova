// -----------------------------------------------------------------------
// <copyright file="SwitchExpression.cs" Company="Michael Tindal">
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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nova.Runtime;

namespace Nova.Expressions
{
    /// <summary>
    ///     Represents a switch statement for Nova.
    /// </summary>
    public class SwitchExpression : NovaExpression
    {
        internal SwitchExpression(Expression test, Expression @default, List<SwitchCase> cases)
        {
            Test = test;
            DefaultBody = @default;
            Cases = cases;
        }

        public Expression Test { get; private set; }

        public Expression DefaultBody { get; private set; }

        public List<SwitchCase> Cases { get; private set; }


        public override Type Type
        {
            get { return Cases.First().Body.Type; }
        }

        public override Expression Reduce()
        {
            return Operation.Switch(Type, Constant(Test), Constant(Cases), Constant(DefaultBody), Constant(Scope));
        }

        public override void SetChildrenScopes(NovaScope scope)
        {
            Test.SetScope(scope);
            if (DefaultBody != null)
            {
                DefaultBody.SetScope(scope);
            }
            foreach (var @case in Cases)
            {
                foreach (var test in @case.TestValues)
                {
                    test.SetScope(scope);
                }
                @case.Body.SetScope(scope);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("switch({0}) ", Test);
            sb.Append("{");
            sb.AppendLine();
            foreach (var @case in Cases)
            {
                sb.AppendFormat("  case {0}:", @case.TestValues[0]);
                sb.AppendLine();
                sb.AppendFormat("{0}", @case.Body);
                sb.AppendLine();
                sb.AppendLine("    break;");
            }
            if (DefaultBody != null)
            {
                sb.AppendLine("  default:");
                sb.AppendFormat("{0}", DefaultBody);
                sb.AppendLine("    break;");
            }
            sb.AppendLine("};");
            return sb.ToString();
        }
    }
}