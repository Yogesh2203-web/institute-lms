using System.IdentityModel.Tokens.Jwt; using System.Security.Claims; using System.Text;
using InstituteLMS.API.Data; using InstituteLMS.API.DTO; using InstituteLMS.API.Models;
using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Identity; using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; using Microsoft.IdentityModel.Tokens;
namespace InstituteLMS.API.Controllers;
[ApiController,Route("api/[controller]")]
public class AuthController(ApplicationDbContext db,IPasswordHasher<User> hasher,IConfiguration cfg):ControllerBase {
 [AllowAnonymous,HttpPost("register")] public async Task<IActionResult> Register(RegisterRequest r) {
  if(string.IsNullOrWhiteSpace(r.FullName)||string.IsNullOrWhiteSpace(r.Email)||r.Password.Length<10) return BadRequest(new{message="Name/email required; password must be at least 10 characters."});
  var role=r.Role.Trim(); if(role!="Student" && !(role=="BusinessHead" && cfg.GetValue<bool>("AllowPublicBusinessHeadRegistration"))) return BadRequest(new{message="Public registration is limited to Student; BusinessHead setup must be explicitly enabled."});
  var email=r.Email.Trim().ToLowerInvariant(); if(await db.Users.AnyAsync(x=>x.Email==email)) return Conflict(new{message="Email already registered."});
  if(role=="Student" && (r.BranchId is null || !await db.Branches.AnyAsync(x=>x.BranchId==r.BranchId && x.IsActive))) return BadRequest(new{message="A valid active BranchId is required."});
  var u=new User{FullName=r.FullName.Trim(),Email=email,Phone=r.Phone.Trim(),Role=role,BranchId=r.BranchId}; u.PasswordHash=hasher.HashPassword(u,r.Password);
  db.Users.Add(u); await db.SaveChangesAsync(); return Ok(new{u.UserId,u.FullName,u.Email,u.Role,u.BranchId});
 }
 [AllowAnonymous,HttpPost("login")] public async Task<IActionResult> Login(LoginRequest r) {
  var u=await db.Users.SingleOrDefaultAsync(x=>x.Email==r.Email.Trim().ToLower()&&x.IsActive);
  if(u is null || hasher.VerifyHashedPassword(u,u.PasswordHash,r.Password)==PasswordVerificationResult.Failed) return Unauthorized(new{message="Invalid email or password."});
  var claims=new List<Claim>{new(ClaimTypes.NameIdentifier,u.UserId.ToString()),new(ClaimTypes.Name,u.FullName),new(ClaimTypes.Email,u.Email),new(ClaimTypes.Role,u.Role)};
  if(u.BranchId is int bid) claims.Add(new("BranchId",bid.ToString()));
  var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!));
  var jwt=new JwtSecurityToken(cfg["Jwt:Issuer"],cfg["Jwt:Audience"],claims,expires:DateTime.UtcNow.AddMinutes(cfg.GetValue<int>("Jwt:ExpiryMinutes",60)),signingCredentials:new SigningCredentials(key,SecurityAlgorithms.HmacSha256));
  return Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(jwt),u.UserId,u.FullName,u.Email,u.Role,u.BranchId));
 }
 [Authorize,HttpGet("me")] public async Task<IActionResult> Me() {
  if(!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var id)) return Unauthorized();
  var u=await db.Users.Where(x=>x.UserId==id).Select(x=>new{x.UserId,x.FullName,x.Email,x.Role,x.BranchId}).SingleOrDefaultAsync();
  return u is null?NotFound():Ok(u);
 }
}
