using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro.Helper;
using BaseLib;
using NHibernate;

namespace faceboardpro.Model
{
    class GroupCampaignManagerRepository 
    {
        public ICollection<GroupCampaign> GetAllAccount(GroupCampaign groupCamp)
        {
            ICollection<GroupCampaign> iCol = null;
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    iCol = session.CreateCriteria(typeof(GroupCampaign)).List<GroupCampaign>();
                }
             }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return iCol;
        }

        public ICollection<GroupCampaign> SelectCampaigns(GroupCampaign groupCamp)
        {
            List<GroupCampaign> lstGroupCampaign=null;
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        lstGroupCampaign = session.CreateQuery("FROM GroupCampaign gc where gc.Module =:module")
                        .SetParameter("module", groupCamp.Module)
                        .List<GroupCampaign>()
                        .ToList<GroupCampaign>();
                        return lstGroupCampaign;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
            return lstGroupCampaign;
        }

        public void Insert(GroupCampaign groupCamp)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        session.Save(groupCamp);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void Update(GroupCampaign groupCamp)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        session.Update(groupCamp.GroupCampaignName, groupCamp);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public int UpdateQuery(GroupCampaign groupCamp)
        {
            using (NHibernate.ISession session = SessionFactory.GetNewSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var query = session.CreateQuery("Update GroupCampaign set Account=:Account,PicFilePath=:PicFilePath,VideoFilePath=:VideoFilePath,MessageFilepath=:MessageFilepath,ScheduleTime=:ScheduleTime,CmpStartTime=:CmpStartTime,Accomplish=:Accomplish,NoOfMessage=:NoOfMessage,MessageMode=:MessageMode,MessageType=:MessageType,TextMessage=:TextMessage where GroupCampaignName=:GroupCampaignName and Module =:module");
                    //session.SaveOrUpdate(groupCamp);
                    query.SetParameter("GroupCampaignName", groupCamp.GroupCampaignName);
                    query.SetParameter("Account", groupCamp.Account);
                    query.SetParameter("PicFilePath", groupCamp.PicFilePath);
                    query.SetParameter("VideoFilePath", groupCamp.VideoFilePath);
                    query.SetParameter("MessageFilepath", groupCamp.MessageFilePath);
                    query.SetParameter("ScheduleTime", groupCamp.ScheduleTime);
                    query.SetParameter("CmpStartTime", groupCamp.CmpStartTime);
                    query.SetParameter("Accomplish", groupCamp.Accomplish);
                    query.SetParameter("NoOfMessage",groupCamp.NoOfMessage);

                    query.SetParameter("MessageMode", groupCamp.MessageMode);
                    query.SetParameter("MessageType", groupCamp.MessageType);
                    query.SetParameter("TextMessage", groupCamp.TextMessage);
                    query.SetParameter("module", groupCamp.Module);                
                    int res = query.ExecuteUpdate();
                    transaction.Commit();
                    return res;
                }
            }
        }

        public void Delete(GroupCampaign groupCamp)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        session.Delete(groupCamp.GroupCampaignName);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void DeleteAll()
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        var query = session.CreateQuery("delete from GroupCampaign");
                        int res = query.ExecuteUpdate();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void DeleteSelectRows(GroupCampaign ObjGrpCamp)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        var query = session.CreateSQLQuery("delete from GroupCampaign WHERE GroupCampaignName = :CampaignName and Module =:module").SetParameter("CampaignName", ObjGrpCamp.GroupCampaignName).SetParameter("module",ObjGrpCamp.Module);
                        int res = query.ExecuteUpdate();
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
