using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Helper;
using BaseLib;
using faceboardpro.Domain;

namespace faceboardpro.Model
{
    class WallPosterUniqueRepository
    {
        public void GetAllMessages(WallPosterUnique WPQ)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {

                        var query = session.CreateQuery("Select From WallUniquePoster  WHERE Username=:username And FriendID=:friendId And MessageType = :messageType");
                        query.SetParameter("username", WPQ.Username);
                        query.SetParameter("friendId", WPQ.FriendID);
                        query.SetParameter("messageType", WPQ.MessageType);
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

        public void Insert(WallPosterUnique WPQ)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        session.Save(WPQ);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void Update(WallPosterUnique WPQ)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        session.Update(WPQ.Status, WPQ);
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
