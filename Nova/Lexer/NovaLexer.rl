// -----------------------------------------------------------------------
// <copyright file="NovaLexer.rl" Company="Michael Tindal">
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

%%{

    machine nova_lexer;

    newline = '\n';
    any_count_line = any | newline;

    c_comment := any_count_line* :>> '*/' @{fgoto main;};

	attach_comment := |*
        '*/' { Token("ATTACHCOMMENT", StringBuf.ToString()); };

        any { AppendChar(); };
    *|;

    heredoc := |*
        space       { if(!DelimiterFound) { DelimiterFound = true; } else { StringBuf.Append(data.Substring(ts, 1)); } };

        any         {
                        if(!DelimiterFound) {
                            Delimiter.Append(data.Substring(ts, 1));
                        } else {
							StringBuf.Append(data.Substring(ts, 1));
							if(StringBuf.ToString().EndsWith(Delimiter.ToString())) {
								Token("STRING", StringBuf.Replace(Delimiter.ToString(),"").ToString());
								fgoto main;
							}
						}
					};

    *|;

    stringarray := |*
        any         {
                        AppendChar();
						if(DelimiterCharMatched(StringBuf.ToString(), DelimiterChar)) {
							Token("STRINGARRAY", StringBuf.ToString().Substring(0, StringBuf.ToString().Length-1));
							fgoto main;
						};
                    };
    *|;

    string := |*
        any         {
						AppendChar();
						if(DelimiterCharMatched(StringBuf.ToString(), DelimiterChar)) {
							Token("STRING", StringBuf.ToString().Substring(0, StringBuf.ToString().Length-1));
							fgoto main;
						};
                    };
    *|;

    regex := |*
        any         {
                        AppendChar();
						if(DelimiterCharMatched(StringBuf.ToString(), DelimiterChar)) {
							Token("REGEX", StringBuf.ToString().Substring(0, StringBuf.ToString().Length-1));
							fgoto main;
						};
                    };
    *|;

    main := |*

		alnum_u = alnum | '_' | '@' | '?' | '!';
		alpha_u = alpha | '_' | '@' | '$';

	    # more sane EOL comments, and the only ones we need
		'#' [^\n]* newline;
		'//' [^\n]* newline;
		'/*' { fgoto c_comment; };
		'//*' { CreateStringBuf(); fgoto attach_comment; };

		# Keywords
		'class'         { Keyword("CLASS");     };
		'module'		{ Keyword("MODULE");	};
		'sync'          { Keyword("SYNC");      };
		'throw'         { Keyword("THROW");     };
		'set'           { Keyword("SET");       };
		'for'           { Keyword("FOR");       };
		'in'            { Keyword("IN");        };
		'if'            { Keyword("IF");        };
		'unless'        { Keyword("UNLESS");    };
		'else'          { Keyword("ELSE");      };
		'ensure'        { Keyword("ENSURE");    };
		'do'            { Keyword("DO");        };
		'end'			{ Keyword("END");		};
		'begin'         { Keyword("BEGIN");     };
		'while'         { Keyword("WHILE");     };
		'until'         { Keyword("UNTIL");     };
		'def'           { Keyword("DEF"); 		};
		'self'          { Keyword("SELF");      };
		'super'         { Keyword("SUPER");     };
		'yield'         { Keyword("YIELD");     };
		'nil'           { Keyword("NIL");       };
		'const'         { Keyword("CONST");     };
		'return'        { Keyword("RETURN");    };
		'break'         { Keyword("BREAK");     };
		'continue'      { Keyword("CONTINUE");  };
		'switch'        { Keyword("SWITCH");    };
		'case'          { Keyword("CASE");      };
		'default'       { Keyword("DEFAULT");   };
		'rescue'        { Keyword("RESCUE");    };
		'true'          { Keyword("TRUE");      };
		'false'         { Keyword("FALSE");     };
		'and'           { Keyword("TAND");      };
		'or'            { Keyword("TOR");       };
		'not'           { Keyword("TNOT");      };
		'defined?'      { Keyword("DEFINED");   };
		'include'		{ Keyword("INCLUDE");	};
		'undef_method'  { Keyword("UNDEF");     };
		'remove_method' { Keyword("REMOVE");    };
		'alias'         { Keyword("ALIAS");     };
		'$:'            { Keyword("CONTEXT");   };
		'$#'            { Keyword("EXCEPTION"); };
		'interface'     { Keyword("INTERFACE"); };
		'retry'			{ Keyword("RETRY");	    };
		'loop'			{ Keyword("LOOP");		};
		'typeof'		{ Keyword("TYPEOF");	};

		# symbols
		'|>'            { Symbol("FORWARDPIPE"); };
		'<|'            { Symbol("BACKWARDPIPE"); };
		':'             { Symbol("COLON");     };
		'::'            { Symbol("DCOLON");    };
		';'             { Symbol("SEMICOLON"); };
		'#'             { Symbol("POUND");     };
		':='            { Symbol("REP");       };
		'['             { Symbol("LBRACKET");  };
		']'             { Symbol("RBRACKET");  };
		'{'             { Symbol("LBRACE");    };
		'}'             { Symbol("RBRACE");    };
		'('             { Symbol("LPAREN");    };
		')'             { Symbol("RPAREN");    };
		','             { Symbol("COMMA");     };
		'='             { Symbol("ASSIGN");    };
		'+='            { Symbol("ADDASSIGN"); };
		'-='            { Symbol("SUBASSIGN"); };
		'*='            { Symbol("MULASSIGN"); };
		'/='            { Symbol("DIVASSIGN"); };
		'%='            { Symbol("MODASSIGN"); };
		'**='           { Symbol("EXPASSIGN"); };
		'<<='           { Symbol("SHLASSIGN"); };
		'>>='           { Symbol("SHRASSIGN"); };
		'&='            { Symbol("ANDASSIGN"); };
		'|='            { Symbol("ORASSIGN");  };
		'^='            { Symbol("XORASSIGN"); };
		'||='           { Symbol("CONDASSIGNO"); };
		'&&='           { Symbol("CONDASSIGNA"); };
		'.'             { Symbol("DOT");       };
		'..'            { Symbol("IRANGE");    };
		'...'           { Symbol("ERANGE");    };
		'=>'            { Symbol("HASH");      };
		'?'             { Symbol("QUESTION");  };
		'%%'            { Symbol("PERCENT");   };
		'=~'            { Symbol("REGEXMATCH"); };
		'!~'            { Symbol("REGEXNOMATCH"); };
		'+@'            { Symbol("UPLUS");     };
		'-@'            { Symbol("UMINUS");    };
		'=='            { Symbol("EQUAL");     };
		'!='            { Symbol("NOTEQUAL");  };
		'==='           { Symbol("WHEN");      };
		'!=='           { Symbol("WHENNOT");   };
		'<=>'           { Symbol("COMPARE");   };
		'<'             { Symbol("LESSTHAN");  };
		'<='            { Symbol("LESSTHANEQUAL"); };
		'>'             { Symbol("GREATERTHAN"); };
		'>='            { Symbol("GREATERTHANEQUAL"); };
		'&&'            { Symbol("LOGICALAND"); };
		'||'            { Symbol("LOGICALOR"); };
		'^^'            { Symbol("LOGICALXOR"); };
		'!'             { Symbol("NOT");       };
		'+'             { Symbol("PLUS");      };
		'-'             { Symbol("MINUS");     };
		'*'             { Symbol("MULTIPLY");  };
		'/'             { Symbol("DIVIDE");    };
		'%'             { Symbol("MODULO");    };
		'**'            { Symbol("EXPONENT");  };
		'<<'            { Symbol("SHIFTLEFT"); };
		'>>'            { Symbol("SHIFTRIGHT"); };
		'&'             { Symbol("BITWISEAND"); };
		'|'             { Symbol("BITWISEOR"); };
		'^'             { Symbol("BITWISEXOR"); };
		'~'             { Symbol("BITWISEINVERSE"); };
		'[]'            { Symbol("BRACKETS");  };
		'[]='           { Symbol("BRACKETSASSIGN"); };
		'++'            { Symbol("INCREMENT"); };
		'--'            { Symbol("DECREMENT"); };
		'??'			{ Symbol("SWITCHOP");  };

		# identifiers for things like variables and such
		alpha_u alnum_u* { Token("IDENTIFIER"); };

		# single quoted string
		sliteralChar = [^'\\] | newline | ( '\\' . any_count_line );
		'\'' . sliteralChar* . '\'' { String(); };

		# double quoted string
		dliteralChar = [^"\\] | newline | ( '\\' . any_count_line );
		'"' . dliteralChar* . '"' { String(); };

		# heredoc
		'<<-' { DelimiterFound = false; CreateDelimiter(); CreateStringBuf(); fgoto heredoc; };

		# special characters
		'%w' . any { CreateStringBuf(); SetDelimiterChar(); fgoto stringarray; };
		('%q' | '%Q' | '%x') . any { CreateStringBuf(); SetDelimiterChar(); fgoto string; };
		'%r' . any { CreateStringBuf(); SetDelimiterChar(); fgoto regex; };

		# regular expression string
		'%/' . [^/]* . '/' { Regex(); };

   		('+' | '-' | '=' | '!' | '~' | '<' | '>' | '&' | '|' | '^' | '%' | '*' | '/' | '@' | '?' | ':')+ { CustomOp(); };

		newline { Symbol("NEWLINE"); };

		# Whitespace is standard ws, newlines and control codes.
		any_count_line - 0x21..0x7e;

		# integers, floats, and hex encoded values
		digit+ { Integer(); };
		digit+ '.' digit+ { Number(); };
		'0x' xdigit+ { Hex(); };
		*|;

}%%

// Unreachable code detected.
#pragma warning disable 162

using Nova.Expressions;
using Nova.Parser;

namespace Nova.Lexer {
	using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
	using System.Text;
	using Microsoft.Scripting;


	public partial class NovaLexer {
		public StringBuilder StringBuf { get; set; }

        // SB
        public void CreateStringBuf() { StringBuf = new StringBuilder(); }

        public StringBuilder Delimiter { get; set; }

        public bool DelimiterFound { get; set; }


        public char DelimiterChar { get; set; }

        // DL
        public void CreateDelimiter() { Delimiter = new StringBuilder(); }

        // AP
        public void AppendChar() { StringBuf.Append(data.Substring(ts, 1)); }

        // DC
        public void SetDelimiterChar() { DelimiterChar = data[ts + 2]; }

        public bool DelimiterCharMatched(string buf, char chr) {
            switch (chr) {
                case '(':
                    chr = ')';
                    break;
                case '[':
                    chr = ']';
                    break;
                case '{':
                    chr = '}';
                    break;
                default:
                    break;
            }

            return buf.Substring(buf.Length - 1)[0] == chr;
        }

		%% write data;

		int ts, te;

		int cs, act;

		int p = 0;

		int pe, eof;

		string data;

		public NovaTokenQueue Queue { get; set; }

		public NovaLexer(string fileName, string source) {
			SetupLineIndexes(source);
			Queue = new NovaTokenQueue(new NovaParser(null), this, fileName);
			SetSource(source);
		}

		public void SetSource(string source) {
			data = source;

			%%write init;
		}

		public void Scan() {
			pe = data.Length;
			eof = pe;

			%% write exec;

			if(cs == nova_lexer_error) {
				throw new SyntaxErrorException("Lexer error");
			}
		}
	}
}
