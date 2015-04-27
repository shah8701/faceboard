using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace faceboardpro.Domain
{  
        interface ICaptcha
        {

            ICollection<Captcha> GetAllCaptchaSetting(Captcha captcha);
            void Insert(Captcha captcha);
            void Update(Captcha captcha);
            void Delete(Captcha captcha);
        }
    
}
