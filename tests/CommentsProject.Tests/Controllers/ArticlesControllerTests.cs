using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

using CommentsProject.Controllers;
using CommentsProject.Services.Interfaces;
using CommentsProject.Models;
using CommentsProject.Models.ViewModels;
using CommentsProject.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CommentsProject.Tests.Controllers
{
    public class ArticlesControllerTests
    {
        [Fact]
        public async void Index_Success()
        {
            // Arrange
            var articleAdapterMock = new Mock<IArticleAdapter>();
            articleAdapterMock
                .Setup(aa => aa.GetArticlesByPage(1, 20))
                .Returns(Task.FromResult(new FilteredModels<Article>
                {
                    Items = new List<Article>
                    {
                        new Article { Id = 1, Text = "Test1", CommentsCount = 0 },
                        new Article { Id = 2, Text = "Test2", CommentsCount = 0 }
                    }
                }));

            var controller = new ArticlesController(articleAdapterMock.Object);

            // Act
            var actual = await controller.Index(1);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticlesViewModel>(view.Model);
            Assert.Null(model.Error);
            Assert.Equal(2, model.Articles.Items.Count);
        }

        [Fact]
        public async void Index_Articles_NotFound()
        {
            // Arrange
            var articleAdapterMock = new Mock<IArticleAdapter>();
            articleAdapterMock
                .Setup(aa => aa.GetArticlesByPage(1, 20))
                .Throws(new AdapterException(StatusCodes.Status404NotFound, "На данной странице нет статей"));

            var controller = new ArticlesController(articleAdapterMock.Object);

            // Act
            var actual = await controller.Index(1);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticlesViewModel>(view.Model);
            Assert.Equal("На данной странице нет статей", model.Error.Text);
        }
    }
}
