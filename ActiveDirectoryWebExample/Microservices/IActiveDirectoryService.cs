using MicroServices.ActiveDirectory.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServices.ActiveDirectory
{
    public interface IActiveDirectoryService
    {
        void AsyncUsers();
        LDAPUser? GetUser(string UserPrincipalName,bool load = false);
        object? GetUserJson(string identity);
        UserPrincipal? GetUserPrincipal(string identityvalue);
        IEnumerable<LDAPUser> GetUsers();
        IEnumerable<LDAPUser> LastLoginUsers();
    }
}
