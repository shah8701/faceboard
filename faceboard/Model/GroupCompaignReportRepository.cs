using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro.Helper;
using BaseLib;
using faceboardpro.Domain;

namespace faceboardpro.Model
{
    public class GroupCompaignReportRepository : IGroupCompaignReport
    {
        public ICollection<GroupCompaignReport> GetAllGroupCompaignReport(GroupCompaignReport groupCompaignReport)
        {
            ICollection<GroupCompaignReport> iCol = null;
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    iCol = session.CreateCriteria(typeof(GroupCompaignReport)).List<GroupCompaignReport>();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return iCol;
        }

        public List<GroupCompaignReport> CheckGroupCompaignReport(GroupCompaignReport groupCompaignReport)
        {
            List<GroupCompaignReport> lstGroupCompaignReport = new List<GroupCompaignReport>();
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    try
                    {
                        lstGroupCompaignReport = session.CreateQuery("from GroupCompaignReport g where g.GroupId = :groupId And g.MessageText= :messageText")
                       .SetParameter("groupId", groupCompaignReport.GroupId)
                       .SetParameter("messageText", groupCompaignReport.MessageText)
                       .List<GroupCompaignReport>().ToList<GroupCompaignReport>();
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
            return lstGroupCompaignReport;
        }

        public List<GroupCompaignReport> GetUniqueFilePath(GroupCompaignReport setting)
        {
            List<GroupCompaignReport> lstGroupCompaignReport = new List<GroupCompaignReport>();
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {

                    // NHibernate.ISQLQuery nHquery = session.CreateSQLQuery("Select FilePath from setting where Module ='" + setting.Module + "'");// and FileType='" + setting.FileType + "'

                 //   NHibernate.IQuery nIquery = session.CreateQuery("from Setting s where s.Module = :module and s.FileType = :fileType").SetParameter("module", setting.Module).SetParameter("fileType", setting.FileType);//.List<Setting>().ToList<Setting>();

                    //lstSetting = session.CreateQuery("from Setting s where s.Module = :module and s.FileType = :fileType").SetParameter("module", setting.Module).SetParameter("fileType", setting.FileType).List<Setting>().ToList<Setting>();


                    //foreach (GroupCompaignReport item in nIquery.Enumerable<GroupCompaignReport>())
                    {
                        try
                        {
                           // lstGroupCompaignReport.Add(item);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    //nHquery.Enumerable<Setting>
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return lstGroupCompaignReport;
        }

        public void InsertOrUpdate(GroupCompaignReport groupCompaignReport)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        List<GroupCompaignReport> lstGroupCompaignReport = GetUniqueFilePath(groupCompaignReport);

                        if (lstGroupCompaignReport.Count > 0)
                        {                        
                                try
                                {
                                    //var query = session.CreateQuery("Update GroupCompaignReport set GroupId = :groupId where Module= :module and MessageText= :messageText");
                                    var query = session.CreateQuery("insert into GroupCompaignReport set GroupId = :groupId,MessageText= :messageText");
                                    query.SetParameter("groupId", groupCompaignReport.GroupId);
                                    query.SetParameter("messageText", groupCompaignReport.MessageText);                                
                                 
                                     int res = query.ExecuteUpdate();
                                   
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }                           
                        }
                       else
                        {
                            try
                            {
                                session.Save(groupCompaignReport);

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }



        public void Update(GroupCompaignReport groupCompaignReport)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        //session.Update(setting.FilePath, setting);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void Delete(GroupCompaignReport groupCompaignReport)
        {

            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        //session.Delete(acc.UserName);
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
