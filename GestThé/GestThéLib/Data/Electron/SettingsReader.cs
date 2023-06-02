/**
 * ETML
 * Author : Sébastien Duruz
 * Date : 02.06.2023
 * Description : SettingsReader for the MainWindow
 */

using GestThéLib.Models.Electron;
using Newtonsoft.Json;

namespace GestThéLib.Data.Electron;

/// <summary>
/// Class SettingsReader
/// </summary>
public class SettingsReader
{
    /// <summary>
    /// Name of the folder to use
    /// </summary>
    private static string FOLDER_NAME = "GestThe";

    /// <summary>
    /// Name of the file to use
    /// </summary>
    private static string FILE_NAME = "settings.json";
    
    /// <summary>
    /// The values of the settings
    /// </summary>
    public Settings SettingsValues { get; set; }
    
    /// <summary>
    /// Full filepath of the settings file
    /// </summary>
    private string FilePath { get; set; }
    
    public SettingsReader()
    {
        // Create the folder if it doesn't exists
        if(!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FOLDER_NAME)))
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FOLDER_NAME));
            
        // Get the full path to use
        FilePath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FOLDER_NAME), FILE_NAME);
        
        // Read the initial Settings values
        ReadSettings();
    }
    
    /// <summary>
    /// Read the Settings json file, create it if not exists
    /// Load the content into Settings Object
    /// </summary>
    public void ReadSettings()
    {
        if(File.Exists(FilePath))
        {
            try
            {
                SettingsValues = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FilePath));
            }
            catch (Exception ex)
            {
                // Reset the settings by recreating a file
                SettingsValues = new Settings();
                WriteSettings();
            }
        }
        else
        {
            SettingsValues = new Settings();
            WriteSettings();
        }
    }
    
    /// <summary>
    /// Write the Settings object to json file
    /// </summary>
    public void WriteSettings()
    {
        try
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(SettingsValues));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}