/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : ListAdd Component logic
*/

using GestThéLib.Models.Database;
using Radzen.Blazor;

namespace GestThé.Pages;

/// <summary>
/// Class ListAdd
/// </summary>
public partial class ListAdd
{
    /// <summary>
    /// The name of the default field (use if user does not select any)
    /// </summary>
    private const string DEFAULT_FIELD = "nom";
    
    /// <summary>
    /// Teas that has been selected on the datagrid
    /// </summary>
    private IEnumerable<TTea> _selectedTeas = new List<TTea>();
    
    /// <summary>
    /// The list to add
    /// </summary>
    private TList ListToAdd { get; set; }

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
    protected override Task OnInitializedAsync()
    {
        ListToAdd = new TList()
        {
            IdFields = new List<TField>(),
            IdTeas = new List<TTea>()
        };

        // Set the fields and preselect the Name field
        Fields = DatabaseContext.TFields.Where(x => x.FieldName != DEFAULT_FIELD).ToList();
        SelectedFields = DatabaseContext.TFields.Where(x => x.FieldName == DEFAULT_FIELD).ToList();
            
        // Fetch the list of teas for Datagrid
        Teas = DatabaseContext.TTeas.Where(x => x.TeaIsArchived == false);
        
        return base.OnInitializedAsync();
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
    private async Task HandleListAdd()
    {
        // Add the tea values to final object
        foreach (TTea tea in _selectedTeas)
            ListToAdd.IdTeas.Add(tea);

        // If user select at least 1 field
        if (SelectedFields.Any())
            ListToAdd.IdFields = SelectedFields.ToList();
        // No field selected, set the default one
        else
            ListToAdd.IdFields = DatabaseContext.TFields.Where(x => x.FieldName == DEFAULT_FIELD).ToList();
        
        // Add the list onto database
        await DatabaseContext.TLists.AddAsync(ListToAdd);
        await DatabaseContext.SaveChangesAsync();
        DatabaseContext.AttachRange(ListToAdd);

        // Reset the values and redirect to list page
        ListToAdd = new TList();
        _selectedTeas = new List<TTea>();
        NavigationManager.NavigateTo($"{NavigationManager.Uri}?message=addedlist", true);
    }
}