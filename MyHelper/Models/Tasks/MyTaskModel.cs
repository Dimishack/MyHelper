using MyHelper.DAL.Entyties;

namespace MyHelper.Models.Tasks
{
    internal class MyTaskModel(MyTask myTask)
    {
        private readonly MyTask _myTask = myTask;

        public int Id { get => _myTask.Id; }
        public string Name { get => _myTask.Name; set => _myTask.Name = value; }
        public string? Note { get => _myTask.Note; set => _myTask.Note = value; }
        public bool Prompt { get => _myTask.Prompt; set => _myTask.Prompt = value; }
        public bool Important { get => _myTask.Important; set => _myTask.Important = value; }
        public string Group { get => _myTask.Group; set => _myTask.Group = value; }
    }
}
