using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro.Domain;
using BaseLib;
using faceboardpro.Helper;

namespace faceboardpro.Model
{
    class MessageRepository:IMessage
    {
        private static readonly object lockerUpdateMsgDate = new object();
        private static readonly object lockerInsert = new object();

        public ICollection<Message> GetAllMessage()
        {
            using (NHibernate.ISession session = SessionFactory.GetNewSession())
            {
                return session.CreateCriteria(typeof(Message)).List<Message>();
            }
        }

        public List<Message> GetMessageUsingUserIdNameSnippedIdSenderNameMsg(Message objMessage)
        {
            List<Message> lstMessage = new List<Message>();
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    NHibernate.IQuery nIquery = session.CreateQuery("from Message msg where msg.UserId = :UserId and msg.UserName = :UserName and msg.MsgSnippedId = :MsgSnippedId and msg.MsgSenderName = :MsgSenderName and msg.MessageText = :Message");
                    nIquery.SetParameter("UserId", objMessage.UserId);
                    nIquery.SetParameter("UserName", objMessage.UserName);
                    nIquery.SetParameter("MsgSnippedId", objMessage.MsgSnippedId);
                    nIquery.SetParameter("MsgSenderName", objMessage.MsgSenderName);
                    nIquery.SetParameter("Message", objMessage.MessageText);

                    lstMessage = nIquery.List<Message>().ToList<Message>();

                    #region MyRegion
                    //foreach (Message item in nIquery.Enumerable<Message>())
                    //{
                    //    try
                    //    {
                    //        lstMessage.Add(item);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    //    }
                    //} 
                    #endregion

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return lstMessage;
        }

        public void Insert(Message msg)
        {
            try
            {
                lock (lockerInsert)
                {
                    using (NHibernate.ISession session = SessionFactory.GetNewSession())
                    {
                        using (NHibernate.ITransaction transaction = session.BeginTransaction())
                        {
                            session.Save(msg);

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

        public void UpdateMsgDate(Message objMessage)
        {
            try
            {
                lock (lockerUpdateMsgDate)
                {
                    using (NHibernate.ISession session = SessionFactory.GetNewSession())
                    {
                        using (NHibernate.ITransaction transaction = session.BeginTransaction())
                        {

                            var query = session.CreateQuery("Update Message set MsgDate = :MsgDate where UserId= :UserId and UserName= :UserName and MsgSnippedId= :MsgSnippedId and MsgSenderName= :MsgSenderName and MessageText= :Message");
                            query.SetParameter("MsgDate", objMessage.MsgDate);
                            query.SetParameter("UserId", objMessage.UserId);
                            query.SetParameter("UserName", objMessage.UserName);
                            query.SetParameter("MsgSnippedId", objMessage.MsgSnippedId);
                            query.SetParameter("MsgSenderName", objMessage.MsgSenderName);
                            query.SetParameter("Message", objMessage.MessageText);

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

        public void Update(Message msg)
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

        public void Delete(Message msg)
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

        public int DeleteSelectedRowData(Message msg)
        {
            int i = 0;
            //Creates a database connection and opens up a session
            using (NHibernate.ISession session = SessionFactory.GetNewSession())
            {
                //Begin session trasaction and opens up.
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {

                        NHibernate.IQuery query = session.CreateQuery("delete from Message where UserName=:userName and MsgSenderName=:msgSenderName and MessagingReadParticipants=:msgReadParticipants")
                             .SetParameter("userName", msg.UserName)
                             .SetParameter("msgSenderName", msg.MsgSenderName)
                             .SetParameter("msgReadParticipants", msg.MessagingReadParticipants);

                        int isUpdated = query.ExecuteUpdate();
                        transaction.Commit();
                        i = 1;
                        return i;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        return 0;
                    }
                }//End Trsansaction
            }//End session
        }
    }
}
