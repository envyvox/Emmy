namespace Emmy.Data.Enums
{
    public enum FractionQuest : byte
    {
        
    }

    public static class FractionQuestHelper
    {
        public static string Localize(this FractionQuest quest)
        {
            return quest switch { };
        }

        public static FractionQuestType Type(this FractionQuest quest)
        {
            return quest switch { };
        }
    }
}