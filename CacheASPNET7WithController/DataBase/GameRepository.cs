using CacheASPNET7.Model;
using CacheASPNET7.Repositories;

namespace CacheASPNET7.DataBase
{
    public class FakeGameRepository : IGameRepository
    {
        List<Game> _games = new List<Game>();

        public FakeGameRepository()
        {
            _games.Add(new Game { Id = 1, Name ="Mortal Kombat 2", Year= 1993  });
            _games.Add(new Game { Id = 2, Name = "Defender of the Crown", Year = 1988 });
            _games.Add(new Game { Id = 3, Name = "Dune 2", Year = 1993 });
            _games.Add(new Game { Id = 4, Name = "Settlers", Year = 1993 });
        }

        public async Task CreateAsync(Game game)
        {
            await Task.Delay(2000);
            _games.Add(game); 
        }

        public async Task<List<Game>> GetAll()
        {
            await Task.Delay(2000);

            List<Game> games = new List<Game>();

            foreach (var item in _games)
            {
                games.Add(new Game() { Id = item.Id, Name = item.Name, Year = item.Year });
            }

            return games;
        }

        public async Task<Game> GetAsync(int id)
        {
            await Task.Delay(2000);
            return _games.FirstOrDefault(k => k.Id == id);
        }

        public async Task<List<Game>> GetGameByLikeName(string likename)
        {
            await Task.Delay(2000);

            List<Game> games = new List<Game>();

            var matched = _games.Where(k => k.Name.ToLowerInvariant()
            .Contains(likename.ToLowerInvariant())).ToList();

            foreach (var item in matched)
            {
                games.Add(new Game() { Id = item.Id, Name = item.Name, Year = item.Year });
            }

            return games;
        }

        public async Task UpdateAsync(Game game)
        {
            await Task.Delay(2000);
            var g = _games.FirstOrDefault(k => k.Id == game.Id);
            _games.Remove(g);
            _games.Add(game);
        }
    }
}
