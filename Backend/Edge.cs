namespace Backend;

public readonly record struct Edge<T>(T From, T To, double Weight) where T : notnull;