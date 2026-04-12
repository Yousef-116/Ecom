using Ecom.Core.Entites.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ecom.infrastructure.Data;

public static class DataSeeder
{
    private const string SeedMarker = "__SEEDED_BY_DATASEEDER_V1__";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            // ─── Step 1: Seed Products if they don't exist ──────────────
            var seededProducts = await context.Products
                .Where(p => p.Description != null && p.Description.Contains(SeedMarker))
                .ToListAsync();

            if (seededProducts.Count == 0)
            {
                logger.LogInformation("DataSeeder: Adding products...");

                var products = new List<Product>
                {
                    // Electronics (CategoryId = 1)
                    new Product { Name = "Wireless Headphones", Description = $"Premium noise-cancelling wireless headphones with 30hr battery life and Hi-Res audio support {SeedMarker}", OldPrice = 349.99m, NewPrice = 249.99m, CategoryId = 1 },
                    new Product { Name = "Smart Watch", Description = $"Advanced smartwatch with health monitoring, GPS tracking, and AMOLED display {SeedMarker}", OldPrice = 399.99m, NewPrice = 299.99m, CategoryId = 1 },
                    new Product { Name = "Mechanical Keyboard", Description = $"RGB mechanical gaming keyboard with Cherry MX switches and aluminum frame {SeedMarker}", OldPrice = 159.99m, NewPrice = 119.99m, CategoryId = 1 },
                    new Product { Name = "4K Monitor", Description = $"27-inch 4K UHD IPS monitor with HDR400, USB-C connectivity, and 99pct sRGB {SeedMarker}", OldPrice = 549.99m, NewPrice = 449.99m, CategoryId = 1 },

                    // Clothing (CategoryId = 2)
                    new Product { Name = "Leather Jacket", Description = $"Premium genuine leather jacket with quilted lining and vintage design {SeedMarker}", OldPrice = 299.99m, NewPrice = 199.99m, CategoryId = 2 },
                    new Product { Name = "Running Shoes", Description = $"Lightweight running shoes with responsive cushioning and breathable mesh upper {SeedMarker}", OldPrice = 129.99m, NewPrice = 89.99m, CategoryId = 2 },
                    new Product { Name = "Denim Jeans", Description = $"Classic slim-fit denim jeans with stretch comfort and premium wash {SeedMarker}", OldPrice = 79.99m, NewPrice = 49.99m, CategoryId = 2 },
                    new Product { Name = "Winter Coat", Description = $"Insulated waterproof winter coat with faux fur hood and thermal lining {SeedMarker}", OldPrice = 249.99m, NewPrice = 179.99m, CategoryId = 2 },

                    // Home & Kitchen (CategoryId = 3)
                    new Product { Name = "Coffee Maker", Description = $"Programmable 12-cup coffee maker with built-in grinder and thermal carafe {SeedMarker}", OldPrice = 199.99m, NewPrice = 149.99m, CategoryId = 3 },
                    new Product { Name = "Air Fryer", Description = $"Digital air fryer with 5.8QT capacity, 8 preset cooking functions and non-stick basket {SeedMarker}", OldPrice = 129.99m, NewPrice = 89.99m, CategoryId = 3 },
                    new Product { Name = "Robot Vacuum", Description = $"Smart robot vacuum with laser navigation, auto-empty station, and app control {SeedMarker}", OldPrice = 499.99m, NewPrice = 349.99m, CategoryId = 3 },
                    new Product { Name = "Stand Mixer", Description = $"Professional 5QT stand mixer with 10-speed control and stainless steel bowl {SeedMarker}", OldPrice = 379.99m, NewPrice = 279.99m, CategoryId = 3 },

                    // Books (CategoryId = 4)
                    new Product { Name = "Clean Code", Description = $"A Handbook of Agile Software Craftsmanship by Robert C. Martin - Essential reading for developers {SeedMarker}", OldPrice = 49.99m, NewPrice = 34.99m, CategoryId = 4 },
                    new Product { Name = "The Pragmatic Programmer", Description = $"Your Journey to Mastery, 20th Anniversary Edition - Timeless guide for software engineers {SeedMarker}", OldPrice = 54.99m, NewPrice = 39.99m, CategoryId = 4 },
                    new Product { Name = "Atomic Habits", Description = $"An Easy and Proven Way to Build Good Habits and Break Bad Ones by James Clear {SeedMarker}", OldPrice = 27.99m, NewPrice = 16.99m, CategoryId = 4 },
                    new Product { Name = "Design Patterns", Description = $"Elements of Reusable Object-Oriented Software by the Gang of Four - CS classic {SeedMarker}", OldPrice = 59.99m, NewPrice = 44.99m, CategoryId = 4 },

                    // Sports & Outdoors (CategoryId = 5)
                    new Product { Name = "Yoga Mat", Description = $"Extra thick non-slip yoga mat with alignment lines and carrying strap {SeedMarker}", OldPrice = 49.99m, NewPrice = 29.99m, CategoryId = 5 },
                    new Product { Name = "Camping Tent", Description = $"4-person waterproof camping tent with easy setup and UV protection {SeedMarker}", OldPrice = 199.99m, NewPrice = 139.99m, CategoryId = 5 },
                    new Product { Name = "Dumbbells Set", Description = $"Adjustable dumbbells set 5-52.5 lbs with quick-change weight system {SeedMarker}", OldPrice = 349.99m, NewPrice = 249.99m, CategoryId = 5 },
                    new Product { Name = "Mountain Bike", Description = $"21-speed mountain bike with aluminum frame, disc brakes, and front suspension {SeedMarker}", OldPrice = 599.99m, NewPrice = 449.99m, CategoryId = 5 },
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
                logger.LogInformation("DataSeeder: Added {Count} products.", products.Count);

                // Reload from DB to get IDs
                seededProducts = await context.Products
                    .Where(p => p.Description != null && p.Description.Contains(SeedMarker))
                    .ToListAsync();
            }
            else
            {
                logger.LogInformation("DataSeeder: Products already exist ({Count} found). Skipping product insert.", seededProducts.Count);
            }

            // Build lookup: Name -> Id
            var productIds = seededProducts.ToDictionary(p => p.Name!, p => p.Id);

            // ─── Step 2: Seed Photos if missing ─────────────────────────
            var seededProductIdsList = seededProducts.Select(p => p.Id).ToList();
            var hasPhotos = await context.Photos.AnyAsync(ph => seededProductIdsList.Contains(ph.ProductId));

            if (!hasPhotos)
            {
                logger.LogInformation("DataSeeder: Adding photos...");

                var photos = new List<Photo>
                {
                    new Photo { ImageName = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600", ProductId = productIds["Wireless Headphones"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1484704849700-f032a568e944?w=600", ProductId = productIds["Wireless Headphones"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600", ProductId = productIds["Smart Watch"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1546868871-af0de0ae72be?w=600", ProductId = productIds["Smart Watch"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1511467687858-23d96c32e4ae?w=600", ProductId = productIds["Mechanical Keyboard"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=600", ProductId = productIds["Mechanical Keyboard"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=600", ProductId = productIds["4K Monitor"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=600", ProductId = productIds["4K Monitor"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=600", ProductId = productIds["Leather Jacket"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1520012218364-3dbe62c99bee?w=600", ProductId = productIds["Leather Jacket"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=600", ProductId = productIds["Running Shoes"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1606107557195-0e29a4b5b4aa?w=600", ProductId = productIds["Running Shoes"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=600", ProductId = productIds["Denim Jeans"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=600", ProductId = productIds["Denim Jeans"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1539533018447-63fcce2678e3?w=600", ProductId = productIds["Winter Coat"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1544923246-77307dd270cb?w=600", ProductId = productIds["Winter Coat"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1517701550927-30cf4ba1dba5?w=600", ProductId = productIds["Coffee Maker"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=600", ProductId = productIds["Coffee Maker"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1648733966427-1e0ace644de4?w=600", ProductId = productIds["Air Fryer"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1585515320310-259814833e62?w=600", ProductId = productIds["Air Fryer"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1558618666-fcd25c85f82e?w=600", ProductId = productIds["Robot Vacuum"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1603618090554-13acb4299d10?w=600", ProductId = productIds["Robot Vacuum"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1594631252845-29fc4cc8cde9?w=600", ProductId = productIds["Stand Mixer"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=600", ProductId = productIds["Stand Mixer"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=600", ProductId = productIds["Clean Code"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=600", ProductId = productIds["The Pragmatic Programmer"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1512820790803-83ca734da794?w=600", ProductId = productIds["Atomic Habits"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1497633762265-9d179a990aa6?w=600", ProductId = productIds["Design Patterns"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=600", ProductId = productIds["Yoga Mat"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=600", ProductId = productIds["Yoga Mat"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=600", ProductId = productIds["Camping Tent"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=600", ProductId = productIds["Camping Tent"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=600", ProductId = productIds["Dumbbells Set"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1583454110551-21f2fa2afe61?w=600", ProductId = productIds["Dumbbells Set"] },

                    new Photo { ImageName = "https://images.unsplash.com/photo-1576435728678-68d0fbf94e91?w=600", ProductId = productIds["Mountain Bike"] },
                    new Photo { ImageName = "https://images.unsplash.com/photo-1485965120184-e220f721d03e?w=600", ProductId = productIds["Mountain Bike"] },
                };

                await context.Photos.AddRangeAsync(photos);
                await context.SaveChangesAsync();
                logger.LogInformation("DataSeeder: Added {Count} photos.", photos.Count);
            }
            else
            {
                logger.LogInformation("DataSeeder: Photos already exist. Skipping.");
            }

            // ─── Step 3: Seed Ratings if missing ────────────────────────
            var hasRatings = await context.ProductRatings.AnyAsync(r => seededProductIdsList.Contains(r.ProductId));

            if (!hasRatings)
            {
                logger.LogInformation("DataSeeder: Adding ratings...");

                var ratings = new List<ProductRating>
                {
                    new ProductRating { Username = "AudioFan", Message = "Incredible sound quality! The noise cancellation is truly next level. Best headphones I have ever owned.", Score = 5, ProductId = productIds["Wireless Headphones"] },
                    new ProductRating { Username = "MusicLover22", Message = "Great bass and comfortable for long listening sessions. Battery lasts forever.", Score = 5, ProductId = productIds["Wireless Headphones"] },
                    new ProductRating { Username = "TechReview", Message = "Sound is decent but the ear cushions started peeling after 3 months. Disappointing build quality.", Score = 2, ProductId = productIds["Wireless Headphones"] },

                    new ProductRating { Username = "FitnessGuru", Message = "Perfect fitness companion. Accurate heart rate and GPS tracking. Love the AMOLED display!", Score = 5, ProductId = productIds["Smart Watch"] },
                    new ProductRating { Username = "DailyUser", Message = "Battery barely lasts a full day with all features on. Expected more for the price.", Score = 2, ProductId = productIds["Smart Watch"] },
                    new ProductRating { Username = "WatchCollector", Message = "Sleek design and smooth interface. Notifications work perfectly with my phone.", Score = 4, ProductId = productIds["Smart Watch"] },

                    new ProductRating { Username = "GamerPro", Message = "The Cherry MX switches feel amazing. RGB lighting is customizable and bright. Worth every penny!", Score = 5, ProductId = productIds["Mechanical Keyboard"] },
                    new ProductRating { Username = "OfficeWorker", Message = "Way too loud for a shared office. Had to return it. The typing experience is great though.", Score = 3, ProductId = productIds["Mechanical Keyboard"] },
                    new ProductRating { Username = "DevCoder", Message = "Best keyboard for programming. The build quality is solid aluminum. No flex at all.", Score = 5, ProductId = productIds["Mechanical Keyboard"] },

                    new ProductRating { Username = "Designer101", Message = "Colors are incredibly accurate. Perfect for photo and video editing. The USB-C is a game changer.", Score = 5, ProductId = productIds["4K Monitor"] },
                    new ProductRating { Username = "BudgetBuyer", Message = "Good monitor but came with a dead pixel. Customer support was slow to respond.", Score = 2, ProductId = productIds["4K Monitor"] },

                    new ProductRating { Username = "StyleKing", Message = "Genuine leather quality is superb. Fits perfectly and looks amazing. Getting tons of compliments!", Score = 5, ProductId = productIds["Leather Jacket"] },
                    new ProductRating { Username = "FashionCritic", Message = "Runs a bit small. Order one size up. The leather smell was strong at first but faded.", Score = 3, ProductId = productIds["Leather Jacket"] },
                    new ProductRating { Username = "WinterReady", Message = "Not warm enough for really cold weather. Looks great but more of a fashion piece.", Score = 3, ProductId = productIds["Leather Jacket"] },

                    new ProductRating { Username = "MarathonRunner", Message = "These shoes changed my running game. Super lightweight and the cushioning is perfect.", Score = 5, ProductId = productIds["Running Shoes"] },
                    new ProductRating { Username = "CasualJogger", Message = "Comfortable for daily use. Good arch support. The breathable mesh keeps feet cool.", Score = 4, ProductId = productIds["Running Shoes"] },
                    new ProductRating { Username = "ShoeExpert", Message = "Sole started wearing out after just 2 months of daily running. Not durable enough.", Score = 1, ProductId = productIds["Running Shoes"] },

                    new ProductRating { Username = "DenimLover", Message = "Perfect fit and great stretch. These are my go-to jeans now. The wash color is exactly as shown.", Score = 5, ProductId = productIds["Denim Jeans"] },
                    new ProductRating { Username = "CasualDresser", Message = "Color faded significantly after just a few washes. Disappointing for the price.", Score = 2, ProductId = productIds["Denim Jeans"] },

                    new ProductRating { Username = "SnowBunny", Message = "Kept me warm in -20C weather. The hood is amazing and the waterproofing really works!", Score = 5, ProductId = productIds["Winter Coat"] },
                    new ProductRating { Username = "CityDweller", Message = "Too bulky for everyday city wear. Great for actual winter activities though.", Score = 3, ProductId = productIds["Winter Coat"] },

                    new ProductRating { Username = "CoffeeAddict", Message = "The built-in grinder makes a huge difference. Fresh ground coffee every morning. Absolute game changer!", Score = 5, ProductId = productIds["Coffee Maker"] },
                    new ProductRating { Username = "MorningPerson", Message = "Programmable timer is great but the machine is quite noisy. Wakes up the whole house.", Score = 3, ProductId = productIds["Coffee Maker"] },
                    new ProductRating { Username = "BaristaAtHome", Message = "Leaks water from the bottom after a month. Already on my second replacement.", Score = 1, ProductId = productIds["Coffee Maker"] },

                    new ProductRating { Username = "HealthyChef", Message = "Makes crispy food without all the oil! Perfect fries every time. Easy to clean too.", Score = 5, ProductId = productIds["Air Fryer"] },
                    new ProductRating { Username = "HomeCook", Message = "Smaller than expected. Can only cook for 2 people max. Food quality is excellent though.", Score = 3, ProductId = productIds["Air Fryer"] },

                    new ProductRating { Username = "SmartHomeFan", Message = "This thing is incredible! Maps out the whole house perfectly and the auto-empty is genius.", Score = 5, ProductId = productIds["Robot Vacuum"] },
                    new ProductRating { Username = "PetOwner", Message = "Great at picking up pet hair. Runs daily on schedule without any issues. Love the app control.", Score = 5, ProductId = productIds["Robot Vacuum"] },
                    new ProductRating { Username = "CleanFreak", Message = "Gets stuck under furniture constantly. Not smart enough for a house with lots of obstacles.", Score = 2, ProductId = productIds["Robot Vacuum"] },

                    new ProductRating { Username = "BakingQueen", Message = "Powerful motor handles even thick bread dough. The stainless steel bowl is huge. Professional quality!", Score = 5, ProductId = productIds["Stand Mixer"] },
                    new ProductRating { Username = "WeekendBaker", Message = "Heavy and takes up a lot of counter space. Great for baking but hard to store.", Score = 3, ProductId = productIds["Stand Mixer"] },

                    new ProductRating { Username = "JuniorDev", Message = "This book transformed how I write code. Every developer should read this. Clear examples and practical advice.", Score = 5, ProductId = productIds["Clean Code"] },
                    new ProductRating { Username = "SeniorEngineer", Message = "Some advice is outdated and too Java-centric. Still worth reading for the core principles though.", Score = 3, ProductId = productIds["Clean Code"] },
                    new ProductRating { Username = "CodeNewbie", Message = "Essential reading! I finally understand why code structure matters. My code reviews improved dramatically.", Score = 5, ProductId = productIds["Clean Code"] },

                    new ProductRating { Username = "FullStackDev", Message = "The 20th anniversary edition is even better. Timeless advice that applies to any language or framework.", Score = 5, ProductId = productIds["The Pragmatic Programmer"] },
                    new ProductRating { Username = "BootcampGrad", Message = "Some concepts were over my head as a beginner. Better suited for intermediate developers.", Score = 3, ProductId = productIds["The Pragmatic Programmer"] },

                    new ProductRating { Username = "SelfImprover", Message = "Life-changing book! The habit stacking technique alone was worth the price. Read it twice already.", Score = 5, ProductId = productIds["Atomic Habits"] },
                    new ProductRating { Username = "BookWorm", Message = "Great ideas but could have been a blog post. Very repetitive after the first few chapters.", Score = 2, ProductId = productIds["Atomic Habits"] },
                    new ProductRating { Username = "ProductivityFan", Message = "Simple, actionable advice. I have built 3 new habits since reading this. Highly recommend!", Score = 5, ProductId = productIds["Atomic Habits"] },

                    new ProductRating { Username = "ArchitectDev", Message = "The bible of software design. Every pattern is explained with clear UML diagrams. A must-have reference.", Score = 5, ProductId = productIds["Design Patterns"] },
                    new ProductRating { Username = "PythonDev", Message = "Examples are all in C++ and Smalltalk which makes it hard to follow. Content is gold though.", Score = 3, ProductId = productIds["Design Patterns"] },

                    new ProductRating { Username = "YogaDaily", Message = "Perfect thickness and grip. The alignment lines help with proper positioning. No slipping at all!", Score = 5, ProductId = productIds["Yoga Mat"] },
                    new ProductRating { Username = "BeginnerYogi", Message = "Mat has a strong chemical smell that took weeks to go away. Functionality is good once aired out.", Score = 2, ProductId = productIds["Yoga Mat"] },

                    new ProductRating { Username = "OutdoorExplorer", Message = "Survived a thunderstorm with zero leaks! Setup takes only 5 minutes. Best tent I have owned.", Score = 5, ProductId = productIds["Camping Tent"] },
                    new ProductRating { Username = "WeekendCamper", Message = "Says 4-person but realistically fits 2 adults with gear comfortably. Typical tent sizing issues.", Score = 3, ProductId = productIds["Camping Tent"] },
                    new ProductRating { Username = "HikingPro", Message = "Too heavy for backpacking. Great for car camping though. Quality materials and construction.", Score = 4, ProductId = productIds["Camping Tent"] },

                    new ProductRating { Username = "HomeGym", Message = "Replaces an entire rack of dumbbells. The quick-change system is smooth and reliable. Amazing space saver!", Score = 5, ProductId = productIds["Dumbbells Set"] },
                    new ProductRating { Username = "PowerLifter", Message = "Max weight of 52.5 lbs is not enough for advanced lifters. Perfect for beginners and intermediates.", Score = 3, ProductId = productIds["Dumbbells Set"] },
                    new ProductRating { Username = "FitnessFan", Message = "One of the adjustment dials broke after 6 months. Build quality could be better for the price.", Score = 2, ProductId = productIds["Dumbbells Set"] },

                    new ProductRating { Username = "TrailRider", Message = "Incredible value for the price! Handles rough trails like a champ. The disc brakes are responsive.", Score = 5, ProductId = productIds["Mountain Bike"] },
                    new ProductRating { Username = "BikeEnthusiast", Message = "Front suspension is decent but the rear is rigid which makes bumpy rides uncomfortable.", Score = 3, ProductId = productIds["Mountain Bike"] },
                    new ProductRating { Username = "CommuterLife", Message = "Great for both trail and city riding. Assembly was straightforward. Love the 21-speed gearing.", Score = 4, ProductId = productIds["Mountain Bike"] },
                };

                await context.ProductRatings.AddRangeAsync(ratings);
                await context.SaveChangesAsync();
                logger.LogInformation("DataSeeder: Added {Count} ratings.", ratings.Count);
            }
            else
            {
                logger.LogInformation("DataSeeder: Ratings already exist. Skipping.");
            }

            logger.LogInformation("DataSeeder: Complete.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DataSeeder: An error occurred while seeding the database.");
        }
    }
}
