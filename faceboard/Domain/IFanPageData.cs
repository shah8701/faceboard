using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface IFanPageData
    {
        ICollection<FanPageData> GetAllFanPageData(FanPageData fpData);
        void Insert(FanPageData fpData);
        void Update(FanPageData fpData);
        void Delete(FanPageData fpData);
    }
}
