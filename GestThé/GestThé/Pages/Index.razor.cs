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
    
    void OnShowArchivedChange(bool isChecked)
    {
        _showArchived = isChecked;
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
}