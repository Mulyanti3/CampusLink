using System;
using System.Collections.Generic;
using System.Linq;
using CampusLinkApp.Interfaces;
using CampusLinkApp.Models;

namespace CampusLinkApp.Managers
{
    public class StudentManager : IManageStudents<Student>
    {
        private readonly List<Student> _students = new List<Student>();
        private static int _nextId = 1000;

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
            return new List<Student>(_students);
        }

        public Student GetById(int id) => _students.FirstOrDefault(s => s.StudentID == id);

        public List<Student> SortByName() => _students.OrderBy(s => s.Name).ToList();

        public List<Student> SortByAge() => _students.OrderBy(s => s.Age).ToList();
    }
}
