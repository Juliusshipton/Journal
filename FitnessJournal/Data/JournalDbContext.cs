
using FitnessJournal.Models;
using Microsoft.EntityFrameworkCore;


namespace FitnessJournal.Data
{
    public class JournalDbContext : DbContext
    {
        public JournalDbContext(DbContextOptions<JournalDbContext> options) : base(options) { }

        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<Meal> Meal { get; set; }
        public DbSet<Day> Day { get; set; }
        public DbSet<MealIngredient> MealIngredient { get; set; }
        public DbSet<DayMeal> DayMeal { get; set; }

    }
}
