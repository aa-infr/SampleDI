namespace Infrabel.ICT.Framework.Entity
{
    /// <summary>
	/// Describe how parser should react to duplicate named arguments that are not multivalue.
	/// </summary>
	internal enum DuplicateArgumentBehaviour
    {
        /// <summary>
        /// Use last value specified
        /// </summary>
        Last,
        /// <summary>
        /// Use first value specified
        /// </summary>
        First,
        /// <summary>
        /// Fail on duplicate arguments
        /// </summary>
        Fail,
        /// <summary>
        /// Treat as unknown argument
        /// </summary>
        Unknown,
    }
}

