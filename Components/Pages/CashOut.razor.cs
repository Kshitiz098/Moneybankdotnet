using Microsoft.AspNetCore.Components;
using MoneyBanks.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Components.Pages
{
    public partial class CashOut: ComponentBase
    {
        private decimal CashOutAmount;
        private string CashOutTitle;
        private string CashOutErrorMessage = string.Empty;
        private List<Transaction> CashOutTransactions = new();
        private List<Inflow> CashOutCredits = new();
        private List<OutFlow> CashOutDebits = new();
        private List<Debt> CashOutDebts = new();
        private List<Tag> CashOutTags = new();
        private int? SelectedCashOutTagId;

        private decimal CashOutInflows;
        private decimal CashOutOutflows;
        private decimal CashOutBalance;
        private decimal CashOutPendingDebts;

        protected override async Task OnInitializedAsync()
        {
            await LoadCashOutDataAsync();
            UpdateCashOutSummary();
        }

        private async Task LoadCashOutDataAsync()
        {
            try
            {
                var dataStore = await CashOutDataService.LoadDataAsync();
                if (dataStore != null)
                {
                    CashOutTransactions = dataStore.Transactions?.ToList() ?? new();
                    CashOutCredits = dataStore.credits?.ToList() ?? new();
                    CashOutDebits = dataStore.debits?.ToList() ?? new();
                    CashOutDebts = dataStore.debts?.ToList() ?? new();
                    CashOutTags = dataStore.Tagss?.ToList() ?? new();
                }
            }
            catch (Exception ex)
            {
                CashOutErrorMessage = $"Error loading data: {ex.Message}";
            }
        }

        private void UpdateCashOutSummary()
        {
            CashOutInflows = CashOutCredits.Sum(c => c.CashInAmount);
            CashOutOutflows = CashOutDebits.Sum(d => d.CashOutAmount);
            CashOutPendingDebts = CashOutDebts.Where(d => d.IsPending).Sum(d => d.DebtsAmount - d.AmountCleared);
            CashOutBalance = CashOutInflows + CashOutPendingDebts - CashOutOutflows;
        }

        private async Task ProcessCashOut()
        {
            try
            {
                CashOutErrorMessage = string.Empty;

                if (CashOutAmount <= 0)
                {
                    CashOutErrorMessage = "Amount must be greater than zero.";
                    return;
                }

                UpdateCashOutSummary();

                if (CashOutBalance < CashOutAmount)
                {
                    CashOutErrorMessage = "Insufficient balance for cash-out.";
                    return;
                }

                var selectedTag = CashOutTags.FirstOrDefault(tag => tag.TagId == SelectedCashOutTagId);
                string tagName = selectedTag?.TagName ?? "Uncategorized";

                AddCashOutTransaction("Debit", CashOutAmount, CashOutTitle, tagName);
                AddCashOutDebit(CashOutAmount, CashOutTitle);

                await SaveCashOutDataAsync();
                UpdateCashOutSummary();

                CashOutAmount = 0;
                CashOutTitle = string.Empty;
                SelectedCashOutTagId = null;
            }
            catch (Exception ex)
            {
                CashOutErrorMessage = $"Error processing cash-out: {ex.Message}";
            }
        }

        private void AddCashOutTransaction(string label, decimal amount, string title, string tagName)
        {
            var currentUser = AuthenticationService.GetCurrentUser();

            if (currentUser == null && string.IsNullOrEmpty(currentUser.UserName))
            {
                throw new Exception("User not logged in.");
            }
            var userName = currentUser.UserName;

            if (CashOutAmount <= 0 || SelectedCashOutTagId == null)
            {
                CashOutErrorMessage = "Please provide a valid amount and select a tag.";
                return;
            }

            CashOutTransactions.Add(new Transaction
            {
                TransactionId = CashOutTransactions.Count + 1,
                TransactionLabel = label,
                Amount = amount,
                TranactionTitle = title,
                TagName = tagName,
                Date = DateTime.Now,
                UserName = userName,
            });
        }

        private void AddCashOutDebit(decimal amount, string title)
        {
            var currentUser = AuthenticationService.GetCurrentUser();

            if (currentUser == null && string.IsNullOrEmpty(currentUser.UserName))
            {
                throw new Exception("User not logged in.");
            }
            var userName = currentUser.UserName;

            CashOutDebits.Add(new OutFlow
            {
                CashOutId = CashOutDebits.Count + 1,
                CashOutAmount = amount,
                CashOutDescription = title,
                CashOutDate = DateTime.Now,
                UserName = userName,
            });
        }

        private async Task SaveCashOutDataAsync()
        {
            var dataStore = await CashOutDataService.LoadDataAsync();
            if (dataStore != null)
            {
                dataStore.debits = CashOutDebits;
                dataStore.Transactions = CashOutTransactions;
                await CashOutDataService.SaveDataAsync(dataStore);
            }
        }
    }
}
