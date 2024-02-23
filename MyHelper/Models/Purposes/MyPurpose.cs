using System.ComponentModel;

namespace MyHelper.Models.Purposes
{
    internal class MyPurpose
    {
        public BindingList<MyPurpose>? Branch { get; set; }
        public bool IsCompleted { get; set; }
        public string? Purpose { get; set; }
        public string? Note { get; set; }
    }
}