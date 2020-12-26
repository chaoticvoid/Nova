// -----------------------------------------------------------------------
// <copyright file="RuntimeOperations.Variable.cs" Company="Michael Tindal">
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
using System.Linq.Expressions;
using Nova.Expressions;
using Nova.Builtins;

namespace Nova.Runtime {
    using E = ExpressionType;

    public static partial class RuntimeOperations {
        internal static dynamic Resolve(object rawName, object rawScope) {
            if (rawName.GetType() == typeof (InstanceReference)) {
                var iref = (InstanceReference) rawName;
                var lval = CompilerServices.CompileExpression(iref.LValue, (NovaScope) rawScope);
                var gmArgs = new List<Expression>();
                gmArgs.Add(Expression.Constant(lval, typeof (object)));
                return Dynamic(typeof (object), new InteropBinder.GetMember(iref.Key, (NovaScope) rawScope), gmArgs);
            }
            var name = (string) rawName;
            var scope = (NovaScope) rawScope;
            if (name.StartsWith("$") && name != "$:") {
                scope = scope.GlobalScope;
                name = name.Substring(1);
            }
            if (name.StartsWith("@") && scope["<nova_context_invokemember>"] != null) {
                if (name.StartsWith("@@")) {
                    var _val = Resolve("self", scope);
                    if (!(_val is NovaInstance)) {
                        // native object?
                        _val = Nova.Box((object) _val, scope);
                    }
                    var @class = ((NovaInstance) _val).Class;
                    return
                        CompilerServices.CompileExpression(
                            NovaExpression.Variable(NovaExpression.InstanceRef(Expression.Constant(@class),
                                Expression.Constant(name.Substring(2)))), scope);
                }
                return
                    CompilerServices.CompileExpression(
                        NovaExpression.Variable(
                            NovaExpression.InstanceRef(NovaExpression.Variable(Expression.Constant("self")),
                                Expression.Constant(name.Substring(1)))), scope);
            }

            var val = scope[name];
            // The cast is needed here because if we get a non-nullable type (such as System.Int32) the check here will throw an exception.
            // By casting to System.Object we can avoid the exception since it is a boxed value that can be null.
            if ((object) val == null) {
                Type type;
                if ((type = NovaTypeResolver.Resolve(name)) != null) {
                    var @class = NovaClass.BoxClass(type);
                    scope.GlobalScope[@class.Name] = @class;
                    val = @class;
                }
            }
            return val;
        }

        internal static dynamic ResolveSymbol(Symbol sym, object scope) {
            return ((NovaScope) scope)[sym];
        }

        internal static dynamic Assign(VariableExpression @var, dynamic value, E type, bool isConst, object rawScope) {
            var scope = (NovaScope) rawScope;
            var map = new Dictionary<ExpressionType, ExpressionType>();
            map[E.AddAssign] = E.Add;
            map[E.AndAssign] = E.And;
            map[E.DivideAssign] = E.Divide;
            map[E.ExclusiveOrAssign] = E.ExclusiveOr;
            map[E.LeftShiftAssign] = E.LeftShift;
            map[E.ModuloAssign] = E.Modulo;
            map[E.MultiplyAssign] = E.Multiply;
            map[E.OrAssign] = E.Or;
            map[E.PowerAssign] = E.Power;
            map[E.RightShiftAssign] = E.RightShift;
            map[E.SubtractAssign] = E.Subtract;

            var incDecMap = new List<ExpressionType> {
                E.PreIncrementAssign,
                E.PreDecrementAssign,
                E.PostIncrementAssign,
                E.PostDecrementAssign
            };

            if (@var.Name is InstanceReferenceExpression) {
                var iref = CompilerServices.CompileExpression(@var.Name as InstanceReferenceExpression, scope);
                var lval = CompilerServices.CompileExpression(iref.LValue, scope);
                if (map.ContainsKey(type)) {
                    value =
                        CompilerServices.CreateLambdaForExpression(
                            NovaExpression.Binary(
                                Expression.Constant(Resolve(CompilerServices.CompileExpression(iref, scope), scope)),
                                Expression.Constant(value), map[type]))();
                }
                if (incDecMap.Contains(type)) {
                    var gmArgs = new List<Expression>();
                    gmArgs.Add(Expression.Constant(lval, typeof (object)));
                    if (type == E.PreIncrementAssign || type == E.PreDecrementAssign) {
                        var val = Resolve(CompilerServices.CompileExpression(iref, scope), scope);
                        Assign(@var, 1, type == E.PreIncrementAssign ? E.AddAssign : E.SubtractAssign, false, rawScope);
                        return val;
                    }
                    Assign(@var, 1, type == E.PostIncrementAssign ? E.AddAssign : E.SubtractAssign, false, rawScope);
                    return Resolve(CompilerServices.CompileExpression(iref, scope), scope);
                }

                var smArgs = new List<Expression>();
                smArgs.Add(Expression.Constant(lval, typeof (object)));
                smArgs.Add(Expression.Constant(value, typeof (object)));
                return Dynamic(typeof (object), new InteropBinder.SetMember(iref.Key, scope), smArgs);
            }
            if (@var.HasSym) {
                var sym = @var.Sym;

                var symFound = false;
                while (scope.ParentScope != null) {
                    scope = scope.ParentScope;
                    if (scope[sym] != null) {
                        symFound = true;
                        break;
                    }
                }
                if (!symFound) {
                    scope = (NovaScope) rawScope;
                }

                if (map.ContainsKey(type)) {
                    var nvalue =
                        CompilerServices.CreateLambdaForExpression(
                            NovaExpression.Binary(Expression.Constant(ResolveSymbol(sym, scope)),
                                Expression.Constant(value), map[type]))();
                    scope[sym] = nvalue;
                    return nvalue;
                }

                if (incDecMap.Contains(type)) {
                    if (type == E.PreIncrementAssign || type == E.PreDecrementAssign) {
                        var val = ResolveSymbol(sym, scope);
                        Assign(@var, 1, type == E.PreIncrementAssign ? E.AddAssign : E.SubtractAssign, false, rawScope);
                        return val;
                    }
                    Assign(@var, 1, type == E.PostIncrementAssign ? E.AddAssign : E.SubtractAssign, false, rawScope);
                    return ResolveSymbol(sym, scope);
                }

                scope[sym] = value;
                if (isConst) {
                    scope.Constants.Add(sym.Name);
                }
                return value;
            }
            string name = CompilerServices.CompileExpression(@var.Name, scope);
            if (name.StartsWith("$") && name != "$:") {
                scope = scope.GlobalScope;
                name = name.Substring(1);
            }
            var found = false;
            if (name.StartsWith("@")) {
                if (scope["<nova_context_invokemember>"] != null) {
                    var ivar =
                        NovaExpression.Variable(
                            NovaExpression.InstanceRef(NovaExpression.Variable(Expression.Constant("self")),
                                Expression.Constant(name.Substring(1))));
                    if (map.ContainsKey(type)) {
                        value =
                            CompilerServices.CreateLambdaForExpression(
                                NovaExpression.Binary(ivar, Expression.Constant(value), map[type]))();
                    }
                    var assn = NovaExpression.Assign(NovaExpression.LeftHandValue(ivar), Expression.Constant(value));
                    return CompilerServices.CompileExpression(assn, scope);
                }
                found = true;
                name = name.Substring(1);
            }
            if (name == "self")
            {
                if (scope["<nova_context_selfname>"] != null &&
                    scope["<nova_context_selfscope>"] != null &&
                    scope["<nova_context_invokemember>"] != null)
                {
                    name = scope["<nova_context_selfname>"];
                    scope = scope["<nova_context_selfscope>"];
                    found = true;
                }
            }
            while (scope.ParentScope != null && !found) {
                scope = scope.ParentScope;
                if (scope[name] != null) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                scope = (NovaScope) rawScope;
            }

            if (map.ContainsKey(type)) {
                var nvalue =
                    CompilerServices.CreateLambdaForExpression(
                        NovaExpression.Binary(Expression.Constant(Resolve(name, scope)), Expression.Constant(value),
                            map[type]))();
                scope[name] = nvalue;
                return nvalue;
            }

            if (incDecMap.Contains(type)) {
                if (type == E.PostIncrementAssign || type == E.PostDecrementAssign) {
                    var val = Resolve(name, scope);
                    Assign(@var, 1, type == E.PostIncrementAssign ? E.AddAssign : E.SubtractAssign, false, rawScope);
                    return val;
                }
                Assign(@var, 1, type == E.PreIncrementAssign ? E.AddAssign : E.SubtractAssign, false, rawScope);
                return Resolve(name, scope);
            }

            scope[name] = value;
            if (isConst) {
                scope.Constants.Add(name);
            }
            return value;
        }

        internal static dynamic ConditionalAssign(VariableExpression @var, dynamic value, NovaExpressionType conditionalAssignmentType, bool isConst,
            object rawScope) {
            var scope = rawScope as NovaScope;
            var v = Resolve(CompilerServices.CompileExpression(@var.Name, scope), scope);
            if (Boolean(v)) {
                if (conditionalAssignmentType == NovaExpressionType.IfNotNullAssign) {
                    return Assign(@var, value, E.Assign, isConst, scope);
                }
            }
            else {
                if (conditionalAssignmentType == NovaExpressionType.IfNullAssign) {
                    return Assign(@var, value, E.Assign, isConst, scope);
                }
            }
            return v;
        }

        internal static dynamic ParallelAssign(
            List<ParallelAssignmentExpression.ParallelAssignmentInfo> leftHandValues,
            List<ParallelAssignmentExpression.ParallelAssignmentInfo> rightHandValues, object rawScope) {
            var scope = rawScope as NovaScope;
            var rvalues = new List<object>();
            var fval = CompilerServices.CompileExpression(rightHandValues[0].Value as Expression, scope);

            if (fval is List<object> && !rightHandValues[0].IsWildcard) {
                rvalues = new List<object>(fval as List<object>);
            }
            else {
                foreach (var rvalue in rightHandValues) {
                    var val = CompilerServices.CompileExpression(rvalue.Value as Expression, scope);
                    if (rvalue.IsWildcard) {
                        if (val is List<object>) {
                            (val as List<object>).ForEach(value => rvalues.Add(value));
                        }
                        else {
                            rvalues.Add(val);
                        }
                    }
                    else {
                        rvalues.Add(val);
                    }
                }
            }

            var i = 0;
            var k = 0;
            var result = new NovaArray();

            foreach (var _lvalue in leftHandValues) {
                var lvalue = _lvalue.Value;
                if (i >= rvalues.Count) {
                    break;
                }
                k++;
                if (lvalue is List<ParallelAssignmentExpression.ParallelAssignmentInfo>) {
                    result.Add(ParallelAssign(lvalue as List<ParallelAssignmentExpression.ParallelAssignmentInfo>,
                        new List<ParallelAssignmentExpression.ParallelAssignmentInfo> {
                            new ParallelAssignmentExpression.ParallelAssignmentInfo {
                                IsWildcard = false,
                                Value = Expression.Constant(rvalues[i++])
                            }
                        }, scope));
                }
                else if (_lvalue.IsWildcard) {
                    var mvalues = new NovaArray();
                    for (var j = i; j < rvalues.Count; j++) {
                        mvalues.Add(rvalues[j]);
                    }
                    result.Add(Assign(lvalue as VariableExpression, mvalues, E.Assign, false, rawScope));
                    break;
                }
                else {
                    result.Add(Assign(lvalue as VariableExpression, rvalues[i++], E.Assign, false, rawScope));
                }
            }

            if (k < leftHandValues.Count) {
                for (var j = k; j < leftHandValues.Count; j++) {
                    if (leftHandValues[j].Value is List<ParallelAssignmentExpression.ParallelAssignmentInfo>) {
                        var lvalues =
                            leftHandValues[j].Value as List<ParallelAssignmentExpression.ParallelAssignmentInfo>;
                        for (var l = 0; l < lvalues.Count; l++) {
                            result.Add(Assign(lvalues[l].Value as VariableExpression, null, E.Assign, false, scope));
                        }
                    }
                    else {
                        result.Add(Assign(leftHandValues[j].Value as VariableExpression, null, E.Assign, false, scope));
                    }
                }
            }

            return result.Count > 1 ? result : result[0];
        }

        internal static dynamic Alias(object rawFrom, object rawTo, object rawScope) {
            string from, to;
            var scope = (NovaScope) rawScope;
            if (rawFrom is string) {
                from = (string) rawFrom;
            }
            else if (rawFrom is Symbol) {
                from = ((Symbol) rawFrom).Name;
            }
            else {
                return null;
            }
            if (rawTo is string) {
                to = (string) rawTo;
            }
            else if (rawTo is Symbol) {
                to = ((Symbol) rawTo).Name;
            }
            else {
                return null;
            }
            if (from.StartsWith("$")) {
                scope = scope.GlobalScope;
                from = from.Substring(1);
            }
            if (to.StartsWith("$")) {
                scope = scope.GlobalScope;
                to = to.Substring(1);
            }
            scope.AddAlias(from, to);
            return null;
        }

        internal static dynamic Typeof(object value) {
            if (value == null) {
                return typeof (object);
            }
            var val = value.GetType();
            if (val == Type.GetType("System.MonoType") || val == Type.GetType("System.RuntimeType")) {
                val = typeof (Type);
            }
            return val;
        }
    }
}