using App.Security.Requirements;
using App.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using App.Models;

var builder = WebApplication.CreateBuilder(args);
// var connectionString = builder.Configuration.GetConnectionString("MyBlogContextConnection") ?? throw new InvalidOperationException("Connection string 'MyBlogContextConnection' not found.");

builder.Services.AddOptions();
// dùng Configuration để đọc MailSettings từ file config
// đăng ký lớp cấu hình MailSettings được thiết lập dữ liệu từ MailSettings đọc được
// và được Inject vào SendMailService
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Đăng ký dịch vụ SendMailService
builder.Services.AddSingleton<IEmailSender, SendMailService>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options => {
    string connectstring = builder.Configuration["ConnectionStrings:MyBlogContext"];
    options.UseSqlServer(connectstring);
});

// Đăng ký Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
// builder.Services.AddDefaultIdentity<AppUser>()
//                 .AddEntityFrameworkStores<MyBlogContext>()
//                 .AddDefaultTokenProviders();


// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions> (options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lần thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;
});
// Authentication: xác thực danh tính -> Login, Logout
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Identity/Account/Login";
    options.LoginPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication()
                .AddGoogle(googleOptions => {
                    var googleConfig = builder.Configuration.GetSection("web:Google");
                    googleOptions.ClientId = googleConfig["client_id"];
                    googleOptions.ClientSecret = googleConfig["client_secret"];
                    // https://localhost:7237/signin-google
                    googleOptions.CallbackPath = "/dang-nhap-tu-google";
                })
                .AddFacebook(facebookOptions => {
                    var facebookConfig = builder.Configuration.GetSection("web:Facebook");
                    facebookOptions.AppId = facebookConfig["app_id"];
                    facebookOptions.AppSecret = facebookConfig["app_secret"];
                    facebookOptions.CallbackPath = "/dang-nhap-tu-facebook";
                });

builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AllowEditRole", policyBuilder => {
        policyBuilder.RequireAuthenticatedUser();
        // policyBuilder.RequireRole("Admin");
        // policyBuilder.RequireRole("Editor");
        policyBuilder.RequireClaim("duocphepthem", "user");
    });
    options.AddPolicy("InGenZ", policyBuilder => {
        policyBuilder.RequireAuthenticatedUser();
        // policyBuilder.RequireRole("Admin");
        // policyBuilder.RequireRole("Editor");
        // policyBuilder.RequireClaim("duocphepthem", "user");
        policyBuilder.Requirements.Add(new GenZRequirement());   // GenZRequirement
    });
    options.AddPolicy("ShowAdminMenu", policyBuilder => {
        policyBuilder.RequireAuthenticatedUser();

        policyBuilder.RequireRole("Admin");
    });
    options.AddPolicy("CanUpdateArticle", policyBuilder => {
        policyBuilder.Requirements.Add(new ArticleUpdateRequirement()); 
    });
});

builder.Services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();

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

app.MapRazorPages();


app.Run();
