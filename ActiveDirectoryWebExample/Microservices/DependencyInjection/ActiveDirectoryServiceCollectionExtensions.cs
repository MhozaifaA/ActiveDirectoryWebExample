using MicroServices.ActiveDirectory;
using MicroServices.ActiveDirectory.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ActiveDirectoryServiceCollectionExtensions
    {
        public static IServiceCollection AddActiveDirectory(this IServiceCollection services,
            Action<ActiveDirectoryOptions> options = null)
        {
            if (options is not null)
                services.Configure(options);

            return services.AddSingleton<IActiveDirectoryService, ActiveDirectoryService>();
        }

    }
}
