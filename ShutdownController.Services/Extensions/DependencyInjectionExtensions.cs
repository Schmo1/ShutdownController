
using Microsoft.Extensions.DependencyInjection;
using ShutdownController.Services.Abstraction;

namespace ShutdownController.Services.Extensions;

public static class DependencyInjectionExtensions
{
	public static void AddServices(this IServiceCollection serviceProvider)
	{
		serviceProvider.AddTransient<IEachSecondTick, EachSecondTick>();
	}
}
