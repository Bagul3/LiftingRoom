using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TutorialMonthlyCalendarAspNetCore.Models;

namespace TutorialMonthlyCalendarAspNetCore.Controllers
{
    public class MembershipPackagesController : Controller
    {
        private readonly CalendarDbContext _context;

        public MembershipPackagesController(CalendarDbContext context)
        {
            _context = context;
        }

        public IActionResult Edit(string membershipNumber, string packageNumber)
        {
            var model = _context.MembershipPackages.FirstOrDefault(x =>
                x.MembershipId == Convert.ToInt32(membershipNumber) & x.PackageId == Convert.ToInt32(packageNumber));
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit([Bind("Id,MembershipId,PackageCount,PackageId")] MembershipPackage package)
        {
            if (ModelState.IsValid)
            {

                _context.Update(package);
                _context.SaveChanges();
                return RedirectToAction("Details", "Memberships", new
                { id = package.MembershipId});
            }
            return View(package);
        }
    }
}