using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Thismaker.Horus.Symmetrics.Tests
{
    [TestClass()]
    public class SymmetricTests
    {
        readonly string Plaintext="Where is my mother";
        [TestMethod()]
        public async Task SymmetricTest()
        {
            var alg = PredefinedSymmetric.AesFixed;
            using var msInput = new MemoryStream(Plaintext.GetBytes<UTF8Encoding>());
            using var msOutput = new MemoryStream();

            await alg.EncryptAsync(msInput, msOutput, "Smith".GetBytes<UTF8Encoding>());
            msOutput.Position = 0;
            using var msFinal = new MemoryStream();
            await alg.DecryptAsync(msOutput, msFinal, "Smith".GetBytes<UTF8Encoding>());

            Assert.AreEqual(Plaintext, msFinal.ToArray().GetString<UTF8Encoding>());
        }
    }
}