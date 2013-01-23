using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddressMatch;

namespace AddressMatch.Training
{
    public class TrainException : Exception
    {
        public TrainException(string message)
            : base(message)
        {
            
        }

        private InsertElement _el;

        public TrainException(InsertElement el, string message)
            : base(message)
        {
            _el = el;
        }

        public void PrintDetail()
        {
            Console.WriteLine("==DEBUG==Print TrainException=======");
            Console.WriteLine("==InsertElement's name is  " + _el.Name);
            Console.WriteLine("==InsertElement's LEVEL is  " + TranslateLEVEL(_el.Level));
            Console.WriteLine("==InsertElement's MODE is  " + TranslateMode(_el.Mode));
        }


        private string TranslateLEVEL(LEVEL level)
        {
            if (level == LEVEL.Building)
            {
                return "Building";
            }
            if (level == LEVEL.City)
            {
                return "City";
            }
            if (level == LEVEL.Contry)
            {
                return "Contry";
            }
            if (level == LEVEL.Other)
            {
                return "Other";
            }
            if (level == LEVEL.Province)
            {
                return "Province";
            }
            if (level == LEVEL.Root)
            {
                return "Root";
            }
            if (level == LEVEL.Street)
            {
                return "Street";
            }
            if (level == LEVEL.Uncertainty)
            {
                return "Uncertainty";
            }
            if (level == LEVEL.Zone)
            {
                return "Zone";
            }
            return "ERROR";
        }

        private string TranslateMode(UInt16 mode)
        {
            InsertMode levelmode = (InsertMode)mode & (InsertMode)TrainMachine.LevelMask;

            InsertMode placemode = (InsertMode)mode & (InsertMode)TrainMachine.PlaceMask;

            string level_message = "", place_message = "";

            if (levelmode == InsertMode.AutoLevel)
            {
                level_message = "AutoLevel";
            }
            if (levelmode == InsertMode.DegradeLevel)
            {
                level_message = "DegradeLevel";
            }
            if (levelmode == InsertMode.UpgradeLevel)
            {
                level_message = "UpgradeLevel";
            }
            if (levelmode == InsertMode.ExactlyLevel)
            {
                level_message = "ExactlyLevel";
            }

            if (placemode == InsertMode.NewPlace)
            {
                place_message = "NewPlace";
            }
            if (placemode == InsertMode.OldPlace)
            {
                place_message = "OldPlace";
            }
            if (placemode == InsertMode.AutoPlace)
            {
                place_message = "AutoPlace";
            }

            return level_message + " AND " + place_message;

        }

    }
}
