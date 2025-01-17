using MoneyBanks.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Service
{
    internal class AuthenticationService
    {
        private const string IsAuthenticatedKey = "IsAuthenticated";
        private const string UserNameKey = "UserName";
        private const string UserEmailKey = "UserEmail";
        private const string UserIdKey = "UserId";
        private string _preferredCurrency;

        public void SetPreferredCurrency(string currency)
        {
            _preferredCurrency = currency;
            // Optionally, save it to session storage or localStorage for persistence
            // e.g., localStorage.SetItem("preferredCurrency", currency);
        }

        public string GetPreferredCurrency()
        {
            // Return the stored currency, or default to USD if not set
            return _preferredCurrency ?? "USD";
        }
        public bool IsAuthenticated()
        {
            return Preferences.Get(IsAuthenticatedKey, false);
        }
        /*public bool IsAuthenticated()
        {
            var user = GetCurrentUser();
            return user != null && !string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.UserEmail);
        }*/
        // Method to set user as authenticated
        public void SetAuthenticated(bool isAuthenticated)
        {
            Preferences.Set(IsAuthenticatedKey, isAuthenticated);
        }
        public void SetCurrentUser(string userId, string userName, string userEmail)
        {
            Preferences.Set(UserIdKey, userId);
            Preferences.Set(UserNameKey, userName);
            Preferences.Set(UserEmailKey, userEmail);
            Preferences.Set(IsAuthenticatedKey, true);
        }

        // Method to get current user details
        public User GetCurrentUser()
        {
            var userId = Preferences.Get(UserIdKey, string.Empty);
            var userName = Preferences.Get(UserNameKey, string.Empty);
            var userEmail = Preferences.Get(UserEmailKey, string.Empty);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
            {
                Preferences.Set(IsAuthenticatedKey, false);
            }

            return new User
            {
                UserName = userName,
                UserEmail = userEmail
            };
        }
        public void Logout()
        {
            Preferences.Remove(UserNameKey);
            Preferences.Remove(UserEmailKey);
            Preferences.Remove(UserIdKey);
            Preferences.Set(IsAuthenticatedKey, false);
        }
    }
}
