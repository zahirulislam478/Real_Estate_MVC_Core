using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Real_Estate_Application.Data;
using Real_Estate_Application.Models;
using Real_Estate_Application.ViewModels;
using X.PagedList;

namespace Real_Estate_Application.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;
        public PropertiesController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            this._context = db;
            this.env = env;
        }
        // GET: Properties
        public async Task<IActionResult> Index(int pg = 1)
        {
            var data = await _context.Properties.OrderBy(p => p.PropertyId).Include(a => a.Customers).ToPagedListAsync(pg, 5);
            return View(data);
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Properties == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            var model = new PropertyInputModel { Bedrooms = null, Bathrooms = null, SquareFootage = null, Price = null };
            model.Customers.Add(new Customer { MoveInDate = null });
            return View(model);
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyInputModel data, string act = "")
        {
            if (act == "add")
            {
                data.Customers.Add(new Customer { });
                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                };
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                data.Customers.RemoveAt(index);

                foreach (var v in ModelState.Values)
                {
                    v.RawValue = null;
                    v.Errors.Clear();
                };

            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var a = new Property
                    {
                        Address = data.Address,
                        Type = data.Type,
                        Bedrooms = data.Bedrooms,
                        Bathrooms = data.Bathrooms,
                        SquareFootage = data.SquareFootage,
                        Price = data.Price,
                        IsAvailable = data.IsAvailable
                    };
                    string ext = Path.GetExtension(data.Picture.FileName);
                    string fn = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string sp = Path.Combine(env.WebRootPath, "Pictures", fn);
                    FileStream fs = new FileStream(sp, FileMode.Create);
                    await data.Picture.CopyToAsync(fs);
                    fs.Close();
                    a.Picture = fn;
                    foreach (var x in data.Customers)
                    {
                        //a.Customers.Add(x);
                        a.Customers.Add(new Customer
                        {
                            CustomerName = x.CustomerName,
                            Phone = x.Phone,
                            Email = x.Email,
                            MoveInDate = x.MoveInDate
                        });
                    }
                    await _context.Properties.AddAsync(a);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(data);
        }

        // GET: Properties/Edit/5
        public IActionResult Edit(int id)
        {
            var a = _context.Properties.Include(x => x.Customers).FirstOrDefault(x => x.PropertyId == id);
            if (a == null) return NotFound();

            var data = new PropertyEditModel
            {
                PropertyId = a.PropertyId,
                Address = a.Address,
                Type = a.Type,
                Bedrooms = a.Bedrooms,
                Bathrooms = a.Bathrooms,
                SquareFootage = a.SquareFootage,
                Price = a.Price,
                IsAvailable = a.IsAvailable,
                Customers = a.Customers.ToList()
            };
            ViewBag.CurrentPic = a.Picture;
            return View(data);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PropertyEditModel data, string act = "")
        {
            var a = await _context.Properties.Include(x => x.Customers).FirstOrDefaultAsync(x => x.PropertyId == data.PropertyId);
            if (a == null)
            {
                return NotFound();
            }
            if (act == "add")
            {
                data.Customers.Add(new Customer { });
                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                data.Customers.RemoveAt(index);

                foreach (var v in ModelState.Values)
                {
                    v.RawValue = null;
                    v.Errors.Clear();
                }
            }
            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    a.Address = data.Address;
                    a.Type = data.Type;
                    a.Bedrooms = data.Bedrooms;
                    a.Bathrooms = data.Bathrooms;
                    a.SquareFootage = data.SquareFootage;
                    a.Price = data.Price;
                    a.IsAvailable = data.IsAvailable;

                    if (data.Picture != null)
                    {
                        string ext = Path.GetExtension(data.Picture.FileName);
                        string fn = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string sp = Path.Combine(env.WebRootPath, "Pictures", fn);
                        using (FileStream fs = new FileStream(sp, FileMode.Create))
                        {
                            await data.Picture.CopyToAsync(fs);
                            fs.Close();
                        }
                        a.Picture = fn;
                    }
                    _context.Customers.RemoveRange(_context.Customers.Where(x => x.PropertyId == data.PropertyId).ToList());

                    foreach (var item in data.Customers)
                    {
                        a.Customers.Add(new Customer
                        {
                            //PropertyId = data.PropertyId,
                            CustomerName = item.CustomerName,
                            Phone = item.Phone,
                            Email = item.Email,
                            MoveInDate = item.MoveInDate
                        });
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.CurrentPic = a.Picture;
            return View(data);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Properties == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Properties == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Properties'  is null.");
            }
            var @property = await _context.Properties.FindAsync(id);
            if (@property != null)
            {
                _context.Properties.Remove(@property);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
          return (_context.Properties?.Any(e => e.PropertyId == id)).GetValueOrDefault();
        }
    }
}
