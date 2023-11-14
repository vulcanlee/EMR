using ReplicaEmrApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Services
{
    public class JsonPersistanceService<T> where T : class
    {
        public StorageJSONService<T> StorageJSONService { get; set; }
    }
}
