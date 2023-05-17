/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : Index Component logic
*/

using System.Collections;
using ElectronNET.API.Entities;
using GestThéLib.Models.Database;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Radzen.Blazor;

namespace GestThé.Pages;

public partial class Index
{
    /// <summary>
    /// Show Archived tea on the datagrid ?
    /// </summary>
    private bool _showArchived;
    
    /// <summary>
    /// List of the teas
    /// </summary>
    private IQueryable<TTea> CollectionTeas { get; set; }

    /// <summary>
    /// Datagrid for the teas
    /// </summary>
    private RadzenDataGrid<TTea> TeaDataGrid { get; set; }

    /// <summary>
    /// OnInitializedAsync Method
    /// </summary>
    /// <returns>Result of the Task</returns>
    protected override Task OnInitializedAsync()
    {
        _showArchived = false;
        
        CollectionTeas = DatabaseContext.TTeas.Include(x => x.IdTypeNavigation)
            .Include(x => x.IdVarietyNavigation)
            .Include(x => x.IdRegionNavigation)
            .Include(x => x.IdRegionNavigation.IdCountryNavigation)
            .Include(x => x.IdProviderNavigation)
            .Where(x => !x.TeaIsArchived.Value);
        
        return base.OnInitializedAsync();
    }
    
    /// <summary>
    /// Update the value of ShowArchived
    /// </summary>
    /// <param name="isChecked">New value to apply</param>
    void OnShowArchivedChange(bool isChecked)
    {
        _showArchived = isChecked;
        
        // Rebuild the corresponding list of tea
        if (_showArchived)
        {
            CollectionTeas = DatabaseContext.TTeas.Include(x => x.IdTypeNavigation)
                .Include(x => x.IdVarietyNavigation)
                .Include(x => x.IdRegionNavigation)
                .Include(x => x.IdRegionNavigation.IdCountryNavigation)
                .Include(x => x.IdProviderNavigation);
        }
        else
        {
            CollectionTeas = DatabaseContext.TTeas.Include(x => x.IdTypeNavigation)
                .Include(x => x.IdVarietyNavigation)
                .Include(x => x.IdRegionNavigation)
                .Include(x => x.IdRegionNavigation.IdCountryNavigation)
                .Include(x => x.IdProviderNavigation)
                .Where(x => !x.TeaIsArchived.Value);
        }
    }

    /// <summary>
    /// Delete the specific tea if user confirm the Dialog
    /// </summary>
    /// <param name="teaToDelete">The tea to delete</param>
    async Task DeleteTea(TTea teaToDelete)
    {
        var result = await DialogService.Confirm($"Voulez-vous vraiment archiver le thé {teaToDelete.TeaName} ?", "Archiver un thé", new ConfirmOptions() { OkButtonText = "Oui", CancelButtonText = "Non", CloseDialogOnOverlayClick = true});

        if (result.HasValue && result.Value)
        {
            // The value exists in the current Data
            if (CollectionTeas.Contains(teaToDelete))
            {
                // Remove from database
                DatabaseContext.Remove<TTea>(teaToDelete);
                await DatabaseContext.SaveChangesAsync();

                // Update the lists used by view
                CollectionTeas = CollectionTeas.Where(x => x.IdTea != teaToDelete.IdTea);

                // Reload the data of the DataGrid component
                await TeaDataGrid.Reload();
            }
            else
            {
                // Cancel the operation
                TeaDataGrid.CancelEditRow(teaToDelete);
                await TeaDataGrid.Reload();
            }
        }
    }
}