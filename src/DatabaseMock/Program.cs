using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Models;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet(
    "/item/{id}",
    async (string id) => {
        var delay = new Random().Next(20, 1000);
        Console.WriteLine($"Delaying for {delay}ms");
        await Task.Delay(delay);

        return new CachedItem { ItemId = id, ItemValue = Guid.NewGuid().ToString()};
    }
);
app.Run();
