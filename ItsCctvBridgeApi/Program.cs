using ItsCctvBridgeApi.Common;
using ItsCctvBridgeApi.Services;

namespace ItsCctvBridgeApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddHttpClient<ItsCctvService>(client =>
            {
                client.BaseAddress = new Uri(AppCommon.baseUrl);
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();

            // CORS 설정 - 외부서버 접근허용
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.UseCors("AllowAll"); // CORS 사용

            app.Run();
        }
    }
}
