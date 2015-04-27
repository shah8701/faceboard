using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    public class Account
    {
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string proxyAddress;

        public string ProxyAddress
        {
            get { return proxyAddress; }
            set { proxyAddress = value; }
        }

        private string proxyPort;

        public string ProxyPort
        {
            get { return proxyPort; }
            set { proxyPort = value; }
        }

        private string proxyUserName;

        public string ProxyUserName
        {
            get { return proxyUserName; }
            set { proxyUserName = value; }
        }

        private string proxyPassword;

        public string ProxyPassword
        {
            get { return proxyPassword; }
            set { proxyPassword = value; }
        }
    }
}
