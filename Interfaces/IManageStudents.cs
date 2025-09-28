using System.Collections.Generic;

namespace CampusLinkApp.Interfaces
{
    public interface IManageStudents<T>
    {
        void Add(T item);
        void Edit(T item);
        void Delete(T item);
        List<T> List();
    }
}
