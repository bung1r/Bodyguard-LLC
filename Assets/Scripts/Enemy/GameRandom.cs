public class GameRandom
{
    private readonly System.Random random;

    public int CallsMade { get; private set; }

    public GameRandom(int seed)
    {
        random = new System.Random(seed);
    }

    public int Next()
    {
        CallsMade++;
        return random.Next();
    }

    public int Next(int min, int max)
    {
        CallsMade++;
        return random.Next(min, max);
    }

    public double NextDouble()
    {
        CallsMade++;
        return random.NextDouble();
    }
}