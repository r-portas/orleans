using BrisbaneRainfallSensors.Grains;

namespace BrisbaneRainfallSensors.Apis;

public static class SensorApi
{
  public static IEndpointRouteBuilder MapSensorApi(this IEndpointRouteBuilder app)
  {
    var sensorApi = app.MapGroup("sensors");

    sensorApi.MapGet("", static async (IGrainFactory grainFactory) => grainFactory.GetGrain<IManagerGrain>("default").ListSensors());

    return app;
  }
}