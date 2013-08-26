namespace Web
{
    using System.Web;
    using System.Web.Http;
    using System.Reflection;

    using Autofac;
    using Autofac.Integration.WebApi;

    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/fwlink/?LinkId=301868
    public class WebApiApplication : HttpApplication
    {
        private static IContainer container;

        protected void Application_Start()
        {
            var assembly = Assembly.GetExecutingAssembly();

            WebApiConfig.Register(GlobalConfiguration.Configuration);


            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(assembly);
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            builder.RegisterAssemblyModules(assembly);
            
            container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver =
                new AutofacWebApiDependencyResolver(container);
        }
    }
}
