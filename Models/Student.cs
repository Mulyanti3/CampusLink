namespace CampusLinkApp.Models
{
    public class Student : Person
    {
        public int StudentID { get; set; }

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
}
