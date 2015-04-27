using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface IFanPageStatus
    {
        ICollection<FanPageStatus> GetAllFanPageStatus(FanPageStatus fpStatus);
        void Insert(FanPageStatus fpStatus);
        void Update(FanPageStatus fpStatus);
        void DeleteUsingMainPageUrl(FanPageStatus fpStatus);
    }
}
