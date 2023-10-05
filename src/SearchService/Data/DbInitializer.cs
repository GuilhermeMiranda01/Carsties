using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Entities;
using System.Text.Json;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));
            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();

            if (count is 0)
            {
                Console.WriteLine("Nenhum dado, tentativa de seed");

                var itemData = await File.ReadAllTextAsync("Data/auctions.json");

                var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var items = JsonSerializer.Deserialize<List<Item>>(itemData, opt);

                await DB.SaveAsync(items);
            }
        }
    }
}
