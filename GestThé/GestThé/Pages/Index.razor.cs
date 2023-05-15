/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : Index Component logic
*/

using GestThéLib.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace GestThé.Pages;

public partial class Index
{
    /// <summary>
    /// Regions List
    /// </summary>
    private IEnumerable<TRegion> Regions { get; set; }

    /// <summary>
    /// OnInitializedAsync Method
    /// </summary>
    /// <returns>Result of the Task</returns>
    protected override Task OnInitializedAsync()
    {
        Regions = DatabaseContext.TRegions.Include(x => x.IdCountryNavigation);
        return base.OnInitializedAsync();
    }
}