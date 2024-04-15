using System.Text.RegularExpressions;

namespace ConsoleApp1;

public static class PosBNCEvaluator
{
    public static int CuvinteCitite = 0;
    public static int IdentificateGresit = 0;
    public static int IdentificateCorect = 0;

    private static readonly Dictionary<string, int[]> DictionarConfusion =
        new()
        {
            {"adverb", new int[4]},
            {"interjectie/prepozitie/conjunctie", new int[4]},
            {"substantiv", new int[4]},
            {"pronume", new int[4]},
            {"articol", new int[4]},
            {"verb", new int[4]},
            {"adjectiv", new int[4]},
            {"punctuatie", new int[4]},
            {"nespecificat", new int[4]},
            {"niciuna", new int[4]}
        };

    public static Dictionary<string, List<string>> DictionarInafaraDe =
        new()
        {
            {"adverb", DictionarConfusion.Keys.Where(key => key != "adverb").ToList()},
            {"interjectie/prepozitie/conjunctie",
                DictionarConfusion.Keys.Where(key => key != "interjectie/prepozitie/conjunctie").ToList()
            },
            {"substantiv", DictionarConfusion.Keys.Where(key => key != "substantiv").ToList()},
            {"pronume", DictionarConfusion.Keys.Where(key => key != "pronume").ToList()},
            {"articol", DictionarConfusion.Keys.Where(key => key != "articol").ToList()},
            {"verb", DictionarConfusion.Keys.Where(key => key != "verb").ToList()},
            {"adjectiv", DictionarConfusion.Keys.Where(key => key != "adjectiv").ToList()},
            {"punctuatie", DictionarConfusion.Keys.Where(key => key != "punctuatie").ToList()},
            {"nespecificat", DictionarConfusion.Keys.Where(key => key != "nespecificat").ToList()},
            {"niciuna", DictionarConfusion.Keys.Where(key => key != "nespecificat").ToList()}
        };

    public static int TP = 0, TN = 1, FP = 2, FN = 3;


    public static Dictionary<string, int> EvalueazaFisiereleBrownCorpus(string directoryPath)
    {
        try
        {
            if (Directory.Exists(directoryPath))
            {
                //var files = Directory.GetFiles(directoryPath);
                //var filteredFilePaths = PosReader.ReturneazaFisiereleFiltrate(files);
                
                Dictionary<string, int> dictionarBrownCorpus = new(50000);
                //aici
                foreach (var filePath in FileInformer.TestingFiles)
                {
                    var contentFisier = CitesteDocumentInString(filePath);
                    TokenizeazaSiEvalueazaInDictioar(contentFisier);
                }

                return dictionarBrownCorpus;
            }
            else
            {
                Console.WriteLine("[Eroare]: Directorul {0} nu exista.", directoryPath);
                return new Dictionary<string, int>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Eroare-Evaluator]: Eroare la directorul {0}: {1}", directoryPath, ex);
        }

        return new Dictionary<string, int>();
    }

    private static IEnumerable<string> BreakTextIntoSentences(string text)
    {
        // Use a simple regex to split sentences based on common punctuation marks
        string[] sentenceDelimiters = { ".", "!", "?" };
        string pattern = "[" + string.Join("", sentenceDelimiters) + "]";
    
        return Regex.Split(text, pattern);
    }

    public static void TokenizeazaSiEvalueazaInDictioar(string fileContent)
    {
        const string pattern = @"(\S+)/(\S+)(?:\s|$)";

        var sentences = BreakTextIntoSentences(fileContent);
        foreach (var sentence in sentences)
        {
            var matches = Regex.Matches(sentence, pattern);
            //aici
            string previousPart = "niciuna";
            string previousWord = "";
            foreach (Match match in matches)
            {
                var word = match.Groups[1].Value;
                var part = match.Groups[2].Value;

                if (StopWords.HashSetStopWords.Contains(word)) continue;
                
                var rezultatCautare =
                    BNCClasifier.ReturneazaRezultatDictionar(word, previousWord, previousPart);

                var dashIndex = rezultatCautare.IndexOf('-');
                var parteReturnata = dashIndex != -1 ? rezultatCautare.Substring(0, dashIndex) : rezultatCautare;

                var parteReala = PosTranslator.ReturneazaParteaDeVorbireSimplificata(part);
                CuvinteCitite++;

                //in caz ca taggerul nu gaseste cuvantul
                /*
                if (parteReturnata.Equals(UtilitatiDictionar.EroareNegasit))
                {
                    //Console.WriteLine(word + " " + parteReala + " " + parteReturnata);
                    DictionarConfusion[parteReala][FP]++;
                    IdentificateGresit++;
                    continue;
                }
                */
                
                //aici vedem ca nu gaseste
                if (parteReturnata.Equals(parteReala))
                {
                    //Tagger-ul a evaluat corect
                    IdentificateCorect++;

                    //pentru partea outcome adauga true pozitive, am zis ca e si a fost
                    DictionarConfusion[parteReala][TP]++;

                    //pentru fiecare alta parte de vb adauga true negative, am zis ca nu e si nu a fost
                    foreach (var altaParte in DictionarInafaraDe[parteReala]) DictionarConfusion[altaParte][TN]++;
                }
                else
                {
                    //Tagger-ul a evaluate greist
                    IdentificateGresit++;

                    //pentru partea returnata adauga false negative (am zis ca e si nu a fost)
                    DictionarConfusion[parteReturnata][FN]++;

                    //pentru partea reala adauga false positive, (am zis ca nu a fost si a fost)
                    DictionarConfusion[parteReala][FP]++;

                    //pentru celalalte parti inafara de reala si falsa adauga true negative
                    foreach (var altaParte in DictionarInafaraDe[parteReala]) DictionarConfusion[altaParte][TN]++;
                    //scadem TN pt ca nu am DictionarInafaraDe cu 2 argumente
                    DictionarConfusion[parteReturnata][TN]--;
                }

                previousPart = parteReala;
                previousWord = word;
            }
        }
    }

    public static string CitesteDocumentInString(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                var documentContent = File.ReadAllText(filePath);
                return documentContent;
            }
            else
            {
                Console.WriteLine("[Eroare]: Fisierul {0} nu exista!.", filePath);
                return PosReader.ErrorString;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Eroare]: Eroare la citirea fisierului {0}: {1}\n", filePath, ex);
            return PosReader.ErrorString;
        }
    }

    public static void AfiseazaPrecizieAcurateteRecallSelectivitatePtParti()
    {
        double precizie, acuratete, recall, selectivitate, numitor, numarator;
        foreach (var parte in PosTranslator.SetPartiSimplificate)
        {
            numarator = DictionarConfusion[parte][TP] + DictionarConfusion[parte][TN];
            numitor = numarator + DictionarConfusion[parte][FP] + DictionarConfusion[parte][FN];
            acuratete = numarator / numitor;

            numarator = DictionarConfusion[parte][TP];
            numitor = DictionarConfusion[parte][TP] + DictionarConfusion[parte][FP];
            precizie = numarator / numitor;

            numarator = DictionarConfusion[parte][TP];
            numitor = DictionarConfusion[parte][TP] + DictionarConfusion[parte][FN];
            recall = numarator / numitor;

            numarator = DictionarConfusion[parte][TN];
            numitor = DictionarConfusion[parte][FP] + DictionarConfusion[parte][TN];
            selectivitate = numarator / numitor;

            Console.WriteLine(
                $"Pt {parte}:\n \t TP:{DictionarConfusion[parte][TP]} TN:{DictionarConfusion[parte][TN]} FP:{DictionarConfusion[parte][FP]} FN:{DictionarConfusion[parte][FN]}  \n" +
                $"\t Precizie:{precizie}, Acuratete:{acuratete}, Recall:{recall} Selectivitate:{selectivitate}");
            
        }
        

    }
}