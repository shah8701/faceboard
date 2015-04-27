using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro.Domain;
using faceboardpro.Helper;

namespace faceboardpro.Model
{
    class GroupRepository:IGroup
    {
        public ICollection<Group> GetAllAccount(Group acc)
        {
            using (NHibernate.ISession session = SessionFactory.GetNewSession())
            {
                return session.CreateCriteria(typeof(Group)).List<Group>();
            }
        }

        public void Insert(Group acc)
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

        public void Update(Group acc)
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

        public void Delete(Group acc)
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
