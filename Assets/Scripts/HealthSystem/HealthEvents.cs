public readonly struct HealthChangedArgs
{
    public readonly int Current;
    public readonly int Max;
    public readonly int Delta;   // - for damage, + for heal
    public readonly object Source;
    public HealthChangedArgs(int current, int max, int delta, object source)
    { Current = current; Max = max; Delta = delta; Source = source; }
}

public readonly struct SpawnedArgs
{
    public readonly int Current;
    public readonly int Max;
    public SpawnedArgs(int current, int max) { Current = current; Max = max; }
}

public readonly struct DeathArgs
{
    public readonly object Source;
    public DeathArgs(object source) { Source = source; }
}
