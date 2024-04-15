using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection.Metadata;

namespace ConsoleApp1;

public static class BNCClasifier
{

    public static int IndexAdverb = 0;
    public static int IndexIpc = 1;
    public static int IndexArticol = 2;
    public static int IndexSubstantiv = 3;
    public static int IndexPronume = 4;
    public static int IndexVerb = 5;
    public static int IndexAdjectiv = 6;
    public static int IndexPunctuatie = 7;
    public static int IndexNespecificat = 8;
    public static int IndexNiciuna = 9;

    public static int[,] MatriceNaiveBayes = new int[10, 10];
    public static ConcurrentDictionary<KeyValuePair<string, string>, int[,]> BncDictionary = new
        ConcurrentDictionary<KeyValuePair<string, string>, int[,]>();

    public const string ErrorString = "!Error!";
    
    public static Dictionary<string,int> AntreneazaClasificatorBNC(string directoryPath)
    {
        try
        {
            if (Directory.Exists(directoryPath))
            {
                //string[] files = Directory.GetFiles(directoryPath);
                //var filteredFilePaths = PosReader.ReturneazaFisiereleFiltrate(files);

                
                Dictionary<string, int> dictionarBrownCorpus = new(50000);

                Parallel.ForEach(FileInformer.TrainingFiles, filePath =>
                {
                    string contentFisier = CitesteDocumentInString(filePath: filePath);
                    TokenizeazaSiAntreneazaBNC(contentFisier);
                });
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
    
    public static void TokenizeazaSiAntreneazaBNC(string fileContent)
    {
        // Split the text into sentences
        var sentences = BreakTextIntoSentences(fileContent);

        string pattern = @"(\S+)/(\S+)(?:\s|$)";
        foreach (var sentence in sentences)
        {
            MatchCollection matches = Regex.Matches(sentence, pattern);

            string previousPart = "niciuna";
            string previousWord = "";
            foreach (Match match in matches)
            {
                string word = match.Groups[1].Value;
                var part = match.Groups[2].Value;

                if (StopWords.HashSetStopWords.Contains(word)) continue;

                var kvp = new KeyValuePair<string, string>(previousWord, word);
                var parteCurenta = PosTranslator.ReturneazaParteaDeVorbireSimplificata(part);
                
                int indexCur = ReturneazaIndexParte(parteCurenta);
                int indexPrev = ReturneazaIndexParte(previousPart);

                if (BncDictionary.ContainsKey(kvp))
                {
                    BncDictionary[kvp][indexCur, indexPrev]++;
                }
                else
                {
                    BncDictionary.TryAdd(kvp, new int[10, 10]);
                    BncDictionary[kvp][indexCur, indexPrev]++;
                }
                
                MatriceNaiveBayes[indexPrev, indexCur]++;
                
                previousPart = parteCurenta;
                previousWord = word;
            }
        }
    }

    
// Helper method to break text into sentences
    private static IEnumerable<string> BreakTextIntoSentences(string text)
    {
        // Use a simple regex to split sentences based on common punctuation marks
        string[] sentenceDelimiters = { ".", "!", "?" };
        string pattern = "[" + string.Join("", sentenceDelimiters) + "]";
    
        return Regex.Split(text, pattern);
    }

    public static string ReturneazaPartePeBazaIndex(int index)
    {
        switch (index)
        {
            case 0:
                return "adverb";
            case 1:
                return  "interjectie/prepozitie/conjunctie";
            case 2:
                return  "articol";
            case 3:
                return  "substantiv";
            case 4:
                return  "pronume";
            case 5:
                return  "verb";
            case 6:
                return  "adjectiv";
            case 7:
                return  "punctuatie";
            case 8:
                return  "nespecificat";
            case 9:
                return  "niciuna";
        }

        return "niciuna";
    }
    
    public static int ReturneazaIndexParte(string parte)
    {
        if (parte.StartsWith("adv"))
            return 0;
        else
        if (parte.StartsWith("int"))
            return 1;
        else
        if (parte.StartsWith("art"))
            return 2;
        else
        if (parte.StartsWith("sub"))
            return 3;
        else
        if (parte.StartsWith("pro"))
            return 4;
        else
        if (parte.StartsWith("ver"))
            return 5;
        else
        if (parte.StartsWith("adj"))
            return 6;
        else
        if (parte.StartsWith("punc"))
            return 7;
        else
        if (parte.StartsWith("nes"))
            return 8;
        else
        if (parte.StartsWith("nic"))
            return 9;
        
        return 0;

    }
    
    
    
    public static string ReturneazaRezultatDictionar(string word, string previousWord, string previousPart)
    {
        if (BncDictionary.TryGetValue(new KeyValuePair<string, string>(word, previousWord), out var rezultatDictionar))
        {
            int indexPrev = ReturneazaIndexParte(previousPart);
            int indexMax = 100000;
            int max = 0;
            for (int i = 0; i < 10; ++i)
            {
                if (rezultatDictionar[i, indexPrev] > max)
                {
                    indexMax = i;
                    max = rezultatDictionar[i, indexPrev];
                }
            }

            return ReturneazaPartePeBazaIndex(indexMax);
            
        }
        else
        {
            //aici vedem ca nu a fost gasit, returnam alta parte de vb?


            return UtilitatiDictionar.ReturneazaParteaDeVorbireBazataPeContext(word, previousPart);
            //return EroareNegasit;
        }
    }
    
    
    
    public static readonly HashSet<string> SetPartiSimplificate =
        new()
        {
            "adverb",
            "interjectie/prepozitie/conjunctie",
            "articol",
            "substantiv",
            "pronume",
            "verb",
            "adjectiv",
            "punctuatie",
            "nespecificat",
            "niciuna"
        };
}