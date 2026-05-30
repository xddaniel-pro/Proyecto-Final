using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ✅ Registrar controladores
builder.Services.AddControllers();

// ✅ Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Proyec API",
        Version = "v1",
        Description = "Documentación de la API del proyecto Proyec Pag"
    });
});

// ✅ Configurar CORS (permitir solo orígenes de la app web en desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica", policy =>
    {
        policy.WithOrigins("https://localhost:7048", "http://localhost:5091")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("NuevaPolitica");
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
