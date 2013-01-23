using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddressMatch
{
    public class MatchHelper
    {
        public static void PrintResult(MatchResult result)
        {
            if (result == null)
            {
                Console.WriteLine("未初始化");
                return;
            }
            Console.WriteLine("==DEBUG==Print MatchResult=======");
            Console.WriteLine("==result's state is  " + TranslateState(result.ResultState));
            Console.WriteLine("==matched node's name is  " + result.Result.Name);
            Console.WriteLine("==matched node's id is  " + result.Result.ID);
            Console.WriteLine("==matched node's LEVEL is  " + result.Result.NodeLEVEL);

        }

        private static string TranslateState(MatchResultState state)
        {
            if (state == MatchResultState.SUCCESS)
            {
                return "SUCCESS";
            }
            if (state == MatchResultState.MULTIMATCHED)
            {
                return "MULTIMATCHED";
            }
            if (state == MatchResultState.NOTFOUND)
            {
                return "NOTFOUND";
            }
            if (state == MatchResultState.NOTMATCHED)
            {
                return "NOTMATCHED";
            }
            if (state == MatchResultState.UNKNOWNFAILED)
            {
                return "UNKNOWNFAILED";
            }
            return "ERROR";
        }

        private static string TranslateLEVEL(LEVEL level)
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


        public static void Assert(bool flag, string message)
        {

        #if DEBUG
            if (flag)
            {
                Console.WriteLine("==DEBUG==  " + message);
            }
        #endif


        }
    }
}
