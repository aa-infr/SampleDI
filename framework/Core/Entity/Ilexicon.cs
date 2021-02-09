namespace Infrabel.ICT.Framework.Entity
{
    public interface ILexicon: IEntityBase
    {
        string Name { get;  }
        string EntityName { get;  }
        string FromProperty { get; }
        string ToProperty { get;  }
    }
}
