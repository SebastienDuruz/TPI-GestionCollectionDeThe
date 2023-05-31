/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : Lists Component logic
*/

using GestThéLib.Models.Database;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Radzen.Blazor;

namespace GestThé.Pages;

/// <summary>
/// Class Lists
/// </summary>
public partial class Lists
{

    /// <summary>
    /// Search value for Name
    /// </summary>
    private string _nameSearchValue;
    
    /// <summary>
    /// Search value for Fields
    /// </summary>
    private string _fieldsSearchValue;
    
    /// <summary>
    /// Collection of the Lists of teas
    /// </summary>
    private IQueryable<TList> CollectionLists { get; set; }
    
    /// <summary>
    /// Lists Datagrid
    /// </summary>
    private RadzenDataGrid<TList> ListsDataGrid { get; set; }

    /// <summary>
    /// OnInitializedAsync Method
    /// </summary>
    /// <returns>Result of the Task</returns>
    protected override Task OnInitializedAsync()
    {
        CollectionLists = DatabaseContext.TLists
            .Include(x => x.IdFields)
            .Include(x => x.IdTeas);
        
        return base.OnInitializedAsync();
    }

    /// <summary>
    /// Update Name method
    /// </summary>
    /// <param name="value">new value</param>
    private void NameOnChange(string value)
    {
        _nameSearchValue = value.ToLower();
        Search();
    }
    
    /// <summary>
    /// Update Field method
    /// </summary>
    /// <param name="value">new value</param>
    private void FieldOnChange(string value)
    {
        if (String.IsNullOrWhiteSpace(value))
            _fieldsSearchValue = "";
        else
            _fieldsSearchValue = value.ToLower();
        Search();
    }
    
    /// <summary>
    /// Custom search function
    /// </summary>
    private void Search()
    {
        // Fetch the teas from the collection
        CollectionLists = DatabaseContext.TLists.Include(x => x.IdTeas);

        if (!String.IsNullOrWhiteSpace(_nameSearchValue))
            CollectionLists = CollectionLists.Where(x => x.ListName.ToLower().Contains(_nameSearchValue));

        if (!String.IsNullOrWhiteSpace(_fieldsSearchValue))
            CollectionLists = CollectionLists.Where(x =>
                x.IdFields.Any(field => field.FieldName.ToLower().Contains(_fieldsSearchValue)));
        
        StateHasChanged();
    }
    
    /// <summary>
    /// Clear the filters function
    /// </summary>
    private void ClearFilters()
    {
        // Reset the values
        _nameSearchValue = "";
        _fieldsSearchValue = "";
        
        // Search again to reset default values
        Search();
    }
    
    /// <summary>
    /// Delete (hard delete) the specific tea if user confirm the Dialog
    /// </summary>
    /// <param name="listToDelete">The list to delete</param>
    async Task DeleteList(TList listToDelete)
    {
        var result = await DialogService.Confirm($"Voulez-vous vraiment supprimer la liste {listToDelete.ListName} ?", "Supprimer une liste", new ConfirmOptions() { OkButtonText = "Oui", CancelButtonText = "Non", CloseDialogOnOverlayClick = true});

        if (result.HasValue && result.Value)
        {
            // The value exists in the current Data
            if (CollectionLists.Contains(listToDelete))
            {
                // Remove from database
                DatabaseContext.Remove<TList>(listToDelete);
                await DatabaseContext.SaveChangesAsync();

                // Update the lists used by view
                CollectionLists = CollectionLists.Where(x => x.IdList != listToDelete.IdList);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success, Summary = "La liste a bien été supprimée !", Duration = 2000,
                    CloseOnClick = true
                });

                // Reload the data of the DataGrid component
                await ListsDataGrid.Reload();
            }
            else
            {
                // Cancel the operation
                ListsDataGrid.CancelEditRow(listToDelete);
                await ListsDataGrid.Reload();
            }
        }
    }
}