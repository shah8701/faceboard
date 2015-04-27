using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface IEmail
    {
      
        ICollection<Email> GetAllEmail(Email email);
        void Insert(Email email);
        void Update(Email email);
        void Delete(Email email);
    }
}
