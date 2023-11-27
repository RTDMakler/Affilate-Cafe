using Cafe.Services;

namespace Cafe
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Другие службы...

            services.AddSingleton<UserService>(provider =>
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "users.json");
                return new UserService(filePath);
            });

            // Другие настройки...
        }

    }
}
