using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class GroupRequestUnique
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private string campaignName;

        public string CampaignName
        {
            get { return campaignName; }
            set { campaignName = value; }
        }

        private string url;

        public string URL
        {
            get { return url; }
            set { url = value; }
        }

        private string account;

        public string Account
        {
            get { return account; }
            set { account = value; }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
