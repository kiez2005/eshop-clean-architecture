using CleanArchitecture.ArchitectureTests.Constants;
using CleanArchitecture.Domain.Common;
using FluentAssertions;
using NetArchTest.Rules;

namespace CleanArchitecture.ArchitectureTests.Layers;

public sealed class DomainLayerTests
{
    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOnAnyOtherLayer()
    {
        // Arrange
        var assembly = Domain.AssemblyReference.Assembly;
        string[] layers =
        [
            Namespace.Application,
            Namespace.Infrastructure,
            Namespace.Persistence,
            Namespace.Identity,
            Namespace.Presentation
        ];

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(layers)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEvents_ShouldBeSealedAndHaveNameEndingWithDomainEvent()
    {
        // Arrange
        var assembly = Domain.AssemblyReference.Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
