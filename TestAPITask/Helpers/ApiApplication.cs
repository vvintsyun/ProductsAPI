using Microsoft.AspNetCore.Mvc.Testing;

namespace TestAPITask.Helpers
{
    class ApiApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            return base.CreateHost(builder);
        }
    }
}
