using System;

namespace Emmy.Data.Util
{
    public interface IUpdatedEntity
    {
        DateTimeOffset UpdatedAt { get; set; }
    }
}
