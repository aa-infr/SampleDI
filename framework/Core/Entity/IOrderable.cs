namespace Infrabel.ICT.Framework.Entity
{
    public interface IOrderable
    {
        int OrderNumber { get; set; }
        bool IsDefault { get; set; }
    }
}