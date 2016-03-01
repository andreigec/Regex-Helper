using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegexHelper.Tests
{
    [TestClass]
    public class UnitTest1
    {


        [TestMethod]
        public void TestMethod1()
        {
            List<string> l;
            #region
            //same generic
            l = new List<string>();
            l.Add(@"andrei");
            l.Add(@"andrei");
            var t1 = Controller.Extract(l, false);
            Assert.AreEqual(t1, "andrei");

            l = new List<string>();
            l.Add("andrei");
            l.Add("andrei");
            var t11 = Controller.Extract(l, true);
            Assert.AreEqual(t11, @"\w{6}");

            //different one char
            l = new List<string>();
            l.Add("andrei");
            l.Add("andres");
            var t2 = Controller.Extract(l, false);
            Assert.AreEqual(t2, "andre[is]");

            //different lengths
            l = new List<string>();
            l.Add("andrei");
            l.Add("andre");
            var t4 = Controller.Extract(l);
            Assert.AreEqual(t4, "andrei?");
            l = new List<string>();
            l.Add("andrei");
            l.Add("andr");
            var t41 = Controller.Extract(l);
            Assert.AreEqual(t41, "andr(ei)?");

            //look backwards
            l = new List<string>();
            l.Add(@"<test>");
            l.Add(@"<test\>");
            var t5 = Controller.Extract(l, false);
            Assert.AreEqual(t5, @"<test\?>");
            l = new List<string>();
            l.Add(@"a>");
            l.Add(@"ab\>");
            var t6 = Controller.Extract(l, false);
            Assert.AreEqual(t6, @"a(b\)?>");

            //space test
            l = new List<string>();
            l.Add(@" ");
            l.Add(@"/");
            var t7 = Controller.Extract(l, false);
            Assert.AreEqual(t7, @"[ /]");

            //multiple test
            l = new List<string>();
            l.Add(@"test a");
            var t8 = Controller.Extract(l, true);
            Assert.AreEqual(t8, @"\w{4} \w");
            #endregion

            l = new List<string>();
            l.Add(@"<street>");
            l.Add("<street type=\"X\">");
            l.Add("<street x=\"y\">");
            var t9 = Controller.Extract(l, false);
            Assert.AreEqual(t9, "<street( (typ)?[ex]=\"[Xy]\")?>");

        }
    }
}
