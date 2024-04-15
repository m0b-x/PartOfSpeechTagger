using ConsoleApp1;

//TIP DICTIONAR: WORD#PART
//PROIECT FACUT IN .NET7

/////////////////////////////////////////////////////
//Program Initial - Realizeaza viitoarele input-uri//
/////////////////////////////////////////////////////

string workingDirectory = Environment.CurrentDirectory;
string slnDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

var dictionarCategorii = FileInformer.GetCategories();

var directoryPath = Path.Combine(slnDirectory, "brown");

var filePath = Path.Combine(slnDirectory, "output.txt");

var filePathDictionarNou = Path.Combine(slnDirectory, "outputDictionarNou.txt");

var filePathDictionarDeCautare = Path.Combine(slnDirectory, "outputDictionarDeCautare.txt");

var dictionarBrownCorpus = PosReader.CitesteFisiereleBrownCorpus(directoryPath: directoryPath);

//PosWriter.AfiseazaDateDictionar(dictionarBrownCorpus, out Dictionary<string,int> dictionarStatistici, numeDictionar:"DictionarVechie");
 
var dictionarSimplificat = PosTranslator.SimplificaDictionar(dictionarBrownCorpus);

var dictionarDeCautare = PosTranslator.ConvertesteDictionarInDictionarDeCautare(dictionarSimplificat);

var dictionarDeCautareOrdonat = PosTranslator.ReturneazaDictionarDeCautareOrdonat(dictionarDeCautare: dictionarDeCautare);

PosWriter.ScrieInFisierDate(pathFisier: filePath, dictionar: dictionarBrownCorpus);

PosTranslator.ScrieInFisierDate(pathFisier: filePathDictionarNou, dictionar: dictionarSimplificat);

PosTranslator.ScrieDictionarDeCautareInFisier(pathFisier: filePathDictionarDeCautare, dictionarDeCautare: dictionarDeCautareOrdonat);

/*
var cuvantCautare = "train";
var rezultat = UtilitatiDictionar.ReturneazaRezultatDictionar(dictionar: dictionarDeCautareOrdonat, cautare: cuvantCautare);
var rezultatListe = UtilitatiDictionar.ConvertestePartiInKVP(rezultat);
Console.WriteLine(UtilitatiDictionar.AfiseazaRezultatKVP(cuvantCautare, rezultatListe));
*/

 
PosEvaluator.DictionarCuprocentaj = PosTranslator.RealizeazaDictionarCuProcentaj(dictionarDeCautareOrdonat);

PosEvaluator.EvalueazaFisiereleBrownCorpus(directoryPath: directoryPath, dictionarDeCautareOrdonat);

PosEvaluator.AfiseazaPrecizieAcurateteRecallSelectivitatePtParti();

Console.WriteLine($"Fisiere Totale:{FileInformer.TrainingFiles.Count+FileInformer.TestingFiles.Count}, din care {FileInformer.TrainingFiles.Count} de antrenare si {FileInformer.TestingFiles.Count} de testare \n");
Console.WriteLine(($"{FileInformer.ProcentajAntrenament}% pt antrenament, {(100 - FileInformer.ProcentajAntrenament)}% pt testare\n"));
Console.WriteLine($"\nCuvinte Citite la antrenare:{PosReader.NrIntrari} (fara StopWords), din care {dictionarDeCautareOrdonat.Count} cuvinte unice\n");
Console.WriteLine($"Cuvinte citite la evaluare:{PosEvaluator.CuvinteCitite}  (fara StopWords)\nEvaluate Corect:{PosEvaluator.IdentificateCorect}\nEvaluate Gresit:{PosEvaluator.IdentificateGresit}");

/*
Console.WriteLine($"Nu apartin la dictionar: {PosTranslator.ContorApartineDictionar}");
Console.WriteLine($"Stopwords numere in datele de intrare: {PosReader.ContorStopWordsNumarDetectate}");
Console.WriteLine($"Stopwords cuvinte/altele in datele de intrare: {PosReader.ContorStopWordsAlteleDetectate}");
Console.WriteLine($"Procentaj cuvinte citite/stopwords {PosReader.ReturneazaRaportStopWords()}");
Console.WriteLine($"Acuratete Tagger Context:{ ( (double) PosEvaluator.PreziseCorectDeTaggerContext / PosEvaluator.TrebuiePreziseDeTaggerContext )}");
Console.WriteLine($"Tagger Context: Total:{PosEvaluator.TrebuiePreziseDeTaggerContext} Prezise Corect: {PosEvaluator.PreziseCorectDeTaggerContext}, PreziseGresit:{PosEvaluator.PreziseGresitDeTaggerContext}");
*/

/////////////////////////////////////////////////////////////////
//Program Intermediar - Citire dictionar si realizare o cautare//
/////////////////////////////////////////////////////////////////

/*
var filePathDictionarCautare =
   Path.Combine(slnDirectory, "outputDictionarDeCautare.txt");

var dictionarDeCautare = UtilitatiDictionar.CitesteDictionarDeCautare(filePathDictionarCautare);
*/
