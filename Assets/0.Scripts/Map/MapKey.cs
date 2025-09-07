using System;

public readonly struct MapKey : IEquatable<MapKey>
{
    public readonly int Chapter;
    public readonly int Map;
    public readonly int Number;

    public MapKey(int chapter, int map, int number)
    {
        Chapter = chapter;
        Map = map;
        Number = number;
    }

    public bool Equals(MapKey other) =>
        Chapter == other.Chapter && Map == other.Map && Number == other.Number;

    public override bool Equals(object obj) => obj is MapKey other && Equals(other);

    public override int GetHashCode() =>
        unchecked((Chapter * 397) ^ (Map * 17) ^ Number);

    public override string ToString() => $"({Chapter},{Map},{Number})";
}

