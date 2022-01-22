using cs4rsa_core.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace cs4rsa_core.Utils
{
    /// <summary>
    /// Ứng dụng quay lui thực hiện sinh các cấu hình
    /// </summary>
    public class Cs4rsaGen
    {
        private readonly List<int> _currentIndexes = new();
        private readonly List<List<ClassGroupModel>> _classGroupModelsOfClass;
        private readonly int PLACEHOLDER = -1;
        public readonly List<List<int>> TempResult = new();
        public Cs4rsaGen(List<List<ClassGroupModel>> classGroupModelsOfClass)
        {
            _classGroupModelsOfClass = classGroupModelsOfClass;
            _classGroupModelsOfClass.ForEach(item => _currentIndexes.Add(PLACEHOLDER));
        }

        public void Backtracking(int k)
        {
            for (int i = 0; i < _classGroupModelsOfClass[k].Count; i++)
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
            string serialized = JsonSerializer.Serialize(source);
            return JsonSerializer.Deserialize<T>(serialized);
        }

        private static bool IsSuccess(List<int> result, int amount)
        {
            foreach (int item in result)
            {
                if (item == -1)
                {
                    return false;
                }
            }
            return result.Count == amount;
        }
    }
}
