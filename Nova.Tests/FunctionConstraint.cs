// -----------------------------------------------------------------------
// <copyright file="FunctionConstraint.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Nova.Runtime;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Nova.Tests {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class FunctionConstraint : Constraint {
        private object actual;

        internal FunctionConstraint(NovaFunction expected) {
            Expected = expected;
        }

        public NovaFunction Expected { get; private set; }

        private ConstraintResult Check() {
            var real = (NovaFunction) actual;
            var success = false;
            try {
                Assert.That(real.Name, Is.EqualTo(Expected.Name));
                Assert.That(real.Arguments, Is.EqualTo(Expected.Arguments));
                Assert.That(real.Body.ToString().Substring(0, Expected.Body.ToString().Length).Replace("; {", "; }"),
                    Is.EqualTo(Expected.Body.ToString()));
                success = true;
            }
            catch (AssertionException) {
                Description = $"Expected {actual}, but got {Expected} instead";
                success = false;
            }
            return new ConstraintResult(this, actual, success);
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            this.actual = actual;
            if (!(actual is NovaFunction)) {
                Description = $"Expected NovaFunction, but got {actual.GetType()} instead";
                return new ConstraintResult(this, actual, false);
            }
            return Check();
        }

        public override string Description
        {
            get
            {
                return base.Description;
            }

            protected set
            {
                base.Description = value;
            }
        }
    }
}