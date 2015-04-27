using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro.Domain;
using faceboardpro.Helper;

namespace faceboardpro.Model
{
    class PhotoRepository:IPhoto
    {
        public ICollection<Photo> GetAllAccount(Photo acc)
        {
            using (NHibernate.ISession session = SessionFactory.GetNewSession())
            {
                return session.CreateCriteria(typeof(Photo)).List<Photo>();
            }
        }

        public void Insert(Photo acc)
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

        public void Update(Photo acc)
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

        public void Delete(Photo acc)
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
