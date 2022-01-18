using System;

namespace MvcApplication.ViewModels
{
    public class UserProfileViewModel
    {
        public string EmailAddress { get; set; }

        public string Name { get; set; }

        public string ProfileImage { get; set; }
    }

    [Serializable]
    public class AccessToken {
        public string access_token { get; set; }
    }

}