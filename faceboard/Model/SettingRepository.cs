using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro.Domain;
using faceboardpro;
using BaseLib;
using faceboardpro.Domain;
using faceboardpro.Helper;

namespace faceboardpro.Model
{
    class SettingRepository:ISetting
    {
        public ICollection<Setting> GetAllSetting(Setting setting)
        {
            ICollection<Setting> iCol = null;
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    iCol= session.CreateCriteria(typeof(Setting)).List<Setting>();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return iCol;
        }

        public List<Setting> GetFilePath(Setting setting)
        {
            List<Setting> lstSetting = new List<Setting>();
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {

                    //NHibernate.ISQLQuery nHquery = session.CreateSQLQuery("Select FilePath from setting where Module ='" + setting.Module + "'");// and FileType='" + setting.FileType + "'

                    NHibernate.IQuery nIquery=session.CreateQuery("from Setting s where s.Module = :module").SetParameter("module",setting.Module);
                    

                    foreach (Setting item in nIquery.Enumerable<Setting>())
                    {
                        try
                        {
                            lstSetting.Add(item);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    //nHquery.Enumerable<Setting>
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return lstSetting;
        }

        public List<Setting> GetUniqueFilePath(Setting setting)
        {
            List<Setting> lstSetting = new List<Setting>();
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {

                    // NHibernate.ISQLQuery nHquery = session.CreateSQLQuery("Select FilePath from setting where Module ='" + setting.Module + "'");// and FileType='" + setting.FileType + "'

                    NHibernate.IQuery nIquery = session.CreateQuery("from Setting s where s.Module = :module and s.FileType = :fileType").SetParameter("module", setting.Module).SetParameter("fileType", setting.FileType);//.List<Setting>().ToList<Setting>();

                    //lstSetting = session.CreateQuery("from Setting s where s.Module = :module and s.FileType = :fileType").SetParameter("module", setting.Module).SetParameter("fileType", setting.FileType).List<Setting>().ToList<Setting>();


                    foreach (Setting item in nIquery.Enumerable<Setting>())
                    {
                        try
                        {
                            lstSetting.Add(item);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    //nHquery.Enumerable<Setting>
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return lstSetting;
        }

        public void InsertOrUpdate(Setting setting)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        List<Setting> lstSetting=GetUniqueFilePath(setting);

                        if (lstSetting.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(setting.FilePath))
                            {

                                var query = session.CreateQuery("Update Setting set FilePath = :filePath where Module= :module and FileType= :fileType");
                                query.SetParameter("filePath", setting.FilePath);
                                query.SetParameter("module", setting.Module);
                                query.SetParameter("fileType", setting.FileType);

                                int res = query.ExecuteUpdate();
                                transaction.Commit();
                                //return res;
                            }
                        }
                        else
                        {
                            session.Save(setting);

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

        #region COdeComment
        //public void SaveOrUpdate(Setting setting)
        //{
        //    try
        //    {
        //        using (NHibernate.ISession session = SessionFactory.GetNewSession())
        //        {
        //            using (NHibernate.ITransaction transaction = session.BeginTransaction())
        //            {
        //                session.SaveOrUpdate(setting);
        //                transaction.Commit();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //} 
        #endregion

        public void Update(Setting setting)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        session.Update(setting.FilePath, setting);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void Delete(Setting setting)
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

        public void DeleteAccounts(Setting setting)
        {           
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {

                        var query = session.CreateQuery("DELETE FROM Setting  WHERE Module = :module AND FileType= :fileType");
                        query.SetParameter("module", setting.Module);
                        query.SetParameter("fileType", setting.FileType);
                       
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
    }
}
