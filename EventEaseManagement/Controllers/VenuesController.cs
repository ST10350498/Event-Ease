using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseManagement.Data;
using EventEaseManagement.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace EventEaseManagement.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile? ImageFile)
        {
            Console.WriteLine("=== CREATE METHOD STARTED ===");
            Console.WriteLine($"Venue Name: {venue.VenueName}");
            Console.WriteLine($"ImageFile is null: {ImageFile == null}");

            if (ImageFile != null)
            {
                Console.WriteLine($"Image file name: {ImageFile.FileName}");
                Console.WriteLine($"Image file size: {ImageFile.Length} bytes");
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is VALID");

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    Console.WriteLine("Starting image upload to Azurite...");

                    try
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var connectionString = "UseDevelopmentStorage=true";
                        var containerName = "venue-images";

                        Console.WriteLine($"Connecting to Azurite with connection string: {connectionString}");

                        var blobServiceClient = new BlobServiceClient(connectionString);
                        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                        await containerClient.CreateIfNotExistsAsync();

                        var blobClient = containerClient.GetBlobClient(fileName);
                        using (var stream = ImageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = ImageFile.ContentType });
                        }
                        venue.ImageUrl = blobClient.Uri.ToString();

                        Console.WriteLine($"Image uploaded successfully. URL: {venue.ImageUrl}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR uploading image: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        ModelState.AddModelError("", $"Image upload failed: {ex.Message}");
                        return View(venue);
                    }
                }
                else
                {
                    Console.WriteLine("No image to upload");
                }

                Console.WriteLine("Adding venue to database...");
                _context.Add(venue);
                await _context.SaveChangesAsync();
                Console.WriteLine("Venue saved to database successfully!");

                TempData["Success"] = $"Venue '{venue.VenueName}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine("ModelState is INVALID");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Model error for {key}: {error.ErrorMessage}");
                    }
                }
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile? ImageFile)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var connectionString = "UseDevelopmentStorage=true";
                        var containerName = "venue-images";

                        var blobServiceClient = new BlobServiceClient(connectionString);
                        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                        await containerClient.CreateIfNotExistsAsync();

                        var blobClient = containerClient.GetBlobClient(fileName);
                        using (var stream = ImageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = ImageFile.ContentType });
                        }
                        venue.ImageUrl = blobClient.Uri.ToString();
                    }
                    else
                    {
                        var existingVenue = await _context.Venues.AsNoTracking().FirstOrDefaultAsync(v => v.VenueId == id);
                        if (existingVenue != null)
                        {
                            venue.ImageUrl = existingVenue.ImageUrl;
                        }
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Venue '{venue.VenueName}' updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
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
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5 (UPDATED with booking count)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(v => v.VenueId == id);

            if (venue != null && venue.Bookings != null && venue.Bookings.Any())
            {
                int bookingCount = venue.Bookings.Count;
                TempData["Error"] = $"❌ Cannot delete venue '{venue.VenueName}'. It has {bookingCount} active booking(s). Please cancel the booking(s) first before deleting this venue.";
                return RedirectToAction(nameof(Index));
            }

            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"✅ Venue '{venue.VenueName}' deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}