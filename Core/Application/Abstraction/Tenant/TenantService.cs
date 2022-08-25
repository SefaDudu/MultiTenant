using Core.Application.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Abstraction.Tenant
{
    public class TenantService : ITenantService
    {
        readonly TenantSettings _tenantSettings;
        readonly Tenants _tenant;
        readonly HttpContext _httpContext;
        public TenantService(IOptions<TenantSettings> tenantSettings, IHttpContextAccessor httpContextAccessor)
        {
            _tenantSettings = tenantSettings.Value;
            _httpContext = httpContextAccessor.HttpContext;
            if (_httpContext != null)
            {
                if (_httpContext.Request.Headers.TryGetValue("TenantId", out var tenantId))
                {
                    _tenant = _tenantSettings.Tenants.FirstOrDefault(t => t.TenantId == tenantId);
                    if (_tenant == null) throw new Exception("Invalid tenant!");

                    //Eğer bu kullanıcı grubu/müşteri/kiracı paylaşımlı veritabanını kullanıyorsa connection string'i boş gelecektir.
                    if (string.IsNullOrEmpty(_tenant.ConnectionString))
                        _tenant.ConnectionString = _tenantSettings.Defaults.ConnectionString;
                }
            }
        }
        public string GetConnectionString() => _tenant?.ConnectionString;
        public string GetDatabaseProvider() => _tenantSettings.Defaults?.DbProvider;
        public Tenants GetTenant() => _tenant;
    }
}
