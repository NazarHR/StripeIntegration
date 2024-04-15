
using Stripe;

namespace StripeItegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            StripeConfiguration.ApiKey = "sk_test_51P4QUoGkl2o80QOPWtwGayTGb83KACe0c2c9qoZR2wZVp7bZY3VnSynUbaRjxMMl4VO4162Eyci9wQgWRaH6PyJr00TTOLLtM2";

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
                
            }
            else
            {
                app.UseExceptionHandler();
            }
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
