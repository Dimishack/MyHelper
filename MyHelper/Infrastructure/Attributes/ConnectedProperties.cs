namespace MyHelper.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class ConnectedPropertiesAttribute(params string[] properties) : Attribute
    {
        private readonly string[] _properties = properties;

        public string[] Properties => _properties;
    }
}
