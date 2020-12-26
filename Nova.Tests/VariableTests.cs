// -----------------------------------------------------------------------
// <copyright file="VariableTests.cs" Company="Michael Tindal">
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

using System;
using Microsoft.Scripting.Hosting;
using Nova.Builtins;
using Nova.Runtime;
using NUnit.Framework;

namespace Nova.Tests {
    [TestFixture]
    public class VariableTests : NovaAbstractTestFixture {
        [Test]
        public void TestAddAssign() {
            Assert.That(CompileAndExecute("x = 5; x += 5; x;"), Is.EqualTo(10));
        }

        [Test]
        public void TestAlias() {
            Assert.That(CompileAndExecute("def osize(s) { return s; }; alias osize size; size(25);"),
                Is.EqualTo(25));
        }

        [Test]
        public void TestAliasConstDoesNotPollute() {
            Assert.That(CompileAndExecute("num = 10; alias num x; def func(y) { x = 10; x + y; }; func(7);"),
                Is.EqualTo(17));
        }

        [Test]
        public void TestAliasConstant() {
            Assert.Throws<ConstantException>(() => CompileAndExecute("x = 15; alias x a; a = 10;"));
        }

        [Test]
        public void TestAliasDoesNotOverwriteSameScope() {
            Assert.That(CompileAndExecute("num = 20; xnum = 40; alias xnum num; num;"), Is.EqualTo(20));
        }

        [Test]
        public void TestAliasOverwriteChildScope() {
            Assert.That(CompileAndExecute("num = 20; xnum = 40; do { alias xnum num; num; } while(0);"),
                Is.EqualTo(40));
        }

        [Test]
        public void TestAliasScope() {
            Assert.That(CompileAndExecute("a=10; def f(x) { alias a x; x = 7; }; a;"), Is.EqualTo(10));
        }

        [Test]
        public void TestAliasTwo() {
            Assert.That(CompileAndExecute("$osize=25; alias $osize $size; $size;"), Is.EqualTo(25));
        }

        [Test]
        public void TestAndAssign() {
            Assert.That(CompileAndExecute("x = 5; x &= 3; x;"), Is.EqualTo(1));
        }

        [Test]
        public void TestAssign() {
            Assert.That(CompileAndExecute("x = 5; x;"), Is.EqualTo(5));
        }

        [Test]
        public void TestCastLeftShiftAssign() {
            Assert.That(CompileAndExecute("x = 5; x <<= 3.0; x;"), Is.EqualTo(40));
        }

        [Test]
        public void TestChainedTypeof()
        {
            var val = CompileAndExecute("typeof(typeof(2));");
            bool isType = val == typeof(Type);
            Assert.IsTrue(isType);
        }

        [Test]
        public void TestConditionalAssignment1() {
            Assert.That(CompileAndExecute("x = nil; x ||= 'test';"), Is.EqualTo("test"));
        }

        [Test]
        public void TestConditionalAssignment2() {
            Assert.That(CompileAndExecute("x = 'hello'; x ||= 'test';"), Is.EqualTo("hello"));
        }

        [Test]
        public void TestConditionalAssignment3() {
            object val = CompileAndExecute("x = nil; x &&= 'test';");
            Assert.That(val, Is.Null);
        }

        [Test]
        public void TestConditionalAssignment4() {
            Assert.That(CompileAndExecute("x = 'hello'; x &&= 'test';"), Is.EqualTo("test"));
        }

        [Test]
        public void TestConstReassignment() {
            Assert.Throws<ConstantException>(() => CompileAndExecute("const x = 10; x = 5;"));
        }

        [Test]
        public void TestDecrement1() {
            Assert.That(CompileAndExecute("x = 5; x--; x;"), Is.EqualTo(4));
        }

        [Test]
        public void TestDecrement2() {
            Assert.That(CompileAndExecute("x = 5; v = x--; v;"), Is.EqualTo(5));
        }

        [Test]
        public void TestDecrement3() {
            Assert.That(CompileAndExecute("x = 5; v = --x; v;"), Is.EqualTo(4));
        }

        [Test]
        public void TestDifferentGlobalScope() {
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString("$x;");

            var scope1 = engine.CreateScope();
            var scope2 = engine.CreateScope();

            scope1.SetVariable("x", 10);
            scope2.SetVariable("x", "hello");

            var val1 = source.Execute(scope1);
            var val2 = source.Execute(scope2);

            Assert.That(val1, Is.EqualTo(10));
            Assert.That(val2, Is.EqualTo("hello"));
        }

        [Test]
        public void TestDivideAssign() {
            Assert.That(CompileAndExecute("x = 49; x /= 7; x;"), Is.EqualTo(7));
        }

        [Test]
        public void TestDynamicReassignment() {
            Assert.That(CompileAndExecute("x = 5; x = 'hello'; x;"), Is.EqualTo("hello"));
        }

        [Test]
        public void TestExclusiveOrAssign() {
            Assert.That(CompileAndExecute("x = 5; x ^= 3; x;"), Is.EqualTo(6));
        }

        [Test]
        public void TestGlobalConstException() {
            Assert.Throws<ConstantException>(() => CompileAndExecute("const $_gct = 5; _gct = 7;"));
        }

        [Test]
        public void TestGlobals() {
            global::Nova.Nova.Execute("def test(x) { return x + 10; }; y = 20;");

            var test = global::Nova.Nova.Globals.GetVariable("test");
            var y = global::Nova.Nova.Globals.GetVariable("y");

            Assert.That(test(y), Is.EqualTo(30));
        }

        [Test]
        public void TestHeredoc() {
            Assert.That(CompileAndExecute("x = <<-EOF Hello world!EOF; x;"), Is.EqualTo("Hello world!"));
        }

        [Test]
        public void TestImplicitScopeEntry() {
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString("x + 10;");
            global::Nova.Nova.Globals.SetVariable("x", 10);
            Assert.That(source.Execute(), Is.EqualTo(20));
            global::Nova.Nova.Globals.RemoveVariable("x");
        }

        [Test]
        public void TestIncrement1() {
            Assert.That(CompileAndExecute("x = 1; x++; x;"), Is.EqualTo(2));
        }

        [Test]
        public void TestIncrement2() {
            Assert.That(CompileAndExecute("x = 1; v = x++; v;"), Is.EqualTo(1));
        }

        [Test]
        public void TestIncrement3() {
            Assert.That(CompileAndExecute("x = 1; v = ++x; v;"), Is.EqualTo(2));
        }

        [Test]
        public void TestLeftShiftAssign() {
            Assert.That(CompileAndExecute("x = 5; x <<= 3; x;"), Is.EqualTo(40));
        }

        [Test]
        public void TestLocalAssignment() {
            Assert.That(CompileAndExecute("x = 15; do { @x = 10; } while(0); x;"), Is.EqualTo(15));
        }

        [Test]
        public void TestLocalConstException() {
            Assert.Throws<ConstantException>(() => CompileAndExecute("def func(a) { const @x = 7; x = 5; }; func(5);"));
        }

        [Test]
        public void TestLocalConstOkay() {
            Assert.That(CompileAndExecute("def func(a) { const _lx = 7; }; func(5); _lx = 5; _lx;"),
                Is.EqualTo(5));
        }

        [Test]
        public void TestModuloAssign() {
            Assert.That(CompileAndExecute("x = 5; x %= 2; x;"), Is.EqualTo(1));
        }

        [Test]
        public void TestMultiplyAssign() {
            Assert.That(CompileAndExecute("x = 10; x *= 5; x;"), Is.EqualTo(50));
        }

        [Test]
        public void TestNestedAssignment() {
            var expect = new NovaArray {5, 5};

            var real = CompileAndExecute("x = y = 5; [x,y];");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestNestedParallelAssignment() {
            var expect = new NovaArray {1, new NovaArray {2, 3}, 4};
            var real = CompileAndExecute("pa,(pb,pc),pd=1,[2,3],4;");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestNestedParallelAssignmentTwo() {
            var expect = new NovaArray {1, new NovaArray {2, null}, 3};
            var real = CompileAndExecute("pa,(pb,pc),pd=1,2,3;");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestOrAssign() {
            Assert.That(CompileAndExecute("x = 5; x |= 3; x;"), Is.EqualTo(7));
        }

        [Test]
        public void TestParallelAssigmentWildcardLhsInMiddle() {
            var expect = new NovaArray {1, new NovaArray {2, 3}, null};
            var real = CompileAndExecute("pa,*pb,pc=1,2,3");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestParallelAssignmentEvalAll() {
            var expect = new NovaArray {0, 1, 2};
            var real = CompileAndExecute("px = 0; pa,pb,pc=px,++px,++px;");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestParallelAssignmentSimple() {
            var expect = new NovaArray {1, 2};
            var real = CompileAndExecute("pa,pb=1,2;");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestParallelAssignmentWildcardLhsAtEnd() {
            var expect = new NovaArray {1, 2, new NovaArray {3, 4}};
            var real = CompileAndExecute("pa,pb,*pc=1,2,3,4");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestParallelAssignmentWildcardLhsRhs() {
            var expect = new NovaArray {1, 2, 3};
            var real = CompileAndExecute("pb=[1,2,3];*pa = *pb;");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestParallelAssignmentWildcardOnlyLhs() {
            var expect = new NovaArray {1, 2, 3};
            var real = CompileAndExecute("*pa=[1,2,3];");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestParallelAssignmentWildcardOnlyRhs() {
            var expect = new NovaArray {1, 2, 3};
            var real = CompileAndExecute("pb=[1,2,3]; pa,pb,pc = *pb;");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestParallelAssignmentWildcardRhsAtEnd() {
            var expect = new NovaArray {1, 2, 3};
            var real = CompileAndExecute("pb=[2,3];pa,pb,pc = 1,*pb;");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestParallelAssignmentWildcardRhsInMiddle() {
            var expect = new NovaArray {1, 2, 3, 4};
            var real = CompileAndExecute("pb=[2,3];pa,pb,pc,pd = 1,*pb,4;");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestPassStringToNova() {
            var engine = GetRuntime().GetEngine("Nova");
            var Novascope1 = engine.CreateScope();

            var source1 = engine.CreateScriptSourceFromString("def test1(x) { x << ' world!'; x; }");
            source1.Execute(Novascope1);

            var test = Novascope1.GetVariable("test1");

            Assert.That((string) test("Hello"), Is.EqualTo("Hello world!"));
        }

        [Test]
        public void TestPowerAssign() {
            Assert.That(CompileAndExecute("x = 5.0; x **= 2; x;"), Is.EqualTo(25));
        }

        [Test]
        public void TestRightShiftAssign() {
            Assert.That(CompileAndExecute("x = 40; x >>= 3; x;"), Is.EqualTo(5));
        }

        [Test]
        public void TestScopeEntry() {
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString("x + 10;");
            var scope = engine.CreateScope();
            scope.SetVariable("x", 5);
            Assert.That(source.Execute(scope), Is.EqualTo(15));
        }

        [Test]
        public void TestScopeExit() {
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString("x = (7 + 3) / 2;");
            var scope = engine.CreateScope();

            source.Execute(scope);

            var val = scope.GetVariable("x");
            Assert.That((int)val, Is.EqualTo(5));
        }

        [Test]
        public void TestStringEval() {
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString("'Hello #{sv_string_desc_test}!';");

            var scope = engine.CreateScope();
            scope.SetVariable("sv_string_desc_test", "world");
            Assert.That(source.Execute(scope), Is.EqualTo("Hello world!"));
        }

        [Test]
        public void TestSubtractAssign() {
            Assert.That(CompileAndExecute("x = 20; x -= 10; x;"), Is.EqualTo(10));
        }

        [Test]
        public void TestSymbol() {
            Assert.That((int)CompileAndExecute("x = 5; :x;"), Is.EqualTo(5));
        }

        [Test]
        public void TestSymbolTwo() {
            var sym1 = Symbol.NewSymbol("ohyeaohyeaohyea");
            var sym2 = Symbol.NewSymbol("ohyeaohyeaohyea");

            Assert.That(sym2, Is.EqualTo(sym1));

            var sym3 = Symbol.NewSymbol("imsexyandiknowit");
            Assert.That(sym1, Is.Not.EqualTo(sym3));
            Assert.That(sym2, Is.Not.EqualTo(sym3));
        }

        [Test]
        public void TestTypeof()
        {
            var val = CompileAndExecute("typeof(1 ** 2);");
            bool isType = val == typeof(double);
            Assert.IsTrue(isType);
        }

        [Test]
        public void TestCustomAssignOp()
        {
            Assert.That((int)CompileAndExecute("class Number { def +*=(v) { self += v; self *= v; self; }; };  x = 2; x +*= 3; x;"), Is.EqualTo(15));
        }
    }
}