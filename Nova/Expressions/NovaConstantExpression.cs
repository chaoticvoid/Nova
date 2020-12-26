using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nova.Runtime;

namespace Nova.Expressions
{
    public class NovaConstantExpression : NovaExpression
    {
        internal NovaConstantExpression(Expression value)
        {
            Value = value;
        }

        public Expression Value { get; private set; }

        public override Type Type => typeof(object);

        public override Expression Reduce()
        {
            return Operation.Resolve(typeof(object), Value, Constant(Scope));
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
