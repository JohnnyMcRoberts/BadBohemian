using BooksControllerUtilities.Settings;

namespace LocalWebApp
{
    public class AppStartup
    {
        public static void SetupServices(WebApplicationBuilder? builder)
        {
            if (builder != null)
            {
                MongoDbSettings mongoDbSettings = new MongoDbSettings();

                // Add our Config object so it can be injected
                builder.Services.Configure<MongoDbSettings>(
                    builder.Configuration.GetSection("MongoDbSettings"));
                builder.Services.Configure<SmtpConfig>(
                    builder.Configuration.GetSection("SmtpConfig"));

                //ConfigurationManager.GetSection("MongoDbSettings"));
                //builder.Services.Configure<SmtpConfig>(ConfigurationManager.GetSection("SmtpConfig"));

                // Add services to the container.
                builder.Services.AddControllersWithViews();

            }
        }
    }
}
