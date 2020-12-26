//
//  Copyright 2013  Michael Tindal
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Nova.Expressions;
using Nova.Parser;
using Nova.Runtime;
using BlockExpression = Nova.Expressions.BlockExpression;
using NovaLexer = Nova.Lexer.NovaLexer;

namespace Nova.Builtins {
    /// <summary>
    ///     The Kernel class provides the runtime methods every Nova instance needs.  It corresponds to core.sv in default
    ///     Nova runtime.
    /// </summary>
    [NovaDoNotExport]
    public class Kernel {
        public Kernel() {
            NovaNativeFunction nfunc;

            GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic).ToList()
                .ForEach(method => {
                    nfunc = new NovaNativeFunction(GetType(), method);
                    Nova.Globals.SetVariable(nfunc.Name, nfunc);
                });
            // Register wrapper for functions which need it, like eval
            var stream = typeof (Kernel).Assembly.GetManifestResourceStream("Nova.Scripts.core.nova");
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int) stream.Length);
            var source = Encoding.UTF8.GetString(buffer);

            Nova.Execute(source);
        }

        [NovaExport("_eval")]
        private static dynamic Eval(string eval, NovaScope scope)
        {
            var xexpression = string.Format("{0};", eval);

            var res = NovaParser.Parse(xexpression);
            Expression block;
            if (res != null) {
                block = NovaExpression.NovaBlock(res);
                // We want eval'd expressions to execute in the current scope, not its own child scopes.  This ensures assignment evals work properly.
                ((BlockExpression) block).Scope = scope;
                ((BlockExpression) block).SetChildrenScopes(scope);
            }
            else {
                return null;
            }
            var val = CompilerServices.CreateLambdaForExpression(block)();
            return val;
        }

        [NovaExport("_class_eval")]
        private static dynamic ClassEval(object self, string eval, NovaScope scope)
        {
            NovaClass @class;
            var instance = self as NovaInstance;
            if (instance != null)
            {
                @class = instance.Class;
            }
            else
            {
                @class = self as NovaClass;
            }
            if (@class == null)
                return null;

            var xexpression = string.Format("{0};", eval);

            var res = NovaParser.Parse(xexpression);

            return res != null ? RuntimeOperations.DefineCategory(@class, res.ToList(), scope) : null;
        }

        [NovaExport("_instance_eval")]
        private static dynamic InstanceEval(object self, string eval, NovaScope scope)
        {
            if (!(self is NovaInstance instance))
            {
                return null;
            }

            var xexpression = string.Format("{0};", eval);
            var res = NovaParser.Parse(xexpression);
            Expression block;
            if (res != null)
            {
                scope["self"] = scope["super"] = instance;
                scope["<nova_context_invokemember>"] = true;
                string selfName;
                var selfScope = scope.SearchForObject(instance, out selfName);
                if (selfScope != null && selfName != null)
                {
                    scope["<nova_context_selfscope>"] = selfScope;
                    scope["<nova_context_selfname>"] = selfName;
                }
                block = NovaExpression.NovaBlock(res);
                // We want eval'd expressions to execute in the current scope, not its own child scopes.  This ensures assignment evals work properly.
                ((BlockExpression)block).Scope = scope;
                ((BlockExpression)block).SetChildrenScopes(scope);
            }
            else
            {
                return null;
            }
            var val = CompilerServices.CreateLambdaForExpression(block)();
            return val;
        }

        [NovaExport("box")]
        private static dynamic Box(object val) {
            return Nova.Box(val);
        }
    }
}