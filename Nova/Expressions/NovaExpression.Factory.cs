// -----------------------------------------------------------------------
// <copyright file="cs" Company="Michael Tindal">
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
using Nova.Runtime;

namespace Nova.Expressions {
    /// <summary>
    ///     Methods for NovaExpression that provide factory expressions similar to how DLR Expression.* functions work.
    /// </summary>
    public partial class NovaExpression {
        /// <summary>
        ///     Creates a new comparison expression.
        /// </summary>
        /// <param name="left">The left side of the comparison.</param>
        /// <param name="right">The right side of the comparison.</param>
        /// <returns>A new CompareExpression with the given left and right operands.</returns>
        public static BinaryExpression Compare(Expression left, Expression right) {
            return new BinaryExpression(left, right, NovaExpressionType.Compare);
        }

        public static BinaryExpression OrButNotAlso(Expression left, Expression right) {
            return new BinaryExpression(left, right, NovaExpressionType.LogicalXor);
        }

        public static BinaryExpression Match(Expression left, Expression right)
        {
            return new BinaryExpression(left, right, NovaExpressionType.Match);
        }

        public static BinaryExpression NotMatch(Expression left, Expression right)
        {
            return new BinaryExpression(left, right, NovaExpressionType.NotMatch);
        }

        public static AssignmentExpression Assign(LeftHandValueExpression left, Expression right) {
            return Assign(left, right, ExpressionType.Assign);
        }

        public static AssignmentExpression Assign(LeftHandValueExpression left, Expression right, ExpressionType type) {
            return new AssignmentExpression(left, right, type);
        }

        public static ConditionalAssignmentExpression ConditionalAssign(LeftHandValueExpression left, Expression right,
            NovaExpressionType conditionalAssignmentType) {
            return new ConditionalAssignmentExpression(left, right, conditionalAssignmentType);
        }

        public static ParallelAssignmentExpression ParallelAssign(
            List<ParallelAssignmentExpression.ParallelAssignmentInfo> lvalues,
            List<ParallelAssignmentExpression.ParallelAssignmentInfo> rvalues) {
            return new ParallelAssignmentExpression(lvalues, rvalues);
        }

        public static SetAssignExpression SetAssign(Expression left, Expression right, ExpressionType type) {
            return new SetAssignExpression(left, right, type);
        }

        public static BinaryExpression Binary(Expression left, Expression right, ExpressionType type) {
            return new BinaryExpression(left, right, type);
        }

        public static UnaryExpression Unary(Expression expr, ExpressionType type) {
            return new UnaryExpression(expr, type);
        }

        public static BooleanExpression Boolean(Expression expr) {
            return new BooleanExpression(expr);
        }

        public new static IfExpression IfThen(Expression test, Expression ifTrue) {
            Expression ifFalse = Expression.Block(Default(ifTrue.Type));

            return IfElse(test, ifTrue, ifFalse);
        }

        public static IfExpression IfElse(Expression test, Expression ifTrue, Expression ifFalse) {
            return new IfExpression(test, ifTrue, ifFalse);
        }

        public static UnlessExpression UnlessThen(Expression test, Expression ifTrue) {
            Expression ifFalse = Expression.Block(Default(ifTrue.Type));

            return UnlessElse(test, ifTrue, ifFalse);
        }

        public static UnlessExpression UnlessElse(Expression test, Expression ifTrue, Expression ifFalse) {
            return new UnlessExpression(test, ifTrue, ifFalse);
        }

        public static WhileExpression While(Expression test, Expression body) {
            return new WhileExpression(test, body);
        }

        public static DoWhileExpression DoWhile(Expression test, Expression body) {
            return new DoWhileExpression(test, body);
        }

        public static ForExpression For(Expression init, Expression test, Expression step, Expression body) {
            return new ForExpression(init, test, step, body);
        }

        public static ForInExpression ForIn(string varName, Expression enumerator, Expression body) {
            return new ForInExpression(varName, enumerator, body);
        }

        public static UntilExpression Until(Expression test, Expression body) {
            return new UntilExpression(test, body);
        }

        public static DoUntilExpression DoUntil(Expression test, Expression body) {
            return new DoUntilExpression(test, body);
        }

        public static LoopExpression NovaLoop(Expression body)
        {
            return new LoopExpression(body);
        }

        public static TypeofExpression TypeOf(Expression expr) {
            return new TypeofExpression(expr);
        }

        public static SwitchExpression Switch(Expression test, List<SwitchCase> cases) {
            return Switch(test, null, cases);
        }

        public static SwitchExpression Switch(Expression test, Expression @default, List<SwitchCase> cases) {
            return new SwitchExpression(test, @default, cases);
        }

        public new static ConvertExpression Convert(Expression expr, Type type) {
            return new ConvertExpression(expr, type);
        }

        public static PutsExpression Puts(Expression expr) {
            return new PutsExpression(expr);
        }

        public static BlockExpression NovaBlock(params Expression[] body) {
            return new BlockExpression(body.ToList(), new NovaScope());
        }

        public static LeftHandValueExpression LeftHandValue(Expression e) {
            return new LeftHandValueExpression(e);
        }

        public static VariableExpression Variable(Expression name) {
            return new VariableExpression(name);
        }

        public static VariableExpression Variable(Symbol sym) {
            return new VariableExpression(sym);
        }

        public static CreateArrayExpression CreateArray(List<Expression> values) {
            return new CreateArrayExpression(values);
        }

        public static CreateDictionaryExpression CreateDictionary(List<Expression> values) {
            return new CreateDictionaryExpression(values);
        }

        public static KeyValuePairExpression KeyValuePair(Expression key, Expression value) {
            return new KeyValuePairExpression(key, value);
        }

        public static AccessExpression Access(Expression container, List<FunctionArgument> args) {
            return new AccessExpression(container, args);
        }

        public static AccessSetExpression AccessSet(Expression container, List<FunctionArgument> args, Expression value) {
            return AccessSet(container, args, value, ExpressionType.Assign);
        }

        public static AccessSetExpression AccessSet(Expression container, List<FunctionArgument> args, Expression value,
            ExpressionType extra) {
            return new AccessSetExpression(container, args, value, extra);
        }

        public static ConditionalAccessSetExpression ConditionalAccessSet(Expression container,
            List<FunctionArgument> args, Expression value,
            NovaExpressionType conditionalAssignmentType) {
            return new ConditionalAccessSetExpression(container, args, value, conditionalAssignmentType);
        }

        public static FunctionDefinitionExpression FunctionDefinition(string name, List<FunctionArgument> args,
            Expression body) {
            return new FunctionDefinitionExpression(name, args, body);
        }

        public static SingletonDefinitionExpression SingletonDefinition(Expression singleton, string name,
            List<FunctionArgument> args, Expression body) {
            return new SingletonDefinitionExpression(singleton, name, args, body);
        }

        public static Expression Call(Expression func, List<FunctionArgument> args) {
            return new FunctionCallExpression(func, args);
        }

        public static Expression CallWithPipe(Expression func, List<FunctionArgument> args, NovaExpressionType pipeType) {
            return new FunctionCallExpression(func, args, pipeType);
        }

        public static Expression CallUnaryOp(Expression func, bool isPostfix) {
            return new FunctionCallExpression(func, new List<FunctionArgument>(), true, isPostfix);
        }

        public static Expression Return(List<FunctionArgument> args) {
            return new ReturnExpression(args);
        }

        public static Expression Yield(List<FunctionArgument> args) {
            return new YieldExpression(args);
        }

        public static Expression String(string value) {
            return new StringExpression(value);
        }

        public static Expression Regex(string value)
        {
            return new RegexExpression(value);
        }

        public static Expression Number(object value)
        {
            return new NumberExpression(value);
        }

        public static Expression Invoke(Expression type, Expression method, List<FunctionArgument> args) {
            return new InvokeExpression(type, method, args);
        }

        public static Expression Alias(Expression from, Expression to) {
            return new AliasExpression(from, to);
        }

        public static Expression InstanceRef(Expression lvalue, Expression key) {
            return new InstanceReferenceExpression(lvalue, key);
        }

        public static Expression Include(List<string> names) {
            return new IncludeExpression(names);
        }

        public static Expression DefineClass(string name, string parent, List<Expression> contents) {
            return new ClassDefinitionExpression(name, parent, contents);
        }

        public static Expression ClassOpen(Expression name, List<Expression> contents)
        {
            return new ClassOpenExpression(name, contents);
        }

        public static Expression DefineModule(string name, List<Expression> contents) {
            return new ModuleDefinitionExpression(name, contents);
        }

        public static Expression Rescue(List<string> exceptionTypes, Expression body, string varName = "$#")
        {
            return new RescueExpression(exceptionTypes, body, varName);
        }

        public static Expression Begin(Expression tryBlock, List<Expression> rescueBlocks, Expression ensureBlock,
            Expression elseBlock)
        {
            return new BeginExpression(tryBlock, rescueBlocks, ensureBlock, elseBlock);
        }

        public new static Expression Throw(Expression exceptionObject)
        {
            return new ThrowExpression(exceptionObject);
        }

        public static Expression Sync(string varName, Expression body)
        {
            return new SyncExpression(varName, body);
        }

        public static Expression MethodChange(string varName, bool isRemove)
        {
            return new MethodChangeExpression(varName, isRemove);
        }

        public static Expression ObjectMethodChange(Expression lvalue, string varName, bool isRemove)
        {
            return new ObjectMethodChangeExpression(lvalue, varName, isRemove);
        }

        public static Expression SwitchOp(Expression test, Expression pairs)
        {
            var _pairs = (CreateDictionaryExpression) pairs;

            var _kvpList = _pairs.Values;

            var caseBlocks = (from KeyValuePairExpression _kvp in _kvpList select SwitchCase(Block(_kvp.Value), _kvp.Key)).ToList();

            var @default = Block(Default(caseBlocks.First().Body.Type));

            return Switch(Convert(test, caseBlocks.First().TestValues.First().Type), @default, caseBlocks);
        }
    }
}