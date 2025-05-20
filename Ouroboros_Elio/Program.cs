using BusinessLogicLayer.Dtos.MailUtils;
using BusinessLogicLayer.Extensions;
using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Ouroboros.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(new PayOS(
	clientId: builder.Configuration["PayOS:ClientId"],
	apiKey: builder.Configuration["PayOS:ApiKey"],
	checksumKey: builder.Configuration["PayOS:ChecksumKey"]
));

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Auth/Register";
	options.AccessDeniedPath = "/Auth/AccessDenied";
	options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
	//options.SlidingExpiration = true;
	//options.Cookie.HttpOnly = true;
	//options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Add services to the container.
builder.AddPresentationLayer();
builder.Services.AddBusinessLogicLayer();

builder.Services.AddScoped<IDesignRepository, DesignRepository>();
builder.Services.AddScoped<IModelRepository, ModelRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<OuroborosContext>()
.AddDefaultTokenProviders();
var mailsettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsettings);

builder.Services.AddDbContext<OuroborosContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("AllowSpecificOrigins", policy =>
//	{
//		policy.WithOrigins("http://localhost:3000")
//			  .AllowAnyHeader()
//			  .AllowAnyMethod();
//	});
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Register}/{id?}");

app.Run();
