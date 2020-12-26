using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Nova.Parser;
using Microsoft.Scripting;

namespace Nova.Lexer {
    public class NovaTokenQueue : ITokenSource {
        private readonly Queue<CommonToken> _tokens;
        private readonly NovaTokenFactory _factory;
        private readonly NovaLexer _lexer;

        public NovaTokenQueue(NovaParser parser, NovaLexer lexer, string sourceName) {
            Parser = parser;
            _lexer = lexer;
            SourceName = sourceName;
            _factory = new NovaTokenFactory {Parser = parser};
            _tokens = new Queue<CommonToken>();
        }

        internal NovaParser Parser { get; }

        public int NumTokens
        {
            get;
            private set;
        }

        public ICharStream InputStream => null;

        public string SourceName { get; }

        public ITokenFactory TokenFactory {
            get => _factory;
            set { }
        }

        public IToken NextToken() {
            return _tokens.Count > 0 ? _tokens.Dequeue() : new CommonToken(-1);
        }

        public int Line => _tokens.First().Line;
        public int Column => _tokens.First().Column;

        public void AddToken<T>(string name, string text, T value, NovaTokenCategory category = NovaTokenCategory.Normal)
        {
            var num = _factory.GetTokenFromName(name);
            if (num == -2) {
                throw new SyntaxErrorException($"Invalid token {name}");
            }
            var item = new NovaToken<T>(num, text, value) {Span = _lexer.GetSourceSpanOfToken()};
            if (category == NovaTokenCategory.Comment)
            {
                item.Channel = TokenConstants.HiddenChannel;
            }
            NumTokens++;
            _tokens.Enqueue(item);
        }
    }
}