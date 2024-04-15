namespace ConsoleApp1;

public static class UtilitatiListe
{
    public static List<KeyValuePair<string, int>> SorteazaListaKVPDupaValoriAsc(List<KeyValuePair<string, int>> pairs)
    {
        // Find the range of values
        int minValue = int.MaxValue;
        int maxValue = int.MinValue;

        foreach (var pair in pairs)
        {
            minValue = Math.Min(minValue, pair.Value);
            maxValue = Math.Max(maxValue, pair.Value);
        }

        // Use Counting Sort
        int[] countingArray = new int[maxValue - minValue + 1];

        foreach (var pair in pairs)
        {
            countingArray[pair.Value - minValue]++;
        }

        List<KeyValuePair<string, int>> sortedPairs = new List<KeyValuePair<string, int>>(pairs.Count);

        for (int i = 0; i < countingArray.Length; i++)
        {
            int count = countingArray[i];

            for (int j = 0; j < count; j++)
            {
                int value = i + minValue;
                var pair = pairs.Find(p => p.Value == value);
                sortedPairs.Add(pair);
            }
        }

        return sortedPairs;
    }
    
    
    public static List<KeyValuePair<string, int>> SorteazaListaKVPDupaValoriDesc(List<KeyValuePair<string, int>> pairs)
    {
        // Find the range of values
        int minValue = int.MaxValue;
        int maxValue = int.MinValue;

        foreach (var pair in pairs)
        {
            minValue = Math.Min(minValue, pair.Value);
            maxValue = Math.Max(maxValue, pair.Value);
        }

        // Use Counting Sort
        int[] countingArray = new int[maxValue - minValue + 1];

        foreach (var pair in pairs)
        {
            countingArray[pair.Value - minValue]++;
        }

        List<KeyValuePair<string, int>> sortedPairs = new List<KeyValuePair<string, int>>(pairs.Count);

        // Process counting array from end to beginning for descending order
        for (int i = countingArray.Length - 1; i >= 0; i--)
        {
            int count = countingArray[i];

            for (int j = 0; j < count; j++)
            {
                int value = i + minValue;
                var pair = pairs.Find(p => p.Value == value);
                sortedPairs.Add(pair);
            }
        }

        return sortedPairs;
    }

}