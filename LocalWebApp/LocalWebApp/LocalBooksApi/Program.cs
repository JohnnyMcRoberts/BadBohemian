using LocalBooksApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AppStartup.SetupServices(builder);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


//IConfigurationSection? corsOrigin = builder.Configuration.GetSection("Cors");
//string? allowedCorsOrigin = corsOrigin["AllowedOrigin"];
//List<string>? allowedOriginsList = new List<string>();
//foreach (string? origin in allowedCorsOrigin.Split(','))
//{
//    Console.WriteLine("Allowed origin:" + origin);
//    allowedOriginsList.Add(origin.Trim());
//}
List<string>? allowedOriginsList = new List<string>()
{
    "http://localhost:5816",
    "https://localhost:58255",
    "https://localhost:4200",
    "http://localhost:4200",
    "*"
};

app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(origin => true)
    .WithExposedHeaders("content-disposition")
    .AllowCredentials());

app.MapControllers();

app.Run();
