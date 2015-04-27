using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class Page
    {

        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string pageURL;

        public string PageURL
        {
            get { return pageURL; }
            set { pageURL = value; }
        }

        
    }
}
