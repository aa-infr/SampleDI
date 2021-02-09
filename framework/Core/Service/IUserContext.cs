namespace Infrabel.ICT.Framework.Service
{
    public interface IUserContext
    {
        IUserIdentity Identity { get; }

        void ClearUserIdentity();
    }
}