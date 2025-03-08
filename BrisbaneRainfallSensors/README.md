# BrisbaneRainfallSensors

## About

This is a sample Microsoft Orleans application that uses real world rainfall telemetry from Brisbane City Council owned telemetry sensors.

Theres a few reasons for picking the rainfall dataset:

- Theres around 100 sensors in Brisbane
- The dataset is updated every 15 minutes

This project is designed to explore Orleans in a real world setting, which explores the following:

- Creating a digital twin like system modelling real world devices
  TODO: Expand on

Data is sourced from the following public APIs:

- [Telemetry sensors - Rainfall and Stream heights](https://data.brisbane.qld.gov.au/explore/dataset/telemetry-sensors-rainfall-and-stream-heights/information/)
- [Telemetry sensors — Rainfall and Stream heights — metadata](https://data.brisbane.qld.gov.au/explore/dataset/telemetry-sensors-rainfall-and-stream-heights-metadata/information/)

## Design

- `RainfallSensorGrain`: Grain for each telemetry sensor
- `MetadataUpdaterGrain`: Grain with a reminder that periodically calls the Metadata API and update the details of each `RainfallSensorGrain`, this is called infrequently
- `TelemetryUpdaterGrain`: Grain with a reminder that periodically calls the Telemetry API and update the details of each `RainfallSensorGrain`, which is called every 15 minutes

## Reference

- The structure of the app was mostly based off the [eShop example](https://github.com/dotnet/eShop/tree/main)
  - API based off the [Catalog.API](https://github.com/dotnet/eShop/tree/main/src/Catalog.API)
