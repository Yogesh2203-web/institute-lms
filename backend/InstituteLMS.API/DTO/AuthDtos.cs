namespace InstituteLMS.API.DTO;
public record RegisterRequest(string FullName,string Email,string Password,string Phone,string Role,int? BranchId);
public record LoginRequest(string Email,string Password);
public record LoginResponse(string Token,int UserId,string FullName,string Email,string Role,int? BranchId);
