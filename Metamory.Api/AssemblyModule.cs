using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace Metamory.Api
{
	public class AssemblyModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
			   .Where(t => t.Name.EndsWith("Service"));

			builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
			   .Where(t => t.Name.EndsWith("Repository"))
			   .AsImplementedInterfaces();
		}
	}
}
