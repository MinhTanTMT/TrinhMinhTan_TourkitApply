using Microsoft.EntityFrameworkCore;
using System;
using TrinhMinhTan_TourkitApply.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        TrinhMinhTanTourkitApplyContext trinhMinhTanTourkitApplyContext = new TrinhMinhTanTourkitApplyContext();
        //trinhMinhTanTourkitApplyContext.Database.Migrate(); 
        SeedData.Seed(trinhMinhTanTourkitApplyContext);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi khi seed dữ liệu: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapRazorPages();

app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=index}/{id?}");

app.Run();
