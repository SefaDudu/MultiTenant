using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Settings
{
    public class Tenants
    {
        public string Name { get; set; }
        public string TenantId { get; set; }
        public string ConnectionString { get; set; }
    }
}
