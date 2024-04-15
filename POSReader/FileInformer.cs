namespace ConsoleApp1;

using System;
using System.Collections.Generic;
using System.IO;

public static class FileInformer
{
    public static string WorkingDirectory = Environment.CurrentDirectory;
    public static string SlnDirectory = Directory.GetParent(WorkingDirectory).Parent.Parent.FullName;
    private static readonly string FilePath = Path.Combine(SlnDirectory, "brown", "cats.txt");

    public static int NrIntrari;
    // Dictionary to store categories (key-value pairs)
    public static Dictionary<string, string> FileCategoryDictionary;

    public static String[] Files;
    public static String[] Categories;
    public static List<string> TrainingFiles;
    public static List<string> TestingFiles;

    public static int ProcentajAntrenament = 60;

    public static string FilePathPrefix = Path.Combine(SlnDirectory, "brown") + "/";

    // Static constructor to load categories when the class is first used
    static FileInformer()
    {
        LoadCategories();
    }

    private static void LoadCategories()
    {
        FileCategoryDictionary = new Dictionary<string, string>();
        
        try
        {
            string[] lines = File.ReadAllLines(FilePath);
            NrIntrari = lines.Length;
            foreach (string line in lines)
            {
                
                string[] parts = line.Split(' ');

                if (parts.Length >= 2)
                {
                    string key = FilePathPrefix + parts[0].Trim();
                    string value = parts[1].Trim();
                    FileCategoryDictionary[key] = value;
                    
                }
                else
                {
                    Console.WriteLine($"[Eroare]: Eroare la citirea liniei: {line}");
                }
            }

            Files = FileCategoryDictionary.Keys.ToArray();
            Categories = FileCategoryDictionary.Values.ToArray();

            int nrFisiereDeSelectat = (int)(Files.Length * (ProcentajAntrenament / 100.0));

            GetRandomFilesWithEqualDistribution(FileCategoryDictionary, nrFisiereDeSelectat, out TrainingFiles, out TestingFiles,randomDistribution:false);

            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Eroare]: Eroare la citirea categoriilor: {ex.Message}");
        }
        Console.WriteLine("[Informatie]: A avut loc cu succes citirea categoriilor\n");
    }

    public static Dictionary<string, string> GetCategories()
    {
        return FileCategoryDictionary;
    }
    
    
    static void GetRandomFilesWithEqualDistribution(Dictionary<string, string> fileCategories,
        int numberOfFilesToSelect, out List<string> trainingFilesList, out List<string> testingFilesList, bool randomDistribution = false)
    {
        if (randomDistribution == true)
        {  
            Random rnd = new Random();

            // Get the total number of files
            int totalFiles = fileCategories.Count;

            // Calculate the number of files for training based on the percentage
            int trainingCount = (int)(totalFiles * ProcentajAntrenament);

            // Randomly select files for training
            trainingFilesList = fileCategories.Keys.OrderBy(x => rnd.Next()).Take(trainingCount).ToList();

            // The remaining files are for testing
            testingFilesList = fileCategories.Keys.Except(trainingFilesList).ToList();

        }
        // Group filenames by category
        var groupedByCategory = fileCategories.GroupBy(kv => kv.Value)
            .ToDictionary(g => g.Key, g => g.Select(kv => kv.Key).ToList());

        // Determine the minimum number of files per category
        int minFilesPerCategory = numberOfFilesToSelect / groupedByCategory.Count;

        // Randomly select filenames from each category
        Random random = new Random();
        trainingFilesList = new List<string>();

        foreach (var categoryFiles in groupedByCategory.Values)
        {
            // Ensure that the minimum number of files per category is selected
            int filesToSelect = Math.Min(minFilesPerCategory, categoryFiles.Count);

            // Randomly select files from the category
            trainingFilesList.AddRange(categoryFiles.OrderBy(f => random.Next()).Take(filesToSelect));
        }

        // If needed, randomly select additional files to meet the specified count
        while (trainingFilesList.Count < numberOfFilesToSelect)
        {
            var remainingFiles = fileCategories.Keys.Except(trainingFilesList);
            trainingFilesList.Add(remainingFiles.ElementAt(random.Next(remainingFiles.Count())));
        }

        testingFilesList = new List<string>();
        testingFilesList = fileCategories.Keys.Except(trainingFilesList).ToList();
    }

}