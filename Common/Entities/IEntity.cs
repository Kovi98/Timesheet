using System;

namespace Timesheet.Common
{
    public interface IEntity : IEntityView
    {
        DateTime CreateTime { get; set; }
    }

    public interface IEntityView
    {
        int Id { get; }
    }
}
