using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.OutputCaching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache(options =>
{

});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseOutputCache();

var count1 = 0;
app.MapGet("p1", async (context)
    =>
{
    await Task.Delay(4000);
    await context.Response.WriteAsync(count1++.ToString());
}
).CacheOutput(p => p.Expire(TimeSpan.FromSeconds(3)).AllowLocking(false));


var count2 = 0;
app.MapGet("p2", async (context)
    =>
{
    await Task.Delay(4000);
    await context.Response.WriteAsync(count2++.ToString());
}
).CacheOutput(p => p.Expire(TimeSpan.FromSeconds(3)).AllowLocking(true));

app.Run();

