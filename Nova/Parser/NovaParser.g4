// ----------------------------------------------------------------------- Copyright 2017 Michael
// Tindal
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
// in compliance with the License. You may obtain a copy of the License at
//
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions and limitations under
// the License. -----------------------------------------------------------------------

parser grammar NovaParser;

tokens {
	NEWLINE,
	SEMICOLON,
	IF,
	WHILE,
	UNLESS,
	UNTIL,
	SWITCHOP,
	ASSIGN,
	BREAK,
	RETRY,
	CONTINUE,
	THROW,
	LBRACE,
	RBRACE,
	SWITCH,
	CASE,
	COLON,
	DEFAULT,
	LOOP,
	FOR,
	LPAREN,
	IDENTIFIER,
	IN,
	RPAREN,
	DO,
	ELSE,
	RETURN,
	SET,
	CONST,
	ADDASSIGN,
	SUBASSIGN,
	MULASSIGN,
	DIVASSIGN,
	MODASSIGN,
	SHLASSIGN,
	SHRASSIGN,
	ANDASSIGN,
	ORASSIGN,
	XORASSIGN,
	EXPASSIGN,
	CONDASSIGNO,
	CONDASSIGNA,
	INCREMENT,
	DECREMENT,
	OP,
	BACKWARDPIPE,
	FORWARDPIPE,
	LOGICALOR,
	LOGICALXOR,
	LOGICALAND,
	COMPARE,
	REGEXMATCH,
	REGEXNOMATCH,
	EQUAL,
	NOTEQUAL,
	LESSTHANEQUAL,
	LESSTHAN,
	GREATERTHANEQUAL,
	GREATERTHAN,
	BITWISEXOR,
	BITWISEOR,
	BITWISEAND,
	SHIFTLEFT,
	SHIFTRIGHT,
	PLUS,
	MINUS,
	MULTIPLY,
	DIVIDE,
	MODULO,
	NOT,
	BITWISEINVERSE,
	TYPEOF,
	YIELD,
	UNDEF,
	STRING,
	REMOVE,
	EXPONENT,
	LBRACKET,
	RBRACKET,
	HASH,
	COMMA,
	BRACKETS,
	NUMBER,
	INTEGER,
	NIL,
	TRUE,
	FALSE,
	DOT,
	CLASS,
	SELF,
	SUPER,
	CONTEXT,
	IRANGE,
	ERANGE,
	WHENCOMP,
	UPLUS,
	UMINUS,
	BRACKETSASSIGN,
	DCOLON,
	DEF,
	END,
	ALIAS,
	INCLUDE,
	MODULE,
	BEGIN,
	ENSURE,
	RESCUE,
	SYNC,
	REGEX,
	COMMENT
}

program: block_contents term* EOF | top_level_statement EOF;

block_contents: (term_top_level_statement | term)*;

term: NEWLINE | SEMICOLON;

term_top_level_statement: top_level_statement term;

top_level_statement: conditional_statement | statement;

conditional_statement:
	statement IF expression
	| statement WHILE expression
	| statement UNLESS expression
	| statement UNTIL expression
	| statement SWITCHOP hash;

statement:
	parallel_assign_left ASSIGN parallel_assign_right
	| loop_construct
	| class_declaration
	| module_declaration
	| switch_construct
	| for_construct
	| for_in_construct
	| until_construct
	| do_until_construct
	| while_construct
	| do_while_construct
	| unless_construct
	| if_else_construct
	| begin_construct
	| sync_construct
	| alias
	| include
	| BREAK
	| RETRY
	| CONTINUE
	| THROW expression
	| return_expression
	| function_definition
	| expression;

block: LBRACE block_contents RBRACE;

switch_construct:
	SWITCH expression LBRACE term? case_block+ term? default_block? term? RBRACE;

case_block:
	CASE expression (COMMA expression)* COLON block term?;

default_block: DEFAULT COLON block;

loop_construct: LOOP block;

for_construct:
	FOR LPAREN expression SEMICOLON expression SEMICOLON expression RPAREN block?;

for_in_construct:
	FOR LPAREN IDENTIFIER IN expression RPAREN block;

until_construct: UNTIL expression block?;

do_until_construct: DO block UNTIL expression;

while_construct: WHILE expression block?;

do_while_construct: DO block WHILE expression;

unless_construct: UNLESS expression block else_part?;

if_else_construct: IF expression block else_part?;

else_part:
	ELSE if_else_construct
	| ELSE unless_construct
	| ELSE block;

return_expression: RETURN call_args?;

expression: assignment | arg;

prefix_increment: (INCREMENT | DECREMENT) (
		lvalue_access
		| lvalue_instance_ref
		| lvalue
	);

postfix_increment: (lvalue_access | lvalue_instance_ref | lvalue) (
		INCREMENT
		| DECREMENT
	);

assignment:
	<assoc = right> (SET | CONST)? (
		lvalue_access
		| lvalue_instance_ref
		| lvalue
	) (
		ASSIGN
		| ADDASSIGN
		| SUBASSIGN
		| MULASSIGN
		| DIVASSIGN
		| MODASSIGN
		| SHLASSIGN
		| SHRASSIGN
		| ANDASSIGN
		| ORASSIGN
		| XORASSIGN
		| EXPASSIGN
		| CONDASSIGNO
		| CONDASSIGNA
	) expression
	| prefix_increment
	| postfix_increment;

prefix_op: OP op_expression;

postfix_op: op_expression OP;

arg: prefix_op | postfix_op | op_expression;

op_expression: pipe_expression ( OP pipe_expression)*;

pipe_op: BACKWARDPIPE | FORWARDPIPE;

pipe_expression:
	logical_or_expression (pipe_op logical_or_expression)*;

logical_or_expression:
	logical_xor_expression (LOGICALOR logical_xor_expression)*;

logical_xor_expression:
	logical_and_expression (LOGICALXOR logical_and_expression)*;

logical_and_expression:
	equality_expression (LOGICALAND equality_expression)*;

equality_op_eq_neq: EQUAL | NOTEQUAL;

equality_op:
	COMPARE
	| REGEXMATCH
	| REGEXNOMATCH
	| equality_op_eq_neq;

equality_expression:
	comparison_expression (equality_op comparison_expression)*;

comparison_op:
	LESSTHANEQUAL
	| LESSTHAN
	| GREATERTHANEQUAL
	| GREATERTHAN;

comparison_expression:
	bitwise_or_expression (comparison_op bitwise_or_expression)*;

bitwise_or_op: BITWISEXOR | BITWISEOR;

bitwise_or_expression:
	bitwise_and_expression (bitwise_or_op bitwise_and_expression)*;

bitwise_and_expression:
	shift_expression (BITWISEAND shift_expression)*;

shift_op: SHIFTLEFT | SHIFTRIGHT;

shift_expression:
	additive_expression (shift_op additive_expression)*;

additive_op: PLUS | MINUS;

additive_expression:
	multiplicative_expression (
		additive_op multiplicative_expression
	)+
	| additive_op+ multiplicative_expression
	| multiplicative_expression;

multiplicative_op: MULTIPLY | DIVIDE | MODULO;

multiplicative_expression:
	unary_expression (multiplicative_op unary_expression)*;

unary_expression:
	NOT+ unary_expression
	| BITWISEINVERSE+ unary_expression
	| TYPEOF unary_expression
	| YIELD call_args?
	| UNDEF STRING
	| UNDEF symbol
	| REMOVE STRING
	| REMOVE symbol
	| power_expression;

power_expression: atom (EXPONENT power_expression)?;

atom: LPAREN expression RPAREN | primary;

primary_right_side_parens:
	LPAREN call_args? RPAREN yield_block?;

primary_right_side_access: LBRACKET call_args RBRACKET;

primary_right_side_yield_block: yield_block | do_yield_block;

primary_right_side_parens_access:
	primary_right_side_parens
	| primary_right_side_access;

primary_function_call: primary_right_side_parens_access+ primary_right_side_yield_block?;

primary_left_side:
	literal
	| lvalue_instance_ref
	| lvalue_method_change
	| lvalue
	| array
	| hash
	| anonymous_function
	| anonymous_class_declaration;

primary: primary_left_side primary_function_call?;

hash:
	LBRACE (
		(( hash_key HASH arg) (COMMA hash_key HASH arg)*)
		| COLON
	) RBRACE;

single_hash_key: IDENTIFIER | STRING | symbol;

hash_key: arg;

array: BRACKETS | LBRACKET ( arg (COMMA arg)*)? RBRACKET;

literal:
	NUMBER
	| INTEGER
	| STRING
	| REGEX
	| NIL
	| TRUE
	| FALSE
	| symbol;

symbol: COLON IDENTIFIER | COLON STRING;

lvalue: variable;

lvalue_instance_ref_opt: function_name | CLASS;

lvalue_instance_ref: variable (DOT lvalue_instance_ref_opt)+;

lvalue_method_change:
	variable DOT UNDEF STRING
	| variable DOT UNDEF symbol
	| variable DOT REMOVE STRING
	| variable DOT REMOVE symbol;

lvalue_access: variable (LBRACKET call_args RBRACKET)+;

variable: IDENTIFIER | SELF | SUPER | CONTEXT;

call_args: call_arg (COMMA call_arg)*;

call_arg_single_hash: (single_hash_key HASH arg);

call_arg: call_arg_single_hash | ( IDENTIFIER COLON)? arg;

first_arg: (COLON)? IDENTIFIER ( ASSIGN arg)?;

first_var_arg: (MULTIPLY IDENTIFIER);

first_block_arg: (BITWISEAND IDENTIFIER);

end_var_arg: (COMMA MULTIPLY IDENTIFIER);

end_block_arg: (COMMA BITWISEAND IDENTIFIER);

next_arg: COMMA (COLON)? IDENTIFIER ( ASSIGN arg)?;

definition_argument_list_no_paren:
	first_var_arg (first_block_arg)?
		| first_block_arg
		| first_arg (next_arg)* (end_var_arg)? (end_block_arg)?;

definition_argument_list:
	LPAREN (
		definition_argument_list_no_paren?
	) RPAREN;

function_name:
	IDENTIFIER
	| IRANGE
	| ERANGE
	| BITWISEOR
	| BITWISEXOR
	| BITWISEAND
	| COMPARE
	| EQUAL
	| WHENCOMP
	| REGEXMATCH
	| LESSTHAN
	| LESSTHANEQUAL
	| GREATERTHAN
	| GREATERTHANEQUAL
	| PLUS
	| MINUS
	| MULTIPLY
	| DIVIDE
	| MODULO
	| EXPONENT
	| SHIFTLEFT
	| SHIFTRIGHT
	| UPLUS
	| UMINUS
	| BRACKETS
	| BRACKETSASSIGN
	| DCOLON
	| OP
	| INCREMENT
	| DECREMENT;

function_definition:
	DEF (lvalue DOT)? function_name definition_argument_list? block;

anonymous_function:
	BITWISEXOR definition_argument_list? block;

yield_block:
	LBRACE (BITWISEOR definition_argument_list_no_paren BITWISEOR)? block_contents RBRACE;

do_yield_block:
	DO (BITWISEOR definition_argument_list_no_paren BITWISEOR)? block_contents END;

parallel_assign_left:
	MULTIPLY lvalue
	| lvalue ( COMMA parallel_assign_left_item)+;

parallel_assign_left_item:
	LPAREN parallel_assign_left RPAREN
	| (MULTIPLY lvalue)
	| lvalue;

parallel_assign_right:
	parallel_assign_right_item (COMMA parallel_assign_right_item)*;

parallel_assign_right_item: ((MULTIPLY lvalue) | expression);

alias: ALIAS alias_part alias_part;

alias_part: IDENTIFIER | symbol;

include: INCLUDE IDENTIFIER (DCOLON IDENTIFIER)*;

class_declaration:
	CLASS (
		(SHIFTLEFT primary)
		| (IDENTIFIER (LESSTHAN IDENTIFIER)?)
	) LBRACE block_contents RBRACE;

anonymous_class_declaration:
	LPAREN LBRACE (LESSTHAN IDENTIFIER)? block_contents RBRACE RPAREN;

module_declaration:
	MODULE IDENTIFIER LBRACE block_contents RBRACE;

begin_construct:
	BEGIN block (rescue_block)* rescue_else_block? rescue_ensure_block?;

rescue_block:
	RESCUE MULTIPLY (HASH IDENTIFIER)? block
	| RESCUE (identifiers (HASH IDENTIFIER)?)? block;

rescue_else_block: ELSE block;

rescue_ensure_block: ENSURE block;

identifiers: IDENTIFIER (COMMA IDENTIFIER)*;

sync_construct: SYNC LPAREN IDENTIFIER RPAREN block;

