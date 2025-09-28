using System;

namespace CampusLinkApp.Models
{
    public class Person
    {
        private string _name = string.Empty;
        private string _gender = string.Empty;
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
                if (value <= 18 || value > 80)
                    throw new ArgumentException("Age must be a positive number (19-80).");
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
}
