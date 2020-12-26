// -----------------------------------------------------------------------
// <copyright file="CompilerServices.cs" company="Michael Tindal">
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
using System.Reflection;
using Nova.Expressions;

namespace Nova.Runtime {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class CompilerServices {
        /// <summary>
        ///     Creates a callable dynamic block object from a given expression.
        /// </summary>
        /// <param name="body">The body of the block to turn into a callable block.</param>
        /// <returns>A callable block with return type and parameters collected by analyzing the given body.</returns>
        internal static dynamic CreateLambdaForExpression(Expression body) {
            var func = typeof (Func<object>).GetGenericTypeDefinition();
            if (body.Type == typeof (void)) {
                func = func.MakeGenericType(typeof (object));
            }
            else {
                func = func.MakeGenericType(body.Type);
            }
            var lambda = (from m in typeof (Expression).GetMethods()
                where m.Name == "Lambda"
                where m.GetParameters()[0].ParameterType == typeof (Expression)
                where m.GetParameters()[1].ParameterType == typeof (ParameterExpression[])
                select m).First();

            // Collect parameters here by analyzing the tree using a custom visitor pattern to see which
            // variables are assigned (without a set statement) or used in a block.
            lambda = lambda.MakeGenericMethod(func);

            Expression real_body = null;
            if (body.Type == typeof (void)) {
                var xbody = new List<Expression>();
                xbody.Add(body);
                xbody.Add(Expression.Constant(null, typeof (object)));
                real_body = Expression.Block(xbody);
            }
            else {
                real_body = body;
            }

            dynamic block =
                ((LambdaExpression)
                    lambda.Invoke(typeof (Expression), new object[] {real_body, new ParameterExpression[] {}})).Compile();
            return block;
        }

        internal static dynamic CompileExpression(Expression e, NovaScope scope) {
            Expression newExpression = NovaExpression.Convert(e, typeof (object));
            newExpression.SetScope(scope);
            var l = CreateLambdaForExpression(newExpression);
            return l();
        }
    }
}