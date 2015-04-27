using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface ICmpFanPageLiker
    {
        ICollection<CmpFanPageLiker> GetAllCampaign();
        void Insert(CmpFanPageLiker cmpFanPageLiker);
        void Update(CmpFanPageLiker cmpFanPageLiker);
        void Delete(CmpFanPageLiker cmpFanPageLiker);
    }
}
