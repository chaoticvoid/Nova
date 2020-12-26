using System.Text;
using Nova.Runtime;

namespace Nova.Builtins {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    [NovaExport("String")]
    public class NovaString {
        public NovaString() {
            _internal = new StringBuilder();
        }

        public NovaString(string @string) {
            _internal = new StringBuilder(@string);
        }

        private StringBuilder _internal { get; set; }

        public override int GetHashCode() {
            return _internal.ToString().GetHashCode();
        }

        public override string ToString() {
            return _internal.ToString();
        }

        public override bool Equals(object obj) {
            if (obj is NovaString) {
                return ((NovaString) obj)._internal.ToString() == _internal.ToString();
            }
            if (obj is string) {
                return (string) obj == _internal.ToString();
            }
            return false;
        }

        [NovaExport("<<")]
        public void StringAdd(dynamic val) {
            _internal.Append((string) val);
        }

        public static implicit operator string(NovaString s) {
            return s._internal.ToString();
        }

        public static implicit operator NovaString(string s) {
            return new NovaString(s);
        }
    }
}