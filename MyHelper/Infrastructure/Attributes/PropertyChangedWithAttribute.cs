namespace MyHelper.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    internal class PropertyChangedWithAttribute(string propertyName) : Attribute
    {
        public string PropertyName { get; } = propertyName;
    }
}
