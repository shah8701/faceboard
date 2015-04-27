using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class Message
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string userId;

        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string messagingReadParticipantsId;

        public string MessagingReadParticipantsId
        {
            get { return messagingReadParticipantsId; }
            set { messagingReadParticipantsId = value; }
        }

        private string messagingReadParticipants;

        public string MessagingReadParticipants
        {
            get { return messagingReadParticipants; }
            set { messagingReadParticipants = value; }
        }

        private string msgFriendId;

        public string MsgFriendId
        {
            get { return msgFriendId; }
            set { msgFriendId = value; }
        }

        private string msgSnippedId;

        public string MsgSnippedId
        {
            get { return msgSnippedId; }
            set { msgSnippedId = value; }
        }

        private string msgSenderName;

        public string MsgSenderName
        {
            get { return msgSenderName; }
            set { msgSenderName = value; }
        }

        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; }
        }

        private string msgDate;

        public string MsgDate
        {
            get { return msgDate; }
            set { msgDate = value; }
        }

        private string messageStatus;

        public string MessageStatus
        {
            get { return messageStatus; }
            set { messageStatus = value; }
        }
    }
}
