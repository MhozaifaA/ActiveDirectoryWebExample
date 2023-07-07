using MicroServices.ActiveDirectory.Models;
using MicroServices.ActiveDirectory.Options;
using Meteors;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MicroServices.ActiveDirectory
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly ActiveDirectoryOptions options;
        public List<LDAPUser> _users { get; private set; }
        /// <summary>
        /// key: identityvalue,
        /// </summary>
        public Dictionary<string, UserPrincipal> _userprincipals { get; private set; } = new();
        public ActiveDirectoryService(IOptions<ActiveDirectoryOptions> options = null)
        {
            this.options = options?.Value ?? new();
        }

        public void AsyncUsers()
        {
            string searchQuery = $"(&(sAMAccountType=805306368)(postalCode=*)(displayName=*)(userPrincipalName=*))";
            DirectorySearcher ds = new DirectorySearcher(searchQuery, new string[] { "displayName",
                "userPrincipalName" ,   "sAMAccountName" ,"postalCode"  });
            var finds = ds.FindAll();
            List<LDAPUser> Users = new List<LDAPUser>();
            foreach (SearchResult user in finds)
            {
                var disname = user.Properties["displayName"];
                var prinname = user.Properties["userPrincipalName"];
                var saname = user.Properties["sAMAccountName"];
                var postalCode = user.Properties["postalCode"];
                //if (disname.Count == 0 || prinname.Count  == 0 || postalCode.Count == 0)
                //    continue;

                Users.Add(new LDAPUser()
                {
                    DisplayName = (string)disname[0],
                    UserPrincipalName = (string)prinname[0],
                    SAMAccountName = (string)saname[0],
                    PostalCode = (string)postalCode[0]
                });
            }
            Users.Sort((u1, u2) => String.Compare(u1.DisplayName, u2.DisplayName, true));

            if (options.IsStoreEnable)
                _users = Users;
        }
        public IEnumerable<LDAPUser> GetUsers()
        {
            if (options.IsStoreEnable && _users is not null)
                return _users;

            string searchQuery = $"(&(sAMAccountType=805306368)(postalCode=*)(displayName=*)(userPrincipalName=*))";
            DirectorySearcher ds = new DirectorySearcher(searchQuery, new string[] { "displayName",
                "userPrincipalName" ,   "sAMAccountName" ,"postalCode"  });
            var finds = ds.FindAll();
            List<LDAPUser> Users = new List<LDAPUser>();
            foreach (SearchResult user in finds)
            {
                var disname = user.Properties["displayName"];
                var prinname = user.Properties["userPrincipalName"];
                var saname = user.Properties["sAMAccountName"];
                var postalCode = user.Properties["postalCode"];
                //if (disname.Count == 0 || prinname.Count  == 0 || postalCode.Count == 0)
                //    continue;

                Users.Add(new LDAPUser()
                {
                    DisplayName = (string)disname[0],
                    UserPrincipalName = (string)prinname[0],
                    SAMAccountName = (string)saname[0],
                    PostalCode = (string)postalCode[0]
                });
            }
            Users.Sort((u1, u2) => String.Compare(u1.DisplayName, u2.DisplayName, true));

            if (options.IsStoreEnable)
                _users = Users;

            return (Users);
        }

        public LDAPUser? GetUser(string identity,bool load = false)
        {

            if (load && options.IsStoreEnable && _users is null)
                this.GetUsers();

            if (options.IsStoreEnable && _users is not null)
                return _users.Find(x => x.SAMAccountName == identity || x.UserPrincipalName == identity);


            string searchQuery = $"(&(sAMAccountType=805306368)(|(sAMAccountName={identity})(userPrincipalName={identity})))";
            DirectorySearcher ds = new DirectorySearcher(searchQuery, new string[] { "displayName",
                "userPrincipalName","sAMAccountName" ,"postalCode"  });
            var finds = ds.FindOne();

            if (finds is null)
                return null;
            var disname = finds.Properties["displayName"];
            var prinname = finds.Properties["userPrincipalName"];
            var saname = finds.Properties["sAMAccountName"];
            var postalCode = finds.Properties["postalCode"];

            return new LDAPUser()
            {
                DisplayName = (string)disname[0],
                UserPrincipalName = (string)prinname[0],
                SAMAccountName = (string)saname[0],
                PostalCode = (string)postalCode[0]
            };
        }

        public UserPrincipal? GetUserPrincipal(string identityvalue)
        {
            // _userprincipals ??= new ();
            if (options.IsStoreEnable && _userprincipals.TryGetValue(identityvalue, out UserPrincipal value))
                return value;


            var res = UserPrincipal.FindByIdentity
            (new PrincipalContext(ContextType.Domain),
            IdentityType.SamAccountName, identityvalue);

            if (options.IsStoreEnable)
                _userprincipals.Add(identityvalue, res);

            return res;
        }

        public object? GetUserJson(string identity)
        {
            
            string searchQuery = $"(|(sAMAccountName={identity})(userPrincipalName={identity}))";
            DirectorySearcher ds = new DirectorySearcher(searchQuery);
            var finds = ds.FindOne();

            if (finds is null)
                return null;

            Dictionary<string, string?> result = new ();
            foreach (var item in finds.Properties.PropertyNames)
            {
                var sad = finds.Properties[item.ToString()];
                List<object> data = new();
                foreach (var rr in sad)
                {
                    data.Add(rr);
                }
                result.TryAdd(item.ToString(), JsonSerializer.Serialize(data));
            }

            return result;
        }


        public IEnumerable<LDAPUser> LastLoginUsers()
        {
            string searchQuery = $"(&(sAMAccountType=805306368)(displayName=*)(userPrincipalName=*)(postalCode=*)" +
                $"(lastLogonTimestamp<={DateTime.Now.AddDays(-60).ToFileTime()}))";
            DirectorySearcher ds = new DirectorySearcher(searchQuery, new string[] { "displayName",
                "userPrincipalName" ,   "sAMAccountName" ,"postalCode"  });
            var finds = ds.FindAll();
            List<LDAPUser> Users = new List<LDAPUser>();
            foreach (SearchResult user in finds)
            {
                var disname = user.Properties["displayName"];
                var prinname = user.Properties["userPrincipalName"];
                var saname = user.Properties["sAMAccountName"];
                var postalCode = user.Properties["postalCode"];
                //if (disname.Count == 0 || prinname.Count  == 0 || postalCode.Count == 0)
                //    continue;

                Users.Add(new LDAPUser()
                {
                    DisplayName = (string)disname[0],
                    UserPrincipalName = (string)prinname[0],
                    SAMAccountName = (string)saname[0],
                    PostalCode = (string)postalCode[0]
                });
            }
            Users.Sort((u1, u2) => String.Compare(u1.DisplayName, u2.DisplayName, true));

            return Users;
        }

    }
}
