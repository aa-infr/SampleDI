using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Infrabel.ICT.Framework.Exception;

namespace Infrabel.ICT.Framework.Entity
{
    internal class CommandLineParser<T> where T : class, new()
    {
        private readonly ArgumentSettingsAttribute _argumentsSettings;
        private readonly Dictionary<string, IArgumentDefinition> _namedArguments;
        private readonly SortedList<uint, IArgumentDefinition> _positionalArguments;
        private readonly List<IArgumentDefinition> _helpArguments;
        private readonly IArgumentDefinition _unknownArguments;
        private readonly IArgumentDefinition _commandArgument;
        private readonly Dictionary<string, ICommandLineParser> _commandArgumentTypes;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="defaultSettings">Settings used if T has no settings attribute</param>
        public CommandLineParser(ArgumentSettingsAttribute defaultSettings)
        {
            _argumentsSettings = (ArgumentSettingsAttribute)typeof(T).GetCustomAttributes(typeof(ArgumentSettingsAttribute), true).FirstOrDefault() ?? defaultSettings;
            _namedArguments = new Dictionary<string, IArgumentDefinition>(_argumentsSettings.GetStringComparer());
            _positionalArguments = new SortedList<uint, IArgumentDefinition>();
            _helpArguments = new List<IArgumentDefinition>();
            _unknownArguments = null;
            _commandArgument = null;
            _commandArgumentTypes = new Dictionary<string, ICommandLineParser>(_argumentsSettings.GetStringComparer());

            Scan();
        }

        /// <summary>
        ///
        /// </summary>
	    public CommandLineParser()
            : this(new ArgumentSettingsAttribute())
        {
        }

        /// <summary>
        /// Scan argument class T for attribute validity
        /// </summary>
        private void Scan()
        {
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var custProps = propertyInfo.GetCustomAttributes(typeof(Attribute), true);
                if (custProps.Any())
                {
                    IArgumentDefinition argumentDefinition = CreateArgumentDefinition(propertyInfo);
                    var nameAttribute = custProps.OfType<ArgumentNameAttribute>().FirstOrDefault();
                    if (nameAttribute != null)
                    {
                        foreach (var name in nameAttribute.Names)
                        {
                            if (_namedArguments.ContainsKey(name))
                                throw new ArgumentDefinitionException(propertyInfo, "Multiple named arguments with name " + name);

                            _namedArguments.Add(name, argumentDefinition);
                        }

                        if (custProps.OfType<ArgumentHelpAttribute>().Any())
                        {
                            if (argumentDefinition.IsBoolean && argumentDefinition.IsMultiValue == false)
                                _helpArguments.Add(argumentDefinition);
                            else
                                throw new ArgumentDefinitionException(propertyInfo, "Help argument attribute can only be applied to boolean properties.");
                        }

                        if (custProps.OfType<ArgumentCommandHelpAttribute>().Any())
                        {
                            if (argumentDefinition.IsString && argumentDefinition.IsMultiValue == false)
                                _helpArguments.Add(argumentDefinition);
                            else
                                throw new ArgumentDefinitionException(propertyInfo, "Command help argument attribute can only be applied to string properties.");
                        }
                    }
                    else throw new ArgumentDefinitionException(propertyInfo, "Missing Positional/Named argument attribute");
                }
            }
        }

        /// <summary>
        /// Parse arguments
        /// </summary>
        /// <param name="args"></param>
        public void Parse(string[] args)
        {
            var enumerator = args.GetEnumerator();

            Parse(enumerator);
        }

        /// <summary>
        /// Parse command line
        /// </summary>
        /// <param name="enumerator">Args enumerator</param>
        public void Parse(IEnumerator enumerator)
        {
            int positionalArgumentIndex = 0;

            Clear();

            while (enumerator.MoveNext())
            {
            InLoop:
                var arg = (string)enumerator.Current;

                // Named argument?
                if (_argumentsSettings.IsNamedArgument(arg))
                {
                    var namedArgument = _argumentsSettings.GetNamedArgument(arg);
                    IArgumentDefinition argumentDefinition;

                    // Try to lookup argument
                    if (_namedArguments.TryGetValue(namedArgument.ArgumentName, out argumentDefinition))
                    {
                        var value = namedArgument.ArgumentValue;
                        if (value == null && !argumentDefinition.IsBoolean)
                        {
                            if (_argumentsSettings.ValueSeparator.Any(x => x == ' '))
                            {
                                if (!enumerator.MoveNext())
                                    throw new CommandLineParserException("No value for " + namedArgument.ArgumentName);
                                value = (string)enumerator.Current;
                            }
                        }

                        if (argumentDefinition.IsMultiValue || (!argumentDefinition.HasValue))
                            argumentDefinition.AddValue(value);
                        else
                        {
                            switch (_argumentsSettings.DuplicateArgumentBehaviour)
                            {
                                case DuplicateArgumentBehaviour.Last:
                                    argumentDefinition.AddValue(value);
                                    break;

                                case DuplicateArgumentBehaviour.First:
                                    break;

                                case DuplicateArgumentBehaviour.Fail:
                                    throw new CommandLineParserException("Duplicate argument " + namedArgument.ArgumentName);
                                case DuplicateArgumentBehaviour.Unknown:
                                    AddUnknownArgument(arg);
                                    break;
                            }
                        }
                    }
                    else
                        AddUnknownArgument(arg);
                }
                else
                {
                    ICommandLineParser commandParser;
                    if (_commandArgumentTypes.TryGetValue(arg, out commandParser))
                    {
                        bool more;
                        _commandArgument.AddRawValue(commandParser.ParseCommand(enumerator, out more));
                        if (more) goto InLoop; // Hey! Look ma! A GOTO!!!
                    }
                    else
                    {
                        // Positional argument?
                        if (_positionalArguments.Count > positionalArgumentIndex)
                        {
                            IArgumentDefinition argumentDefinition =
                                _positionalArguments.ElementAt(positionalArgumentIndex).Value;

                            argumentDefinition.AddValue(arg);

                            // Multivalue (array) positional argument definitions can recieve multiple values. Don't increment index.
                            if (!argumentDefinition.IsMultiValue)
                                positionalArgumentIndex++;
                        }
                        else
                            AddUnknownArgument(arg);
                    }
                }
            }
        }

        /// <summary>
        /// Generate command line help
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        public void GenerateHelp(TextWriter writer)
        {
            var cmdArgument = _helpArguments.FirstOrDefault(x => x.HasValue && x.IsString);
            if (cmdArgument != null)
            {
                GenerateCommandHelp(writer, (string)cmdArgument.Value);
            }
            else
            {
                GenerateHelpDescription(writer);
                GenerateCommandLineHelp(writer, false);
                GenerateDetailedCommandLineHelp(writer);
            }
        }

        /// <summary>
        /// Generate help for command
        /// </summary>
        /// <param name="writer">output to this writer</param>
        /// <param name="command">command to find</param>
        public void GenerateCommandHelp(TextWriter writer, string command)
        {
            var commandMatch = FindCommandCommandLineParser(command);
            if (commandMatch != null)
            {
                commandMatch.Parser.GenerateCommandHelp(writer, commandMatch.Aliases);
            }
            else
            {
                writer.WriteLine("Unknown command: {0}", command);
            }
        }

        /// <summary>
        /// Generate command line usage help.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        /// <param name="isCommand"></param>
        public void GenerateCommandLineHelp(TextWriter writer, bool isCommand)
        {
            var sw = new StringWriter();

            if (!isCommand)
            {
                sw.Write("Usage: ");
                sw.Write(_argumentsSettings.HelpExecutableName);
            }

            GeneratePositionalCommandLineHelp(sw);
            GenerateNamedCommandLineHelp(sw);
            GenerateCommandCommandLineHelp(sw);

            WordWrapText(writer, sw.ToString(), _argumentsSettings.HelpWidth, 0);
        }

        /// <summary>
        /// Generate detailed command line usage help for each argument
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        public void GenerateDetailedCommandLineHelp(TextWriter writer)
        {
            foreach (var argument in _positionalArguments.Values)
            {
                var description = (ArgumentDescriptionAttribute)argument.Property.GetCustomAttributes(typeof(ArgumentDescriptionAttribute), true).FirstOrDefault();
                // Only list argument if there is some info related to it.
                if (description != null || argument.IsEnum)
                {
                    writer.WriteLine();
                    writer.Write('<');
                    writer.Write(argument.Property.Name);
                    writer.WriteLine('>');
                    WriteEnumValueSet(writer, argument);

                    if (description != null)
                        WordWrapText(writer, description.Description, _argumentsSettings.HelpWidth - _argumentsSettings.HelpIndent, _argumentsSettings.HelpIndent);
                }
            }

            foreach (var argument in _namedArguments.Values.GroupBy(x => x).Select(x => x.Key))
            {
                var argumentName = (ArgumentNameAttribute)argument.Property.GetCustomAttributes(typeof(ArgumentNameAttribute), true).First();

                writer.WriteLine();
                writer.Write(_argumentsSettings.StartOfArgument[0]);
                writer.Write(argumentName.Names[0]);

                if (!argument.IsBoolean)
                {
                    writer.Write(_argumentsSettings.ValueSeparator[0]);
                    writer.Write('<');
                    writer.Write(argument.Property.Name);
                    writer.Write('>');
                }

                if (argumentName.Names.Length > 1)
                {
                    writer.Write(" (Alias: ");
                    writer.Write(
                        String.Join("/",
                                    argumentName
                                        .Names
                                        .Skip(1)
                                        .Select(x => _argumentsSettings.StartOfArgument[0] + x)
                                        .ToArray()));
                    writer.Write(')');
                }
                writer.WriteLine();
                WriteEnumValueSet(writer, argument);

                var description = (ArgumentDescriptionAttribute)argument.Property.GetCustomAttributes(typeof(ArgumentDescriptionAttribute), true).FirstOrDefault();
                if (description != null)
                {
                    WordWrapText(writer, description.Description, _argumentsSettings.HelpWidth - _argumentsSettings.HelpIndent, _argumentsSettings.HelpIndent);
                }
            }

            if (_commandArgument != null)
            {
                writer.WriteLine();
                var description = (ArgumentDescriptionAttribute)_commandArgument.Property.GetCustomAttributes(typeof(ArgumentDescriptionAttribute), true).FirstOrDefault();
                if (description != null)
                {
                    WordWrapText(writer, description.Description, _argumentsSettings.HelpWidth, 0);
                }

                var argumentName = (ArgumentNameAttribute)_commandArgument.Property.GetCustomAttributes(typeof(ArgumentNameAttribute), true).FirstOrDefault();
                var name = argumentName != null ? argumentName.Names[0] : _commandArgument.Property.Name;
                writer.Write(name);
                writer.WriteLine(" (Command):");

                foreach (var group in _commandArgumentTypes.GroupBy(x => x.Value, x => x.Key))
                {
                    var names = group.ToArray();

                    writer.Write(new string(' ', _argumentsSettings.HelpIndent));
                    writer.Write(names[0]);

                    if (names.Length > 1)
                    {
                        writer.Write(" (Alias: ");
                        writer.Write(String.Join("/", names.Skip(1).ToArray()));
                        writer.Write(')');
                    }

                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// Generate description part of help from ArgumentDescriprion attribute on the settings class.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        public void GenerateHelpDescription(TextWriter writer)
        {
            var description = (ArgumentDescriptionAttribute)typeof(T).GetCustomAttributes(typeof(ArgumentDescriptionAttribute), true).FirstOrDefault();
            if (description != null)
            {
                WordWrapText(writer, description.Description, _argumentsSettings.HelpWidth, 0);
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Check that required values has been specified
        /// </summary>
        public void CheckRequired()
        {
            foreach (var keyValue in _namedArguments)
                if (keyValue.Value.IsRequired && !keyValue.Value.HasValue)
                    throw new CommandLineParserException("Required argument " + _argumentsSettings.StartOfArgument.First() + keyValue.Key);

            foreach (var keyValue in _positionalArguments)
                if (keyValue.Value.IsRequired && !keyValue.Value.HasValue)
                    throw new CommandLineParserException("Required argument at position " + keyValue.Key);

            if (_commandArgument != null && _commandArgument.IsRequired && !_commandArgument.HasValue)
                throw new CommandLineParserException("One or more commands expected.");
        }

        /// <summary>
        /// Transfer parsed arguments into parameter
        /// </summary>
        /// <param name="argumentClass">Instance of T that arguments are to be transfered to</param>
        public void GetValues(T argumentClass)
        {
            foreach (var argument in GetAllArgumentDefinitions())
            {
                argument.GetValues(argumentClass);
            }
        }

        /// <summary>
        /// Reset all arguments to default state
        /// </summary>
        public void Clear()
        {
            foreach (var argument in GetAllArgumentDefinitions())
            {
                argument.Clear();
            }
        }

        /// <summary>
        /// Return true if one of the argument targets has ArgumentHelp and is set to true
        /// </summary>
        public bool IsHelp
        {
            get
            {
                return _helpArguments.Any(x => x.HasValue
                                               && ((x.IsBoolean && ((bool)x.Value))
                                                   || x.IsString));
            }
        }

        private IEnumerable<IArgumentDefinition> GetAllArgumentDefinitions()
        {
            foreach (IArgumentDefinition argument in _namedArguments.Values)
                yield return argument;

            foreach (IArgumentDefinition argument in _positionalArguments.Values)
                yield return argument;

            if (_commandArgument != null)
                yield return _commandArgument;

            if (_unknownArguments != null)
                yield return _unknownArguments;
        }

        /// <summary>
        /// Add argument to list of unknown arguments, or throw exception if no unknown argument property has been defined
        /// </summary>
        /// <param name="arg"></param>
        private void AddUnknownArgument(string arg)
        {
            if (_unknownArguments != null)
                _unknownArguments.AddValue(arg);
            else
                throw new RetryableCommandLineArgumentException("Unknown argument " + arg);
        }

        private IArgumentDefinition CreateArgumentDefinition(PropertyInfo propertyInfo)
        {
            var argType = propertyInfo.PropertyType;
            if (argType.IsArray) argType = argType.GetElementType();
            Type generic = typeof(ArgumentDefinition<>).MakeGenericType(argType);
            return (IArgumentDefinition)Activator.CreateInstance(generic, new object[] { propertyInfo });
        }

        private void WordWrapText(TextWriter writer, string text, int lineLength, int indent)
        {
            if (text != null)
            {
                foreach (var line in WordWrapText(text, lineLength))
                {
                    for (int i = 0; i < indent; i++)
                        writer.Write(' ');
                    writer.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Wraps the specified text into lines. CR/LF are honored.
        /// </summary>
        /// <param name="text">The text that are to be split into lines.</param>
        /// <param name="lineLength">Maximum line length.</param>
        /// <returns>List of lines </returns>
        private IEnumerable<string> WordWrapText(string text, int lineLength)
        {
            var lines = new List<string>();
            var paragraphs = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var paragraph in paragraphs)
            {
                lines.AddRange(WordWrapParagraph(paragraph, lineLength));
            }

            return lines;
        }

        /// <summary>
        /// Wraps the specified text into lines.
        /// </summary>
        /// <param name="paragraph">The text that are to be split into lines.</param>
        /// <param name="lineLength">Maximum line length.</param>
        /// <returns>List of lines </returns>
        private IEnumerable<string> WordWrapParagraph(string paragraph, int lineLength)
        {
            var lines = new List<string>();
            int startPos = 0;

            while (startPos < paragraph.Length)
            {
                int endPos = startPos + lineLength - 1;
                if (endPos < paragraph.Length - 1)
                {
                    while (endPos > startPos && !Char.IsSeparator(paragraph, endPos))
                        endPos--;
                    if (endPos == startPos)
                        endPos = startPos + lineLength - 1;
                }
                else endPos = paragraph.Length - 1;

                var line = paragraph.Substring(startPos, endPos - startPos + 1);
                lines.Add(line);
                startPos = endPos + 1;
            }

            return lines;
        }

        /// <summary>
        /// Generate command line usage help for positional arguments.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        private void GeneratePositionalCommandLineHelp(TextWriter writer)
        {
            foreach (var argument in _positionalArguments.Values)
            {
                writer.Write(' ');
                if (!argument.IsRequired) writer.Write('[');
                writer.Write('<');
                writer.Write(argument.Property.Name);
                writer.Write('>');
                if (argument.IsMultiValue) writer.Write("...");
                if (!argument.IsRequired) writer.Write(']');
            }
        }

        /// <summary>
        /// Find command by name in current parser or in any sub parsers.
        /// </summary>
        /// <param name="command">Name of command to find</param>
        /// <returns>Command match containg command parser and array of names associated with parser, or null if not found</returns>
        private CommandMatch FindCommandCommandLineParser(string command)
        {
            IEqualityComparer<string> equalityComparer = _argumentsSettings.GetStringComparer();
            IEnumerable<IGrouping<ICommandLineParser, string>> groups = _commandArgumentTypes.GroupBy(x => x.Value, x => x.Key);

            foreach (var group in groups)
            {
                var names = group.ToArray();
                if (names.Any(x => equalityComparer.Equals(x, command)))
                {
                    return new CommandMatch(group.Key, names);
                }
            }

            foreach (var commandType in groups.Select(x => x.Key))
            {
                var parser = commandType.FindCommandCommandLineParser(command);
                if (parser != null) return parser;
            }

            return null;
        }

        /// <summary>
        /// Generate command line usage help for named arguments.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        private void GenerateNamedCommandLineHelp(TextWriter writer)
        {
            foreach (var argument in _namedArguments.Values.GroupBy(x => x).Select(x => x.Key))
            {
                var argumentName = (ArgumentNameAttribute)argument.Property.GetCustomAttributes(typeof(ArgumentNameAttribute), true).First();

                writer.Write(' ');
                if (!argument.IsRequired) writer.Write('[');
                writer.Write(_argumentsSettings.StartOfArgument[0]);
                writer.Write(argumentName.Names[0]);

                if (!argument.IsBoolean)
                {
                    writer.Write(_argumentsSettings.ValueSeparator[0]);
                    writer.Write('<');
                    writer.Write(argument.Property.Name);
                    writer.Write('>');
                    if (argument.IsMultiValue) writer.Write("...");
                }
                if (!argument.IsRequired) writer.Write(']');
            }
        }

        private void GenerateCommandCommandLineHelp(StringWriter writer)
        {
            if (_commandArgument != null)
            {
                var argumentName = (ArgumentNameAttribute)_commandArgument.Property.GetCustomAttributes(typeof(ArgumentNameAttribute), true).FirstOrDefault();
                var name = argumentName != null ? argumentName.Names[0] : _commandArgument.Property.Name;

                writer.Write(' ');
                if (!_commandArgument.IsRequired) writer.Write('{');
                writer.Write(name);
                if (_commandArgument.IsMultiValue) writer.Write("...");
                if (!_commandArgument.IsRequired) writer.Write('}');
            }
        }

        private void WriteEnumValueSet(TextWriter writer, IArgumentDefinition argument)
        {
            if (argument.IsEnum)
            {
                var valueText = "Value set: " + String.Join(", ", argument.EnumValueList);
                WordWrapText(writer, valueText, _argumentsSettings.HelpWidth - _argumentsSettings.HelpIndent, _argumentsSettings.HelpIndent);
            }
        }

        /// <summary>
        /// Automatically generate command line help when an argument error occur.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        internal void AutoGenerateHelp(TextWriter writer)
        {
            if (_argumentsSettings.ShowHelpOnArgumentErrors)
                GenerateHelp(writer);
        }
    }
}