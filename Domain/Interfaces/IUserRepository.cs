using AspNetControllerApiBoilerPlate.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;

namespace AspNetControllerApiBoilerPlate.Domain.Interfaces;

public interface IUserRepository
{
    public Task<IdentityResult> CreateAsync(User user, string password);
    public Task<string> GenerateEmailConfirmationTokenAsync(User user);
    public Task<User?> FindByIdAsync(string userId);
    public Task<User?> FindByEmailAsync(string email);
    public Task<IdentityResult> ConfirmEmailAsync(User user, string code);
    public Task<string> GenerateTwoFactorTokenAsync(User user, string tokenProvider);
    public Task<bool> VerifyTwoFactorTokenAsync(User user, string tokenProvider, string tokenToVerify);
}