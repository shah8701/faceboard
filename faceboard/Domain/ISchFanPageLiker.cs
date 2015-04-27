using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface ISchFanPageLiker
    {
        ICollection<SchFanPageLiker> GetAllScheduler();
        void Insert(SchFanPageLiker schFanPageLiker);
        void Update(SchFanPageLiker schFanPageLiker);
        void Delete(SchFanPageLiker schFanPageLiker);
    }
}
