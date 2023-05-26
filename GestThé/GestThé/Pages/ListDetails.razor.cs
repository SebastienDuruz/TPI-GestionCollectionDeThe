/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : ListDetails Component logic
*/

using ElectronNET.API;
using ElectronNET.API.Entities;
using GestThéLib.Data.CSV;
using GestThéLib.Data.Electron;
using GestThéLib.Models.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Radzen.Blazor;

namespace GestThé.Pages;

/// <summary>
/// Class ListDetails
/// </summary>
public partial class ListDetails
{
    /// <summary>
    /// Id of the tea to show
    /// </summary>
    [Parameter]
    public long? ListId { get; set; }
    
    /// <summary>
    /// The tea to show info
    /// </summary>
    private TList List { get; set; }
    
    /// <summary>
    /// Datagrid that contains the list of tea for the collection
    /// </summary>
    private RadzenDataGrid<TTea> TeaDataGrid { get; set; }

    /// <summary>
    /// List of the teas contains into the list
    /// </summary>
    private IEnumerable<TTea> TeaList { get; set; } = new List<TTea>();

    /// <summary>
    /// OnInitializedAsync method
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        if (ListId != null)
            List = await DatabaseContext.TLists
                .Include(x => x.IdFields)
                .Include(x => x.IdTeas)
                .Where(x => x.IdList == ListId).FirstAsync();
        else
            List = new TList();

        TeaList = DatabaseContext.TTeas
            .Include(x => x.IdProviderNavigation)
            .Include(x => x.IdRegionNavigation)
            .Include(x => x.IdRegionNavigation.IdCountryNavigation)
            .Include(x => x.IdVarietyNavigation)
            .Include(x => x.IdTypeNavigation)
            .Include(x => x.IdLists).Where(x => x.IdLists.Contains(List));
    }
    
    /// <summary>
    /// Navigate back to Listspage
    /// </summary>
    private async Task ReturnToLists()
    {
        NavigationManager.NavigateTo(Path.Combine($"{NavigationManager.BaseUri}", "lists"));
    }
    
    /// <summary>
    /// Export the list of tea to CSV
    /// </summary>
    private async Task ExportCSV()
    {
        string folderPath = null;
        CsvGenerator csvGenerator = new CsvGenerator();
        
        // If Electron is active, open a Dialog, user will select the folder where to save the export
        if (HybridSupport.IsElectronActive)
            folderPath = await ElectronHandler.OpenSelectFolderDialog();
        
        // Build a clone of the list for export
        TList listToExport = new TList()
        {
            ListName = List.ListName,
            ListDescription = List.ListDescription,
            ListAddDate = List.ListAddDate,
            ListModificationDate = List.ListModificationDate,
            IdList = List.IdList,
            IdTeas = TeaList.ToList(),
            IdFields = List.IdFields.ToList()
        };
        
        // Generate the export if user selected a folder
        if (String.IsNullOrWhiteSpace(folderPath))
        {
            // DO NOTHING
        }
        else
        {
            csvGenerator.WriteListExport(listToExport, folderPath);
        
            // Notify the user about the export
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success, Summary = "L'export CSV a été sauvegardé", Duration = 2000,
                CloseOnClick = true
            });
        }
    }
}