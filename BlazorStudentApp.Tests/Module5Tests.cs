using BlazorStudentApp.Data.Models;
using BlazorStudentApp.Tests.Helpers;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;

namespace BlazorStudentApp.Tests;

public class Module5Tests
{
    [Fact]
    public void UserNotAuthenticated_IndexComponent_Test()
    {
        //Arrange
        using var ctx = new TestContext();
        ctx.AddTestAuthorization();

        //Act
        var renderedComponent = ctx.RenderComponent<Pages.Index>();

        //Assert
        Assert.Contains(@"<h1>Please log in!</h1>", renderedComponent.Markup);
    }

    [Fact]
    public void UserAuthenticated_IndexComponent_Test()
    {
        //Arrange
        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetAuthorized("ervis@trupja.com", AuthorizationState.Authorized);

        //Act
        var renderedComponent = ctx.RenderComponent<Pages.Index>();

        //Assert
        Assert.Contains(@"<h1>Welcome to our school</h1>", renderedComponent.Markup);
    }

    [Fact]
    public async Task HttpClient_Request_SuccessResponse_Test()
    {
        //Arrange
        var studentList = new List<Student>() {
            new Student()
            {
                Id = 1,
                Name = "Ervis Trupja",
                Email = "ervis@trupja.com",
                Phone = "12345678",
                Address = "Address sample value"
            }
        };

        using var ctx = new TestContext();
        var httpMock = ctx.Services.AddMockHttpClient();
        httpMock.When("/getAllStudents").RespondJson(studentList);

        //Act
        var httpClient = ctx.Services.BuildServiceProvider().GetService<HttpClient>();
        var httpClientResponse = await httpClient.GetAsync("/getAllStudents");
        httpClientResponse.EnsureSuccessStatusCode();

        string httpClientResponseJsonContent = httpClientResponse.Content.ReadAsStringAsync().Result;
        List<Student> httpResponseStudents = JsonConvert.DeserializeObject<List<Student>>(httpClientResponseJsonContent);

        //Assert
        Assert.Equal(studentList.Count(), httpResponseStudents.Count());
        Assert.Equal(studentList.First().Name, httpResponseStudents.First().Name);
        Assert.Equal(studentList.First().Email, httpResponseStudents.First().Email);
        Assert.Equal(studentList.First().Phone, httpResponseStudents.First().Phone);
        Assert.Equal(studentList.First().Address, httpResponseStudents.First().Address);
    }

    [Fact]
    public void NavigationManager_Navigate_Test()
    {
        //Arrange
        string expectedNavigateToUrl = "welcome";

        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetAuthorized("ervis@trupja.com", AuthorizationState.Authorized);

        var navigationManager = ctx.Services.GetRequiredService<FakeNavigationManager>();
        var renderedComponent = ctx.RenderComponent<Pages.Index>();

        //Act
        navigationManager.NavigateTo(expectedNavigateToUrl);

        //Assert
        Assert.Equal($"http://localhost/{expectedNavigateToUrl}", navigationManager.Uri);
    }

}