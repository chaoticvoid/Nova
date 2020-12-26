using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nova.Runtime;

namespace Nova.Expressions {
    using CS = CompilerServices;

    public class ModuleDefinitionExpression : NovaExpression {
        internal ModuleDefinitionExpression(string name, List<Expression> contents) {
            Name = name;
            Contents = contents;
        }

        public string Name { get; private set; }

        public List<Expression> Contents { get; private set; }

        public override Type Type {
            get { return typeof (NovaModule); }
        }

        public override string ToString() {
            return "";
        }

        public override Expression Reduce() {
            return Operation.DefineModule(typeof (NovaModule), Constant(Name), Constant(Contents), Constant(Scope));
        }

        public override void SetChildrenScopes(NovaScope scope) {
            Contents.ForEach(content => content.SetScope(scope));
        }
    }
}