namespace ConsoleApp1;

public static class StopWords
{
    private const int NrStopWords = 1298; 
    public static HashSet<string> HashSetStopWords = new(capacity: NrStopWords);
    public static string WorkingDirectory = Environment.CurrentDirectory;
    public static string SlnDirectory = Directory.GetParent(WorkingDirectory).Parent.Parent.FullName;
    
    static StopWords()
    {
        string filePath = Path.Combine(SlnDirectory, "StopWords.txt");
        try
        {
            string[] words = File.ReadAllLines(filePath);

            foreach (string word in words)
            {
                HashSetStopWords.Add(word);
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("The file could not be found.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    public static bool EsteNumar(string cuvant)
    {
        return int.TryParse(cuvant, out _);
    }
}