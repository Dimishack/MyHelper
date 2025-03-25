using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHelper.DAL;
using MyHelper.DAL.Context;
using System.Globalization;

namespace MyHelper.Data
{
    internal static class DbRegistrator
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration) => services
            .AddDbContext<MyHelperDB>(opt =>
            {
                var type = configuration["Type"];
                switch (type)
                {
                    case null: throw new ArgumentNullException("Не определен тип БД");
                    case "SQLite":
                        {
                            var connection = new SqliteConnection(configuration.GetConnectionString(type));
                            connection.Open();
                            connection.CreateCollation("RUSSIAN_NOCASE", (x, y) =>
                                string.Compare(x, y, CultureInfo.GetCultureInfo("ru-RU"), CompareOptions.IgnoreCase));

                            opt.UseSqlite(connection);
                            break;
                        }

                    default:
                        throw new InvalidOperationException($"Тип подключения {type} не поддерживается");
                }
            })
            .AddTransient<DbInitializer>()
            .AddRepositoriesInDB()
            ;
    }
}
