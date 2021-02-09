namespace Infrabel.ICT.Framework.Entity
{
    public interface ILexiconItem: IEntityBase
    {
        long? LexiconId { get; }
        string Value { get; }
        long? TargetItemId { get; }
        long? LanguageId { get; }

    }
}
