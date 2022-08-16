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
    options.AddBasePolicy(builder => builder.With
        (xx => 
        xx.HttpContext.Request.Query["nocache"] == 1)
        .NoCache());

    options.AddPolicy("nocache", x => x.NoCache());

    options.AddBasePolicy(builder =>
    {
        builder.With(p => p.HttpContext.Request.Headers.ContainsKey("X-Cached"));
    });
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
).CacheOutput(x => x.NoCache());


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
).CacheOutput("nocache");



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





var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var wf = app.MapGroup("weatherforecast").CacheOutput();

wf.MapGet("", async () =>
{
    await Task.Delay(1000);
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});



app.Run();



internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

