using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LEIUTAD.Data;
using LEIUTAD.Filters;

var builder = WebApplication.CreateBuilder(args);

// Configuração do base de dados
builder.Services.AddDbContext<LEIUTADContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Configuração dos controladores com filtros
builder.Services.AddControllersWithViews();

// Adicionar suporte a sessões
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Injeção de dependência para acesso ao HttpContext
builder.Services.AddHttpContextAccessor();

// Adicionar logging à consola
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Inicialização da base de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LEIUTADContext>();
        DBInitialize.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao inicializar a BD");
    }
}

// Configure o pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Ativar sessões

// Autorização
app.UseAuthorization();

// Roteamento
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Livros}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "leitor",
    pattern: "Leitors/{action=Index}/{id?}",
    defaults: new { controller = "Leitor" });

app.Run();
