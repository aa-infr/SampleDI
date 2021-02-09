namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public class JsonOptions
    {
        public bool UseNewtonSoftJson { get; set; } = true;
        public bool IgnoreNullValue { get; set; } = true;
        public bool UseCamelCase { get; set; } = true;
        public bool EnumAsString { get; set; } = false;
        public bool BeTolerant { get; set; } = true;
        public bool SerializeReferenceLoop { get; set; } = true;
    }
}