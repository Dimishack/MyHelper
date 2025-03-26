using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Services.Interfaces
{
    internal interface IPropertyDependency
    {
        void RegisterDependencies(Type viewModelType);
        bool DependencyContains(string key);
        IReadOnlyCollection<string> GetDependentProperties(string propertyName);
        void DependeciesClear();
    }
}
