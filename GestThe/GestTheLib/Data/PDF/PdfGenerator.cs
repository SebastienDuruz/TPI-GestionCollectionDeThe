/**
 * ETML
 * Author : Sébastien Duruz
 * Date : 25.05.2023
 * Description : Generate PDF with iTextSharp.LGPLv2 library.
 */

using GestTheLib.Models.Database;
using iText.Html2pdf;
using Microsoft.JSInterop;

namespace GestTheLib.Data.PDF;

/// <summary>
/// Class PdfGenerator
/// </summary>
public class PdfGenerator
{
    /// <summary>
    /// Header of the PDF export, contains the header values and CSS classes
    /// </summary>
    private static string PDF_HEADER = @"<!DOCTYPE html>
<html lang='fr'>
<head>
    <meta charset='UTF-8'>
    <title>PDF export</title>
    <style>
        body {
            font-family: 'gg sans', 'Noto Sans', 'Helvetica Neue', Helvetica, Arial, sans-serif;
            width: 400px;
        }

        table {
            border-collapse: collapse;
            width: 100%;
            margin-bottom: 15px;
        }
        table,
        th,
        td {
            border: 1px solid #707070;
            text-align: center;
            vertical-align: middle;
        }
        td,
        th {
            padding-left: 2px;
            height: 25px;
        }
        th {
            font-weight: bold;
        }
        h1 {
            font-weight: bold;
            font-size: 24px;
            margin: 15px 0;
        }
        h2{
            font-weight: bold;
            font-size: 18px;
            margin: 15px 0;
        }
        p{
            margin-bottom: 5px;
        }

        .archived {
            color: red;
        }
        .center {
            text-align: center;
        }
        .right {
            text-align: right;
        }
        .bold {
            font-weight: bold;
        }
    </style>
</head>
<body>";
    
    /// <summary>
    /// Show the list export onto a given iFrame
    /// </summary>
    /// <param name="jsRuntime">JSRuntime instance used to call JS functions</param>
    /// <param name="idFrame">Id of the DOM div that contains the IFrame</param>
    /// <param name="listToPrint">The List to export</param>
    public void ViewListPdf(IJSRuntime jsRuntime, string idFrame, TList listToPrint)
    {
        jsRuntime.InvokeVoidAsync("ViewPdf", idFrame, Convert.ToBase64String(BuildListExport(listToPrint)));
    }
    
    /// <summary>
    /// Show the teas export onto a given iFrame
    /// </summary>
    /// <param name="jsRuntime">JSRuntime instance used to call JS functions</param>
    /// <param name="idFrame">Id of the DOM div that contains the IFrame</param>
    /// <param name="teasToPrint">The teas to export</param>
    public void ViewTeasPdf(IJSRuntime jsRuntime, string idFrame, List<TTea> teasToPrint)
    {
        jsRuntime.InvokeVoidAsync("ViewPdf", idFrame, Convert.ToBase64String(BuildTeasExport(teasToPrint)));
    }
    
    /// <summary>
    /// Build the PDF file for a List of tea
    /// </summary>
    /// <param name="listToPrint">The list to print</param>
    /// <returns>Content of the Generated pdf formated as byte array</returns>
    private byte[] BuildListExport(TList listToPrint)
    {
        // Header of the HTML
        string html = PDF_HEADER;
        
        // Main Content
        string listDescription = String.IsNullOrWhiteSpace(listToPrint.ListDescription)
            ? "Aucune description"
            : listToPrint.ListDescription;
        
        html += $"<h1 class='center'>{listToPrint.ListName}</h1><h2>Description</h2><p>{listDescription}</p><h2>Date d'ajout</h2><p>{listToPrint.ListAddDate}</p><h2>Date de modification</h2><p>{listToPrint.ListModificationDate}</p><h2>Thés (total: {listToPrint.IdTeas.Count})</h2>";
        
        // Table header
        html += "<table><tr>";
        foreach (TField field in listToPrint.IdFields)
        {
            html += $"<th>{field.FieldName}</th>";
        }
        html += "</tr>";
        

        // Content of the tea table
        foreach (TTea tea in listToPrint.IdTeas)
        {
            html += "<tr>";
            foreach (TField field in listToPrint.IdFields)
            {
                switch (field.FieldName)
                {
                    case "nom":
                        if(tea.TeaIsArchived.Value)
                            html += $"<td class='archived'>{tea.TeaName}</td>";
                        else 
                            html += $"<td>{tea.TeaName}</td>";
                        break;
                    case "description":
                        html += $"<td>{tea.TeaDescription}</td>";
                        break;
                    case "variété":
                        html += $"<td>{tea.IdVarietyNavigation.VarietyName}</td>";
                        break;
                    case "type":
                        html += $"<td>{tea.IdTypeNavigation.TypeName}</td>";
                        break;
                    case "provenance":
                        if(String.IsNullOrWhiteSpace(tea.IdRegionNavigation.RegionName))
                            html += $"<td>{tea.IdRegionNavigation.IdCountryNavigation.CountryName}</td>";
                        else
                            html += $"<td>{tea.IdRegionNavigation.IdCountryNavigation.CountryName} / {tea.IdRegionNavigation.RegionName}</td>";
                        break;
                    case "fournisseur":
                        html += $"<td>{tea.IdProviderNavigation.ProviderName}</td>";
                        break;
                    case "prix":
                        html += $"<td>{tea.TeaPrice} CHF</td>";
                        break;
                    case "quantité":
                        html += $"<td>{tea.TeaQuantity} g</td>";
                        break;
                    case "année":
                        html += $"<td>{tea.TeaYear}</td>";
                        break;
                }
            }
            html += "</tr>";
        }
        
        // End of the body
        html += $"</table><p class='right'>Exporté le : <b class='bold'>{DateTime.Now}</b></p></body></html>";
        
        // Convert to PDF and return the content of the generated PDF
        HtmlConverter.ConvertToPdf(html, new FileStream("output.pdf", FileMode.Create));
        return File.ReadAllBytes("output.pdf");
    }
    
    /// <summary>
    /// Build the PDF file for a List of tea
    /// </summary>
    /// <param name="teasToPrint">The Teas to print</param>
    /// <returns>Content of the Generated pdf formated as byte array</returns>
    private byte[] BuildTeasExport(List<TTea> teasToPrint)
    {
        // Header of the HTML
        string html = PDF_HEADER;

        html += "<h1 class='center'>Export - Thés</h1>";
        
        // Table header
        html += "<table><tr><th>Nom</th><th>Variété</th><th>Type</th><th>Pays / Région</th><th>Prix 100g</th><th>Quantité g</th><th>Année</th></tr>";

        // Table content foreach tea to print
        foreach (TTea tea in teasToPrint)
        {
            html += "<tr>";

            if (tea.TeaIsArchived.Value)
                html += $"<td class='archived'>{tea.TeaName}</td>";
            else
                html += $"<td>{tea.TeaName}</td>";
                
            html += $"<td>{tea.IdVarietyNavigation.VarietyName}</td><td>{tea.IdTypeNavigation.TypeName}</td>";
            if (String.IsNullOrWhiteSpace(tea.IdRegionNavigation.RegionName))
                html += $"<td>{tea.IdRegionNavigation.IdCountryNavigation.CountryName}</td>";
            else
                html += $"<td>{tea.IdRegionNavigation.IdCountryNavigation.CountryName} / {tea.IdRegionNavigation.RegionName}</td>";
            html += $"<td>{tea.TeaPrice} CHF</td><td>{tea.TeaQuantity} g</td><td>{tea.TeaYear}</td>";
            html += "</tr>";
        }
        
        // End of the body
        html += $"</table><p class='right'>Exporté le : <b class='bold'>{DateTime.Now}</b></p></body></html>";
        
        // Convert to PDF and return the content of the generated PDF
        HtmlConverter.ConvertToPdf(html, new FileStream("output.pdf", FileMode.Create));
        return File.ReadAllBytes("output.pdf");
    }
}