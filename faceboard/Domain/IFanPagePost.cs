using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface IFanPagePost
    {
        ICollection<FanPagePost> GetAllFanPagePost(FanPagePost fpPost);
        void Insert(FanPagePost fpPost);
        void Update(FanPagePost fpPost);
        void Delete(FanPagePost fpPost);
    }
}
