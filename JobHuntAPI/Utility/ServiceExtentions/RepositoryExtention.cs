using DK.GenericLibrary.ServiceCollection;
using JobHuntAPI.Repository;
using Microsoft.EntityFrameworkCore.Design;

namespace JobHuntAPI.Utility.ServiceExtentions
{
	public static class RepositoryExtention
	{
		public static IServiceCollection RegisterRepository(this IServiceCollection services)
		{
			//singletons to fit with applicationservice for caching
			services.AddSingletonAsyncRepository<ApplicationContext>();
			services.AddSingletonRepository<ApplicationContext>();

			services.AddDbContextFactory<ApplicationContext>();
			services.AddTransient<IDesignTimeDbContextFactory<ApplicationContext>, ApplicationContextFactory>();
			return services;
		}
	}
}
