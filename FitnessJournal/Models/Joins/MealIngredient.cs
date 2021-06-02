using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessJournal.Models
{
    public class MealIngredient
    {
        public int MealIngredientId { get; set; }
        public int MealId { get; set; }
        public int IngredientId { get; set; }
        public double Quantity { get; set; }

        public virtual Meal Meal { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
