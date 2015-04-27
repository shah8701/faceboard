using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro;
using BaseLib;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace faceboardpro
{
    public class FBUtils
    {
        public static string GetParamValue(string pgSrc, string paramName)
        {
            string valueparamName=string.Empty;
            try
            {
                if (pgSrc.Contains("name='" + paramName + "'"))
                {
                    string param = "name='" + paramName + "'";
                    int startparamName = pgSrc.IndexOf(param) + param.Length;
                    startparamName = pgSrc.IndexOf("value=", startparamName) + "value=".Length + 1;
                    int endparamName = pgSrc.IndexOf("'", startparamName);
                    valueparamName = pgSrc.Substring(startparamName, endparamName - startparamName);
                    return valueparamName;
                }
                else if (pgSrc.Contains("name=\"" + paramName + "\""))
                {
                    string param = "name=\"" + paramName + "\"";
                    int startparamName = pgSrc.IndexOf(param) + param.Length;
                    startparamName = pgSrc.IndexOf("value=", startparamName) + "value=".Length + 1;
                    int endcommentPostID = pgSrc.IndexOf("\"", startparamName);
                    valueparamName = pgSrc.Substring(startparamName, endcommentPostID - startparamName);
                    return valueparamName;
                }
                return null;
            }
            catch (Exception ex)
            {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return valueparamName;
        }

        public static string GetErrorSummary(string pageSource)
        {
            string errorSummary = string.Empty;

            try
            {
                if (pageSource.Contains("\"errorSummary\":"))
                {
                    errorSummary = pageSource.Substring(pageSource.IndexOf("\"errorSummary\":"), pageSource.IndexOf("\"payload\":", pageSource.IndexOf("\"errorSummary\":")) - pageSource.IndexOf("\"errorSummary\":")).Replace("\"", string.Empty).Trim();//"payload":
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return errorSummary;
        }

        public static List<string> GetAllFriends(ref GlobusHttpHelper globusHttpHelper, string userId)
        {
            List<string> finalList_Friends = new List<string>();

            List<string> list_finalFriendsID = new List<string>();

            try
            {
                string pgSource_Friends = globusHttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/profile.php?id=" + userId + "&sk=friends"));////fbProfileUrl = "https://www.facebook.com/profile.php?id=";

                string[] Friends = Regex.Split(pgSource_Friends, "user.php");

                ParseFriendIDs(Friends, finalList_Friends);

                List<string> lstnewfriendid = new List<string>();

                // {"collection_token":"1220529617:2356318349:2","cursor":"MDpub3Rfc3RydWN0dXJlZDoxMDAwMDA5MjM1MTg5MjY=","tab_key":"friends","profile_id":1220529617,"overview":false,"ftid":null,"order":null,"sk":"friends"}


                string collection_token = "";
                string cursor = "";

                //check if all friends loaded
                string patternAllFriendsLoaded = "\"TimelineAppCollection\",\"setFullyLoaded\",[],[\"pagelet_timeline_app_collection_";

                int tempCount_AjaxRequests = 0;

                do //
                {
                    try
                    {
                        collection_token = "";
                        cursor = "";

                        string[] arry = Regex.Split(pgSource_Friends, "enableContentLoader");
                        if (arry.Length > 1)
                        {
                            try
                            {
                                string rawData = arry[1];

                                int startIndx_collection_token = rawData.IndexOf("pagelet_timeline_app_collection_") + "pagelet_timeline_app_collection_".Length;
                                int endIndx_collection_token = rawData.IndexOf("\"", startIndx_collection_token);
                                collection_token = rawData.Substring(startIndx_collection_token, endIndx_collection_token - startIndx_collection_token);

                                int startIndx_cursor = rawData.IndexOf(",\"", endIndx_collection_token) + ",\"".Length;
                                int endIndx_cursor = rawData.IndexOf("\"", startIndx_cursor);
                                cursor = rawData.Substring(startIndx_cursor, endIndx_cursor - startIndx_cursor);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                        }

                        string raw_data = "{\"collection_token\":\"" + collection_token + "\",\"cursor\":\"" + cursor + "\",\"tab_key\":\"friends\",\"profile_id\":" + userId + ",\"overview\":false,\"ftid\":null,\"order\":null,\"sk\":\"friends\"}";
                        string encoded_raw_data = Uri.EscapeDataString(raw_data);

                        string getURL_MoreFriendsAjax = "https://www.facebook.com/ajax/pagelet/generic.php/AllFriendsAppCollectionPagelet?data=" + encoded_raw_data + "&__user=" + userId + "&__a=1&__dyn=7n8ahyj2qmudwNAEU&__req=2";
                        string res_getURL_MoreFriendsAjax = globusHttpHelper.getHtmlfromUrl(new Uri(getURL_MoreFriendsAjax));

                        pgSource_Friends = res_getURL_MoreFriendsAjax;

                        string[] arry_UserData = Regex.Split(pgSource_Friends, "user.php");

                        ParseFriendIDs(arry_UserData, finalList_Friends);                       

                        tempCount_AjaxRequests++;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                } while ((collection_token != "" && cursor != "") || tempCount_AjaxRequests < 15 || pgSource_Friends.Contains(patternAllFriendsLoaded));



                finalList_Friends.ForEach(delegate(String friendID)
                {
                    if (friendID.Contains("&"))
                    {
                        friendID = friendID.Remove(friendID.IndexOf("&"));

                    }
                  
                    list_finalFriendsID.Add(friendID);
                });

                list_finalFriendsID = list_finalFriendsID.Distinct().ToList();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return list_finalFriendsID;
        }


        private static void ParseFriendIDs(string[] Friends, List<string> lstnewfriendid)
        {           
            foreach (string iditem in Friends)
            {
                try
                {
                    if (!iditem.Contains("<!DOCTYPE html>")&&!iditem.Contains("friend_browser"))
                    {
                        try
                        {
                            string FriendIDS = iditem.Substring(iditem.IndexOf("id="), (iditem.IndexOf(">", iditem.IndexOf("id=")) - iditem.IndexOf("id="))).Replace("id=", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                            lstnewfriendid.Add(FriendIDS);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public static List<string> GetAllFriends(ref ChilkatHttpHelpr chilkatHttpHelper, string userId)
        {
            List<string> finalList_Friends = new List<string>();

            List<string> list_finalFriendsID = new List<string>();

            try
            {
                string pgSource_Friends = chilkatHttpHelper.GetHtml("http://www.facebook.com/profile.php?id=" + userId + "&sk=friends");

                string[] Friends = Regex.Split(pgSource_Friends, "user.php");

                ParseFriendIDs(Friends, finalList_Friends);

                List<string> lstnewfriendid = new List<string>();

                // {"collection_token":"1220529617:2356318349:2","cursor":"MDpub3Rfc3RydWN0dXJlZDoxMDAwMDA5MjM1MTg5MjY=","tab_key":"friends","profile_id":1220529617,"overview":false,"ftid":null,"order":null,"sk":"friends"}


                string collection_token = "";
                string cursor = "";

                //check if all friends loaded
                string patternAllFriendsLoaded = "\"TimelineAppCollection\",\"setFullyLoaded\",[],[\"pagelet_timeline_app_collection_";

                int tempCount_AjaxRequests = 0;

                do //
                {
                    try
                    {
                        collection_token = "";
                        cursor = "";

                        string[] arry = Regex.Split(pgSource_Friends, "enableContentLoader");
                        if (arry.Length > 1)
                        {
                            try
                            {
                                string rawData = arry[1];

                                int startIndx_collection_token = rawData.IndexOf("pagelet_timeline_app_collection_") + "pagelet_timeline_app_collection_".Length;
                                int endIndx_collection_token = rawData.IndexOf("\"", startIndx_collection_token);
                                collection_token = rawData.Substring(startIndx_collection_token, endIndx_collection_token - startIndx_collection_token);

                                int startIndx_cursor = rawData.IndexOf(",\"", endIndx_collection_token) + ",\"".Length;
                                int endIndx_cursor = rawData.IndexOf("\"", startIndx_cursor);
                                cursor = rawData.Substring(startIndx_cursor, endIndx_cursor - startIndx_cursor);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                        }

                        string raw_data = "{\"collection_token\":\"" + collection_token + "\",\"cursor\":\"" + cursor + "\",\"tab_key\":\"friends\",\"profile_id\":" + userId + ",\"overview\":false,\"ftid\":null,\"order\":null,\"sk\":\"friends\"}";
                        string encoded_raw_data = Uri.EscapeDataString(raw_data);

                        string getURL_MoreFriendsAjax = "https://www.facebook.com/ajax/pagelet/generic.php/AllFriendsAppCollectionPagelet?data=" + encoded_raw_data + "&__user=" + userId + "&__a=1&__dyn=7n8ahyj2qmudwNAEU&__req=2";
                        string res_getURL_MoreFriendsAjax = chilkatHttpHelper.GetHtml(getURL_MoreFriendsAjax);

                        pgSource_Friends = res_getURL_MoreFriendsAjax;

                        string[] arry_UserData = Regex.Split(pgSource_Friends, "user.php");

                        ParseFriendIDs(arry_UserData, finalList_Friends);

                        tempCount_AjaxRequests++;
                    
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                } while ((collection_token != "" && cursor != "") || tempCount_AjaxRequests < 15 || pgSource_Friends.Contains(patternAllFriendsLoaded));



                finalList_Friends.ForEach(delegate(String friendID)
                {
                    if (friendID.Contains("&"))
                    {
                        friendID = friendID.Remove(friendID.IndexOf("&"));
                    }
                    list_finalFriendsID.Add(friendID);
                });

                list_finalFriendsID = list_finalFriendsID.Distinct().ToList();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return list_finalFriendsID;
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            // //\":471693342959395,\"
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                    Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                    End = strSource.IndexOf(strEnd, Start);
                    return strSource.Substring(Start, End - Start);//.Replace(":", "").Replace("", "");
              

            }
            else
            {
                return "";
            }
        }


        public static List<string> GetPages_FBSearch(string pgSrc)
        {
            List<string> lst_Pages = new List<string>();

            string splitPattern = "/hovercard/";

            string[] splitPgSrc = Regex.Split(pgSrc, splitPattern);

            foreach (string item in splitPgSrc)
            {
                if (!item.Contains("<!DOCTYPE html>"))
                {
                    int startIndx = item.IndexOf("page.php?id=") + "page.php?id=".Length;
                    int endIndx = item.IndexOf(">", startIndx);
                    string pageURL = "http://www.facebook.com/" + item.Substring(startIndx, endIndx - startIndx).Replace("\"", "").Replace("=", "");

                    lst_Pages.Add(pageURL);
                }
            }

            return lst_Pages;
        }
    }
}
