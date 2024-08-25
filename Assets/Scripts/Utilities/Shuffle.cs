using System.Collections.Generic;

/// <summary>
/// The <c>ListExtensions</c> class provides extension methods for the <c>IList&lt;T&gt;</c> interface.
/// It includes utility methods to enhance the functionality of generic lists.
/// </summary>
public static class ListExtensions
{
    // A static random number generator used for shuffling the list
    private static System.Random rng = new System.Random();

    /// <summary>
    /// Shuffles the elements of the list in a random order.
    /// This method uses the Fisher-Yates shuffle algorithm to randomly reorder the elements in the list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to be shuffled.</param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--; // Decrement the total number of elements to shuffle
            int k = rng.Next(n + 1); // Generate a random index from 0 to n
            T value = list[k]; // Temporarily store the value at the random index
            list[k] = list[n]; // Replace the element at the random index with the last unshuffled element
            list[n] = value; // Assign the stored value to the last unshuffled position
        }
    }
}
