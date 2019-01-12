// ******************************************************************************
//  © 2019 Sebastiaan Dammann | damsteen.nl
// 
//  File:           : LocaltunnelOptions.cs
//  Project         : LocaltunnelClient
// ******************************************************************************

using System;

namespace LocaltunnelClient
{
    public sealed class LocaltunnelOptions
    {
        public string Server { get; set; }
        public string Subdomain { get; set; }
        public int LocalPort { get; set; }

        /// <summary>
        ///     Validates the options - side effect of changing <see cref="Server" /> to a full URL
        /// </summary>
        /// <exception cref="LocaltunnelOptionsValidationException"></exception>
        public void Validate()
        {
            if (!Uri.TryCreate(this.Server, UriKind.Absolute, out _))
            {
                if (!Uri.TryCreate($"{Uri.UriSchemeHttp}://{this.Server}/", UriKind.Absolute, out Uri fullUri))
                    throw new LocaltunnelOptionsValidationException($"Invalid {nameof(this.Server)} option: {fullUri}",
                                                                    nameof(this.Server));

                this.Server = fullUri.ToString();
            }

            if (!Uri.TryCreate($"{Uri.UriSchemeHttp}://{this.Subdomain}/", UriKind.Absolute, out _))
                throw new LocaltunnelOptionsValidationException(
                    $"Invalid {nameof(this.Subdomain)} option: {this.Subdomain}", nameof(this.Subdomain));

            if (this.LocalPort < 1 || this.LocalPort >= ushort.MaxValue)
                throw new LocaltunnelOptionsValidationException(
                    $"Out of range: {nameof(this.LocalPort)} - {this.LocalPort}", nameof(this.LocalPort));
        }

        internal LocaltunnelOptions Clone() => (LocaltunnelOptions) this.MemberwiseClone();

        internal string GetHostName() => new Uri(this.Server, UriKind.Absolute).Host;
    }
}