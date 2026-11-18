namespace Backend;

/// <summary>
/// ZULU The kind of task done at a stop.
/// </summary>

public enum StopKind { Pickup, Delivery }


/// <summary>
/// ZULU Una parada de la ruta como una ubicación qué se realiza allí y para qué paquete, sin perderlo.
/// </summary>
public readonly record struct Stop(string Location, StopKind Kind, int PackageId);