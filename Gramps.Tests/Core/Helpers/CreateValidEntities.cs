﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gramps.Core.Domain;

namespace Gramps.Tests.Core.Helpers
{
    public static class CreateValidEntities
    {
        #region Helper Extension

        private static string Extra(this int? counter)
        {
            var extraString = "";
            if (counter != null)
            {
                extraString = counter.ToString();
            }
            return extraString;
        }

        #endregion Helper Extension

        public static CallForProposal CallForProposal(int? counter, bool populateAllFields = false)
        {
            var rtValue = new CallForProposal();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.EndDate = DateTime.Now.Date.AddDays(30);
            rtValue.IsActive = true;

            return rtValue;
        }

        public static Template Template(int? counter)
        {
            var rtValue = new Template();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.IsActive = true;

            return rtValue;
        }

        public static EmailTemplate EmailTemplate(int? counter, bool populateAllFields = false)
        {
            var rtValue = new EmailTemplate();
            rtValue.Subject = "Subject" + counter.Extra();
            rtValue.TemplateType = EmailTemplateType.InitialCall;
            if(populateAllFields)
            {
                rtValue.Text = "Text" + counter.Extra();
                //rtValue.TemplateType = "Type" + counter.Extra();
            }

            return rtValue;
        }

        public static EmailsForCall EmailsForCall(int? counter)
        {
            var rtValue = new EmailsForCall(string.Format("Test{0}@testy.com", counter.Extra()));

            return rtValue;
        }

        public static User User(int? counter)
        {
            var rtValue = new User();
            rtValue.LoginId = "LoginId" + counter.Extra();
            rtValue.Email = "test" + counter.Extra() + "@testy.com";            

            return rtValue;
        }

        public static Editor Editor(int? counter)
        {
            var rtValue = new Editor();
            rtValue.IsOwner = false;
            rtValue.ReviewerName = "ReviewerName" + counter.Extra();
            rtValue.ReviewerEmail = "test" + counter.Extra() + "@testy.com"; 

            return rtValue;
        }

        public static Proposal Proposal(int? counter)
        {
            var rtValue = new Proposal();
            rtValue.Email = "Email" + counter.Extra() + "@testy.com";

            return rtValue;
        }

        public static Comment Comment(int? counter)
        {
            var rtValue = new Comment();
            rtValue.Text = "Text" + counter.Extra();
            
            return rtValue;
        }

        //public static Unit Unit(int? counter, bool populateAllFields = false)
        //{
        //    var rtValue = new Unit();
        //    rtValue.FullName = "FullName" + counter.Extra();
        //    rtValue.FisCode = "F" + counter.Extra();
        //    rtValue.School = new School();
        //    if (populateAllFields)
        //    {
        //        rtValue.ShortName = "x".RepeatTimes(50);
        //        rtValue.PpsCode = counter.Extra();
        //    }
        //    return rtValue;
        //}
        public static EmailQueue EmailQueue(int? counter)
        {
            var rtValue = new EmailQueue();
            rtValue.CallForProposal = new CallForProposal();
            rtValue.Subject = "Subject" + counter.Extra();
            rtValue.Body = "Body" + counter.Extra();
            rtValue.EmailAddress = string.Format("some{0}@test.com", counter.Extra());

            return rtValue;
        }

        public static ReviewedProposal ReviewedProposal(int? counter)
        {
            var rtValue = new ReviewedProposal();
            var count = 0;
            if (counter.HasValue)
            {
                count = counter.Value;
            }
            rtValue.LastViewedDate = new DateTime(2011, 01, 01).AddDays(count);
            rtValue.Proposal = new Proposal();
            rtValue.Editor = new Editor();

            return rtValue;
        }
    }
}
