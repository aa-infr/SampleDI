using System;

namespace Infrabel.ICT.Framework.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ArgumentNameAttribute : Attribute
    {
        /// <summary>
        /// Name/Alias of a named argument
        /// </summary>
        public string[] Names { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="names">Name/Aliases of a named argument</param>
        public ArgumentNameAttribute(params string[] names)
        {
            Names = names;
        }
    }
}
