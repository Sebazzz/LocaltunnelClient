// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : Program.cs
//  Project         : LocaltunnelClient.Commandline
// ******************************************************************************

using System.Reflection;
using LocaltunnelClient.Commandline.Internal;
using Microsoft.Extensions.CommandLineUtils;

namespace LocaltunnelClient.Commandline
{
    using System;
    internal static class Program
    {
        private static readonly Assembly ProgramAssembly = typeof(Program).Assembly;
        private static readonly string ShortVersion = ProgramAssembly.GetName().Version.ToString();
        private static readonly string LongVersion =  ProgramAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? ShortVersion;

        static int Main(string[] args)
        {
            CommandLineApplication cmdApp = new CommandLineApplication
            {
                FullName = "Localtunnel client",
                Name = "localtunnel",
                Description = "Expose your local computer",
                Syntax = "-p [local port] [--server my-own-tunnelserver.net] [--open]"
            };

            CommandOption serverOption = cmdApp.Option(
                "--server | -s",
                "Localtunnel host server - can be a host name or a full URI. In case of a host name, an HTTP connection on default port is attempted.",
                 CommandOptionType.SingleValue);

            CommandOption localPortOption = cmdApp.Option(
                "--local-port | -p",
                "The local port at which incoming traffic is redirected.",
                CommandOptionType.SingleValue
            );

            CommandOption hostNameOption = cmdApp.Option(
                "--domain || -d",
                "The subdomain of the server to request",
                CommandOptionType.SingleValue
            );

            CommandOption openInBrowserOption = cmdApp.Option("--open | -o", "Open the link to the tunnel in the webbrowser",
                                                    CommandOptionType.NoValue);


            cmdApp.VersionOption("--version", ShortVersion, LongVersion);

            cmdApp.HelpOption("-? | -h | --help");

            cmdApp.OnExecute(() =>
            {
                serverOption.SetDefault("https://localtunnel.me/");

                string server = serverOption.Value();
                string subDomain = hostNameOption.Value();
                int localPort = localPortOption.RequiredValue<int>(cmdApp);
                bool open = openInBrowserOption.HasValue();

                Console.WriteLine($"Server: {server}");
                Console.WriteLine($"Sub-domain: {subDomain}");
                Console.WriteLine($"Port: {localPort}");
                Console.WriteLine($"Browser: {open}");

                return 0;
            });

            try
            {
                cmdApp.ShowRootCommandFullNameAndVersion();

                return cmdApp.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                Console.WriteLine(ex.Message);

                return -1;
            }
        }
    }
}
