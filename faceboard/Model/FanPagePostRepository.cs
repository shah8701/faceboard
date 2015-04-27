using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using BaseLib;
using faceboardpro.Helper;

namespace faceboardpro.Model
{
    class FanPagePostRepository:Domain.IFanPagePost
    {
        private static readonly object lockerInsert = new object();
        private static readonly object lockerInsertChasngeLevel = new object();
        private static readonly object lockerUpdateStatusUsingFriendId = new object();

        public ICollection<FanPagePost> GetAllFanPagePost(FanPagePost objFPPost)
        {
            ICollection<FanPagePost> iCol = null;
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    iCol = session.CreateCriteria(typeof(FanPagePost)).List<FanPagePost>();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return iCol;
        }

        public List<FanPagePost> GetFanPagePostFriendIdMainPageUrlUsingLevelStatusMainPageUrl(FanPagePost objFPPost)
        {
            List<FanPagePost> lstFanPagePost = new List<FanPagePost>();
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {

                    // NHibernate.ISQLQuery nHquery = session.CreateSQLQuery("Select FilePath from setting where Module ='" + setting.Module + "'");// and FileType='" + setting.FileType + "'

                    NHibernate.IQuery nIquery = session.CreateQuery("from FanPagePost fpPost where fpPost.Level = :level and fpPost.Status = :status and fpPost.MainPageUrl = :mainPageUrl")
                        .SetParameter("level", objFPPost.Level)
                        .SetParameter("status", objFPPost.Status)
                        .SetParameter("mainPageUrl", objFPPost.MainPageUrl);



                    foreach (FanPagePost item in nIquery.Enumerable<FanPagePost>())
                    {
                        try
                        {
                            lstFanPagePost.Add(item);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return lstFanPagePost;
        }

        public List<FanPagePost> GetFanPagePostUsingLevelStatusMainPageUrl(FanPagePost objFPPost)
        {
            List<FanPagePost> lstFanPagePost = new List<FanPagePost>();
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {

                    // NHibernate.ISQLQuery nHquery = session.CreateSQLQuery("Select FilePath from setting where Module ='" + setting.Module + "'");// and FileType='" + setting.FileType + "'

                    NHibernate.IQuery nIquery = session.CreateQuery("from FanPagePost fpPost where fpPost.Level = :level and fpPost.Status = :status and fpPost.MainPageUrl = :mainPageUrl")
                        .SetParameter("level", objFPPost.Level)
                        .SetParameter("status", objFPPost.Status)
                        .SetParameter("mainPageUrl", objFPPost.MainPageUrl);



                    foreach (FanPagePost item in nIquery.Enumerable<FanPagePost>())
                    {
                        try
                        {
                            lstFanPagePost.Add(item);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return lstFanPagePost;
        }

        public void Insert(FanPagePost objFPPost)
        {
            try
            {
                lock (lockerInsert)
                {
                    using (NHibernate.ISession session = SessionFactory.GetNewSession())
                    {
                        using (NHibernate.ITransaction transaction = session.BeginTransaction())
                        {
                            session.Save(objFPPost);

                            transaction.Commit();
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void InsertChasngeLevel(FanPagePost objFPPost)
        {
            try
            {
                lock (lockerInsertChasngeLevel)
                {
                    using (NHibernate.ISession session = SessionFactory.GetNewSession())
                    {
                        using (NHibernate.ITransaction transaction = session.BeginTransaction())
                        {
                            session.Save(objFPPost);

                            transaction.Commit();
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void UpdateStatusUsingFriendId(FanPagePost objFPPost)
        {
            try
            {
                lock (lockerUpdateStatusUsingFriendId)
                {
                    using (NHibernate.ISession session = SessionFactory.GetNewSession())
                    {
                        using (NHibernate.ITransaction transaction = session.BeginTransaction())
                        {

                            var query = session.CreateQuery("Update FanPagePost set Status = :status where FriendId= :FriendId");
                            query.SetParameter("status", objFPPost.Status);
                            query.SetParameter("FriendId", objFPPost.FriendId);

                            int res = query.ExecuteUpdate();
                            transaction.Commit();
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void Update(FanPagePost objFPPost)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        //session.Update(acc.Password, acc);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void Delete(FanPagePost objFPPost)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        // session.Delete(acc.UserName);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
    }
}
