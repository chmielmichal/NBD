using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using NBD.Models;
using NBD.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<ComputerDatabaseSettings>(
    builder.Configuration.GetSection("ComputerDatabase"));

builder.Services.AddSingleton<GridFSBucket>(provider =>
{
    var mongoClient = new MongoClient("mongodb://localhost:27017"); // Zmieñ na swoje po³¹czenie MongoDB
    var database = mongoClient.GetDatabase("test"); // Zmieñ na nazwê swojej bazy danych
    return new GridFSBucket(database);
});
builder.Services.AddScoped<IComputerService, ComputerService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
