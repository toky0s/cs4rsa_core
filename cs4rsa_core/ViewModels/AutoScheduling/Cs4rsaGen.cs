using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.ViewModels.AutoScheduling
{
    /// <summary>
    /// Ứng dụng quay lui thực hiện sinh các cấu hình
    /// </summary>
    public class Cs4rsaGen
    {
        private readonly List<int> _currentIndexes = new();
        private readonly List<IEnumerable<ClassGroupModel>> _classGroupModelsOfClass;
        private const int Placeholder = -1;
        public readonly List<List<int>> TempResult = new();
        public Cs4rsaGen(List<IEnumerable<ClassGroupModel>> classGroupModelsOfClass)
        {
            _classGroupModelsOfClass = classGroupModelsOfClass;
            _classGroupModelsOfClass.ForEach(item => _currentIndexes.Add(Placeholder));
        }

        public void Backtracking(int k)
        {
            int count = _classGroupModelsOfClass[k].Count();
            for (int i = 0; i < count; i++)
            {
                int stringClone = Clone(i);
                _currentIndexes[k] = stringClone;
                if (IsSuccess(_currentIndexes, _classGroupModelsOfClass.Count))
                {
                    List<int> clone = Clone(_currentIndexes);
                    TempResult.Add(clone);
                }
                else
                {
                    Backtracking(k + 1);
                    _currentIndexes[k + 1] = -1;
                }
            }
        }

        private static T Clone<T>(T source)
        {

            if (source is null)
            {
                return default;
            }

            var deserializeSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);

        }

        private static bool IsSuccess(IReadOnlyCollection<int> result, int amount)
        {
            if (result.Any(item => item == -1))
            {
                return false;
            }

            return result.Count == amount;
        }
    }
}
