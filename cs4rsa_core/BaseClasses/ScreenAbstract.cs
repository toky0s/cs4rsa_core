using System.Collections.Generic;

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
            _components = new List<IComponent>();
        }

        /// <summary>
        /// Load thông tin khởi tạo của từng Component
        /// được đăng ký cho Màn hình này.
        /// </summary>
        /// <returns>Task</returns>
        public void LoadComponentsData()
        {
            _components.ForEach(ca => ca.LoadData());
        }

        /// <summary>
        /// Đăng ký một Component cho Màn hình này.
        /// Thực hiện gọi method này trong constructor của
        /// Component cha.
        /// </summary>
        /// <param name="component">Component con.</param>
        protected void RegisterComponent(IComponent component)
        {
            _components.Add(component);
        }
    }
}
