using System;
using MenuSystem;
using System.ComponentModel;

namespace MenuSystem
{
    public class MenuItemDefault : IMenuItem
    {
        #region Fields and Properties                     
        protected const int MIN_LEN = 1;                              // Minimum length of Label and Option string
        protected const int MAX_LEN_LABEL = 60;                       // Maximum length for Label string
        protected const string DEF_LABEL = "Not Assigned Option";     // Default Label of Menu Item
        protected const int MAX_LEN_VAL = 20;                         // Maximum length of value in Menu Item
        protected const int FIELD_DIST = 5;

        public string Label
        {
            get;
            set;
        }
        public MenuItemType Type
        {
            get;
            protected set;
        }
        public Func<MenuItemType> MethodToExecute
        {
            get;
            protected set;
        }
        #endregion Fields and Properties

        #region Constructor
        /// <summary>
        /// Constructor of Menu Item
        /// </summary>
        /// <param name="aLabel">
        /// Text Label of Menu Item
        /// </param>
        /// <param name="aMethodToExecute">
        /// Method executed when Menu Item is selected
        /// </param>
        public MenuItemDefault(MenuItemType aType = MenuItemType.Execute, string aLabel = DEF_LABEL, Func<MenuItemType>? aMethodToExecute = null)
        {
            if (aLabel.Length < MIN_LEN || aLabel.Length > MAX_LEN_LABEL)
                throw new Exception(message: $"Label \"{aLabel}\" size not in range [{MIN_LEN};{MAX_LEN_LABEL}]");
            Label = aLabel;
            Type = aType;
            MethodToExecute = aMethodToExecute ?? DefaultMethod;
        }
        #endregion

        #region Methods
        public virtual MenuItemType DefaultMethod()
        {
            return Type;
        }

        public override string ToString()
        {
            return " " + Label;
        }
        #endregion Methods
    }
}
