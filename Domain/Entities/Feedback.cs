using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string FeedbackMessage { get; set; }
        public string FeedbackAuthorName { get; set;}
        public DateTime Date { get; set; }

    }
}
