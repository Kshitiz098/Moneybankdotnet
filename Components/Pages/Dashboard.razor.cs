using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MoneyBanks.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoneyBanks.Components.Pages
{
    public partial class Dashboard: ComponentBase
    {
        private int TotalTransactions;
        private decimal TotalInflows;
        private decimal TotalOutflows;
        private decimal Balance;
        private decimal TotalDebts;
        private int ClearedDebtsCount;
        private int PendingDebtsCount;
        private decimal PendingDebts;
        private decimal ClearedDebts;
        private decimal TotalTransactionAmount;

        private List<Transaction> Transactions = new();
        private List<Transaction> FilteredTransactions = new();
        private List<string> tags = new();
        private List<Debt> debtss = new();

        private DateTime? StartDate;
        private DateTime? EndDate;
        private DateTime? FilterDate;
        private DateTime? DebtStartDate;
        private DateTime? DebtEndDate;
        private string SearchTitle = "";
        private string errorMessage = null;

        private string SelectedType = "";
        private string SelectedOrder = "";

        private List<Debt> FilteredDebts = new();

        private IBrowserFile file;
        private bool importSuccess = false;


        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            file = e.File;
        }

        // Import Data logic
        private async Task ImportData()
        {
            if (file != null)
            {
                try
                {
                    // Read the file content
                    var fileContent = await ReadFileAsync(file);
                    // Deserialize file content (assuming JSON)
                    var transactions = JsonSerializer.Deserialize<List<Transaction>>(fileContent);
                    if (transactions != null)
                    {
                        Transactions.AddRange(transactions);
                        await SaveImportedData();
                        importSuccess = true;  // Set success flag to true
                        errorMessage = null;   // Clear any previous error message
                    }
                    else
                    {
                        importSuccess = false; // Set success flag to false
                        errorMessage = "Failed to parse the file data.";
                    }
                }
                catch (Exception ex)
                {
                    importSuccess = false; // Set success flag to false
                    errorMessage = $"Error importing data: {ex.Message}";
                }
            }
            else
            {
                importSuccess = false; // Set success flag to false
                errorMessage = "Please select a file first.";
            }
        }

        // Helper function to read file content as string
        private async Task<string> ReadFileAsync(IBrowserFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }

        // Save the imported data into your data store
        private async Task SaveImportedData()
        {
            try
            {
                // Assume DataStoreService is implemented and saves data
                var dataStore = await DataStoreService.LoadDataAsync();
                if (dataStore != null)
                {
                    dataStore.Transactions.AddRange(Transactions);
                    await DataStoreService.SaveDataAsync(dataStore);
                }
                else
                {
                    importSuccess = false; // Set success flag to false
                    errorMessage = "Failed to load the data store.";
                }
            }
            catch (Exception ex)
            {
                importSuccess = false; // Set success flag to false
                errorMessage = $"Failed to save the imported data: {ex.Message}";
            }
        }
        private void ToggleEditNoteMode(Transaction transaction)
        {
            // When you toggle the note editor, reset other rows to not be in editing mode
            foreach (var trans in FilteredTransactions)
            {
                if (trans != transaction)
                {
                    trans.IsEditingNote = false;
                }
            }
            transaction.IsEditingNote = !transaction.IsEditingNote;
        }

        private async Task SaveNote(Transaction transaction)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transaction.Note))
                {
                    errorMessage = "Note cannot be empty.";
                    return;
                }

                // Update the note for the transaction in memory
                var transactionToUpdate = FilteredTransactions.FirstOrDefault(t => t.TransactionId == transaction.TransactionId);
                if (transactionToUpdate != null)
                {
                    transactionToUpdate.Note = transaction.Note;
                    transactionToUpdate.IsEditingNote = false;

                    // Now persist the changes permanently by saving to the backend or file system
                    var dataStore = await DataStoreService.LoadDataAsync();
                    if (dataStore != null)
                    {
                        // Find the corresponding transaction in the data store and update it
                        var originalTransaction = dataStore.Transactions.FirstOrDefault(t => t.TransactionId == transaction.TransactionId);
                        if (originalTransaction != null)
                        {
                            originalTransaction.Note = transaction.Note;
                        }
                        else
                        {
                            errorMessage = "Transaction not found in data store.";
                            return;
                        }

                        // Save updated data back to the data store or file
                        await DataStoreService.SaveDataAsync(dataStore);

                        errorMessage = null; // Clear any previous error message
                    }
                    else
                    {
                        errorMessage = "Failed to load data store.";
                    }
                }
                else
                {
                    errorMessage = "Transaction not found.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to save the note: {ex.Message}";
            }
        }



        private void CancelEdit(Transaction transaction)
        {
            // Cancel the edit (reset to original value)
            transaction.IsEditingNote = false;
        }


        protected override async Task OnInitializedAsync()
        {
            var dataStore = await DataStoreService.LoadDataAsync();
            Transactions = dataStore?.Transactions ?? new List<Transaction>();
            debtss = dataStore?.debts ?? new List<Debt>();

            tags = dataStore?.Tagss.Select(t => t.TagName).Distinct().ToList();
            UpdateSummary();

            // Default filtering to current month
            var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);
            StartDate = currentMonthStart;
            EndDate = currentMonthEnd;
            FilteredTransactions = new List<Transaction>(Transactions);
            FilteredDebts = debtss.Where(d => d.IsPending).ToList();

            FilterTransactionsByDateRange();




        }



        private void UpdateSummary()
        {
            TotalTransactions = Transactions.Count;
            TotalInflows = Transactions.Where(t => t.TransactionLabel == "Credit").Sum(t => t.Amount);
            TotalOutflows = Transactions.Where(t => t.TransactionLabel == "Debit").Sum(t => t.Amount);
            TotalDebts = Transactions.Where(t => t.TransactionLabel == "debt").Sum(t => t.Amount);

            // Update cleared and pending debts
            ClearedDebtsCount = debtss.Count(d => !d.IsPending && (d.DebtsAmount - d.AmountCleared) == 0);
            PendingDebtsCount = debtss.Count(d => d.IsPending && (d.DebtsAmount - d.AmountCleared) > 0);
            PendingDebts = debtss.Where(d => d.IsPending).Sum(d => d.DebtsAmount - d.AmountCleared);
            ClearedDebts = debtss.Sum(d => d.AmountCleared);

            TotalTransactionAmount = TotalInflows + TotalOutflows + TotalDebts;
            // Update balance
            Balance = TotalInflows - TotalOutflows + PendingDebts;

            // Initialize filtered debts
            FilteredDebts = debtss.Where(d => d.IsPending).ToList();
            FilteredTransactions = new List<Transaction>(Transactions);
        }

        private void FilterPendingDebtsByDateRange()
        {
            if (DebtStartDate.HasValue && DebtEndDate.HasValue && DebtEndDate.Value >= DebtStartDate.Value)
            {
                // Apply date range filter
                FilteredDebts = debtss
                    .Where(d => d.IsPending && d.DebtDate >= DebtStartDate.Value && d.DebtDate <= DebtEndDate.Value)
                    .OrderBy(d => d.DueDate)
                    .ToList();
            }
            else
            {
                // Reset to all pending debts if date range is invalid or not provided
                FilteredDebts = debtss.Where(d => d.IsPending).OrderBy(d => d.DueDate).ToList();
            }
        }

        private void FilterTransactionsByTitle()
        {
            if (!string.IsNullOrWhiteSpace(SearchTitle))
            {
                FilteredTransactions = Transactions
                    .Where(t => !string.IsNullOrEmpty(t.TranactionTitle) &&
                                t.TranactionTitle.Contains(SearchTitle, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                // Reset to all transactions if search input is empty
                FilteredTransactions = new List<Transaction>(Transactions);
            }
        }

        private void FilterTransactionsByDateRange()
        {
            // Ensure both dates are provided and EndDate >= StartDate
            if (StartDate.HasValue && EndDate.HasValue && EndDate.Value >= StartDate.Value)
            {
                FilteredTransactions = Transactions
                    .Where(t => t.Date >= StartDate.Value && t.Date <= EndDate.Value)
                    .OrderBy(t => t.Date)
                    .ToList();
            }
            else
            {
                // Reset to all transactions if dates are invalid
                FilteredTransactions = new List<Transaction>(Transactions);
            }
        }


        private void SortTransactionsByType(ChangeEventArgs e)
        {
            SelectedType = e.Value?.ToString();
            FilteredTransactions = string.IsNullOrEmpty(SelectedType)
                ? new List<Transaction>(Transactions)
                : Transactions.Where(t => t.TransactionLabel == SelectedType).ToList();
        }

        private void SortTransactionsByAmount(ChangeEventArgs e)
        {
            SelectedOrder = e.Value?.ToString();
            FilteredTransactions = SelectedOrder switch
            {
                "Asc" => FilteredTransactions.OrderBy(t => t.Amount).ToList(),
                "Desc" => FilteredTransactions.OrderByDescending(t => t.Amount).ToList(),
                _ => new List<Transaction>(Transactions)
            };
        }

        private void SortTransactionsByTag(ChangeEventArgs e)
        {
            var selectedTag = e.Value?.ToString();
            FilteredTransactions = string.IsNullOrEmpty(selectedTag)
                ? new List<Transaction>(Transactions)
                : Transactions.Where(t => t.TagName == selectedTag).ToList();
        }

        private void SortTransactionsByDate(ChangeEventArgs e)
        {
            var sortOrder = e.Value?.ToString();
            FilteredTransactions = sortOrder switch
            {
                "Newest" => Transactions.OrderByDescending(t => t.Date).ToList(),
                "Oldest" => Transactions.OrderBy(t => t.Date).ToList(),
                _ => new List<Transaction>(Transactions)
            };
        }


        private void FilterTransactionsBySingleDate()
        {
            if (FilterDate.HasValue)
            {
                // Filter transactions by the specific date
                FilteredTransactions = Transactions
                    .Where(t => t.Date.Date == FilterDate.Value.Date)
                    .OrderBy(t => t.Date)
                    .ToList();
            }
            else
            {
                // Reset to all transactions if no date is selected
                FilteredTransactions = new List<Transaction>(Transactions);
            }
        }

        private async Task ExportFilteredTransactionsToJson()
        {
            try
            {
                // Serialize the filtered transactions to JSON
                var jsonData = JsonSerializer.Serialize(FilteredTransactions);

                // Create a Blob and trigger download
                var dataUri = $"data:text/json;charset=utf-8,{Uri.EscapeDataString(jsonData)}";
                var a = new ElementReference();
                await JS.InvokeVoidAsync("downloadJsonFile", dataUri, "filtered_transactions.json");
            }
            catch (Exception ex)
            {
                errorMessage = $"Error exporting data: {ex.Message}";
            }
        }
    }
}
