using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using One.Database.Users.DbContext;
using Microsoft.Extensions.Configuration;

namespace One.Database.Users
{
    public static class Config
    {
        public static IServiceCollection Initialize(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddDbContext<UserDbContext>(
                x => x.UseSqlServer(configuration.GetConnectionString("UserDbContext")));
           
            return services;
        }

    }
}
