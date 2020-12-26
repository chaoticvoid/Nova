using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nova.Runtime;

namespace Nova.Expressions
{
    public class InstanceAccessExpression : NovaExpression
    {
        internal InstanceAccessExpression(LeftHandValueExpression lvalue, List<FunctionArgument> callArgs)
        {
            Value = lvalue;
            Arguments = callArgs;
        }

        public LeftHandValueExpression Value { get; private set; }

        /// <summary>
        ///     Gets the arguments used for this expression.
        /// </summary>
        /// <value>The arguments.</value>
        public List<FunctionArgument> Arguments { get; private set; }

        /// <summary>
        ///     The type returned by this expression.
        /// </summary>
        /// <value>The type.</value>
        public override Type Type => typeof(object);

        public override Expression Reduce()
        {
            return Operation.Access(typeof(object), Value, Constant(Arguments), Constant(Scope));
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents the current <see cref="Nova.Expressions.AccessExpression" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="Nova.Expressions.AccessExpression" />.</returns>
        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendFormat("{0}[", Value);
            Arguments.ForEach(arg => str.AppendFormat("{0},", arg));

            str.Remove(str.Length - 1, 1);
            str.Append("]");
            return str.ToString();
        }

        /// <summary>
        ///     Called by SetScope to set the children scope on expressions. Should be overridden to tell the runtime which
        ///     children should have scopes set.
        /// </summary>
        /// <param name="scope"></param>
        public override void SetChildrenScopes(NovaScope scope)
        {
            Value.SetScope(scope);
            foreach (var arg in Arguments)
            {
                arg.Value.SetScope(scope);
            }

        }
    }
}
