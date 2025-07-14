using System.Security.Cryptography;

namespace Kadense.RPG.Models;
public class KadenseRandomizer
{
    public int Roll(int startingNumber, int sides)
    {
        if (sides <= 0)
            throw new ArgumentOutOfRangeException(nameof(sides), "Number of sides must be greater than zero.");

        return RandomNumberGenerator.GetInt32(startingNumber, sides + 1);
    }
    public void Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}