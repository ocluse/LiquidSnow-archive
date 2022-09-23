using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Esna;
using Thismaker.Horus;

namespace Thismaker.Esna.ConsoleTest
{
    internal class App
    {
        private readonly string _dirPath = "database";
        private readonly string _horusPath = "horus.zip";
        public IContainerHandler<Student, Student> Students { get; set; }
        public async Task RunAsync()
        {
            var containerSettings = new ContainerSettings<Student, Student>(pkPropertyName: "Stream");
            Students = StorageContainerHandler<Student, Student>.CreateCryptoContainerHandler(_horusPath,"thismaker", containerSettings);

            while (true)
            {
                var command=Console.ReadLine();

                if (command.StartsWith("create"))
                {
                    await CreateAsync(command[7..]);
                }
                else if (command.StartsWith("delete"))
                {
                    Console.Write("Stream: ");
                    var stream = Console.ReadLine();
                    
                    if(!await Students.DeleteAsync(command[7..], stream))
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Not Deleted");
                        Console.ResetColor();
                    }
                }
                else if (command.StartsWith("read"))
                {
                    Console.Write("Stream: ");
                    var stream = Console.ReadLine();

                    try
                    {
                        var student = await Students.ReadAsync(command[5..], stream);
                        WriteStudent(student);
                    }
                    catch (ResourceNotFoundException) 
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("NOT FOUND");
                        Console.ResetColor();
                    }
                }
                else if (command.StartsWith("find"))
                {
                    Console.Write("Stream: ");
                    var stream = Console.ReadLine();

                    var students = await Students.Query().Where(x => x.Stream == stream).OrderBy(x=>x.Form).ExecuteAsync();

                    foreach(var student in students)
                    {
                        WriteStudent(student);
                    }
                }
            }
        }
        private void WriteStudent(Student student)
        {
            Console.WriteLine();
            Console.WriteLine($"\tName: {student.Name}");
            Console.WriteLine($"\tForm: {student.Form}");
            Console.WriteLine($"\tStream: {student.Stream}");
            Console.WriteLine();
        }

        private async Task CreateAsync(string id)
        {
            Student student = new();
            student.Id = id;
            Console.Write("Name: ");
            student.Name = Console.ReadLine();
            Console.Write("Form: ");
            student.Form = int.Parse(Console.ReadLine());
            Console.Write("Stream: ");
            student.Stream = Console.ReadLine();

            await Students.CreateAsync(student);
        }
    }

    public class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Form { get; set; }
        public string Stream { get; set; }
    }
}
