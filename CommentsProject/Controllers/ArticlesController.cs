using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using CommentsProject.Exceptions;
using CommentsProject.Services.Interfaces;
using CommentsProject.Models;
using CommentsProject.Models.ViewModels;

namespace CommentsProject.Controllers
{
    public class ArticlesController : Controller
    {
        private IArticleAdapter _articleAdapter;

        public ArticlesController(IArticleAdapter articleAdapter)
        {
            _articleAdapter = articleAdapter;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            FilteredModels<Article> filteredModels;

            try
            {
                filteredModels = await _articleAdapter.GetArticlesByPage(page);
            }
            catch (AdapterException ex)
            {
                return View(new ArticlesViewModel
                {
                    Error = new ErrorViewModel { Text = ex.Message }
                });
            }

            return View(new ArticlesViewModel
            {
                Articles = filteredModels
            });
        }
    }
}
