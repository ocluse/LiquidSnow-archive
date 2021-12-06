using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
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
            
            using var alg = IOBuilder.CreateFile(key, cryptoFile);

            //Write to the cryptofile
            await alg.WriteTextAsync(plainText);

            var readOut = await alg.ReadTextAsync();

            Assert.AreEqual(plainText, readOut);
        }

        [TestMethod()]
        public async Task DeserializeSerializeTest()
        {
            Student student = Student.Create();

            using var cryptoFile = IOBuilder.CreateFile(key);
            
            await cryptoFile.SerializeAsync(student);

            Student output = await cryptoFile.DeserializeAsync<Student>();

            Assert.AreEqual(student.Name, output.Name);
        }

        [TestMethod()]
        public async Task ReadWriteTest()
        {
            using MemoryStream data = new MemoryStream(plainText.GetBytes<UTF8Encoding>());

            using var cryptoFile = IOBuilder.CreateFile(key);

            await cryptoFile.WriteAsync(data);

            using MemoryStream output=new MemoryStream();

            await cryptoFile.ReadAsync(output);

            Assert.AreEqual(data.ToArray().GetString<UTF8Encoding>(), output.ToArray().GetString<UTF8Encoding>());

            string outputString = await cryptoFile.ReadTextAsync();

            Assert.AreEqual(plainText, outputString);

            byte[] outputBytes = await cryptoFile.ReadBytesAsync();

            Assert.AreEqual(data.ToArray().GetString<UTF8Encoding>(), outputBytes.GetString<UTF8Encoding>());

            using MemoryStream outputStream = new MemoryStream();

            await cryptoFile.ReadAsync(outputStream);

            Assert.AreEqual(data.ToArray().GetString<UTF8Encoding>(), outputStream.ToArray().GetString<UTF8Encoding>());
        }
    }
}