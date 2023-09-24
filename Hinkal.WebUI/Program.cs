using System.Security.Cryptography;
using Blazored.Toast;
using Hinkal.Domain.Services;
using Hinkal.Domain.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// the key for each auction. can be polished
var aes = Aes.Create();
aes.KeySize = 256;
aes.GenerateKey();

builder.Services.AddSingleton(aes.Key);

// simulate store
var store = new BidRecordStore();
builder.Services.AddSingleton<IBidRecordStore>(store);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddBlazoredToast();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();