using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PrimesWebService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPrimesSearcher, PrimesSearcher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogInformation($"Get request with url {context.Request.Path} is called!");

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync("Created by Kirill Babich\n");
                    await context.Response.WriteAsync("This program is a web service for performing simple actions with primes");

                    logger.LogInformation($"Get request with url {context.Request.Path} was performed successfully!");
                    logger.LogInformation($"Get request with url {context.Request.Path} returned Status code: {context.Response.StatusCode}!");
                });
                endpoints.MapGet("/primes", async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogInformation($"Get request with url {context.Request.Path} is called!");
                    var primesSearcher = context.RequestServices.GetRequiredService<IPrimesSearcher>();

                    if (int.TryParse((string)context.Request.Query["from"].FirstOrDefault(), out var from)
                        && int.TryParse((string)context.Request.Query["to"].FirstOrDefault(), out var to))
                    {
                        logger.LogInformation($"Get request with url {context.Request.Path} parameters are valid!");
                        var primes = await primesSearcher.FindPrimesAsync(from, to);
                        var responseString = string.Join(", ", primes);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        await context.Response.WriteAsync($"[{responseString}]");
                        logger.LogInformation($"Get request with url {context.Request.Path} and parameters from: {from}; to: {to} was performed successfully!");
                    }
                    else
                    {
                        logger.LogInformation($"Get request with url {context.Request.Path} parameters are invalid!");
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    logger.LogInformation($"Get request with url {context.Request.Path} returned Status code: {context.Response.StatusCode}!");
                });
                endpoints.MapGet("/primes/{number:int}", context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogInformation($"Get request with url {context.Request.Path} is called!");
                    var primesSearcher = context.RequestServices.GetRequiredService<IPrimesSearcher>();
                    var isPrime = primesSearcher
                    .IsPrime(int.Parse((string)context.Request.RouteValues["number"]));

                    if (isPrime)
                    {
                        logger.LogInformation($"Get request with url {context.Request.Path} was performed successfully!");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }

                    logger.LogInformation($"Get request with url {context.Request.Path} returned Status code: {context.Response.StatusCode}!");
                    return Task.CompletedTask;
                });

            });
        }
    }
}
