namespace Infrabel.ICT.Framework.Entity
{
    public abstract class EntityBase : IEntityBase
    {
        public long Id { get; set; }

        public bool IsNew() => Id == 0L;

        public virtual object Clone() => MemberwiseClone();
    }
}