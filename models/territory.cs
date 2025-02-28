using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Territory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public int TerritoryId { get; set; }
    public Dictionary<string, bool> Blocks { get; set; } = new(); // Bloques asociados al territorio
    public List<Assignament> Assignaments { get; set; } = new(); // Asignaciones realizadas sobre el territorio
}

public class Assignament
{
    public string Assignee { get; set; } = null!;
    public DateTime AssignamentDate { get; set; }
    public DateTime? FinishDate { get; set; }
}
