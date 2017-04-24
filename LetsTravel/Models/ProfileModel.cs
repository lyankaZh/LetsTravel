using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LetsTravel.Models
{
    public class ProfileModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string AboutMyself { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public int SubscribedExcursionsAmount { get; set; }
        public int OwnedExcursionsAmount { get; set; }
        public int Mark { get; set; }
    }
}