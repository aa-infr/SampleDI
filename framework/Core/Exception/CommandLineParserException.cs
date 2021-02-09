using System.Runtime.Serialization;

namespace Infrabel.ICT.Framework.Exception
{
    public class CommandLineParserException : System.Exception
    {
        /// <summary>
		/// Constructor (overloaded)
		/// </summary>
		public CommandLineParserException()
        {
        }

        /// <summary>
        /// Constructor (overloaded)
        /// </summary>
        public CommandLineParserException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor (overloaded)
        /// </summary>
        public CommandLineParserException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor (overloaded)
        /// </summary>
        protected CommandLineParserException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
