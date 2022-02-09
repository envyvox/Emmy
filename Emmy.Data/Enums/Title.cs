using System;

namespace Emmy.Data.Enums
{
    public enum Title : byte
    {
        Newbie = 1,
        Yatagarasu = 2
    }

    public static class TitleHelper
    {
        public static string Localize(this Title title)
        {
            return title switch
            {
                Title.Newbie => "Новичок",
                Title.Yatagarasu => "Ятагарасу",
                _ => throw new ArgumentOutOfRangeException(nameof(title), title, null)
            };
        }

        public static string EmoteName(this Title title)
        {
            return "Title" + title;
        }
    }
}