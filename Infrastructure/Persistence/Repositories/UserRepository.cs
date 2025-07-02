using AspNetControllerApiBoilerPlate.Domain.Interfaces;
using AspNetControllerApiBoilerPlate.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;

namespace AspNetControllerApiBoilerPlate.Infrastructure.Persistence.Repositories;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    public async Task<IdentityResult> CreateAsync(User user, string password)
    {
        return await userManager.CreateAsync(user, password);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        return await userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<User?> FindByIdAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(User user, string code)
    {
        return await userManager.ConfirmEmailAsync(user, code);
    }

    public async Task<string> GenerateTwoFactorTokenAsync(User user, string tokenProvider)
    {
        return await userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
    }

    public async Task<bool> VerifyTwoFactorTokenAsync(User user, string tokenProvider, string tokenToVerify)
    {
        return await userManager.VerifyTwoFactorTokenAsync(user, tokenProvider, tokenToVerify);
    }
}