using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MoneyBanks.Database;
using MoneyBanks.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Components.Pages
{
    public partial class Login : ComponentBase
    {
        private User user = new User(); // Temporary user object for form binding
        private string ErrorMessage = string.Empty;
        private void GoToRegisterPage()
        {
            Navigation.NavigateTo("/register"); // Navigate to Register page
        }
        private async Task HandleLogin()
        {
            try
            {
                if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.UserPassword))
                {
                    ErrorMessage = "Please fill out all fields.";
                    return;
                }

                // Load existing users from data store
                var dataStore = await DataStoreService.LoadDataAsync();
                var userInDb = dataStore.Users.FirstOrDefault(u => u.UserName == user.UserName);

                if (userInDb == null)
                {
                    ErrorMessage = "User not found.";
                    return;
                }

                // Validate password
                var isPasswordValid = PasswordHelper.VerifyPassword(user.UserPassword, userInDb.UserPassword, userInDb.PasswordSalt);
                if (!isPasswordValid)
                {
                    ErrorMessage = "Invalid password.";
                    return;
                }

                // Set authentication and save user state
                AuthenticationService.SetAuthenticated(true);
                AuthenticationService.SetCurrentUser(userInDb.UserId, userInDb.UserName, userInDb.UserEmail);
                AuthenticationService.SetPreferredCurrency(user.PreferredCurrency);

                // Save to session storage
                await JSRuntime.InvokeVoidAsync("sessionStorage.setItem", "preferredCurrency", user.PreferredCurrency);

                ErrorMessage = "Login successful! Redirecting...";
                await Task.Delay(1000); // Delay for user feedback
                Navigation.NavigateTo("/dashboard");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }
    }
}
