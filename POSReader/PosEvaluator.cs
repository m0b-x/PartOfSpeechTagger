using System.Text.RegularExpressions;

namespace ConsoleApp1;

public static class PosEvaluator
{
    public static int CuvinteCitite = 0;
    public static int IdentificateGresit = 0;
    public static int IdentificateCorect = 0;
    public static Dictionary<string, string> DictionarCuprocentaj;
    public static double ProcentSiguranta = 0.35;//schimbat
    public static HashSet<string> HashSetCuvinteUnice = new();

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
            {"nespecificat", new int[4]}
        };

    public static Dictionary<string, List<string>> DictionarInafaraDe =
        new()
        {
            {"adverb", DictionarConfusion.Keys.Where(key => key != "adverb").ToList()},
            {
                "interjectie/prepozitie/conjunctie",
                DictionarConfusion.Keys.Where(key => key != "interjectie/prepozitie/conjunctie").ToList()
            },
            {"substantiv", DictionarConfusion.Keys.Where(key => key != "substantiv").ToList()},
            {"pronume", DictionarConfusion.Keys.Where(key => key != "pronume").ToList()},
            {"articol", DictionarConfusion.Keys.Where(key => key != "articol").ToList()},
            {"verb", DictionarConfusion.Keys.Where(key => key != "verb").ToList()},
            {"adjectiv", DictionarConfusion.Keys.Where(key => key != "adjectiv").ToList()},
            {"punctuatie", DictionarConfusion.Keys.Where(key => key != "punctuatie").ToList()},
            {"nespecificat", DictionarConfusion.Keys.Where(key => key != "nespecificat").ToList()}
        };
    
    public static int TP = 0, TN = 1, FP = 2, FN = 3;


    public static void EvalueazaFisiereleBrownCorpus(string directoryPath,
        Dictionary<string, string> dictionarDeCautare)
    {
        try
        {
            if (Directory.Exists(directoryPath))
            {
                //var files = Directory.GetFiles(directoryPath);
                //var filteredFilePaths = PosReader.ReturneazaFisiereleFiltrate(files);

                //aici
                foreach (var filePath in FileInformer.TestingFiles)
                {
                    var contentFisier = CitesteDocumentInString(filePath);
                    TokenizeazaSiEvalueazaInDictioar(contentFisier, dictionarDeCautare);
                }

            }
            else
            {
                Console.WriteLine("[Eroare]: Directorul {0} nu exista.", directoryPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Eroare-Evaluator]: Eroare la directorul {0}: {1}", directoryPath, ex);
        }

    }


    private static IEnumerable<string> BreakTextIntoSentences(string text)
    {
        // Use a simple regex to split sentences based on common punctuation marks
        string[] sentenceDelimiters = {".", "!", "?"};
        var pattern = "[" + string.Join("", sentenceDelimiters) + "]";

        return Regex.Split(text, pattern);
    }

    public static int PreziseCorectDeTaggerContext = 0;
    public static int PreziseGresitDeTaggerContext = 0;
    public static int TrebuiePreziseDeTaggerContext = 0;
    public static void TokenizeazaSiEvalueazaInDictioar(string fileContent,
        Dictionary<string, string> dictionarDeCautare)
    {
        const string pattern = @"(\S+)/(\S+)(?:\s|$)";

        var sentences = BreakTextIntoSentences(fileContent);
        foreach (var sentence in sentences)
        {
            var matches = Regex.Matches(sentence, pattern);
            var previousPart = "substantiv";
            var previousWord = " ";

            foreach (Match match in matches)
            {
                var word = match.Groups[1].Value;
                var part = match.Groups[2].Value;
                
                if (StopWords.HashSetStopWords.Contains(word) || StopWords.EsteNumar(word)) continue;
                HashSetCuvinteUnice.Add(word);
                
                var procent = PosTranslator.ReturneazaProcentajDinDictionar(word, DictionarCuprocentaj);
                string rezultatCautare;

                bool aTrebuitPrezis = false;
                if (procent < ProcentSiguranta)
                {
                    rezultatCautare = UtilitatiDictionar.ReturneazaParteaDeVorbireBazataPeContext(word, previousPart, previousWord);
                    aTrebuitPrezis = true;
                    TrebuiePreziseDeTaggerContext++;
                }
                else
                {
                    rezultatCautare =
                        UtilitatiDictionar.ReturneazaRezultatDictionar(dictionarDeCautare, word, previousPart);
                }

                var dashIndex = rezultatCautare.IndexOf('-');
                var parteReturnata = dashIndex != -1 ? rezultatCautare.Substring(0, dashIndex) : rezultatCautare;

                var parteReala = PosTranslator.ReturneazaParteaDeVorbireSimplificata(part);
                /*
                if (parteReala.Equals("nespecificat"))
                {
                    Console.WriteLine(word);
                }
                */
                CuvinteCitite++;

                //aici vedem ca nu gaseste
                if (parteReturnata.Equals(parteReala) || HasAlreadyPredictedPunctuation(parteReala, parteReturnata))
                {
                    if (aTrebuitPrezis) PreziseCorectDeTaggerContext++;
                    
                    //Tagger-ul a evaluat corect
                    IdentificateCorect++;

                    //pentru partea outcome adauga true pozitive, am zis ca e si a fost
                    DictionarConfusion[parteReala][TP]++;

                    //pentru fiecare alta parte de vb adauga true negative, am zis ca nu e si nu a fost
                    foreach (var altaParte in DictionarInafaraDe[parteReala]) DictionarConfusion[altaParte][TN]++;
                }
                else
                {
                    if (aTrebuitPrezis) PreziseGresitDeTaggerContext++;
                    //Tagger-ul a evaluate greist
                    IdentificateGresit++;

                    //pentru partea reala adauga false positive, (am zis ca nu a fost si a fost)
                    DictionarConfusion[parteReala][FP]++;

                    //pentru partea returnata adauga false negative (am zis ca e si nu a fost)
                    DictionarConfusion[parteReturnata][FN]++;

                    //pentru celalalte parti inafara de reala si falsa adauga true negative
                    foreach (var altaParte in DictionarInafaraDe[parteReala]) DictionarConfusion[altaParte][TN]++;
                    //scadem TN pt ca nu am DictionarInafaraDe cu 2 argumente
                    DictionarConfusion[parteReturnata][TN]--;
                }

                previousPart = parteReturnata;
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
        double totPrecizie = 0, totAcuratete = 0, totRecall = 0, totSelectivitate = 0;

        var partiSimplificate = new HashSet<string>(PosTranslator.SetPartiSimplificate);
        partiSimplificate.Remove("nespecificat");
        
        foreach (var parte in partiSimplificate)
        {
            numarator = DictionarConfusion[parte][TP] + DictionarConfusion[parte][TN];
            numitor = numarator + DictionarConfusion[parte][FP] + DictionarConfusion[parte][FN];
            acuratete = numarator / numitor;
            totAcuratete += acuratete;

            numarator = DictionarConfusion[parte][TP];
            numitor = DictionarConfusion[parte][TP] + DictionarConfusion[parte][FP];
            precizie = numarator / numitor;
            totPrecizie += precizie;

            numarator = DictionarConfusion[parte][TP];
            numitor = DictionarConfusion[parte][TP] + DictionarConfusion[parte][FN];
            recall = numarator / numitor;
            totRecall += recall;

            numarator = DictionarConfusion[parte][TN];
            numitor = DictionarConfusion[parte][FP] + DictionarConfusion[parte][TN];
            selectivitate = numarator / numitor;
            totSelectivitate += selectivitate;

            Console.WriteLine(
                $"Pt {parte}:\n \t TP:{DictionarConfusion[parte][TP]} TN:{DictionarConfusion[parte][TN]} FP:{DictionarConfusion[parte][FP]} FN:{DictionarConfusion[parte][FN]}  \n" +
                $"\t  Acuratete:{acuratete},Precizie:{precizie}, Recall:{recall} Selectivitate:{selectivitate}\n");
        }
        
        totAcuratete /= partiSimplificate.Count;
        totPrecizie /= partiSimplificate.Count;
        totSelectivitate /= partiSimplificate.Count;
        totRecall /= partiSimplificate.Count;
        
        partiSimplificate.Add("nespecificat");
        
        Console.WriteLine($"\nAcuratete:{totAcuratete}\nPrecizie:{totPrecizie}\nRecall:{totRecall}\nSelectivitate:{totSelectivitate}");
        Console.WriteLine($"\nAproximari:\nAcuratete:{Math.Round(totAcuratete,2)}\nPrecizie:{Math.Round(totPrecizie,2)}\nRecall:{Math.Round(totRecall,2)}\nSelectivitate:{Math.Round(totSelectivitate,2)}\n");

    }

    public static bool HasAlreadyPredictedPunctuation(string parteReala,string parteReturnata)
    {
        return parteReala.Equals("punctuatie") || parteReturnata.Equals("punctuatie");
    }
}