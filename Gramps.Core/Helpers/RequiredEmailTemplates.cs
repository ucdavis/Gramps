using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gramps.Core.Domain;

namespace Gramps.Core.Helpers
{
    public class RequiredEmailTemplates
    {
        public static Dictionary<string ,bool> GetRequiredEmailTemplates()
        {
            var emailTemplates = new Dictionary<string ,bool>
                                     {
                                         {"CAvail", false},
                                         {"RFR", false},
                                         {"CRemind", false},
                                         {"Conf", false},
                                         {"Approved", false},
                                         {"Denied", false}
                                     };

            return emailTemplates;
        }
    }
}
