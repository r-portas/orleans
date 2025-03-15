using Orleans.TestingHost;
using URLShortener.Grains;

namespace URLShortener.Tests;

[Collection(ClusterCollection.Name)]
public class URLShortenerGrainTests(ClusterFixture fixture)
{
  private readonly TestCluster cluster = fixture.Cluster;

  [Fact]
  public async Task CanSetAndGetUrlCorrectly()
  {
    var grain = cluster.GrainFactory.GetGrain<IUrlShortenerGrain>("test");
    var url = "https://www.microsoft.com";
    await grain.SetUrl(url);
    var result = await grain.GetUrl();
    Assert.Equal(url, result);
  }
}