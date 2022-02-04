using System;
using System.Collections.Generic;
using System.Linq;

namespace Timesheet.Common.Models
{
    public abstract class ImportBase<TEntity, TError>
        where TEntity : class, IEntity
        where TError : struct, Enum
    {
        public bool Success { get; set; }
        public abstract ICollection<TError> NotPassableErrors { get; }
        public virtual bool CanPassErrors => !Errors.Any(x => NotPassableErrors.Contains(x));
        public TEntity Entity { get; set; }
        public ICollection<TError> Errors { get; set; }
        public ImportBase(TEntity entity, ICollection<TError> errors = null)
        {
            Entity = entity;
            Success = errors == null || errors.Count == 0;
            Errors = errors ?? new HashSet<TError>();
        }
        public void AddError(TError error)
        {
            if (Success) Success = false;
            Errors.Add(error);
        }
    }
}
