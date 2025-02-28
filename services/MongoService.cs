using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

public class MongoService
{
    private readonly IMongoCollection<Territory> _territoryCollection;

    public MongoService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _territoryCollection = database.GetCollection<Territory>(
            mongoDbSettings.Value.CollectionName
        );
    }

    // ------------------------------- GET all territories ----------------------- //


    public async Task<List<Territory>> GetTerritoriesAsync()
    {
        try
        {
            var data = await _territoryCollection.Find(_ => true).ToListAsync();

            if (data.Count == 0)
            {
                Debug.WriteLine("No territories found");
                throw new Exception("No territories found in database.");
            }

            return data;
        }
        catch (MongoException ex)
        {
            Debug.WriteLine($"Error fetching territories: {ex.Message}");
            throw new Exception("Error fetching territories from database.", ex);
        }
    }

    // ------------------------------  CREATE new territoy -------------------------------- //


    public async Task CreateTerritoryAsync(Territory territory)
    {
        try
        {
            await _territoryCollection.InsertOneAsync(territory);
        }
        catch (Exception ex)
        {
            throw new Exception("Error creating territory", ex);
        }
    }

    // ------------------------------ ADD new assignament ----------------------------------- //

    public async Task AddAssignamentAsync(int territoryId, Assignament assignament)
    {
        try
        {
            var filter = Builders<Territory>.Filter.Eq(
                territory => territory.TerritoryId,
                territoryId
            );
            var update = Builders<Territory>.Update.Push(
                territory => territory.Assignaments,
                assignament
            );
            await _territoryCollection.UpdateOneAsync(filter, update);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding assignament: {ex.Message}");
            throw new Exception("Error adding assignament to territory", ex);
        }
    }

    // ------------------------------- Marcar un bloque como completo ----------------------------------- //

    public async Task MarkBlockAsCompleteAsync(int territoryId, string assignee, string block)
    {
        try
        {
            var filter = Builders<Territory>.Filter.And(
                Builders<Territory>.Filter.Eq(t => t.TerritoryId, territoryId),
                Builders<Territory>.Filter.Eq("Assignaments.Assignee", assignee)
            );

            var update = Builders<Territory>.Update.Set($"Assignaments.$.Blocks.{block}", true);
            await _territoryCollection.UpdateOneAsync(filter, update);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error marking block as complete: {ex.Message}");
            throw new Exception("Error marking block as complete", ex);
        }
    }

    // ------------------------------- Marcar una asignación como completa --------------------------------//

    public async Task MarkAssignamentAsCompleteAsync(
        int territoryId,
        string assignee,
        DateTime finishDate
    )
    {
        try
        {
            var filter = Builders<Territory>.Filter.And(
                Builders<Territory>.Filter.Eq(t => t.TerritoryId, territoryId),
                Builders<Territory>.Filter.Eq("Assignaments.Assignee", assignee)
            );

            var update = Builders<Territory>.Update.Set("Assignaments.$.FinishDate", finishDate);
            await _territoryCollection.UpdateOneAsync(filter, update);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error marking assignament as complete: {ex.Message}");
            throw new Exception("Error marking assignament as complete", ex);
        }
    }
}
