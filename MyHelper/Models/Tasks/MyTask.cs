namespace MyHelper.Models.MyTasks
{
    internal class MyTask
    {
        public int Id { get; set; }
        public string? Task { get; set; }

        public bool Prompt { get; set; }

        public bool Important { get; set; }

        public DateTime? Term { get; set; }

        public string? Group { get; set; }

        public string? Note { get; set; }

    }
}
