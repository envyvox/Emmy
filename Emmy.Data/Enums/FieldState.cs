using System;

namespace Emmy.Data.Enums
{
    public enum FieldState : byte
    {
        Empty = 0,
        Planted = 1,
        Watered = 2,
        Completed = 3
    }

    public static class FieldStateHelper
    {
        public static string Localize(this FieldState state) => state switch
        {
            FieldState.Empty => "Пустая",
            FieldState.Planted => "Засажена",
            FieldState.Watered => "Полита",
            FieldState.Completed => "Готово к сбору",
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}