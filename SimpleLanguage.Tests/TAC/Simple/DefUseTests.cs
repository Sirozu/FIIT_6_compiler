﻿using System.Collections.Generic;
using NUnit.Framework;
using SimpleLang;

namespace SimpleLanguage.Tests.TAC.Simple
{
    [TestFixture]
    internal class DefUseTests : OptimizationsTestBase
    {
        [TestCase(@"
var a, b, x;
x = a;
x = b;
",
            ExpectedResult = new string[]
            {
                "noop",
                "x = b"
            },
            TestName = "VarAssignSimple")]

        [TestCase(@"
var a, b, x;
x = -a;
x = b;
",
            ExpectedResult = new string[]
            {
                "noop",
                "noop",
                "x = b"
            },
            TestName = "NegVarAssignSimple1")]

        [TestCase(@"
var a, b, x;
x = a;
x = -b;
",
            ExpectedResult = new string[]
            {
                "noop",
                "#t1 = -b",
                "x = #t1"
            },
            TestName = "NegVarAssignSimple2")]

        [TestCase(@"
var x;
x = 1;
x = 2;
",
            ExpectedResult = new string[]
            {
                "noop",
                "x = 2"
            },
            TestName = "ContAssignSimple")]

        [TestCase(@"
var x;
x = -1;
x = 2;
",
            ExpectedResult = new string[]
            {
                "noop",
                "noop",
                "x = 2"
            },
            TestName = "NegContAssignSimple1")]

        [TestCase(@"
var x;
x = 1;
x = -2;
",
            ExpectedResult = new string[]
            {
                "noop",
                "#t1 = -2",
                "x = #t1"
            },
            TestName = "NegContAssignSimple2")]

        [TestCase(@"
var a, b, x;
x = a or b;
x = !x;
",
            ExpectedResult = new string[]
            {
                "#t1 = a or b",
                "x = #t1",
                "#t2 = !x",
                "x = #t2"
            },
            TestName = "BoolNoDead")]

        [TestCase(@"
var a, b, x;
x = a or b;
x = !a;
",
            ExpectedResult = new string[]
            {
                "noop",
                "noop",
                "#t2 = !a",
                "x = #t2"
            },
            TestName = "BoolDead")]

        [TestCase(@"
var a, b, c;
a = 2;
b = a + 4;
c = a * b;
",
            ExpectedResult = new string[]
            {
                "a = 2",
                "#t1 = a + 4",
                "b = #t1",
                "#t2 = a * b",
                "c = #t2"
            },
            TestName = "NoDeadCode")]

        [TestCase(@"
var a, b;
a = 1;
input(a);
b = a + 1;
",
            ExpectedResult = new string[]
            {
                "noop",
                "input a",
                "#t1 = a + 1",
                "b = #t1"
            },
            TestName = "DeadBeforeInput")]

        [TestCase(@"
var a, b;
a = 1;
print(a);
input(a);
b = a + 1;
",
            ExpectedResult = new string[]
            {
                "a = 1",
                "print a",
                "input a",
                "#t1 = a + 1",
                "b = #t1"
            },
            TestName = "NoDeadInputPrint")]

        [TestCase(@"
var a, b;
input(a);
input(a);
b = a + 1;
",
            ExpectedResult = new string[]
            {
                "noop",
                "input a",
                "#t1 = a + 1",
                "b = #t1"
            },
            TestName = "DeadInput")]

        [TestCase(@"
var a, b, c, x;
a = (2 + x) - a;
b = (c * 3) - (b / 4);
c = (a * 10 + b * 2) / 3;
",
            ExpectedResult = new string[]
            {
                "#t1 = 2 + x",
                "#t2 = #t1 - a",
                "a = #t2",
                "#t3 = c * 3",
                "#t4 = b / 4",
                "#t5 = #t3 - #t4",
                "b = #t5",
                "#t6 = a * 10",
                "#t7 = b * 2",
                "#t8 = #t6 + #t7",
                "#t9 = #t8 / 3",
                "c = #t9"
            },
            TestName = "ArithmeticOpsNoDead")]

        [TestCase(@"
var a, b, c, x;
a = (2 + x) - a;
b = (c * 3) - (b / 4);
c = (a * 10 + b * 2) / 3;
a = 1;
b = 1;
c = 1;
",
            ExpectedResult = new string[]
            {
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "noop",
                "a = 1",
                "b = 1",
                "c = 1"
            },
            TestName = "ArithmeticOpsDead")]

        [TestCase(@"
var a, b, c;
a = a or b;
goto 777;
b = 2;
777:
c = 1;
",
            ExpectedResult = new string[]
            {
                "#t1 = a or b",
                "a = #t1",
                "goto 777",
                "b = 2",
                "777: c = 1"
            },
            TestName = "GotoNoDead")]

        [TestCase(@"
var a, b, c;
a = a or b;
goto 777;
777: c = 1;
c = 2;
",
            ExpectedResult = new string[]
            {
                "#t1 = a or b",
                "a = #t1",
                "goto 777",
                "777: noop",
                "c = 2"
            },
            TestName = "GotoDead")]

        [TestCase(@"
var a, b, c;
a = 2;
if a
{
    b = 1;
    c = b + 3;
}
a = 3;
",
            ExpectedResult = new string[]
            {
                "a = 2",
                "#t1 = !a",
                "if #t1 goto L1",
                "b = 1",
                "#t2 = b + 3",
                "c = #t2",
                "L1: noop",
                "a = 3"
            },
            TestName = "IfNoDead1")]

        [TestCase(@"
var a;
input(a);
if (a == true)
    a = false;
",
            ExpectedResult = new string[]
            {
                "input a",
                "#t1 = a != True",
                "if #t1 goto L1",
                "a = False",
                "L1: noop",
            },
            TestName = "IfNoDead2")]

        [TestCase(@"
var a, b, c;
a = 1;
a = 2;
b = 11;
b = 22;
a = 3;
a = b;
c = 1;
a = b + c;
b = -c;
c = 1;
b = a - c;
a = -b;
",
            ExpectedResult = new string[]
            {
                "noop",
                "noop",
                "noop",
                "b = 22",
                "noop",
                "noop",
                "c = 1",
                "#t1 = b + c",
                "a = #t1",
                "noop",
                "noop",
                "c = 1",
                "#t3 = a - c",
                "b = #t3",
                "#t4 = -b",
                "a = #t4",
            },
            TestName = "DefUseTest")]

        public IEnumerable<string> TestDefUse(string sourceCode) =>
            TestTACOptimization(sourceCode, ThreeAddressCodeDefUse.DeleteDeadCode);
    }
}
