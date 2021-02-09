namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public abstract class AuthConfigBase
    {
        public bool IsEnabled { get; set; }
        public abstract string AuthenticationScheme { get; }
    }
}