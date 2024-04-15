using System.Text.RegularExpressions;

namespace ConsoleApp1;

public static class PosReader
{
    
    const string Separator = "#";
    public const string ErrorString = "!Error!";
    const string StringEroareCautare = "NOT_FOUND";
    public static int NrIntrari = 0;
    
    
    public static HashSet<string> PartiVorbire = new();

    
    static string[] ReturnFilesByPercent(string[] stringArray, int percentage)
    {
        int numStringsToSelect = (int)(stringArray.Length * (percentage / 100));

        var random = new Random();
        string[] selectedStrings = stringArray.OrderBy(s => random.Next()).Take(numStringsToSelect).ToArray();

        return selectedStrings;
    }
    public static Dictionary<string,int> CitesteFisiereleBrownCorpus(string directoryPath)
    {
        try
        {
            if (Directory.Exists(directoryPath))
            {
                //string[] files = Directory.GetFiles(directoryPath);
                //var filteredFilePaths = PosReader.ReturneazaFisiereleFiltrate(files);

                
                Dictionary<string, int> dictionarBrownCorpus = new(50000);
                
                foreach (string filePath in FileInformer.TrainingFiles)
                { 
                    string contentFisier = CitesteDocumentInString(filePath: filePath);
                    TokenizeazaInDictionar(contentFisier, dictionarBrownCorpus);
                }
                return dictionarBrownCorpus;
            }
            else
            {
                Console.WriteLine("[Eroare]: Directorul {0} nu exista.",directoryPath);
                return new Dictionary<string, int>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Eroare]: Eroare la directorul {0}: {1}",directoryPath, ex);
        }
        return new Dictionary<string, int>();
    }
    
    
    
    public static string CitesteDocumentInString(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string documentContent = File.ReadAllText(filePath);
                return documentContent;
            }
            else
            {
                Console.WriteLine("[Eroare]: Fisierul {0} nu exista!.",filePath);
                return ErrorString;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Eroare]: Eroare la citirea fisierului {0}: {1}",filePath, ex);
            return ErrorString;
        }
    }
    
    
    public static string ImbinaWordSiPart(string word, string part)
    {
        return $"{word}{Separator}{part}";
    }
     
    public static string ReturneazaCuvantDictionar(string intrare)
    {
        int hashIndex = intrare.IndexOf("#");
        return intrare.Substring(0, hashIndex);
    }
     
    public static string ReturneazaParteDictionar(string intrare)
    {
        int hashIndex = intrare.IndexOf("#");
        return intrare.Substring(hashIndex, intrare.Length);
    }
    public static void DescifreazaIntrare(KeyValuePair<string, int> intrare, out string cuvant, out string parte, out int intrari, out string parteNedescifrata)
    {
        string[] words = intrare.Key.Split('#');
        cuvant = words[0];
        parte = DescifreazaParte(words[1]);
        parteNedescifrata = words[1];
        intrari = intrare.Value;
    }
 
    public static string DescifreazaParte(string part)
    {
        if (!string.IsNullOrEmpty(part))
        {
            PartiVorbire.Add(part);
            if (ReaderDictionar.Dictionar.TryGetValue(part, out var parte))
            {
                return parte;
            }
            return "tag exception: "+part;
        }
        return StringEroareCautare;
    }
    public static string[] CopiazaCheiInDictionar(Dictionary<string, int> originalDict)
    {
        string[] stringArray = new string[originalDict.Count];
        int index = 0;
     
        foreach (var kvp in originalDict)
        {
            stringArray[index] = kvp.Key;
            index++;
        }
     
        return stringArray;
    }

    public static int ContorStopWordsNumarDetectate = 0;
    public static int ContorStopWordsAlteleDetectate = 0;
    public static int ContorIntrariTotale = 0;

    public static double ReturneazaRaportStopWords()
    {
        return (double)  (ContorStopWordsAlteleDetectate + ContorStopWordsNumarDetectate) / ContorIntrariTotale ;
    }
    public static void TokenizeazaInDictionar(string fileContent, Dictionary<string,int> dictionar)
    {
 
        const string pattern = @"(\S+)/(\S+)(?:\s|$)";
 
        MatchCollection matches = Regex.Matches(fileContent, pattern);
        
        foreach (Match match in matches)
        {
            string word = match.Groups[1].Value;
            string part = match.Groups[2].Value;
            ContorIntrariTotale++;
            if (StopWords.HashSetStopWords.Contains(word) || StopWords.EsteNumar(word))
            {
                if (StopWords.EsteNumar(word))
                {
                    ContorStopWordsNumarDetectate++;
                }
                else
                {
                    ContorStopWordsAlteleDetectate++;
                }
                continue;
            }
            NrIntrari++;
            string wordPart = ImbinaWordSiPart(word, part);
        
            if (dictionar.ContainsKey(wordPart))
            {
                dictionar[wordPart] += 1;
            }
            else
            {
                dictionar.Add(wordPart,1);
            }

        }
    }

     
    public static bool ContineSemnePunctuatie(string input)
    {
        string pattern = @"[\p{P}]";
     
        return Regex.IsMatch(input, pattern);
    }
    
    public static string[] ReturneazaFisiereleFiltrate(string[] files)
    {
        var filteredFiles = files.Where(file =>
            Path.GetFileName(file).StartsWith("C", StringComparison.OrdinalIgnoreCase) &&
            char.IsDigit(Path.GetFileNameWithoutExtension(file).LastOrDefault())
        ).ToArray();
        return filteredFiles;
    }


}