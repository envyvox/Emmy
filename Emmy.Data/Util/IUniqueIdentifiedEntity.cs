using System;

namespace Emmy.Data.Util
{
    public interface IUniqueIdentifiedEntity
    {
        Guid Id { get; set; }
    }
}
