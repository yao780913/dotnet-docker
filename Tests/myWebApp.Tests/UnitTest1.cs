using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
namespace myWebApp.Tests;

public class Tests: IDisposable
{
    private const ushort HttpPort = 80;
    
    private readonly CancellationTokenSource  _cts = new (TimeSpan.FromMinutes(1));
    
    private readonly INetwork _network;
    private readonly IContainer _appContainer;
    private readonly IContainer _dbContainer;

    public Tests ()
    {
        _network = new NetworkBuilder()
            .Build();
        
        _appContainer = new ContainerBuilder()
            .WithImage("dotnet-docker")
            .WithNetwork(_network)
            .WithNetworkAliases("db")
            .WithVolumeMount("postgres-data", "/var/lib/postgresql/data")
            .Build();

        _dbContainer = new ContainerBuilder()
            .WithImage("postgres:12.1-alpine")
            .WithNetwork(_network)
            .WithPortBinding(HttpPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPath("/")))
            .Build();

    }

    [SetUp]
    public async Task SetUp ()
    {
        await _network.CreateAsync(_cts.Token).ConfigureAwait(false);
        await _dbContainer.StartAsync(_cts.Token).ConfigureAwait(false);
        await _appContainer.StartAsync(_cts.Token).ConfigureAwait(false);
    }

    [Test]
    public async Task Test1 ()
    {
        using var httpClient = new HttpClient ();
        httpClient.BaseAddress =
            new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

        var httpResponseMessage = await httpClient.GetAsync(string.Empty).ConfigureAwait(false);

        var body = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        Assert.Equals(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.That(body, Does.Contain("Welcome"));
    }


    public void Dispose ()
    {
        _cts.Dispose();
    }
}