/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : TeaAdd Component logic
*/

using GestThéLib.Models.Database;
using Radzen;

namespace GestThé.Pages;

/// <summary>
/// Class TeaAdd
/// </summary>
public partial class TeaAdd
{
    /// <summary>
    /// The Tea to add
    /// </summary>
    private TTea TeaToAdd { get; set; }
    
    /// <summary>
    /// List of the 30 last years
    /// </summary>
    private List<long> LastYears { get; set; }

    /// <summary>
    /// OnInitializedAsync method
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Set the default values for the form
        TeaToAdd = new TTea()
        {
            TeaPrice = 1,
            TeaQuantity = 100,
            TeaIsArchived = false,
            TeaYear = DateTime.Now.Year
        };

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
        if (TeaToAdd.IdVarietyNavigation != null && TeaToAdd.IdRegionNavigation != null &&
            TeaToAdd.IdProviderNavigation != null && TeaToAdd.IdTypeNavigation != null)
        {
            await DatabaseContext.TTeas.AddAsync(TeaToAdd);
            await DatabaseContext.SaveChangesAsync();
            DatabaseContext.AttachRange(TeaToAdd);

            TeaToAdd = new TTea();
            
            NavigationManager.NavigateTo($"{NavigationManager.Uri}?message=addedtea", true);
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