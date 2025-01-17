using Microsoft.AspNetCore.Components;
using MoneyBanks.Database;
using MoneyBanks.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Components.Pages
{
    public partial class Register : ComponentBase
    {
        private User NewUser = new User();
        private string ErrorMessage;

        private void GoToLoginPage()
        {
            Navigation.NavigateTo("/login");
        }

        private async Task HandleRegister()
        {
            try
            {
                var dataStore = await DataStoreService.LoadDataAsync();
                if (dataStore == null)
                {
                    ErrorMessage = "Failed to load data.";
                    return;
                }

                var users = dataStore.Users?.ToList() ?? new List<User>();

                if (users.Any(u => u.UserName == NewUser.UserName))
                {
                    ErrorMessage = "Username already exists. Please choose another.";
                    return;
                }

                var (hashedPassword, salt) = PasswordHelper.HashPasswordWithSalt(NewUser.UserPassword);
                NewUser.UserId = Guid.NewGuid().ToString();
                NewUser.UserPassword = hashedPassword;
                NewUser.PasswordSalt = salt;
                NewUser.PreferredCurrency = NewUser.PreferredCurrency ?? "USD";

                users.Add(NewUser);
                dataStore.Users = users;

                await DataStoreService.SaveDataAsync(dataStore);

                Navigation.NavigateTo("/login");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error during registration: {ex.Message}";
            }
        }
    }
}
