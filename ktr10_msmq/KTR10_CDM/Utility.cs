using System;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace KTR10_CDM
{
    public class Utility
    {
        public const string TraceDebugStr = @"""{0}"" - Write Debug Problem - ex detail :  {1}";

        #region Debug && Log

        public static void DebugMe(string _log, string level)
        {
            DebugMeSub(_log, level);
        }

        public static void DebugMe(string _log)
        {
            DebugMeSub(_log, "Debug");
        }

        private static void DebugMeSub(string _log, string level)
        {
            String path = "C:\\WEBATMLOGS\\IscepMatik\\" + "KTR10_" + DateTime.Now.ToString("yyMMdd") + ".log";

            DateTime now = DateTime.Now;
            String header = "";
            header += now.ToString("yyMMdd-HHmmss.fff");
            header += " ";
            header += level;
            header += " ";

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(header + _log);
                writer.Close();
            }
        }

        #endregion

        #region O B E B  | |  Greatest Common Divisor
        public static int GCD(int[] numbers)
        {
            return numbers.Aggregate(GCD);
        }

        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        #endregion

        private static string iniFile = "KTR_10.Settings.ini";

        private static string strSectionCassette = "Cassettes";
        private static string strSCCounterLoaded = "Cassette{0}CounterLoaded";
        private static string strSCCounterDispensed = "Cassette{0}CounterDispensed";
        private static string strSCCounterRejected = "Cassette{0}CounterRejected";
        private static string strSCCounterTotalDispensed = "Cassette{0}CounterTotalDispensed";
        private static string strSCCounterLastDispensed = "Cassette{0}CounterLastDispensed";
        private static string strSCCounterLastDispensedTemp = "Cassette{0}CounterLastDispensedTemp";

        private static string strSectionRejectVault = "RejectVault";
        private static string strSRVCounterRejected = "RejectVaultCounterRejected";
        private static string strSRVCounterRetracted = "RejectVaultCounterRetracted";

        private static string strSectionApplicationSettings = "ApplicationSettings";
        private static string strLogFilePath = "LogFilePath";

        public static void InitiateIniFile(int CassetteCount)
        {
            var MyIni = new KTR10_CDM.Ini.IniFile(iniFile);

            int cassetteNo = 0;
            string _key = "";
            for (int i = 0; i < CassetteCount; i++)
            {
                cassetteNo = i + 1;
                _key = String.Format(strSCCounterLoaded, cassetteNo);

                if (!MyIni.KeyExists(_key, strSectionCassette))
                {
                    MyIni.Write(_key, "0", strSectionCassette);
                }
                _key = String.Format(strSCCounterDispensed, cassetteNo);
                if (!MyIni.KeyExists(_key, strSectionCassette))
                {
                    MyIni.Write(_key, "0", strSectionCassette);
                }
                _key = String.Format(strSCCounterRejected, cassetteNo);
                if (!MyIni.KeyExists(_key, strSectionCassette))
                {
                    MyIni.Write(_key, "0", strSectionCassette);
                }
                _key = String.Format(strSCCounterTotalDispensed, cassetteNo);
                if (!MyIni.KeyExists(_key, strSectionCassette))
                {
                    MyIni.Write(_key, "0", strSectionCassette);
                }
                _key = String.Format(strSCCounterLastDispensed, cassetteNo);
                if (!MyIni.KeyExists(_key, strSectionCassette))
                {
                    MyIni.Write(_key, "0", strSectionCassette);
                }
                _key = String.Format(strSCCounterLastDispensedTemp, cassetteNo);
                if (!MyIni.KeyExists(_key, strSectionCassette))
                {
                    MyIni.Write(_key, "0", strSectionCassette);
                }
            }

            if (!MyIni.KeyExists(strSRVCounterRejected, strSectionRejectVault))
            {
                MyIni.Write(strSRVCounterRejected, "0", strSectionRejectVault);
            }
            if (!MyIni.KeyExists(strSRVCounterRetracted, strSectionRejectVault))
            {
                MyIni.Write(strSRVCounterRetracted, "0", strSectionRejectVault);
            }

            if (!MyIni.KeyExists(strLogFilePath, strSectionApplicationSettings))
            {
                MyIni.Write(strLogFilePath, "", strSectionApplicationSettings);
            }
        }

        public static void GetCassetteCountersIniFile(int cassetteNo, ref int counterLoaded, ref int counterDispensed, ref int counterRejected, ref int counterTotalDispensed, ref int counterLastDispensed, ref int counterLastDispensedTemp)
        {
            var MyIni = new KTR10_CDM.Ini.IniFile(iniFile);

            string _key = "", tempValue = "";

            #region Loaded Counter
            _key = String.Format(strSCCounterLoaded, cassetteNo);
            if (MyIni.KeyExists(_key, strSectionCassette))
            {
                tempValue = MyIni.Read(_key, strSectionCassette);
                if (!Int32.TryParse(tempValue, out counterLoaded))
                {
                    counterLoaded = 0;
                    MyIni.Write(_key, "0", strSectionCassette);
                }
            }
            else
            {
                counterLoaded = 0;
                MyIni.Write(_key, "0", strSectionCassette);
            }
            #endregion

            #region Dispensed Counter
            _key = String.Format(strSCCounterDispensed, cassetteNo);
            if (MyIni.KeyExists(_key, strSectionCassette))
            {
                tempValue = MyIni.Read(_key, strSectionCassette);
                if (!Int32.TryParse(tempValue, out counterDispensed))
                {
                    counterDispensed = 0;
                    MyIni.Write(_key, "0", strSectionCassette);
                }
            }
            else
            {
                counterDispensed = 0;
                MyIni.Write(_key, "0", strSectionCassette);
            }

            #endregion

            #region Rejected Counter
            _key = String.Format(strSCCounterRejected, cassetteNo);
            if (MyIni.KeyExists(_key, strSectionCassette))
            {
                tempValue = MyIni.Read(_key, strSectionCassette);
                if (!Int32.TryParse(tempValue, out counterRejected))
                {
                    counterRejected = 0;
                    MyIni.Write(_key, "0", strSectionCassette);
                }
            }
            else
            {
                counterRejected = 0;
                MyIni.Write(_key, "0", strSectionCassette);
            }

            #endregion

            #region Total Dispensed Counter
            _key = String.Format(strSCCounterTotalDispensed, cassetteNo);
            if (MyIni.KeyExists(_key, strSectionCassette))
            {
                tempValue = MyIni.Read(_key, strSectionCassette);
                if (!Int32.TryParse(tempValue, out counterTotalDispensed))
                {
                    counterTotalDispensed = 0;
                    MyIni.Write(_key, "0", strSectionCassette);
                }
            }
            else
            {
                counterTotalDispensed = 0;
                MyIni.Write(_key, "0", strSectionCassette);
            }

            #endregion

            #region Last Dispensed Counter
            _key = String.Format(strSCCounterLastDispensed, cassetteNo);
            if (MyIni.KeyExists(_key, strSectionCassette))
            {
                tempValue = MyIni.Read(_key, strSectionCassette);
                if (!Int32.TryParse(tempValue, out counterLastDispensed))
                {
                    counterLastDispensed = 0;
                    MyIni.Write(_key, "0", strSectionCassette);
                }
            }
            else
            {
                counterLastDispensed = 0;
                MyIni.Write(_key, "0", strSectionCassette);
            }

            #endregion

            #region Last Dispensed Counter Temp
            _key = String.Format(strSCCounterLastDispensedTemp, cassetteNo);
            if (MyIni.KeyExists(_key, strSectionCassette))
            {
                tempValue = MyIni.Read(_key, strSectionCassette);
                if (!Int32.TryParse(tempValue, out counterLastDispensedTemp))
                {
                    counterLastDispensedTemp = 0;
                    MyIni.Write(_key, "0", strSectionCassette);
                }
            }
            else
            {
                counterLastDispensedTemp = 0;
                MyIni.Write(_key, "0", strSectionCassette);
            }

            #endregion
        }

        /// <summary>
        /// just set the counters, they are calculating before coming to that step
        /// </summary>
        /// <param name="cassetteNo"></param>
        /// <param name="counterLoaded"></param>
        /// <param name="counterDispensed"></param>
        /// <param name="counterRejected"></param>
        /// <param name="addCurrentValue"></param>
        /// <param name="counterTotalDispensed">Toplamda müşteriye verilen adet</param>
        /// <param name="counterLastDispensed">Son para çekme işleminde müşteriye verilen adet</param>
        /// <param name="counterLastDispensedTemp">Son dispense edilen adet</param>
        public static void SetCassetteCountersIniFile(int cassetteNo, int? counterLoaded, int? counterDispensed, int? counterRejected, bool addCurrentValue, int? counterTotalDispensed, int? counterLastDispensed, int? counterLastDispensedTemp)
        {
            var MyIni = new KTR10_CDM.Ini.IniFile(iniFile);

            string _key = "", tempValue = "";
            int counter = 0;

            #region Loaded Counter
            if (counterLoaded.HasValue)
            {
                _key = String.Format(strSCCounterLoaded, cassetteNo);
                if (addCurrentValue)
                {
                    tempValue = MyIni.Read(_key, strSectionCassette);
                    counter = Convert.ToInt32(tempValue) + Convert.ToInt32(counterLoaded);
                    MyIni.Write(_key, counter.ToString(), strSectionCassette);
                }
                else
                {
                    MyIni.Write(_key, counterLoaded.ToString(), strSectionCassette);
                }
            }
            #endregion

            #region Dispensed Counter
            if (counterDispensed.HasValue)
            {
                _key = String.Format(strSCCounterDispensed, cassetteNo);
                if (addCurrentValue)
                {
                    tempValue = MyIni.Read(_key, strSectionCassette);
                    counter = Convert.ToInt32(tempValue) + Convert.ToInt32(counterDispensed);
                    MyIni.Write(_key, counter.ToString(), strSectionCassette);
                }
                else
                {
                    MyIni.Write(_key, counterDispensed.ToString(), strSectionCassette);
                }
            }
            #endregion

            #region Rejected Counter
            if (counterRejected.HasValue)
            {
                _key = String.Format(strSCCounterRejected, cassetteNo);
                if (addCurrentValue)
                {
                    tempValue = MyIni.Read(_key, strSectionCassette);
                    counter = Convert.ToInt32(tempValue) + Convert.ToInt32(counterRejected);
                    MyIni.Write(_key, counter.ToString(), strSectionCassette);
                }
                else
                {
                    MyIni.Write(_key, counterRejected.ToString(), strSectionCassette);
                }
            }
            #endregion

            #region Total Dispensed Counter || addCurrentValue logic is not working there
            if (counterTotalDispensed.HasValue)
            {
                _key = String.Format(strSCCounterTotalDispensed, cassetteNo);

                tempValue = MyIni.Read(_key, strSectionCassette);
                counter = Convert.ToInt32(tempValue) + Convert.ToInt32(counterTotalDispensed);
                MyIni.Write(_key, counter.ToString(), strSectionCassette);
            }
            #endregion

            #region Last Dispensed Counter || addCurrentValue logic is not working there
            if (counterLastDispensed.HasValue)
            {
                _key = String.Format(strSCCounterLastDispensed, cassetteNo);

                MyIni.Write(_key, counterLastDispensed.ToString(), strSectionCassette);
            }
            #endregion

            #region Last Dispensed Counter Temp || addCurrentValue logic is not working there
            if (counterLastDispensedTemp.HasValue)
            {
                _key = String.Format(strSCCounterLastDispensedTemp, cassetteNo);

                MyIni.Write(_key, counterLastDispensedTemp.ToString(), strSectionCassette);
            }
            #endregion
        }

        /// <summary>
        /// Son Dispense Temp Counter larini, gercek yerlerine set et!
        /// <para></para>
        /// SET counterTotalDispensed = counterTotalDispensed + counterLastDispensedTemp;
        /// <para></para>
        /// SET counterLastDispensed = counterLastDispensedTemp;
        /// </summary>
        /// <param name="CassetteCount"></param>
        public static void SetCassetteCountersCounterLastDispensedTemp2CounterTotalDispensedAndCounterLastDispensed(int CassetteCount)
        {
            var MyIni = new KTR10_CDM.Ini.IniFile(iniFile);

            int cassetteNo = 0;

            string _key = "", tempValue = "";
            int counter = 0;

            for (int i = 0; i < CassetteCount; i++)
            {
                cassetteNo = i + 1;

                #region Last Dispensed Counter Temp
                _key = String.Format(strSCCounterLastDispensedTemp, cassetteNo);

                string counterLastDispensedTemp = MyIni.Read(_key, strSectionCassette);
                #endregion

                #region Total Dispensed Counter
                _key = String.Format(strSCCounterTotalDispensed, cassetteNo);

                tempValue = MyIni.Read(_key, strSectionCassette);
                counter = Convert.ToInt32(tempValue) + Convert.ToInt32(counterLastDispensedTemp);
                MyIni.Write(_key, counter.ToString(), strSectionCassette);
                #endregion

                #region Last Dispensed Counter
                _key = String.Format(strSCCounterLastDispensed, cassetteNo);
                counter = Convert.ToInt32(counterLastDispensedTemp);
                MyIni.Write(_key, counter.ToString(), strSectionCassette);
                #endregion
            }
        }

        /// <summary>
        /// Get xCassetteEmpty Error! Counter Problem!
        /// Set Loaded Counter = Loaded Dispensed + Loaded Rejected
        /// </summary>
        /// <param name="cassetteNo"></param>
        public static void SetCassetteLoadedCounterIniFile(int cassetteNo)
        {
            var MyIni = new KTR10_CDM.Ini.IniFile(iniFile);

            string _key = "";
            int counter = 0;

            _key = String.Format(strSCCounterDispensed, cassetteNo);
            string counterDispensed = MyIni.Read(_key, strSectionCassette);

            _key = String.Format(strSCCounterRejected, cassetteNo);
            string counterRejected = MyIni.Read(_key, strSectionCassette);

            counter = Convert.ToInt32(counterDispensed) + Convert.ToInt32(counterRejected);

            _key = String.Format(strSCCounterLoaded, cassetteNo);
            MyIni.Write(_key, counter.ToString(), strSectionCassette);
        }

        public static void GetRejectVaultCountersIniFile(ref int counterRejected, ref int counterRetracted)
        {
            var MyIni = new KTR10_CDM.Ini.IniFile(iniFile);

            string _key = "", tempValue = "";

            #region Loaded Counter
            _key = strSRVCounterRejected;
            if (MyIni.KeyExists(_key, strSectionRejectVault))
            {
                tempValue = MyIni.Read(_key, strSectionRejectVault);
                if (!Int32.TryParse(tempValue, out counterRejected))
                {
                    counterRejected = 0;
                    MyIni.Write(_key, "0", strSectionRejectVault);
                }
            }
            else
            {
                counterRejected = 0;
                MyIni.Write(_key, "0", strSectionRejectVault);
            }
            #endregion

            #region Dispensed Counter
            _key = strSRVCounterRetracted;
            if (MyIni.KeyExists(_key, strSectionRejectVault))
            {
                tempValue = MyIni.Read(_key, strSectionRejectVault);
                if (!Int32.TryParse(tempValue, out counterRetracted))
                {
                    counterRetracted = 0;
                    MyIni.Write(_key, "0", strSectionRejectVault);
                }
            }
            else
            {
                counterRetracted = 0;
                MyIni.Write(_key, "0", strSectionRejectVault);
            }

            #endregion
        }

        /// <summary>
        /// just set the counters, they are calculating before coming to that step
        /// </summary>
        public static void SetRejectVaultCountersIniFile(int? counterRejected, int? counterRetracted, bool addCurrentValue)
        {
            var MyIni = new KTR10_CDM.Ini.IniFile(iniFile);

            string _key = "", tempValue = ""; ;
            int counter = 0;

            #region Rejected Counter
            if (counterRejected.HasValue)
            {
                _key = strSRVCounterRejected;
                if (addCurrentValue)
                {
                    tempValue = MyIni.Read(_key, strSectionRejectVault);
                    counter = Convert.ToInt32(tempValue) + Convert.ToInt32(counterRejected);
                    MyIni.Write(_key, counter.ToString(), strSectionRejectVault);
                }
                else
                {
                    MyIni.Write(_key, counterRejected.ToString(), strSectionRejectVault);
                }
            }
            #endregion
            if (counterRetracted.HasValue)
            {
                #region Retracted Counter
                _key = strSRVCounterRetracted;
                if (addCurrentValue)
                {
                    tempValue = MyIni.Read(_key, strSectionRejectVault);
                    counter = Convert.ToInt32(tempValue) + Convert.ToInt32(counterRetracted);
                    MyIni.Write(_key, counter.ToString(), strSectionRejectVault);
                }
                else
                {
                    MyIni.Write(_key, counterRetracted.ToString(), strSectionRejectVault);
                }
            }
                #endregion
        }

        public static void GetApplicationSettings(ref string LogFilePath)
        {
            var MyIni = new KTR10_CDM.Ini.IniFile(iniFile);

            string _key = "", tempValue = "";

            #region LogFilePath
            _key = strLogFilePath;
            if (MyIni.KeyExists(_key, strSectionApplicationSettings))
            {
                tempValue = MyIni.Read(_key, strSectionApplicationSettings);
                LogFilePath = tempValue;
            }
            else
            {
                LogFilePath = "";
                MyIni.Write(_key, "", strSectionApplicationSettings);
            }
            #endregion

        }
    }

}
