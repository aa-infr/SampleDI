namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public class HttpClientProxyOptions
    {
        public string ProxyUrl { get; set; }
        public bool EnableProxy { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public bool EnableCompression { get; set; }
        public bool EnableFollowRedirect { get; set; }
    }
}