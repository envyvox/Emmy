using System;

namespace Emmy.Data.Enums
{
    public enum Title : byte
    {
        Newbie = 1,
        Yatagarasu = 2,
        HarbingerOfSummer = 3,
        FirstSamurai = 4,
        BelievingInLuck = 5,
        KingExcitement = 6,
        Toxic = 7,
        StockyFarmer = 8,
        WineSamurai = 9,
        DescendantOcean = 10,
        ResourcefulCatcher = 11,
        Lucky = 12
    }

    public static class TitleHelper
    {
        public static string Localize(this Title title)
        {
            return title switch
            {
                Title.Newbie => "Новичок",
                Title.Yatagarasu => "Ятагарасу",
                Title.HarbingerOfSummer => "Предвестник лета",
                Title.FirstSamurai => "Первый самурай",
                Title.BelievingInLuck => "Верящий в удачу",
                Title.KingExcitement => "Король азарта",
                Title.Toxic => "Токсичный",
                Title.StockyFarmer => "Запасливый фермер",
                Title.WineSamurai => "Винный самурай",
                Title.DescendantOcean => "Потомок океана",
                Title.ResourcefulCatcher => "Находчивый ловец",
                Title.Lucky => "Приносящий удачу",
                _ => throw new ArgumentOutOfRangeException(nameof(title), title, null)
            };
        }

        public static string EmoteName(this Title title)
        {
            return "Title" + title;
        }
    }
}