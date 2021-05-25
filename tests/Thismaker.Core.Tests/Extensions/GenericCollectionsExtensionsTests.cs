using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Generic.Tests
{
    [TestClass()]
    public class GenericCollectionsExtensionsTests
    {
        [TestMethod()]
        public void MoveTest()
        {
            var list = new List<string> { "Orange", "Potato", "Banana", "Milk" };
            list.Move(1, 3);
            Assert.AreEqual("Potato", list[3]);
        }

        [TestMethod()]
        public void MoveTest1()
        {
            var list = new List<string> { "Orange", "Potato", "Banana", "Milk" };
            list.Move("Potato", 3);
            Assert.AreEqual("Potato", list[3]);
        }

        [TestMethod()]
        public void ShuffleTest()
        {
            var list = new List<string> { "Orange", "Potato", "Banana", "Milk" };
            list.Shuffle();

            Assert.IsFalse(list[1] == "Orange" && list[1] == "Potato"&& list[3] == "Banana" && list[3] == "Milk");
        }

        [TestMethod()]
        public void RotateTest()
        {
            var list = new List<string> { "Orange", "Potato", "Banana", "Milk" };
            list.Rotate(3);
            Assert.AreEqual("Milk", list[0]);
        }
    }
}