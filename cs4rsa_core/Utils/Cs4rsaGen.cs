﻿using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using LightMessageBus;
using System.Collections.Generic;
using System.ComponentModel;
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
        public Cs4rsaGen(List<List<ClassGroupModel>> classGroupModelsOfClass)
        {
            _classGroupModelsOfClass = classGroupModelsOfClass;
            _classGroupModelsOfClass.ForEach(item => _currentIndexes.Add(PLACEHOLDER));
        }

        public void Backtracking(int k, BackgroundWorker backgroundWorker = null)
        {
            for (int i = 0; i < _classGroupModelsOfClass[k].Count; i++)
            {
                int stringClone = Clone(i);
                _currentIndexes[k] = stringClone;
                if (IsSuccess(_currentIndexes, _classGroupModelsOfClass.Count))
                {
                    List<int> clone = Clone(_currentIndexes);
                    if (backgroundWorker != null)
                    {
                        backgroundWorker.ReportProgress(0, clone);
                    }
                    //MessageBus.Default.Publish(new AddCombinationMessage(clone));
                }
                else
                {
                    Backtracking(k + 1, backgroundWorker);
                    _currentIndexes[k + 1] = -1;
                    continue;
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
