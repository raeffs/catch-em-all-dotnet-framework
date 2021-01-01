using Cirrus.Module.CatchEmAll.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cirrus.Module.CatchEmAll.Tests
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        public void ToDecimal_1000AsParameter_ShouldReturn1000()
        {
            var actual = Parse.ToDecimal("1'000.00");

            Assert.AreEqual(1000, actual);
        }
    }
}
