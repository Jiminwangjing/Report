using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class ConnetionJSon
    {
        public UsersConnection UsersConnection { get; set; }
    }
    public class UsersConnection
    {
        public string ConnectionString { get; set; }
    }
   
}
