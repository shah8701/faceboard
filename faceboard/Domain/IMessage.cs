using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    interface IMessage
    {
        ICollection<Message> GetAllMessage();
        void Insert(Message msg);
        void Update(Message msg);
        void Delete(Message msg);
    }
}
