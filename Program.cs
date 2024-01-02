
using MusicalChairs.Game;
using MusicalChairs.Game.Interfaces;
using MusicalChairs.Spotify;
using MusicalChairs.Spotify.Interfaces;

namespace MusicalChairs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ISpotifyTokenService, SpotifyTokenService>();
            builder.Services.AddSingleton<ISpotifyService, SpotifyService>();
            builder.Services.AddSingleton<IGameService, GameService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseWebSockets();

            app.MapControllers();

            app.Run();
        }
    }
}
