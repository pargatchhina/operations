using Operations.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Operations;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Operations
{
    public class Startup : FunctionsStartup
	{
		public IConfiguration Configuration { get; private set; }

		public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.AddConfiguration(configBuilder =>
            {
                Configuration = configBuilder
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.AddKeyVault()
					.AddEnvironmentVariables()
                    .Build();
            });

			// Bind options 
			builder.Services.BindConfigurationOptions(Configuration);

			// explicitly call ConfigureServices to setup DI 
			builder.Services.ConfigureServices();
		}
	}
}
