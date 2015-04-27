using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using BaseLib;
using faceboardpro.Helper;

namespace faceboardpro.Model
{
    public class FanPageStatusRepository:Domain.IFanPageStatus
    {
        private static readonly object lockerInsert = new object();
        private static readonly object lockerDeleteUsingMainPageUrl = new object();

        public ICollection<FanPageStatus> GetAllFanPageStatus(FanPageStatus acc)
        {
            ICollection<FanPageStatus> iCol = null;
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    iCol = session.CreateCriteria(typeof(FanPageStatus)).List<FanPageStatus>();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return iCol;
        }

        public void Insert(FanPageStatus acc)
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

        public void Update(FanPageStatus acc)
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

        public void DeleteUsingMainPageUrl(FanPageStatus fpStatus)
        {
            try
            {
                lock (lockerDeleteUsingMainPageUrl)
                {
                    using (NHibernate.ISession session = SessionFactory.GetNewSession())
                    {
                        using (NHibernate.ITransaction transaction = session.BeginTransaction())
                        {
                            var query = session.CreateQuery("Delete from FanPageStatus where MainPageUrl= :mainPageUrl");
                            query.SetParameter("mainPageUrl", fpStatus.MainPageUrl);
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
    }
}
