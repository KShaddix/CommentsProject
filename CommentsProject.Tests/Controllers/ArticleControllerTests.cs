using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

using CommentsProject.Controllers;
using CommentsProject.Services.Interfaces;
using CommentsProject.Models.ViewModels;
using CommentsProject.Exceptions;
using Microsoft.AspNetCore.Http;
using CommentsProject.Models;

namespace CommentsProject.Tests.Controllers
{
    public class ArticleControllerTests
    {
        [Fact]
        public async void IndexGet_Success()
        {
            // Arrange
            var articleAdapterMock = new Mock<IArticleAdapter>();
            articleAdapterMock
                .Setup(aa => aa.GetArticleById(1))
                .Returns(Task.FromResult(new Models.Article { Id = 1, Text = "Test", CommentsCount = 0 }));

            var commentAdapterMock = new Mock<ICommentAdapter>();
            commentAdapterMock
                .Setup(ca => ca.GetCommentsByPage(1, 1, 10))
                .Returns(Task.FromResult(new FilteredModels<CommentViewModel>
                {
                    Items = new List<CommentViewModel>()
                }));

            var controller = new ArticleController(articleAdapterMock.Object, commentAdapterMock.Object, null);

            // Act
            var actual = await controller.Index(1, 1);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Null(model.Error);
            Assert.Equal(1, model.Article.Id);
            Assert.Empty(model.Comments.Items);
        }

        [Fact]
        public async void IndexGet_Article_DoesntExist()
        {
            // Arrange
            var controller = new ArticleController(null, null, null);

            // Act
            var actual = await controller.Index(0, 1);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Equal("Данная статья не существует", model.Error.Text);
        }

        [Fact]
        public async void IndexGet_Article_NotFound()
        {
            // Arrange
            var articleAdapterMock = new Mock<IArticleAdapter>();
            articleAdapterMock
                .Setup(aa => aa.GetArticleById(1))
                .Throws(new AdapterException(StatusCodes.Status404NotFound, "Данная статья не найдена"));

            var controller = new ArticleController(articleAdapterMock.Object, null, null);

            // Act
            var actual = await controller.Index(1, 1);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Equal("Данная статья не найдена", model.Error.Text);
        }

        [Fact]
        public async void IndexGet_Page_DoesntHave_Comments()
        {
            // Arrange
            var articleAdapterMock = new Mock<IArticleAdapter>();
            articleAdapterMock
                .Setup(aa => aa.GetArticleById(1))
                .Returns(Task.FromResult(new Models.Article { Id = 1, Text = "Test", CommentsCount = 2 }));

            var commentAdapterMock = new Mock<ICommentAdapter>();
            commentAdapterMock
                .Setup(ca => ca.GetCommentsByPage(1, 3, 10))
                .Throws(new AdapterException(StatusCodes.Status404NotFound, "На данной странице нет комментариев"));

            var controller = new ArticleController(articleAdapterMock.Object, commentAdapterMock.Object, null);

            // Act
            var actual = await controller.Index(1, 3);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Equal("На данной странице нет комментариев", model.Error.Text);
        }

        [Fact]
        public async void IndexGet_Page_Has_Comments()
        {
            // Arrange
            var articleAdapterMock = new Mock<IArticleAdapter>();
            articleAdapterMock
                .Setup(aa => aa.GetArticleById(1))
                .Returns(Task.FromResult(new Models.Article { Id = 1, Text = "Test", CommentsCount = 2 }));

            var commentAdapterMock = new Mock<ICommentAdapter>();
            commentAdapterMock
                .Setup(ca => ca.GetCommentsByPage(1, 1, 10))
                .Returns(Task.FromResult(new FilteredModels<CommentViewModel>
                {
                    Items = new List<CommentViewModel>
                    {
                        new CommentViewModel { Id = 1 },
                        new CommentViewModel { Id = 2 }
                    }
                }));

            var controller = new ArticleController(articleAdapterMock.Object, commentAdapterMock.Object, null);

            // Act
            var actual = await controller.Index(1, 1);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Equal(2, model.Comments.Items.Count);
        }

        [Fact]
        public async void IndexPost_Success()
        {
            // Arrange
            var articleAdapterMock = new Mock<IArticleAdapter>();
            articleAdapterMock
                .Setup(aa => aa.GetArticleById(1))
                .Returns(Task.FromResult(new Models.Article { Id = 1, Text = "Test", CommentsCount = 0 }));

            var commentAdapterMock = new Mock<ICommentAdapter>();
            commentAdapterMock
                .Setup(ca => ca.GetCommentsByPage(1, 1, 10))
                .Returns(Task.FromResult(new FilteredModels<CommentViewModel>
                {
                    Items = new List<CommentViewModel>()
                }));

            var userAdapterMock = new Mock<IUserAdapter>();
            userAdapterMock
                .Setup(ca => ca.GetById(1))
                .Returns(Task.FromResult(new Entities.User { Id = 1 }));

            var createCommentRequest = new CreateCommentRequest
            {
                Text = "Test",
                ArticleId = 1,
                UserId = 1,
                ParentId = null
            };

            var controller = new ArticleController(articleAdapterMock.Object, commentAdapterMock.Object, userAdapterMock.Object);

            // Act
            var actual = await controller.Index(createCommentRequest);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Null(model.Error);
            Assert.Equal(1, model.Article.Id);
            Assert.Empty(model.Comments.Items);
        }

        [Fact]
        public async void IndexPost_Article_DoesntExist()
        {
            // Arrange
            var createCommentRequest = new CreateCommentRequest
            {
                Text = "Test",
                ArticleId = 0,
                UserId = 1,
                ParentId = null
            };

            var controller = new ArticleController(null, null, null);

            // Act
            var actual = await controller.Index(createCommentRequest);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Equal("Данная статья не существует", model.Error.Text);
        }

        [Fact]
        public async void IndexPost_User_DoesntExist()
        {
            // Arrange
            var createCommentRequest = new CreateCommentRequest
            {
                Text = "Test",
                ArticleId = 1,
                UserId = 0,
                ParentId = null
            };

            var controller = new ArticleController(null, null, null);

            // Act
            var actual = await controller.Index(createCommentRequest);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Equal("Данный пользователь не существует", model.Error.Text);
        }

        [Fact]
        public async void IndexPost_Comment_DoesntExist()
        {
            // Arrange
            var createCommentRequest = new CreateCommentRequest
            {
                Text = "Test",
                ArticleId = 1,
                UserId = 1,
                ParentId = 0
            };

            var controller = new ArticleController(null, null, null);

            // Act
            var actual = await controller.Index(createCommentRequest);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Equal("Комментарий, под которым было оставлено сообщение, не найден", model.Error.Text);
        }

        [Fact]
        public async void IndexPost_User_NotFound()
        {
            // Arrange
            var articleAdapterMock = new Mock<IArticleAdapter>();
            articleAdapterMock
                .Setup(aa => aa.GetArticleById(1))
                .Returns(Task.FromResult(new Models.Article { Id = 1, Text = "Test", CommentsCount = 0 }));

            var userAdapterMock = new Mock<IUserAdapter>();
            userAdapterMock
                .Setup(ca => ca.GetById(1))
                .Throws(new AdapterException(StatusCodes.Status404NotFound, "Данный пользователь не найден"));

            var createCommentRequest = new CreateCommentRequest
            {
                Text = "Test",
                ArticleId = 1,
                UserId = 1,
                ParentId = null
            };

            var controller = new ArticleController(articleAdapterMock.Object, null, userAdapterMock.Object);

            // Act
            var actual = await controller.Index(createCommentRequest);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Equal("Данный пользователь не найден", model.Error.Text);
        }

        [Fact]
        public async void IndexPost_ParentComment_NotFound()
        {
            // Arrange
            var articleAdapterMock = new Mock<IArticleAdapter>();
            articleAdapterMock
                .Setup(aa => aa.GetArticleById(1))
                .Returns(Task.FromResult(new Models.Article { Id = 1, Text = "Test", CommentsCount = 0 }));

            var commentAdapterMock = new Mock<ICommentAdapter>();
            commentAdapterMock
                .Setup(ca => ca.GetCommentById(1))
                .Throws(new AdapterException(StatusCodes.Status404NotFound, "Комментарий, под которым было оставлено сообщение, не найден"));

            var userAdapterMock = new Mock<IUserAdapter>();
            userAdapterMock
                .Setup(ca => ca.GetById(1))
                .Returns(Task.FromResult(new Entities.User { Id = 1 }));

            var createCommentRequest = new CreateCommentRequest
            {
                Text = "Test",
                ArticleId = 1,
                UserId = 1,
                ParentId = 1
            };

            var controller = new ArticleController(articleAdapterMock.Object, commentAdapterMock.Object, userAdapterMock.Object);

            // Act
            var actual = await controller.Index(createCommentRequest);

            // Assert
            var view = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsType<ArticleViewModel>(view.Model);
            Assert.Equal("Комментарий, под которым было оставлено сообщение, не найден", model.Error.Text);
        }
    }
}
