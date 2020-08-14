using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using CommentsProject.Exceptions;
using CommentsProject.Models;
using CommentsProject.Models.ViewModels;
using CommentsProject.Services.Interfaces;

namespace CommentsProject.Controllers
{
    [Route("[controller]")]
    public class ArticleController : Controller
    {
        private IArticleAdapter _articleAdapter;
        private ICommentAdapter _commentAdapter;
        private IUserAdapter _userAdapter;

        public ArticleController(IArticleAdapter articleAdapter, ICommentAdapter commentAdapter, IUserAdapter userAdapter)
        {
            _articleAdapter = articleAdapter;
            _commentAdapter = commentAdapter;
            _userAdapter = userAdapter;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id, int page)
        {
            if (page < 1)
                page = 1;
            try
            {
                if (id < 1)
                    throw new Exception("Данная статья не существует");
            }
            catch (Exception ex)
            {
                return View(new ArticleViewModel
                {
                    Error = new ErrorViewModel { Text = ex.Message }
                });
            }

            return await ShowArticlePage(id, page);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] CreateCommentRequest createCommentRequest)
        {
            try
            {
                if (createCommentRequest.ArticleId < 1)
                    throw new Exception("Данная статья не существует");

                if (createCommentRequest.UserId < 1)
                    throw new Exception("Данный пользователь не существует");

                if (createCommentRequest.ParentId < 1)
                    throw new Exception("Комментарий, под которым было оставлено сообщение, не найден");
            }
            catch (Exception ex)
            {
                return View(new ArticleViewModel
                {
                    Error = new ErrorViewModel { Text = ex.Message }
                });
            }

            try
            {
                await _userAdapter.GetById(createCommentRequest.UserId);
            }
            catch (AdapterException ex)
            {
                return View(new ArticleViewModel
                {
                    Error = new ErrorViewModel { Text = ex.Message }
                });
            }

            // Check parent comment if ParentId isn't null
            if (createCommentRequest.ParentId != null)
            {
                try
                {
                    await _commentAdapter.GetCommentById(createCommentRequest.ParentId.Value);
                }
                catch (AdapterException)
                {
                    return View(new ArticleViewModel
                    {
                        Error = new ErrorViewModel
                        { Text = "Комментарий, под которым было оставлено сообщение, не найден" }
                    });
                }
            }

            await _commentAdapter.Create(new Entities.Comment
            {
                Text = createCommentRequest.Text,
                ArticleId = createCommentRequest.ArticleId,
                UserId = createCommentRequest.UserId,
                ParentId = createCommentRequest.ParentId
            });

            return await ShowArticlePage(createCommentRequest.ArticleId, 1);
        }

        private async Task<IActionResult> ShowArticlePage(int id, int page)
        {
            Article article;
            var comments = new FilteredModels<CommentViewModel>
            {
                Items = new List<CommentViewModel>()
            };

            // Get article
            try
            {
                article = await _articleAdapter.GetArticleById(id);
            }
            catch (AdapterException ex)
            {
                return View(new ArticleViewModel
                {
                    Error = new ErrorViewModel { Text = ex.Message }
                });
            }

            // Get comments
            try
            {
                comments = await _commentAdapter.GetCommentsByPage(id, page, 10);
            }
            catch (AdapterException ex)
            {
                // Comment list can't be empty if there are comments in article
                if (article.CommentsCount != 0 && comments.Items.Count == 0)
                {
                    return View(new ArticleViewModel
                    {
                        Error = new ErrorViewModel { Text = ex.Message }
                    });
                }
            }

            return View(new ArticleViewModel
            {
                Article = article,
                Comments = comments
            });
        }
    }
}
