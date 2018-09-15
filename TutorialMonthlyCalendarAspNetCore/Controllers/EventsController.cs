using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TutorialMonthlyCalendarAspNetCore.Models;

namespace TutorialMonthlyCalendarAspNetCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Events")]
    public class EventsController : Controller
    {
        private readonly CalendarDbContext _context;

        public EventsController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public IEnumerable<CalendarEvent> GetEvents([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                return from e in _context.Events where !((e.End <= start) || (e.Start >= end)) select e;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [Route("GetMembersPackage")]
        public string GetMembers(string membershipNumber)
        {
            var members = (from ep in _context.Package
                join e in _context.MembershipPackages on ep.Id equals e.PackageId
                where e.MembershipId == Convert.ToInt32(membershipNumber)
                select new
                {
                    ep.Id,
                    ep.PackageName
                }).ToList();
            return JsonConvert.SerializeObject(members);
        }

        [Route("GetPackages")]
        public string GetPackages(int membershipNumber)
        {
            var members = (from ep in _context.Package
                select new
                {
                    ep.Id,
                    ep.PackageName
                }).ToList();
            return JsonConvert.SerializeObject(members);
        }

        [Route("GetMemebershipDetailsPackage")]
        public string GetMemebershipDetailsPackage(string membershipNumber)
        {
            var members = (from ep in _context.Package
                join e in _context.MembershipPackages on ep.Id equals e.PackageId
                where e.MembershipId == Convert.ToInt32(membershipNumber)
                select new
                {
                    ep.Id,
                    ep.PackageName,
                    e.PackageCount,
                    e.MembershipId,
                    e.PackageId
                }).ToList();
            return JsonConvert.SerializeObject(members);
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);

                if (@event == null)
                {
                    return NotFound();
                }

                return Ok(@event);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        [Route("GetMembers")]
        public IEnumerable<Memberships> GetMembers()
        {
            var members = _context.Memberships.ToList();
            return members;
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent([FromRoute] int id, [FromBody] CalendarEvent @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != @event.Id)
            {
                return BadRequest();
            }

            _context.Entry(@event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Events
        [HttpPost]
        public async Task<IActionResult> PostEvent([FromBody] CalendarEvent calEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var member = _context.MembershipPackages.FirstOrDefault(x =>
                x.MembershipId == Convert.ToInt32(calEvent.MembershipNumber) & x.PackageId == Convert.ToInt32(calEvent.PackageNumber));

            if (member.PackageCount < 1)
            {
                return StatusCode(500);
            }
            else
            {
                member.PackageCount-=1;
            }

            calEvent.Text = calEvent.Text + " " + calEvent.MembershipName + " " + calEvent.PackageName;
            
            _context.Events.Add(calEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = calEvent.Id }, calEvent);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var packageEvent = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
            
            if (packageEvent == null)
            {
                return NotFound();
            }

            var package = _context.MembershipPackages.FirstOrDefault(x =>
                x.PackageId == Convert.ToInt32(packageEvent.PackageNumber) &
                x.MembershipId == Convert.ToInt32(packageEvent.MembershipNumber));

            _context.Events.Remove(packageEvent);

            if (package != null)
            {
                package.PackageCount += 1;
                _context.MembershipPackages.Update(package);
            }

            await _context.SaveChangesAsync();

            return Ok(packageEvent);
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        // PUT: api/Events/5/move
        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveEvent([FromRoute] int id, [FromBody] EventMoveParams param)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.Start = param.Start;
            @event.End = param.End;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Events/5/color
        [HttpPut("{id}/color")]
        public async Task<IActionResult> SetEventColor([FromRoute] int id, [FromBody] EventColorParams param)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.Color = param.Color;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

    }

    public class EventMoveParams
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class EventColorParams
    {
        public string Color { get; set; }
    }
}