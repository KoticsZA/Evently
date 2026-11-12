using Evently.Data;
using Evently.Models;
using Microsoft.EntityFrameworkCore;

namespace Evently.Data.Seeding
{
    public static class CategorySeeder
    {
        public static async Task SeedAsync(EventlyDBContext context)
        {
            var categories = new List<Category>
            {
                new()
                {
                    Name = "Music",
                    Description = "Concerts, music festivals and live performances."
                },
                new()
                {
                    Name = "Food & Drink",
                    Description = "Food festivals, culinary experiences, wine tastings and dining events."
                },
                new()
                {
                    Name = "Arts & Culture",
                    Description = "Art exhibitions, theatre, cultural festivals and creative experiences."
                },
                new()
                {
                    Name = "Technology",
                    Description = "Technology conferences, hackathons, workshops and innovation events."
                },
                new()
                {
                    Name = "Sports",
                    Description = "Sporting events, tournaments, fitness competitions and live matches."
                },
                new()
                {
                    Name = "Business & Networking",
                    Description = "Business conferences, networking events and professional gatherings."
                },
                new()
                {
                    Name = "Education",
                    Description = "Educational workshops, seminars, training sessions and academic events."
                },
                new()
                {
                    Name = "Festivals",
                    Description = "Community festivals, celebrations and large-scale entertainment events."
                },
                new()
                {
                    Name = "Lifestyle",
                    Description = "Lifestyle experiences, wellness events, fashion and social gatherings."
                }
            };

            var existingNames = await context.Categories
                .Select(c => c.Name)
                .ToListAsync();

            var existingNamesSet = new HashSet<string>(
                existingNames,
                StringComparer.OrdinalIgnoreCase
            );

            var newCategories = categories
                .Where(c => !existingNamesSet.Contains(c.Name))
                .ToList();

            if (newCategories.Count == 0)
                return;

            await context.Categories.AddRangeAsync(newCategories);
            await context.SaveChangesAsync();
        }
    }
}