using Entities;
using FoodDiaryApp.Services;
using Google.Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});


// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the OpenAI service
var apiKey = builder.Configuration["OpenAI:ApiKey"];
builder.Services.AddSingleton(new CalorieEstimatorService(apiKey));

string credentialPath = "C:\\Project\\Food Diary Application\\FoodDiaryApp\\ApiDocs\\food-diary-427611-d71dbf0ba0b4.json";
string nutritionApiKey = "aff171108f0c00dc99027a7011049451";
string nutritionAppId = "1eaa9d86";
builder.Services.AddSingleton(new GoogleCloudAiService(credentialPath, nutritionApiKey, nutritionAppId));

string textAnalyticsEndpoint = builder.Configuration["AzureService:TextAnalyticsEndpoint"];
string textAnalyticsApiKey = builder.Configuration["AzureService:TextAnalyticsApiKey"];
string nutritionixApiKey = builder.Configuration["Nutritionix:ApiKey"];
string nutritionixAppId = builder.Configuration["Nutritionix:AppId"];

builder.Services.AddSingleton(new AiNutritionService(nutritionixApiKey, nutritionixAppId, textAnalyticsEndpoint, textAnalyticsApiKey));

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
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
