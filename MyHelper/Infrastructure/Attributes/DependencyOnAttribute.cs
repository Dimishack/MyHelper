namespace MyHelper.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    internal class DependencyOnAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}