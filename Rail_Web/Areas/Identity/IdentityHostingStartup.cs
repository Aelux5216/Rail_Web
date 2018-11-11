﻿using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rail_Web.Models;

[assembly: HostingStartup(typeof(Rail_Web.Areas.Identity.IdentityHostingStartup))]
namespace Rail_Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<Rail_WebContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("Rail_WebContextConnection")));

                services.AddDefaultIdentity<IdentityUser>()
                    .AddEntityFrameworkStores<Rail_WebContext>();
            });
        }
    }
}