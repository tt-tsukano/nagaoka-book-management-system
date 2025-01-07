using BookManagement.Models;
using BookManagement.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    // �p�X���[�h�|���V�[�̐ݒ��ǉ�
    options.Password.RequireNonAlphanumeric = false;  // ���ꕶ����K�{�Ƃ��Ȃ�
    options.Password.RequireLowercase = true;         // ��������K�{
    options.Password.RequireUppercase = true;         // �啶����K�{
    options.Password.RequireDigit = true;             // ������K�{
    options.Password.RequiredLength = 8;              // �ŏ�������
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// ���[���T�[�r�X�̓o�^
builder.Services.AddScoped<IEmailService, EmailService>();

// �o�b�N�O���E���h�T�[�r�X�̓o�^
builder.Services.AddHostedService<BookLoanReminderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();