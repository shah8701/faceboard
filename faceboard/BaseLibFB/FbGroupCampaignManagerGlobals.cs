using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro
{
    public sealed class FbGroupCampaignManagerGlobals
    {

        ////////////Group CampainManager Schedule setting
        # region ScheduleTime
        public static string Scheduletime = DateTime.Now.ToString();
        public static int NoOfMessages = 0;
        public static int NoOfMessageserHour = 0;

        
        #endregion

        ///////////////GlobalCampianManager Data
        public static string GroupCampiagnName = string.Empty;
        public static string Account = string.Empty;
        public static string PicFilePath = string.Empty;

        public static string VideoFilePath = string.Empty;
        public static string MessageFilePath = string.Empty;
        public static string ScheduleTime = string.Empty;

        public static string cmpStartTime = string.Empty;
        public static string Accomplish = string.Empty;
        public static string NoOfMessage = string.Empty;

        public static string MessageMode = string.Empty;
        public static string MessageType = string.Empty;
        public static string TextMessage = string.Empty;

    }
}
