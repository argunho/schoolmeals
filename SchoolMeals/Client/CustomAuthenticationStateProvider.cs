using Microsoft.AspNetCore.Components.Authorization;
using SchoolMeals.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SchoolMeals.Client
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _http;

        public CustomAuthenticationStateProvider(
                    HttpClient http
            )
        {
            _http = http;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //var anonymous = new ClaimsIdentity("Aslan");
            //return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
            Users currentUser = await _http.GetFromJsonAsync<Users>("Users/GetCurrent");
            Console.WriteLine("Test01");
            if (currentUser != null && currentUser.Email != null)
            {
                Console.WriteLine("Test02");
                //var roles = await _http.GetFromJsonAsync<string>("Users/GetUserRoles/" + currentUser.Email);

                //var claim = new Claim(ClaimTypes.Name, currentUser.Email);
                var claim = new List<Claim>();
                claim.Add(new Claim("Id", currentUser.Id.ToString()));
                claim.Add(new Claim("SchoolId", currentUser.School.Id.ToString()));
                claim.Add(new Claim(ClaimTypes.Name, currentUser.Email));
                claim.Add(new Claim(ClaimTypes.Email, currentUser.Email));
                //foreach (var r in roles)
                //    claim.Add(new Claim(ClaimTypes.Role, r.ToString()));

                var claimIdentity = new ClaimsIdentity(claim, "serverAuth");
                var claimPrincipial = new ClaimsPrincipal(claimIdentity);
                return new AuthenticationState(claimPrincipial);
            }else
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}
