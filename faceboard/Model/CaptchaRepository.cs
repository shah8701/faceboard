using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Captchas;
using BaseLib;
using faceboardpro.Helper;
using faceboardpro.Domain;

namespace faceboardpro.Model
{
    class CaptchaRepository :ICaptcha
    {

        public ICollection<faceboardpro.Domain.Captcha> GetAllCaptchaSetting(faceboardpro.Domain.Captcha captcha)
        {
            ICollection<faceboardpro.Domain.Captcha> eCol = null;
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    eCol = session.CreateCriteria(typeof(faceboardpro.Domain.Captcha)).List<faceboardpro.Domain.Captcha>();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return eCol;
        }

        public void Insert(faceboardpro.Domain.Captcha captcha)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        session.Save(captcha);

                        transaction.Commit();
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void Update(faceboardpro.Domain.Captcha captcha)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        session.Update(captcha.CaptchaService , captcha);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void UpdateCaptchaSetting(faceboardpro.Domain.Captcha Captchasetting)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                       
                       var query = session.CreateQuery("Update Captcha set Username = :username,Password= :password , Status= :status where CaptchaService= :captchaservice");
                       query.SetParameter("username", Captchasetting.Username );
                       query.SetParameter("password", Captchasetting.Password );
                       query.SetParameter("status", Captchasetting.Status );
                       query.SetParameter("captchaservice", Captchasetting.CaptchaService);

                       int res = query.ExecuteUpdate();
                       transaction.Commit();
                                //return res;
                           
                     
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void UpdateCaptchaStatusForOtherService(faceboardpro.Domain.Captcha Captchasetting)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {

                        var query = session.CreateQuery("Update Captcha set Status= :status where CaptchaService!= :captchaservice");                      
                        query.SetParameter("status", "False");
                        query.SetParameter("captchaservice", Captchasetting.CaptchaService);

                        int res = query.ExecuteUpdate();
                        transaction.Commit();
                        //return res;


                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void Delete(faceboardpro.Domain.Captcha captcha)
        {
            try
            {
                using (NHibernate.ISession session = SessionFactory.GetNewSession())
                {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction())
                    {
                        session.Delete(captcha.CaptchaService);
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
