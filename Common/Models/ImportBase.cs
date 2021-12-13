using System.Collections.Generic;

namespace Timesheet.Common.Models
{
    public abstract class ImportBase<TEntity, TError>
        where TEntity : class, IEntity
        where TError : struct
    {
        public bool Success { get; set; }
        public abstract bool CanPassErrors { get; }
        public TEntity Entity { get; set; }
        public List<TError> Errors { get; set; }
        public ImportBase(TEntity entity, List<TError> errors = null)
        {
            Entity = entity;
            Success = errors == null || errors.Count == 0;
            Errors = errors ?? new List<TError>();
        }
        public void AddError(TError error)
        {
            if (Success) Success = false;
            Errors.Add(error);
        }
    }
}
