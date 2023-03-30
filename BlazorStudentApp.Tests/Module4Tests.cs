using BlazorStudentApp.Data.Models;
using BlazorStudentApp.Data.Services;
using BlazorStudentApp.Pages.Student;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BlazorStudentApp.Tests;

public class Module4Tests
{
    [Fact]
    public void Submit_Event_ShouldFail_NewStudentComponent_Test()
    {
        //Arrange
        var student = new Student()
        {
            Id = 1,
            Name = "Ervis Trupja",
            Email = "ervis@trupja.com",
            Phone = "12345678",
            Address = "Address sample value"
        };
        var mockedService = new Mock<IStudentsService>();
        mockedService.Setup(s => s.AddStudentAsync(student)).ReturnsAsync(student);

        using var ctx = new TestContext();
        ctx.Services.AddScoped(provider => mockedService.Object);
        var renderedComponent = ctx.RenderComponent<NewStudent>();

        //Act
        var submitButton = renderedComponent.Find("button[type=submit]");
        submitButton.Click();

        //Assert
        Assert.Contains(@"<div class=""validation-message"">The Name field is required.</div>", renderedComponent.Markup);
        Assert.Contains(@"<div class=""validation-message"">The Email field is required.</div>", renderedComponent.Markup);
        Assert.Contains(@"<div class=""validation-message"">The Phone field is required.</div>", renderedComponent.Markup);
        Assert.Contains(@"<div class=""validation-message"">The Address field is required.</div>", renderedComponent.Markup);

        Assert.False(renderedComponent.Instance.isSuccessMessageVisible);
    }

    [Fact]
    public void Submit_Event_ShouldSucceed_NewStudentComponent_Test()
    {
        //Arrange
        var student = new Student()
        {
            Id = 1,
            Name = "Ervis Trupja",
            Email = "ervis@trupja.com",
            Phone = "12345678",
            Address = "Address sample value"
        };
        var mockedService = new Mock<IStudentsService>();
        mockedService.Setup(s => s.AddStudentAsync(student)).ReturnsAsync(student);

        using var ctx = new TestContext();
        ctx.Services.AddScoped(provider => mockedService.Object);
        var renderedComponent = ctx.RenderComponent<NewStudent>();

        string expectedStudentFullName = "Ervis Trupja";

        renderedComponent.Find("#name").Change(expectedStudentFullName);
        renderedComponent.Find("#email").Change("Ervis@Trupja.com");
        renderedComponent.Find("#phone").Change("12345678");
        renderedComponent.Find("#address").Change("Ervis Trupja sample address");

        //Act
        var submitButton = renderedComponent.Find("button[type=submit]");
        submitButton.Click();

        //Assert
        Assert.True(renderedComponent.Instance.isSuccessMessageVisible);
        Assert.Equal(expectedStudentFullName, renderedComponent.Instance.studentName);
    }

    [Fact]
    public void Render_NewStudentComponent_Test()
    {
        //Arrange
        var student = new Student();
        var mockedService = new Mock<IStudentsService>();
        mockedService.Setup(s => s.AddStudentAsync(student)).ReturnsAsync(student);

        using var ctx = new TestContext();
        ctx.Services.AddScoped(provider => mockedService.Object);
        var renderedComponent = ctx.RenderComponent<NewStudent>();

        //Act
        renderedComponent.Render();
        renderedComponent.Render();

        //Assert
        Assert.Equal(3, renderedComponent.RenderCount);
    }

    [Fact]
    public void AsyncAwait_Parameter_IndexComponent_Test()
    {
        //Arrage
        string expectedStringValue = "Welcome to Blazor Unit Testing with bUnit";
        using var ctx = new TestContext();
        var textService = new TaskCompletionSource<string>();
        var rC = ctx.RenderComponent<Pages.Index>(p => p
            .Add(ts => ts.TextService, textService.Task));

        //Act
        textService.SetResult(expectedStringValue);
        rC.WaitForState(() => rC.Find("h6").TextContent == expectedStringValue);

        //Assert
        Assert.Contains(@"<h6>" + expectedStringValue + "</h6>", rC.Markup);
    }

    [Fact]
    public void ElementNotFoundException_Thrown_NewStudentComponent_Test()
    {
        //Arrange
        var student = new Student();
        var mockedService = new Mock<IStudentsService>();
        mockedService.Setup(s => s.AddStudentAsync(student)).ReturnsAsync(student);

        using var ctx = new TestContext();
        ctx.Services.AddScoped(provider => mockedService.Object);
        var renderedComponent = ctx.RenderComponent<NewStudent>();

        //Act
        //Assert
        Assert.Throws<Bunit.ElementNotFoundException>(() => renderedComponent.Find("button[type=submitdoesnotexist]"));
    }

    [Fact]
    public void MissingEventHandlerException_Thrown_NewStudentComponent_Test()
    {
        //Arrange
        var student = new Student();
        var mockedService = new Mock<IStudentsService>();
        mockedService.Setup(s => s.AddStudentAsync(student)).ReturnsAsync(student);

        using var ctx = new TestContext();
        ctx.Services.AddScoped(provider => mockedService.Object);
        var renderedComponent = ctx.RenderComponent<NewStudent>();

        //Act
        var h1Element = renderedComponent.Find("h1");

        //Assert
        Assert.Throws<Bunit.MissingEventHandlerException>(() => h1Element.Click());
    }

}