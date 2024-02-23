using MyHelper.ViewModels.Base;
using System.ComponentModel;

namespace MyHelper.Models.Purposes
{
    internal class MyPurposes
    {
        public string? Name { get; set; }

        public BindingList<MyPurpose>? ListPurposes { get; set; }

    }

    internal class MyPurpose : ViewModel
    {
        private string? _purpose;
        public string? Purpose
        {
            get => _purpose;
            set => Set(ref _purpose, value);
        }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set => Set(ref _isCompleted, value);
        }
    }
}