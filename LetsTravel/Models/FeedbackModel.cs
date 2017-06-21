using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LetsTravel.Models
{
    public class FeedbackModel
    {

        [Required(ErrorMessage = "Please enter your message")]
        public string FeedbackMessage { get; set; }

        [Required(ErrorMessage = "Please enter your nickname")]
            public string FeedbackAuthorName { get; set; }
    }
}