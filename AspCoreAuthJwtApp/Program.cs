using AspCoreAuthJwtApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AspCoreAuthJwtApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<User> users = new()
            {
                new(){ Email = "max@mail.com", Password = "qwerty" },
                new(){ Email = "user@mail.ru", Password = "asd123asd" }
            };


            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();
            var authBuilder = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            authBuilder.AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = UserAuthOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = UserAuthOptions.Audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = UserAuthOptions.SecurityKey
                };
            });
                       

            var app = builder.Build();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapGet("/", () => "Hello World!");

            app.MapGet("/info", [Authorize] () => "Secret Info");

            app.MapPost("/login", (User userForm) =>
            {
                User? user = users.FirstOrDefault(u => 
                    u.Email == userForm.Email && u.Password == userForm.Password);

                if (user is null) return Results.Unauthorized();

                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Email),
                };

                JwtSecurityToken token = new(
                    issuer: UserAuthOptions.Issuer,
                    audience: UserAuthOptions.Audience,
                    claims: claims,
                    expires: DateTime.Now.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new Microsoft.IdentityModel
                                                     .Tokens
                                                     .SigningCredentials(UserAuthOptions.SecurityKey, 
                                                                         SecurityAlgorithms.HmacSha256)
                    );
                var securityToken = new JwtSecurityTokenHandler().WriteToken(token);

                var responseToken = new
                {
                    token = securityToken,
                    email = user.Email,
                };

                return Results.Json(responseToken);
            });

            app.Run();
        }
    }
}
