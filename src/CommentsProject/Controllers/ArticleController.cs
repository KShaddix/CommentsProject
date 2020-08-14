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

        [HttpPost("{articleId}")]
        public async Task<IActionResult> Index(string text, int articleId, int userId, int? parentId)
        {
            text = text?.Trim();

            try
            {
                if (string.IsNullOrEmpty(text))
                    throw new Exception("Сообщение комментария не может быть пустым");

                if (text.Length > 200)
                    throw new Exception("Длина комментария не должно превышать 200 символов");

                if (articleId < 1)
                    throw new Exception("Данная статья не существует");

                if (userId < 1)
                    throw new Exception("Данный пользователь не существует");

                if (parentId < 1)
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
                await _userAdapter.GetById(userId);
            }
            catch (AdapterException ex)
            {
                return View(new ArticleViewModel
                {
                    Error = new ErrorViewModel { Text = ex.Message }
                });
            }

            // Check parent comment if ParentId isn't null
            if (parentId != null)
            {
                try
                {
                    await _commentAdapter.GetCommentById(parentId.Value);
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
                Text = text,
                ArticleId = articleId,
                UserId = userId,
                ParentId = parentId
            });

            return await ShowArticlePage(articleId, 1);
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
