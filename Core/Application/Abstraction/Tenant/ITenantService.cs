using Core.Application.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Abstraction.Tenant
{
    public interface ITenantService
    {
        string GetDatabaseProvider();
        string GetConnectionString();
        Tenants GetTenant();
    }
}
