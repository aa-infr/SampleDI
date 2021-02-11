namespace Infrabel.ICT.Framework.Service
{
    public class OptionsResolver<TOptions> : IOptionsResolver<TOptions> where TOptions : class, new()
    {
        private readonly IOptionsResolver _resolver;

        public OptionsResolver(IOptionsResolver resolver)
        {
            _resolver = resolver;
        }

        public TOptions Resolve()
        {
            return _resolver.Resolve<TOptions>();
        }
    }
}