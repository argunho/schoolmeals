using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoleEnabledWebAssembly.Client;
using SchoolMeals.Client.Helpers;
using SchoolMeals.Client.Intefice;
using SchoolMeals.Client.Repository;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace SchoolMeals.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");            
            

            builder.Services.AddHttpClient("SchoolMeals.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SchoolMeals.ServerAPI"));

            builder.Services.AddApiAuthorization();
            // Call the factory 
            //builder.Services.AddApiAuthorization().AddAccountClaimsPrincipalFactory<RolesClaimsPrincipalFactory>();

            ///  /////
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            //builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            // Configure http client ----------------
            builder.Services.AddScoped(x => new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // File reader -----------------
            builder.Services.AddFileReaderService(options =>
            {
                options.UseWasmSharedBuffer = true;
            });

            // Add repository -------------
            builder.Services.AddTransient<IHelpRepository, HelpRepository>();

            // Add singelton service --------------
            builder.Services.AddSingleton<SingletonService>();



            await builder.Build().RunAsync();
        }
    }
}
