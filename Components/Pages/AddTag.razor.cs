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
    public partial class AddTag : ComponentBase
    {
        private string NewTagName;
        private string EditTagName;
        private string ErrorText;
        private int? currentEditId;
        private List<Tag> TagList = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var data = await TagDataService.LoadDataAsync();
                TagList = data?.Tagss ?? new List<Tag>();
            }
            catch (Exception ex)
            {
                ErrorText = $"Error loading tags: {ex.Message}";
                Console.WriteLine(ex.Message);
            }
        }

        private async Task AddNewTag()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewTagName))
                {
                    ErrorText = "Tag cannot be empty.";
                    return;
                }

                if (TagList.Any(t => t.TagName.Equals(NewTagName, StringComparison.OrdinalIgnoreCase)))
                {
                    ErrorText = "This tag already exists.";
                    return;
                }

                var newTag = new Tag { TagId = TagList.Count + 1, TagName = NewTagName };
                TagList.Add(newTag);

                var data = await TagDataService.LoadDataAsync() ?? new ModelList();
                data.Tagss = TagList;
                await TagDataService.SaveDataAsync(data);

                NewTagName = string.Empty;
                ErrorText = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorText = $"Error adding tag: {ex.Message}";
            }
        }

        private void StartEdit(int tagId, string tagName)
        {
            currentEditId = tagId;
            EditTagName = tagName;
        }

        private void CancelEdit()
        {
            currentEditId = null;
            EditTagName = string.Empty;
        }

        private async Task UpdateTag(int tagId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditTagName))
                {
                    ErrorText = "Tag name cannot be empty.";
                    return;
                }

                if (TagList.Any(t => t.TagName.Equals(EditTagName, StringComparison.OrdinalIgnoreCase) && t.TagId != tagId))
                {
                    ErrorText = "This tag name is already in use.";
                    return;
                }

                var tag = TagList.FirstOrDefault(t => t.TagId == tagId);
                if (tag != null)
                {
                    tag.TagName = EditTagName;

                    var data = await TagDataService.LoadDataAsync() ?? new ModelList();
                    data.Tagss = TagList;
                    await TagDataService.SaveDataAsync(data);

                    CancelEdit();
                }
            }
            catch (Exception ex)
            {
                ErrorText = $"Error updating tag: {ex.Message}";
            }
        }
    }
}
