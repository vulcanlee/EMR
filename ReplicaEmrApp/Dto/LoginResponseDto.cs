using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Dto
{
    public class LoginResponseDto
    {
            public string returnCode { get; set; }
            public List<object> mights { get; set; }
            public string returnMessage { get; set; }
            public string userid { get; set; }
            public string username { get; set; }
    }
}
