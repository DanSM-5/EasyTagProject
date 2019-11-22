using EasyTagProject.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public static class SeedDataIdentiy
    {
        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            // Get context object for Identity Database and run the migrations scripts
            ETIdentityDbContext context = app.ApplicationServices.GetRequiredService<ETIdentityDbContext>();
            context.Database.Migrate();

            // Executed is the identity database is empty
            if (!(context.UserRoles.Any() && context.Users.Any()))
            {
                //Admin Data
                string adminUser = "Admin";
                string adminPass = "Secret@123";
                string adminRoleName = nameof(UserRoles.Admin);

                //Manager Data
                string professorUser = "Andre";
                string profPass = "Faculty@123";
                string professorRoleName = nameof(UserRoles.Professor);

                //RoleManager declaration
                RoleManager<IdentityRole> roleManager = app.ApplicationServices
                                                            .GetRequiredService<RoleManager<IdentityRole>>();

                //Admin Role
                IdentityRole adminRole = await roleManager.FindByNameAsync(adminRoleName);

                if (adminRole == null)
                {
                    adminRole = new IdentityRole(adminRoleName);
                    await roleManager.CreateAsync(adminRole);
                }

                //Professor Role
                IdentityRole professorRole = await roleManager.FindByNameAsync(professorRoleName);

                if (professorRole == null)
                {
                    professorRole = new IdentityRole(professorRoleName);
                    await roleManager.CreateAsync(professorRole);
                }

                //UserManager Declaration
                UserManager<EasyTagUser> userManager = app.ApplicationServices
                                                            .GetRequiredService<UserManager<EasyTagUser>>();

                //Admin User
                EasyTagUser admin = await userManager.FindByIdAsync(adminUser);

                if (admin == null)
                {
                    admin = new EasyTagUser(adminUser) 
                    { 
                        Role = UserRoles.Admin,
                        Email = "admin@admin.com",
                        PhoneNumber = "1111111111"
                    };
                    await userManager.CreateAsync(admin, adminPass);
                    await userManager.AddToRoleAsync(admin, adminRoleName);
                }
                else
                {
                    if (!(await userManager.IsInRoleAsync(admin, adminRoleName)))
                    {
                        await userManager.AddToRoleAsync(admin, adminRoleName);
                    }
                }

                //Manager User
                EasyTagUser professor = await userManager.FindByIdAsync(professorUser);

                if (professor == null)
                {
                    professor = new EasyTagUser(professorUser) 
                    { 
                        Role = UserRoles.Professor,
                        Email = "andreTeacher@mail.com",
                        PhoneNumber = "123456790"
                    };
                    await userManager.CreateAsync(professor, profPass);
                    await userManager.AddToRoleAsync(professor, professorRoleName);
                }
                else
                {
                    if (!(await userManager.IsInRoleAsync(professor, professorRoleName)))
                    {
                        await userManager.AddToRoleAsync(professor, professorRoleName);
                    }
                } 
            }
        }
    }
}
