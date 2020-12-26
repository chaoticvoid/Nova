// -----------------------------------------------------------------------
// <copyright file="NovaScriptCode.cs" company="Michael Tindal">
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
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Schema;
using Nova.Expressions;
using Nova.Builtins;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using BlockExpression = Nova.Expressions.BlockExpression;

namespace Nova.Runtime {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class NovaScriptCode : ScriptCode {
        public NovaScriptCode(Expression body, SourceUnit sourceUnit) : base(sourceUnit) {
            Body = body;
        }

        /// <summary>
        ///     Returns the body associated with this script code.
        /// </summary>
        public Expression Body { get; private set; }

        private dynamic ConvertElements(NovaArray res)
        {
            for (var i = 0; i < res.Count(); i++)
            {
                if (res[i] is NovaString)
                {
                    res[i] = (string)res[i];
                }
                if (res[i] is NovaNumber)
                {
                    res[i] = NovaNumber.Convert(res[i]);
                }
                if (res[i] is NovaArray)
                {
                    res[i] = ConvertElements((NovaArray)res[i]);
                }
                if (res[i] is NovaDictionary)
                {
                    res[i] = ConvertElements((NovaDictionary)res[i]);
                }
            }
            return res;
        }

        private dynamic ConvertElements(NovaDictionary res)
        {
            List<dynamic> keysToRemove = new List<object>();
            keysToRemove.AddRange(res.Keys.OfType<NovaString>());
            keysToRemove.ForEach(o =>
            {
                string s = o;
                var val = res[o];
                res.Remove(o);
                res[s] = val;
            });

            keysToRemove.Clear();

            keysToRemove.AddRange(
                res.Keys.Where(
                    key =>
                        res[key] is NovaString || res[key] is NovaNumber || res[key] is NovaArray ||
                        res[key] is NovaDictionary));

            keysToRemove.ForEach(o =>
            {
                if (res[o] is NovaString)
                {
                    string s = res[o];
                    res[o] = s;
                }
                else if (res[o] is NovaNumber)
                {
                    res[o] = NovaNumber.Convert(res[o]);
                }
                else if (res[o] is NovaArray)
                {
                    res[o] = ConvertElements((NovaArray) res[o]);
                }
                else if (res[o] is NovaDictionary)
                {
                    res[o] = ConvertElements((NovaDictionary) res[o]);
                }
            });

            return res;
        }

        public override object Run(Scope scope) {
            var body = (Body as BlockExpression);
            body.Scope.MergeWithScope(Nova.Globals);
            body.Scope.MergeWithScope(scope);

            var visitor = new VariableNameVisitor();
            visitor.Visit(body);

            body.SetChildrenScopes(body.Scope);

            var block = CompilerServices.CreateLambdaForExpression(body);
            var res = block();

            if (res is Symbol) {
                var symval = new BlockExpression(new List<Expression> {new VariableExpression(res)}, body.Scope);
                res = CompilerServices.CreateLambdaForExpression(symval)();
            }
            else if (res is NovaInstance) {
                var so = (NovaInstance) res;
                if (so is NovaBoxedInstance) {
                    res = ((NovaBoxedInstance) so).BoxedObject;
                }
            }
            else if (res is NovaNumber)
            {
                res = NovaNumber.Convert(res);
            }
            else if (res is NovaString) {
                res = (string) res;
            }
            else if (res is NovaArray) {
                res = ConvertElements((NovaArray)res);
            }
            else if (res is NovaDictionary) {
                res = ConvertElements((NovaDictionary)res);
            }

            body.Scope.MergeIntoScope(scope);

            return res;
        }

        internal object Run(NovaScope scope) {
            var body = (BlockExpression) Body;

            body.SetScope(scope);

            body.SetChildrenScopes(body.Scope);

            var block = CompilerServices.CreateLambdaForExpression(Expression.Block(body));

            var res = block();

            if (res is Symbol) {
                var symval = new BlockExpression(new List<Expression> {new VariableExpression(res)}, body.Scope);
                res = CompilerServices.CreateLambdaForExpression(symval)();
            }

            return res;
        }
    }
}