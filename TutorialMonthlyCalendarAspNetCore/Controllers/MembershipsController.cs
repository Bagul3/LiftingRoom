using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorialMonthlyCalendarAspNetCore.Models;

namespace TutorialMonthlyCalendarAspNetCore.Controllers
{
    public class MembershipsController : Controller
    {
        private readonly CalendarDbContext _context;

        public MembershipsController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: Memberships
        public async Task<IActionResult> Index()
        {
            return View(await _context.Memberships.ToListAsync());
        }

        // GET: Memberships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberships = await _context.Memberships
                .SingleOrDefaultAsync(m => m.Id == id);
            if (memberships == null)
            {
                return NotFound();
            }

            return View(memberships);
        }

        // GET: Memberships/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Memberships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Phone,Package,EmergencyName,EmergencyContact,Notes")] Memberships memberships)
        {
            var membershipNumber = 0;
            if (ModelState.IsValid)
            {
                _context.Add(memberships);
                await _context.SaveChangesAsync();
                membershipNumber = memberships.Id;
            }
            var splitPackages = memberships.Package.Split(',');
            var packagePackage = _context.Package.ToList();
            foreach (var package in splitPackages)
            {
                if (package != "")
                {
                    var actuaPackage = _context.Package.FirstOrDefault(c => c.PackageName.Equals(package));
                    var memebershipPackage = new MembershipPackage()
                    {
                        MembershipId = membershipNumber,
                        PackageId = actuaPackage.Id,
                        PackageCount = actuaPackage.CountPackage
                    };
                    _context.Add(memebershipPackage);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Memberships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberships = await _context.Memberships.SingleOrDefaultAsync(m => m.Id == id);
            if (memberships == null)
            {
                return NotFound();
            }
            return View(memberships);
        }

        // POST: Memberships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,Package,EmergencyName,EmergencyContact,Notes")] Memberships memberships)
        {
            if (id != memberships.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberships);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipsExists(memberships.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(memberships);
        }

        // GET: Memberships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberships = await _context.Memberships
                .SingleOrDefaultAsync(m => m.Id == id);
            if (memberships == null)
            {
                return NotFound();
            }

            return View(memberships);
        }

        // POST: Memberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberships = await _context.Memberships.SingleOrDefaultAsync(m => m.Id == id);
            _context.Memberships.Remove(memberships);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipsExists(int id)
        {
            return _context.Memberships.Any(e => e.Id == id);
        }
    }
}
