using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressMatch.Training
{
    public class InsertElement
    {
        public string Name;
        
        public LEVEL Level;

        public UInt16 Mode;

        private InsertElement() { }

        public InsertElement(string name)
        {
            Name = name;
            Level = LEVEL.Default;
            Mode = (UInt16)(InsertMode.AutoLevel | InsertMode.AutoPlace);
        }

        public InsertElement(string name,LEVEL level)
        {
            Name = name;
            Level = level;
            Mode = (UInt16)(InsertMode.AutoLevel | InsertMode.AutoPlace);
        }
        /// <summary>
        /// in the case that place_mode is OldPlace, level_mode must be ExactlyLevel. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        /// <param name="mode"></param>
        public InsertElement(string name, LEVEL level, InsertMode mode)
        {
            Name = name;
            Level = level;
            Mode = (UInt16)mode;
        }
        
    }

    
    public enum InsertMode
    {
        //标志位的前8位
        ExactlyLevel            = 0x1,
        DegradeLevel         = 0x2,
        UpgradeLevel         = 0x4,
        AutoLevel               = 0x8,
        //标志位的9 - 16位
        NewPlace            = 0x100,
        OldPlace             = 0x200,
        AutoPlace            = 0x400,
    }

}
