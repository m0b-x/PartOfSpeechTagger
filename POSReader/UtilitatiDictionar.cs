using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1;

public static class UtilitatiDictionar
{
    
    public const string SeparatorParteNr = "-";
    public const string SeparatorCombinatie = "#";
    public const string EroareNegasit = "NEGASIT";
    public static Dictionary<string, string> DictionarInterjectii = new()
    {
        {"ah", "interjectie/prepozitie/conjunctie"},
        {"oh", "interjectie/prepozitie/conjunctie"},
        {"oops", "interjectie/prepozitie/conjunctie"},
        {"wow", "interjectie/prepozitie/conjunctie"},
        {"yikes", "interjectie/prepozitie/conjunctie"},
        {"aha", "interjectie/prepozitie/conjunctie"},
        {"haha", "interjectie/prepozitie/conjunctie"},
        {"huh", "interjectie/prepozitie/conjunctie"},
        {"eek", "interjectie/prepozitie/conjunctie"},
        {"ouch", "interjectie/prepozitie/conjunctie"},
        {"hooray", "interjectie/prepozitie/conjunctie"},
        {"alas", "interjectie/prepozitie/conjunctie"},
        {"ugh", "interjectie/prepozitie/conjunctie"},
        {"yay", "interjectie/prepozitie/conjunctie"},
        {"phew", "interjectie/prepozitie/conjunctie"},
        {"well", "interjectie/prepozitie/conjunctie"},
        {"hey", "interjectie/prepozitie/conjunctie"},
        {"hmm", "interjectie/prepozitie/conjunctie"},
        {"darn", "interjectie/prepozitie/conjunctie"},
        {"gosh", "interjectie/prepozitie/conjunctie"},
        {"duh", "interjectie/prepozitie/conjunctie"},
        {"shh", "interjectie/prepozitie/conjunctie"},
        {"shoo", "interjectie/prepozitie/conjunctie"},
        {"uh-oh", "interjectie/prepozitie/conjunctie"},
        {"hmmm", "interjectie/prepozitie/conjunctie"},
        {"eureka", "interjectie/prepozitie/conjunctie"},
        {"bravo", "interjectie/prepozitie/conjunctie"},
        {"ooh", "interjectie/prepozitie/conjunctie"},
        {"heavens", "interjectie/prepozitie/conjunctie"},
        {"shit", "interjectie/prepozitie/conjunctie"},
        {"bullshit", "interjectie/prepozitie/conjunctie"},
        {"ahem", "interjectie/prepozitie/conjunctie"},
        {"golly", "interjectie/prepozitie/conjunctie"}
    };

    public static Dictionary<string, string> DictionarPreVerb = new()
    {
        {"am", "verb"},
        {"is", "verb"},
        {"are", "verb"},
        {"will", "verb"},
        {"had", "verb"},
        {"has", "verb"}
    };
    public static Dictionary<string, string> DictionarPreAdjectiv = new()
    {
        {"being", "adjectiv"},
    };

    
    public static HashSet<string> ReturneazaToatePartileDeVorbireDinDictionar(Dictionary<string, int> dictionar)
    {
        HashSet<string> partiVorbire = new();
        foreach (var intrare in dictionar)
        {
            PosReader.DescifreazaIntrare(intrare, cuvant: out var cuvant, parte: out var parte, intrari: out var intrari, out var _);
            partiVorbire.Add(parte);
        }

        return partiVorbire;
    }

    public static void AfiseazaCuvinteCuCountDinDictionar(Dictionary<string,string> dictionarDeCautare, int nrMinimIntrari = 2)
    {
        
        foreach (var kvp in dictionarDeCautare)
        {
            int count = kvp.Value.Count(c => c == '#');
            bool containsTwoHashes = count >= nrMinimIntrari;
    
            if(containsTwoHashes)
                Console.WriteLine(kvp.Key+" "+kvp.Value);
        }
    }

    public static int ContorNefolosire = 0;
    public static int negasitInDictionar = 0;
    public static string ReturneazaRezultatDictionar(Dictionary<string, string> dictionar, string cautare, string previousPart)
    {
        if (dictionar.TryGetValue(cautare, out var rezultatDictionar))
        {
            return rezultatDictionar;
        }
        else
        {
            negasitInDictionar++;
            //aici vedem ca nu a fost gasit, returnam alta parte de vb?

            return ReturneazaParteaDeVorbireBazataPeContext(cautare, previousPart);
            //return EroareNegasit;
        }
    }
    public static string ReturneazaParteaDeVorbireBazataPeContext(string cautare, string previousPart, string previousWord = "")
    {
        
        ContorNefolosire++;
        if (IsAllDigits(cautare))
        {
            return "substantiv";
        }
        if (IsOnlyPunctuation(cautare) || IsOnlyPunctuation(cautare))
        {
            return "punctuatie";
        }
        if(cautare.EndsWith("tion") ||
           cautare.EndsWith("ness") ||
           cautare.EndsWith("ity")  ||
           cautare.EndsWith("ment") )
        {
            return "substantiv";
        }
        if(cautare.EndsWith("ed"))
        {
            if (previousPart.Equals("verb") || DictionarPreAdjectiv.ContainsKey(previousWord))
                return "adjectiv";
            return "verb";
        }
        if(cautare.EndsWith("ing"))
        {
            if (previousPart.Equals("articol") || previousPart.Equals("adjectiv"))
                return "substantiv";
            return "verb";
        }
        if (cautare.EndsWith("able")  ||
            cautare.EndsWith("ible") ||
            cautare.EndsWith("al")   ||
            cautare.EndsWith("ful") ||
            cautare.EndsWith("ic") ||
            cautare.EndsWith("ous"))
        {
            return "adjectiv";
        }
        if (cautare.StartsWith("un")  ||
            cautare.StartsWith("non") ||
            cautare.StartsWith("in")  ||
            cautare.StartsWith("im"))
        {
            if (previousPart.Equals("verb"))
                return "adjectiv";
        }
        if (cautare.EndsWith("ly"))
        {
            if (DictionarPreAdjectiv.ContainsKey(previousWord))
                return "adjectiv";
            return "adverb";
        }
        var toLowerWord = cautare.ToLower();
        if (DictionarInterjectii.ContainsKey(toLowerWord))
        {
            return "interjectie/prepozitie/conjunctie";
        }
        if (cautare == cautare.ToUpper())
        {
            return "substantiv";
        }
        if(IsOrdinalNumber(cautare))
            return "adjectiv";
        
        if (IsAdjectivalCompoundNoun(cautare))
            return "adjectiv";
        if (cautare.Contains("-positive") || cautare.Contains("-negative"))
            return "adjectiv";
        
        return "substantiv";
    }
    
    
    static bool IsAdjectivalCompoundNoun(string phrase)
    {
        // Define a regular expression pattern for adjectival compound nouns
        string pattern = @"^\d+-\w+$";

        // Use Regex.IsMatch to check if the phrase matches the pattern
        return Regex.IsMatch(phrase, pattern);
    }
    static bool IsOrdinalNumber(string word)
    {
        // Define a regular expression to match ordinal numbers
        string pattern = @"^\d+(st|nd|rd|th)$";

        // Use Regex.IsMatch to check if the word matches the pattern
        return Regex.IsMatch(word, pattern, RegexOptions.IgnoreCase);
    }
    static bool IsOnlyPunctuation(string inputStr)
    {
        // Check if all characters in the input string are punctuation characters
        return inputStr.All(char.IsPunctuation);
    }
    static bool IsNonAlphanumeric(string word)
    {
        // Use a regular expression to check if the word contains characters other than letters or digits
        // ^ means the start of the string, [a-zA-Z0-9] means any letter or digit
        // You can customize the pattern based on your specific requirements
        string pattern = @"^[a-zA-Z0-9]+$";

        return !Regex.IsMatch(word, pattern);
    }
    static bool IsAllDigits(string inputStr)
    {
        if (int.TryParse(inputStr, out _))
        {
            // Parsing succeeded, which means the input is an integer
            return true;
        }
        else
        {
            // Parsing failed, indicating the presence of non-digit characters
            return false;
        }
    }

    public static string AranjeazaRezultat(string rezultat)
    {
        return rezultat.Replace("#", ", ") + ";";
    }
        
    
    public static List<KeyValuePair<string, int>> ConvertestePartiInKVP(string combinatiiParti)
    {
        var combinatiiPartiSiNr = combinatiiParti.Split(SeparatorCombinatie);
        List<KeyValuePair<string, int>> listaPartiSiNr = new();
        //adunam parti
        foreach (var combinatieParteSiNr in combinatiiPartiSiNr)
        {
            if (!String.IsNullOrEmpty(combinatieParteSiNr))
            {
                var partiSiNrArray = combinatieParteSiNr.Split(SeparatorParteNr);
                var parte = partiSiNrArray[0];
                var nr = Int32.Parse(partiSiNrArray[1]);
                listaPartiSiNr.Add(new KeyValuePair<string, int>(parte, nr));
            }
        }

        return listaPartiSiNr;
    }

    public static StringBuilder AfiseazaRezultatKVP(string cuvant, List<KeyValuePair<string, int>> rezultate)
    {
        StringBuilder stringRezultat = new();
        stringRezultat.Append($"Rezultate pt {cuvant}: ");

        foreach (var kvp in rezultate)
        {
            stringRezultat.Append($"{kvp.Key} - {kvp.Value} instante, ");
        }
        
        stringRezultat.Remove(stringRezultat.Length - 2, 2);
        stringRezultat.Append(";");
        
        return stringRezultat;
    }

    static Dictionary<string, string> CitesteDictionarDeCautare(string filePath)
    {
        Dictionary<string, string> dictionarCautare = new Dictionary<string, string>();

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string linie;
                while ((linie = reader.ReadLine()) != null)
                {
                    if (linie.Contains("Cuvant:") && linie.Contains("Parti-Intrari:"))
                    {
                        // Extragere cuvant si pos prin indexing
                        int indexWordStart = linie.IndexOf("~WORD~") + "~WORD~".Length;
                        int indexWordSfarsit = linie.IndexOf("; Parti-Intrari:");
                        string cuvant = linie.Substring(indexWordStart, indexWordSfarsit - indexWordStart).Trim();

                        int indexPosStart = linie.IndexOf("~PARTSOFSPEECH~") + "~PARTSOFSPEECH~".Length;
                        int indexPosSfarsit = linie.IndexOf("~", indexPosStart);
                        string parteVorbire = linie.Substring(indexPosStart, indexPosSfarsit - indexPosStart).Trim();

                        // Adaugare intrare dictionar
                        dictionarCautare[cuvant] = parteVorbire;
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"[Eroare]: Fisierul '{filePath}' nu exista.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Eroare]: {ex.Message}");
        }

        return dictionarCautare;
    }
}