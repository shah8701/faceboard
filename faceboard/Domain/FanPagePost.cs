using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    public class FanPagePost
    {
        private int autoId;

        public int AutoId
        {
            get { return autoId; }
            set { autoId = value; }
        }

        private string friendId;

        public string FriendId
        {
            get { return friendId; }
            set { friendId = value; }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        private string level;

        public string Level
        {
            get { return level; }
            set { level = value; }
        }

        private string date;

        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        private string mainPageUrl;

        public string MainPageUrl
        {
            get { return mainPageUrl; }
            set { mainPageUrl = value; }
        }
    }
}
