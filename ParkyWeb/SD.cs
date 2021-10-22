using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkyWeb
{
    public static class SD
    {
        public static string APIBaseUrl = "https://localhost:44361/";
        public static string NationalParkAPIPath = APIBaseUrl + "api/v1/nationalparks/";
        public static string TrailsAPIPath = APIBaseUrl + "api/v1/trails/";
        public static string UserAPIPath = APIBaseUrl + "api/v1/users/";
    }
   
}
