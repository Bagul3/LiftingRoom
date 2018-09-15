using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TutorialMonthlyCalendarAspNetCore.Models
{
    public class MembershipPackage
    {
        public int Id { get; set; }

        public int MembershipId { get; set; }

        public int PackageId { get; set; }

        public int PackageCount { get; set; }
    }
}
