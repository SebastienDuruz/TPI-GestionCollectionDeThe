/**
 * ETML
 * Author : Sébastien Duruz
 * Date : 26.05.2023
 * Description : Generate CSV with CsvHelper library.
 */

using System.Globalization;
using System.Text;
using CsvHelper;
using GestTheLib.Models.Database;

namespace GestTheLib.Data.CSV;

/// <summary>
/// Class CsvGenerator
/// </summary>
public class CsvGenerator
{

 /// <summary>
 /// Write a CSV file with given Tea elements
 /// </summary>
 /// <param name="teasToExport">The teas to export</param>
 /// <param name="folderPath">Folder path of the export (folder only)</param>
 public void WriteTeaExport(List<TTea> teasToExport, string folderPath = null)
 {
  // Set the export Path and Name of the file
  string fileName = $"thés-{DateTime.Now.ToFileTime()}.csv";
  string exportPath = folderPath == null
   ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName)
   : Path.Combine(folderPath, fileName);

  // Write the Teas to the destination file
  using (var writer = new StreamWriter(exportPath, false, Encoding.UTF8))
  using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
  {
   csv.WriteHeader<TTea>();
   csv.NextRecord();
   foreach (TTea tea in teasToExport)
   {
    csv.WriteRecord(tea);
    csv.NextRecord();
   }
  }
 }
 
 /// <summary>
 /// Write a CSV file with given Tea elements
 /// </summary>
 /// <param name="folderPath">File path of the export (folder only)</param>
 public void WriteListExport(TList listToExport, string folderPath = null)
 {
  // Set the name of the export files
  string listFileName = $"list-{listToExport.ListName}-{DateTime.Now.ToFileTime()}.csv";
  string teaFileName = $"list-{listToExport.ListName}-thés-{DateTime.Now.ToFileTime()}.csv";
  string fieldsFileName = $"list-{listToExport.ListName}-champs-{DateTime.Now.ToFileTime()}.csv";
  
  // Set the export Path and Name of the files
  string listExportPath = String.IsNullOrWhiteSpace(folderPath)
   ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), listFileName)
   : Path.Combine(folderPath, listFileName);
  string teaExportPath = String.IsNullOrWhiteSpace(folderPath)
   ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), teaFileName)
   : Path.Combine(folderPath, teaFileName);
  string fieldsExportPath = String.IsNullOrWhiteSpace(folderPath)
   ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fieldsFileName)
   : Path.Combine(folderPath, fieldsFileName);

  // Write the List to the destination file
  using (var writer = new StreamWriter(listExportPath, false, Encoding.UTF8))
  using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
  {
   csv.WriteHeader<TList>();
   csv.NextRecord();
   csv.WriteRecord(listToExport);
  }
  
  // Write the Teas of the list to the destination file
  using (var writer = new StreamWriter(teaExportPath, false, Encoding.UTF8))
  using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
  {
   csv.WriteHeader<TTea>();
   csv.NextRecord();

   foreach (TTea tea in listToExport.IdTeas)
   {
    csv.WriteRecord(tea);
    csv.NextRecord();    
   }
  }
  
  // Write the Fields of the list to the destination file
  using (var writer = new StreamWriter(fieldsExportPath, false, Encoding.UTF8))
  using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
  {
   csv.WriteHeader<TField>();
   csv.NextRecord();

   foreach (TField field in listToExport.IdFields)
   {
    csv.WriteRecord(field);
    csv.NextRecord();    
   }
  }
 }
}