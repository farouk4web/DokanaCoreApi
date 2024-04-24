using Dokana.Models;
using Dokana.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration;

// Add services to the container.
// My services
builder.Services.AddScoped<IAppMethodsService, AppMethodsService>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
      );

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Dokana",
        Description =
                "E-commerce website created using asp.net core web api in (.net core v6) \r\n\r\n" +
                "We have 4 roles { Owners, admins, sellers, shippingStaff } \r\n\r\n" +
                "You can use role names to filter users on Users Get EndPoint \r\n\r\n" +
                "And you can filter orders by level {notPaid, paid, confirmed, shipping, delivered} \r\n\r\n" +
                "And you can filter products Too by category Id \r\n\r\n" +
                "And You can search Too on all of products \r\n\r\n" +
                "And also we have pagenation on products and orders and users \r\n\r\n" +
                "You can checkout with {Cash On Delivery} onlay at now, \r\n\r\n" +
                "On checkout end point add 'paymentMethodId' with (1) for cash on delivery \r\n\r\n"
        ,
        Contact = new OpenApiContact
        {
            Name = "Farouk Abdelhamid",
            Email = "FaroukAbdelhamid.eg@gmail.com",
            Url = new Uri("https://farouk4web.github.io")
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});






var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHttpsRedirection();

// Authantication And Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
