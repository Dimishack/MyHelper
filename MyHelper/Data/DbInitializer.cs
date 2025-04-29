using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyHelper.DAL.Context;
using MyHelper.DAL.Entyties;
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

            if (!await _db.TargetsGroups.AnyAsync()) await InitializeAsync(InitializeTargetsAsync, "целей");
            if (!await _db.Genres.AnyAsync()) await InitializeAsync(InitializeGenresAsync, "жанров");

            _logger.LogInformation("Инициализация БД выполнена за {0} с", timer.Elapsed.TotalSeconds);
        }

        private async Task InitializeAsync(Func<Task> initialize, string message = "коллекции")
        {
            var timer = Stopwatch.StartNew();
            _logger.LogInformation($"Инициализация {message}...");
            await initialize();
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Инициализация {message} выполнена за {0} мс", timer.ElapsedMilliseconds);
        }

        private async Task InitializeTargetsAsync()
        {
            var targetsGroup = new TargetsGroup()
            {
                Name = "Пожизненные цели",
                Year = 0,
            };
            await _db.TargetsGroups.AddAsync(targetsGroup);
        }

        string[] _genres = [ "аниме", "биография", "боевик", "вестерн", "военный", "детектив", "детский",
                            "документальный", "драма", "история", "кинокомикс", "комедия", "криминал",
                            "мелодрама", "мистика", "музыка", "мультфильм", "мюзикл", "научный", "нуар", "приключения",
                            "семейный", "спорт", "триллер", "ужасы", "фантастика", "фэнтези", "эротика"];

        private async Task InitializeGenresAsync()
        {
            int index = 1;
            foreach (var genre in _genres)
                await _db.Genres.AddAsync(new Genre() { Id = index++, Name = genre });
        }
    }
}
