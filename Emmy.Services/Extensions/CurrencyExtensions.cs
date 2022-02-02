using System;

namespace Emmy.Services.Extensions
{
    public static class CurrencyExtensions
    {
        public static uint ConvertTokensToLobbs(this uint tokensAmount)
        {
            return (uint) Math.Ceiling((decimal) (tokensAmount / 100.0));
        }
    }
}