using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Thismaker.Horus.IO.Tests
{
    [TestClass()]
    public class CryptoContainerTests
    {
        readonly string fileName = "Inverse";
        readonly string containerPath = "container.sk";
        readonly string plainText = "Where is my mother";
        readonly string key = "Ginger";
        
        [TestMethod()] 
        public async Task CryptoContainerTest()
        {
            using var fsContainer = File.Create(containerPath);
            using ICryptoContainer container=IOBuilder.CreateContainer(key, fsContainer);
            await container.AddTextAsync(fileName, plainText, true);

            Student student = Student.Create();

            var readBack = await container.GetTextAsync(fileName);
            await container.AddAsync("student", student, true);
            var readStudent = await container.GetAsync<Student>("student");
            Assert.AreEqual(plainText, readBack);
            Assert.IsTrue(Student.AreEqual(student, readStudent));

            File.WriteAllText("input.txt", plainText);

            using FileStream fsFile = File.OpenRead("input.txt");
            
            await container.AddAsync("file", fsFile, true, null);

            using FileStream fs = File.Open("output.txt", FileMode.Create);

            await container.GetAsync("file", fs,null);

            fs.Close();

            Assert.AreEqual(plainText, File.ReadAllText("output.txt"));
        }
    }

    public class Student
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Class { get; set; }

        public static Student Create()
        {
            return new Student
            {
                DateOfBirth = DateTime.Today,
                Class = 3,
                Name = "Ivy Rose"
            };
        }

        public static bool AreEqual(Student a, Student test)
        {
            return a.Name == test.Name && a.DateOfBirth == test.DateOfBirth && a.Class == test.Class;
        }
    }
}