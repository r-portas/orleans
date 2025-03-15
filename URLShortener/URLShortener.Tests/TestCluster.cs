using Orleans.TestingHost;

namespace URLShortener.Tests;

public sealed class ClusterFixture : IDisposable
{
  public TestCluster Cluster { get; } = new TestClusterBuilder()
    .AddSiloBuilderConfigurator<TestSiloConfigurations>()
    .Build();

  public ClusterFixture() => Cluster.Deploy();

  void IDisposable.Dispose() => Cluster.StopAllSilos();
}

file sealed class TestSiloConfigurations : ISiloConfigurator
{
  public void Configure(ISiloBuilder siloBuilder)
  {
    siloBuilder.AddMemoryGrainStorage("urls");
    siloBuilder.ConfigureServices(static services =>
    {
      // Call required service registrations here.
    });
  }
}

[CollectionDefinition(Name)]
public sealed class ClusterCollection : ICollectionFixture<ClusterFixture>
{
  public const string Name = nameof(ClusterCollection);
}