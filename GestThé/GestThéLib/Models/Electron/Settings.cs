/**
 * ETML
 * Author : Sébastien Duruz
 * Date : 02.06.2023
 * Description : User Settings for the mainWindow
 */

namespace GestThéLib.Models.Electron;

/// <summary>
/// Class UserSettings
/// </summary>
public class Settings
{
    /// <summary>
    /// Width of the main Window
    /// </summary>
    public int WindowWidth { get; set; } = 900;

    /// <summary>
    /// Height of the main Window
    /// </summary>
    public int WindowHeight { get; set; } = 650;

    /// <summary>
    /// Default TOP position of the main Window
    /// </summary>
    public int WindowTop { get; set; } = 100;

    /// <summary>
    /// Default LEFT position of the main Window
    /// </summary>
    public int WindowLeft { get; set; } = 100;
}