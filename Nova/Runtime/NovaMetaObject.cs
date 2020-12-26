// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using Nova.Expressions;

// <copyright file="NovaMetaObject.cs" Company="Michael Tindal">
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
/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * ironruby@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

namespace Nova.Runtime {
    public interface INovaDynamicMetaObjectProvider : IDynamicMetaObjectProvider, IScopeExpression {}

    public abstract class NovaMetaObject : DynamicMetaObject, IScopeExpression {
        private NovaScope _scope;

        #region IScopeExpression implementation

        public void SetScope(NovaScope scope) {
            _scope = scope;
        }

        public NovaScope Scope {
            get { return _scope; }
        }

        #endregion

        internal NovaMetaObject(Expression expression, BindingRestrictions restrictions, object value)
            : base(expression, restrictions, value) {}

        internal static BindingRestrictions GetRes() {
            return
                BindingRestrictions.GetExpressionRestriction(Expression.Equal(new TrueOnce(), Expression.Constant(true)));
        }

        public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args) {
            return InteropBinder.Invoke.Bind(new InteropBinder.Invoke(Scope, binder.CallInfo), this, args);
        }

        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes) {
            return InteropBinder.GetIndex.Bind(new InteropBinder.GetIndex(Scope, binder.CallInfo), this, indexes);
        }

        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes,
            DynamicMetaObject value) {
            return InteropBinder.SetIndex.Bind(new InteropBinder.SetIndex(Scope, binder.CallInfo), this, indexes, value);
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

        public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder) {
            if (Value is NovaInstance) {
                // Check to see which method we should try to call
                var NovaName = InteropBinder.MapExpressionType(binder.Operation);

                if (NovaName == null) {
                    return InteropBinder.Unary.Bind(binder, this);
                }

                if (
                    InteropBinder.InvokeMember.SearchForFunction(((NovaInstance) Value).Class, NovaName,
                        (NovaInstance) Value, L(), true) != null) {
                    return
                        InteropBinder.InvokeMember.Bind(
                            new InteropBinder.InvokeMember(NovaName, new CallInfo(0), Scope), this, _DMO(DMO(Scope)));
                }
                var clrName = InteropBinder.ToClrOperatorName(NovaName);
                if (
                    InteropBinder.InvokeMember.SearchForFunction(((NovaInstance) Value).Class, clrName,
                        (NovaInstance) Value, L(), true) != null) {
                    return InteropBinder.Unary.Bind(binder, this);
                }

                return new DynamicMetaObject(Expression.Constant(null),
                    BindingRestrictions.GetExpressionRestriction(Expression.Constant(true)));
            }

            return InteropBinder.Unary.Bind(binder, this);
        }

        public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg) {
            if (Value is NovaInstance) {
                var NovaName = InteropBinder.MapExpressionType(binder.Operation);
                if (NovaName == null) {
                    return InteropBinder.Binary.Bind(binder, this, arg);
                }

                if (
                    InteropBinder.InvokeMember.SearchForFunction(((NovaInstance) Value).Class, NovaName,
                        (NovaInstance) Value, L(Arg(arg.Value)), true) != null) {
                    return
                        InteropBinder.InvokeMember.Bind(
                            new InteropBinder.InvokeMember(NovaName, new CallInfo(0), Scope), this,
                            _DMO(DMO(Scope), DMO(Arg(arg.Value))));
                }
                var clrName = InteropBinder.ToClrOperatorName(NovaName);
                if (
                    InteropBinder.InvokeMember.SearchForFunction(((NovaInstance) Value).Class, clrName,
                        (NovaInstance) Value, L(Arg(arg.Value)), true) != null) {
                    return
                        InteropBinder.InvokeMember.Bind(
                            new InteropBinder.InvokeMember(clrName, new CallInfo(0), Scope), this,
                            _DMO(DMO(Scope), DMO(Arg(arg.Value))));
                }
                return InteropBinder.Binary.Bind(binder, this, arg);
            }
            return InteropBinder.Binary.Bind(binder, this, arg);
        }

        private class TrueOnce : NovaExpression {
            private bool _return;

            public TrueOnce() {
                _return = true;
            }

            public override Type Type {
                get { return typeof (bool); }
            }

            public override string ToString() {
                return string.Format("[TrueOnce: Type={0}]", Type);
            }

            public override Expression Reduce() {
                if (_return) {
                    _return = false;
                    return Constant(true);
                }
                return Constant(false);
            }
        }
    }

    public abstract class NovaMetaObject<T> : NovaMetaObject {
        protected NovaMetaObject(Expression expression, BindingRestrictions restrictions, T value)
            : base(expression, restrictions, value) {}

        public new T Value {
            get { return (T) base.Value; }
        }
    }
}