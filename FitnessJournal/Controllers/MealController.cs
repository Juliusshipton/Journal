using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessJournal.Data;
using FitnessJournal.Models;
using Microsoft.AspNetCore.Http;

namespace FitnessJournal.Controllers
{
    public class MealController : Controller
    {
        
        private readonly JournalDbContext _context;


        public MealController(JournalDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            // Display all meals that arent the temp meal.
            return View(await _context.Meal.Where(m => !m.Name.Equals("$$$$_TEMP_MEAL_$$$$")).ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }

            var meal = await _context.Meal.FirstOrDefaultAsync(m => m.MealId == id);

            if (meal == null) { return NotFound(); }

            // Initialize array of mealIngrediens to pass to view
            List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.MealId == id).Include(i => i.Ingredient).ToList();
            ViewBag.MealIngredients = mealIngredients;

            return View(meal);
        }


        public IActionResult Create()
        {
            // Get and make list of ingredients for dropdown. 
            InitializeIngredientDropDown();

            // Ensure all temp MealItems are removed.
            List<MealIngredient> tempItems = _context.MealIngredient.Where(m => m.Meal.Name.Equals("$$$$_TEMP_MEAL_$$$$")).ToList();
            _context.RemoveRange(tempItems);
            _context.SaveChanges();

            // Initialize array of mealIngrediens to pass to view
            List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.Meal.Name.Equals("$$$$_TEMP_MEAL_$$$$")).ToList();
            ViewBag.MealIngredients = mealIngredients;


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Meal meal)
        {
            if (ModelState.IsValid)
            {
                double cals = 0;
                double pTot = 0;
                double cTot = 0;
                double fTot = 0;

                // Get all associated meal ingredients, update totals, and update their meal.
                List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.Meal.Name.Equals("$$$$_TEMP_MEAL_$$$$")).Include(i => i.Ingredient).ToList();
                foreach(MealIngredient ing in mealIngredients)
                {
                    // Associate ingredient to this meal
                    ing.Meal = meal;
                    cals += (ing.Ingredient.Calories * ing.Quantity);
                    pTot += (ing.Ingredient.Protein * ing.Quantity);
                    cTot += (ing.Ingredient.Carbs * ing.Quantity);
                    fTot += (ing.Ingredient.Fat * ing.Quantity);
                }

                meal.Calories = cals;
                meal.Protein = pTot;
                meal.Carbs = cTot;
                meal.Fat = fTot;
                
                _context.Add(meal);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(meal);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { return NotFound(); }

            var meal = await _context.Meal.FindAsync(id);
            
            if (meal == null) { return NotFound(); }

            // Get and make list of ingredients for dropdown. 
            InitializeIngredientDropDown();

            // Initialize array of mealIngrediens to pass to view
            List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.MealId == id).Include(i => i.Ingredient).ToList();
            ViewBag.MealIngredients = mealIngredients;

            MealEditViewModel item = new MealEditViewModel
            {
                MealId = meal.MealId,
                Name = meal.Name,
                Description = meal.Description
            };

            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MealId,Name,Description")] MealEditViewModel mealView)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Meal meal = _context.Meal.Where(m => m.MealId == mealView.MealId).FirstOrDefault();
                    double cals = 0;
                    double pTot = 0;
                    double cTot = 0;
                    double fTot = 0;

                    // Get all associated meal ingredients, update totals, and update their meal.
                    List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.MealId == mealView.MealId).Include(i => i.Ingredient).ToList();
                    foreach (MealIngredient ing in mealIngredients)
                    {
                        // Associate ingredient to this meal
                        ing.Meal = meal;
                        cals += (ing.Ingredient.Calories * ing.Quantity);
                        pTot += (ing.Ingredient.Protein * ing.Quantity);
                        cTot += (ing.Ingredient.Carbs * ing.Quantity);
                        fTot += (ing.Ingredient.Fat * ing.Quantity);
                    }

                    meal.Calories = cals;
                    meal.Protein = pTot;
                    meal.Carbs = cTot;
                    meal.Fat = fTot;

                    // Save changes.
                    _context.Update(meal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meal = await _context.Meal
                .FirstOrDefaultAsync(m => m.MealId == id);
            if (meal == null)
            {
                return NotFound();
            }

            return View(meal);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meal = await _context.Meal.FindAsync(id);

            // Get and remove all MealIngredients associated with this meal.
            List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.MealId == id).ToList();
            _context.RemoveRange(mealIngredients);
            _context.SaveChanges();

            _context.Meal.Remove(meal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MealExists(int id)
        {
            return _context.Meal.Any(e => e.MealId == id);
        }



        [HttpPost]
        public IActionResult AddMealIngredient([Bind("IngredientId,Quantity")] MealIngredient mealIngredient)
        {
            // Re-initialize select list for add ingredient.
            InitializeIngredientDropDown();

            // Make mealIngredient and add to db with temp meal as it's meal. 
            Meal tempMeal = _context.Meal.Where(m => m.Name.Equals("$$$$_TEMP_MEAL_$$$$")).FirstOrDefault();
            Ingredient selection = _context.Ingredient.Where(i => i.IngredientId == mealIngredient.IngredientId).FirstOrDefault();
            MealIngredient item = new MealIngredient()
            {
                Quantity = mealIngredient.Quantity,
                Ingredient = selection,
                Meal = tempMeal
            };
            _context.MealIngredient.Add(item);
            _context.SaveChanges();

            // Initialize array of mealIngrediens to pass to view
            List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.Meal.Name.Equals("$$$$_TEMP_MEAL_$$$$")).ToList();
            ViewBag.MealIngredients = mealIngredients;

            // Clear input fields and return view.
            ModelState.Clear();
            return View("Create");
        }

        public IActionResult RemoveMealIngredient(int id)
        {

            // Remove specified meal ingredient
            MealIngredient mealIngredient = _context.MealIngredient.Where(m => m.MealIngredientId == id).FirstOrDefault();
            _context.Remove(mealIngredient);
            _context.SaveChanges();

            // Re-initialize select list for add ingredient.
            InitializeIngredientDropDown();

            // Initialize array of mealIngrediens to pass to view
            List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.Meal.Name.Equals("$$$$_TEMP_MEAL_$$$$")).ToList();
            ViewBag.MealIngredients = mealIngredients;

            return View("Create");
        }

        public IActionResult RemoveMealIngredientFromEdit(int id)
        {

            // Remove specified meal ingredient
            MealIngredient mealIngredient = _context.MealIngredient.Where(m => m.MealIngredientId == id).FirstOrDefault();

            Meal associatedMeal = _context.Meal.Where(m => m.MealId == mealIngredient.MealId).FirstOrDefault();
            
            
            _context.Remove(mealIngredient);
            _context.SaveChanges();


            // Re-initialize select list for add ingredient.
            InitializeIngredientDropDown();

            // Initialize array of mealIngrediens to pass to view
            List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.MealId == associatedMeal.MealId).Include(i => i.Ingredient).ToList();
            ViewBag.MealIngredients = mealIngredients;

            MealEditViewModel item = new MealEditViewModel
            {
                MealId = associatedMeal.MealId,
                Name = associatedMeal.Name,
                Description = associatedMeal.Description
            };



            return View("Edit", item);
        }


        public IActionResult AddMealIngredientEdit([Bind("MealId,IngredientId,Quantity,Name,Description")] MealEditViewModel editViewModel)
        {
            // Create and add MealIngredient with info from ViewModel
            Meal tempMeal = _context.Meal.Where(m => m.MealId == editViewModel.MealId).FirstOrDefault();
            Ingredient selection = _context.Ingredient.Where(i => i.IngredientId == editViewModel.IngredientId).FirstOrDefault();
            MealIngredient item = new MealIngredient()
            {
                Quantity = editViewModel.Quantity,
                Ingredient = selection,
                Meal = tempMeal
            };
            _context.MealIngredient.Add(item);
            _context.SaveChanges();

            // Re-initialize select list for add ingredient.
            InitializeIngredientDropDown();

            // Initialize array of mealIngrediens to pass to view
            List<MealIngredient> mealIngredients = _context.MealIngredient.Where(m => m.Meal.MealId == editViewModel.MealId).ToList();
            ViewBag.MealIngredients = mealIngredients;

            // Clear input fields and return view.
            ModelState.Clear();
            return View("Edit", editViewModel);
        }

        /**
         * Helper methods for data passing / initializing drop down.
         */
        private void InitializeIngredientDropDown()
        {
            // Re-initialize select list for add ingredient.
            var ingredients = _context.Ingredient.ToList();
            List<SelectListItem> listIngredients = new List<SelectListItem>();
            foreach (var ingredient in ingredients)
            {
                listIngredients.Add(new SelectListItem { Text = ingredient.Name + " (" + ingredient.Quantity + " " + ingredient.Unit + ")", Value = ingredient.IngredientId.ToString() });
            }
            ViewData["AllIngredients"] = listIngredients;
        }

    }

    public class MealEditViewModel
    {
        public int MealId { get; set; }
        public int IngredientId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }

        public Ingredient Ingredient { get; set; }

    }
}
