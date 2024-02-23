using MyHelper.ViewModels.Base;
using System.ComponentModel;

namespace MyHelper.Models.Purposes
{
    internal class MyPurposes
    {
        public int Year { get; set; }

        public string? Name { get; set; }

        public BindingList<MyPurpose> ListPurposes { get; set; } = [];

    }

    internal class MyPurpose
    {
        public BindingList<MyPurpose>? Branch { get; set; }
        public bool IsCompleted { get; set; }
        public string? Purpose { get; set; }
        public string? Note { get; set; }
    }
}