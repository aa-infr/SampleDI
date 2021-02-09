using System;

namespace Infrabel.ICT.Framework.Entity
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class ArgumentDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Description test
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">Description of an argument target or argument class</param>
        public ArgumentDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}