using InstituteLMS.API.Data; using InstituteLMS.API.Models; using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using System.Security.Claims;
namespace InstituteLMS.API.Controllers;
[ApiController,Route("api/[controller]"),Authorize(Roles="BusinessHead","Manager")]
public class TrainersController(ApplicationDbContext db):ControllerBase {
 IQueryable<Trainer> Query() {
  var q=db.Set<Trainer>().AsQueryable(); var c=User.FindFirstValue("BranchId");
  if(int.TryParse(c,out var bid) && typeof(Trainer).GetProperty("BranchId")!=null)
   q=q.Where(x=>EF.Property<int>(x,"BranchId")==bid);
  return q;
 }
 [HttpGet] public async Task<IActionResult> GetAll()=>Ok(await Query().Take(500).ToListAsync());
 [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id) { var x=await Query().FirstOrDefaultAsync(e=>EF.Property<int>(e,"TrainerId")==id); return x is null?NotFound():Ok(x); }
 [HttpPost] public async Task<IActionResult> Create(Trainer item) {
  if(typeof(Trainer).GetProperty("BranchId")!=null && int.TryParse(User.FindFirstValue("BranchId"),out var bid)) typeof(Trainer).GetProperty("BranchId")!.SetValue(item,bid);
  db.Set<Trainer>().Add(item); await db.SaveChangesAsync(); return Ok(item);
 }
 [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,Trainer item) {
  var old=await Query().FirstOrDefaultAsync(e=>EF.Property<int>(e,"TrainerId")==id); if(old is null)return NotFound();
  db.Entry(old).CurrentValues.SetValues(item); typeof(Trainer).GetProperty("TrainerId")!.SetValue(old,id); await db.SaveChangesAsync(); return NoContent();
 }
 [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) {
  var old=await Query().FirstOrDefaultAsync(e=>EF.Property<int>(e,"TrainerId")==id); if(old is null)return NotFound(); db.Remove(old); await db.SaveChangesAsync(); return NoContent();
 }
}
