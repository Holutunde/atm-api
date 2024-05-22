using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMAPI.Dto
{
    public class LoginDto
    {
        public long AccountNumber { get; set; }
        public int Pin { get; set; }

    }
}