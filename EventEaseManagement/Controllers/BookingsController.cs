using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseManagement.Data;
using EventEaseManagement.Models;

namespace EventEaseManagement.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings (Consolidated View WITH FILTERS - Part 3)
        public async Task<IActionResult> Index(
            string searchTerm,
            int? eventTypeId,
            DateTime? startDate,
            DateTime? endDate,
            int? venueId)
        {
            // Start with all bookings including related data
            var bookings = _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            // Apply search filter (Booking ID or Event Name)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                bookings = bookings.Where(b =>
                    b.BookingId.ToString().Contains(searchTerm) ||
                    (b.Event != null && b.Event.EventName.ToLower().Contains(searchLower)));
            }

            // Apply Event Type filter (Part 3)
            if (eventTypeId.HasValue && eventTypeId.Value > 0)
            {
                bookings = bookings.Where(b => b.Event != null && b.Event.EventTypeId == eventTypeId.Value);
            }

            // Apply Date Range filter (Part 3)
            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.Event != null && b.Event.StartDate.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.Event != null && b.Event.EndDate.Date <= endDate.Value.Date);
            }

            // Apply Venue filter (Part 3)
            if (venueId.HasValue && venueId.Value > 0)
            {
                bookings = bookings.Where(b => b.VenueId == venueId.Value);
            }

            // Populate filter dropdowns
            ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeId", "EventTypeName");
            ViewBag.Venues = new SelectList(await _context.Venues.ToListAsync(), "VenueId", "VenueName");

            // Preserve current filter values for the view
            ViewBag.CurrentSearchTerm = searchTerm;
            ViewBag.CurrentEventTypeId = eventTypeId;
            ViewBag.CurrentStartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentVenueId = venueId;

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events.Include(e => e.EventType), "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Bookings/Create (WITH double booking prevention)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,BookingDate,VenueId,EventId")] Booking booking)
        {
            // Get the selected event to check its date
            var selectedEvent = await _context.Events.FindAsync(booking.EventId);

            if (selectedEvent != null)
            {
                // Check for double booking - same venue on same event date
                var existingBooking = await _context.Bookings
                    .Include(b => b.Event)
                    .FirstOrDefaultAsync(b => b.VenueId == booking.VenueId && b.Event.StartDate.Date == selectedEvent.StartDate.Date);

                if (existingBooking != null)
                {
                    TempData["Error"] = $"This venue is already booked on {selectedEvent.StartDate:dd MMM yyyy} for '{existingBooking.Event.EventName}'. Please select another venue or date.";
                    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
                    return View(booking);
                }
            }

            if (ModelState.IsValid)
            {
                booking.BookingDate = DateTime.Now;
                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingDate,VenueId,EventId")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Booking updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking cancelled successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}