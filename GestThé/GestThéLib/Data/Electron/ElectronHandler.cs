/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : ElectronNET methods used by the main project.
*/

using ElectronNET.API;
using ElectronNET.API.Entities;

namespace GestThéLib.Data.Electron;

/// <summary>
/// Class ElectronHandler
/// </summary>
public static class ElectronHandler
{
    /// <summary>
    /// Main Window of the application
    /// </summary>
    private static BrowserWindow MainWindow { get; set; }
    
    /// <summary>
    /// Build the Electron Window
    /// </summary>
    public static async Task BuildElectronWindow()
    {
        // Setup the Electron Window
        MainWindow = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(
            new BrowserWindowOptions()
            {
                Title = "GestThé",
                AutoHideMenuBar = true
            });

        // Clear the cache before showing the window
        await MainWindow.WebContents.Session.ClearCacheAsync();

        // Add events to the Electron Window
        MainWindow.OnReadyToShow += () => MainWindow.Show();
    }

    /// <summary>
    /// Open a Dialog that let user select a folder from windows
    /// </summary>
    /// <returns>The selected folder or null if nothing has been selected</returns>
    public static async Task<string> OpenSelectFolderDialog()
    {
        string[] result = await ElectronNET.API.Electron.Dialog.ShowOpenDialogAsync(
            MainWindow, 
            new OpenDialogOptions() { 
                Properties = new []
                {
                    OpenDialogProperty.openFile, 
                    OpenDialogProperty.openDirectory
                },
                Title = "Dossier de destination : export CSV",
                ButtonLabel = "Valider",
            });

        // Return the selected folder if user selected one
        return result.Any() ? result[0] : null;
    }
}