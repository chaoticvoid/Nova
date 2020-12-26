// -----------------------------------------------------------------------
// <copyright file="NovaToken.cs" Company="Michael Tindal">
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

using Antlr4.Runtime;
using Microsoft.Scripting;

namespace Nova.Lexer {
    /// <summary>
    ///     Token used by Nova, deriving from CommonToken.
    /// </summary>
    public abstract class NovaToken : CommonToken {
        protected NovaToken(int type, string text) : base(type, text) {}

        public SourceSpan Span { get; internal set; }

        public NovaTokenCategory Category { get; protected set; }
    }

    public enum NovaTokenCategory
    {
        Keyword,
        Identifier,
        Number,
        String,
        Comment,
        Normal
    }

    public class NovaToken<T> : NovaToken {
        public NovaToken(int type, string text) : this(type, text, default) {}

        public NovaToken(int type, string text, T value, NovaTokenCategory category = NovaTokenCategory.Normal) : base(type, text) {
            Value = value;

            Category = category;
        }

        public T Value { get; set; }
    }
}