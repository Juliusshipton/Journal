using FitnessJournal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessJournal.Data
{
    /*
     * Helper class for common queries to refactor code from controllers. 
     */
    public class Queries
    {
        // Queries for meal controller.
        public class Meal
        {
            public static List<MealIngredient> GetTempMealIngredients(JournalDbContext _context) => 
                _context.MealIngredient.Where(m => m.Meal.Name.Equals("$$$$_TEMP_MEAL_$$$$")).Include(i => i.Ingredient).ToList();
            

        }
    }
}
