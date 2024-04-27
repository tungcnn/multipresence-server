using multipresence_backend.Objects;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
Console.WriteLine("Tung Debug: Port = " + port);

var dbHost = Environment.GetEnvironmentVariable("PGHOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("PGDATABASE") ?? "multipresence";
var dbUser = Environment.GetEnvironmentVariable("PGUSER") ?? "postgres";
var dbPassword = Environment.GetEnvironmentVariable("PGPASSWORD") ?? "123456";

string connectionString = $"Host={dbHost};Port={dbPort};Database = {dbName};User Id = {dbUser};Password = {dbPassword};";


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls($"http://*:{port}/;http://localhost:{port}/");

var app = builder.Build();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
connection.Open();

#region APIs
//Find all players
app.MapGet("/players", async () =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM players", connection);

    using NpgsqlDataReader reader = cmd.ExecuteReader();

    var result = new List<Player>();

    while (await reader.ReadAsync())
    {
        result.Add(new Player(
            id: (int)reader["id"],
            name: (string)reader["name"]
            ));
    }

    reader.Close();
    return Results.Ok(result);
});

//Add new player
app.MapPost("/players/", async (string name) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO players (name) VALUES (($1))", connection)
    {
        Parameters =
        {
            new() {Value = name }
        }
    };

    await using NpgsqlDataReader reader = cmd.ExecuteReader();

    reader.Close();
    return Results.Ok();
});

//Edit player
app.MapPut("/players/", async (Player player) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("UPDATE players set name = ($1) WHERE id = ($2)", connection)
    {
        Parameters =
        {
            new() {Value = player.Name },
            new() {Value = player.Id }
        }
    };

    await using NpgsqlDataReader reader = cmd.ExecuteReader();

    reader.Close();
    return Results.Ok();
});

//Delete player
app.MapDelete("/players/{name}", async (string name) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM players WHERE name = ($1)", connection)
    {
        Parameters =
        {
            new() {Value = name }
        }
    };

    await using NpgsqlDataReader reader = cmd.ExecuteReader();

    reader.Close();
    return Results.Ok();
});

//Get all relationships
app.MapGet("/friends", async () =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM playerrelationships", connection);

    using NpgsqlDataReader reader = cmd.ExecuteReader();

    var result = new List<PlayerRelationship>();

    while (await reader.ReadAsync())
    {
        result.Add(new PlayerRelationship(
            PlayerId: (int)reader["playerId"],
            FriendId: (int)reader["friendId"]
            ));
    }

    reader.Close();
    return Results.Ok(result);
});

//Add friends
app.MapPost("/friends/{player1Id}/{player2Id}", async (int player1Id, int player2Id) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO playerrelationships VALUES (($1), ($2))", connection)
    {
        Parameters =
        {
            new() {Value = player1Id },
            new() {Value = player2Id }
        }
    };

    await using NpgsqlDataReader reader = cmd.ExecuteReader();

    reader.Close();
    return Results.Ok();
});

//Remove friends
app.MapDelete("/friends/{player1Id}/{player2Id}", async (int player1Id, int player2Id) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM playerrelationships WHERE playerId = ($1) AND friendId = ($2)", connection)
    {
        Parameters =
        {
            new() {Value = player1Id },
            new() {Value = player2Id }
        }
    };

    await using NpgsqlDataReader reader = cmd.ExecuteReader();

    reader.Close();
    return Results.Ok();
});

//Get all avatars
app.MapGet("/avatars", async () =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM avatars", connection);

    using NpgsqlDataReader reader = cmd.ExecuteReader();

    var result = new List<Avatar>();

    while (await reader.ReadAsync())
    {
        result.Add(new Avatar(
            playerId: (int)reader["playerId"],
            name: (string)reader["name"]
            ));
    }

    reader.Close();
    return Results.Ok(result);
});

//Add avatar
app.MapPost("/avatars/{playerId}/{avatarName}", async (int playerId, string avatarName) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO avatars VALUES (($1), ($2))", connection)
    {
        Parameters =
        {
            new() {Value = playerId },
            new() {Value = avatarName }
        }
    };

    await using NpgsqlDataReader reader = cmd.ExecuteReader();

    reader.Close();
    return Results.Ok();
});

//Edit avatar
app.MapPut("/avatars/{playerId}/{avatarName}", async (int playerId, string avatarName) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("UPDATE avatars SET name = ($1) WHERE playerId = ($2)", connection)
    {
        Parameters =
        {
            new() {Value = avatarName },
            new() {Value = playerId }
        }
    };

    await using NpgsqlDataReader reader = cmd.ExecuteReader();

    reader.Close();
    return Results.Ok();
});

//Delete avatar
app.MapDelete("/avatars/{playerId}", async (int playerId) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM avatars WHERE playerId = ($1)", connection)
    {
        Parameters =
        {
            new() {Value = playerId }
        }
    };

    await using NpgsqlDataReader reader = cmd.ExecuteReader();

    reader.Close();
    return Results.Ok();
});

//Fetch avatar by name
app.MapGet("/avatar/{name}", async (string name) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand("SELECT a.name FROM players p join avatars a on p.id = a.playerId WHERE p.name = ($1);", connection)
    {
        Parameters =
        {
            new() {Value = name }
        }
    };

    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

    var result = new List<string>();

    while (await reader.ReadAsync())
    {
        result.Add((string)reader["name"]);
    }
    reader.Close();

    if (result.Count > 0) return Results.Ok(result[0]);
    else return Results.NotFound();
});

//Verify relationship
app.MapGet("/friends/{playerName}/{otherName}", async (string playerName, string otherName) =>
{
    using NpgsqlCommand cmd = new NpgsqlCommand(
        "select * from players where name = ($1) and (id in (select pr.friendId from playerrelationships pr join players p on pr.playerId = p.id where p.name = ($2)) OR id in (select pr.playerId from playerrelationships pr join players p on pr.friendId = p.id where p.name = ($3)));", connection)
    {
        Parameters =
        {
            new() {Value = playerName },
            new() {Value = otherName },
            new() {Value = otherName }
        }
    };

    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

    var result = new List<Player>();

    while (await reader.ReadAsync())
    {
        result.Add(new Player());
    }
    reader.Close();
    if (result.Count > 0) return Results.Ok();
    else return Results.NotFound();
});
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
