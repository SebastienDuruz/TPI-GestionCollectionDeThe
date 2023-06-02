/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : ListModify Component logic
*/

using GestTheLib.Models.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen.Blazor;

namespace GestThé.Pages;

/// <summary>
/// Class ListModify
/// </summary>
public partial class ListModify
{
    /// <summary>
    /// The name of the default field (use if user does not select any)
    /// </summary>
    private const string DEFAULT_FIELD = "nom";

    /// <summary>
    /// Teas that has been selected on the datagrid
    /// </summary>
    private IEnumerable<TTea> _selectedTeas;
    
    /// <summary>
    /// Id of the tea to show
    /// </summary>
    [Parameter]
    public long ListId { get; set; }
    
    /// <summary>
    /// The list to add
    /// </summary>
    private TList ListToModify { get; set; }

    /// <summary>
    /// Datagrid that contains the selectable teas
    /// </summary>
    private RadzenDropDownDataGrid<IEnumerable<TTea>> TeaDataGrid { get; set; }
    
    /// <summary>
    /// List of teas that can be selected
    /// </summary>
    private IEnumerable<TTea> Teas { get; set; }
    
    /// <summary>
    /// All the possible fields
    /// </summary>
    private List<TField> Fields { get; set; }
    
    /// <summary>
    /// Selected fields for the list
    /// </summary>
    private List<TField> SelectedFields { get; set; }
    
    /// <summary>
    /// OnInitializedAsync method
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        // get the list to modify by the ID
        ListToModify = await DatabaseContext.TLists
            .Include(x => x.IdFields)
            .Include(x => x.IdTeas)
            .FirstAsync(x => x.IdList == ListId);

        // Set the fields and preselect the selected values
        SelectedFields = ListToModify.IdFields.ToList();
        Fields = DatabaseContext.TFields.Where(x => !SelectedFields.Contains(x)).ToList();

        _selectedTeas = ListToModify.IdTeas.ToList();
        
        // Fetch the list of teas for Datagrid
        Teas = DatabaseContext.TTeas.Where(x => x.TeaIsArchived == false);
    }
    
    /// <summary>
    /// Select all the fields at once
    /// </summary>
    private void SelectAllFields()
    {
        Fields.Clear();
        SelectedFields = DatabaseContext.TFields.ToList();
    }

    /// <summary>
    /// Clear all the fields at once
    /// </summary>
    private void ClearFields()
    {
        Fields = DatabaseContext.TFields.ToList();
        SelectedFields.Clear();
    }
    
    /// <summary>
    /// Handle the list add process
    /// </summary>
    private async Task HandleListModify()
    {
        // Add the tea values to final object
        ListToModify.IdTeas.Clear();
        foreach (TTea tea in _selectedTeas)
            ListToModify.IdTeas.Add(tea);

        // If user select at least 1 field
        if (SelectedFields.Any())
            ListToModify.IdFields = SelectedFields.ToList();
        // No field selected, set the default one
        else
            ListToModify.IdFields = DatabaseContext.TFields.Where(x => x.FieldName == DEFAULT_FIELD).ToList();

        // Update the modification date
        ListToModify.ListModificationDate = DateTime.Now;
        
        // Add the list onto database
        await Task.Run(() => (DatabaseContext.TLists.Update(ListToModify)));
        await DatabaseContext.SaveChangesAsync();
        DatabaseContext.AttachRange(ListToModify);

        // Reset the values and redirect to list page
        ListToModify = new TList();
        _selectedTeas = new List<TTea>();
        NavigationManager.NavigateTo($"{NavigationManager.Uri}?message=updatedlist", true);
    }
}