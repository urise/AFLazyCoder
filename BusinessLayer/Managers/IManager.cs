using System.Collections.Generic;

namespace BusinessLayer.Managers
{
    public interface IManager
    {
        void Init(string sourceFolder);
        List<string> GetClassList();
        void AddMethods(string className, IEnumerable<string> methods);
        void AddClass(string serviceName);
    }
}