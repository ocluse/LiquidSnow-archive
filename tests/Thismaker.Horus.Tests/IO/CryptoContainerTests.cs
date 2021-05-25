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
            using var container = new CryptoContainer(fsContainer, key);
            await container.AddTextAsync(fileName, plainText, true);

            var student = new Student
            {
                DateOfBirth = DateTime.Today,
                Class = 3,
                Name = "Ivy Rose"
            };

            var readBack = await container.GetTextAsync(fileName);
            await container.AddAsync("student", student, true);
            var readStudent = await container.GetAsync<Student>("student");
            Assert.AreEqual(plainText, readBack);

            Assert.AreEqual(student, readStudent);
        }
    }

    public class Student
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Class { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() == typeof(Student))
            {
                var test = (Student)obj;
                return Name == test.Name && DateOfBirth == test.DateOfBirth && Class == test.Class;
            }
            return false;
        }
    }
}