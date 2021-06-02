using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessJournal.Models
{
    public class DayMeal
    {
        public int DayMealId { get; set; }
        public int DayId { get; set; }
        public int MealId { get; set; }
        public DateTime DateTime { get; set; }

        public virtual Meal Meal { get; set; }
        public virtual Day Day { get; set; }
    }
}
