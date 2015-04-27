using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    public class FanPageData
    {
        private int autoId;

        public int AutoId
        {
            get { return autoId; }
            set { autoId = value; }
        }

        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string firstName;

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        private string middleName;

        public string MiddleName
        {
            get { return middleName; }
            set { middleName = value; }
        }

        private string lastName;

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        private string link;

        public string Link
        {
            get { return link; }
            set { link = value; }
        }

        private string gender;

        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        private string locale;

        public string Locale
        {
            get { return locale; }
            set { locale = value; }
        }

        private string profileStatus;

        public string ProfileStatus
        {
            get { return profileStatus; }
            set { profileStatus = value; }
        }

        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private string showUser;

        public string ShowUser
        {
            get { return showUser; }
            set { showUser = value; }
        }
    }
}
