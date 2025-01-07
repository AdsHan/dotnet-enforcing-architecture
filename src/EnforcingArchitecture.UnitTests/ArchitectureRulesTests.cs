using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Fluent.Syntax.Elements.Types.Classes;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace EnforcingArchitecture.UnitTests;

public class ArchitectureRulesTests
{
    private readonly ITestOutputHelper _outputHelper;

    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(typeof(EnforcingArchitecture.API.AssemblyMetadata).Assembly)
        .Build();

    private static GivenTypesConjunction ResideInNamespaceRule(string namespacePattern)
        => Types().That().ResideInNamespace(namespacePattern, true);

    public ArchitectureRulesTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact(DisplayName = "Architecture - Camadas Inferiores Não Podem Depender das Camadas Externas")]
    [Trait("Layer", "Architecture - Layers")]
    public void CheckDependencyRule()
    {
        // Arrange            
        var domain = ResideInNamespaceRule("EnforcingArchitecture.API.Domain.*");
        var application = ResideInNamespaceRule("EnforcingArchitecture.API.Application.*");
        var controllers = ResideInNamespaceRule("EnforcingArchitecture.API.Controllers.*");
        var infrastructure = ResideInNamespaceRule("EnforcingArchitecture.API.Infrastructure.*");

        // Act
        var rule = domain
            .Should().OnlyDependOn(domain)
            .And(infrastructure.Should().NotDependOnAny(application))
            .And(application.Should().NotDependOnAny(controllers));

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Controllers Não Podem Depender da Camada de Infraestrutura")]
    [Trait("Layer", "Architecture - Layers")]
    public void ControllersShouldNotDependDirectlyOnRepositories()
    {
        // Arrange            
        var controllers = ResideInNamespaceRule("EnforcingArchitecture.API.Controllers.*");
        var infrastructure = ResideInNamespaceRule("EnforcingArchitecture.API.Infrastructure.*");

        // Act
        var rule = controllers.Should().NotDependOnAny(infrastructure);

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Métodos Públicos de Controladores Devem Retornar ApiResponse")]
    [Trait("Layer", "Architecture - Controllers")]
    public void ControllersPublicMethodShouldOnlyReturnApiResponse()
    {
        // Arrange
        var controllers = ResideInNamespaceRule("EnforcingArchitecture.API.Controllers.*");

        var publicMethods = MethodMembers()
            .That()
            .ArePublic()
            .And()
            .AreNoConstructors()
            .And()
            .AreDeclaredIn(controllers);

        // Act
        var rule = publicMethods.Should().HaveReturnType(typeof(Task<IActionResult>));

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Todos os DTOs Devem Ser Records")]
    [Trait("Layer", "Architecture - Application")]
    public void DtosShouldBeRecords()
    {
        // Arrange
        var dtos = ResideInNamespaceRule("EnforcingArchitecture.API.Application.DTO.*");

        // Act
        var rule = ArchRuleDefinition.Classes().That().Are(dtos).Should().BeRecord();

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Command Handlers Devem Ter Sufixo 'CommandHandler'")]
    [Trait("Layer", "Architecture - Application")]
    public void CommandHandlersShouldBeSuffixedByCommandHandler()
    {
        // Arrange
        var commandHandlers = Classes()
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>));

        // Act
        var rule = commandHandlers.Should()
            .HaveNameEndingWith("CommandHandler").OrShould()
            .HaveNameEndingWith("QueryHandler");

        // Opcional: Imprimir os tipos para verificação
        PrintResult(commandHandlers);

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Repositórios Devem Implementar Interfaces")]
    [Trait("Layer", "Architecture - Infrastructure")]
    public void RepositoriesShouldImplementInterfaces()
    {
        // Arrange            
        var repositories = ResideInNamespaceRule("EnforcingArchitecture.API.Infrastructure.Repositories.*");

        // Act
        var rule = repositories.Should().ImplementInterface("EnforcingArchitecture.API.Domain.Interfaces.*", true);

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Repositórios Não Podem Fazer Chamadas HTTP")]
    [Trait("Layer", "Architecture - Infrastructure")]
    public void RepositoriesShouldNotDependOnHttpClientOrHttpClasses()
    {
        // Arrange            
        var repositories = ResideInNamespaceRule("EnforcingArchitecture.API.Infrastructure.Repositories");
        var httpClasses = Types().That()
            .ResideInNamespace("System.Net.Http")
            .Or().HaveName("HttpClient")
            .Or().HaveName("HttpRequestMessage")
            .Or().HaveName("HttpResponseMessage");

        // Act
        var rule = repositories.Should().NotDependOnAny(httpClasses);

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Domínio Não Pode Depender de Bibliotecas de Terceiros")]
    [Trait("Layer", "Architecture - Domain")]
    public void DomainShouldNotDependOnThirdPartyLibraries()
    {
        // Arrange
        var domain = ResideInNamespaceRule("EnforcingArchitecture.API.Domain.*");
        var thirdPartyLibraries = ResideInNamespaceRule("AutoMapper|EntityFrameworkCore");

        // Act
        var rule = domain.Should().NotDependOnAny(thirdPartyLibraries);

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Objetos de Domínio Não Podem Depender de Bibliotecas Externas")]
    [Trait("Layer", "Architecture - Domain")]
    public void DomainObjectsShouldNotDependOnExternalLibraries()
    {
        // Arrange
        var domainObjects = ResideInNamespaceRule("EnforcingArchitecture.API.Domain.*");

        // Act
        var rule = domainObjects.Should().NotDependOnAny("AutoMapper|EntityFrameworkCore", true);

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Métodos Get Não Devem Retornar Void")]
    [Trait("Layer", "Architecture - Naming Convention")]
    public void NoGetMethodShouldReturnVoid()
    {
        // Arrange
        var methods = MethodMembers()
            .That()
            .HaveName("Get[A-Z].*", useRegularExpressions: true);

        // Act
        var rule = methods.Should().NotHaveReturnType(typeof(void));

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Métodos Is ou Has Devem Retornar Booleano")]
    [Trait("Layer", "Architecture - Naming Convention")]
    public void IserAndHaserShouldReturnBooleans()
    {
        // Arrange
        var methods = MethodMembers()
            .That()
            .HaveName("Is[A-Z].*|Has[A-Z].*", useRegularExpressions: true);

        // Act
        var rule = methods.Should().HaveReturnType(typeof(bool));

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Interfaces Devem Começar com I")]
    [Trait("Layer", "Architecture - Naming Convention")]
    public void InterfacesShouldStartWithI()
    {
        // Arrange
        var interfaces = Interfaces();

        // Act
        var rule = interfaces.Should()
            .HaveName("^I[A-Z].*", useRegularExpressions: true);

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Classes de Serviços Devem Ter Sufixo 'Service'")]
    [Trait("Layer", "Architecture - Naming Convention")]
    public void ServicesShouldBeSuffixedByService()
    {
        // Arrange
        var services = ResideInNamespaceRule("Services");

        // Act
        var rule = services.Should().HaveNameEndingWith("Service");

        // Assert
        rule.Check(Architecture);
    }

    [Fact(DisplayName = "Architecture - Convenção de Nomenclatura das Controller")]
    [Trait("Layer", "Architecture - Naming Convention")]
    public void ControllersShouldHaveProperNaming()
    {
        // Arrange            
        var controllers = ResideInNamespaceRule("EnforcingArchitecture.API.Controllers.*");

        // Act
        var rule = controllers.Should().HaveNameEndingWith("Controller");

        // Assert
        rule.Check(Architecture);
    }

    private void PrintResult(GivenClassesConjunction domainTypes)
    {
        foreach (var domainClass in domainTypes.GetObjects(Architecture))
        {
            _outputHelper.WriteLine($"Domain Type: {domainClass.FullName}");
        }
    }
}
