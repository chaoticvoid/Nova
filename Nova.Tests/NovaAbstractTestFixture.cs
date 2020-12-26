// -----------------------------------------------------------------------
// <copyright file="NovaAbstractTestFixture.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime;
using IronRuby;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Nova.Builtins;
using Nova.Expressions;
using Nova.Lexer;
using Nova.Parser;
using Nova.Runtime;
using NovaC = Nova.Nova;
using NUnit.Framework;

namespace Nova.Tests {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    [TestFixture]
    public abstract class NovaAbstractTestFixture {
        public static ScriptRuntime GetRuntime() {
            return Nova.CreateRuntime(Nova.CreateNovaSetup(), Ruby.CreateRubySetup());
        }

        public dynamic CompileAndExecute(string rawsource) {
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString(rawsource);
            return source.Execute(engine.CreateScope());
        }

        public static Symbol XS(string name) {
            return Symbol.NewSymbol(name);
        }

        public static NovaDictionary SD(Dictionary<object, object> dict) {
            return new NovaDictionary(dict);
        }


        public Expression Compile(string rawsource)
        {
            var res = NovaParser.Parse(rawsource);

            return res != null ? NovaExpression.NovaBlock(res) : null;
        }
    }
}