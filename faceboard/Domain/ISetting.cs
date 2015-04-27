using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface ISetting
    {
        ICollection<Setting> GetAllSetting(Setting setting);
        void InsertOrUpdate(Setting setting);
        void Update(Setting setting);
        void Delete(Setting setting);
    }
}
