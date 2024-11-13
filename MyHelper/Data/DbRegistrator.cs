using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHelper.DAL.Context;
using Microsoft.EntityFrameworkCore;
using MyHelper.DAL;

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
                    //case "MSSQL":
                    //    opt.UseS
                    //    break;
                    case "SQLite":
                        opt.UseSqlite(configuration.GetConnectionString(type));
                        break;

                    default:
                        throw new InvalidOperationException($"Тип подключения {type} не поддерживается");
                }
            })
            .AddTransient<DbInitializer>()
            .AddRepositoriesInDB()
            ;
    }
}
