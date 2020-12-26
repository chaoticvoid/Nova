// -----------------------------------------------------------------------
// <copyright file="RuntimeOperations.cs" Company="Michael Tindal">
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
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Nova.Expressions;
using Nova.Builtins;

namespace Nova.Runtime {
    using E = ExpressionType;

    public static class StringExtensions {
        public static string Capitalize(this string @this) {
            return string.Format("{0}{1}", @this.ToUpper().Substring(0, 1), @this.Substring(1));
        }
    }

    /// <summary>
    ///     This class provides the operations Nova needs to operate.  It houses the methods that makes up the IS
    ///     runtime, which works in conjunction with the DLR runtime.
    /// </summary>
    public static partial class RuntimeOperations {
        private static MethodInfo OpToMethod(string op) {
            return typeof (RuntimeOperations).GetMethod(op,
                BindingFlags.NonPublic |
                BindingFlags.Static);
        }

        internal static Expression Op(string op, Type expectedType, params Expression[] args) {
            if (op == "Convert") {
                return Expression.Convert(Expression.Call(null, OpToMethod(op), args), expectedType);
            }
            return NovaExpression.Convert(Expression.Call(null, OpToMethod(op), args), expectedType);
        }


        private static dynamic Binary(object left, object right, E type, object _scope) {
            var args = new List<Expression>();
            args.Add(Expression.Constant(left, typeof (object)));
            args.Add(Expression.Constant(right, typeof (object)));

            return Dynamic(typeof (object), new InteropBinder.Binary((NovaScope) _scope, type),
                args);
        }

        private static FunctionArgument Arg(object val) {
            return val as FunctionArgument ?? new FunctionArgument(null, Expression.Constant(val));
        }

        private static DynamicMetaObject DMO(dynamic val) {
            return DynamicMetaObject.Create(val, Expression.Constant(val));
        }

        private static List<FunctionArgument> L(params FunctionArgument[] args) {
            return new List<FunctionArgument>(args);
        }

        private static DynamicMetaObject[] _DMO(params DynamicMetaObject[] args) {
            return args;
        }

        private static dynamic Compare(object left, object right, object rawScope) {
            var scope = (NovaScope) rawScope;

            var NovaName = "<=>";
            var clrName = InteropBinder.ToClrOperatorName(NovaName);

            if (left is NovaInstance) {
                var lo = (NovaInstance) left;
                var dmo = lo.GetMetaObject(Expression.Constant(left));
                if (InteropBinder.InvokeMember.SearchForFunction(lo.Class, NovaName, lo, L(Arg(right)), true) != null) {
                    return dmo.BindInvokeMember(new InteropBinder.InvokeMember(NovaName, new CallInfo(1), scope),
                        _DMO(DMO(scope), DMO(Arg(right))));
                }
                if (InteropBinder.InvokeMember.SearchForFunction(lo.Class, clrName, lo, L(Arg(right)), true) != null) {
                    return dmo.BindInvokeMember(new InteropBinder.InvokeMember(clrName, new CallInfo(1), scope),
                        _DMO(DMO(scope), DMO(Arg(right))));
                }
            }

            var Value = Nova.Box(left);
            if (Value.Class != null) {
                var _dmo = Value.GetMetaObject(Expression.Constant(Value));
                if (InteropBinder.InvokeMember.SearchForFunction(Value.Class, NovaName, Value, L(Arg(right)), true) !=
                    null) {
                    return _dmo.BindInvokeMember(new InteropBinder.InvokeMember(NovaName, new CallInfo(1), scope),
                        _DMO(DMO(scope), DMO(Arg(right))));
                }
                if (InteropBinder.InvokeMember.SearchForFunction(Value.Class, clrName, Value, L(Arg(right)), true) !=
                    null) {
                    return _dmo.BindInvokeMember(new InteropBinder.InvokeMember(clrName, new CallInfo(1), scope),
                        _DMO(DMO(scope), DMO(Arg(right))));
                }
            }

            dynamic _left = left;
            dynamic _right = right;
            if (_left < _right) {
                return -1;
            }

            if (_left > _right) {
                return 1;
            }

            return 0;
        }

        private static dynamic Match(object rawLeft, object rawRight, NovaExpressionType novaBinaryNodeType, object rawScope)
        {
            var scope = (NovaScope)rawScope;

            var left = CompilerServices.CompileExpression((Expression) rawLeft, scope);
            var right = CompilerServices.CompileExpression((Expression) rawRight, scope);

            var NovaName = "=~";
            var clrName = InteropBinder.ToClrOperatorName(NovaName);

            if (left is NovaInstance)
            {
                var lo = (NovaInstance)left;
                DynamicMetaObject dmo = lo.GetMetaObject(Expression.Constant(left));
                if (InteropBinder.InvokeMember.SearchForFunction(lo.Class, NovaName, lo, L(Arg(right)), true) != null)
                {
                    if (novaBinaryNodeType == NovaExpressionType.NotMatch)
                    {
                        return !dmo.BindInvokeMember(new InteropBinder.InvokeMember(NovaName, new CallInfo(1), scope),
                            _DMO(DMO(scope), DMO(Arg(right))));
                    }
                    return dmo.BindInvokeMember(new InteropBinder.InvokeMember(NovaName, new CallInfo(1), scope),
                        _DMO(DMO(scope), DMO(Arg(right))));
                }
                if (InteropBinder.InvokeMember.SearchForFunction(lo.Class, clrName, lo, L(Arg(right)), true) != null)
                {
                    if (novaBinaryNodeType == NovaExpressionType.NotMatch)
                    {
                        return !dmo.BindInvokeMember(new InteropBinder.InvokeMember(clrName, new CallInfo(1), scope),
                            _DMO(DMO(scope), DMO(Arg(right))));
                    }
                    return dmo.BindInvokeMember(new InteropBinder.InvokeMember(clrName, new CallInfo(1), scope),
                        _DMO(DMO(scope), DMO(Arg(right))));
                }
            }

            var Value = Nova.Box(left);
            if (Value.Class != null)
            {
                var _dmo = Value.GetMetaObject(Expression.Constant(Value));
                if (InteropBinder.InvokeMember.SearchForFunction(Value.Class, NovaName, Value, L(Arg(right)), true) !=
                    null)
                {
                    if (novaBinaryNodeType == NovaExpressionType.NotMatch)
                    {
                        return !_dmo.BindInvokeMember(new InteropBinder.InvokeMember(NovaName, new CallInfo(1), scope),
                            _DMO(DMO(scope), DMO(Arg(right))));
                    }
                    return _dmo.BindInvokeMember(new InteropBinder.InvokeMember(NovaName, new CallInfo(1), scope),
                        _DMO(DMO(scope), DMO(Arg(right))));
                }
                if (InteropBinder.InvokeMember.SearchForFunction(Value.Class, clrName, Value, L(Arg(right)), true) !=
                    null)
                {
                    if (novaBinaryNodeType == NovaExpressionType.NotMatch)
                    {
                        return !_dmo.BindInvokeMember(new InteropBinder.InvokeMember(clrName, new CallInfo(1), scope),
                            _DMO(DMO(scope), DMO(Arg(right))));
                    }
                    return _dmo.BindInvokeMember(new InteropBinder.InvokeMember(clrName, new CallInfo(1), scope),
                        _DMO(DMO(scope), DMO(Arg(right))));
                }
            }

            if (!(left is Regex || right is Regex))
            {
                return null;
            }

            var left1 = left as Regex;
            var regex = left1 ?? (Regex)right;

            var str = (left is Regex) ? (string) right : (string)left;

            if (!regex.Match(str).Success) return novaBinaryNodeType == NovaExpressionType.NotMatch;
            var groups = regex.Match(str).Groups;

            foreach (var groupName in regex.GetGroupNames())
            {
                scope[groupName] = groups[groupName].Value;
            }


            return novaBinaryNodeType == NovaExpressionType.Match;
        }

        private static dynamic Unary(object expr, E type, object _scope) {
            var args = new List<Expression>();
            args.Add(Expression.Constant(expr, typeof (object)));

            return Dynamic(typeof (object), new InteropBinder.Unary((NovaScope) _scope, type), args);
        }

        private static dynamic Dynamic(Type returnType, CallSiteBinder binder, IEnumerable<Expression> args) {
            return CompilerServices.CreateLambdaForExpression(Expression.Dynamic(binder, returnType, args.ToArray()))();
        }

        private static dynamic Boolean(object value) {
            var numberTypes = new[] {
                typeof (short),
                typeof (ushort),
                typeof (int),
                typeof (uint),
                typeof (long),
                typeof (ulong),
                typeof (float),
                typeof (double),
                typeof (decimal)
            };
            if (value == null) {
                return false;
            }
            if (value is NovaNumber)
            {
                value = NovaNumber.Convert((NovaNumber)value);
            }
            if (value is bool) {
                return (bool) value;
            }
            if (numberTypes.Contains(value.GetType())) {
                dynamic _value = value;
                return _value != 0;
            }
            return value != null;
        }

        private static dynamic TryConvert<T>(object value) {
            try {
                return (T) value;
            }
            catch {
                // Workarounds for known conversion problems.

                // double->int
                if (value is double && typeof (T) == typeof (int)) {
                    return (int) (double) value;
                }
                if (value is int && typeof (T) == typeof (double)) {
                    // int->double
                    return (double) (int) value;
                }
                return null;
            }
        }

        internal static dynamic Convert(object value, Type type) {
            var tryConvert =
                typeof (RuntimeOperations).GetMethod("TryConvert", BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(type);

            return tryConvert.Invoke(null, new[] {value});
        }

        internal static dynamic ConvertIfNumber(object value)
        {
            var number = value as NovaNumber;
            return number != null ? NovaNumber.Convert(number) : value;
        }
    }
}