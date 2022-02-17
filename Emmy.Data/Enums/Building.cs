using System;

namespace Emmy.Data.Enums
{
    public enum Building : byte
    {
        Farm = 1,
        FarmExpansionL1 = 2,
        FarmExpansionL2 = 3
    }

    public static class BuildingTypeHelper
    {
        public static string Localize(this Building building)
        {
            return building switch
            {
                Building.Farm => "",
                Building.FarmExpansionL1 => "",
                Building.FarmExpansionL2 => "",
                _ => throw new ArgumentOutOfRangeException(nameof(building), building, null)
            };
        }

        public static string Description(this Building building)
        {
            return building switch
            {
                Building.Farm => "",
                Building.FarmExpansionL1 => "",
                Building.FarmExpansionL2 => "",
                _ => throw new ArgumentOutOfRangeException(nameof(building), building, null)
            };
        }
    }
}