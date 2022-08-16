using CacheASPNET7.Model;

namespace CacheASPNET7.Repositories
{
    public interface IGameRepository
    {
        Task CreateAsync(Game game);

        Task<Game> GetAsync(int id);

        Task UpdateAsync(Game game);

        Task<List<Game>> GetAll();

        Task<List<Game>> GetGameByLikeName(string likename);
    }


}
