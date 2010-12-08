using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
