using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class Group
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

        private string groupId;

        public string GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        private string groupURL;

        public string GroupURL
        {
            get { return groupURL; }
            set { groupURL = value; }
        }

        
    }
}
