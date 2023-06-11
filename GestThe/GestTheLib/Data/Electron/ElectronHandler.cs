/**
* ETML
* Author : Sébastien Duruz
* Date : 15.05.2023
* Description : ElectronNET methods used by the main project.
*/

using ElectronNET.API;
using ElectronNET.API.Entities;

namespace GestTheLib.Data.Electron;

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

        // Make sure the Window is on a valid display bounds
        await ValidateWindowPosition();
        
        // Setup the Electron Window
        MainWindow = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(
            new BrowserWindowOptions()
            {
                Title = "GestThe",
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
        MainWindow.OnMove += async () => await SaveMainWindowSizeAndPosition();
        MainWindow.OnResize += async () => await SaveMainWindowSizeAndPosition();
        MainWindow.OnClose += SettingsReader.WriteSettings;
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
    /// Save the current Electron values to the settings
    /// </summary>
    private static async Task SaveMainWindowSizeAndPosition()
    {
        // Get the current value from MainWindow
        int[] mainWindowSize = await MainWindow.GetSizeAsync();
        int[] mainWindowPosition = await MainWindow.GetPositionAsync();

        // Set the new values
        SettingsReader.SettingsValues.WindowWidth = mainWindowSize[0] - 14;
        SettingsReader.SettingsValues.WindowHeight = mainWindowSize[1] - 8;
        SettingsReader.SettingsValues.WindowLeft = mainWindowPosition[0];
        SettingsReader.SettingsValues.WindowTop = mainWindowPosition[1];
    }

    /// <summary>
    /// Validate that the application is on the bounds of the displays
    /// <returns>True if valid position, false if not</returns>
    /// </summary>
    private static async Task<bool> ValidateWindowPosition()
    {
        bool windowPositionIsValid = false;
        
        // Get the connected displays
        Display[] displays = await ElectronNET.API.Electron.Screen.GetAllDisplaysAsync();
        
        // check Window positions
        foreach (Display display in displays)
        {
            if (display.Bounds.X <= SettingsReader.SettingsValues.WindowLeft
                && display.Bounds.X + display.Bounds.Width >= SettingsReader.SettingsValues.WindowLeft
                && display.Bounds.Y <= SettingsReader.SettingsValues.WindowHeight
                && display.Bounds.Y + display.Bounds.Height >= SettingsReader.SettingsValues.WindowTop)
            {
                windowPositionIsValid = true;
                break;
            }
        }

        // Reset the position values for MainWindow
        if (!windowPositionIsValid)
        {
            SettingsReader.SettingsValues.WindowLeft = 100;
            SettingsReader.SettingsValues.WindowTop = 100;
            SettingsReader.WriteSettings();
        }

        // Return the result
        return windowPositionIsValid;
    }
}