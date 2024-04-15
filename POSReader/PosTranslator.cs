using System.Text;

namespace ConsoleApp1;

public static class PosTranslator
{
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
            "nespecificat"
        };
    
    public const string SeparatorParteNr = "-";
    public const string SeparatorCombinatie = "#";
    public static readonly HashSet<string> HashSetExceptiiInceput = new HashSet<string>
        (new[] { "fw-", "nc-", "tl-" });

    public static readonly Dictionary<string, string> DictionarConversieManuala =
        new(StringComparer.OrdinalIgnoreCase)
        {
            {"nn", "substantiv"},
            {"cd", "substantiv"}, //cardinal
            {"nr", "adverb"},
            {"rn", "adverb"},
            {"rb", "adverb"},
            {"rp", "adverb"},
            {"at", "articol"},
            {"ap", "adverb"}, // multi/cativa
            {"ab", "adverb"}, // pre cuantificatori
            {"ex", "pronume"}, // existentil there, pronume introductiv
            {"np", "substantiv"}, //posesiv
            {"wp", "pronume"},
            {"pn", "pronume"},
            {"pp", "pronume"},
            {"vb", "verb"},
            {"hv", "verb"},
            {"md", "verb"},
            {"to", "verb"},
            {"be", "verb"},
            {"do", "verb"},
            {"did", "verb"},
            {"does", "verb"},
            {"jj", "adjectiv"},
            {"od", "adjectiv"}, // first,second,etc
            {"dt", "adjectiv"}, // determinator
            {"ql", "adjectiv"}, // calificator
            {"hr", "adjectiv"}, //who, why? adj relativ
            {"uh", "interjectie/prepozitie/conjunctie"}, //interjectie
            {"cc", "interjectie/prepozitie/conjunctie"},
            {"cs", "interjectie/prepozitie/conjunctie"},
            {"in", "interjectie/prepozitie/conjunctie"},
            {".", "punctuatie"},
            {"(", "punctuatie"},
            {")", "punctuatie"},
            {"*", "punctuatie"},
            {"--", "punctuatie"},
            {",", "punctuatie"},
            {":", "punctuatie"},
            {"nil", "nespecificat"},
            {"tl", "exceptie"},
            {"fw", "exceptie"},
            {"nc", "exceptie"}
        };

    
    static readonly char[] Separatori = { '-', '+' };
    
    public static Dictionary<string, int> SimplificaDictionar(Dictionary<string, int> dictionarIntrare)
    {
        Dictionary<string, int> dictionarIesire = new Dictionary<string, int>(dictionarIntrare.Count);
        foreach (var kvp in dictionarIntrare)
        {
            PosReader.DescifreazaIntrare(kvp, cuvant: out var cuvant, parte: out var parteVeche, intrari: out var intrari, out var parteNedescifrata);
            var parteNoua = ReturneazaParteaDeVorbireSimplificata(parteNedescifrata);
            var cheieNoua = PosReader.ImbinaWordSiPart(cuvant, parteNoua);
            
            dictionarIesire.TryAdd(cheieNoua, kvp.Value);
        }

        return dictionarIesire;
    }




    public static string ReturneazaParteaDeVorbireSimplificata(string parte)
    {
        var parteSchimbata = parte;

        //stergem tagurile fw/tl/nc de la inceput
        foreach (string exceptie in HashSetExceptiiInceput)
        {
            if (!parteSchimbata.StartsWith(exceptie)) continue;
            parteSchimbata = parteSchimbata.Substring(exceptie.Length);
            break;
        }
        
        // Triere semne - + (luam doar prima parte)
        var index = parteSchimbata.IndexOfAny(Separatori);

        if (index != -1)
        {
            parteSchimbata = parteSchimbata.Substring(0, index);
        }
        else
        {
            //parteSchimbata = parteSchimbata;
        }

        //daca contine caratere nealfabetice(doar *,$) sterge caractere ne-alphabetice
        //exceptie daca e numai *, atunci e not
        if (parteSchimbata.Length > 1)
        {
            parteSchimbata = parteSchimbata.Replace("$", "").Replace("*", "");
        }

        //daca contine punctuatie sau are `` il notam cu .
        if (PosWriter.ContineDoarPunctuatie(parteSchimbata) || parteSchimbata.Contains("``"))
        {
            parteSchimbata = PosWriter.SemnPunctuatie;
        }
        //Codul pana mai sus face sa fie 71 de parti de vb

        foreach (var cuvant in PosTranslator.DictionarConversieManuala)
        {
            if (parteSchimbata.Contains(cuvant.Key))
            {
                return cuvant.Value;
            }
        }
        return $"EROARE{parte}";
    }
    
    
    public static void ScrieInFisierDate(string pathFisier, Dictionary<string, int> dictionar)
    {
        string filePath = pathFisier;
 
        try
        {
            // Create the text document
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var intrare in dictionar)
                {
                    PosReader.DescifreazaIntrare(intrare, cuvant: out var cuvant, parte:  out var parte, intrari:  out var intrari, out var parteNedescifrata);
                    writer.WriteLine("Cuvant: ~{0}~; Parte expl.: ~{1}~; Nr. intrari: ~{2}~",cuvant, parteNedescifrata, intrari);
                }
            }
 
            Console.WriteLine($"[Informatie]: A avut loc cu succes scrierea in fisier {filePath}\n");
        }
        catch (IOException e)
        {
            Console.WriteLine($"[Eroare]: Eroare la scriere: {e.Message}");
        }
    }
    
    
    
    //Dictionar de cautare
    

    public static Dictionary<string, string> ConvertesteDictionarInDictionarDeCautare(Dictionary<string, int> dictionarOriginal)
    {
        Dictionary<string, string> dictionarDeCautare = new Dictionary<string, string>();

        foreach (var kvp in dictionarOriginal)
        {
            string[] combinatieCuvantParte = kvp.Key.Split(SeparatorCombinatie);
            string cuvant = combinatieCuvantParte[0];
            string parte = combinatieCuvantParte[1];

            // Build the value string using StringBuilder
            StringBuilder valueStringBuilder = new StringBuilder();
            valueStringBuilder.Append(parte);
            valueStringBuilder.Append(SeparatorParteNr);
            valueStringBuilder.Append(kvp.Value);

            string valueString = valueStringBuilder.ToString();

            if (dictionarDeCautare.TryGetValue(cuvant, out var existingValue))
            {
                dictionarDeCautare[cuvant] = $"{existingValue}{valueString}{SeparatorCombinatie}";
            }
            else
            {
                dictionarDeCautare[cuvant] = $"{valueString}{SeparatorCombinatie}";
            }
        }

        return dictionarDeCautare;
    }

    public static Dictionary<string, string>  ReturneazaDictionarDeCautareOrdonat(Dictionary<string, string> dictionarDeCautare)
    {
        Dictionary<string, string> dictionarOrdonat = new(dictionarDeCautare.Count);
        foreach (var kvp in dictionarDeCautare)
        {
            var combinatiiPartiSiNr = kvp.Value.Split(SeparatorCombinatie);
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
            //sortam lista
            listaPartiSiNr = UtilitatiListe.SorteazaListaKVPDupaValoriDesc(listaPartiSiNr);
            //transformam iar string-ul

            StringBuilder valueStringBuilder = new StringBuilder();
            foreach (var combinatieParteSiNr in listaPartiSiNr)
            {
                valueStringBuilder.Append(combinatieParteSiNr.Key);
                valueStringBuilder.Append(SeparatorParteNr);
                valueStringBuilder.Append(combinatieParteSiNr.Value);
                valueStringBuilder.Append(SeparatorCombinatie);
            }
            //stergem ultimul caracter
            valueStringBuilder.Length = valueStringBuilder.Length - 1;
            //schimbam valuarea dictionarului in cea sortata, adaugam in dictionar
            dictionarOrdonat.Add(kvp.Key,valueStringBuilder.ToString());
        }

        return dictionarOrdonat;
    }


    
    public static void ScrieDictionarDeCautareInFisier(string pathFisier, Dictionary<string, string> dictionarDeCautare)
    {
        string filePath = pathFisier;
 
        try
        {
            // Create the text document
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var kvp in dictionarDeCautare)
                {
                    writer.WriteLine("Cuvant: ~{0}~; Parti-Intrari: ~{1}~",kvp.Key, kvp.Value);
                }
            }
 
            Console.WriteLine($"[Informatie]: A avut loc cu succes scrierea in fisier {filePath}\n");
        }
        catch (IOException e)
        {
            Console.WriteLine($"[Eroare]: Eroare la scriere: {e.Message}");
        }
    }
    
    
    public static Dictionary<string,string> RealizeazaDictionarCuProcentaj(Dictionary<string, string> dictionarDeCautare)
    {
        Dictionary<string, string> dictionarPartiProcente = new(dictionarDeCautare.Count);
        foreach (var kvp in dictionarDeCautare)
        {
            int nrTotal = 0;
            var perechiPartiNr = kvp.Value.Split('#');
            var primulNr = Int32.Parse(perechiPartiNr[0].Split('-')[1]);
            
            foreach (var pereche in perechiPartiNr)
            {
                var combinatie = pereche.Split('-');
                var nr = Int32.Parse(combinatie[1]);
                nrTotal += nr;
            }

            StringBuilder combinatiiCuvantProcent = new();
            foreach (var pereche in perechiPartiNr)
            {
                var combinatie = pereche.Split('-');
                var parte = combinatie[0];
                var nr = Int32.Parse(combinatie[1]);
                double procent = (double) nr / nrTotal;
                combinatiiCuvantProcent.Append($"{parte}-{procent}#");
            }
            combinatiiCuvantProcent.Remove(combinatiiCuvantProcent.Length - 1, 1);
            dictionarPartiProcente.Add(kvp.Key, combinatiiCuvantProcent.ToString());
        }

        return dictionarPartiProcente;
    }

    public static int ContorApartineDictionar = 0;
    public static double ReturneazaProcentajDinDictionar(string cuvant, Dictionary<string,string> dictionar)
    {
        if (dictionar.TryGetValue(cuvant, out var dinDictionar))
        {
            return Double.Parse(dinDictionar.Split('#')[0].Split('-')[1]);
        }
        else
        {
            ContorApartineDictionar++;
            return -1;
        }
    }
    
}
