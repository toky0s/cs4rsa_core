using System.Collections.Generic;
using System.Linq;

namespace cs4rsa.Helpers
{
    /// <summary>
    /// Sinh tổ hợp chính tắc từ tập class Group
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Gen<T>
    {
        /// <summary>
        /// Gen cấu hình đầu tiên.
        /// </summary>
        /// <param name="k">Số lượng phần tử muốn có trong tập hợp (nhỏ hơn hoặc bằng số lượng phần tử được cấp).</param>
        /// <param name="elements">Tập hợp các phần tử</param>
        /// <returns></returns>
        public static List<T> GenFirst(int k, List<T> elements)
        {
            List<T> firstCombination = new List<T>();
            for (int i = 0; i < k; i++)
            {
                firstCombination.Add(elements[i]);
            }
            return firstCombination;
        }

        #region Generation Algorithm
        /// <summary>
        /// Sinh tổ hợp chính tắc từ tập class Group
        /// </summary>
        /// <param name="k">Số lượng phần tử trong mỗi tổ hợp.</param>
        /// <param name="elements">Danh sách các phần tử duy nhất.</param>
        /// <returns></returns>
        public static List<List<T>> StartGen(int k, List<T> elements)
        {
            List<List<T>> result = new List<List<T>>();
            List<T> firstCombination = new List<T>();
            for (int i = 0; i < k; i++)
            {
                firstCombination.Add(elements[i]);
            }
            while (true)
            {
                result.Add(firstCombination.ToList());
                if (IsLastCombination(firstCombination, k, elements))
                {
                    return result;
                }
                firstCombination = GenNext(firstCombination, k, elements);
            }
        }

        /// <summary>
        /// Kiểm tra tổ hợp có phải là tổ hợp cuối cùng trong tập hợp hay không.
        /// </summary>
        /// <param name="combination">Tổ hợp phần tử.</param>
        /// <param name="k">Số lượng phần tử mỗi tổ hợp (chập).</param>
        /// <param name="elements">Danh sách phần tử.</param>
        /// <returns></returns>
        public static bool IsLastCombination(List<T> combination, int k, List<T> elements)
        {
            List<T> lastCombination = elements.GetRange(elements.Count - k, k);
            if (combination.Count != lastCombination.Count)
                return false;
            int count = 0;
            for (int i = 0; i < k; i++)
            {
                if (combination[i].Equals(lastCombination[i]))
                    count++;
            }
            if (count == lastCombination.Count)
                return true;
            return false;
        }

        /// <summary>
        /// Kiểm tra một cấu hình có phải là cấu hình đầu tiên hay không.
        /// </summary>
        /// <param name="combination"></param>
        /// <param name="k"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static bool IsFirstCombination(List<T> combination, int k, List<T> elements)
        {
            List<T> firstCombination = elements.GetRange(0, k);
            if (combination.Count != firstCombination.Count)
                return false;
            int count = 0;
            for (int i = 0; i < k; i++)
            {
                if (combination[i].Equals(firstCombination[i]))
                    count++;
            }
            if (count == firstCombination.Count)
                return true;
            return false;
        }

        /// <summary>
        /// Sinh tổ hợp kế tiếp.
        /// </summary>
        /// <param name="currentCombination">Tổ hợp k phẩn tử.</param>
        /// <param name="k">Số lượng phần tử mỗi tổ hợp.</param>
        /// <param name="elements">Danh sách các phần tử.</param>
        /// <returns></returns>
        public static List<T> GenNext(List<T> currentCombination, int k, List<T> elements)
        {
            int i = k - 1;
            while (i > 0 && currentCombination[i].Equals(elements[elements.Count + i - k]))
                i -= 1;
            T value = currentCombination[i];
            int index = elements.IndexOf(value);
            currentCombination[i] = elements[index + 1];
            if (i >= 0)
            {
                for (int j = i; j < elements.Count; ++j)
                {
                    if (j >= (currentCombination.Count - 1))
                        break;
                    T valueTemp = currentCombination[j];
                    int indexTemp = elements.IndexOf(valueTemp);
                    currentCombination[j + 1] = elements[indexTemp + 1];
                }

            }
            return currentCombination;
        }
        #endregion
    }
}
