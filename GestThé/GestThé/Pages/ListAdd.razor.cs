/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : ListAdd Component logic
*/

using ElectronNET.API.Entities;
using GestThéLib.Models.Database;
using Radzen.Blazor;

namespace GestThé.Pages;

/// <summary>
/// Class ListAdd
/// </summary>
public partial class ListAdd
{
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
    /// OnInitializedAsync method
    /// </summary>
    protected override Task OnInitializedAsync()
    {
        ListToAdd = new TList()
        {
            IdFields = new List<TField>(),
            IdTeas = new List<TTea>()
        };

        Teas = DatabaseContext.TTeas.Where(x => x.TeaIsArchived == false);
        
        return base.OnInitializedAsync();
    }

    /// <summary>
    /// Handle the list add process
    /// </summary>
    private async Task HandleListAdd()
    {
        // Add the tea values to final object
        foreach (TTea tea in _selectedTeas)
            ListToAdd.IdTeas.Add(tea);
        
        await DatabaseContext.TLists.AddAsync(ListToAdd);
        await DatabaseContext.SaveChangesAsync();
        DatabaseContext.AttachRange(ListToAdd);

        ListToAdd = new TList();
        _selectedTeas = new List<TTea>();
        
        NavigationManager.NavigateTo($"{NavigationManager.Uri}?message=addedlist", true);
    }
}