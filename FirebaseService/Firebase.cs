using FirebaseService.Interfaces;
using FirebaseService.Requests;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FirebaseService
{
    public class Firebase : IFirebase
    {

        public static IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "pnI5glEAPxwLOWB2a2Ivsad2HrFweywacdjP4uc9",
            BasePath = "https://cs4rsafast-default-rtdb.firebaseio.com/"
        };

        public static IFirebaseClient client;

        public Firebase()
        {
        }

        public string GetLatestVersion()
        {
            client = new FireSharp.FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"/appInfomations/Version");
            string version = res.Body.ToString();
            char[] chars = { '\"' };
            string[] result = version.Split(chars, StringSplitOptions.RemoveEmptyEntries);
            return result[0];
        }

        public async Task<bool> PostUser(string studentId, string specialString)
        {
            client = new FireSharp.FirebaseClient(ifc);
            Dictionary<string, string> data = new() { { studentId, specialString } };
            await client.SetTaskAsync(@"/users", data);
            return true;
        }
    }
}
