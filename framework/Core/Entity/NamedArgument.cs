﻿namespace Infrabel.ICT.Framework.Entity
{
    internal class NamedArgument
    {
        /// <summary>
        /// Name of argument without Start of Argument marker (typically - or /)
        /// </summary>
        public string ArgumentName { get; set; }
        /// <summary>
        /// Optional argument value
        /// </summary>
        public string ArgumentValue { get; set; }
    }

}
