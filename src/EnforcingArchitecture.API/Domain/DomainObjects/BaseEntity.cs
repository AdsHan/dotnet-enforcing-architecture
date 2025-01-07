using EnforcingArchitecture.API.Domain.Enums;

namespace EnforcingArchitecture.API.Domain.DomainObjects;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public EntityStatusEnum Status { get; set; }
    public DateTime DateCreateAt { get; private set; }

    protected BaseEntity()
    {
        DateCreateAt = DateTime.Now;
        Status = EntityStatusEnum.Active;
    }
}