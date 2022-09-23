using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Tests
{
    [TestClass()]
    public class GlobalExtensionsTests
    {
        enum Fruits { Apple, Banana, Mango, Orange };
        readonly char[] IvyChars = new char[] { 'I', 'v', 'y', ' ', 'R', 'o','s','e' };
        readonly string IvyString = "Ivy Rose";
        [TestMethod()]
        public void MaxFactorTest()
        {
            Assert.AreEqual(31ul, ((ulong)124).MaxFactor());
        }

        [TestMethod()]
        public void IsStringTest()
        {
            Assert.IsTrue(IvyChars.IsString(IvyString));
        }

        [TestMethod()]
        public void IsCharArrayTest()
        {
            Assert.IsTrue(IvyString.IsCharArray(IvyChars));
        }

        [TestMethod()]
        public void IsPerfectSquareTest()
        {
            Assert.IsTrue(625d.IsPerfectSquare());
        }

        [TestMethod()]
        public void SubarrayTest()
        {
            var secondName = IvyChars.Subarray(4, 4);

            Assert.IsTrue(
                secondName[0] == 'R' &&
                secondName[1] == 'o' &&
                secondName[2] == 's' &&
                secondName[3] == 'e');
        }

        [TestMethod()]
        public void GetBytesTest()
        {
            var bytes = IvyString.GetBytes<ASCIIEncoding>();

            //In ascii,I is represented by 73
            Assert.AreEqual(bytes[0], 73);
        }

        [TestMethod()]
        public void GetStringTest()
        {
            var bytes = new byte[] { 73 };

            Assert.AreEqual("I", bytes.GetString<ASCIIEncoding>());
        }

    }
}