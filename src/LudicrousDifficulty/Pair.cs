namespace SavageDifficulty;

public class Pair<TFirst, TSecond>
{
    public TFirst first;
    public TSecond second;

    public Pair(TFirst first, TSecond second)
    {
        this.first = first;
        this.second = second;
    }

    public bool Equals(Pair<TFirst, TSecond> other)
    {
        if (this == null && other == null) return true;
        return first.Equals(other.first) && second.Equals(other.second);
    }
}
