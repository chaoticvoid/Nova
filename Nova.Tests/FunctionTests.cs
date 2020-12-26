// -----------------------------------------------------------------------
// <copyright file="FunctionTests.cs" Company="Michael Tindal">
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
using System.Collections.Generic;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Nova.Builtins;
using Nova.Expressions;
using Nova.Runtime;
using NUnit.Framework;

namespace Nova.Tests {
    [TestFixture]
    public class FunctionTests : NovaAbstractTestFixture {
        private class NativeHelper {
            public int AddXToY(int x, int y) {
                return x + y;
            }

            public int MethodTableTest(string x) {
                return 17;
            }

            public int MethodTableTest(int y) {
                return y + 10;
            }
        }

        public List<FunctionArgument> A(params FunctionArgument[] args) {
            return new List<FunctionArgument>(args);
        }

        public NovaFunction F(string name, List<FunctionArgument> args, string body) {
            return new NovaFunction(name, args, (BlockExpression)Compile(body), null);
        }

        [Test]
        public void CallIronRubyFunction() {
            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource =
               novaengine.CreateScriptSourceFromString("def testmethod(func) { func(10); }; testmethod(rubyfunc);");

            var rubyengine = GetRuntime().GetEngine("IronRuby");
            var rubysource = rubyengine.CreateScriptSourceFromString("def func(num); return (num/2) * 5; end;");

            var rubyscope = rubyengine.CreateScope();

            rubysource.Execute(rubyscope);
            var x = rubyscope.GetVariable("func");

            var novascope = novaengine.CreateScope();
            novascope.SetVariable("rubyfunc", x);
            Assert.That(novasource.Execute(novascope), Is.EqualTo(25));
        }

        [Test]
        public void TestAnonFuncCall() {
            Assert.That(CompileAndExecute("^(num) { num * 17; }(3);"), Is.EqualTo(51));
        }

        [Test]
        public void TestBlockCSharp() {
            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource =
                novaengine.CreateScriptSourceFromString("x = 20; def testmethod(*p) { for(a in p) { yield a; }; };");

            var novascope = novaengine.CreateScope();

            novasource.Execute(novascope);

            var test = novascope.GetVariable("testmethod");
            var res = 0;
            test(10, 20, 30, 40, (Action<int>) (x => res += x));
            Assert.That(res, Is.EqualTo(100));
        }

        [Test]
        public void TestBlockIronRuby() {
            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource =
                novaengine.CreateScriptSourceFromString("x = 20; def testmethod(*p) { for(a in p) { yield a; }; };");

            var novascope = novaengine.CreateScope();

            novasource.Execute(novascope);

            var rubyengine = GetRuntime().GetEngine("IronRuby");
            var rubysource =
                rubyengine.CreateScriptSourceFromString(
                    "res = 0; testmethod.call(10,20,30,50,lambda { |x| res = res + x; }); res;");

            var rubyscope = rubyengine.CreateScope();
            rubyscope.SetVariable("testmethod", novascope.GetVariable("testmethod"));

            Assert.That(rubysource.Execute(rubyscope), Is.EqualTo(110));
        }

        [Test]
        public void TestBlockIronRubyTwo() {
            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource =
                novaengine.CreateScriptSourceFromString(
                    "x = 20; def testmethod() { for(a in [1,2,3,4]) { yield a; }; };");

            var novascope = novaengine.CreateScope();

            novasource.Execute(novascope);

            var rubyengine = GetRuntime().GetEngine("IronRuby");
            var rubysource =
                rubyengine.CreateScriptSourceFromString("res = 0; testmethod.call lambda { |x| res = res + x; }; res;");

            var rubyscope = rubyengine.CreateScope();
            rubyscope.SetVariable("testmethod", novascope.GetVariable("testmethod"));

            Assert.That(rubysource.Execute(rubyscope), Is.EqualTo(10));
        }

        [Test]
        public void TestCallFunctionFunction() {
            Assert.That(
                CompileAndExecute("def testmethod(num) { return ^(n) { return num * n; }; }; testmethod(10)(10);"),
                Is.EqualTo(100));
        }

        [Test]
        public void TestCallNovaFunctionCSharp() {
            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource = novaengine.CreateScriptSourceFromString("def testmethod(num) { num + 17; };");
                
            var novascope = novaengine.CreateScope();

            novasource.Execute(novascope);

            var testmethod = novascope.GetVariable("testmethod");
            Assert.That((int)testmethod(10), Is.EqualTo(27));
        }


        [Test]
        public void TestCallNovaFunctionIronRuby() {
            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource =
                novaengine.CreateScriptSourceFromString("x = 20; def testmethod(num) { num + x + 17; };");

            var rubyengine = GetRuntime().GetEngine("IronRuby");
            var rubysource = rubyengine.CreateScriptSourceFromString("testmethod.call(10)");

            var scope = novaengine.CreateScope();

            novasource.Execute(scope);
            Assert.That((int)rubysource.Execute(scope), Is.EqualTo(47));
        }

        [Test]
        public void TestCannotHaveLiteralDefault() {
            Assert.Throws<SyntaxErrorException>(() => Compile("def shouldNotBeAllowed(:i=10) { i - 10; };"));
        }

        [Test]
        public void TestCommandSingleHash() {
            var expect = new NovaDictionary(new Dictionary<object, object> {{"hello", "world"}});
            var real = CompileAndExecute("def func(h) { return h; }; func('hello' => 'world');");
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestEval() {
            Assert.That(CompileAndExecute("x = 3; n = 'x + 4;'; eval(n);"), Is.EqualTo(7));
        }

        [Test]
        public void TestEval2() {
            Assert.That(CompileAndExecute("n = 'x = 3;'; eval(n); x + 4;"), Is.EqualTo(7));
        }


        [Test]
        public void TestEval3() {
            Assert.That(
                CompileAndExecute(
                    "n = '@x = 4;'; class EvalTest { def test { eval(n); }; }; y = EvalTest(); y.test(); y.x;"),
                Is.EqualTo(4));
        }

        [Test]
        public void TestEval4()
        {
            Assert.That(
                CompileAndExecute(
                    "m = 'xx'; n = '#{m} = 7;';  eval(n);  xx + 3;"), Is.EqualTo(10));
        }
        [Test]
        public void TestExplicitFunction() {
            Assert.That(
                CompileAndExecute(
                    "def each(a,&f) { for(o in a) { f(o); }; }; z = 10; each([5,10,25]) { |x| z += x; }; z;"),
                Is.EqualTo(50));
        }

        [Test]
        public void TestExplicitFunction2() {
            Assert.That(
                CompileAndExecute(
                    "def each(a,&f) { for(o in a) { f(o); }; }; z = 5; def f(x) { z += x; }; each([5,10,20],f); z;"),
                Is.EqualTo(40));
        }

        [Test]
        public void TestFunctionContext() {
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString("x = 15; def testfunc(num) { return num*x; };");

            var scope = engine.CreateScope();

            source.Execute(scope);

            var source1 = engine.CreateScriptSourceFromString("testfunc(7);");
            var scope1 = engine.CreateScope();
            scope1.SetVariable("testfunc", scope.GetVariable("testfunc"));
            var source2 = engine.CreateScriptSourceFromString("testfunc(5);");
            var scope2 = engine.CreateScope();
            scope2.SetVariable("testfunc", scope.GetVariable("testfunc"));

            Assert.That(source1.Execute(scope1), Is.EqualTo(105));
            Assert.That(source2.Execute(scope2), Is.EqualTo(75));
        }

        [Test]
        public void TestFunctionDefaultKeywordVararg() {
            Assert.That(
                CompileAndExecute("def testfunc(a=10,*c) { for(b in c) { a += b; }; return a; }; testfunc(c:10,15,20);"),
                Is.EqualTo(55));
        }

        [Test]
        public void TestFunctionDefaultValue() {
            Assert.That(CompileAndExecute("def testfunc(num=15) { x = 10; return x + num; }; testfunc();"),
                Is.EqualTo(25));
        }

        [Test]
        public void TestFunctionDefinitionDefaultNormalVararg() {
            Assert.That(
                CompileAndExecute(
                    "def testfunc(x=27,y,*z) { n = x + y; for(a in z) { n -= a; }; return n; }; testfunc(y:10,1,2,3);"),
                Is.EqualTo(31));
        }

        [Test]
        public void TestFunctionEmptyArgumentList() {
            Assert.That(CompileAndExecute("def testfunc() { return 10 + 5; }; testfunc();"),
                Is.EqualTo(15));
        }

        [Test]
        public void TestFunctionEncapsulation() {
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString("def testfunc(num) { return num**2; };");

            var scope = engine.CreateScope();

            source.Execute(scope);

            var source1 = engine.CreateScriptSourceFromString("testfunc(7);");
            var source2 = engine.CreateScriptSourceFromString("testfunc(5);");

            var val1 = source1.Execute(scope);
            var val2 = source2.Execute(scope);
            Assert.That(val1, Is.EqualTo(49));
            Assert.That(val2, Is.EqualTo(25));
        }

        [Test]
        public void TestFunctionKeywordCSharp() {
            var engine = GetRuntime().GetEngine("Nova");
            var source = engine.CreateScriptSourceFromString("def testfunc(a=10,b,c) { return (a + b) * c; };");

            var scope = engine.CreateScope();

            source.Execute(scope);
            var testfunc = scope.GetVariable("testfunc");
            Assert.That(testfunc(global::Nova.Nova.Arg("b", 5), 2), Is.EqualTo(30));
        }

        [Test]
        public void TestFunctionKeywordOffset() {
            Assert.That(CompileAndExecute("def testfunc(a=10,b,c) { x = (a*b)+c; return x; }; testfunc(b:10,25);"),
                Is.EqualTo(125));
        }

        [Test]
        public void TestFunctionKeywordOverflow() {
            Assert.That(CompileAndExecute("def testfunc(a=17,b) { return a + b; }; testfunc(b:30,20);"),
                Is.EqualTo(47));
        }

        [Test]
        public void TestFunctionKeywordOverwrite() {
            Assert.That(CompileAndExecute("def testfunc(a=10,b) { return a + b; }; testfunc(12,10,b:20);"),
                Is.EqualTo(32));
        }

        [Test]
        public void TestFunctionKeywordSimple() {
            Assert.That(CompileAndExecute("def testfunc(a,b) { return a - b; }; testfunc(b:5,a:10);"),
                Is.EqualTo(5));
        }

        [Test]
        public void TestFunctionReturnInMiddle() {
            Assert.That(CompileAndExecute("def testfunc(a,b) { return a * b; a/b; }; testfunc(10,5);"),
                Is.EqualTo(50));
        }

        [Test]
        public void TestFunctionReturnValue() {
            Assert.That(CompileAndExecute("def testfunc(x,y) { return x/y; }; z=18; n=3; testfunc(z,n);"),
                Is.EqualTo(6));
        }

        [Test]
        public void TestFunctionSimple() {
            Assert.That(CompileAndExecute("def testfunc(num) { x = 10; return x + num; }; testfunc(7);"),
                Is.EqualTo(17));
        }

        [Test]
        public void TestFunctionSimpleMultipleArgumentVarArg() {
            Assert.That(
                CompileAndExecute(
                    "def testfunc(num,*y) { for(x in y) { num += x; }; return num; }; testfunc(10,5,10,15,20,25);"),
                Is.EqualTo(85));
        }

        [Test]
        public void TestFunctionSimpleMultipleArguments() {
            Assert.That(CompileAndExecute("def testfunc(num,y) { x = 10; (x + num) * y; }; testfunc(17,2);"),
                Is.EqualTo(54));
        }

        [Test]
        public void TestMethodTable() {
            var expect = new NovaArray {200, 5, 23};
            var engine = GetRuntime().GetEngine("Nova");
            var novascope1 = engine.CreateScope();

            var source1 =
                engine.CreateScriptSourceFromString(
                    "def test1(x) { return x * 20; }; def test2(x,y) { return x/y; }; def test3(a,b,c) { return a+(b-c); };");
            source1.Execute(novascope1);

            var test1 = novascope1.GetVariable("test1");
            var test2 = novascope1.GetVariable("test2");
            var test3 = novascope1.GetVariable("test3");

            var table = new NovaMethodTable("test");
            table.AddFunction(test1);
            table.AddFunction(test2);
            table.AddFunction(test3);
            var novascope2 = engine.CreateScope();
            novascope2.SetVariable("test", table);
            var source2 = engine.CreateScriptSourceFromString("[test(10),test(10,2),test(10,20,7)];");
            var real = source2.Execute(novascope2);

            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestMethodTableCSharp() {
            var expect = new List<object> {200, 5, 23};
            var engine = GetRuntime().GetEngine("Nova");
            var novascope1 = engine.CreateScope();

            var source1 =
                engine.CreateScriptSourceFromString(
                    "def test1(x) { return x * 20; }; def test2(x,y) { return x/y; }; def test3(a,b,c) { return a+(b-c); };");
            source1.Execute(novascope1);

            var test1 = novascope1.GetVariable("test1");
            var test2 = novascope1.GetVariable("test2");
            var test3 = novascope1.GetVariable("test3");

            var table = new NovaMethodTable("test");
            table.AddFunction(test1);
            table.AddFunction(test2);
            table.AddFunction(test3);

            dynamic test = table;
            var real = new List<object> {test(10), test(10, 2), test(10, 20, 7)};

            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestMethodTableNameMatch() {
            var expect = new NovaArray {6, 18};
            var engine = GetRuntime().GetEngine("Nova");
            var novascope1 = engine.CreateScope();

            var source1 =
                engine.CreateScriptSourceFromString(
                    "def test1(x,y,z) { return (x%y)+z; }; def test2(a,b,c) { return a+b+c; };");
            source1.Execute(novascope1);

            var test1 = novascope1.GetVariable("test1");
            var test2 = novascope1.GetVariable("test2");

            var table = new NovaMethodTable("test");
            table.AddFunction(test1);
            table.AddFunction(test2);
            var novascope2 = engine.CreateScope();
            novascope2.SetVariable("test", table);
            var source2 = engine.CreateScriptSourceFromString("[test(4,y:3,5),test(a:7,6,5)];");
            var real = source2.Execute(novascope2);

            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestMethodTableNativeFunction() {
            var expect = new NovaArray {17, 37};
            var engine = GetRuntime().GetEngine("Nova");
            var scope = engine.CreateScope();

            var table = new NovaMethodTable("test");
            table.AddFunction(new NovaNativeFunction(typeof (NativeHelper),
                typeof (NativeHelper).GetMethod("MethodTableTest", new[] {typeof (int)})));
            table.AddFunction(new NovaNativeFunction(typeof (NativeHelper),
                typeof (NativeHelper).GetMethod("MethodTableTest", new[] {typeof (string)})));

            scope.SetVariable("test", table);
            var source = engine.CreateScriptSourceFromString("[test('hello'),test(27)];");
            var real = source.Execute(scope);
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestMethodTableResolveFail() {
            var engine = GetRuntime().GetEngine("Nova");
            var novascope1 = engine.CreateScope();

            var source1 =
                engine.CreateScriptSourceFromString(
                    "def test1(x,y,z) { return (x%y)+z; }; def test2(a,b,c) { return a+b+c; };");
            source1.Execute(novascope1);

            var test1 = novascope1.GetVariable("test1");
            var test2 = novascope1.GetVariable("test2");

            var table = new NovaMethodTable("test");
            table.AddFunction(test1);
            table.AddFunction(test2);
            var novascope2 = engine.CreateScope();
            novascope2.SetVariable("test", table);
            var source2 = engine.CreateScriptSourceFromString("test(2,3);");

            object val = source2.Execute(novascope2);
            Assert.That(val, Is.Null);
        }

        [Test]
        public void TestMethodTableTwo() {
            var expect = new NovaArray {200, 400, 60};
            var engine = GetRuntime().GetEngine("Nova");
            var novascope1 = engine.CreateScope();

            var source1 =
                engine.CreateScriptSourceFromString(
                    "def test1(x=10) { return x * 20; }; def test2(x,z,y=20) { return (x/y)*z; }; def test3(a,b,*c) { res = a; for(z in c) { res += (z-b); }; res; };");
            source1.Execute(novascope1);

            var test1 = novascope1.GetVariable("test1");
            var test2 = novascope1.GetVariable("test2");
            var test3 = novascope1.GetVariable("test3");

            var table = new NovaMethodTable("test");
            table.AddFunction(test1);
            table.AddFunction(test2);
            table.AddFunction(test3);
            var novascope2 = engine.CreateScope();
            novascope2.SetVariable("test", table);
            var source2 = engine.CreateScriptSourceFromString("[test(),test(400,20),test(10,5,10,15,20,25)];");
            var real = source2.Execute(novascope2);

            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestNativeFunction() {
            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource = novaengine.CreateScriptSourceFromString("nadd(3,7);");

            var scope = novaengine.CreateScope();

            scope.SetVariable("nadd",
                new NovaNativeFunction(typeof (NativeHelper), typeof (NativeHelper).GetMethod("AddXToY")));
            Assert.That(novasource.Execute(scope), Is.EqualTo(10));
        }

        [Test]
        public void TestNestedFunction() {
            var engine = GetRuntime().GetEngine("Nova");
            var source =
                engine.CreateScriptSourceFromString(
                    "def nest(n) { def nesthelper(y) { return n * y; }; return nesthelper(10); };");

            var scope = engine.CreateScope();

            source.Execute(scope);

            var source1 = engine.CreateScriptSourceFromString("nest(10);");
            var source2 = engine.CreateScriptSourceFromString("begin { nesthelper(10); } rescue * { nil; };");

            var val1 = source1.Execute(scope);
            Assert.That(val1, Is.EqualTo(100));
            object val2 = source2.Execute(scope);
            Assert.That(val2, Is.Null);
        }

        [Test]
        public void TestPartialFunctionCSharp() {
            var expect = new List<object> {10, 15, 25};

            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource = novaengine.CreateScriptSourceFromString("def mul(x,y) { return x * y; };");

            var novascope = novaengine.CreateScope();

            novasource.Execute(novascope);

            var mul = novascope.GetVariable("mul");
            var mul5 = mul(5);

            var real = new List<object> {mul5(2), mul5(3), mul5(5)};
            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestPartialFunctionCall() {
            var expect = new NovaArray {2, 3, 4};

            var real = CompileAndExecute("def add(x,y) { return x + y; }; add1 = add(1); [add1(1),add1(2),add1(3)];");

            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestPartialFunctionCallBackward() {
            var expect = new NovaArray {10, 4, 2};

            var real =
                CompileAndExecute(
                    "def div(x,y) { return y / x; }; div20 = div(20); [div20 <| 2,div20 <| 5,div20 <| 10];");

            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestPartialFunctionCallForward() {
            var expect = new NovaArray {2, 3, 4};

            var real =
                CompileAndExecute("def add(x,y) { return x + y; }; add1 = add(1); [1 |> add1,2 |> add1,3 |> add1];");

            Assert.That(real, Is.EqualTo(expect));
        }

        [Test]
        public void TestPartialFunctionIronRuby() {
            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource =
                novaengine.CreateScriptSourceFromString("x = 17; def addX(y,z) { return x + y + z; }; add10=addX(10);");

            var rubyengine = GetRuntime().GetEngine("IronRuby");
            var rubysource = rubyengine.CreateScriptSourceFromString("add10.call(30);");

            var scope = novaengine.CreateScope();

            novasource.Execute(scope);

            Assert.That(rubysource.Execute(scope), Is.EqualTo(57));
        }

        [Test]
        public void TestSimpleFunctionDefinition() {
            var expect = F("simple", A(new FunctionArgument("i")), "i+88;");
            var real = CompileAndExecute("def simple(i) { i + 88; };");
            Assert.That(real, Is.Function(expect));
        }

        [Test]
        public void TestSubDictionaryFunctionCall() {
            Assert.That(
                CompileAndExecute(
                    "dictClass = { 'dict0' => ^(num) { return num*10; }, 'dict1' => ^(num0,num1) { [num0,num1,num0 + num1];}, 'subdictfunc' => ^{ return({'test' => {'testfunc' => ^{ return 'wtf was that?!'; }}}); }, 'subdict' => {'test' => ^{ return 'subdict function'; }}}; dictClass['subdictfunc']()['test']['testfunc']();"),
                Is.EqualTo("wtf was that?!"));
        }

        [Test]
        public void TestVarArgAnonFunc() {
            var varArg = new FunctionArgument("p") {IsVarArg = true};
            var expect = F("nova$anonFunc0", A(new FunctionArgument("x"), varArg), "for(pp in p) { i * pp; };");
            var real = CompileAndExecute("^(x,*p) { for(pp in p) { i * pp; }; };");
            Assert.That(real, Is.Function(expect));
        }

        [Test]
        public void TestYieldNoArgs() {
            Assert.That(
                CompileAndExecute(
                    "def times(a) { b = 0; while(b < a) { yield; b+=1; }; }; z = 5; times(3) { z += 5; }; z;"),
                Is.EqualTo(20));
        }

        [Test]
        public void TestYieldOneArg() {
            Assert.That(
                CompileAndExecute(
                    "def each(a) { for(o in a) { yield o; }; }; z = 5; each([5,10,10]) { |x| z += x; }; z;"),
                Is.EqualTo(30));
        }

        [Test]
        public void TestYieldCSharp()
        {
            var yieldFunc = CompileAndExecute("def each (a) { for(o in a) { yield o; }; };");
            var z = 5;
            yieldFunc(new List<int> { 5, 10, 10 }, (Action<int>)(x => { z += x; }));
            Assert.That(z, Is.EqualTo(30));
        }

        [Test]
        public void TestYieldIronRuby()
        {
            var novaengine = GetRuntime().GetEngine("Nova");
            var novasource =
                novaengine.CreateScriptSourceFromString("def novaeach (a) { for(o in a) { yield o; }; };");

            var rubyengine = GetRuntime().GetEngine("IronRuby");
            var rubysource = rubyengine.CreateScriptSourceFromString("z = 5; novaeach.call([5,10,10], lambda { |x| z = z + x; }); z;");

            var scope = novaengine.CreateScope();

            novasource.Execute(scope);

            Assert.That(rubysource.Execute(scope), Is.EqualTo(30));
        }
    }
}