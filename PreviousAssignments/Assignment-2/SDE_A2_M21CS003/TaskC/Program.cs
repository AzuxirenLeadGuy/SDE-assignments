namespace TaskC
{
    public static class Program
    {
        internal static string KeyPath = "";
        public static void Main(string[] args)
        {
            if (args.Length == 0) { Console.WriteLine("Please specify the key file for the key to consume the APIs"); return; }
            string path = System.IO.Path.GetFullPath(args[0]);
            System.Console.WriteLine($"Using key ${path}");
            KeyPath = path;
            var builder = WebApplication.CreateBuilder();
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}