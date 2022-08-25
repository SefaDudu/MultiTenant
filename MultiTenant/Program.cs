using Core.Application.Settings;
using Core.Infrastructure;
using Core.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ApplicationDbContext>();
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection("TenantSettings"));
builder.Services.AddInfrastructureService();
await builder.Services.AddPersistenceService();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
