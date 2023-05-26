/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : PrintPDF Component logic
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestThéLib.Data.PDF;
using GestThéLib.Models.Database;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace GestThé.Pages;

/// <summary>
/// Class PrintPDF
/// </summary>
public partial class PrintPDF
{
    /// <summary>
    /// Id of the div to use
    /// </summary>
    private string _idPreviewPdf = "pdfContainer";
 
    /// <summary>
    /// Id of the list to print
    /// </summary>
    [Parameter]
    public long ListId { get; set; }
    
    /// <summary>
    /// Teas to print
    /// </summary>
    [Parameter]
    public List<TTea> Teas { get; set; }
 
    /// <summary>
    /// List objects to print
    /// </summary>
    private TList List { get; set; }

    /// <summary>
    /// PdfGenerator instance
    /// </summary>
    private PdfGenerator PdfGenerator { get; set; }
 
    /// <summary>
    /// OnInitializedAsync Method
    /// </summary>
    /// <returns>Result of the Task</returns>
    protected override Task OnInitializedAsync()
    {
        PdfGenerator = new PdfGenerator();

        if (ListId != 0)
        {
            List = DatabaseContext.TLists
                .Include(x => x.IdFields)
                .First(x => x.IdList == ListId);
        
            List.IdTeas = DatabaseContext.TTeas
                .Include(x => x.IdProviderNavigation)
                .Include(x => x.IdRegionNavigation)
                .Include(x => x.IdRegionNavigation.IdCountryNavigation)
                .Include(x => x.IdVarietyNavigation)
                .Include(x => x.IdTypeNavigation)
                .Include(x => x.IdLists).Where(x => x.IdLists.Contains(List)).ToList();
        }
        
        return base.OnInitializedAsync();
    }

    /// <summary>
    /// OnAfterRenderAsync
    /// </summary>
    /// <param name="firstRender">It is the first render ?</param>
    /// <returns>Result of the Task</returns>
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        // Check which object to print by the given parameter : if Teas has been passed -> TEAS // Else it's a LIST
        if (Teas != null)
            PdfGenerator.ViewTeasPdf(JsRuntime, _idPreviewPdf, Teas);
        else
            PdfGenerator.ViewListPdf(JsRuntime, _idPreviewPdf, List);
        
        return base.OnAfterRenderAsync(firstRender);
    }
}