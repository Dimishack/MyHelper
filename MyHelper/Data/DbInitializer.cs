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
            try
            {
                await _db.Database.MigrateAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                await _db.Database.EnsureDeletedAsync().ConfigureAwait(false);
                await _db.Database.MigrateAsync().ConfigureAwait(false);
            }
            _logger.LogInformation("Миграция БД выполнена за {0} мс", timer.ElapsedMilliseconds);

            if (!await _db.TargetsGroups.AnyAsync()) await InitializeAsync(InitializeTargetsAsync, "целей");
            if (!await _db.Genres.AnyAsync()) await InitializeAsync(InitializeGenresAsync, "жанров");
            if (!await _db.Films.AnyAsync()) await InitializeAsync(InitializeMoviesAsync, "видео");
            if (!await _db.FilmGenres.AnyAsync()) await InitializeAsync(InitializeFilmGenresAsync);

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

        string[] _genres = [ "аниме", "биографический", "боевик", "вестерн", "военный", "детектив", "детский",
                            "документальный", "драма", "исторический", "кинокомикс", "комедия", "криминал",
                            "мелодрама", "мистика", "музыкальный", "мюзикл", "научный", "нуар", "приключения",
                            "семейный", "спорт", "триллер", "ужасы", "фантастика", "фэнтези", "эротика"];

        private async Task InitializeGenresAsync()
        {
            foreach (var genre in _genres)
                await _db.Genres.AddAsync(new Genre() { Name = genre });
        }

        private const int MAXSIZEFILM = 1000;

        private async Task InitializeMoviesAsync()
        {
            foreach (var index in Enumerable.Range(1, MAXSIZEFILM))
                await _db.Films.AddAsync(new Film()
                {
                    Name = $"Movie {index}",
                    Producer = $"Producer {index}",
                    Format = Random.Shared.Next(0, 10),
                    Raiting = Random.Shared.Next(1, 11),
                    ReleaseYear = 2000 + Random.Shared.Next(1, 24),
                    Status = Random.Shared.Next(0, 3),
                });
        }

        private async Task InitializeFilmGenresAsync()
        {
            if (!await _db.Genres.AnyAsync() && !await _db.Films.AnyAsync()) return;

            foreach (var index in Enumerable.Range(1, MAXSIZEFILM))
            {
                int genreCount = Random.Shared.Next(1, _genres.Length / 2);
                int[] genreIds = new int[genreCount];
                for (int i = 0; i < genreCount; i++)
                {
                    int genreId = -1;
                    do
                    {
                        genreId = Random.Shared.Next(0, _genres.Length);
                    } while (Array.IndexOf(genreIds, genreId) != -1);
                    genreIds[i] = genreId;

                    await _db.FilmGenres.AddAsync(new FilmGenre()
                    {
                        FilmId = index,
                        GenreId = genreId,
                    });
                }
            }
        }
    }
}
