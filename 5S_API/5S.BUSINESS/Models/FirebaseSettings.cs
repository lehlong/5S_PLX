using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Models
{
    public class FirebaseSettings
    {
        public string ProjectId { get; set; } = string.Empty;
        public string PrivateKeyId { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string AuthUri { get; set; } = string.Empty;
        public string TokenUri { get; set; } = string.Empty;
        public string AuthProviderX509CertUrl { get; set; } = string.Empty;
        public string ClientX509CertUrl { get; set; } = string.Empty;
        public string CredentialPath { get; set; } = string.Empty;
    }
}