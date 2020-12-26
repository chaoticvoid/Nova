// -----------------------------------------------------------------------
// <copyright file="BinaryExpressionTests.cs" Company="Michael Tindal">
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

using Microsoft.Scripting.Hosting;
using Nova.Builtins;
using NUnit.Framework;

namespace Nova.Tests {
    [TestFixture]
    public class BinaryExpressionTests : NovaAbstractTestFixture {
        public bool O(int a, string z, int b) {
            var sourceString = $"{a} {z} {b};";
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString(sourceString);
            return (bool) source.Execute(engine.CreateScope());
        }

        public bool L(bool a, string z, bool b) {
            var sourceString = $"{a.ToString().ToLower()} {z} {b.ToString().ToLower()};";
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString(sourceString);
            return (bool) source.Execute(engine.CreateScope());
        }

        public int B(int a, string z, int b) {
            var sourceString = $"{a} {z} {b};";
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString(sourceString);
            return (int) source.Execute(engine.CreateScope());
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        public void TestEqualTo(int x, int y, bool expected) {
            Assert.That(O(x, "==", y), Is.EqualTo(expected));
        }

        [TestCase(1, 2, true)]
        [TestCase(1, 1, false)]
        public void TestNotEqualTo(int x, int y, bool expected) {
            Assert.That(O(x, "!=", y), Is.EqualTo(expected));
        }

        [TestCase(1, 2, true)]
        [TestCase(2, 2, false)]
        [TestCase(2, 1, false)]
        public void TestLessThan(int x, int y, bool expected) {
            Assert.That(O(x, "<", y), Is.EqualTo(expected));
        }

        [TestCase(1, 2, true)]
        [TestCase(2, 2, true)]
        [TestCase(2, 1, false)]
        public void TestLessThanEqualTo(int x, int y, bool expected) {
            Assert.That(O(x, "<=", y), Is.EqualTo(expected));
        }

        [TestCase(2, 1, true)]
        [TestCase(2, 2, false)]
        [TestCase(2, 3, false)]
        public void TestGreaterThan(int x, int y, bool expected) {
            Assert.That(O(x, ">", y), Is.EqualTo(expected));
        }

        [TestCase(2, 1, true)]
        [TestCase(2, 2, true)]
        [TestCase(2, 3, false)]
        public void TestGreaterThanEqualTo(int x, int y, bool expected) {
            Assert.That(O(x, ">=", y), Is.EqualTo(expected));
        }

        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void TestLogicalAnd(bool x, bool y, bool expected) {
            Assert.That(L(x, "&&", y), Is.EqualTo(expected));
        }

        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void TestLogicalOr(bool x, bool y, bool expected) {
            Assert.That(L(x, "||", y), Is.EqualTo(expected));
        }

        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void TestLogicalXor(bool x, bool y, bool expected) {
            Assert.That(L(x, "^^", y), Is.EqualTo(expected));
        }

        [TestCase(1, 2, -1)]
        [TestCase(2, 2, 0)]
        [TestCase(2, 1, 1)]
        public void TestCompare(int x, int y, int expected) {
            Assert.That(B(x, "<=>", y), Is.EqualTo(expected));
        }

        [Test]
        public void TestAdd() {
            Assert.That(B(5, "+", 7), Is.EqualTo(12));
        }

        [Test]
        public void TestBitwiseAnd() {
            Assert.That(B(2, "&", 1), Is.EqualTo(0));
        }

        [Test]
        public void TestBitwiseInverse() {
            Assert.That(CompileAndExecute("~5"), Is.EqualTo(~5));
        }

        [Test]
        public void TestBitwiseOr() {
            Assert.That(B(4, "|", 3), Is.EqualTo(7));
        }

        [Test]
        public void TestBitwiseXor() {
            Assert.That(B(10, "^", 8), Is.EqualTo(2));
        }

        [Test]
        public void TestComplexMath1() {
            Assert.That(CompileAndExecute("5+7*3"), Is.EqualTo(26));
        }

        [Test]
        public void TestComplexMath2() {
            Assert.That(CompileAndExecute("(5 + 7) * 3;"), Is.EqualTo(36));
        }

        [Test]
        public void TestComplexMath3() {
            Assert.That(CompileAndExecute("5 + (7 * 3) - 4;"), Is.EqualTo(22));
        }

        [Test]
        public void TestComplexMath4() {
            Assert.That(CompileAndExecute("5 + 7 * 3 - 4;"), Is.EqualTo(22));
        }

        [Test]
        public void TestComplexMath5() {
            Assert.That(CompileAndExecute("(5 << 3) + 3 * 5 ** (8 / 4) | 10;"), Is.EqualTo(123));
        }

        [Test]
        public void TestComplexMath6() {
            Assert.That(CompileAndExecute("- 3 * 7;"), Is.EqualTo(-21));
        }

        [Test]
        public void TestDivide() {
            Assert.That(B(100, "/", 5), Is.EqualTo(20));
        }

        [Test]
        public void TestExp() {
            Assert.That(CompileAndExecute("3.0 ** 4.0;"), Is.EqualTo(81));
        }

        [Test]
        public void TestIntegrationRuby() {
            var Novaengine = GetRuntime().GetEngine("Nova");
            var Novasource = Novaengine.CreateScriptSourceFromString("x + 2;");

            var rubyengine = GetRuntime().GetEngine("IronRuby");
            var rubysource =
                rubyengine.CreateScriptSourceFromString(
                    "class AddTest; def initialize(num); @num = num; end; def +(other); @num + other; end; end; x = AddTest.new(5);");

            var x = rubysource.Execute(rubyengine.CreateScope());

            var Novascope = Novaengine.CreateScope();
            Novascope.SetVariable("x", x);
            Assert.That(Novasource.Execute(Novascope), Is.EqualTo(7));
        }

        [Test]
        public void TestLogicalNot() {
            Assert.That(CompileAndExecute("!true;"), Is.EqualTo(false));
        }

        [Test]
        public void TestModulus() {
            Assert.That(B(5, "%", 2), Is.EqualTo(1));
        }

        [Test]
        public void TestMultiply() {
            Assert.That(B(10, "*", 17), Is.EqualTo(170));
        }

        [Test]
        public void TestNegate() {
            Assert.That(CompileAndExecute("- - 3;"), Is.EqualTo(3));
        }

        [Test]
        public void TestPositive() {
            Assert.That(CompileAndExecute("+ + +3;"), Is.EqualTo(3));
        }

        [Test]
        public void TestShiftLeft() {
            Assert.That(B(2, "<<", 2), Is.EqualTo(8));
        }

        [Test]
        public void TestShiftRight() {
            Assert.That(B(2, ">>", 2), Is.EqualTo(0));
        }

        [Test]
        public void TestSubtract() {
            Assert.That(B(7, "-", 5), Is.EqualTo(2));
        }

        [Test]
        public void TestRegex1()
        {
            Assert.That(CompileAndExecute("r = %/(H(?<a>el+)o) ([A-Z](?<b>.+)d)./; x = 'Hello World!'; x =~ r; a;"), Is.EqualTo("ell"));
        }

        [Test]
        public void TestRegex2()
        {
            var expect = new NovaArray {"ell", "orl"};
            Assert.That(
                CompileAndExecute("x = 'Hello World!'; x =~ %/[^e]+(?<test>el+).+W(?<word>o.+)d/; [test,word];"),
                Is.EqualTo(expect));
        }
    }
}