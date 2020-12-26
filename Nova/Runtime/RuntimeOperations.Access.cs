// -----------------------------------------------------------------------
// <copyright file="RuntimeOperations.Access.cs" Company="Michael Tindal">
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

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Nova.Expressions;
using Nova.Builtins;

namespace Nova.Runtime {
    using E = ExpressionType;

    /// <summary>
    ///     This class provides the operations Nova needs to operate.  It houses the methods that makes up the IS
    ///     runtime, which works in conjunction with the DLR runtime.
    /// </summary>
    public static partial class RuntimeOperations {
        internal static dynamic Access(object container, List<FunctionArgument> args, object rawScope) {
            var names = new List<string>();
            for (var i = 0; i < args.Count; i++) {
                names.Add(string.Format("index{0}", i));
            }
            var scope = rawScope as NovaScope;
            var realArgs = new List<object>();
            args.ForEach(arg => realArgs.Add(CompilerServices.CompileExpression(arg.Value, scope)));

            var eArgs = new List<Expression>();
            eArgs.Add(Expression.Constant(container, typeof (object)));
            realArgs.ForEach(arg => eArgs.Add(Expression.Constant(arg)));
            return Dynamic(typeof (object),
                new InteropBinder.GetIndex(scope, new CallInfo(args.Count, names)), eArgs);
        }

        internal static dynamic AccessSet(object container, List<FunctionArgument> args, object value, E type,
            object rawScope) {
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

            var names = new List<string>();
            for (var i = 0; i < args.Count; i++) {
                names.Add(string.Format("index{0}", i));
            }

            var scope = rawScope as NovaScope;
            var realArgs = new List<object>();
            args.ForEach(arg => realArgs.Add(CompilerServices.CompileExpression(arg.Value, scope)));


            if (map.ContainsKey(type)) {
                var nvalue =
                    CompilerServices.CreateLambdaForExpression(
                        NovaExpression.Binary(Expression.Constant(Access(container, args, scope)),
                            Expression.Constant(value),
                            map[type]))();
                value = nvalue;
            }

            if (incDecMap.Contains(type)) {
                if (type == E.PostIncrementAssign || type == E.PostDecrementAssign) {
                    var val = Access(container, args, scope);
                    AccessSet(container, args, 1, type == E.PostIncrementAssign ? E.AddAssign : E.SubtractAssign,
                        rawScope);
                    return val;
                }
                AccessSet(container, args, 1, type == E.PreIncrementAssign ? E.AddAssign : E.SubtractAssign, rawScope);
                return Access(container, args, scope);
            }

            var eArgs = new List<Expression>();
            eArgs.Add(Expression.Constant(container, typeof (object)));
            realArgs.ForEach(arg => eArgs.Add(Expression.Constant(arg)));
            eArgs.Add(Expression.Constant(value, typeof (object)));
            return Dynamic(typeof (object),
                new InteropBinder.SetIndex(scope, new CallInfo(args.Count, names)), eArgs);
        }

        internal static dynamic ConditionalAccessSet(object container, List<FunctionArgument> args, object value,
            NovaExpressionType conditionalAssignmentType, object rawScope) {
            var scope = rawScope as NovaScope;
            var v = Access(container, args, scope);
            if (Boolean(v)) {
                if (conditionalAssignmentType == NovaExpressionType.IfNotNullAssign) {
                    return AccessSet(container, args, value, E.Assign, scope);
                }
            }
            else {
                if (conditionalAssignmentType == NovaExpressionType.IfNullAssign) {
                    return AccessSet(container, args, value, E.Assign, scope);
                }
            }
            return v;
        }

        internal static dynamic CreateArray(List<Expression> values) {
            var list = new List<object>();
            values.ForEach(value => list.Add(CompilerServices.CreateLambdaForExpression(value)()));
            return new NovaArray(list);
        }

        internal static dynamic CreateDictionary(IEnumerable<Expression> values) {
            var dict = new Dictionary<object, object>();
            foreach (var val in values.Select(_val => CompilerServices.CreateLambdaForExpression(_val)()))
            {
                dict[((KeyValuePair<object, object>) val).Key] = ((KeyValuePair<object, object>) val).Value;
            }
            return new NovaDictionary(dict);
        }

        internal static dynamic KeyValuePair(object key, object value) {
            return new KeyValuePair<object, object>(key, value);
        }
    }
}