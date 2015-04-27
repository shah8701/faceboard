using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emails
{
    public class Email
    {
        public string username { get; set; }
        public string password { get; set; }
        public string body { get; set; }
        public string subject { get; set; }
        public string to { get; set; }
        public string from { get; set; }
        public MailType mailType { get; set; }
    }

    public enum MailType
    { 
        gmail, yahoomail, hotmail,live,aol 
    }
}
