using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyHelper.DAL.Context;
using MyHelper.DAL.Entyties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
        private TargetsGroup _targetsGroup;
        private async Task InitializeTargets()
        {
            var timer = Stopwatch.StartNew();
            _logger.LogInformation("Инициализация целей...");

            _targetsGroup = new TargetsGroup()
            {
                Name = "Targets",
                Year = (uint)_year,
            };
            await _db.TargetsGroups.AddAsync(_targetsGroup);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Инициализация целей выполнена за {0} мс", timer.ElapsedMilliseconds);
        }
    }
}
