using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;

namespace MockWPF.CDS
{
    public class ClientCredentialProvider : ICredentialProvider
    {
        private const string USERNAME = "";
        private const string PASSWORD = "";
        public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            var credentials = new NetworkCredential
            {
                UserName = USERNAME,
                SecurePassword = convertToSecureString(PASSWORD),
            };

            return credentials;
        }

        private SecureString convertToSecureString(string strPassword)
        {
            var secureStr = new SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }
    }
}
