using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using NHibernate;
using BaseLib;

namespace faceboardpro.Helper
{
    class SessionFactory
    {
        private static NHibernate.ISessionFactory sFactory;

        /// <summary>
        /// initializes the session for database
        /// </summary>
        private static void Init()
        {
            try
            {
                NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();



                //string path = HttpContext.Current.Server.MapPath("~/hibernateconfigfile");
                //config.Configure(path);

                Assembly assem=Assembly.GetExecutingAssembly();

                config.AddAssembly(Assembly.GetExecutingAssembly());//adds all the embedded resources .hbm.xml
                sFactory = config.BuildSessionFactory();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }


        /// <summary>
        /// checks wheteher the session already exists. if not then creates it
        /// </summary>
        /// <returns></returns>
        public static NHibernate.ISessionFactory GetSessionFactory()
        {
            if (sFactory == null)
            {
                Init();
            }
            return sFactory;

        }

        /// <summary>
        /// creates a database connection and opens up a session
        /// </summary>
        /// <returns></returns>
        public static NHibernate.ISession GetNewSession()
        {
            return GetSessionFactory().OpenSession();
        }
    }
}
