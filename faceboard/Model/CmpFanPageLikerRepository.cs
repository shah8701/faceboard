using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro.Helper;
using BaseLib;

namespace faceboardpro.Model
{
    class CmpFanPageLikerRepository:Domain.ICmpFanPageLiker
    {
        private static readonly object lockerInsert = new object();
        private static readonly object lockerUpdateUsingCmpName = new object();

        public ICollection<CmpFanPageLiker> GetAllCampaign()
        {
            ICollection<CmpFanPageLiker> iCol = null;
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    iCol = session.CreateCriteria(typeof(CmpFanPageLiker)).List<CmpFanPageLiker>();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return iCol;
        }

        public List<CmpFanPageLiker> GetCmpFanPageLikerDataUsingCmpName(CmpFanPageLiker cmpFanPageLiker)
        {
            List<CmpFanPageLiker> lstCmpFanPageLiker = new List<CmpFanPageLiker>();
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {

                    NHibernate.IQuery nIquery = session.CreateQuery("from CmpFanPageLiker s where s.CampaignName = :CampaignName");
                    nIquery.SetParameter("CampaignName", cmpFanPageLiker.CampaignName);


                    foreach (CmpFanPageLiker item in nIquery.Enumerable<CmpFanPageLiker>())
                    {
                        try
                        {
                            lstCmpFanPageLiker.Add(item);
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
            return lstCmpFanPageLiker;
        }
        public void Insert(CmpFanPageLiker acc)
        {
           
            try
            {
                lock (lockerInsert)
                {
                    using (NHibernate.ISession session = SessionFactory.GetNewSession())
                    {
                        using (NHibernate.ITransaction transaction = session.BeginTransaction())
                        {
                            session.Save(acc);

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
        public bool InsertCompaign(CmpFanPageLiker acc)
        {
            bool Flage = false;
            try
            {
                lock (lockerInsert)
                {
                    using (NHibernate.ISession session = SessionFactory.GetNewSession())
                    {
                        using (NHibernate.ITransaction transaction = session.BeginTransaction())
                        {
                            session.Save(acc);

                            transaction.Commit();
                            Flage = true;
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return Flage;
        }

        public int UpdateUsingCmpName(CmpFanPageLiker cmpFanPageLiker)
        {
            lock (lockerUpdateUsingCmpName)
            {
                int res = 0;
                try
                {
                    using (NHibernate.ISession session = SessionFactory.GetNewSession())
                    {
                        using (NHibernate.ITransaction transaction = session.BeginTransaction())
                        {
                            var query = session.CreateQuery("Update CmpFanPageLiker set CampaignProcess = :CampaignProcess, AccountsFile= :AccountsFile, FanPageURLsFile= :FanPageURLsFile, FanPageMessageFile= :FanPageMessageFile, FanPageCommentsFile = :FanPageCommentsFile where CampaignName= :CampaignName");
                            query.SetParameter("CampaignProcess", cmpFanPageLiker.CampaignProcess);
                            query.SetParameter("AccountsFile", cmpFanPageLiker.AccountsFile);
                            query.SetParameter("FanPageURLsFile", cmpFanPageLiker.FanPageURLsFile);
                            query.SetParameter("FanPageMessageFile", cmpFanPageLiker.FanPageMessageFile);
                            query.SetParameter("FanPageCommentsFile", cmpFanPageLiker.FanPageCommentFile);
                            query.SetParameter("CampaignName", cmpFanPageLiker.CampaignName);

                            res = query.ExecuteUpdate();
                            transaction.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                return res; 
            }
        }

        public void Update(CmpFanPageLiker acc)
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

        public void Delete(CmpFanPageLiker acc)
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
