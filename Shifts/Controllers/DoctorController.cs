using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shifts.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shifts.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ShiftsContext _context;

        public DoctorController(ShiftsContext context)
        {
            this._context = context;
        }

        // GET: Doctor
        public async Task<IActionResult> Index()
        {
            return View(await this._context.Doctors.ToListAsync());
        }

        // GET: Doctor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await this._context.Doctors
                .Where(m => m.DoctorId == id).Include(ds => ds.DoctorSpecialties)
                .ThenInclude(s => s.Specialty).FirstOrDefaultAsync();

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctor/Create
        public IActionResult Create()
        {
            var specialties = this._context.Specialties.ToList();

            if (!specialties.Any())
            {
                ViewBag.NoSpecialties = true;
            }
            else
            {
                ViewData["SpecialtiesList"] = new SelectList(this._context.Specialties, "SpecialtyId", "Description");
            }

            return View();
        }

        // POST: Doctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,FirstName,LastName,Address,PhoneNumber,Email,WorkingHoursFrom,WorkingHoursTo")] Doctor doctor, int? SpecialtyId)
        {
            if (ModelState.IsValid)
            {
                this._context.Doctors.Add(doctor);
                await this._context.SaveChangesAsync();
                //IF SPECIALTY ID IS NULL FIRST MUST BE CREATED A SPECIALITY FOR THIS DOCTOR!!!
                if (SpecialtyId.HasValue)
                {
                    var doctorSpeciality = new DoctorSpecialties
                    {
                        DoctorId = doctor.DoctorId,
                        SpecialtyId = SpecialtyId.Value
                    };
                    this._context.DoctorSpecialties.Add(doctorSpeciality);

                    await this._context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("", "Specialty is required.");
                    ViewData["SpecialitiesList"] = new SelectList(this._context.Specialties, "SpecialtyId", "Description");
                    return View(doctor);
                }




                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialitiesList"] = new SelectList(this._context.Specialties, "SpecialtyId", "Description");
            return View(doctor);
        }

        // GET: Doctor/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Inner Join applied
            var doctor = await this._context.Doctors.Where(d => d.DoctorId == id)
            .Include(ds => ds.DoctorSpecialties).FirstOrDefaultAsync();

            if (doctor == null)
            {
                return NotFound();
            }

            ViewData["SpecialitiesList"] = new SelectList(
                this._context.Specialties, "SpecialityId", "Description", doctor.DoctorSpecialties[0].SpecialtyId
            );

            return View(doctor);
        }

        // POST: Doctor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,FirstName,LastName,Address,PhoneNumber,Email,WorkingHoursFrom,WorkingHoursTo")] Doctor doctor, int SpecialityId)
        {
            if (id != doctor.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await this._context.SaveChangesAsync();

                    var doctorSpeciality = await this._context.DoctorSpecialties
                    .FirstOrDefaultAsync(ds => ds.DoctorId == id);

                    this._context.Remove(doctorSpeciality!);
                    await this._context.SaveChangesAsync();

                    doctorSpeciality!.SpecialtyId = SpecialityId;

                    this._context.Add(doctorSpeciality);

                    await this._context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.DoctorId))
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
            return View(doctor);
        }

        // GET: Doctor/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctorSpeciality = await this._context.DoctorSpecialties
            .FirstOrDefaultAsync(ds => ds.DoctorId == id);

            if (doctorSpeciality != null)
            {
                this._context.DoctorSpecialties.Remove(doctorSpeciality);
                await _context.SaveChangesAsync();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            this._context.Doctors.Remove(doctor!);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }

        public string SetWorkingTimeFrom(int doctorId)
        {

            //TODO: Validate the case if the doctorId is null!!!!!
            var WorkingHoursFrom = _context.Doctors.Where(m => m.DoctorId == doctorId).FirstOrDefault()!.WorkingHoursFrom;

            return WorkingHoursFrom.Hour + ":" + WorkingHoursFrom.Minute;
        }

        public string SetWorkingTimeTo(int doctorId)
        {
            var WorkingHoursTo = _context.Doctors.Where(m => m.DoctorId == doctorId).FirstOrDefault()!.WorkingHoursTo;

            return WorkingHoursTo.Hour + ":" + WorkingHoursTo.Minute;
        }
    }
}
