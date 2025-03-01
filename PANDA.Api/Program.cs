using PANDA.Api.Extensions;

// Build
// --------

var builder = WebApplication
    .CreateBuilder(args)
    .InitiateServices(nameof(PANDA), additionalSpaces: [".Services", ".Infrastructure"]);

// Add additional Services

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Run
// --------

var app = builder
    .Build()
    .InitiateApp();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();