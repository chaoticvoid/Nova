// -----------------------------------------------------------------------
// <copyright file="YieldCheckVisitor.cs" company="Michael Tindal">
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
using Nova.Expressions;
using BinaryExpression = Nova.Expressions.BinaryExpression;
using BlockExpression = Nova.Expressions.BlockExpression;
using SwitchExpression = Nova.Expressions.SwitchExpression;
using UnaryExpression = Nova.Expressions.UnaryExpression;

namespace Nova.Runtime {
    /// <summary>
    ///     Base visitor for Nova expression visitors.
    /// </summary>
    public abstract class NovaExpressionVisitor : ExpressionVisitor {
        protected virtual Expression VisitAccess(AccessExpression node) {
            Visit(node.Container);
            node.Arguments.ForEach(arg => Visit(arg.Value));
            return node;
        }

        protected virtual Expression VisitAccessSet(AccessSetExpression node) {
            Visit(node.Container);
            node.Arguments.ForEach(arg => Visit(arg.Value));
            Visit(node.Value);
            return node;
        }

        protected virtual Expression VisitAlias(AliasExpression node) {
            Visit(node.From);
            Visit(node.To);
            return node;
        }

        protected virtual Expression VisitAssignment(AssignmentExpression node) {
            Visit(node.Left);
            Visit(node.Right);
            return node;
        }

        protected virtual Expression VisitBegin(BeginExpression node)
        {
            Visit(node.TryBlock);
            node.RescueBlocks.ForEach(block => Visit(block));
            Visit(node.EnsureBlock);
            Visit(node.ElseBlock);
            return node;
        }

        protected virtual Expression VisitBinary(BinaryExpression node) {
            Visit(node.Left);
            Visit(node.Right);
            return node;
        }

        protected virtual Expression VisitBlock(BlockExpression node) {
            node.Body.ForEach(arg => Visit(arg));
            return node;
        }

        protected virtual Expression VisitBoolean(BooleanExpression node) {
            Visit(node.Value);
            return node;
        }

        protected virtual Expression VisitClassDefinition(ClassDefinitionExpression node) {
            node.Contents.ForEach(content => Visit(content));
            return node;
        }

        protected virtual Expression VisitClassOpen(ClassOpenExpression node)
        {
            Visit(node.Name);
            node.Contents.ForEach(content => Visit(content));
            return node;
        }

        protected virtual Expression VisitConditionalAccessSet(ConditionalAccessSetExpression node) {
            Visit(node.Container);
            node.Arguments.ForEach(arg => Visit(arg.Value));
            Visit(node.Value);
            return node;
        }

        protected virtual Expression VisitConditionalAssignment(ConditionalAssignmentExpression node) {
            Visit(node.Left);
            Visit(node.Right);
            return node;
        }

        protected virtual Expression VisitConvert(ConvertExpression node) {
            Visit(node.Expression);
            return node;
        }

        protected virtual Expression VisitCreateArray(CreateArrayExpression node) {
            node.Values.ForEach(expr => Visit(expr));
            return node;
        }

        protected virtual Expression VisitCreateDictionary(CreateDictionaryExpression node) {
            node.Values.ForEach(expr => Visit(expr));
            return node;
        }

        protected virtual Expression VisitDoUntil(DoUntilExpression node) {
            Visit(node.Test);
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitDoWhile(DoWhileExpression node) {
            Visit(node.Test);
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitFor(ForExpression node) {
            Visit(node.Test);
            Visit(node.Step);
            Visit(node.Init);
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitForIn(ForInExpression node) {
            Visit(node.Enumerator);
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitFunctionCall(FunctionCallExpression node) {
            Visit(node.Function);
            node.Arguments.ForEach(arg => Visit(arg.Value));
            return node;
        }

        protected virtual Expression VisitFunctionDefinition(FunctionDefinitionExpression node) {
            node.Arguments.ForEach(arg => {
                if (arg.HasDefault) {
                    Visit(arg.DefaultValue);
                }
            });
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitIf(IfExpression node) {
            Visit(node.Test);
            Visit(node.IfTrue);
            Visit(node.IfFalse);
            return node;
        }

        protected virtual Expression VisitInclude(IncludeExpression node) {
            return node;
        }

        protected virtual Expression VisitInstanceReference(InstanceReferenceExpression node) {
            Visit(node.LValue);
            Visit(node.Key);
            return node;
        }

        protected virtual Expression VisitInvoke(InvokeExpression node) {
            Visit(node.TargetType);
            Visit(node.Method);
            node.Arguments.ForEach(arg => Visit(arg.Value));
            return node;
        }

        protected virtual Expression VisitKeyValuePair(KeyValuePairExpression node) {
            Visit(node.Key);
            Visit(node.Value);
            return node;
        }

        protected virtual Expression VisitLeftHandValue(LeftHandValueExpression node) {
            Visit(node.Value);
            return node;
        }

        protected virtual Expression VisitModuleDefinition(ModuleDefinitionExpression node) {
            node.Contents.ForEach(content => Visit(content));
            return node;
        }

        protected virtual Expression VisitParallelAssignmentExpression(ParallelAssignmentExpression node) {
            return node;
        }

        protected virtual Expression VisitPuts(PutsExpression node) {
            Visit(node.Value);
            return node;
        }

        protected virtual Expression VisitRescue(RescueExpression node)
        {
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitReturn(ReturnExpression node) {
            node.Arguments.ForEach(arg => Visit(arg.Value));
            return node;
        }

        protected virtual Expression VisitSetAssign(SetAssignExpression node) {
            Visit(node.Left);
            Visit(node.Right);
            return node;
        }

        protected virtual Expression VisitSingletonDefinition(SingletonDefinitionExpression node) {
            Visit(node.Singleton);
            node.Arguments.ForEach(arg => {
                if (arg.HasDefault) {
                    Visit(arg.DefaultValue);
                }
            });
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitString(StringExpression node) {
            return node;
        }

        protected virtual Expression VisitSwitch(SwitchExpression node) {
            Visit(node.Test);
            node.Cases.ForEach(c => {
                Visit(c.Body);
                c.TestValues.ToList().ForEach(t => Visit(t));
            });
            if (node.DefaultBody != null) {
                Visit(node.DefaultBody);
            }
            return node;
        }

        protected virtual Expression VisitSync(SyncExpression node)
        {
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitThrow(ThrowExpression node)
        {
            Visit(node.ExceptionObject);
            return node;
        }

        protected virtual Expression VisitTypeof(TypeofExpression node) {
            Visit(node.Expression);
            return node;
        }

        protected virtual Expression VisitUnary(UnaryExpression node) {
            Visit(node.Expression);
            return node;
        }

        protected virtual Expression VisitUnless(UnlessExpression node) {
            Visit(node.Test);
            Visit(node.IfTrue);
            Visit(node.IfFalse);
            return node;
        }

        protected virtual Expression VisitUntil(UntilExpression node) {
            Visit(node.Test);
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitVariable(VariableExpression node) {
            return node;
        }

        protected virtual Expression VisitWhile(WhileExpression node) {
            Visit(node.Test);
            Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitYield(YieldExpression node) {
            node.Arguments.ForEach(arg => Visit(arg.Value));
            return node;
        }

        private static Expression VisitIf(Expression node, Type type, Func<Expression, Expression> func)
        {
            return node.GetType() == type ? func(node) : node;
        }

        protected override sealed Expression VisitExtension(Expression node) {
            node = VisitIf(node, typeof (AccessExpression), myNode => VisitAccess((AccessExpression) myNode));

            node = VisitIf(node, typeof (AccessSetExpression), myNode => VisitAccessSet((AccessSetExpression) myNode));

            node = VisitIf(node, typeof (AssignmentExpression), myNode => VisitAssignment((AssignmentExpression) myNode));

            node = VisitIf(node, typeof (BeginExpression), myNode => VisitBegin((BeginExpression) myNode));

            node = VisitIf(node, typeof (BinaryExpression), myNode => VisitBinary((BinaryExpression) myNode));

            node = VisitIf(node, typeof (BlockExpression), myNode => VisitBlock((BlockExpression) myNode));

            node = VisitIf(node, typeof (BooleanExpression), myNode => VisitBoolean((BooleanExpression) myNode));

            node = VisitIf(node, typeof (ClassDefinitionExpression),
                myNode => VisitClassDefinition((ClassDefinitionExpression) myNode));

            node = VisitIf(node, typeof (ClassOpenExpression),
                myNode => VisitClassOpen((ClassOpenExpression) myNode));

            node = VisitIf(node, typeof (ConditionalAccessSetExpression),
                myNode => VisitConditionalAccessSet((ConditionalAccessSetExpression) myNode));

            node = VisitIf(node, typeof (ConditionalAssignmentExpression),
                myNode => VisitConditionalAssignment((ConditionalAssignmentExpression) myNode));

            node = VisitIf(node, typeof (ConvertExpression), myNode => VisitConvert((ConvertExpression) myNode));

            node = VisitIf(node, typeof (CreateArrayExpression),
                myNode => VisitCreateArray((CreateArrayExpression) myNode));

            node = VisitIf(node, typeof (CreateDictionaryExpression),
                myNode => VisitCreateDictionary((CreateDictionaryExpression) myNode));

            node = VisitIf(node, typeof (DoUntilExpression), myNode => VisitDoUntil((DoUntilExpression) myNode));

            node = VisitIf(node, typeof (DoWhileExpression), myNode => VisitDoWhile((DoWhileExpression) myNode));

            node = VisitIf(node, typeof (ForExpression), myNode => VisitFor((ForExpression) myNode));

            node = VisitIf(node, typeof (ForInExpression), myNode => VisitForIn((ForInExpression) myNode));

            node = VisitIf(node, typeof (FunctionCallExpression),
                myNode => VisitFunctionCall((FunctionCallExpression) myNode));

            node = VisitIf(node, typeof (FunctionDefinitionExpression),
                myNode => VisitFunctionDefinition((FunctionDefinitionExpression) myNode));

            node = VisitIf(node, typeof (IfExpression), myNode => node = VisitIf((IfExpression) myNode));

            node = VisitIf(node, typeof (IncludeExpression), myNode => node = VisitInclude((IncludeExpression) myNode));

            node = VisitIf(node, typeof (InstanceReferenceExpression),
                myNode => node = VisitInstanceReference((InstanceReferenceExpression) myNode));

            node = VisitIf(node, typeof (InvokeExpression), myNode => node = VisitInvoke((InvokeExpression) myNode));

            node = VisitIf(node, typeof (KeyValuePairExpression),
                myNode => VisitKeyValuePair((KeyValuePairExpression) myNode));

            node = VisitIf(node, typeof (LeftHandValueExpression),
                myNode => VisitLeftHandValue((LeftHandValueExpression) myNode));

            node = VisitIf(node, typeof (ModuleDefinitionExpression),
                myNode => VisitModuleDefinition((ModuleDefinitionExpression) myNode));

            node = VisitIf(node, typeof (PutsExpression), myNode => VisitPuts((PutsExpression) myNode));

            node = VisitIf(node, typeof (RescueExpression), myNode => VisitRescue((RescueExpression) myNode));

            node = VisitIf(node, typeof (ReturnExpression), myNode => VisitReturn((ReturnExpression) myNode));

            node = VisitIf(node, typeof (SetAssignExpression),
                myNode => VisitSetAssign((SetAssignExpression) myNode));

            node = VisitIf(node, typeof (SingletonDefinitionExpression),
                myNode => VisitSingletonDefinition((SingletonDefinitionExpression) myNode));

            node = VisitIf(node, typeof (StringExpression), myNode => VisitString((StringExpression) myNode));

            node = VisitIf(node, typeof (SwitchExpression), myNode => VisitSwitch((SwitchExpression) myNode));

            node = VisitIf(node, typeof (SyncExpression), myNode => VisitSync((SyncExpression) myNode));

            node = VisitIf(node, typeof (ThrowExpression), myNode => VisitThrow((ThrowExpression) myNode));

            node = VisitIf(node, typeof (TypeofExpression), myNode => VisitTypeof((TypeofExpression) myNode));

            node = VisitIf(node, typeof (UnaryExpression), myNode => VisitUnary((UnaryExpression) myNode));

            node = VisitIf(node, typeof (UnlessExpression), myNode => VisitUnless((UnlessExpression) myNode));

            node = VisitIf(node, typeof (UntilExpression), myNode => VisitUntil((UntilExpression) myNode));

            node = VisitIf(node, typeof (VariableExpression), myNode => VisitVariable((VariableExpression) myNode));

            node = VisitIf(node, typeof (WhileExpression), myNode => VisitWhile((WhileExpression) myNode));

            node = VisitIf(node, typeof (YieldExpression), myNode => VisitYield((YieldExpression) myNode));

            return node;
        }
    }
}