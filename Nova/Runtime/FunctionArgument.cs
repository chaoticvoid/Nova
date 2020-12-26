// -----------------------------------------------------------------------
// <copyright file="FunctionArgument.cs" Company="Michael Tindal">
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

using System.Linq.Expressions;

namespace Nova.Runtime {
    /// <summary>
    ///     The class to represent function arguments to the Nova runtime.  The same class is used by both calls and
    ///     definitions.
    /// </summary>
    public class FunctionArgument {
        public FunctionArgument(string name) : this(name, null) {}

        public FunctionArgument(string name, int index) : this(name, null) {
            Index = index;
        }

        public FunctionArgument(string name, Expression value) {
            Name = name;
            Value = value;
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public bool HasDefault { get; set; }

        public Expression DefaultValue { get; set; }

        public bool IsVarArg { get; set; }

        public bool IsFunction { get; set; }

        public bool IsLiteral { get; set; }

        public Expression Value { get; set; }

        public override string ToString() {
            return
                string.Format(
                    "[FunctionArgument: Index={0}, Name={1}, HasDefault={2}, DefaultValue={3}, IsVarArg={4}, IsFunction={5}, IsLiteral={6}, Value={7}]",
                    Index, Name, HasDefault, DefaultValue, IsVarArg, IsFunction, IsLiteral, Value);
        }

        public override int GetHashCode() {
            return Name.GetHashCode() + base.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            if (!(obj is FunctionArgument)) {
                return false;
            }

            if (this == obj) {
                return true;
            }

            var other = (FunctionArgument) obj;

            if (!Name.Equals(other.Name)) {
                return false;
            }

            if (Value != null) {
                if (!Value.Equals(other.Value)) {
                    return false;
                }
            }

            if (HasDefault) {
                if (!DefaultValue.Equals(other.DefaultValue)) {
                    return false;
                }
            }

            return true;
        }
    }
}