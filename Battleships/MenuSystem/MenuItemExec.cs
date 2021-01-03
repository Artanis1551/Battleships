using System;
using System.Collections.Generic;
using System.Text;

namespace MenuSystem
{
    public class MenuItemExec<T> : MenuItemDefault
    {
        public MenuItemExec(MenuItemType aType = MenuItemType.Execute, string aLabel = DEF_LABEL, Func<MenuItemType>? aMethodToExecute = null)
        {
            if (aLabel.Length < MIN_LEN || aLabel.Length > MAX_LEN_LABEL)
                throw new Exception(message: $"Label \"{aLabel}\" size not in range [{MIN_LEN};{MAX_LEN_LABEL}]");
            Label = aLabel;
            Type = aType;
            MethodToExecute = aMethodToExecute ?? DefaultMethod;
        }
    }
}
