namespace ConsoleApp1;

public static class PosWriter
{
    public static readonly String CaracterCombinatieParti = "-";
    public static readonly String CaracterSecundarCombinatieParti = "+";
    public static readonly String SemnPunctuatie = ".";
    
    public static Dictionary<string, string> DictionarSimplificareParti = new();
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
                    writer.WriteLine("Cuvant: ~{0}~; Parte:  ~{3}~; Parte expl.: ~{1}~; Nr. intrari: ~{2}~",cuvant, parte, intrari, parteNedescifrata);
                }
            }
 
            Console.WriteLine($"[Informatie]: A avut loc cu succes scrierea in fisier {filePath}\n");
        }
        catch (IOException e)
        {
            Console.WriteLine($"[Eroare]: Eroare la scriere: {e.Message}");
        }
    }
 
    public static void AfiseazaPartiVorbire()
    {
        Console.WriteLine(PosReader.PartiVorbire.Count+"\nLista Parti de vorbire:\n");
        int nrParte = 0;
        foreach (var parte in PosReader.PartiVorbire)
        {
            Console.WriteLine($"{nrParte++}. {parte}");
        }
    }

    public static HashSet<string> SimplificaPartileDDeVorbire()
    {
        //caractere ne-alfabetice
        HashSet<String> partiVorbirePrimaConversie = new(PosReader.PartiVorbire.Count());

        foreach (var parte in PosReader.PartiVorbire)
        {
            var parteSchimbata = parte;
            //contine macar 1 caracter ne alfabetice
            
            // Triere semne - + (luam doar prima parte)
            if (parteSchimbata.Contains(CaracterCombinatieParti))
            {
                parteSchimbata = parte.Split('-')[0];
                if (parteSchimbata.Equals(""))
                {
                    parteSchimbata = "-";
                }
            }
            if (parteSchimbata.Contains(CaracterSecundarCombinatieParti))
            {
                parteSchimbata = parte.Split('+')[0];
            }

            //daca contine caratere nealfabetice(doar *,$) sterge caractere ne-alphabetice
            //exceptie daca e numai *, atunci e not
            if (parteSchimbata.Length > 1)
            {
                parteSchimbata = parteSchimbata.Replace("$", "").Replace("*", "");
            }

            //daca contine punctuatie sau are `` il notam cu .
            if (ContineDoarPunctuatie(parteSchimbata) || parteSchimbata.Contains("``"))
            {
                DictionarSimplificareParti.TryAdd(parte, SemnPunctuatie);
                partiVorbirePrimaConversie.Add(SemnPunctuatie);
            }
            else
            {
                DictionarSimplificareParti.TryAdd(parte, parteSchimbata);
                partiVorbirePrimaConversie.Add(parteSchimbata);
            }
        }
        //Codul pana mai sus face sa fie 71 de parti de vb
        
        HashSet<String> partiVorbireADouaConversie = new(partiVorbirePrimaConversie.Count);

        foreach (var parte in partiVorbirePrimaConversie)
        {
            foreach (var cuvant in PosTranslator.DictionarConversieManuala)
            {
                if (parte.Contains(cuvant.Key))
                {
                    partiVorbireADouaConversie.Add(cuvant.Value);
                }
            }
        }
        //Cautare manuala
        return partiVorbireADouaConversie;
    }

    public static bool ContineDoarPunctuatie(string input)
    {
        foreach (var c in input)
        {
            if (!Char.IsPunctuation(c))
            {
                return false;
            }
        }

        return true;
    }

    public static void AfiseazaStatisticiPartiVorbire()
    {
        /*
           - the total number of distinct words in the corpus
           - the total number of distinct parts of speech in the corpus
           - the number of occurrences for each PoS separately
         */
        Console.WriteLine(PosReader.PartiVorbire.Count+"\nStatistici Parti de vorbire:\n");
        Console.WriteLine($"Combinatii distincte parti vorbire: {PosReader.PartiVorbire}");
        Console.WriteLine($"Combinatii parti vorbire: {PosReader.PartiVorbire}");
        Console.WriteLine($"NrTotalCuvinte: {PosReader.PartiVorbire}");
    }

    public static void AfiseazaDateDictionar(Dictionary<string, int> dictionarInitial, out Dictionary<string,int> dictionarStatistici, String numeDictionar = "Dictionar")
    {
        var array = PosReader.CopiazaCheiInDictionar(dictionarInitial);
        dictionarStatistici = new();
     
        foreach (var cuvant in array)
        {
            dictionarStatistici.Add(cuvant,0);
        }
     
        int nrSemnePunctuatie = 0;
        int nrSemnePunctuatieDistincte = 0;
        int nrCuvinte = 0;
        int nrCuvinteDistincte = 0;
     
        foreach (var kvp in dictionarInitial)
        {
            PosReader.DescifreazaIntrare(kvp, cuvant: out var cuvant, parte: out var parte, intrari: out var intrari, out var _);
     
            if (PosReader.ContineSemnePunctuatie(cuvant))
            {
                nrSemnePunctuatie += intrari;
                nrSemnePunctuatieDistincte++;
            }
            else
            {
                nrCuvinte += intrari;
                nrCuvinteDistincte++;
            }
     
     
            if (dictionarStatistici.ContainsKey(parte))
            {
                dictionarStatistici[parte] += 1;
            }
            else
            {
                dictionarStatistici.Add(parte,1);
            }
        }
     
        Console.WriteLine($"Date {numeDictionar}:\n");
        Console.WriteLine($"Nr. intrari totale: {dictionarInitial.Count}");
        Console.WriteLine($"Nr. cuvinte distincte: {nrCuvinteDistincte}");
        Console.WriteLine($"Nr. cuvinte: {nrCuvinte}"); 
        Console.WriteLine($"Nr. punctuatii totale: {nrSemnePunctuatie}");
        Console.WriteLine($"Nr. punctuatii distincte: {nrSemnePunctuatieDistincte}");
        Console.WriteLine($"Nr. parti vorbire: {PosReader.PartiVorbire.Count()}");
    }
     
    public static void AfiseazaIntrareDictionar(KeyValuePair<string, int> intrare)
    {
        PosReader.DescifreazaIntrare(intrare, cuvant: out var cuvant, parte:  out var parte, intrari:  out var intrari, out var _);
        Console.WriteLine("Cuvant: \"{0}\"; Parte: \'{1}\'; Nr. intrari: {2};",cuvant, parte, intrari);
    }
 

}