// -----------------------------------------------------------------------
// <copyright file="DynamicScope.cs" Company="Michael Tindal">
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
using System.Reflection;
using Nova.Builtins;
using Nova.Runtime;
using Microsoft.Scripting.Hosting;

namespace Nova {
    /// <summary>
    ///     Main class for users of Nova.
    /// </summary>
    public static class Nova {
        private static readonly LanguageSetup _NovaSetup =
            new LanguageSetup("Nova.Runtime.NovaContext,Nova,Version=0.5.0.0,Culture=neutral",
                "Nova 0.5", new[] {"Nova"}, new[] {".nova", ".nv"});

        static Nova() {
            Globals = CreateRuntime().GetEngine("Nova").CreateScope();
            new Kernel();
        }

        public static NovaScope CurrentContext { get; internal set; }

        public static ScriptScope Globals { get; private set; }

        /// <summary>
        ///     Generate a FunctionArgument object to be used for named arguments when calling Nova functions from a .NET
        ///     language.
        /// </summary>
        /// <example>(Where testfunc is a Nova dynamic function): testfunc(Nova.Arg("b",2),10);</example>
        /// <param name="name">Name of the argument.</param>
        /// <param name="value">Value for the argument.</param>
        public static FunctionArgument Arg(dynamic name, dynamic value) {
            return new FunctionArgument(name.ToString(), Expression.Constant(value));
        }

        /// <summary>
        ///     Returns a NovaNativeFunction invokable wrapper around the Arg() function that allows you to consume Nova
        ///     keyword arguments from other languages, pursuant to the limitations of your language.
        /// </summary>
        /// <example>scope.SetVariable("sa",Nova.GetFunctionArgumentGenerator())</example>
        /// <returns>A NovaNativeFuncton dynamic wrapper around Nova.Arg(name,value).</returns>
        public static dynamic GetFunctionArgumentGenerator() {
            return new NovaNativeFunction(typeof (Nova),
                typeof (Nova).GetMethod("Arg", BindingFlags.Public | BindingFlags.Static));
        }

        public static dynamic Box(object obj, NovaScope scope = null) {
            return NovaBoxedInstance.Box(obj, scope ?? new NovaScope());
        }

        public static dynamic BoxNoCache(object obj, NovaScope scope = null) {
            return NovaBoxedInstance.BoxNoCache(obj, scope ?? new NovaScope());
        }

        public static ScriptRuntime CreateRuntime(params LanguageSetup[] setups) {
            var setup = new ScriptRuntimeSetup();
            setups.ToList().ForEach(lsetup => setup.LanguageSetups.Add(lsetup));
            return new ScriptRuntime(setup);
        }

        public static ScriptRuntime CreateRuntime() {
            return CreateRuntime(_NovaSetup);
        }

        public static LanguageSetup CreateNovaSetup() {
            return _NovaSetup;
        }

        public static dynamic Execute(string source, ScriptScope scope) {
            return CreateRuntime().GetEngine("Nova").CreateScriptSourceFromString(source).Execute(scope);
        }

        public static dynamic Execute(string source) {
            return Execute(source, Globals);
        }

        public static dynamic Box(Type type) {
            return NovaClass.BoxClass(type);
        }
    }

    public static class NovaExtensions {
        public static dynamic Eval(this string source) {
            return Nova.Execute(source);
        }
    }
}
