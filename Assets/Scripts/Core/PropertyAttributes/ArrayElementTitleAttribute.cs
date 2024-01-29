using UnityEngine;

namespace Core.PropertyAttributes
{
    public class ArrayElementTitleAttribute : PropertyAttribute
    {
        public string baseHeader = "";

        /// <summary>
        /// массив перменных, по которым можно озаглавить
        /// </summary>
        public string[] Varnames = new string[0];

        /// <summary>
        /// Режим выбора имени: true - выведется первое имя, не содержащее пустую строку. false - выведутся все имена, не сожержащие поустую строку
        /// </summary>
        public bool switchmode;
        public bool lockEnumNull;

        public ArrayElementTitleAttribute()
        {}
    
        public ArrayElementTitleAttribute(string ElementTitleVar)
        {
            Varnames = new[] { ElementTitleVar };
            switchmode = false;
            lockEnumNull = false;
        }
    
        public ArrayElementTitleAttribute(string[] ElementTitleVars)
        {
            Varnames = ElementTitleVars;
        }
    }
}

