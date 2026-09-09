using System.Net;
using System.Text;
using FactChecker.Services;
using FluentAssertions;

namespace FactChecker.Tests.Services;

public class CatFactServiceTests
{
    [Fact]
    public async Task GetCatFact_WhenApiReturnsValidPayload_ShouldReturnMappedCatFact()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"fact\":\"Cats have five toes on their front paws.\",\"length\":41}",
                    Encoding.UTF8,
                    "application/json")
            });
        var httpClient = new HttpClient(handler);
        var service = new CatFactService(httpClient);

        var result = await service.GetCatFact("https://catfact.ninja/fact");

        result.Fact.Should().Be("Cats have five toes on their front paws.");
        result.Length.Should().Be(41);
    }

    [Fact]
    public async Task GetCatFact_WhenApiReturnsServerError_ShouldThrowHttpRequestException()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var httpClient = new HttpClient(handler);
        var service = new CatFactService(httpClient);

        var act = async () => await service.GetCatFact("https://catfact.ninja/fact");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetCatFact_WhenApiReturnsNullPayload_ShouldThrowException()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", Encoding.UTF8, "application/json")
            });
        var httpClient = new HttpClient(handler);
        var service = new CatFactService(httpClient);

        var act = async () => await service.GetCatFact("https://catfact.ninja/fact");

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Failed to retrieve cat fact.");
    }

    [Fact]
    public void SaveCatFactToFile_WhenFactIsNull_ShouldThrowArgumentNullException()
    {
        var service = new CatFactService(new HttpClient(new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK))));

        var act = () => service.SaveCatFactToFile(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SaveCatFactToFile_WhenFactTextIsEmpty_ShouldThrowArgumentException()
    {
        var service = new CatFactService(new HttpClient(new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK))));
        var fact = new CatFact { Fact = string.Empty, Length = 0 };

        var act = () => service.SaveCatFactToFile(fact);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Fact cannot be null or empty.*");
    }

    [Fact]
    public void SaveCatFactToFile_WhenFactIsValid_ShouldAppendLineToTxtFile()
    {
        var service = new CatFactService(new HttpClient(new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK))));
        var fact = new CatFact { Fact = "Cats can rotate their ears independently.", Length = 42 };
        var txtPath = Path.Combine(AppContext.BaseDirectory, "catFacts.txt");

        if (File.Exists(txtPath))
        {
            File.Delete(txtPath);
        }

        try
        {
            service.SaveCatFactToFile(fact);

            File.Exists(txtPath).Should().BeTrue();
            var content = File.ReadAllText(txtPath);
            content.Should().Contain("Fact: Cats can rotate their ears independently. ; Length: 42");
        }
        finally
        {
            if (File.Exists(txtPath))
            {
                File.Delete(txtPath);
            }
        }
    }

    [Fact]
    public void SaveCatFactToCSV_WhenFactIsNull_ShouldThrowArgumentNullException()
    {
        var service = new CatFactService(new HttpClient(new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK))));

        var act = () => service.SaveCatFactToCSV(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SaveCatFactToCSV_WhenFactTextIsEmpty_ShouldThrowArgumentException()
    {
        var service = new CatFactService(new HttpClient(new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK))));
        var fact = new CatFact { Fact = string.Empty, Length = 0 };

        var act = () => service.SaveCatFactToCSV(fact);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Fact cannot be null or empty.*");
    }

    [Fact]
    public void SaveCatFactToCSV_WhenFactIsValid_ShouldNotThrow()
    {
        var service = new CatFactService(new HttpClient(new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK))));
        var fact = new CatFact { Fact = "Cats have a strong night vision.", Length = 32 };

        var act = () => service.SaveCatFactToCSV(fact);

        act.Should().NotThrow();
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseFactory(request));
        }
    }
}