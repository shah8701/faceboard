using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class Event
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string eventURL;

        public string EventURL
        {
            get { return eventURL; }
            set { eventURL = value; }
        }
    }
}
