using System.Collections.Generic;

namespace MyHelper.Models.Purposes
{
    internal class MyPurposes
    {
        public int Year { get; set; }

        public string? Name { get; set; }

        public IList<MyPurpose> ListPurposes { get; set; } = [];

    }
}