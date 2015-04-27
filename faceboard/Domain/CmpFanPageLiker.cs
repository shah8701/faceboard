using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    public class CmpFanPageLiker
    {
        private string autoId;

        public string AutoId
        {
            get { return autoId; }
            set { autoId = value; }
        }



        private string campaignName;

        public string CampaignName
        {
            get { return campaignName; }
            set { campaignName = value; }
        }

        private string campaignProcess;

        public string CampaignProcess
        {
            get { return campaignProcess; }
            set { campaignProcess = value; }
        }

        private string accountsFile;

        public string AccountsFile
        {
            get { return accountsFile; }
            set { accountsFile = value; }
        }

        private string fanPageURLsFile;

        public string FanPageURLsFile
        {
            get { return fanPageURLsFile; }
            set { fanPageURLsFile = value; }
        }

        private string fanPageMessageFile;

        public string FanPageMessageFile
        {
            get { return fanPageMessageFile; }
            set { fanPageMessageFile = value; }
        }

        private string fanPageCommentFile;

        public string FanPageCommentFile
        {
            get { return fanPageCommentFile; }
            set { fanPageCommentFile = value; }
        }
    }
}
