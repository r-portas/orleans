using System.Text.Json.Serialization;

namespace BrisbaneRainfallSensors.Grains;

public interface IRainfallSensorGrain : IGrainWithStringKey
{
  Task SetMetadata(SourceRainfallSensorMetadata metadata);
  Task<RainfallSensorGrainState> GetState();
  Task SetReading(DateTime time, double value);
}

/// <summary>
/// Represents a single rainfall sensors
/// </summary>
/// <remarks>
/// Do some calculations, like rate of rise, as its a good use case of grains
/// </remarks>
public sealed class RainfallSensorGrain(
  [PersistentState("rainfall-sensor")]
  IPersistentState<RainfallSensorGrainState> metadataState,
  ILogger<RainfallSensorGrain> logger
) : Grain, IRainfallSensorGrain
{
  public Task<RainfallSensorGrainState> GetState() => Task.FromResult(metadataState.State);

  public async Task SetMetadata(SourceRainfallSensorMetadata metadata)
  {
    metadataState.State.SensorId = metadata.SensorId;
    metadataState.State.LocationId = metadata.LocationId;
    metadataState.State.LocationName = metadata.LocationName;
    metadataState.State.SensorType = metadata.SensorType;
    if (metadata.UnitOfMeasurement is not null)
    {
      metadataState.State.UnitOfMeasurement = metadata.UnitOfMeasurement;
    }
    metadataState.State.Latitude = metadata.Latitude;
    metadataState.State.Longitude = metadata.Longitude;
    await metadataState.WriteStateAsync();

    logger.LogInformation("Metadata updated for sensor {SensorId}", metadata.SensorId);
  }

  public async Task SetReading(DateTime time, double value)
  {
    metadataState.State.Readings[time] = value;
    await metadataState.WriteStateAsync();

    logger.LogInformation("Reading updated for sensor {SensorId} at {Time} updated to {Value}", metadataState.State.SensorId, time, value);
  }

}

[GenerateSerializer, Alias(nameof(RainfallSensorGrainState))]
public sealed record class RainfallSensorGrainState
{
  [Id(0)]
  public string SensorId { get; set; } = string.Empty;

  [Id(1)]
  public string LocationId { get; set; } = string.Empty;

  [Id(2)]
  public string LocationName { get; set; } = string.Empty;

  [Id(3)]
  public string SensorType { get; set; } = string.Empty;

  [Id(4)]
  public string UnitOfMeasurement { get; set; } = string.Empty;

  [Id(5)]
  public double Latitude { get; set; } = 0.0;

  [Id(6)]
  public double Longitude { get; set; } = 0.0;

  [Id(7)]
  public Dictionary<DateTime, double> Readings { get; set; } = [];
}
