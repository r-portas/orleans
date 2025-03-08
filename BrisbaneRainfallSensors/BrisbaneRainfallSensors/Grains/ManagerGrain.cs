using System.Text.Json.Serialization;

namespace BrisbaneRainfallSensors.Grains;

/// <summary>
/// A background service to activate the manager grain.
/// </summary>
/// <param name="grainFactory"></param>
public class ManagerBackgroundService(IGrainFactory grainFactory) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken _)
  {
    await grainFactory.GetGrain<IManagerGrain>("default").Start();
  }
}

public interface IManagerGrain : IGrainWithStringKey
{
  Task<RainfallSensorGrainState[]> ListSensors();
  Task Start();
}

/// <summary>
/// The manager grain manages the rainfall sensor grains,
/// the API calls methods on this grain, which then calls methods
/// on the individual sensor grains.
/// </summary>
/// <remarks>
/// This grain keeps a list of the sensor ids, for the purpose of
/// supporting a list endpoint.
/// </remarks>
public sealed class ManagerGrain(
  [PersistentState("manager")]
  IPersistentState<ManagerGrainState> state,
  IHttpClientFactory httpClientFactory,
  ILogger<ManagerGrain> logger
) : Grain, IManagerGrain
{
  const string MetadataApiUrl = "https://data.brisbane.qld.gov.au/api/explore/v2.1/catalog/datasets/telemetry-sensors-rainfall-and-stream-heights-metadata/exports/json?lang=en&timezone=Australia%2FBrisbane";
  const string SensorReadingsApiUrl = "https://data.brisbane.qld.gov.au/api/explore/v2.1/catalog/datasets/telemetry-sensors-rainfall-and-stream-heights/exports/csv?lang=en&timezone=Australia%2FBrisbane&use_labels=true&delimiter=%2C";
  /// <summary>
  /// List all the sensors.
  /// </summary>
  /// <remarks>
  /// In a real world application, you'd want to query this from a database,
  /// instead of having to query each grain individually
  /// </remarks>
  public Task<RainfallSensorGrainState[]> ListSensors()
  {
    var tasks = state.State.SensorIds.Select(sensorId => GrainFactory.GetGrain<IRainfallSensorGrain>(sensorId).GetState());
    return Task.WhenAll(tasks);
  }

  public async Task Start()
  {
    // Fetch the metadata once on activation
    await FetchMetadata();

    // Fetch the data, every 15 minutes
    await FetchSensorReadings();
  }

  async Task FetchMetadata()
  {
    using var client = httpClientFactory.CreateClient();

    var sensors = await client.GetFromJsonAsync<SourceRainfallSensorMetadata[]>(MetadataApiUrl);
    if (sensors is null)
    {
      logger.LogError("Failed to fetch metadata");
      return;
    }
    logger.LogInformation("Fetched metadata for {Count} sensors", sensors.Length);

    // Store it into state
    state.State.SensorIds = sensors.Select(sensor => sensor.SensorId).ToList();
    await state.WriteStateAsync();

    // Activate the grains
    var tasks = sensors.Select(sensor => GrainFactory.GetGrain<IRainfallSensorGrain>(sensor.SensorId).SetMetadata(sensor));
    await Task.WhenAll(tasks);
  }

  async Task FetchSensorReadings()
  {
    using var client = httpClientFactory.CreateClient();

    var csvData = await client.GetStringAsync(SensorReadingsApiUrl);
    using var reader = new StringReader(csvData);
    var rawHeader = await reader.ReadLineAsync();
    if (rawHeader is null)
    {
      logger.LogError("Failed to read header");
      return;
    }
    var headers = rawHeader.Split(',');

    List<Task> tasks = [];

    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
    {
      var parts = line.Split(",");

      if (parts.Length != headers.Length)
      {
        logger.LogError("Skipping line, as it has an invalid number of fields");
        continue;
      }

      // First field is the timestamp
      var timestamp = DateTime.Parse(parts[0]);

      for (var i = 1; i < parts.Length; i++)
      {
        if (parts[i] == "-")
        {
          continue;
        }
        var sensorId = headers[i];
        var value = double.Parse(parts[i]);
        var task = GrainFactory.GetGrain<IRainfallSensorGrain>(sensorId).SetReading(timestamp, value).ConfigureAwait(false);
      }
    }

    await Task.WhenAll(tasks);
  }

}

[GenerateSerializer, Alias(nameof(ManagerGrainState))]
public sealed record class ManagerGrainState
{
  [Id(0)]
  public List<string> SensorIds { get; set; } = new();
}

#region Source API Models

[GenerateSerializer]
public record class SourceRainfallSensorMetadata
{
  [JsonPropertyName("sensor_id")]
  [Id(0)]
  public required string SensorId { get; set; }

  [JsonPropertyName("location_id")]
  [Id(1)]
  public required string LocationId { get; set; }

  [JsonPropertyName("location_name")]
  [Id(2)]
  public required string LocationName { get; set; }

  [JsonPropertyName("sensor_type")]
  [Id(3)]
  public required string SensorType { get; set; }

  [JsonPropertyName("unit_of_measurement")]
  [Id(4)]
  public string? UnitOfMeasurement { get; set; }

  [JsonPropertyName("latitude")]
  [Id(5)]
  public required double Latitude { get; set; }

  [JsonPropertyName("longitude")]
  [Id(6)]
  public required double Longitude { get; set; }
}

#endregion