namespace Backend;

/// <summary>
/// I'll leave a brief interpretation of the project here, Zulu.
/// A package to deliver: its number, the location where it is picked up and the location where
/// it is delivered.
/// </summary>

public readonly record struct Package(int Id, string Pickup, string Delivery);