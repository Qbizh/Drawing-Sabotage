using System;
using System.Collections.Generic;

public static class ListOperations
{
    public static void Shuffle<T>(IList<T> list)
    {
        Random rng = new Random(); // Create a new Random instance
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1); // Generate a random index between 0 and n (inclusive)
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
