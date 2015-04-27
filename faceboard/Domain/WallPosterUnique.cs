using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class WallPosterUnique
    {
        private int id;
        
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string friendID;

        public string FriendID
        {
            get { return friendID; }
            set { friendID = value; }
        }

        private string messageType;

        public string MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
