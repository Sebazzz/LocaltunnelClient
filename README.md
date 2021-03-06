# Localtunnel client - .NET implementation of localtunnel client
[Localtunnel](https://github.com/localtunnel/localtunnel) is an utility that allows you to access HTTP services on your own laptop, without needing to open ports in your firewall. In some corporate environment opening ports are blocked, which is very cumbersome if you want to test a responsive website running on your own laptop.

Localtunnel requires a server, where the website is proxied to, and a client which actually runs the website.

This package is the .NET implementation of the client. Used for development purposes only.

[![NuGet Version and Downloads count](https://buildstats.info/nuget/LocaltunnelClient?includePreReleases=true)](https://www.nuget.org/packages/LocaltunnelClient)

## Features

Works on .NET Standard 1.6 and higher. Split in a library and a command line utility.

### CLI
This project also provides CLI. 

You can download the no-dependencies, standalone, deployment from the Github releases page.

Or you can install it from the command-line if you have the .NET SDK installed:

    dotnet tool install -g localtunnel

Then run it by using:

    localtunnel

## Building the project
To build the project ensure you have:

- .NET Core SDK
- Visual Studio with .NET Core tools
- Powershell 4 or higher

To build the project run:

    build

To package the project run:

    build -Target Pack

## Contributions
This project is accepting contributions. Please keep the following guidelines in mind:

- Add tests for new code added
- Keep in line with the existing code style
- Don't reformat existing code
- Propose new features before creating pull requests to prevent disappointment

## Attribution

Tunnel by David Brossard from the Noun Project