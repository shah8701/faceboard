using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface IGroupCompaignReport
    {
        ICollection<GroupCompaignReport> GetAllGroupCompaignReport(GroupCompaignReport groupCompaignReport);
        void InsertOrUpdate(GroupCompaignReport groupCompaignReport);
        void Update(GroupCompaignReport groupCompaignReport);
        void Delete(GroupCompaignReport groupCompaignReport);
    }
}
