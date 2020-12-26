// -----------------------------------------------------------------------
// <copyright file="Is.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Nova.Tests;
using Nova.Runtime;
using NUnit.Framework.Constraints;

namespace Nova.Tests {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class Is : NUnit.Framework.Is {
        public static FunctionConstraint Function(NovaFunction expected) {
            return new FunctionConstraint(expected);
        }
    }

    public static class ConstraintExpressionExtensions {
        public static FunctionConstraint Function(this ConstraintExpression expr, NovaFunction expected) {
            return Is.Function(expected);
        }
    }
}