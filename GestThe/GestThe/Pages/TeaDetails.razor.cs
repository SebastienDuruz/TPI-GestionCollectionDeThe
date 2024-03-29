﻿/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : TeaDetails Component logic
*/

using GestTheLib.Models.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace GestThé.Pages;

/// <summary>
/// Class TeaDetails
/// </summary>
public partial class TeaDetails
{
    /// <summary>
    /// Id of the tea to show
    /// </summary>
    [Parameter]
    public long? TeaId { get; set; }
    
    /// <summary>
    /// The tea to show info
    /// </summary>
    private TTea Tea { get; set; }

    /// <summary>
    /// OnInitializedAsync method
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        if (TeaId != null)
            Tea = await DatabaseContext.TTeas.Include(x => x.IdTypeNavigation)
                .Include(x => x.IdVarietyNavigation)
                .Include(x => x.IdRegionNavigation)
                .Include(x => x.IdRegionNavigation.IdCountryNavigation)
                .Include(x => x.IdProviderNavigation).Where(x => x.IdTea == TeaId).FirstOrDefaultAsync();
        else
        {
            Tea = new TTea();
        }
    }

    /// <summary>
    /// Navigate back to homepage
    /// </summary>
    private async Task ReturnToHome()
    {
        NavigationManager.NavigateTo(NavigationManager.BaseUri);
    }
}