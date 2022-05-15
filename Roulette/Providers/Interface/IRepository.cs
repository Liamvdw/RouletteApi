using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.Providers.Interface
{
    public interface IRepository
    {       
        void ExecuteWrite(string query, Dictionary<string, object> args);
        DataTable Execute(string query, Dictionary<string, object> args);
    }
}
