// -----------------------------------------------------------------------
// <copyright file="KeyValuePairExpression.cs" Company="Michael Tindal">
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
using Nova.Runtime;

namespace Nova.Expressions {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class KeyValuePairExpression : NovaExpression {
        internal KeyValuePairExpression(Expression key, Expression value) {
            Key = key;
            Value = value;
        }

        public Expression Key { get; private set; }

        public Expression Value { get; private set; }

        public override Type Type {
            get { return typeof (KeyValuePair<object, object>); }
        }

        public override Expression Reduce() {
            return Operation.KeyValuePair(typeof (KeyValuePair<object, object>), Convert(Key, typeof (object)),
                Convert(Value, typeof (object)));
        }

        public override string ToString() {
            return string.Format("{0} => {1}", Key, Value);
        }

        public override void SetChildrenScopes(NovaScope scope) {
            Key.SetScope(scope);
            Value.SetScope(scope);
        }
    }
}