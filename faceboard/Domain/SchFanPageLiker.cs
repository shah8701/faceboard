using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    public class SchFanPageLiker
    {
        private string autoId;

        public string AutoId
        {
            get { return autoId; }
            set { autoId = value; }
        }

        private string schedulerName;

        public string SchedulerName
        {
            get { return schedulerName; }
            set { schedulerName = value; }
        }

        private string schedulerProcess;

        public string SchedulerProcess
        {
            get { return schedulerProcess; }
            set { schedulerProcess = value; }
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

        private string fanPageCommentsFile;

        public string FanPageCommentsFile
        {
            get { return fanPageCommentsFile; }
            set { fanPageCommentsFile = value; }
        }

        private DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        private DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        private string startTime;

        public string StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        private string endTime;

        public string EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
