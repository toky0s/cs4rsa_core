using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.BaseClasses
{
    /// <summary>
    /// Các màn hình của Crediz phải kế thừa từ lớp này.
    /// </summary>
    public abstract class ScreenAbstract : BaseUserControl
    {
        private readonly List<IComponent> _components;

        protected ScreenAbstract()
        {
            _components = new();
        }

        /// <summary>
        /// Load thông tin khởi tạo của từng Component
        /// được đăng ký cho Màn hình này.
        /// </summary>
        /// <returns>Task</returns>
        public async Task LoadComponentsData()
        {
            List<Task> tasks = _components.Select(ca => ca.LoadData()).ToList();
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Đăng ký một Component cho Màn hình này.
        /// Thực hiện đăng ký các component trong
        /// phần xử lý sự kiện Loaded khi màn hình đã
        /// tải thành công.
        /// </summary>
        /// <param name="component">IComponent</param>
        protected void RegisterComponent(IComponent component)
        {
            _components.Add(component);
        }
    }
}
