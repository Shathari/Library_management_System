using LMS.Razor.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Register the Database helper (scoped per request)
builder.Services.AddScoped<Database>();
builder.Services.AddScoped<BooksRepository>();
builder.Services.AddScoped<LoansRepository>();

// If you have repository classes, prefer injecting Database (recommended)
// builder.Services.AddScoped<BooksRepository>();
// builder.Services.AddScoped<LoansRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();