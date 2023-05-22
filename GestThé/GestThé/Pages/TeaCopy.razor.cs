/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : TeaCopy Component logic
*/

using GestThéLib.Models.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace GestThé.Pages;

/// <summary>
/// Class TeaCopy
/// </summary>
public partial class TeaCopy
{
    /// <summary>
    /// Id of the tea to show
    /// </summary>
    [Parameter]
    public int TeaId { get; set; }
    
    /// <summary>
    /// The Tea to add
    /// </summary>
    public TTea CopiedTea { get; set; }
    
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
        TTea teaToCopy = await DatabaseContext.TTeas.Include(x => x.IdProviderNavigation)
            .Include(x => x.IdRegionNavigation)
            .Include(x => x.IdVarietyNavigation)
            .Include(x => x.IdTypeNavigation)
            .Where(x => x.IdTea == TeaId).FirstAsync();

        // Copy the values to a new object (avoir editing the initial object)
        CopiedTea = new TTea()
        {
            TeaName = teaToCopy.TeaName,
            TeaDescription = teaToCopy.TeaDescription,
            TeaYear = teaToCopy.TeaYear,
            TeaQuantity = teaToCopy.TeaQuantity,
            TeaPrice = teaToCopy.TeaPrice,
            TeaIsArchived = teaToCopy.TeaIsArchived,
            IdProviderNavigation = teaToCopy.IdProviderNavigation,
            IdRegionNavigation = teaToCopy.IdRegionNavigation,
            IdTypeNavigation = teaToCopy.IdTypeNavigation,
            IdVarietyNavigation = teaToCopy.IdVarietyNavigation
        };

        // Reset the ID for avoiding rewrite the same Object
        CopiedTea.IdTea = 0;

        // Build a list that contains the 30 last years
        LastYears = new List<long>();
        for (int i = 0; i < 30; ++i)
            LastYears.Add(DateTime.Now.Year - i);
    }

    /// <summary>
    /// Handle the tea add if form is valid
    /// </summary>
    private async Task HandleTeaCopy()
    {
        // The form has been filled
        if (CopiedTea.IdVarietyNavigation != null && CopiedTea.IdRegionNavigation != null &&
            CopiedTea.IdProviderNavigation != null && CopiedTea.IdTypeNavigation != null)
        {
            await DatabaseContext.TTeas.AddAsync(CopiedTea);
            await DatabaseContext.SaveChangesAsync();
            DatabaseContext.AttachRange(CopiedTea);

            // Notify user and reset the form for next operation
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success, Summary = $"Le thé {CopiedTea.TeaName} a été copié et ajouté !"
            });

            CopiedTea = new TTea();

            NavigationManager.NavigateTo(NavigationManager.Uri, true);
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