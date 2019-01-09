// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : CommandOptionExtensions.cs
//  Project         : LocaltunnelClient.Commandline
// ******************************************************************************

using System;
using System.Globalization;
using Microsoft.Extensions.CommandLineUtils;

namespace LocaltunnelClient.Commandline.Internal
{
    internal static class CommandOptionExtensions
    {
        public static T Value<T>(this CommandOption commandOption, CommandLineApplication app)
        {
            try
            {
                return (T) Convert.ChangeType(commandOption.Value(), typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                throw new CommandParsingException(app, $"Invalid option '{commandOption.Value()}'");
            }
        }

        public static T RequiredValue<T>(this CommandOption commandOption, CommandLineApplication app)
        {
            if (!commandOption.HasValue())
            {
                throw new CommandParsingException(app, $"Option '{commandOption.LongName}' is required");
            }

            return commandOption.Value<T>(app);
        }

        public static void SetDefault(this CommandOption commandOption, string defaultValue)
        {
            if (!commandOption.HasValue())
            {
                commandOption.Values.Add(defaultValue);
            }
        }
    }
}