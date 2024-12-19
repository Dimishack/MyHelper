using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyHelper.DAL.Context;
using MyHelper.DAL.Entyties;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MyHelper.Data
{
    internal class DbInitializer(MyHelperDB db, ILogger<DbInitializer> logger)
    {
        private readonly MyHelperDB _db = db;
        private readonly ILogger<DbInitializer> _logger = logger;

        public async Task InitializeAsync()
        {
            var timer = Stopwatch.StartNew();
            _logger.LogInformation("Инициализация БД...");
            _logger.LogInformation("Миграция БД...");
            await _db.Database.MigrateAsync().ConfigureAwait(false);
            _logger.LogInformation("Миграция БД выполнена за {0} мс", timer.ElapsedMilliseconds);
            if (!await _db.TargetsGroups.AnyAsync()) await InitializeTargets();
            if (!await _db.Challenges.AnyAsync()) await InitializeChallenges();
            if (!await _db.Tasks.AnyAsync()) await InitializeTasks();
            _logger.LogInformation("Инициализация БД выполнена за {0} с", timer.Elapsed.TotalSeconds);
        }

        private async Task InitializeTargets()
        {
            var timer = Stopwatch.StartNew();
            _logger.LogInformation("Инициализация целей...");

            var targetsGroup = new TargetsGroup()
            {
                Name = "Пожизненные цели",
                Year = 0,
            };
            await _db.TargetsGroups.AddAsync(targetsGroup);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Инициализация целей выполнена за {0} мс", timer.ElapsedMilliseconds);
        }

        private async Task InitializeChallenges()
        {
            var timer = Stopwatch.StartNew();
            _logger.LogInformation("Инициализация челленджей...");

            var challenges = new Collection<Challenge>();
            foreach (var challenge in challenges)
                await _db.Challenges.AddAsync(challenge);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Инициализация челленджей выполнена за {0} мс", timer.ElapsedMilliseconds);
        }

        private async Task InitializeTasks()
        {
            var timer = Stopwatch.StartNew();
            _logger.LogInformation("Инициализация челленджей...");

            Random rnd = new Random();
            var tasks = new Collection<MyTask>(Enumerable.Range(0,20).Select(t => new MyTask()
            {
                Name = $"Task {t}",
                Note = $"Note {t}",
                Prompt = rnd.Next(0,2) == 1,
                Important = rnd.Next(0,2) == 1,
                Group = $"Group {rnd.Next(0,3)}"
            }).ToList());
            foreach (var myTask in tasks)
                await _db.Tasks.AddAsync(myTask);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Инициализация челленджей выполнена за {0} мс", timer.ElapsedMilliseconds);
        }
    }
}
