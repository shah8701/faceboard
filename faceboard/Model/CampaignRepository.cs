using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro.Domain;
using faceboardpro.Helper;

namespace faceboardpro.Model
{
    class CampaignRepository:IGroupCampaign 
    {
        public ICollection<GroupCampaign> GetAllAccount(Campaign acc)
        {
            using (NHibernate.ISession session = SessionFactory.GetNewSession())
            {
                return session.CreateCriteria(typeof(GroupCampaign)).List<GroupCampaign>();
            }
        }

        public void Insert(GroupCampaign acc)
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

        public void Update(GroupCampaign acc)
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

        public void Delete(GroupCampaign acc)
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
    }
}
