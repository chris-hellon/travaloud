using System.ComponentModel.DataAnnotations.Schema;

namespace Travaloud.Domain.Common.Contracts;

public abstract class AuditableEntity : AuditableEntity<DefaultIdType>
{
}

public abstract class AuditableEntity<T> : BaseEntity<T>, IAuditableEntity, ISoftDelete
{
    public DefaultIdType CreatedBy { get; set; }
    
    [NotMapped]
    public bool OverrideCreatedBy { get; set; }
    
    public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;
    public DefaultIdType LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedOn { get; set; }
    public DefaultIdType? DeletedBy { get; set; }

    public void FlagAsDeleted(DefaultIdType userId)
    {
        DeletedOn = DateTime.Now;
        DeletedBy = userId;
    }
}