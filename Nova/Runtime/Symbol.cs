// -----------------------------------------------------------------------
// <copyright file="Symbol.cs" Company="Michael Tindal">
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

namespace Nova.Runtime {
    /// <summary>
    ///     Symbols are like variable names except they are unique in any given execution environment.
    /// </summary>
    public class Symbol {
        private Symbol(string name) {
            Name = name;
        }

        public string Name { get; set; }

        public static Symbol NewSymbol(string name) {
            Symbol sym;
            if (NovaScope.Symbols.ContainsKey(name)) {
                sym = NovaScope.Symbols[name];
            }
            else {
                sym = new Symbol(name);
                NovaScope.Symbols[name] = sym;
            }
            return sym;
        }
    }
}