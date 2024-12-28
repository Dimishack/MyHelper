namespace MyHelper.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class IsMoveToTreeAttribute(bool isMove = true) : Attribute
    {
        public bool IsMoveToTree { get; set; } = isMove;
    }
}
