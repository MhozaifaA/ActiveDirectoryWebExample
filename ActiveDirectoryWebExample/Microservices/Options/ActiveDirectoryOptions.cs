using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServices.ActiveDirectory.Options
{
    public class ActiveDirectoryOptions
    {
        internal bool IsStoreEnable { get; set; }

        public void EnableStore() => IsStoreEnable = true;
    }
}
