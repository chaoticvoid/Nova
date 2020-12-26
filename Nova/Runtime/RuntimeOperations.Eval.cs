// -----------------------------------------------------------------------
// <copyright file="RuntimeOperations.Eval.cs" Company="Michael Tindal">
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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Nova.Expressions;
using Nova.Builtins;
using Nova.Parser;
using NovaLexer = Nova.Lexer.NovaLexer;

namespace Nova.Runtime {
    using E = ExpressionType;

    /// <summary>
    ///     This class provides the operations Nova needs to operate.  It houses the methods that makes up the IS
    ///     runtime, which works in conjunction with the DLR runtime.
    /// </summary>
    public static partial class RuntimeOperations {
        internal static dynamic String(object rawEval, object rawScope) {
            StringBuilder @new;
            var eval = rawEval as String;

            var components = eval.Split(new[] {"#{"}, StringSplitOptions.None);
            if (components.Count() == 1) {
                return new NovaString(eval);
            }
            @new = new StringBuilder(components[0]);
            for (var i = 1; i < components.Count(); i++) {
                var parts = components[i].Split(new[] {"}"}, StringSplitOptions.None);
                var expression = parts[0];
                var escapeString = false;
                if (expression != null && expression[0] == ':') {
                    escapeString = true;
                    expression = expression.Substring(1);
                }
                if (expression != null) {
                    var scope = (NovaScope) rawScope;
                    var xexpression = string.Format("{0};", expression);

                    var res = NovaParser.Parse(xexpression);
                    Expression block;
                    if (res != null) {
                        block = NovaExpression.NovaBlock(res);
                    }
                    else {
                        return null;
                    }
                    var myScope = new NovaScope();
                    var visitor = new VariableNameVisitor();
                    visitor.Visit(block);
                    visitor.VariableNames.ForEach(name => myScope[name] = scope[name]);
                    var val = CompilerServices.CompileExpression(block, myScope);
                    if (val != null) {
                        string stringVal = val.ToString();
                        if (escapeString && val is string) {
                            stringVal = string.Format("\"{0}\"", stringVal);
                        }
                        @new.Append(stringVal ?? "");
                    }
                    else {
                        @new.Append(expression);
                    }
                }
                if (parts.Count() > 1) {
                    @new.Append(parts[1]);
                    var j = 2;
                    while (j < parts.Count()) {
                        @new.Append("}");
                        @new.Append(parts[j++]);
                    }
                }
            }

            return new NovaString(@new.ToString());
        }

        internal static dynamic Regex(object rawValue)
        {
            var value = (string) rawValue;

            return new Regex(value);
        }

        internal static dynamic Number(object rawValue)
        {
            return new NovaNumber(rawValue);
        }
    }
}