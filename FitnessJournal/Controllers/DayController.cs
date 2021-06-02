using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessJournal.Data;
using FitnessJournal.Models;

namespace FitnessJournal.Controllers
{
    public class DayController : Controller
    {
        private readonly JournalDbContext _context;

        public DayController(JournalDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            // Get all days except the temp day
            return View(await _context.Day.Where(d => !d.Name.Equals("$$$$_TEMP_DAY_$$$$")).ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var day = await _context.Day
                .FirstOrDefaultAsync(m => m.DayId == id);
            if (day == null)
            {
                return NotFound();
            }

            return View(day);
        }

 
        public IActionResult Create()
        {
            // Initialize meal dropdown
            InitializeMealDropDown();

            // Initialize array of mealIngrediens to pass to view
            List<DayMeal> dayMeals = _context.DayMeal.Where(m => m.Day.Name.Equals("$$$$_TEMP_DAY_$$$$")).ToList();
            ViewBag.DayMeals = dayMeals;


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DayId,Date,Name,Description")] Day day)
        {
            if (ModelState.IsValid)
            {

                double cals = 0;
                double pTot = 0;
                double cTot = 0;
                double fTot = 0;

                List<DayMeal> dayMeals = _context.DayMeal.Where(m => m.Day.Name.Equals("$$$$_TEMP_DAY_$$$$")).Include(m => m.Meal).ToList();
                
                foreach(DayMeal dayMeal in dayMeals)
                {
                    // Set to this day
                    dayMeal.Day = day;

                    // Update totals
                    Meal meal = dayMeal.Meal;
                    cals += meal.Calories;
                    pTot += meal.Protein;
                    cTot += meal.Carbs;
                    fTot += meal.Fat;
                }

                day.Calories = cals;
                day.Protein = pTot;
                day.Carbs = cTot;
                day.Fat = fTot;

                _context.Add(day);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(day);
        }

       
        // GET: Day/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var day = await _context.Day.FindAsync(id);
            if (day == null)
            {
                return NotFound();
            }

            DayEditViewModel item = new DayEditViewModel()
            {
                DayId = day.DayId,
                Name = day.Name,
                Description = day.Description,
                Date = day.Date
            };

            // Initialize meal dropdown
            InitializeMealDropDown();

            // Initialize array of mealIngrediens to pass to view
            List<DayMeal> dayMeals = _context.DayMeal.Where(m => m.Day.DayId == id).Include(m => m.Meal).ToList();
            ViewBag.DayMeals = dayMeals;

            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DayId,Date,Name,Description,Calories,Protein,Carbs,Fat")] DayEditViewModel dayEdit)
        {
            Day day = _context.Day.Where(d => d.DayId == dayEdit.DayId).FirstOrDefault();

            if (id != day.DayId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Transfer base info
                    day.Name = dayEdit.Name;
                    day.Description = dayEdit.Description;
                    day.Date = dayEdit.Date;

                    List<DayMeal> dayMeals = _context.DayMeal.Where(m => m.DayId == day.DayId).Include(m => m.Meal).ToList();
                    double cals = 0;
                    double pTot = 0;
                    double cTot = 0;
                    double fTot = 0;

                    foreach (DayMeal dayMeal in dayMeals)
                    {
                        // Set to this day
                        dayMeal.Day = day;

                        // Update totals
                        Meal meal = dayMeal.Meal;
                        cals += meal.Calories;
                        pTot += meal.Protein;
                        cTot += meal.Carbs;
                        fTot += meal.Fat;
                    }

                    day.Calories = cals;
                    day.Protein = pTot;
                    day.Carbs = cTot;
                    day.Fat = fTot;

                    _context.Update(day);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DayExists(day.DayId))
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
            return View(day);
        }

        // GET: Day/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var day = await _context.Day
                .FirstOrDefaultAsync(m => m.DayId == id);
            if (day == null)
            {
                return NotFound();
            }

            return View(day);
        }

   
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            List<DayMeal> dayMeals = _context.DayMeal.Where(m => m.Day.DayId == id).ToList();
            _context.DayMeal.RemoveRange(dayMeals);
            _context.SaveChanges();

            var day = await _context.Day.FindAsync(id);
            _context.Day.Remove(day);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DayExists(int id)
        {
            return _context.Day.Any(e => e.DayId == id);
        }











        public IActionResult AddDayMeal([Bind("MealId")] DayMeal dayMeal)
        {
            // Add dayMeal with temp day as it's day.
            Day day = _context.Day.Where(d => d.Name.Equals("$$$$_TEMP_DAY_$$$$")).FirstOrDefault();
            dayMeal.Day = day;
            _context.DayMeal.Add(dayMeal);
            _context.SaveChanges();

            // Initialize meal dropdown
            InitializeMealDropDown();

            // Initialize array of mealIngrediens to pass to view
            List<DayMeal> dayMeals = _context.DayMeal.Where(m => m.Day.Name.Equals("$$$$_TEMP_DAY_$$$$")).ToList();
            ViewBag.DayMeals = dayMeals;

            // Clear input fields and return view.
            ModelState.Clear();
            return View("Create");
        }


        public IActionResult RemoveDayMeal(int id)
        {
            // Remove specified meal ingredient
            DayMeal dayMeal = _context.DayMeal.Where(m => m.DayMealId == id).FirstOrDefault();
            _context.Remove(dayMeal);
            _context.SaveChanges();

            // Re-initialize select list for add ingredient.
            InitializeMealDropDown();

            // Initialize array of mealIngrediens to pass to view
            List<DayMeal> dayMeals = _context.DayMeal.Where(m => m.Day.Name.Equals("$$$$_TEMP_DAY_$$$$")).ToList();
            ViewBag.DayMeals = dayMeals;

            return View("Create");
        }

        public IActionResult AddDayMealEdit([Bind("DayId,MealId,Date,Name,Description")] DayEditViewModel dayEdit)
        {
            Day day = _context.Day.Where(d => d.DayId == dayEdit.DayId).FirstOrDefault();
            Meal meal = _context.Meal.Where(m => m.MealId == dayEdit.MealId).FirstOrDefault();

            DayMeal dayMeal = new DayMeal()
            {
                Day = day,
                Meal = meal
            };
            _context.DayMeal.Add(dayMeal);
            _context.SaveChanges();


            // Initialize meal dropdown
            InitializeMealDropDown();

            // Initialize array of mealIngrediens to pass to view
            List<DayMeal> dayMeals = _context.DayMeal.Where(m => m.DayId == dayEdit.DayId).Include(m => m.Meal).ToList();
            ViewBag.DayMeals = dayMeals;

            return View("Edit", dayEdit);
        }

        private void InitializeMealDropDown()
        {
            // Re-initialize select list for add ingredient.
            var meals = _context.Meal.Where(m => !m.Name.Equals("$$$$_TEMP_MEAL_$$$$")).ToList();
            List<SelectListItem> listMeals = new List<SelectListItem>();
            foreach (var meal in meals)
            {
                listMeals.Add(new SelectListItem { Text = meal.Name + " (" + meal.Calories + " calories)", Value = meal.MealId.ToString() });
            }
            ViewData["AllMeals"] = listMeals;
        }
    }


    public class DayEditViewModel
    {
        public int DayId { get; set; }
        public int MealId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public Meal Meal { get; set; }

    }


}
