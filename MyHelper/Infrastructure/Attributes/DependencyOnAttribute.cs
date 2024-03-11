namespace MyHelper.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    internal class DependencyOnAttribute : Attribute
    {
        public string Name { get; }

        public DependencyOnAttribute(string name) => Name = name;
    }
}