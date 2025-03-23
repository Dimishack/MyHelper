using System.ComponentModel;

namespace MyHelper.Models.Enums
{
    internal enum FilmStatus
    {
        [Description("Запланирован")]
        Planned = 0,
        [Description("Смотрю")]
        Watching = 1,
        [Description("Просмотрен")]
        Watched = 2,
    }
}
