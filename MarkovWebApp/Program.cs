using MarkovWebApp.Data;
using MarkovWebApp.Hubs;
using MarkovWebApp.Logic;
using MarkovWebApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<MarkovDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<MarkovDbService>();
builder.Services.AddScoped<MarkovGenerator>();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(options =>
{
    // Geliştirme ortamında, sunucudaki hatanın detaylarını istemciye (tarayıcıya) gönder.
    // Bu, hatayı doğrudan tarayıcı konsolunda görmemizi sağlar.
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapHub<GeneratorHub>("/generatorHub");

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
