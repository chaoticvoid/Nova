// -----------------------------------------------------------------------
// <copyright file="DynamicScope.cs" Company="Michael Tindal">
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
using System.Text;
using System.Xml.Schema;
using Nova.Builtins;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;

namespace Nova.Runtime {
    public static class DictionaryExtensions {
        // Works in C#3/VS2008:
        // Returns a new dictionary of this ... others merged leftward.
        // Keeps the type of 'this', which must be default-instantiable.
        // Example: 
        //   result = map.MergeLeft(other1, other2, ...)
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new() {
            var newMap = new T();
            foreach (var src in
                (new List<IDictionary<K, V>> {me}).Concat(others)) {
                // ^-- echk. Not quite there type-system.
                foreach (var p in src) {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }
    }

    /// <summary>
    ///     This class is the scope we use for managing variables.
    /// </summary>
    public class NovaScope {
        public static readonly Dictionary<string, Symbol> Symbols = new Dictionary<string, Symbol>();

        private static int scope_id;

        public readonly int ScopeId = scope_id++;

        public NovaScope(NovaScope parent) {
            Variables = new Dictionary<string, dynamic>();
            SymVars = new Dictionary<Symbol, dynamic>();
            ParentScope = parent;
            Aliases = new Dictionary<string, string>();
            Constants = new List<string>();
        }

        public NovaScope(ScriptScope scope)
            : this((NovaScope) null) {
            MergeWithScope(scope);
        }

        public NovaScope() : this((NovaScope) null) {}

        internal NovaScope(List<string> constants) : this((NovaScope) null) {
            Constants = constants;
        }

        internal NovaScope GlobalScope {
            get {
                if (ParentScope == null) {
                    return this;
                }
                return ParentScope.GlobalScope;
            }
        }

        internal List<string> Constants { get; private set; }

        internal Dictionary<string, dynamic> Variables { get; private set; }

        internal Dictionary<Symbol, dynamic> SymVars { get; private set; }

        internal Dictionary<string, string> Aliases { get; private set; }

        public dynamic this[string name] {
            get { return Resolve(name); }
            set {
                if (CheckConstant(name)) {
                    throw new ConstantException(
                        string.Format("{0} is already defined as a constant in this scope or a parent scope.", name));
                }
                var val = value;
                if (val is string) {
                    val = new NovaString(val);
                }
                if (NovaNumber.IsConvertable(val))
                {
                    val = new NovaNumber(val);
                }
                Variables[name] = val;
            }
        }

        public dynamic this[Symbol sym] {
            get { return Resolve(sym, this); }
            set {
                var val = value;
                if (val is string) {
                    val = new NovaString(val);
                }
                if (NovaNumber.IsConvertable(val))
                {
                    val = new NovaNumber(val);
                }
                SymVars[sym] = val;
            }
        }

        public NovaScope ParentScope { get; set; }

        public void MergeWithScope(Scope scope) {
            List<KeyValuePair<string, dynamic>> items = scope.Storage.GetItems();

            foreach (var item in items) {
                if (!Variables.ContainsKey(item.Key)) {
                    var val = item.Value;
                    // Runtime classes we should wrap to get desired effect
                    if (val is NovaArray || val is NovaDictionary || val is NovaString || val is NovaNumber) {
                        val = Nova.Box(val);
                    }
                    Variables[item.Key] = val;
                }
            }
        }

        public void MergeWithScope(ScriptScope scope) {
            scope.GetVariableNames().ToList().ForEach(name => {
                if (!Variables.ContainsKey(name)) {
                    var val = scope.GetVariable(name);
                    // Runtime classes we should wrap to get desired effect
                    if (val is NovaArray || val is NovaDictionary || val is NovaString || val is NovaNumber) {
                        val = Nova.Box(val);
                    }
                    Variables[name] = val;
                }
            });
        }

        public void MergeWithScope(NovaScope other) {
            Variables = other.Variables.MergeLeft(Variables);
            SymVars = other.SymVars.MergeLeft(SymVars);
        }

        internal void MergeIntoScope(Scope scope) {
            // Exit scope
            foreach (var @var in Variables) {
                var val = @var.Value;
                if (val is NovaInstance) {
                    var so = (NovaInstance) val;
                    if (so is NovaBoxedInstance) {
                        val = ((NovaBoxedInstance) so).BoxedObject;
                    }
                }
                scope.Storage[@var.Key] = val;
            }
        }

        private bool CheckAliases(string name) {
            if (Aliases.ContainsKey(name)) {
                return true;
            }
            if (ParentScope != null) {
                return ParentScope.CheckAliases(name);
            }
            return false;
        }

        private bool CheckConstant(string name) {
            if (Constants.Contains(name)) {
                return true;
            }
            if (ParentScope != null) {
                return ParentScope.CheckConstant(name);
            }
            return false;
        }

        private NovaScope GetAlias(string name) {
            if (Aliases.ContainsKey(name)) {
                return this;
            }
            if (ParentScope != null) {
                return ParentScope.GetAlias(name);
            }
            return null;
        }

        internal void AddAlias(string from, string to) {
            if (GetAlias(to) != null || CheckConstant(to) || GetAlias(from) != null || Resolve(from) == null ||
                Variables.ContainsKey(to)) {
                return;
            }
            Aliases[to] = from;
            Constants.Add(to);
        }

        private dynamic Resolve(string name) {
            if (CheckAliases(name)) {
                var scope = GetAlias(name);
                return scope.Resolve(scope.Aliases[name]);
            }
            if (Variables.ContainsKey(name)) {
                return Variables[name];
            }
            if (ParentScope != null) {
                return ParentScope.Resolve(name);
            }
            return null;
        }

        private dynamic Resolve(Symbol sym, NovaScope startingScope) {
            if (SymVars.ContainsKey(sym)) {
                return SymVars[sym];
            }
            if (ParentScope != null) {
                return ParentScope.Resolve(sym, startingScope);
            }
            return startingScope.Resolve(sym.Name);
        }

        public void PrintVariables() {
            PrintVariables(false);
        }

        private string WasParentString(bool wasParent, int depth = 0) {
            if (wasParent) {
                var sb = new StringBuilder("");
                for (var i = 0; i < depth; i++) {
                    sb.Append("  ");
                }
                return sb.ToString();
            }
            return "";
        }

        private void PrintVariables(bool wasParent, int depth = 0) {
#if DEBUG
            if (!wasParent) {
                Console.WriteLine("---BEGIN---");
            }
            Console.WriteLine("{0}Scope {1}", WasParentString(wasParent, depth), ScopeId);
            foreach (var p in Variables) {
                Console.WriteLine("{0}{1} => {2}", WasParentString(wasParent, depth + 1), p.Key, p.Value);
            }
            if (ParentScope != null) {
                ParentScope.PrintVariables(true, depth + 1);
            }
            else {
                Console.WriteLine("---END---");
            }
#endif
        }

        internal NovaScope SearchForObject(object obj, out string name) {
            foreach (var p in Variables) {
                if ((object) p.Value == obj && p.Key != "self" && p.Key != "super") {
                    name = p.Key;
                    return this;
                }
            }
            if (ParentScope != null) {
                return ParentScope.SearchForObject(obj, out name);
            }
            name = null;
            return null;
        }

        public override string ToString() {
            return string.Format("[Scope {0}]", ScopeId);
        }

        public static implicit operator Scope(NovaScope @this) {
            var scope = new Scope();
            @this.MergeIntoScope(scope);
            return scope;
        }

        public static implicit operator ScriptScope(NovaScope @this) {
            var engine = Nova.CreateRuntime().GetEngine("sv");
            var scope = engine.CreateScope();

            foreach (var @var in @this.Variables) {
                scope.SetVariable(@var.Key, @var.Value);
            }

            return scope;
        }
    }
}