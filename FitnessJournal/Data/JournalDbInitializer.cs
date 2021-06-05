using FitnessJournal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessJournal.Data
{
    public class JournalDbInitializer
    {


        public static void InitializeDatabase(JournalDbContext _context)
        {
            // Ensure database is created.
            _context.Database.EnsureCreated();

            // Initialize ingredients
            InitializeIngredients(_context);

            // Initialize meals
            InitializeMeals(_context);

            // Initialize days
            InitializeDays(_context);

        }

        private static void InitializeIngredients(JournalDbContext _context)
        {
            // Don't do anything if there are existing ingredients
            if(_context.Ingredient.ToList().Count != 0)
            {
                return;
            }

            List<Ingredient> items = new List<Ingredient>();

            //Add test item to db to fix error.
            Ingredient GroundBeef = new Ingredient()
            {
                Name = "Ground Beef",
                Description = "Specifically 93/7 ground beef",
                Quantity = 4,
                Unit = "ounces",
                Calories = 172,
                Protein = 23.7,
                Carbs = 0,
                Fat = 7.9,
            };
            items.Add(GroundBeef);


            //Add test item to db to fix error.
            Ingredient KetoBread = new Ingredient()
            {
                Name = "Keto Bread",
                Description = "Specifically keto bread",
                Quantity = 1,
                Unit = "slice",
                Calories = 40,
                Protein = 4,
                Carbs = 12,
                Fat = 1.5,
            };
            items.Add(KetoBread);

            //Add test item to db to fix error.
            Ingredient PancakeMix = new Ingredient()
            {
                Name = "Pancake Mix",
                Description = "Specifically buttermilk Kodiac cakes",
                Quantity = 53,
                Unit = "grams",
                Calories = 190,
                Protein = 14,
                Carbs = 30,
                Fat = 2,
            };
            items.Add(PancakeMix);

            //Add test item to db to fix error.
            Ingredient Egg = new Ingredient()
            {
                Name = "Egg",
                Description = "Large regular egg",
                Quantity = 1,
                Unit = "egg",
                Calories = 70,
                Protein = 6,
                Carbs = 0,
                Fat = 5,
            };
            items.Add(Egg);

            //Add test item to db to fix error.
            Ingredient SugarFreeSyrup = new Ingredient()
            {
                Name = "Sugar Free Syrup",
                Description = "Mrs. Buttersworth sugar free syrup",
                Quantity = 2,
                Unit = "tbs",
                Calories = 10,
                Protein = 0,
                Carbs = 4,
                Fat = 0,
            };
            items.Add(SugarFreeSyrup);



            // Add all to database.
            _context.Ingredient.AddRange(items);
            _context.SaveChanges();
        }
    
        
        private static void InitializeMeals(JournalDbContext _context)
        {
            // Ensure temp Meal is created for proper data binding and passing within Meal Create
            if (_context.Meal.ToList().Count == 0)
            {
                Meal temp = new Meal()
                {
                    Name = "$$$$_TEMP_MEAL_$$$$",
                    Description = "$$$$_TEMP_MEAL_$$$$",
                };

                _context.Meal.Add(temp);
                _context.SaveChanges();
            }
        }
    

        private static void InitializeDays(JournalDbContext _context)
        {
            // Ensure temp Meal is created for proper data binding and passing within Meal Create
            if (_context.Day.ToList().Count == 0)
            {
                Day temp = new Day()
                {
                    Name = "$$$$_TEMP_DAY_$$$$",
                    Description = "$$$$_TEMP_DAY_$$$$",
                };

                _context.Day.Add(temp);
                _context.SaveChanges();
            }
        }
    }
}
