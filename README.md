# Part 11: Add Identity on ASP.NET Core

>Users can create an account with the login information stored in Identity or they can use an external login provider. Supported external login providers include Facebook, Google, Microsoft Account, and Twitter. For help in choosing between minimal APIs and controller-based APIs. For a tutorial on creating a minimal API, see [Tutorial: Create a minimal API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api).

In this section:

- Add Identity
- Validate properties for Entities
- Seed data for identity

Before coming to this guide, please refer to [File Storage](https://github.com/NguyenPhuDuc307/file-storage).

## Add Microsoft.AspNetCore.Identity.EntityFrameworkCore package

For install Identity run the following command:

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

## Add a User implement from IdentityUser

Add a file named `User.cs` to the `Data/Entities` folder, create it in your project's source folder.

Update the `Data/Entities/User.cs` file with the following code:

```c#
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CourseManagement.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(50)]
        [Required]
        public string? FullName { get; set; }
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }
    }
}
```

## Configuration in DbContext

Update `Data/CourseDbContext.cs` with the following code:

```c#
using Microsoft.EntityFrameworkCore;
using CourseManagement.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CourseManagement.Data;

public class CourseDbContext : IdentityDbContext<User>
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>().ToTable("Roles").Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
        modelBuilder.Entity<User>().ToTable("Users").Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
    }

    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Lesson> Lessons { get; set; } = null!;
}
```

Here I configure the `Roles`, `Users`, `UserRoles`, `RoleClaims`, `UserClaims`, `UserLogins`, `UserTokens`.

## Seed the database

Replace the class `DbInitializer.cs` with the following:

```c#
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using CourseManagement.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace CourseManagement.Data
{
    public class DbInitializer
    {
        private readonly CourseDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string AdminRoleName = "Admin";
        private readonly string UserRoleName = "Member";

        public DbInitializer(CourseDbContext context,
          UserManager<User> userManager,
          RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Seed()
        {
            // Seeding role
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = AdminRoleName,
                    NormalizedName = AdminRoleName.ToUpper(),
                });
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = UserRoleName,
                    NormalizedName = UserRoleName.ToUpper(),
                });
            }

            // Seeding user
            if (!_userManager.Users.Any())
            {
                var result = await _userManager.CreateAsync(new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "example.com",
                    FullName = "Example",
                    Email = "example.com",
                    LockoutEnabled = false,
                    PhoneNumber = "0987654321",
                    Dob = new DateTime(2000, 01, 01)
                }, "Admin@123");
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync("example.com");
                    if (user != null)
                        await _userManager.AddToRoleAsync(user, AdminRoleName);
                }
            }

            // Seeding course and lesson
            var course1 = new Course
            {
                Title = "ASP.NET Core MVC",
                Topic = ".NET Programming",
                ReleaseDate = DateTime.Today,
                Author = "vnLab"
            };

            var course2 = new Course
            {
                Title = "ASP.NET Core API",
                Topic = ".NET Programming",
                ReleaseDate = DateTime.Today,
                Author = "vnLab"
            };

            var course3 = new Course
            {
                Title = "Java Spring Boot",
                Topic = "Java Programming",
                ReleaseDate = DateTime.Today,
                Author = "vnLab"
            };

            var course4 = new Course
            {
                Title = "Laravel - The PHP Framework",
                Topic = "PHP Programming",
                ReleaseDate = DateTime.Today,
                Author = "vnLab"
            };

            var course5 = new Course
            {
                Title = "Angular Tutorial For Beginner",
                Topic = "Angular Programming",
                ReleaseDate = DateTime.Today,
                Author = "vnLab"
            };

            var lesson1 = new Lesson
            {
                Course = course1,
                Title = "Tutorial: Get started with ASP.NET Core",
                Introduction = "This tutorial shows how to create and run an ASP.NET Core web app using the .NET Core CLI.",
                DateCreated = DateTime.Today
            };

            var lesson2 = new Lesson
            {
                Course = course2,
                Title = "Choose between controller-based APIs and minimal APIs",
                Introduction = "ASP.NET Core supports two approaches to creating APIs: a controller-based approach and minimal APIs. Controllers in an API project are classes that derive from ControllerBase.",
                DateCreated = DateTime.Today
            };

            var lesson3 = new Lesson
            {
                Course = course3,
                Title = "Spring Framework",
                Introduction = "The Spring Framework provides a comprehensive programming and configuration model for modern Java-based enterprise applications - on any kind of deployment platform.",
                DateCreated = DateTime.Today
            };

            var lesson4 = new Lesson
            {
                Course = course4,
                Title = "The PHP Framework for Web Artisans",
                Introduction = "Laravel is a web application framework with expressive, elegant syntax. We’ve already laid the foundation — freeing you to create without sweating the small things.",
                DateCreated = DateTime.Today
            };

            var lesson5 = new Lesson
            {
                Course = course5,
                Title = "Getting started with standalone components",
                Introduction = "Standalone components provide a simplified way to build Angular applications. Standalone components, directives, and pipes aim to streamline the authoring experience by reducing the need for NgModules.",
                DateCreated = DateTime.Today
            };

            if (!_context.Courses.Any())
            {
                _context.Courses.AddRange(course1, course2, course3, course4, course5);
                await _context.SaveChangesAsync();
            }

            if (!_context.Lessons.Any())
            {
                _context.Lessons.AddRange(lesson1, lesson2, lesson3, lesson4, lesson5);
                await _context.SaveChangesAsync();
            }
        }
    }
}
```

## Register services for DI

For register services for DI, let's add the following code in `Program.cs`:

```c#
//Setup identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<CourseDbContext>()
    .AddDefaultTokenProviders();
```

```c#
//DbInitializer
builder.Services.AddTransient<DbInitializer>();
```

```c#
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    db.Database.Migrate();
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Seeding data...");
        var dbInitializer = serviceProvider.GetService<DbInitializer>();
        if (dbInitializer != null)
            dbInitializer.Seed()
                         .Wait();
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
```

Refer to the [Introduction to Identity on ASP.NET Core]([https://github.com/NguyenPhuDuc307/web-api](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio)https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity) by `Microsoft`.

Next let's [HttpClient](https://github.com/NguyenPhuDuc307/http-client).
