using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Net;
using System.IO;

namespace BaseLib
{
    public class Utils
    {
        public static Regex IdCheck = new Regex("^[0-9]*$");

        public string GetAssemblyVersion()
        {
            string appName = Assembly.GetAssembly(this.GetType()).Location;
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(appName);
            string versionNumber = assemblyName.Version.ToString();
            return versionNumber;
        }

        public static string DecodeGetCharector(string PageString)
        {
            GlobusHttpHelper obj_http = new GlobusHttpHelper();
            string GetOrginalString = string.Empty;
            try
            {
                string[] Arr = System.Text.RegularExpressions.Regex.Split(PageString, "&#");
                foreach (var item_Arr in Arr)
                {
                    try
                    {

                        string ss = string.Empty;

                        ss = "&#" + Uri.EscapeDataString(Utils.getBetween("&#" + item_Arr, "&#", ";")) + ";";
                        string Url = "http://chars.suikawiki.org/string?s=" +Uri.EscapeDataString(ss);

                        string PageSource = obj_http.getHtmlfromUrl(new Uri(Url));
                        string[] Arr11 = System.Text.RegularExpressions.Regex.Split(PageSource, "escapes");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return GetOrginalString;
        }


        public static int GenerateRandom(int minValue, int maxValue)
        {
            int randomNo = 0;
            try
            {
                if (minValue <= maxValue)
                {
                    Random random = new Random();
                    randomNo = random.Next(minValue, maxValue);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return randomNo;
        }

        public int GetProcessor()
        {
            int processor = 1;
            try
            {

                processor = Environment.ProcessorCount;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return processor;
        }

        public static bool IsNumeric(string strInputNo)
        {
            Regex IdCheck = new Regex("^[0-9]*$");

            if (!string.IsNullOrEmpty(strInputNo) && IdCheck.IsMatch(strInputNo))
            {
                return true;
            }

            return false;
        }

        public static List<List<string>> Split(List<string> source, int splitNumber)
        {

            if (splitNumber <= 0)
            {
                splitNumber = 1;
            }

            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / splitNumber)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

        }

        public static string GenerateTimeStamp()
        {
            string strGenerateTimeStamp = string.Empty;
            try
            {
                // Default implementation of UNIX time of the current UTC time
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                strGenerateTimeStamp = Convert.ToInt64(ts.TotalMilliseconds).ToString();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return strGenerateTimeStamp;
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static ArrayList RandomNumbers(int max)
        {
            // Create an ArrayList object that will hold the numbers
            ArrayList lstNumbers = new ArrayList();

            try
            {
                // The Random class will be used to generate numbers
                Random rndNumber = new Random();

                // Generate a random number between 1 and the Max
                int number = rndNumber.Next(0, max + 1);
                // Add this first random number to the list
                lstNumbers.Add(number);
                // Set a count of numbers to 0 to start
                int count = 0;

                do // Repeatedly...
                {
                    // ... generate a random number between 1 and the Max
                    number = rndNumber.Next(0, max + 1);

                    // If the newly generated number in not yet in the list...
                    if (!lstNumbers.Contains(number))
                    {
                        // ... add it
                        lstNumbers.Add(number);
                    }

                    // Increase the count
                    count++;
                } while (count <= 10 * max); // Do that again

                /////Casting from ArrayList to List<string>
                //List<int> randomNoList = new List<int>();
                //int[] tempArr = (int[])lstNumbers.ToArray();
                //randomNoList = tempArr.ToList();

                // Once the list is built, return it
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }


            return lstNumbers;
        }

        public static string UploadFolderData(string DirectoryPath)
        {
            string FilePath = string.Empty;
            try
            {
                using (FolderBrowserDialog ofd = new FolderBrowserDialog())
                {
                    ofd.SelectedPath = Application.StartupPath;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        FilePath = ofd.SelectedPath;
                    }
                }
            }
            catch (Exception)
            {

                return null;
            }

            return FilePath;
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        public static List<string> GetSpinnedComments(string RawComment, char separator)
        {

            #region Using Dictionary
           
            Dictionary<Match, string[]> commentsHashTable = new Dictionary<Match, string[]>();

            ///This is final possible cominations of comments
            List<string> listModComments = new List<string>();

            ///Put braces data in list of string array
            List<string[]> listDataInsideBracesArray = new List<string[]>();


            ///This Regex will fetch data within braces and put it in list of string array
            var regex = new Regex(@"\(([^)]*)\)", RegexOptions.Compiled);
            foreach (Match Data in regex.Matches(RawComment))
            {
                string data = Data.Value.Replace("(", "").Replace(")", "");
                string[] DataInsideBracesArray = data.Split(separator);//data.Split('/');
                commentsHashTable.Add(Data, DataInsideBracesArray);
                listDataInsideBracesArray.Add(DataInsideBracesArray);
            }

            string ModifiedComment = RawComment;

            IDictionaryEnumerator en = commentsHashTable.GetEnumerator();

            List<string> listModifiedComment = new List<string>();

            listModifiedComment.Add(ModifiedComment);

            //en.Reset();

            string ModifiedComment1 = ModifiedComment;

            #region Assigning Values and adding in List
            foreach (string[] item in listDataInsideBracesArray)
            {
                en.MoveNext();
                foreach (string modItem in listModifiedComment)
                {
                    foreach (string innerItem in item)
                    {
                        string ModComment = modItem.Replace(en.Key.ToString(), innerItem);
                        listModComments.Add(ModComment);
                    }
                }
                listModifiedComment.AddRange(listModComments);
                //string ModComment = ModifiedComment1.Replace(en.Key, item
            }
            #endregion

            List<string> listRequiredComments = listModifiedComment.FindAll(s => !s.Contains("("));

            //listComments.AddRange(listRequiredComments);
            return listRequiredComments;
            #endregion
        }

        public static bool CheckIfMessagePosted(string Username, string FriendID, string MessageType)
        {
            bool Result = false;

            DataSet DS=DataBaseHandler.SelectQuery("Select * from WallPosterUnique where Username='"+Username+"' and FriendID='"+FriendID+"' and MessageType='"+MessageType+"'","WallPosterUnique");
            if (DS.Tables[0].Rows.Count > 0)
            {
                Result = true;
            }
            else
            {
                Result = false;
            }
            return Result;
        }

        public static bool InsertIntoDB(string Username, string FriendID, string MessageType)
        {
            bool Result = false;

            DataBaseHandler.InsertQuery("Insert into WallPosterUnique(Username,FriendID,MessageType,Status) Values('"+Username+"','"+FriendID+"','"+MessageType+"','Posted')","WallPosterUnique");
            return Result;
        }

        public static string GetDataWithTagValueByTagAndAttributeName(string pageSrcHtml, string TagName, string AttributeName)
        {
            string dataDescription = string.Empty;
            try
            {
                bool success = false;
                string xHtml = string.Empty;

                Chilkat.HtmlToXml htmlToXml = new Chilkat.HtmlToXml();

                //*** Check DLL working or not **********************
                success = htmlToXml.UnlockComponent("THEBACHtmlToXml_7WY3A57sZH3O");
                if ((success != true))
                {
                    Console.WriteLine(htmlToXml.LastErrorText);
                    return null;
                }

                htmlToXml.Html = pageSrcHtml;

                //** Convert Data Html to XML ******************************************* 
                xHtml = htmlToXml.ToXml();

                //******************************************
                Chilkat.Xml xNode = default(Chilkat.Xml);
                Chilkat.Xml xBeginSearchAfter = default(Chilkat.Xml);
                Chilkat.Xml xml = new Chilkat.Xml();
                xml.LoadXml(xHtml);

                #region Data Save in list From using XML Tag and Attribut
                string DescriptionMain = string.Empty;

                string dataDescriptionValue = string.Empty;


                xBeginSearchAfter = null;

                xNode = xml.SearchForAttribute(xBeginSearchAfter, TagName, "class", AttributeName);
                while ((xNode != null))
                {
                    //** Get Data Under Tag only Text Value**********************************



                    dataDescription = xNode.GetXml();//.AccumulateTagContent("text", "script|style");

                    dataDescriptionValue = dataDescriptionValue + dataDescription;
                    //    string text = xNode.AccumulateTagContent("text", "script|style");
                    //    lstData.Add(text);

                    //    //** Get Data Under Tag All  Html value * *********************************
                    //    //dataDescription = xNode.GetXml();

                    xBeginSearchAfter = xNode;
                    xNode = xml.SearchForAttribute(xBeginSearchAfter, TagName, "class", AttributeName);
                    //if (dataDescription.Length > 500)
                    //{
                    //    break;
                    //}
                }
                #endregion
                return dataDescriptionValue;
            }
            catch (Exception)
            {
                return dataDescription = null;

            }
        }

        public static void GetImageFromUrl(string url, string PicName, string Path)
        {
            string FileData = Path +"\\"+ PicName + ".jpg";
            try
            {
                byte[] data;
                using (WebClient client = new WebClient())
                {
                    data = client.DownloadData(url);
                }
                File.WriteAllBytes(FileData, data);
            }
            catch
            { };
        }


       public static List<string> list_SpinnedCreatorMessages = new List<string>();
        public static void GetStartSpinnedListItem(string parameters)
        {
            try
            {
                //Array paramsArray = new object[2];
               //string paramsArray = (Array)parameters;
                string item = parameters;
                int count = 0;
                List<string> lstCheckDuplicate = new List<string>();
                if (item.Length > 150)
                {
                    while (true)
                    {
                        string spinnedItem = spinLargeText(new Random(), item);

                        if (lstCheckDuplicate.Contains(spinnedItem))
                        {
                            continue;
                        }

                        count++;
                        lstCheckDuplicate.Add(spinnedItem);
                        lstCheckDuplicate = lstCheckDuplicate.Distinct().ToList();
                        list_SpinnedCreatorMessages = lstCheckDuplicate;
                        list_SpinnedCreatorMessages = list_SpinnedCreatorMessages.Distinct().ToList();
                        if (string.IsNullOrEmpty(spinnedItem) || count > 1000)
                        {
                            break;
                        }
                        try
                        {
                            //GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Spinned Message : " + spinnedItem + " ]");
                            // GlobusFileHelper.AppendStringToTextfileNewLine(spinnedItem, path_TweetCreatorExportFile);
                        }
                        catch (Exception ex)
                        {
                            //   AddToTweetCreatorLogs("[ " + DateTime.Now + " ] => [ Generated " + list_Spinned_TweetCreatorMessages.Count + " Spinned Messages and Exported to : " + path_TweetCreatorExportFile + " ]");
                        }

                    }
                }
                else
                {
                    list_SpinnedCreatorMessages = GetSpinnedList(new List<string> { item });

                    foreach (string _item in list_SpinnedCreatorMessages)
                    {
                        try
                        {
                            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Spinned Message : " + _item + " ]");

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("[ " + DateTime.Now + " ] =>" + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("[ " + DateTime.Now + " ] =>" + ex.Message);
            }

            finally
            {
                //countlist_TweetCreatorMessages--;
                // if (countlist_TweetCreatorMessages == 0)
                {
                    //  GlobusLogHelper.log.Error("[ " + DateTime.Now + " ] => [ Process completed. ]");
                }
            }
        }

        public static string spinLargeText(Random rnd, string str)
        {
            // Loop over string until all patterns exhausted.
            //string pattern = "{[^{}]*}";
            string pattern = @"\(([^)]*)\)";
            Match m = Regex.Match(str, pattern);
            while (m.Success)
            {
                // Get random choice and replace pattern match.
                string seg = str.Substring(m.Index + 1, m.Length - 2);
                string[] choices = seg.Split('|');
                str = str.Substring(0, m.Index) + choices[rnd.Next(choices.Length)] + str.Substring(m.Index + m.Length);
                m = Regex.Match(str, pattern);
            }

            // Return the modified string.
            return str;
        }

        public static List<string> GetSpinnedList(List<string> inputList)
        {
            List<string> tempList = new List<string>();
            foreach (string item in inputList)
            {
                tempList.Add(item);
            }
            inputList.Clear();
            foreach (string item in tempList)
            {
                List<string> tempSpunList = GetSpinnedComments(item);
                inputList.AddRange(tempSpunList);
            }
            return inputList;
        }

        public static List<string> GetSpinnedComments(string RawComment)
        {

            #region Using Dictionary
            /// <summary>
            /// Hashtable that stores (DataInsideBraces) as Key and DataInsideBracesArray as Value
            /// </summary>
            //Hashtable commentsHashTable = new Hashtable();
            Dictionary<Match, string[]> commentsHashTable = new Dictionary<Match, string[]>();

            ///This is final possible cominations of comments
            List<string> listModComments = new List<string>();

            ///Put braces data in list of string array
            List<string[]> listDataInsideBracesArray = new List<string[]>();

            ///This Regex will fetch data within braces and put it in list of string array
            var regex = new Regex(@"\(([^)]*)\)", RegexOptions.Compiled);

            //var regex = new Regex(@"{[^{}]*}", RegexOptions.Compiled);

            foreach (Match Data in regex.Matches(RawComment))
            {
                try
                {
                    string data = Data.Value.Replace("(", "").Replace(")", "");
                    string[] DataInsideBracesArray = data.Split('|');
                    commentsHashTable.Add(Data, DataInsideBracesArray);
                    listDataInsideBracesArray.Add(DataInsideBracesArray);
                }
                catch { };
            }

            string ModifiedComment = RawComment;

            IDictionaryEnumerator en = commentsHashTable.GetEnumerator();

            List<string> listModifiedComment = new List<string>();

            listModifiedComment.Add(ModifiedComment);

            //en.Reset();

            string ModifiedComment1 = ModifiedComment;

            #region Assigning Values and adding in List
            foreach (string[] item in listDataInsideBracesArray)
            {
                en.MoveNext();
                foreach (string modItem in listModifiedComment)
                {
                    foreach (string innerItem in item)
                    {
                        try
                        {
                            string ModComment = modItem.Replace(en.Key.ToString(), innerItem);
                            listModComments.Add(ModComment);
                        }
                        catch { };
                    }
                }

                listModifiedComment.AddRange(listModComments);
                //string ModComment = ModifiedComment1.Replace(en.Key, item
            }
            #endregion

            List<string> listRequiredComments = listModifiedComment.FindAll(s => !s.Contains("("));

            //listComments.AddRange(listRequiredComments);
            return listRequiredComments;
            #endregion
        }
    }

       public class DataBaseHandler
        {
            public static string CONstr = "Data Source=C:\\Facedominator\\Data\\FaceDominator.db" + ";Version=3;";

            public static DataSet SelectQuery(string query, string tablename)
            {
                DataSet DS = new DataSet();
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    AD.Fill(DS, tablename);

                }
                return DS;
            }

            public static void InsertQuery(string query, string tablename)
            {
                try
                {
                    using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                    {
                        SQLiteCommand CMD = new SQLiteCommand(query, CON);
                        SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                        DataSet DS = new DataSet();
                        AD.Fill(DS, tablename);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            public static void DeleteQuery(string query, string tablename)
            {
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    DataSet DS = new DataSet();
                    AD.Fill(DS, tablename);
                }
            }

            public static void UpdateQuery(string query, string tablename)
            {
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    DataSet DS = new DataSet();
                    AD.Fill(DS, tablename);
                }
            }

            /// <summary>
            /// This method is use for find data form sqlite table 
            /// </summary>
            /// <param name="query">Sqlite query</param>
            /// <param name="tablename">Name of Table</param>


            public static void PerformQuery(string query, string tablename)
            {
                try
                {
                    using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                    {
                        SQLiteCommand CMD = new SQLiteCommand(query, CON);
                        SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                        DataSet DS = new DataSet();
                        AD.Fill(DS, tablename);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }



           
        }
}
