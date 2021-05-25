using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thismaker.Horus.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Thismaker.Horus.IO.Tests
{
    [TestClass()]
    public class CryptoFileTests
    {
        readonly string plainName = "plainfile.txt";
        readonly string cipherName = "cipherfile.txt";
        readonly string plainText = "Where is my mother";
        readonly string key = "Ginger";
        [TestMethod()]
        public async Task CryptoFileTest()
        {
            using var cryptoFile = File.Open(cipherName, FileMode.Create);
            using var alg = new CryptoFile(cryptoFile, key);

            //Write to the cryptofile
            await alg.WriteTextAsync(plainText);

            var readOut = await alg.ReadTextAsync();

            Assert.AreEqual(plainText, readOut);
        }
    }
}