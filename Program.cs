using System;
using System.Collections.Generic;
using System.Linq;
using CampusLinkApp.Models;
using CampusLinkApp.Managers;

namespace CampusLinkApp
{
    class Program
    {
        static StudentManager manager = new StudentManager();

        static void Main()
        {
            Console.WriteLine(" CampusLink - Student Manager ");
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
            Console.WriteLine(" Register Student ");
            var name = ReadRequired("Name: ");
            var gender = ReadGender("Gender (Male/Female): ");
            var age = ReadPositiveInt("Age: ");

            var student = new Student(name, gender, age);
            manager.Add(student);
            Console.WriteLine($"\nStudent added successfully. Assigned StudentID: {student.StudentID}");
        }

        static void ViewRoster(List<Student> students)
        {
            Console.WriteLine(" Student Roster ");
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
            Console.WriteLine(" Edit Student ");
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
            Console.WriteLine(" Delete Student ");
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
            Console.WriteLine(" Sort Roster ");
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

        static string? ReadOptional(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        }

        static string? ReadOptionalGender(string prompt)
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
