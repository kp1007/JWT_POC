using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using JwtPoc.Core.Entities;
using JwtPoc.Core.Interfaces;

namespace JwtPoc.Infrastructure.Data;

/// <summary>
/// Database seeder for initial data
/// Creates default roles, permissions, and test users
/// Demonstrates hierarchical role structure and permission assignments
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // Ensure database is created
        await context.Database.MigrateAsync();

        // Check if already seeded
        if (await context.Users.AnyAsync())
        {
            return; // Database already seeded
        }

        // Seed Permissions
        var permissions = await SeedPermissions(context);

        // Seed Roles
        var roles = await SeedRoles(context);

        // Assign Permissions to Roles
        await AssignPermissionsToRoles(context, roles, permissions);

        // Seed Users
        await SeedUsers(context, passwordHasher, roles);

        await context.SaveChangesAsync();
    }

    private static async Task<Dictionary<string, Permission>> SeedPermissions(ApplicationDbContext context)
    {
        var permissions = new List<Permission>
        {
            // User Management Permissions
            new Permission
            {
                Name = "users.read",
                NormalizedName = "USERS.READ",
                Description = "View user information",
                Category = "User Management",
                IsActive = true
            },
            new Permission
            {
                Name = "users.create",
                NormalizedName = "USERS.CREATE",
                Description = "Create new users",
                Category = "User Management",
                IsActive = true
            },
            new Permission
            {
                Name = "users.update",
                NormalizedName = "USERS.UPDATE",
                Description = "Update user information",
                Category = "User Management",
                IsActive = true
            },
            new Permission
            {
                Name = "users.delete",
                NormalizedName = "USERS.DELETE",
                Description = "Delete users",
                Category = "User Management",
                IsActive = true
            },

            // Role Management Permissions
            new Permission
            {
                Name = "roles.read",
                NormalizedName = "ROLES.READ",
                Description = "View role information",
                Category = "Role Management",
                IsActive = true
            },
            new Permission
            {
                Name = "roles.create",
                NormalizedName = "ROLES.CREATE",
                Description = "Create new roles",
                Category = "Role Management",
                IsActive = true
            },
            new Permission
            {
                Name = "roles.update",
                NormalizedName = "ROLES.UPDATE",
                Description = "Update role information",
                Category = "Role Management",
                IsActive = true
            },
            new Permission
            {
                Name = "roles.delete",
                NormalizedName = "ROLES.DELETE",
                Description = "Delete roles",
                Category = "Role Management",
                IsActive = true
            },

            // Permission Management
            new Permission
            {
                Name = "permissions.read",
                NormalizedName = "PERMISSIONS.READ",
                Description = "View permissions",
                Category = "Permission Management",
                IsActive = true
            },
            new Permission
            {
                Name = "permissions.manage",
                NormalizedName = "PERMISSIONS.MANAGE",
                Description = "Manage permission assignments",
                Category = "Permission Management",
                IsActive = true
            },

            // Audit Log Permissions
            new Permission
            {
                Name = "auditlogs.read",
                NormalizedName = "AUDITLOGS.READ",
                Description = "View audit logs",
                Category = "Audit",
                IsActive = true
            },

            // Profile Management
            new Permission
            {
                Name = "profile.read",
                NormalizedName = "PROFILE.READ",
                Description = "View own profile",
                Category = "Profile",
                IsActive = true
            },
            new Permission
            {
                Name = "profile.update",
                NormalizedName = "PROFILE.UPDATE",
                Description = "Update own profile",
                Category = "Profile",
                IsActive = true
            },

            // Reports (example resource)
            new Permission
            {
                Name = "reports.read",
                NormalizedName = "REPORTS.READ",
                Description = "View reports",
                Category = "Reports",
                IsActive = true
            },
            new Permission
            {
                Name = "reports.create",
                NormalizedName = "REPORTS.CREATE",
                Description = "Create reports",
                Category = "Reports",
                IsActive = true
            }
        };

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        return permissions.ToDictionary(p => p.Name, p => p);
    }

    private static async Task<Dictionary<string, Role>> SeedRoles(ApplicationDbContext context)
    {
        var roles = new List<Role>
        {
            new Role
            {
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                Description = "System administrator with full access",
                Level = 1000,
                IsSystemRole = true,
                IsActive = true
            },
            new Role
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Administrator with management access",
                Level = 100,
                IsSystemRole = true,
                IsActive = true
            },
            new Role
            {
                Name = "Manager",
                NormalizedName = "MANAGER",
                Description = "Manager with elevated privileges",
                Level = 50,
                IsSystemRole = true,
                IsActive = true
            },
            new Role
            {
                Name = "User",
                NormalizedName = "USER",
                Description = "Standard user with basic access",
                Level = 1,
                IsSystemRole = true,
                IsActive = true
            },
            new Role
            {
                Name = "Guest",
                NormalizedName = "GUEST",
                Description = "Guest user with read-only access",
                Level = 0,
                IsSystemRole = true,
                IsActive = true
            }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        return roles.ToDictionary(r => r.Name, r => r);
    }

    private static async Task AssignPermissionsToRoles(
        ApplicationDbContext context,
        Dictionary<string, Role> roles,
        Dictionary<string, Permission> permissions)
    {
        var rolePermissions = new List<RolePermission>();

        // SuperAdmin - All permissions
        foreach (var permission in permissions.Values)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = roles["SuperAdmin"].Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                IsActive = true
            });
        }

        // Admin - All except super admin specific ones
        var adminPermissions = new[]
        {
            "users.read", "users.create", "users.update", "users.delete",
            "roles.read", "roles.create", "roles.update",
            "permissions.read", "permissions.manage",
            "auditlogs.read",
            "profile.read", "profile.update",
            "reports.read", "reports.create"
        };

        foreach (var permName in adminPermissions)
        {
            if (permissions.ContainsKey(permName))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = roles["Admin"].Id,
                    PermissionId = permissions[permName].Id,
                    GrantedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
        }

        // Manager - User and report management
        var managerPermissions = new[]
        {
            "users.read", "users.create", "users.update",
            "roles.read",
            "profile.read", "profile.update",
            "reports.read", "reports.create"
        };

        foreach (var permName in managerPermissions)
        {
            if (permissions.ContainsKey(permName))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = roles["Manager"].Id,
                    PermissionId = permissions[permName].Id,
                    GrantedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
        }

        // User - Basic permissions
        var userPermissions = new[]
        {
            "profile.read", "profile.update",
            "reports.read"
        };

        foreach (var permName in userPermissions)
        {
            if (permissions.ContainsKey(permName))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = roles["User"].Id,
                    PermissionId = permissions[permName].Id,
                    GrantedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
        }

        // Guest - Read only
        var guestPermissions = new[] { "profile.read" };

        foreach (var permName in guestPermissions)
        {
            if (permissions.ContainsKey(permName))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = roles["Guest"].Id,
                    PermissionId = permissions[permName].Id,
                    GrantedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
        }

        await context.RolePermissions.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsers(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        Dictionary<string, Role> roles)
    {
        var users = new List<User>
        {
            new User
            {
                Username = "superadmin",
                Email = "superadmin@example.com",
                PasswordHash = passwordHasher.HashPassword("SuperAdmin@123"),
                FirstName = "Super",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString()
            },
            new User
            {
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = passwordHasher.HashPassword("Admin@123"),
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString()
            },
            new User
            {
                Username = "manager",
                Email = "manager@example.com",
                PasswordHash = passwordHasher.HashPassword("Manager@123"),
                FirstName = "Manager",
                LastName = "User",
                EmailConfirmed = true,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString()
            },
            new User
            {
                Username = "user",
                Email = "user@example.com",
                PasswordHash = passwordHasher.HashPassword("User@123"),
                FirstName = "Standard",
                LastName = "User",
                EmailConfirmed = true,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString()
            },
            new User
            {
                Username = "guest",
                Email = "guest@example.com",
                PasswordHash = passwordHasher.HashPassword("Guest@123"),
                FirstName = "Guest",
                LastName = "User",
                EmailConfirmed = true,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString()
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Assign roles to users
        var userRoles = new List<UserRole>
        {
            new UserRole
            {
                UserId = users[0].Id,
                RoleId = roles["SuperAdmin"].Id,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            },
            new UserRole
            {
                UserId = users[1].Id,
                RoleId = roles["Admin"].Id,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            },
            new UserRole
            {
                UserId = users[2].Id,
                RoleId = roles["Manager"].Id,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            },
            new UserRole
            {
                UserId = users[3].Id,
                RoleId = roles["User"].Id,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            },
            new UserRole
            {
                UserId = users[4].Id,
                RoleId = roles["Guest"].Id,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        await context.UserRoles.AddRangeAsync(userRoles);
        await context.SaveChangesAsync();
    }
}
