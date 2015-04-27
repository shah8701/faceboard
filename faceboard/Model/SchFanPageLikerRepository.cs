using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro.Helper;
using BaseLib;

namespace faceboardpro.Model
{
    public class SchFanPageLikerRepository:Domain.ISchFanPageLiker
    {
        private static readonly object lockerInsert = new object();
        private static readonly object lockerUpdateUsingCmpName = new object();

        public ICollection<SchFanPageLiker> GetAllScheduler()
        {
            ICollection<SchFanPageLiker> iCol = null;
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    iCol = session.CreateCriteria(typeof(SchFanPageLiker)).List<SchFanPageLiker>();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return iCol;
        }

        public List<SchFanPageLiker> GetSchFanPageLikerDataUsingSchName(SchFanPageLiker schFanPageLiker)
        {
            List<SchFanPageLiker> lstSchFanPageLiker = new List<SchFanPageLiker>();
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {

                    NHibernate.IQuery nIquery = session.CreateQuery("from SchFanPageLiker s where s.SchedulerName = :SchedulerName");
                    nIquery.SetParameter("SchedulerName", schFanPageLiker.SchedulerName);


                    foreach (SchFanPageLiker item in nIquery.Enumerable<SchFanPageLiker>())
                    {
                        try
                        {
                            lstSchFanPageLiker.Add(item);
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
            return lstSchFanPageLiker;
        }

        public void Insert(SchFanPageLiker acc)
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

        public int UpdateUsingSchName(SchFanPageLiker schFanPageLiker)
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
                            var query = session.CreateQuery("Update SchFanPageLiker set SchedulerProcess = :SchedulerProcess, AccountsFile= :AccountsFile, FanPageURLsFile= :FanPageURLsFile, FanPageMessageFile= :FanPageMessageFile, FanPageCommentsFile = :FanPageCommentsFile, StartDate = :StartDate, EndDate = :EndDate, StartTime = :StartTime, EndTime = :EndTime  where SchedulerName= :SchedulerName");
                            query.SetParameter("SchedulerProcess", schFanPageLiker.SchedulerProcess);
                            query.SetParameter("AccountsFile", schFanPageLiker.AccountsFile);
                            query.SetParameter("FanPageURLsFile", schFanPageLiker.FanPageURLsFile);
                            query.SetParameter("FanPageMessageFile", schFanPageLiker.FanPageMessageFile);
                            query.SetParameter("FanPageCommentsFile", schFanPageLiker.FanPageCommentsFile);
                            query.SetParameter("StartDate", schFanPageLiker.StartDate);
                            query.SetParameter("EndDate", schFanPageLiker.EndDate);
                            query.SetParameter("StartTime", schFanPageLiker.StartTime);
                            query.SetParameter("EndTime", schFanPageLiker.EndTime);
                            query.SetParameter("SchedulerName", schFanPageLiker.SchedulerName);

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

        public void Update(SchFanPageLiker acc)
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

        public void Delete(SchFanPageLiker acc)
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
