namespace Infrabel.ICT.Framework.Entity
{
    internal class CommandMatch
    {
        /// <summary>
        /// Matching command line parser
        /// </summary>
        public ICommandLineParser Parser { get; }

        /// <summary>
        /// Aliases associated with command line parser
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        /// Construct CommandMatch object
        /// </summary>
        /// <param name="parser">Parser instance</param>
        /// <param name="aliases">Aliases associated with parser</param>
        public CommandMatch(ICommandLineParser parser, string[] aliases)
        {
            Parser = parser;
            Aliases = aliases;
        }
    }
}