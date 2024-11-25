namespace MyHelper.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    internal sealed class ChangesWithPropertiesAttribute : Attribute
    {
        public Dictionary<string, bool> PropertiesName { get; } = [];

        public ChangesWithPropertiesAttribute(string[] propertiesName, bool[]? IsNexts = null)
        {
            int index = 0;
            if (IsNexts != null)
            {
                var minLenght = Math.Min(propertiesName.Length, IsNexts.Length);
                while (index < minLenght)
                    PropertiesName.TryAdd(propertiesName[index], IsNexts[index++]);
            }
            while (index < propertiesName.Length)
                PropertiesName.TryAdd(propertiesName[index++], false);
        }

        public ChangesWithPropertiesAttribute(string propertyName, bool isNext = false) 
            : this([propertyName], isNext ? [isNext] : null) { }
    }
}
