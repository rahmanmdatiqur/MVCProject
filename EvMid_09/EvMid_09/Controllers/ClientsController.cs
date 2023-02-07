using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EvMid_09.Models;
using EvMid_09.Models.ViewModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace EvMid_09.Controllers
{
    public class ClientsController : Controller
    {
        private readonly TourDbContext _context;
        private readonly IWebHostEnvironment _he;

        public ClientsController(TourDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }

        public IActionResult Index()
        {
            return View(_context.Clients.Include(x => x.BookingEntries).ThenInclude(b => b.Spot).ToList());
        }

        public IActionResult AddNewSpot(int? id)
        {
            ViewBag.Spots = new SelectList(_context.Spots.ToList(), "SpotId", "SpotName", id.ToString() ?? "");
            return PartialView("_AddNewSpot");
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClientVM clientVM, int[] SpotId)
        {
            if (ModelState.IsValid)
            {
                Client client = new Client()
                {
                    ClientName = clientVM.ClientName,
                    BirthDate = clientVM.BirthDate,
                    Phone = clientVM.Phone,
                    MaritialStatus = clientVM.MaritialStatus
                };
                //Img
                string webroot = _he.WebRootPath;
                string folder = "Images";
                string imgFileName = Path.GetFileName(clientVM.PictureFile.FileName);
                string fileToWrite = Path.Combine(webroot, folder, imgFileName);

                using (var stream = new FileStream(fileToWrite, FileMode.Create))
                {
                    await clientVM.PictureFile.CopyToAsync(stream);
                    client.Picture = "/" + folder + "/" + imgFileName;
                }
                foreach (var item in SpotId)
                {
                    BookingEntry bookingEntry = new BookingEntry()
                    {
                        Client = client,
                        ClientId = client.ClientId,
                        SpotId = item
                    };
                    _context.BookingEntries.Add(bookingEntry);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            Client client = _context.Clients.First(x => x.ClientId == id);
            var spotList = _context.BookingEntries.Where(x => x.ClientId == id).Select(x => x.SpotId).ToList();


            ClientVM clientVM = new ClientVM
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                BirthDate = client.BirthDate,
                Phone = client.Phone,
                MaritialStatus = client.MaritialStatus,
                SpotList = spotList
            };
            return View(clientVM);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(ClientVM clientVM, int[] SpotId)
        {

            if (ModelState.IsValid)
            {
                Client client = new Client()
                {
                    ClientId = clientVM.ClientId,
                    ClientName = clientVM.ClientName,
                    BirthDate = clientVM.BirthDate,
                    Phone = clientVM.Phone,
                    MaritialStatus = clientVM.MaritialStatus
                };
                //Img
                string webroot = _he.WebRootPath;
                string folder = "Images";
                string imgFileName = Path.GetFileName(clientVM.PictureFile.FileName);
                string fileToWrite = Path.Combine(webroot, folder, imgFileName);

                using (var stream = new FileStream(fileToWrite, FileMode.Create))
                {
                    await clientVM.PictureFile.CopyToAsync(stream);
                    client.Picture = "/" + folder + "/" + imgFileName;
                }
                //exists spotList
                var existsSpot = _context.BookingEntries.Where(x => x.ClientId == clientVM.ClientId).ToList();
                foreach (var item in existsSpot)
                {
                    _context.BookingEntries.Remove(item);
                }
                //add new spotList
                foreach (var item in SpotId)
                {
                    BookingEntry bookingEntry = new BookingEntry()
                    {
                        ClientId = client.ClientId,
                        SpotId = item
                    };
                    _context.BookingEntries.Add(bookingEntry);
                }
                _context.Entry(client).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            Client client = _context.Clients.First(x => x.ClientId == id);
            var spotList = _context.BookingEntries.Where(x => x.ClientId == id).ToList();

            foreach (var item in spotList)
            {
                _context.BookingEntries.Remove(item);
            }
            _context.Entry(client).State = EntityState.Deleted;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
