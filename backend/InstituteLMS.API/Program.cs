using System.Text;
using InstituteLMS.API.Data;
using InstituteLMS.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
if (jwtKey.Length < 32) throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => {
 o.TokenValidationParameters = new TokenValidationParameters {
  ValidateIssuer=true, ValidateAudience=true, ValidateLifetime=true, ValidateIssuerSigningKey=true,
  ValidIssuer=builder.Configuration["Jwt:Issuer"], ValidAudience=builder.Configuration["Jwt:Audience"],
  IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), ClockSkew=TimeSpan.FromSeconds(30)
 };
});
builder.Services.AddAuthorization();
builder.Services.AddCors(o=>o.AddPolicy("frontend",p=>p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));
var app=builder.Build();
if(app.Environment.IsDevelopment()){app.UseSwagger();app.UseSwaggerUI();}
app.UseHttpsRedirection(); app.UseCors("frontend"); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); app.Run();
