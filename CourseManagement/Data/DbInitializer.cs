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
                    UserName = "example@gmail.com",
                    FullName = "Example",
                    Email = "example@gmail.com",
                    LockoutEnabled = false,
                    PhoneNumber = "0987654321",
                    Dob = new DateTime(2000, 01, 01)
                }, "Admin@123");
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync("example@gmail.com");
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