using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Simplex;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ValueZnachenie vz = new ValueZnachenie();
            vz.simplexBol();
            Assert.AreEqual(vz.Tab_Rezultat[vz.Tab_Rezultat.GetLength(0) - 1, 0] , - 45.6);
        }
        [TestMethod]
        public void TestMethod2()
        {
            ValueZnachenie vz = new ValueZnachenie();
            vz.simplexBol();
            Assert.AreEqual(vz.Tab_Rezultat[vz.Tab_Rezultat.GetLength(0) - 1, 0] *-1, 45.6);
        }
        [TestMethod]
        public void TestMethod3()
        {
            ValueZnachenie vz = new ValueZnachenie();
            vz.simplexBol();
            Assert.AreEqual(vz.Tab_Rezultat[0,0], 5.6);
        }
        [TestMethod]
        public void TestMethod4()
        {
            ValueZnachenie vz = new ValueZnachenie();
            vz.simplexBol();
            Assert.AreEqual(vz.Tab_Rezultat[2, 0], 0.4);
        }
    }
}
