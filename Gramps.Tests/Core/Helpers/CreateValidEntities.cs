using System;
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
    }
}
