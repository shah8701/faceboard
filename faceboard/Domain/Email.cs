using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    public class Email
    {
        private int autoId;

        public int AutoId
        {
            get { return autoId; }
            set { autoId = value; }
        }

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

        private string isUsed;

        public string IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }

        }

    }
}
