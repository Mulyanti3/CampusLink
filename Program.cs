// Program.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace CampusLinkApp
{
    // Person base class demonstrating encapsulation
    public class Person
    {
        private string _name;
        private string _gender;
        private int _age;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value.Trim();
            }
        }

        public string Gender
        {
            get => _gender;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Gender cannot be empty.");
                var g = value.Trim();
                var normalized = char.ToUpper(g[0]) + g.Substring(1).ToLower();
                if (normalized != "Male" && normalized != "Female")
                    throw new ArgumentException("Gender must be 'Male' or 'Female'.");
                _gender = normalized;
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                if (value <= 0 || value > 120)
                    throw new ArgumentException("Age must be a positive number (1-120).");
                _age = value;
            }
        }

        public Person() { }

        public Person(string name, string gender, int age)
        {
            Name = name;
            Gender = gender;
            Age = age;
        }
    }

    // Add this constructor to Student class
    public class Student : Person
    {
        public int StudentID { get; set; } // assigned by StudentManager

        public Student() { }

        public Student(string name, string gender, int age)
            : base(name, gender, age)
        {
        }

        public override string ToString()
        {
            return $"{Name,-25} {Gender,-6} {Age,3}  {StudentID}";
        }
    }

    // Generic interface as required
    public interface IManageStudents<T>
    {
        void Add(T item);
        void Edit(T item);
        void Delete(T item);
        List<T> List();
    }

    // StudentManager implementing the interface
    public class StudentManager : IManageStudents<Student>
    {
        private readonly List<Student> _students = new List<Student>();
        private static int _nextId = 1000; // start id (you can change or use GUID)

        public void Add(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            item.StudentID = _nextId++;
            _students.Add(item);
        }

        public void Edit(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var existing = _students.FirstOrDefault(s => s.StudentID == item.StudentID);
            if (existing == null) throw new KeyNotFoundException($"Student ID {item.StudentID} not found.");
            existing.Name = item.Name;
            existing.Gender = item.Gender;
            existing.Age = item.Age;
        }

        public void Delete(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var existing = _students.FirstOrDefault(s => s.StudentID == item.StudentID);
            if (existing == null) throw new KeyNotFoundException($"Student ID {item.StudentID} not found.");
            _students.Remove(existing);
        }

        public List<Student> List()
        {
            // return a shallow copy to protect internal list
            return new List<Student>(_students);
        }

        public Student GetById(int id) => _students.FirstOrDefault(s => s.StudentID == id);

        public List<Student> SortByName() => _students.OrderBy(s => s.Name).ToList();
        public List<Student> SortByAge() => _students.OrderBy(s => s.Age).ToList();
    }

    class Program
    {
        static StudentManager manager = new StudentManager();

        static void Main()
        {
            Console.WriteLine("=== CampusLink - Student Manager ===");
            while (true)
            {
                ShowMenu();
                Console.Write("Choose option: ");
                var choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1": RegisterStudent(); break;
                        case "2": ViewRoster(manager.List()); break;
                        case "3": EditStudent(); break;
                        case "4": DeleteStudent(); break;
                        case "5": SortRoster(); break;
                        case "6": Console.WriteLine("Exiting..."); return;
                        default: Console.WriteLine("Invalid option. Try again."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("1) Register new student");
            Console.WriteLine("2) View student roster");
            Console.WriteLine("3) Edit student details");
            Console.WriteLine("4) Delete student record");
            Console.WriteLine("5) Sort roster (Name / Age)");
            Console.WriteLine("6) Exit");
            Console.WriteLine();
        }

        static void RegisterStudent()
        {
            Console.WriteLine("-- Register Student --");
            var name = ReadRequired("Name: ");
            var gender = ReadGender("Gender (Male/Female): ");
            var age = ReadPositiveInt("Age: ");

            var student = new Student(name, gender, age);
            manager.Add(student);
            Console.WriteLine($"\nStudent added successfully. Assigned StudentID: {student.StudentID}");
        }

        static void ViewRoster(List<Student> students)
        {
            Console.WriteLine("-- Student Roster --");
            if (students == null || !students.Any())
            {
                Console.WriteLine("No students registered yet.");
                return;
            }

            Console.WriteLine($"{"Name",-25} {"Gender",-6} {"Age",3}  ID");
            Console.WriteLine(new string('-', 50));
            foreach (var s in students)
                Console.WriteLine(s.ToString());
        }

        static void EditStudent()
        {
            Console.WriteLine("-- Edit Student --");
            var id = ReadPositiveInt("Enter StudentID to edit: ");
            var existing = manager.GetById(id);
            if (existing == null)
            {
                Console.WriteLine($"Student ID {id} not found.");
                return;
            }

            Console.WriteLine("Leave blank to keep current value.");
            Console.WriteLine($"Current Name: {existing.Name}");
            var name = ReadOptional("New Name: ");
            Console.WriteLine($"Current Gender: {existing.Gender}");
            var gender = ReadOptionalGender("New Gender (Male/Female): ");
            Console.WriteLine($"Current Age: {existing.Age}");
            var age = ReadOptionalInt("New Age: ");

            var updated = new Student
            {
                StudentID = existing.StudentID,
                Name = string.IsNullOrEmpty(name) ? existing.Name : name,
                Gender = string.IsNullOrEmpty(gender) ? existing.Gender : gender,
                Age = age == null ? existing.Age : age.Value
            };

            manager.Edit(updated);
            Console.WriteLine("Student updated successfully.");
        }

        static void DeleteStudent()
        {
            Console.WriteLine("-- Delete Student --");
            var id = ReadPositiveInt("Enter StudentID to delete: ");
            var existing = manager.GetById(id);
            if (existing == null)
            {
                Console.WriteLine($"Student ID {id} not found.");
                return;
            }
            Console.Write($"Are you sure you want to delete {existing.Name} (ID {existing.StudentID})? (y/n): ");
            var confirm = Console.ReadLine()?.Trim().ToLower();
            if (confirm == "y" || confirm == "yes")
            {
                manager.Delete(existing);
                Console.WriteLine("Student deleted.");
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }
        }

        static void SortRoster()
        {
            Console.WriteLine("-- Sort Roster --");
            Console.WriteLine("1) By Name");
            Console.WriteLine("2) By Age");
            Console.Write("Choose: ");
            var c = Console.ReadLine()?.Trim();
            if (c == "1")
                ViewRoster(manager.SortByName());
            else if (c == "2")
                ViewRoster(manager.SortByAge());
            else
                Console.WriteLine("Invalid choice.");
        }

        // Utilities for reading and validating input
        static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) return input.Trim();
                Console.WriteLine("Input required.");
            }
        }

        static string ReadGender(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) { Console.WriteLine("Gender required."); continue; }
                var normalized = char.ToUpper(input.Trim()[0]) + input.Trim().Substring(1).ToLower();
                if (normalized == "Male" || normalized == "Female") return normalized;
                Console.WriteLine("Invalid gender. Enter 'Male' or 'Female'.");
            }
        }

        static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (int.TryParse(input, out int val) && val > 0) return val;
                Console.WriteLine("Enter a valid positive integer.");
            }
        }

        static string ReadOptional(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        }

        static string ReadOptionalGender(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return null;
            var normalized = char.ToUpper(input.Trim()[0]) + input.Trim().Substring(1).ToLower();
            if (normalized == "Male" || normalized == "Female") return normalized;
            Console.WriteLine("Invalid gender. Keeping old value.");
            return null;
        }

        static int? ReadOptionalInt(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (int.TryParse(input, out int val) && val > 0) return val;
            Console.WriteLine("Invalid number. Keeping old value.");
            return null;
        }
    }
}
