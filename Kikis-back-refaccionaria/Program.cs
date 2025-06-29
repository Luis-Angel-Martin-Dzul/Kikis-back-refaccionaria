using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Settings;
using Kikis_back_refaccionaria.Infrastructure.Data;
using Kikis_back_refaccionaria.Infrastructure.Filters;
using Kikis_back_refaccionaria.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

// Exceptions and filters
builder.Services.AddControllersWithViews(opt => {
    opt.Filters.Add<GlobalExceptionFilter>();
    opt.Filters.Add<ValidationFilter>();
});
builder.Services.Configure<ApiBehaviorOptions>(options => {
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var connectionString = builder.Configuration.GetConnectionString("DbContext");
builder.Services.AddDbContext<KikisDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// Services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IServiceEMail, ServiceEMail>();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IServiceCatalogs, ServiceCatalogs>();
builder.Services.AddTransient<IServiceClient, ServiceClient>();
builder.Services.AddTransient<IServiceDelivery, ServiceDelivery>();
builder.Services.AddTransient<IServiceProduct, ServiceProduct>();
builder.Services.AddTransient<IServiceSale, ServiceSale>();
builder.Services.AddTransient<IServiceSupplier, ServiceSupplier>();
builder.Services.AddTransient<IServiceUser, ServiceUser>();

// Cors
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
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
app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
