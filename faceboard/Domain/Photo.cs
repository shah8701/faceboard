using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class Photo
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string photoId;

        public string PhotoId
        {
            get { return photoId; }
            set { photoId = value; }
        }

        private string photoURL;

        public string PhotoURL
        {
            get { return photoURL; }
            set { photoURL = value; }
        }
    }
}
