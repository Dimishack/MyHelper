namespace MyHelper.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    internal sealed class DependencyOnAttribute(string[] propertiesName) : Attribute
    {
        public string[] PropertiesName { get; } = propertiesName;

        public DependencyOnAttribute(string propertyName) : this([propertyName]) { }
    }
}