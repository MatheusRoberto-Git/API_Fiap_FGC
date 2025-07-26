using FGC.Domain.UserManagement.Entities;
using System.Security.Claims;

namespace FGC.Domain.UserManagement.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);

        ClaimsPrincipal ValidateToken(string token);

        Guid? GetUserIdFromToken(string token);
    }
}
