using CacheASPNET7.DataBase;
using CacheASPNET7.Model;
using CacheASPNET7.Repositories;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.OutputCaching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IGameRepository, FakeGameRepository>();

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(x => x.With
        (xx => 
        xx.HttpContext.Request.Query["nocache"] == 1)
        .NoCache());

    options.AddPolicy("nocache", x => x.NoCache());
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseOutputCache();


app.MapGet("games/{id}", async (int id, IGameRepository repo)
    =>
{
    var game = await repo.GetAsync(id);

    if (game is null)
        return Results.NotFound();

    return Results.Ok(game);
}
).CacheOutput();


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
).CacheOutput(x => x.VaryByQuery("likename").Expire(TimeSpan.FromSeconds(20)));



app.MapPut("games", [OutputCache(PolicyName ="nocache")]async (Game game, IGameRepository repo)
    =>
    {
        await repo.UpdateAsync(game);
        return Results.Ok(game);
    }
);


app.MapPost("games", async (Game game, IGameRepository repo)
    =>
{
    await repo.CreateAsync(game);
    return Results.Created($"games/{game.Id}", game);
}
);


app.Run();

