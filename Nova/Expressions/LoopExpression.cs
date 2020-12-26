using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nova.Parser;
using Nova.Runtime;

namespace Nova.Expressions
{
    public class LoopExpression : NovaExpression
    {
        internal LoopExpression(Expression body)
        {
            Body = body;
        }

        public Expression Body { get; private set; }

        public override Type Type => Body.Type;

        public override Expression Reduce()
        {
            var loopLabel = Label("<nova_loop>");
            ParameterExpression loopReturn = null;
            var useReturn = true;
            if (Body.Type == typeof(void))
            {
                useReturn = false;
            }
            else
            {
                loopReturn = Variable(Body.Type, "<nova_loop_return>");
            }
            var realBody = new List<Expression> {
                Label(loopLabel),
                Label(NovaParser.ContinueTarget),
                Label(NovaParser.RetryTarget),
                useReturn
                    ? Assign(loopReturn, Body)
                    : Body,
                Goto(loopLabel),
                Label(NovaParser.BreakTarget)
            };
            if (useReturn)
            {
                realBody.Add(Convert(loopReturn, Body.Type));

                return Block(new[] { loopReturn }, realBody);
            }

            return Block(new ParameterExpression[] { }, realBody);
        }

        public override void SetChildrenScopes(NovaScope scope)
        {
            Body.SetScope(scope);
        }

        public override string ToString()
        {
            return $"loop {{ {Body} }}";
        }
    }
}
