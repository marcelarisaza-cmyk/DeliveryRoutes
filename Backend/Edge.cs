namespace Backend;

/// <summary>
///Here I present a brief interpretation of the Zulu project.
/// A directed edge in the city graph: it can only be traversed from an origin point
/// FROM to a destination point TO.
/// The weight corresponds to the length of the street in meters (m).
/// </summary>
/// 
public readonly record struct Edge<T>(T From, T To, double Weight) where T : notnull;