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
                "--local-port | -p | --port",
                "The local port at which incoming traffic is redirected.",
                CommandOptionType.SingleValue
            );

            CommandOption hostNameOption = cmdApp.Option(
                "--domain || -d",
                "The subdomain of the server to request",
                CommandOptionType.SingleValue
            );

            CommandOption openInBrowserOption = cmdApp.Option("--open | -o", "Open the link to the tunnel in the webbrowser", CommandOptionType.NoValue);
            CommandOption verboseOption = cmdApp.Option("-v | --verbose", "Output more information", CommandOptionType.NoValue);


            cmdApp.VersionOption("--version", ShortVersion, LongVersion);

            cmdApp.HelpOption("-? | -h | --help");

            cmdApp.OnExecute(async () =>
            {
                serverOption.SetDefault("https://localtunnel.me/");

                string server = serverOption.Value();
                int localPort = localPortOption.RequiredValue<int>(cmdApp);
                string subDomain = hostNameOption.Value();

                LocaltunnelOptions options = new LocaltunnelOptions
                {
                    Server = server,
                    LocalPort = localPort,
                    Subdomain = subDomain
                };

                bool open = openInBrowserOption.HasValue();
                bool verbose = verboseOption.HasValue();

                Console.WriteLine($"Connecting to {server} for establishing tunnel to {localPort}");

                ConsoleLocaltunnelRunner runner = null;
                try
                {
                    runner = new ConsoleLocaltunnelRunner(options, open, verbose);

                    await runner.RunAsync();
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine("Operation canceled");

                    if (verbose)
                    {
                        Console.WriteLine(ex);
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error 0x{ex.HResult:x8} - {ex.GetType().FullName} - {ex.Message}");

                    if (verbose)
                    {
                        Console.WriteLine(ex);
                    }

                    return ex.HResult;
                }
                finally
                {
                    runner?.Dispose();
                }

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
