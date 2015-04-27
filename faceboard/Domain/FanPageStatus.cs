using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    public class FanPageStatus
    {
        private int autoId;

        public int AutoId
        {
            get { return autoId; }
            set { autoId = value; }
        }

        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string nextPageUrl;

        public string NextPageUrl
        {
            get { return nextPageUrl; }
            set { nextPageUrl = value; }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
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
