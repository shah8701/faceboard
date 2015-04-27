using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class Friend
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

        private string friendId;

        public string FriendId
        {
            get { return friendId; }
            set { friendId = value; }
        }

        private string friendProfileURL;

        public string FriendProfileURL
        {
            get { return friendProfileURL; }
            set { friendProfileURL = value; }
        }

        private string friendName;

        public string FriendName
        {
            get { return friendName; }
            set { friendName = value; }
        }
    }
}
