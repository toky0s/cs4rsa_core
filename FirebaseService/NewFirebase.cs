using Firebase.Database;
using Firebase.Database.Query;

using FirebaseService.Interfaces;
using FirebaseService.Requests;

using System.Linq;
using System.Threading.Tasks;

namespace FirebaseService
{
    public class NewFirebase : IFirebase
    {
        private readonly FirebaseClient _firebaseClient;
        public NewFirebase()
        {
            string auth = "pnI5glEAPxwLOWB2a2Ivsad2HrFweywacdjP4uc9";
            _firebaseClient = new(
                  "https://cs4rsafast-default-rtdb.firebaseio.com/",
                  new FirebaseOptions
                  {
                      AuthTokenAsyncFactory = () => Task.FromResult(auth)
                  }
            );
        }

        public async Task<string> GetLatestVersion()
        {
            var appInfomations = await _firebaseClient.Child("appInfomations").OnceAsync<object>();
            return (string)appInfomations.ElementAtOrDefault(0).Object;
        }

        public async Task<bool> PostUser(string studentId, string specialString)
        {
            User user = new()
            {
                Id = studentId,
                SpecialString = specialString
            };
            FirebaseObject<User> response = await _firebaseClient
                      .Child("users")
                      .PostAsync(user);
            return response.Object != null;
        }
    }
}
