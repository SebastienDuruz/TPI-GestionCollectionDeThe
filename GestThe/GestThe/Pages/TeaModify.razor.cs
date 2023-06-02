/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : TeaModify Component logic
*/

using GestTheLib.Models.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace GestThé.Pages;

/// <summary>
/// Class TeaModify
/// </summary>
public partial class TeaModify
{
    /// <summary>
    /// Id of the tea to show
    /// </summary>
    [Parameter]
    public int TeaId { get; set; }
    
    /// <summary>
    /// The Tea to add
    /// </summary>
    public TTea TeaToModify { get; set; }
    
    /// <summary>
    /// List of the 30 last years
    /// </summary>
    private List<long> LastYears { get; set; }

    /// <summary>
    /// OnInitializedAsync method
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Get the tea from database
        TeaToModify = DatabaseContext.TTeas.Include(x => x.IdProviderNavigation)
            .Include(x => x.IdRegionNavigation)
            .Include(x => x.IdVarietyNavigation)
            .Include(x => x.IdTypeNavigation)
            .Where(x => x.IdTea == TeaId).First();

        // Build a list that contains the 30 last years
        LastYears = new List<long>();
        for (int i = 0; i < 30; ++i)
            LastYears.Add(DateTime.Now.Year - i);
    }
    
    /// <summary>
    /// Handle the tea add if form is valid
    /// </summary>
    private async Task HandleTeaAdd()
    {
        // The form has been filled
        if (TeaToModify.IdVarietyNavigation != null && TeaToModify.IdRegionNavigation != null &&
            TeaToModify.IdProviderNavigation != null && TeaToModify.IdTypeNavigation != null)
        {
            // Update the modification date
            TeaToModify.TeaModificationDate = DateTime.Now;
            
            await Task.Run(() => (DatabaseContext.TTeas.Update(TeaToModify)));
            await DatabaseContext.SaveChangesAsync();
            DatabaseContext.AttachRange(TeaToModify);

            TeaToModify = new TTea();
            
            NavigationManager.NavigateTo($"{NavigationManager.Uri}?message=updatedtea", true);
        }
        else
        {
            // Notify user that the operation failed
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error, Summary = $"Veuillez entrer toutes les données requises !"
            });
        }
    }
}