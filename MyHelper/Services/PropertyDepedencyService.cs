using MyHelper.Infrastructure.Attributes;
using MyHelper.Services.Interfaces;
using System.Reflection;

namespace MyHelper.Services
{
    internal class PropertyDepedencyService : IPropertyDependency
    {
        Dictionary<string, string[]> _dependencies = [];

        public bool DependencyContains(string key) => _dependencies.ContainsKey(key);

        public void DependeciesClear() => _dependencies.Clear();

        public IReadOnlyCollection<string> GetDependentProperties(string propertyName) =>
            _dependencies.TryGetValue(propertyName, out var dependencies)
            ? dependencies
            : [];

        public void RegisterDependencies(Type viewModelType)
        {
            PropertyInfo[] propertiesInfo = viewModelType.GetProperties();
            _dependencies.Clear();
            foreach (PropertyInfo propertyInfo in propertiesInfo)
            {
                var depedencyAttribute = propertyInfo.GetCustomAttribute<ConnectedPropertiesAttribute>();
                if (depedencyAttribute is not null)
                    _dependencies.Add(propertyInfo.Name, depedencyAttribute.Properties);
            }
        }
    }
}
