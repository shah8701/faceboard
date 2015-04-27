using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro.Helper;

namespace faceboardpro.Model
{
    class GroupRequestUniqueRepository:IGroupRequestUnique
    {
        public ICollection<GroupCampaign> GetAllAccount(GroupRequestUnique gru)
        {
            using (NHibernate.ISession session = SessionFactory.GetNewSession())
            {
                return session.CreateCriteria(typeof(GroupCampaign)).List<GroupCampaign>();
            }
        }

        public void Insert(GroupRequestUnique gru)
        {
            using (NHibernate.ISession session = SessionFactory.GetNewSession())
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(gru);

                    transaction.Commit();
                }
            }
        }
    }
}
