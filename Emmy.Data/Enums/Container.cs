namespace Emmy.Data.Enums
{
    public enum Container : byte
    {
        Token = 1,
        Supply = 2
    }

    public static class ContainerHelper
    {
        public static string EmoteName(this Container container)
        {
            return "Container" + container;
        }
    }
}