using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Helpers
{
    public class ApplicationInfo
    {
        public string Version { get; set; }
    }

    public static class Cs4rsaVersion
    {
        public static IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "pnI5glEAPxwLOWB2a2Ivsad2HrFweywacdjP4uc9",
            BasePath = "https://cs4rsafast-default-rtdb.firebaseio.com/"
        };

        public static IFirebaseClient client;

        public static string CheckVer()
        {
            client = new FireSharp.FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"/appInfomations");
            var version = res.ResultAs<ApplicationInfo>();
            return version.Version;
        }
    }
}
