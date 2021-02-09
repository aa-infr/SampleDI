namespace Infrabel.ICT.Framework.Entity
{
    public interface ILanguage: IEntityBase
    {
        string Name { get; }
        string IsoCode { get; }
        bool Default { get; }
    }
}
