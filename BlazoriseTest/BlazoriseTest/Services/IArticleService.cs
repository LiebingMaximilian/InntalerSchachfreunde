using InntalerSchachfreunde.Entities;
using InntalerSchachfreunde.Pages;

namespace InntalerSchachfreunde.Services
{
    public interface IArticleService
    {
        public Task<(bool, int)> CreateNewArticle(Article article);
        public Task<bool> AddImageToArticle(InntalerSchachfreunde.Entities.Image image, int articleId);
    }
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ArticleService> _logger;
        public ArticleService(AppDbContext dbContext, ILogger<ArticleService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> AddImageToArticle(Entities.Image image, int articleId)
        {
            try
            {
                image.ArticleId = articleId;
               
                _dbContext.Images.Add(image);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while inserting image");
                return false;
            }
        }

        public async Task<(bool, int)> CreateNewArticle(Article article)
        {
            try
            {
                article.ReleaseDate = DateTime.Now;
                _dbContext.Articles.Add(article);
                await _dbContext.SaveChangesAsync();
                return (true, article.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating new article");
                return (false, -1);
            }
        }
    }
}
