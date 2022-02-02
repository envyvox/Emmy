using System;

namespace Emmy.Data.Util
{
    public interface ICreatedEntity
    {
        DateTimeOffset CreatedAt { get; set; }
    }
}
