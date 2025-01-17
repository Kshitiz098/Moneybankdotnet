using Microsoft.AspNetCore.Components;
using MoneyBanks.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Components.Pages
{
    public partial class CashIn : ComponentBase
    {
        private decimal CashInAmount;
        private decimal OriginalCashInAmount; // Store the original cash-in amount
        private string CashInDescription;
        private int? SelectedTagId;
        private string ErrorNotification;
        private List<Inflow> PreviousCredits = new();
        private List<Transaction> TransactionLog = new();
        private List<Tag> AvailableTags = new();
        private List<Debt> PendingDebts = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var dataStore = await DataStoreService.LoadDataAsync();
                PreviousCredits = dataStore?.credits ?? new List<Inflow>();
                TransactionLog = dataStore?.Transactions ?? new List<Transaction>();
                AvailableTags = dataStore?.Tagss ?? new List<Tag>();
                PendingDebts = dataStore?.debts ?? new List<Debt>();
            }
            catch (Exception ex)
            {
                ErrorNotification = $"Error loading data: {ex.Message}";
            }
        }

        private async Task HandleCashIn()
        {
            try
            {
                var currentUser = AuthenticationService.GetCurrentUser();

                if (currentUser == null || string.IsNullOrEmpty(currentUser.UserName))
                {
                    throw new Exception("User not logged in.");
                }

                if (CashInAmount <= 0 || SelectedTagId == null)
                {
                    ErrorNotification = "Please provide a valid amount and select a tag.";
                    return;
                }

                OriginalCashInAmount = CashInAmount;
                var selectedTagName = AvailableTags.FirstOrDefault(tag => tag.TagId == SelectedTagId)?.TagName ?? "Uncategorized";

                // Add a new credit
                var newCredit = new Inflow
                {
                    CashInId = PreviousCredits.Count + 1,
                    CashInAmount = OriginalCashInAmount,
                    CashInDescription = CashInDescription,
                    TagName = selectedTagName,
                    CashInDate = DateTime.Now,
                    UserName = currentUser.UserName
                };

                // Add a new transaction
                var newTransaction = new Transaction
                {
                    TransactionId = TransactionLog.Count + 1,
                    Amount = OriginalCashInAmount,
                    TranactionTitle = CashInDescription,
                    TransactionLabel = "Credit",
                    TagName = selectedTagName,
                    Date = DateTime.Now,
                    UserName = currentUser.UserName
                };

                PreviousCredits.Add(newCredit);
                TransactionLog.Add(newTransaction);

                // Handle pending debts
                decimal totalPendingDebt = PendingDebts.Where(d => d.IsPending).Sum(d => d.DebtsAmount - d.AmountCleared);

                if (totalPendingDebt > 0)
                {
                    foreach (var debt in PendingDebts.Where(d => d.IsPending).OrderBy(d => d.DueDate))
                    {
                        if (CashInAmount >= (debt.DebtsAmount - debt.AmountCleared))
                        {
                            CashInAmount -= (debt.DebtsAmount - debt.AmountCleared);
                            debt.AmountCleared = debt.DebtsAmount;
                            debt.IsPending = false;
                        }
                        else
                        {
                            debt.AmountCleared += CashInAmount;
                            CashInAmount = 0;
                            break;
                        }
                    }
                }

                // Save updates to the datastore
                var dataStore = await DataStoreService.LoadDataAsync();
                if (dataStore != null)
                {
                    dataStore.credits = PreviousCredits;
                    dataStore.Transactions = TransactionLog;
                    dataStore.debts = PendingDebts;
                    await DataStoreService.SaveDataAsync(dataStore);
                }

                // Reset input fields
                CashInAmount = 0;
                CashInDescription = string.Empty;
                SelectedTagId = null;
                ErrorNotification = "";
            }
            catch (Exception ex)
            {
                ErrorNotification = $"Error saving cash-in: {ex.Message}";
            }
        }
    }
}
