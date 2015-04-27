using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class GroupCampaign
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string groupCampaignName;

        public string GroupCampaignName
        {
            get { return groupCampaignName; }
            set { groupCampaignName = value; }
        }

        private string account;

        public string Account
        {
            get { return account; }
            set { account = value; }
        }

        private string picFilePath;

        public string PicFilePath
        {
            get { return picFilePath; }
            set { picFilePath = value; }
        }

        private string videoFilePath;

        public string VideoFilePath
        {
            get { return videoFilePath; }
            set { videoFilePath = value; }
        }

        private string messageFilePath;

        public string MessageFilePath
        {
            get { return messageFilePath; }
            set { messageFilePath = value; }
        }

        private string scheduleTime;

        public string ScheduleTime
        {
            get { return scheduleTime; }
            set { scheduleTime = value; }
        }

        private string cmpStartTime;

        public string CmpStartTime
        {
            get { return cmpStartTime; }
            set { cmpStartTime = value; }
        }

        private string accomplish;

        public string Accomplish
        {
            get { return accomplish; }
            set { accomplish = value; }
        }

        private string noOfMessage;

        public string NoOfMessage
        {
            get { return noOfMessage; }
            set { noOfMessage = value; }
        }

        private string messageMode;

        public string MessageMode
        {
            get { return messageMode; }
            set { messageMode = value; }
        }

        private string messageType;

        public string MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        private string textMessage;

        public string TextMessage
        {
            get { return textMessage; }
            set { textMessage = value; }
        }

        private string module;

        public string Module
        {
            get { return module; }
            set { module = value; }
        } 

    }
}
