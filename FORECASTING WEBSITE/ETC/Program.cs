
using ETCDAL; 
using ETC.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Negotiate;
using ETC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(10);
}); 

builder.Services.AddProtectedBrowserStorage();
builder.Services.AddTransient<ISQLDataAccess, SQLDataAccess>();
builder.Services.AddTransient<IETCData, ETCData>();
builder.Services.AddAuthentication();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddSignalR(options =>
{
    options.HandshakeTimeout = TimeSpan.FromSeconds(90);
    options.KeepAliveInterval = TimeSpan.FromSeconds(120);
    //options.ClientTimeoutInterval = TimeSpan.FromSeconds(480); 
});

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});
var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
