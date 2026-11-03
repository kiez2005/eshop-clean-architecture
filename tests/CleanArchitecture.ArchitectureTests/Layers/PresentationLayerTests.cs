using CleanArchitecture.Api.Controllers;
using FluentAssertions;
using NetArchTest.Rules;

namespace CleanArchitecture.ArchitectureTests.Layers;

public class PresentationLayerTests
{
    [Fact]
    public void Controllers_ShouldNotHaveDependencyOnRepositories()
    {
        // Arrange
        var assembly = Api.AssemblyReference.Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .Inherit(typeof(BaseApiController))
            .ShouldNot()
            .HaveDependencyOn("CleanArchitecture.Persistence.Repositories")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
