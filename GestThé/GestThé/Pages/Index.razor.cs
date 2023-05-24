/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : Index Component logic
*/

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
    /// Search value for Name
    /// </summary>
    private string _teaNameSearchValue;
    
    /// <summary>
    /// Search value for Variety
    /// </summary>
    private string _teaVarietySearchValue;
    
    /// <summary>
    /// Search value for Type
    /// </summary>
    private string _teaTypeSearchValue;
    
    /// <summary>
    /// Search value for Region
    /// </summary>
    private string _teaRegionSearchValue;
    
    /// <summary>
    /// Search value for Min Price
    /// </summary>
    private double _teaMinPriceSearchValue;
    
    /// <summary>
    /// Search value for Max Price
    /// </summary>
    private double _teaMaxPriceSearchValue;
    
    /// <summary>
    /// Search value for Years
    /// </summary>
    private List<long> _teaYearSearchValues;
    
    /// <summary>
    /// Values for the filter : Years
    /// </summary>
    private List<long> _teaYearsValues;

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
        _teaMinPriceSearchValue = 0;
        _teaMaxPriceSearchValue = 1000;
        _teaYearsValues = new List<long>();
        _teaYearSearchValues = new List<long>();

        CollectionTeas = DatabaseContext.TTeas.Include(x => x.IdTypeNavigation)
            .Include(x => x.IdVarietyNavigation)
            .Include(x => x.IdRegionNavigation)
            .Include(x => x.IdRegionNavigation.IdCountryNavigation)
            .Include(x => x.IdProviderNavigation)
            .Where(x => !x.TeaIsArchived.Value);

        BuildYearList();

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

        Search();
        BuildYearList();
    }

    /// <summary>
    /// Delete (hard delete) the specific tea if user confirm the Dialog
    /// </summary>
    /// <param name="teaToDelete">The tea to delete (hard delete)</param>
    async Task DeleteTea(TTea teaToDelete)
    {
        var result = await DialogService.Confirm($"Voulez-vous vraiment supprimer le thé {teaToDelete.TeaName} ?", "Supprimer un thé", new ConfirmOptions() { OkButtonText = "Oui", CancelButtonText = "Non", CloseDialogOnOverlayClick = true});

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

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success, Summary = "Le thé a bien été supprimé !", Duration = 2000,
                    CloseOnClick = true
                });

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

    /// <summary>
    /// Build a new list of years from the current data shown
    /// </summary>
    private void BuildYearList()
    {
        _teaYearsValues = new List<long>();
        foreach (TTea tea in CollectionTeas.ToList())
        {
            if(!_teaYearsValues.Contains(tea.TeaYear.Value))
                _teaYearsValues.Add(tea.TeaYear.Value);
        }
    }

    /// <summary>
    /// Update Name method
    /// </summary>
    /// <param name="value">new value</param>
    private void NameOnChange(string value)
    {
        _teaNameSearchValue = value.ToLower();
        Search();
    }
    
    /// <summary>
    /// Update Variety method
    /// </summary>
    /// <param name="value">new value</param>
    private void VarietyOnChange(string value)
    {
        _teaVarietySearchValue = value.ToLower();
        Search();
    }
    
    /// <summary>
    /// Update Type method
    /// </summary>
    /// <param name="value">new value</param>
    private void TypeOnChange(string value)
    {
        _teaTypeSearchValue = value.ToLower();
        Search();
    }
    
    /// <summary>
    /// Update Region method
    /// </summary>
    /// <param name="value">new value</param>
    private void RegionOnChange(string value)
    {
        _teaRegionSearchValue = value.ToLower();
        Search();
    }
    
    /// <summary>
    /// Update MinPrice method
    /// </summary>
    /// <param name="value">new value</param>
    private void MinPriceOnChange(double value)
    {
        _teaMinPriceSearchValue = value;
        Search();
    }
    
    /// <summary>
    /// Update MaxPrice method
    /// </summary>
    /// <param name="value">new value</param>
    private void MaxPriceOnChange(double value)
    {
        _teaMaxPriceSearchValue = value;
        Search();
    }

    /// <summary>
    /// Custom search function
    /// </summary>
    private void Search()
    {
        if (_teaYearSearchValues == null)
            _teaYearSearchValues = new List<long>();
            
        // Fetch the teas from the collection
        CollectionTeas = DatabaseContext.TTeas.Include(x => x.IdTypeNavigation)
            .Include(x => x.IdVarietyNavigation)
            .Include(x => x.IdRegionNavigation)
            .Include(x => x.IdRegionNavigation.IdCountryNavigation)
            .Include(x => x.IdProviderNavigation);
        
        // Apply the price filter (range between)
        CollectionTeas = CollectionTeas.Where(x =>
            x.TeaPrice >= _teaMinPriceSearchValue && x.TeaPrice <= _teaMaxPriceSearchValue);

        // Remove archived tea if needed
        if (!_showArchived)
            CollectionTeas = CollectionTeas.Where(x => x.TeaIsArchived == false);

        // Apply Name filter
        if (!String.IsNullOrWhiteSpace(_teaNameSearchValue))
            CollectionTeas = CollectionTeas.Where(x => x.TeaName.ToLower().Contains(_teaNameSearchValue));
        
        // Apply Variety filter
        if (!String.IsNullOrWhiteSpace(_teaVarietySearchValue))
            CollectionTeas =
                CollectionTeas.Where(x => x.IdVarietyNavigation.VarietyName.ToLower().Contains(_teaVarietySearchValue));

        // Apply Type filter
        if (!String.IsNullOrWhiteSpace(_teaTypeSearchValue))
            CollectionTeas =
                CollectionTeas.Where(x => x.IdTypeNavigation.TypeName.ToLower().Contains(_teaTypeSearchValue));
            
        // Apply region filter
        if (!String.IsNullOrWhiteSpace(_teaRegionSearchValue))
            CollectionTeas = CollectionTeas.Where(x =>
                x.IdRegionNavigation.RegionName.ToLower().Contains(_teaRegionSearchValue)
                || x.IdRegionNavigation.IdCountryNavigation.CountryName.ToLower().Contains(_teaRegionSearchValue));

        // Apply Year filter
        if (_teaYearSearchValues != null && _teaYearSearchValues.Any())
            CollectionTeas = CollectionTeas.Where(x => _teaYearSearchValues.Contains(x.TeaYear.Value));
        
        StateHasChanged();
    }

    /// <summary>
    /// Clear the filters function
    /// </summary>
    private void ClearFilters()
    {
        // Reset the values
        _teaNameSearchValue = "";
        _teaVarietySearchValue = "";
        _teaTypeSearchValue = "";
        _teaRegionSearchValue = "";
        _teaMaxPriceSearchValue = 1000;
        _teaMinPriceSearchValue = 0;
        _teaYearSearchValues = new List<long>();
        
        // Search again to reset default values
        Search();
    }
}