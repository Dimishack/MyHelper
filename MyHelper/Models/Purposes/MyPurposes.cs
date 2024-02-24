namespace MyHelper.Models.Purposes
{
    internal class MyPurposes
    {
        public int Year { get; set; }

        public string? Name { get; set; }

        public SortableBindingList<MyPurpose> ListPurposes { get; set; } = [];

    }
}