using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class Campaign
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string compaignName;

        public string CompaignName
        {
            get { return compaignName; }
            set { compaignName = value; }
        }

        private string taskName;

        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; }
        }

        
    }
}
