using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Credentials
{
    public static class RequestHeader
    {
        public static string ForwardedIp = "X-FORWARDED-FOR";
        public static string RefreshToken = "X-REFRESH-TOKEN";
    }
}