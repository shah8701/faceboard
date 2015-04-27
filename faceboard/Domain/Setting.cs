using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    public class Setting
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string module;

        public string Module
        {
            get { return module; }
            set { module = value; }
        }

        private string fileType;

        public string FileType
        {
            get { return fileType; }
            set { fileType = value; }
        }

        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
    }
}
