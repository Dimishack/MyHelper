using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyHelper.Models.Purposes
{
    internal class MyPurposes
    {
        public int Year { get; set; }

        public string? Name { get; set; }

        public ObservableCollection<MyPurpose> ListPurposes { get; set; } = [];

    }
}