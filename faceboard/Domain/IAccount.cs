using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface IAccount
    {
        ICollection<Account> GetAllAccount(Account acc);
        void Insert(Account acc);
        void Update(Account acc);
        void Delete(Account acc);
    }
}
