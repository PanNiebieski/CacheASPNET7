using CacheASPNET7.DataBase;
using CacheASPNET7.Model;
using CacheASPNET7.Repositories;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IGameRepository, FakeGameRepository>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("games", async (Game game, IGameRepository repo)
    =>
    {
        await repo.CreateAsync(game);
        return Results.Created($"games/{game.Id}", game);
    }
);


app.MapGet("games/{id}", async (int id, IGameRepository repo)
    =>
{
    var game = await repo.GetAsync(id);

    if (game is null)
    {
        return Results.NotFound();

    }

    return Results.Ok(game);
}
);

app.MapGet("games", async (string? likename, IGameRepository repo)
    =>
    {
        if (likename is null)
        {
            var allgames = await repo.GetAll();
            return Results.Ok(allgames);
        }

        var matchedGames = await repo.GetGameByLikeName(likename);
        return Results.Ok(matchedGames);
    }
);
    
app.MapPut("games", async (Game game, IGameRepository repo)
    =>
    {
        await repo.UpdateAsync(game);
        return Results.Ok(game);
    }
);



app.Run();

