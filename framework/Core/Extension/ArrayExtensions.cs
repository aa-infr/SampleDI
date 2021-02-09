using System.IO;
using System;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Exception;

namespace Infrabel.ICT.Framework.Extension
{
    public static class ArrayExtensions
    {
        public static T ParseCommandLine<T>(this string[] args) where T : class, new() => ParseCommandLine<T>(args, Console.Out, Console.Error);

        public static T ParseCommandLine<T>(this string[] args, TextWriter output, TextWriter error) where T : class, new()
        {
            CommandLineParser<T> commandLineParser;

            try
            {
                commandLineParser = new CommandLineParser<T>();
            }
            catch (ArgumentDefinitionException ex)
            {
                error.WriteLine(ex.Message);
                return null;
            }

            try
            {
                commandLineParser.Parse(args);
                if (commandLineParser.IsHelp)
                {
                    commandLineParser.GenerateHelp(output);
                    return null;
                }

                commandLineParser.CheckRequired();

                var parameterClass = new T();
                commandLineParser.GetValues(parameterClass);
                return parameterClass;
            }
            catch (CommandLineParserException ex)
            {
                error.WriteLine(ex.Message);
                commandLineParser.AutoGenerateHelp(error);
                return null;
            }
        }
    }
}