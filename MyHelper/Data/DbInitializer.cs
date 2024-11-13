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
            if (await _db.TargetsGroups.AnyAsync()) return;
            await InitializeTargets();
            _logger.LogInformation("Инициализация БД выполнена за {0} с", timer.Elapsed.TotalSeconds);
        }

        private readonly int _year = DateTime.Now.Year;
        private TargetsGroup? _targetsGroup;
        private async Task InitializeTargets()
        {
            var timer = Stopwatch.StartNew();
            _logger.LogInformation("Инициализация целей...");

            var rnd = new Random();
            _targetsGroup = new TargetsGroup()
            {
                Name = "Пожизненные цели",
                Year = 0,
                Targets = new Collection<Target>(Enumerable.Range(1, 10).Select(i => new Target()
                {
                    IsComplete = Random.Shared.Next(0, 2) == 1,
                    Name = "GroupsTargets " + i,
                    Note = "Note" + i
                }).ToList())
            };
            await _db.TargetsGroups.AddAsync(_targetsGroup);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Инициализация целей выполнена за {0} мс", timer.ElapsedMilliseconds);
        }
    }
}
