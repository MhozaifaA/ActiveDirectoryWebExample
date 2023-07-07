using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServices.ActiveDirectory.Models
{
    public class LDAPUser
    {
        public string DisplayName { get; set; }
        ///// <summary>
        ///// arabic name
        ///// </summary>
        //public string ExtensionAttribute1 { get; set; }
        public string UserPrincipalName { get; set; }
        public string SAMAccountName { get; set; }
        public string PostalCode { get; set; }
    }
}
