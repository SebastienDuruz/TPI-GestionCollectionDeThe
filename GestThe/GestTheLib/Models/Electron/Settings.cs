﻿/**
 * ETML
 * Author : Sébastien Duruz
 * Date : 02.06.2023
 * Description : User Settings for the mainWindow
 */

namespace GestTheLib.Models.Electron;

/// <summary>
/// Class UserSettings
/// </summary>
public class Settings
{
    /// <summary>
    /// Width of the main Window
    /// </summary>
    public int WindowWidth { get; set; } = 1200;

    /// <summary>
    /// Height of the main Window
    /// </summary>
    public int WindowHeight { get; set; } = 720;

    /// <summary>
    /// Default TOP position of the main Window
    /// </summary>
    public int WindowTop { get; set; } = 100;

    /// <summary>
    /// Default LEFT position of the main Window
    /// </summary>
    public int WindowLeft { get; set; } = 100;
}