using InstituteLMS.API.Data; using InstituteLMS.API.Models; using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using System.Security.Claims;
namespace InstituteLMS.API.Controllers;
[ApiController,Route("api/[controller]"),Authorize(Roles="BusinessHead","Manager")]
public class FeePaymentsController(ApplicationDbContext db):ControllerBase {
 IQueryable<FeePayment> Query() {
  var q=db.Set<FeePayment>().AsQueryable(); var c=User.FindFirstValue("BranchId");
  if(int.TryParse(c,out var bid) && typeof(FeePayment).GetProperty("BranchId")!=null)
   q=q.Where(x=>EF.Property<int>(x,"BranchId")==bid);
  return q;
 }
 [HttpGet] public async Task<IActionResult> GetAll()=>Ok(await Query().Take(500).ToListAsync());
 [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id) { var x=await Query().FirstOrDefaultAsync(e=>EF.Property<int>(e,"PaymentId")==id); return x is null?NotFound():Ok(x); }
 [HttpPost] public async Task<IActionResult> Create(FeePayment item) {
  if(typeof(FeePayment).GetProperty("BranchId")!=null && int.TryParse(User.FindFirstValue("BranchId"),out var bid)) typeof(FeePayment).GetProperty("BranchId")!.SetValue(item,bid);
  db.Set<FeePayment>().Add(item); await db.SaveChangesAsync(); return Ok(item);
 }
 [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,FeePayment item) {
  var old=await Query().FirstOrDefaultAsync(e=>EF.Property<int>(e,"PaymentId")==id); if(old is null)return NotFound();
  db.Entry(old).CurrentValues.SetValues(item); typeof(FeePayment).GetProperty("PaymentId")!.SetValue(old,id); await db.SaveChangesAsync(); return NoContent();
 }
 [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) {
  var old=await Query().FirstOrDefaultAsync(e=>EF.Property<int>(e,"PaymentId")==id); if(old is null)return NotFound(); db.Remove(old); await db.SaveChangesAsync(); return NoContent();
 }
}
