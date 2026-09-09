using FactChecker.Controllers;
using FactChecker.Services;
using FluentAssertions;
using Moq;

namespace FactChecker.Tests.Controllers;

public class CatFactControllerTests
{
    [Fact]
    public async Task Get_WhenServiceReturnsFact_ShouldReturnFactAndSaveToTxtAndCsv()
    {
        var expectedFact = new CatFact
        {
            Fact = "Cats can jump up to six times their length.",
            Length = 43
        };

        var serviceMock = new Mock<ICatFactService>(MockBehavior.Strict);
        serviceMock
            .Setup(s => s.GetCatFact("https://catfact.ninja/fact"))
            .ReturnsAsync(expectedFact);
        serviceMock
            .Setup(s => s.SaveCatFactToFile(expectedFact));
        serviceMock
            .Setup(s => s.SaveCatFactToCSV(expectedFact));

        var controller = new CatFactController(serviceMock.Object);

        var result = await controller.Get();

        result.Should().BeEquivalentTo(expectedFact);
        serviceMock.Verify(s => s.GetCatFact("https://catfact.ninja/fact"), Times.Once);
        serviceMock.Verify(s => s.SaveCatFactToFile(expectedFact), Times.Once);
        serviceMock.Verify(s => s.SaveCatFactToCSV(expectedFact), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_ShouldUseExpectedApiUrl()
    {
        var expectedFact = new CatFact
        {
            Fact = "Cats purr to communicate.",
            Length = 26
        };

        var serviceMock = new Mock<ICatFactService>(MockBehavior.Strict);
        serviceMock
            .Setup(s => s.GetCatFact(It.Is<string>(url => url == "https://catfact.ninja/fact")))
            .ReturnsAsync(expectedFact);
        serviceMock
            .Setup(s => s.SaveCatFactToFile(expectedFact));
        serviceMock
            .Setup(s => s.SaveCatFactToCSV(expectedFact));

        var controller = new CatFactController(serviceMock.Object);

        var result = await controller.Get();

        result.Should().BeSameAs(expectedFact);
        serviceMock.Verify(s => s.GetCatFact("https://catfact.ninja/fact"), Times.Once);
        serviceMock.Verify(s => s.SaveCatFactToFile(expectedFact), Times.Once);
        serviceMock.Verify(s => s.SaveCatFactToCSV(expectedFact), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenGetCatFactThrows_ShouldPropagateExceptionAndNotSaveAnything()
    {
        var serviceMock = new Mock<ICatFactService>(MockBehavior.Strict);
        serviceMock
            .Setup(s => s.GetCatFact("https://catfact.ninja/fact"))
            .ThrowsAsync(new HttpRequestException("API unavailable"));

        var controller = new CatFactController(serviceMock.Object);

        var act = async () => await controller.Get();

        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*API unavailable*");
        serviceMock.Verify(s => s.GetCatFact("https://catfact.ninja/fact"), Times.Once);
        serviceMock.Verify(s => s.SaveCatFactToFile(It.IsAny<CatFact>()), Times.Never);
        serviceMock.Verify(s => s.SaveCatFactToCSV(It.IsAny<CatFact>()), Times.Never);
        serviceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenSaveCatFactToFileThrows_ShouldPropagateExceptionAndNotSaveCsv()
    {
        var expectedFact = new CatFact
        {
            Fact = "Cats sleep most of the day.",
            Length = 30
        };

        var serviceMock = new Mock<ICatFactService>(MockBehavior.Strict);
        serviceMock
            .Setup(s => s.GetCatFact("https://catfact.ninja/fact"))
            .ReturnsAsync(expectedFact);
        serviceMock
            .Setup(s => s.SaveCatFactToFile(expectedFact))
            .Throws(new IOException("Disk write failed"));

        var controller = new CatFactController(serviceMock.Object);

        var act = async () => await controller.Get();

        await act.Should().ThrowAsync<IOException>()
            .WithMessage("*Disk write failed*");
        serviceMock.Verify(s => s.GetCatFact("https://catfact.ninja/fact"), Times.Once);
        serviceMock.Verify(s => s.SaveCatFactToFile(expectedFact), Times.Once);
        serviceMock.Verify(s => s.SaveCatFactToCSV(It.IsAny<CatFact>()), Times.Never);
        serviceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenSaveCatFactToCsvThrows_ShouldPropagateExceptionAfterSavingTxt()
    {
        var expectedFact = new CatFact
        {
            Fact = "Cats use whiskers to navigate tight spaces.",
            Length = 43
        };

        var serviceMock = new Mock<ICatFactService>(MockBehavior.Strict);
        serviceMock
            .Setup(s => s.GetCatFact("https://catfact.ninja/fact"))
            .ReturnsAsync(expectedFact);
        serviceMock
            .Setup(s => s.SaveCatFactToFile(expectedFact));
        serviceMock
            .Setup(s => s.SaveCatFactToCSV(expectedFact))
            .Throws(new IOException("CSV write failed"));

        var controller = new CatFactController(serviceMock.Object);

        var act = async () => await controller.Get();

        await act.Should().ThrowAsync<IOException>()
            .WithMessage("*CSV write failed*");
        serviceMock.Verify(s => s.GetCatFact("https://catfact.ninja/fact"), Times.Once);
        serviceMock.Verify(s => s.SaveCatFactToFile(expectedFact), Times.Once);
        serviceMock.Verify(s => s.SaveCatFactToCSV(expectedFact), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }
}