using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NCRSocialNetwork.Models;

namespace NCRSocialNetwork.ViewModels
{
    public class FeedbackViewModel : BaseViewModel
    {
        public List<Feedback> Feedback { get; set; }

        public List<User> User { get; set; }
    }
}