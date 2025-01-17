using Microsoft.AspNetCore.Components;
using MoneyBanks.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Components.Pages
{
    public partial class DebtLoan: ComponentBase
    {
        private decimal DebtAmount;
        private string DebtSource;
        private int? SelectedTagId;
        private string DebtDescription;
        private DateTime SelectedDueDate = DateTime.Now.AddDays(30);

        private string SuccessMessage;
        private string ErrorMessage;

        private List<Transaction> transactions = new();
        private List<Tag> Tags = new();
        private List<Debt> debtss = new();

        private List<Debt> UnclearedDebts => debtss
            .Where(d => d.IsPending && (d.DebtsAmount - d.AmountCleared) > 0)
            .ToList();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var dataStore = await DataStoreService.LoadDataAsync();

                transactions = dataStore?.Transactions?.ToList() ?? new List<Transaction>();
                Tags = dataStore?.Tagss?.ToList() ?? new List<Tag>();
                debtss = dataStore?.debts?.ToList() ?? new List<Debt>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }
        }

        private async Task HandleAddDebt()
        {
            try
            {
                var currentUser = AuthenticationService.GetCurrentUser();

                if (currentUser == null && string.IsNullOrEmpty(currentUser.UserName))
                {
                    throw new Exception("User not logged in.");
                }

                if (DebtAmount <= 0 || string.IsNullOrWhiteSpace(DebtDescription))
                {
                    ErrorMessage = "Invalid debt amount or description.";
                    return;
                }

                var tagName = Tags.FirstOrDefault(tag => tag.TagId == SelectedTagId)?.TagName ?? "Uncategorized";

                var newDebt = new Debt
                {
                    DebtId = debtss.Count + 1,
                    DebtsAmount = DebtAmount,
                    DeptSource = DebtSource,
                    TagName = tagName,
                    AmountCleared = 0,
                    IsPending = true,
                    DueDate = SelectedDueDate,
                    DeptDescription = DebtDescription,
                    DebtDate = DateTime.Now,
                    UserName = currentUser.UserName,
                };

                debtss.Add(newDebt);

                var newTransaction = new Transaction
                {
                    TransactionId = transactions.Count + 1,
                    Amount = DebtAmount,
                    Date = DateTime.Now,
                    TranactionTitle = DebtDescription,
                    TransactionLabel = "debt",
                    UserName = currentUser.UserName,
                };

                transactions.Add(newTransaction);

                var currentData = await DataStoreService.LoadDataAsync();
                currentData.debts = debtss;
                currentData.Transactions = transactions;
                await DataStoreService.SaveDataAsync(currentData);

                SuccessMessage = "Debt added successfully!";
                ResetForm();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ResetForm()
        {
            DebtAmount = 0;
            DebtSource = string.Empty;
            DebtDescription = string.Empty;
            SelectedTagId = null;
            SelectedDueDate = DateTime.Now.AddDays(30);
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;
        }
    }
}
