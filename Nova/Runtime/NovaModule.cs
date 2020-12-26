using System.Collections.Generic;

namespace Nova.Runtime {
    public class NovaModule {
        public NovaModule(string name, NovaScope context, List<object> contents) {
            Name = name;
            Contents = contents;
            Context = context;
        }

        internal NovaModule() {
            Contents = new List<object>();
        }

        public string Name { get; internal set; }

        public List<object> Contents { get; private set; }

        public NovaScope Context { get; internal set; }
    }
}