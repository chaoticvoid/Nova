// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nova.Builtins;

// <copyright file="NovaMethodTable.cs" Company="Michael Tindal">
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
    ///     Table used for overloading methods.
    /// </summary>
    public partial class NovaMethodTable : NovaFunction {
        private readonly List<NovaFunction> _functions;

        public NovaMethodTable(string name) : base(name, null, null, null) {
            _functions = new List<NovaFunction>();
        }

        internal List<NovaFunction> Functions {
            get { return _functions; }
        }

        public void AddFunction(NovaFunction function)
        {
            NovaFunction funcToRemove = null;
            _functions.ForEach(func =>
            {
                if (func.Name.Equals(function.Name) && func.Arguments.Count == function.Arguments.Count && !(function is NovaNativeFunction))
                {
                    funcToRemove = func;
                }
            });
            if (funcToRemove != null)
                _functions.Remove(funcToRemove);
            _functions.Add(function);
        }

        internal NovaFunction Resolve(List<FunctionArgument> args, bool exact_match = false) {
            if (!_functions.Any()) {
                return null;
            }
            if (_functions.Count == 1 && !exact_match) {
                return _functions[0];
            }
            var q = _functions.Where(f => f.Arguments.Count == args.Count);
            IEnumerable<NovaFunction> fq;
            IEnumerable<NovaFunction> rq;
            if (q.Any()) {
                if (q.Count() == 1) {
                    return q.First();
                }
                var nq = _functions.Where(f => CheckForNameMatch(f, args));
                if (nq.Any()) {
                    return nq.First();
                }
                fq = _functions.Where(f => f is NovaNativeFunction)
                    .Where(f => CheckForMatch(f as NovaNativeFunction, args));
                if (fq.Any()) {
                    var _fq = fq.Where(f => CheckForNameMatch(f, args));
                    if (_fq.Any()) {
                        return _fq.First();
                    }
                    return fq.First();
                }
                rq = _functions.Where(f => CheckForMatch(f, args));
                if (rq.Any()) {
                    var _rq = rq.Where(f => CheckForNameMatch(f, args));
                    if (_rq.Any()) {
                        return _rq.First();
                    }
                    return rq.First();
                }
                return null;
            }
            fq = _functions.Where(f => f is NovaNativeFunction)
                .Where(f => CheckForMatch(f as NovaNativeFunction, args));
            if (fq.Any()) {
                var _fq = fq.Where(f => CheckForNameMatch(f, args));
                if (_fq.Any()) {
                    return _fq.First();
                }
                return fq.First();
            }
            rq = _functions.Where(f => CheckForMatch(f, args));
            if (rq.Any()) {
                var _rq = rq.Where(f => CheckForNameMatch(f, args));
                if (_rq.Any()) {
                    return _rq.First();
                }
                return rq.First();
            }
            return null;
        }

        private bool CheckForNameMatch(NovaFunction function, List<FunctionArgument> args) {
            var match = false;
            for (var i = 0; i < args.Count; i++) {
                if (args[i].Name != null) {
                    var nameMatch = function.Arguments.Where(arg => arg.Name == args[i].Name);
                    if (!nameMatch.Any()) {
                        return false;
                    }
                    match = true;
                }
            }
            return match;
        }

        private bool CheckForMatch(NovaFunction function, List<FunctionArgument> args) {
            if (args.Count == function.Arguments.Count) {
                return true;
            }
            if (args.Count > function.Arguments.Count) {
                if (function.Arguments.Any() && function.Arguments.Last().IsVarArg) {
                    return true;
                }
                return false;
            }
            var myCount = args.Count;
            var theirCount = function.Arguments.Count;
            function.Arguments.ForEach(arg => {
                if (arg.HasDefault) {
                    theirCount--;
                }
            });
            var vo = 0;
            if (function.Arguments.Any() && function.Arguments.Last().IsVarArg) {
                vo = 1;
            }
            if (myCount == theirCount) {
                return true;
            }
            if (myCount + vo == theirCount) {
                return true;
            }
            return false;
        }

        private bool CheckForMatch(NovaNativeFunction function, List<FunctionArgument> args) {
            if (function.Arguments.Count == args.Count) {
                var _args = new List<object>();
                args.ForEach(arg => {
                    var val = CompilerServices.CreateLambdaForExpression(arg.Value)();
                    if (val is NovaString) {
                        val = (string) val;
                    }
                    if (val is NovaNumber)
                    {
                        val = NovaNumber.Convert((NovaNumber)val);
                    }
                    _args.Add(val);
                });
                var match = true;
                var i = 0;
                foreach (var param in function.Method.GetParameters()) {
                    if (_args[i++].GetType() != param.ParameterType) {
                        match = false;
                        break;
                    }
                }
                return match;
            }
            if (args.Count > function.Arguments.Count) {
                if (function.Arguments.Any() && function.Arguments.Last().IsVarArg) {
                    return true;
                }
                return false;
            }
            var myCount = args.Count;
            var theirCount = function.Arguments.Count;
            function.Arguments.ForEach(arg => {
                if (arg.HasDefault) {
                    theirCount--;
                }
            });
            var vo = 0;
            if (function.Arguments.Any() && function.Arguments.Last().IsVarArg) {
                vo = 1;
            }
            if (myCount == theirCount) {
                return true;
            }
            if (myCount + vo == theirCount) {
                return true;
            }
            return false;
        }

        public override string ToString() {
            var sb = new StringBuilder(string.Format("[NovaMethodTable]: {0} ({1})", Name, _functions.Count));
            sb.AppendLine();
            _functions.ForEach(func => {
                sb.AppendFormat("  {0}(", func.Name);
                if (func is NovaNativeFunction) {
                    var snf = (NovaNativeFunction) func;
                    var sep = "";
                    foreach (var p in snf.Method.GetParameters()) {
                        sb.AppendFormat("{0}{1} {2}", sep, p.ParameterType.Name, p.Name);
                        sep = ",";
                    }
                }
                else {
                    var sep = "";
                    foreach (var p in func.Arguments) {
                        sb.AppendFormat("{0}{1}", sep, p.Name);
                        sep = ",";
                    }
                }
                sb.AppendLine(")");
            });
            return sb.ToString();
        }
    }
}