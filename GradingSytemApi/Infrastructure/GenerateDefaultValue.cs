using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using GradingSytemApi.Common.Constant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using GradingSytemApi.Entities;
using GradingSytemApi.Common.Helpers;

namespace GradingSytemApi.Infrastructure
{
    public static class GenerateDefaultValue
    {
        public static async Task GenerateData(IServiceProvider serviceProvider, IHostEnvironment hostingEnvironment, string apiConectionString)
        {
            try
            {
                await CreateUserRoles(serviceProvider);
                GenerateInitData(hostingEnvironment, apiConectionString);
                await InitRoleModuleMapForSuperAdmin(serviceProvider);
                await InitDefaultRoleAndRoleModule(serviceProvider);
               
            }
            catch(Exception e)
            {

            }
        }

        private static void GenerateInitData(IHostEnvironment hostingEnvironment, string apiConectionString)
        {
            InitModule(hostingEnvironment, apiConectionString);
        }

        private static void InitModule(IHostEnvironment hostingEnvironment, string connectionString)
        {
            try
            {
                ExcuteSQL(ReadFile(INIT_SQL_PATH.INIT_MODULES, hostingEnvironment), connectionString);
            }
            catch(Exception e) { }
        }

        private static string ReadFile(string path, IHostEnvironment hostingEnvironment)
        {
            var fileInfo = hostingEnvironment.ContentRootFileProvider.GetFileInfo(path);

            if(fileInfo.Exists)
            {
                return File.ReadAllText(fileInfo.PhysicalPath);
            }

            return String.Empty;
        }

        private static void ExcuteSQL(string sql, string connectionString)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(sql);
            }
        }

        private static async Task InitDefaultRoleAndRoleModule(IServiceProvider servicePovider)
        {
            await CreateRoles(servicePovider, Settings.DEFAULT_TEACHER_ROLE_NAME);
            await CreateRoles(servicePovider, Settings.DEFAULT_STUDENT_ROLE_NAME);
            //await CreateRoleModuleMap(servicePovider, Settings.DEFAULT_TEACHER_ROLE_NAME);
            //await CreateRoleModuleMap(servicePovider, Settings.DEFAULT_STUDENT_ROLE_NAME);
            await InitRoleModuleMapForStudent(servicePovider);
            await InitRoleModuleMapForTeacher(servicePovider);
        }

        private static async Task CreateRoles(IServiceProvider servicePovider, string roleName)
        {
            var roleManager = servicePovider.GetRequiredService<RoleManager<Role>>();

            var role = await roleManager.FindByNameAsync(roleName);

            if(role == null)
            {
                role = new Role()
                {
                    Name = roleName,
                    Active = true,
                    Deleted = false,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    RoleModuleMaps = new List<RoleModuleMap>(),
                    AccountRoleMaps = new List<AccountRoleMap>()
                };

                await roleManager.CreateAsync(role);
            }
        }

        private static async Task CreateRoleModuleMap(IServiceProvider serviceProvider, string roleName)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var dbContext = serviceProvider.GetRequiredService<ApiDbContext>();

            var role = await roleManager.FindByNameAsync(roleName);

            if(role != null)
            {
                role.RoleModuleMaps = dbContext.Modules.Where(x => !x.Deleted).Select(x => new RoleModuleMap()
                {
                    Module = x,
                    Active = true
                }).ToList();
            }
        }

        private static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Account>>();
            var dbContext = serviceProvider.GetRequiredService<ApiDbContext>();

            string roleName = Settings.DEFAULT_ADMIN_ROLE_NAME;
            var role = await roleManager.FindByNameAsync(roleName);

            if(role == null)
            {
                role = new Role()
                {
                    Name = roleName,
                    Active = true,
                    Deleted = false,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    RoleModuleMaps = new List<RoleModuleMap>(),
                    AccountRoleMaps = new List<AccountRoleMap>()
                };

                await roleManager.CreateAsync(role);
            }

            string email = Settings.DEFAULT_ADMIN_EMAIL;

            var defaultSuperAdmin = await userManager.FindByEmailAsync(email);

            if(defaultSuperAdmin == null)
            {
                Account account = new Account()
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Active = true,
                    FirstName = "Super",
                    LastName = "Admin",
                    Deleted = false,
                    UpdatedDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    TotalScore = 0
                };

                account.AccountRoleMaps.Add(new AccountRoleMap()
                {
                    Role = role,
                    RoleId = role.Id,
                    Account = account
                });

                string password = Settings.DEFAULT_ADMIN_PASSWORD;

                await userManager.CreateAsync(account, password);
                await userManager.AddToRoleAsync(account, roleName);
            }
        }

        private static async Task InitRoleModuleMapForSuperAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var dbContext = serviceProvider.GetRequiredService<ApiDbContext>();

            string roleName = Settings.DEFAULT_ADMIN_ROLE_NAME;
            var role = await roleManager.FindByNameAsync(roleName);


            if (role != null)
            {
                // Init values
                List<RoleModuleMap> map = new List<RoleModuleMap>();
                var allModules = dbContext.Modules.Where(x => !x.Deleted).ToList();

                // Get role existed
                var roleModules = dbContext.RoleModuleMaps.Where(x => !x.Deleted && x.RoleId == role.Id).Select(x => x.Module).ToList();

                allModules.Except(roleModules).ToList().ForEach(x =>
                {
                    map.Add(new RoleModuleMap()
                    {
                        Role = role,
                        Module = x,
                        Active = true
                    });
                });

                dbContext.RoleModuleMaps.AddRange(map);
                dbContext.SaveChanges();
            }
        }

        private static async Task InitRoleModuleMapForStudent(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var dbContext = serviceProvider.GetRequiredService<ApiDbContext>();

            string roleName = Settings.DEFAULT_STUDENT_ROLE_NAME;
            var role = await roleManager.FindByNameAsync(roleName);


            if (role != null)
            {
                // Init values
                List<RoleModuleMap> map = new List<RoleModuleMap>();
                var allModules = dbContext.Modules.Where(x => !x.Deleted).ToList();

                // Get role existed
                var roleModules = dbContext.RoleModuleMaps.Where(x => !x.Deleted && x.RoleId == role.Id).Select(x => x.Module).ToList();

                allModules.Except(roleModules).ToList().ForEach(x =>
                {
                    map.Add(new RoleModuleMap()
                    {
                        Role = role,
                        Module = x,
                        Active = (x.Name == "Account" || x.Name == "Workspace") ? true : false
                    });
                });

                dbContext.RoleModuleMaps.AddRange(map);
                dbContext.SaveChanges();

            }

        }

        private static async Task InitRoleModuleMapForTeacher(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var dbContext = serviceProvider.GetRequiredService<ApiDbContext>();

            string roleName = Settings.DEFAULT_TEACHER_ROLE_NAME;
            var role = await roleManager.FindByNameAsync(roleName);


            if (role != null)
            {
                // Init values
                List<RoleModuleMap> map = new List<RoleModuleMap>();
                var allModules = dbContext.Modules.Where(x => !x.Deleted).ToList();

                // Get role existed
                var roleModules = dbContext.RoleModuleMaps.Where(x => !x.Deleted && x.RoleId == role.Id).Select(x => x.Module).ToList();

                allModules.Except(roleModules).ToList().ForEach(x =>
                {
                    map.Add(new RoleModuleMap()
                    {
                        Role = role,
                        Module = x,
                        Active = (x.Name == "Account" || x.Name == "Workspace" || x.Name == "Course") ? true : false
                    });
                });

                dbContext.RoleModuleMaps.AddRange(map);
                dbContext.SaveChanges();

            }
        }
    }
}
