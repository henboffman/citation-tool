using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FluentValidation;
using Blazored.LocalStorage;
using CitationTool.Client;
using CitationTool.Client.Models;
using CitationTool.Client.Services;
using CitationTool.Client.Validators;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP Client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Local Storage
builder.Services.AddBlazoredLocalStorage();

// Storage Services
builder.Services.AddScoped<IStorageService, IndexedDbStorageService>();

// Application Services
builder.Services.AddScoped<ICitationService, CitationService>();
builder.Services.AddScoped<IDomainService, DomainService>();
builder.Services.AddScoped<IImportExportService, ImportExportService>();
builder.Services.AddScoped<IUrlHealthService, UrlHealthService>();
builder.Services.AddScoped<SeedDataService>();

// Validators
builder.Services.AddScoped<IValidator<Citation>, CitationValidator>();
builder.Services.AddScoped<IValidator<Domain>, DomainValidator>();

var host = builder.Build();

// Seed data on first run
using (var scope = host.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<SeedDataService>();
    await seedService.SeedIfEmptyAsync();
}

await host.RunAsync();
