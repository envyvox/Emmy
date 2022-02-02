using System;

namespace Emmy.Data.Util
{
    public interface IExpirationEntity
    {
        DateTimeOffset Expiration { get; set; }
    }
}