using System.ComponentModel;

namespace MyHelper.Models.Enums
{
    internal enum FilmFormat
    {
        [Description("Полнометражный фильм")]
        Feature_Film = 0,
        [Description("Короткометражный фильм")]
        Short_Film = 1,
        [Description("ТВ-сериал")]
        TV_Series = 2,
        [Description("Мини-сериал")]
        Miniseries = 3,
        [Description("ТВ-фильм")]
        TV_Movie = 4,
        [Description("Веб-сериал")]
        Web_Series = 5,
        [Description("Реалити-шоу")]
        Reality_Show = 6,
        [Description("Ток-шоу")]
        Talk_Show = 7,
        [Description("Концерт")]
        Concert = 8,
        [Description("Музыкальное видео")]
        Music_Video = 9
    }
}
