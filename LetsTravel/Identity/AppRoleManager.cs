using System;
using LetsTravel.Identity.Models;
using LetsTravel.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace LetsTravel.Identity
{
    public class AppRoleManager : RoleManager<AppRole>, IDisposable
    {
        public AppRoleManager(RoleStore<AppRole> store)
        : base(store)
        {
        }
        public static AppRoleManager Create(
        IdentityFactoryOptions<AppRoleManager> options,
        IOwinContext context)
        {
            return new AppRoleManager(new
            RoleStore<AppRole>(context.Get<AppIdentityDbContext>()));
        }
    }
}