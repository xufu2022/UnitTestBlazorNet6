using BlazorStudentApp.Data.Models;
using BlazorStudentApp.Data.Services;
using BlazorStudentApp.Pages.Student;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BlazorStudentApp.Tests
{
    public class Module3Test
    {
        [Fact]
        public void basic_mockup_Index_Render_Correctly()
        {
            //Arrange
            using var ctx = new TestContext();
            //act
            var renderComponent = ctx.RenderComponent<Pages.Index>();
            //Assert
            renderComponent.MarkupMatches(
                "<h1>Welcome to our school</h1>\r\n<p>This is the official page to manage students of our School.</p>");
        }

        [Fact]
        public void DOM_Markup_IndexComponentRendersCorrectly_Test()
        {
            //Arrange
            using var ctx = new TestContext();

            //Act
            var renderedComponent = ctx.RenderComponent<Pages.Index>();

            //Assert
            var h1Element = renderedComponent.Find("h1");
            h1Element.MarkupMatches("<h1>Welcome to our school</h1>");

            var pElement = renderedComponent.Find("p");
            pElement.MarkupMatches("<p>This is the official page to manage students of our School.</p>");
        }

        [Fact]
        public void Parameter_Passing_DetailsComponentRendersCorrently_Test()
        {
            //Arrange
            using var ctx = new TestContext();
            var expectedHeading = "Student details";
            var expectedStudent = new Student()
            {
                Id = 1,
                Name = "Ervis Trupja",
                Email = "ervis@trupja.com",
                Phone = "12345678",
                Address = "Address sample value"
            };

            //Act
            var rC = ctx.RenderComponent<DetailsStudent>(p => p
                .Add(n => n.Heading, expectedHeading)
                .Add(n => n.SelectedStudent, expectedStudent)
            );

            //Assert
            Assert.Contains(@"<h4 class=""modal-title"">" + expectedHeading + "</h4>", rC.Markup);
            Assert.Contains(@"<p class=""card-text"">" + expectedStudent.Address + "</p>", rC.Markup);
            Assert.Contains(@"<h5 class=""card-title"">" + expectedStudent.Name + "</h5>", rC.Markup);

            Assert.Equal(expectedHeading, rC.Instance.Heading);
            Assert.Equal(expectedStudent, rC.Instance.SelectedStudent);
        }

        [Fact]
        public void Service_Injection_AllStudentsComponentRendersCorrently_Test()
        {
            //Arrange
            var studentsList = new List<Student>()
            {
                new Student()
                {
                    Id = 1,
                    Name = "Ervis Trupja",
                    Email = "ervis@trupja.com",
                    Phone = "12345678",
                    Address = "Address sample value"
                }
            };
            var mockedService = new Mock<IStudentsService>();
            mockedService.Setup(s => s.GetStudentsAsync()).ReturnsAsync(studentsList);
            using var ctx = new TestContext();
            ctx.Services.AddScoped(provider => mockedService.Object);
            //Act
            var renderedComponent = ctx.RenderComponent<AllStudents>();

            //Assert
            Assert.Equal(studentsList.Count, renderedComponent.Instance.students.Count);
        }
    }
}