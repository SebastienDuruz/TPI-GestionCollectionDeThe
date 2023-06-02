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
    /// Reader for the settings file
    /// </summary>
    private static SettingsReader SettingsReader { get; set; }
    
    /// <summary>
    /// Build the Electron Window
    /// </summary>
    public static async Task BuildElectronWindow()
    {
        SettingsReader = new SettingsReader();
        
        // Setup the Electron Window
        MainWindow = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(
            new BrowserWindowOptions()
            {
                Title = "GestThé",
                AutoHideMenuBar = true,
                Width = SettingsReader.SettingsValues.WindowWidth,
                Height = SettingsReader.SettingsValues.WindowHeight,
                X = SettingsReader.SettingsValues.WindowLeft,
                Y = SettingsReader.SettingsValues.WindowTop,
                Show = false
            });

        // Clear the cache before showing the window
        await MainWindow.WebContents.Session.ClearCacheAsync();

        // Add events to the Electron Window
        MainWindow.OnReadyToShow += () => MainWindow.Show();

        // Save the windowSettings before the MainWindow is closed
        await ElectronNET.API.Electron.IpcMain.On("saveWindowSettings", async (e) =>
        {
            await SaveMainWindowSettings();
            MainWindow.Destroy();
            ElectronNET.API.Electron.App.Exit();
        });
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
    
    /// <summary>
    /// Save the current Electron values to settings file
    /// </summary>
    private static async Task SaveMainWindowSettings()
    {
        // Get the current value from MainWindow
        int[] mainWindowSize = await MainWindow.GetSizeAsync();
        int[] mainWindowPosition = await MainWindow.GetPositionAsync();

        // Set the new values
        SettingsReader.SettingsValues.WindowWidth = mainWindowSize[0];
        SettingsReader.SettingsValues.WindowHeight = mainWindowSize[1];
        SettingsReader.SettingsValues.WindowLeft = mainWindowPosition[0];
        SettingsReader.SettingsValues.WindowTop = mainWindowPosition[1];

        // Write the new Settings
        SettingsReader.WriteSettings();
    }
}