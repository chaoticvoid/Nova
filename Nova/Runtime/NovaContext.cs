// -----------------------------------------------------------------------
// <copyright file="NovaContext.cs" company="Michael Tindal">
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

namespace Nova.Runtime {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.Scripting.Ast;
    using Microsoft.Scripting.Runtime;
    using Microsoft.Scripting;
    using System.Text;
    using System.Reflection;
	using Expressions;
	using Parser;
	using Runtime;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class NovaContext : LanguageContext {
		public NovaContext(ScriptDomainManager domainManager, IDictionary<string, object> options) : base(domainManager) {

		}

		public override ScriptCode CompileSourceCode(SourceUnit sourceUnit, CompilerOptions options, ErrorSink errorSink)
		{
		    var res = NovaParser.Parse(sourceUnit.GetCode(), sourceUnit);

			if (res != null) {
				Expression mainBlock = NovaExpression.NovaBlock (res);
				return new NovaScriptCode(mainBlock, sourceUnit);
			} else {
				throw new SyntaxErrorException("Syntax error", sourceUnit, SourceSpan.None, 0, Severity.Error);
			}
		}
	}
}
