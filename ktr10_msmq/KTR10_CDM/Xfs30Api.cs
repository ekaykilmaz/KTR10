using System;
using System.Collections.Generic;
using KT.WOSA.CDM;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KTR10_CDM
{

    using CDMDefinitions = XfsCdmDefine;
    using GDef = KT.WOSA.CDM.XfsGlobalDefine;
    using KT.WOSA.CDM.IMP;

    public class Xfs30Api : IXfs3
    {
        public XfsCdmImp imp = new XfsCdmImp();

        public Xfs30Api(ushort CassetteCount)
        {
            cassetteCount = CassetteCount;

            Utility.DebugMe("KTR10_CDM.Xfs30Api Constructed!");

            Utility.DebugMe("KTR10_CDM.Xfs30Api action: InitiateIniFile");

            Utility.InitiateIniFile(cassetteCount);
        }

        #region Basic Commands
        public void OpenRegister()
        {
            string strLogicalName = "CashDispenser";
            string s = "";

            Trace("(OpenRegister) --> Begin...");

            int hRet = imp.OpenSP_Impl(strLogicalName);

            #region debug
            try
            {
                if (GDef.WFS_SUCCESS != hRet)
                {
                    s = "(OpenRegister) --> Failed! Return: " + hRet;
                }
                else
                {
                    s = "(OpenRegister) --> Success! Return: " + hRet;
                }

                Trace(s);
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);

            KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION WFSVersion;
            KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION SrvcVersion;
            KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION SPIVersion;
            GetSpInfoVersion(out WFSVersion, out SrvcVersion, out SPIVersion);
        }

        public void GetSpInfoVersion(out KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION WFSVersion, out KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION SrvcVersion, out KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION SPIVersion)
        {
            imp.GetSpInfoVersion_Impl(out WFSVersion, out  SrvcVersion, out SPIVersion);

            #region debug
            try
            {
                string s = "";
                s += "\r\nWFSVersion[ ";
                s += "wVersion(0x" + WFSVersion.wVersion.ToString("X").PadLeft(4, '0') + ")-";
                s += "wLowVersion(0x" + WFSVersion.wLowVersion.ToString("X").PadLeft(4, '0') + ")-";
                s += "wHighVersion(0x" + WFSVersion.wHighVersion.ToString("X").PadLeft(4, '0') + ")-";
                s += "tszDescription(" + WFSVersion.szDescription + ")-";
                s += "szSystemStatus(" + WFSVersion.szSystemStatus + ")";
                s += " ]\r\n";

                s += "SrvcVersion[ ";
                s += "wVersion(0x" + SrvcVersion.wVersion.ToString("X").PadLeft(4, '0') + ")-";
                s += "wLowVersion(0x" + SrvcVersion.wLowVersion.ToString("X").PadLeft(4, '0') + ")-";
                s += "wHighVersion(0x" + SrvcVersion.wHighVersion.ToString("X").PadLeft(4, '0') + ")-";
                s += "szDescription(" + SrvcVersion.szDescription + ")-";
                s += "szSystemStatus(" + SrvcVersion.szSystemStatus + ")";
                s += " ]\r\n";

                s += "SPIVersion[ ";
                s += "wVersion(0x" + SPIVersion.wVersion.ToString("X").PadLeft(4, '0') + ")-";
                s += "wLowVersion( 0x" + SPIVersion.wLowVersion.ToString("X").PadLeft(4, '0') + ")-";
                s += "wHighVersion(0x" + SPIVersion.wHighVersion.ToString("X").PadLeft(4, '0') + ")-";
                s += "szDescription(" + SPIVersion.szDescription + ")-";
                s += "szSystemStatus(" + SPIVersion.szSystemStatus + ")";
                s += " ]";

                Trace(s);
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }

            #endregion
        }

        public void Close()
        {
            imp.CloseSP_Impl();

            #region debug
            try
            {
                string s = "\tClosed!";

                Trace(s);
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion
        }

        #endregion

        void CheckIfNoRegisterSoRegisterOneMoreTime()
        {
            XfsCdmDefine.WFSCDMSTATUS_Dim lpDataOut;
            System.IntPtr FreeResultHandle;

            int hRet = imp.WFS_INF_CDM_STATUS_Impl(out lpDataOut, out FreeResultHandle);
            String s = "<Check> WFS_INF_CDM_STATUS_Impl: ";
            s += "return :" + hRet;

            Trace(s);

            imp.WFSFreeResult_Impl(FreeResultHandle);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
            {
                OpenRegister();
            }
        }

        private int CheckIfNoRegisterSoRegisterOneMoreTime(out XfsCdmDefine.WFSCDMSTATUS_Dim lpDataOut)
        {
            System.IntPtr FreeResultHandle;

            int hRet = imp.WFS_INF_CDM_STATUS_Impl(out lpDataOut, out FreeResultHandle);
            String s = "<Check> WFS_INF_CDM_STATUS_Impl: ";
            s += "return :" + hRet;
            Trace(s);

            imp.WFSFreeResult_Impl(FreeResultHandle);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
            {
                OpenRegister();
            }

            return hRet;
        }

        #region Info Commands
        /// <summary>
        /// This command is used to obtain the status of the CDM. It may also return vendor-specific status information
        /// </summary>
        /// <returns></returns>
        public XfsCdmDefine.WFSCDMSTATUS_Dim WFS_INF_CDM_STATUS()
        {
            XfsCdmDefine.WFSCDMSTATUS_Dim lpDataOut;
            System.IntPtr FreeResultHandle;

            int hRet = CheckIfNoRegisterSoRegisterOneMoreTime(out lpDataOut);
            
            string s = "";

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
            {
                hRet = imp.WFS_INF_CDM_STATUS_Impl(out lpDataOut, out FreeResultHandle);

                s = "WFS_INF_CDM_STATUS_Impl: ";
                s += "return :" + hRet;

                imp.WFSFreeResult_Impl(FreeResultHandle);
            }

            #region Debug
            try
            {

                string fwDevice = lpDataOut.fwDevice.ToString().PadLeft(4, '0');
                string fwSafeDoor = lpDataOut.fwSafeDoor.ToString().PadLeft(4, '0');
                string fwDispenser = lpDataOut.fwDispenser.ToString().PadLeft(4, '0');
                string fwIntermediateStacker = lpDataOut.fwIntermediateStacker.ToString().PadLeft(4, '0');
                s += "\r\n";
                s += "\t";
                s += "fwDevice(" + KTR10_CDM.Xfs30Definitions.CDM.CDMStatusDeviceConvert(fwDevice) + "(" + fwDevice + "))-";
                s += "fwSafeDoor(" + KTR10_CDM.Xfs30Definitions.CDM.CDMStatusDeviceConvert(fwSafeDoor) + "(" + fwSafeDoor + "))-";
                s += "fwDispenser(" + KTR10_CDM.Xfs30Definitions.CDM.CDMStatusDispenserConvert(fwDispenser) + "(" + fwDispenser + "))-";
                s += "fwIntermediateStacker(" + KTR10_CDM.Xfs30Definitions.CDM.CDMStatusIntermediateStackerConvert(fwIntermediateStacker) + "(" + fwIntermediateStacker + "))";

                s += "\r\n";
                s += "\t";
                s += "lppPositions";
                foreach (KT.WOSA.CDM.XfsCdmDefine.WFSCDMOUTPOS pos in lpDataOut.lppPositionsDim)
                {
                    s += "\r\n";
                    s += "\t\t";
                    s += "fwPstn(" + KTR10_CDM.Xfs30Definitions.CDM.CDMOUTPosPositionStatusConvert(pos.fwPosition) + "(" + pos.fwPosition + "))-";
                    s += "fwPstnStts(" + KTR10_CDM.Xfs30Definitions.CDM.CDMOUTPosPositionStatusConvert(pos.fwPositionStatus) + "(" + pos.fwPositionStatus + "))-";
                    s += "fwShttr(" + KTR10_CDM.Xfs30Definitions.CDM.CDMOUTPosShutterConvert(pos.fwShutter) + "(" + pos.fwShutter + "))-";
                    s += "fwTrnsprt(" + KTR10_CDM.Xfs30Definitions.CDM.CDMOUTPosTransportConvert(pos.fwTransport) + "(" + pos.fwTransport + "))-";
                    s += "fwTrnsprtStts(" + KTR10_CDM.Xfs30Definitions.CDM.CDMOUTPosTransportStatusConvert(pos.fwTransportStatus) + "(" + pos.fwTransportStatus + "))";
                }

                s += "\r\n";
                s += "\t";

                s += "szExtra(";
                if (IntPtr.Zero.Equals(lpDataOut.lpszExtra) == false)
                {
                    for (Int32 i = 0; true; i++)
                    {
                        System.Byte b1 = Marshal.ReadByte(lpDataOut.lpszExtra, i);
                        System.Byte b2 = Marshal.ReadByte(lpDataOut.lpszExtra, i + 1);
                        if (b1 == 0 && b2 == 0)
                            break;
                        if (b1 == 0)
                            s += "-";
                        else
                            s += Convert.ToChar(b1);
                    }
                }
                s += ")";
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }

            #endregion

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
            {
                throw new Exceptions.StatusException(lpDataOut, hRet);
            }

            return lpDataOut;
        }

        /// <summary>
        /// This command retrieves the capabilities of the CDM. It may also return vendor specific capability information. The intermediate stacker and the transport are treated as separate areas. Some devices may have the capability to move items from the cash units to the intermediate stacker while there are items on the transport. Similarly some devices may be able to retract items to the transport or the cash units while there are items on the intermediate stacker.  
        /// </summary>
        /// <returns>LPWFSCDMCAPS</returns>
        public XfsCdmDefine.WFSCDMCAPS WFS_INF_CDM_CAPABILITIES()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            System.IntPtr FreeResultHandle = IntPtr.Zero;
            XfsCdmDefine.WFSCDMCAPS lpCaps;

            int hRet = imp.WFS_INF_CDM_CAPABILITIES_Impl(out lpCaps, out FreeResultHandle);

            String s = "WFS_INF_CDM_CAPABILITIES_Impl: ";
            s += "return :" + hRet;

            #region Debug
            try
            {
                s += "\r\n";
                s += "\twClass(" + lpCaps.wClass + ")";
                s += "fwType(" + KTR10_CDM.Xfs30Definitions.CDM.CDMCAPSTypeConvert(lpCaps.fwType) + "(" + lpCaps.fwType.ToString() + ")-";
                s += "wMaxDispenseItems(" + lpCaps.wMaxDispenseItems.ToString() + ")-";
                s += "bCompound(" + (lpCaps.bCompound == 1 ? "true" : "false") + ")-";
                s += "bShutter(" + (lpCaps.bShutter == 1 ? "true" : "false") + ")-";
                s += "bShutterControl(" + (lpCaps.bShutterControl == 1 ? "true" : "false") + ")";
                s += "\r\n";

                s += "\tfwRetractAreas(" + lpCaps.fwRetractAreas + ")-";
                s += "fwRetractTransportActions(" + lpCaps.fwRetractStackerActions + ")-";
                s += "fwRetractStackerActions(" + lpCaps.fwRetractTransportActions + ")-";
                s += "bSafeDoor(" + (lpCaps.bSafeDoor == 1 ? "true" : "false") + ")-";
                s += "bCashBox(" + (lpCaps.bCashBox == 1 ? "true" : "false") + ")-";
                s += "bIntermediateStacker(" + (lpCaps.bIntermediateStacker == 1 ? "true" : "false") + ")-";
                s += "bItemsTakenSensor:" + (lpCaps.bItemsTakenSensor == 1 ? "true" : "false") + ")-";
                s += "\tfwPositions:" + lpCaps.fwPositions + ")-";
                s += "\tfwMoveItems:" + lpCaps.fwMoveItems + ")-";
                s += "\tfwExchangeType:" + lpCaps.fwExchangeType + ")";
                s += "\r\n";

                s += "\tlpszExtra[";
                if (IntPtr.Zero.Equals(lpCaps.lpszExtra) == false)
                {
                    for (Int32 i = 0; true; i++)
                    {
                        System.Byte b1 = Marshal.ReadByte(lpCaps.lpszExtra, i);
                        System.Byte b2 = Marshal.ReadByte(lpCaps.lpszExtra, i + 1);
                        if (b1 == 0 && b2 == 0)
                            break;
                        if (b1 == 0)
                            s += "\r\n";

                        s += Convert.ToChar(b1);
                    }
                }
                s += "]";
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }

            #endregion

            Trace(s);

            imp.WFSFreeResult_Impl( FreeResultHandle);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.CapabilityException(lpCaps, hRet);

            return lpCaps;
        }

        /// <summary>
        /// This command is used to obtain information regarding the status and contents of the cash units in the CDM.
        /// </summary>
        /// <returns></returns>
        public CDMDefinitions.WFSCDMCUINFO_Dim WFS_INF_CDM_CASH_UNIT_INFO()
        {
            String s = "WFS_INF_CDM_CASH_UNIT_INFO_Impl: ";

            CheckIfNoRegisterSoRegisterOneMoreTime();

            System.IntPtr FreeResultHandle = IntPtr.Zero;

            CDMDefinitions.WFSCDMCUINFO_Dim lpCashUnitInfo;
            int hRet = imp.WFS_INF_CDM_CASH_UNIT_INFO_Impl(out lpCashUnitInfo, out FreeResultHandle);

            s += "return :" + hRet;

            Trace(s);

            imp.WFSFreeResult_Impl( FreeResultHandle);

            if (hRet == XfsGlobalDefine.WFS_SUCCESS)
            {
                DebugCashUnitInfo(lpCashUnitInfo);
            }

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.CashUnitInfoException(lpCashUnitInfo, hRet);

            return lpCashUnitInfo;
        }

        /// <summary>
        /// This command returns each exponent assigned to each currency known to the service provider
        /// </summary>
        /// <returns></returns>
        public List<CDMDefinitions.WFSCDMCURRENCYEXP> WFS_INF_CDM_CURRENCY_EXP()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            System.IntPtr FreeResultHandle = IntPtr.Zero;
            List<CDMDefinitions.WFSCDMCURRENCYEXP> CurrencyExpList;

            int hRet = imp.WFS_INF_CDM_CURRENCY_EXP_Impl(out CurrencyExpList, out  FreeResultHandle);

            string s = "WFS_INF_CDM_CURRENCY_EXP: ";
            s += "return :" + hRet;

            #region Debug
            try
            {
                String strTmp = "";
                if (hRet == XfsGlobalDefine.WFS_SUCCESS)
                {
                    foreach (CDMDefinitions.WFSCDMCURRENCYEXP CurrencyExp in CurrencyExpList)
                    {
                        s += "\r\n";
                        strTmp = "";
                        foreach (SByte v in CurrencyExp.cCurrencyID)
                        {
                            strTmp += Convert.ToChar(v);
                        }

                        s += "\tcCurrencyID(" + strTmp + ")-";
                        s += "sExponent(" + CurrencyExp.sExponent + ")";
                    }
                }
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            Trace(s);

            imp.WFSFreeResult_Impl( FreeResultHandle);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.CurrencyExponentException(CurrencyExpList, hRet);

            return CurrencyExpList;
        }

        /// <summary>
        /// This command returns each exponent assigned to each currency known to the service provider
        /// </summary>
        /// <returns></returns>
        public List<CDMDefinitions.WFSCDMMIXTYPE> WFS_INF_CDM_MIX_TYPES()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            System.IntPtr FreeResultHandle = IntPtr.Zero;
            List<CDMDefinitions.WFSCDMMIXTYPE> MixTypesList;

            int hRet = imp.WFS_INF_CDM_MIX_TYPES_Impl(out MixTypesList, out FreeResultHandle);

            string s = "WFS_INF_CDM_MIX_TYPES: ";
            s += "return :" + hRet;

            #region Debug
            try
            {
                if (hRet == XfsGlobalDefine.WFS_SUCCESS)
                {
                    foreach (CDMDefinitions.WFSCDMMIXTYPE MixTypes in MixTypesList)
                    {
                        s += "\r\n";
                        s += "\tusMixNumber(" + MixTypes.usMixNumber + ")-usMixType(" + MixTypes.usMixType + ")-usSubType(" + MixTypes.usSubType + ")-lpszName(" + MixTypes.lpszName + ")";
                    }
                }
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }

            #endregion

            Trace(s);

            imp.WFSFreeResult_Impl( FreeResultHandle);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.MixTypeException(MixTypesList, hRet);

            return MixTypesList;
        }

        /// <summary>
        /// This command is used to obtain the house mix table specified by the supplied mix number
        /// </summary>
        /// <returns></returns>
        public CDMDefinitions.WFSCDMMIXTABLE_Dim WFS_INF_CDM_MIX_TABLE()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            System.IntPtr FreeResultHandle = IntPtr.Zero;
            UInt16 lpusMixNumber = 0;
            CDMDefinitions.WFSCDMMIXTABLE_Dim MixTable;

            int hRet = imp.WFS_INF_CDM_MIX_TABLE_Impl(lpusMixNumber, out MixTable, out FreeResultHandle);

            string s = "WFS_INF_CDM_MIX_TABLE: ";
            s += "return :" + hRet;

            #region Debug
            try
            {
                if (hRet == XfsGlobalDefine.WFS_SUCCESS)
                {
                    s += "\r\n";
                    s += "\tusMixNumber=" + MixTable.usMixNumber + ")-";
                    s += "lpszName(" + MixTable.lpszName + ")-";
                    s += "usRows(" + MixTable.usRows + ")-";
                    s += "usCols(" + MixTable.usCols + ")-";
                    s += "lpulMixHeader(" + MixTable.lpulMixHeader + ")-";
                    foreach (System.UInt32 v in MixTable.lpulMixHeaderDim)
                    {
                        s += "\r\n";
                        s += "\t\tlpulMixHeader(" + v + ")";
                    }
                    foreach (CDMDefinitions.WFSCDMMIXROW_Dim v in MixTable.lppMixRowsDim)
                    {
                        s += "\r\n";
                        s += "\t\tlppMixRows.ulAmount(" + v.ulAmount + ")";
                        foreach (UInt16 t in v.lpusMixtureDim)
                        {
                            s += "\r\n";
                            s += "\t\t\tlppMixRows.lpusMixture(" + t + ")";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            Trace(s);

            imp.WFSFreeResult_Impl( FreeResultHandle);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.MixTableException(MixTable, hRet);

            return MixTable;
        }

        /// <summary>
        /// This command is used to obtain the status of the most recent attempt to present items to the customer. The items may have been presented as a result of the WFS_CMD_CDM_PRESENT or WFS_CMD_CDM_DISPENSE command
        /// </summary>
        /// <returns></returns>
        public CDMDefinitions.WFSCDMPRESENTSTATUS_Dim WFS_INF_CDM_PRESENT_STATUS()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            System.IntPtr FreeResultHandle = IntPtr.Zero;
            UInt32 dwPosition = XfsCdmDefine.WFS_CDM_POSNULL;
            CDMDefinitions.WFSCDMPRESENTSTATUS_Dim PresentStatus;

            int hRet = imp.WFS_INF_CDM_PRESENT_STATUS_Impl(dwPosition, out PresentStatus, out  FreeResultHandle);

            string s = "WFS_INF_CDM_PRESENT_STATUS: ";
            s += "return :" + hRet;

            #region Debug
            try
            {
                do
                {
                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                        break;
                    if (IntPtr.Zero.Equals(PresentStatus.lpDenomination) == false)
                    {
                        s += "\r\n";
                        String strTmp = "";
                        foreach (SByte v in PresentStatus.DenominationDim.cCurrencyID)
                        {
                            if (v != '\0')
                            {
                                strTmp += Convert.ToChar(v);
                            }
                        }
                        s += "\tulCashBox(" + PresentStatus.DenominationDim.ulCashBox + ")-";
                        s += "cCurrencyID(" + strTmp + ")-";
                        s += "ulAmount(" + PresentStatus.DenominationDim.ulAmount + ")-";
                        s += "usCount(" + PresentStatus.DenominationDim.usCount + ")-";
                        foreach (UInt32 v in PresentStatus.DenominationDim.lpulValuesDim)
                        {
                            s += "\r\n";
                            s += "\t\tlpulValues(" + v + ")";
                        }
                        PresentStatus.DenominationDim.lpulValuesDim.Clear();
                    }

                    s += "\r\n";
                    s += "\tPresentStatus.wPresentState(" + PresentStatus.wPresentState + ")";
                    s += "lpszExtra[";
                    if (IntPtr.Zero.Equals(PresentStatus.lpszExtra) == false)
                    {
                        s += "\r\n";
                        for (Int32 i = 0; true; i++)
                        {
                            System.Byte b1 = Marshal.ReadByte(PresentStatus.lpszExtra, i);
                            System.Byte b2 = Marshal.ReadByte(PresentStatus.lpszExtra, i + 1);
                            if (b1 == 0 && b2 == 0)
                                break;
                            if (b1 == 0)
                            {
                                s += "\r\n";
                            }
                            s += Convert.ToChar(b1);
                        }
                    }
                    s += "]";

                } while (false);
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }

            #endregion

            Trace(s);

            imp.WFSFreeResult_Impl( FreeResultHandle);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.PresentStatusException(PresentStatus, hRet);

            return PresentStatus;
        }

        #endregion

        #region Execute Commands
        /// <summary>
        /// This command provides a denomination. A denomination specifies the number of items which are required from each cash unit in order to satisfy a given amount. The denomination depends upon the currency, the mix algorithm and any partial denomination supplied by the application
        /// </summary>
        /// <returns></returns>
        public CDMDefinitions.WFSCDMDENOMINATION_Dim WFS_CMD_CDM_DENOMINATE(uint amount, string currency, bool log = true)
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime(log);

            System.IntPtr FreeResultHandle = IntPtr.Zero;
            CDMDefinitions.WFSCDMDENOMINATION_Dim lpDenominationOut = new CDMDefinitions.WFSCDMDENOMINATION_Dim();

            CDMDefinitions.WFSCDMDENOMINATE_Dim lpDenominateIn = new CDMDefinitions.WFSCDMDENOMINATE_Dim();
            lpDenominateIn.usTellerID = TellerId;
            lpDenominateIn.usMixNumber = MixNumber;

            lpDenominateIn.lpDenominationDim = new List<CDMDefinitions.WFSCDMDENOMINATION_Dim>();
            lpDenominateIn.lpDenominationDim.Add(FillWFSCDMDENOMINATION_Dim(amount, currency));

            String s = "WFS_CMD_CDM_DENOMINATE: ";
            s += "\r\n";
            s += "\tusTellerID(" + lpDenominateIn.usTellerID.ToString() + ")-";
            s += "usMixNumber(" + lpDenominateIn.usMixNumber.ToString() + ")";

            if (log)
                Trace(s);

            int hRet = imp.WFS_CMD_CDM_DENOMINATE_Impl(lpDenominateIn, out lpDenominationOut, out  FreeResultHandle);

            imp.WFSFreeResult_Impl( FreeResultHandle);

            s = "";
            s += "\r\n";
            s += "return :" + hRet;

            if (log)
                Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.DenominateException(lpDenominationOut, hRet);
                            
            return lpDenominationOut;
        }

        /// <summary>
        /// This command performs the dispensing of items to the customer. The command provides the same functionality as the WFS_CMD_CDM_DENOMINATE command plus the additional functionality of dispensing the items.
        /// </summary>
        public CDMDefinitions.WFSCDMDENOMINATION_Dim WFS_CMD_CDM_DISPENSE(uint amount, string currency)
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            String strTmp = "";
            CDMDefinitions.WFSCDMDISPENSE_Dim lpDispense = new CDMDefinitions.WFSCDMDISPENSE_Dim();
            lpDispense.bPresent = (uint)(this.AutoPresent ? 1 : 0);
            lpDispense.fwPosition = dispensefwPosition;
            lpDispense.usMixNumber = MixNumber;
            lpDispense.usTellerID = TellerId;

            lpDispense.lpDenominationDim = new List<CDMDefinitions.WFSCDMDENOMINATION_Dim>(1);
            lpDispense.lpDenominationDim.Add(FillWFSCDMDENOMINATION_Dim(amount, currency));

            CDMDefinitions.WFSCDMDENOMINATION_Dim lpDenominationOut;
            System.IntPtr FreeResultHandle = IntPtr.Zero;

            string s = "WFS_CMD_CDM_DISPENSE: ";
            s += "\r\n";
            s += "bPresent(" + lpDispense.bPresent.ToString() + ")-";
            s += "fwPosition(" + lpDispense.fwPosition.ToString() + ")-";
            s += "usMixNumber(" + lpDispense.usMixNumber.ToString() + ")-";
            s += "usTellerID(" + lpDispense.usTellerID.ToString() + ")";
            Trace(s);

            int hRet = imp.WFS_CMD_CDM_DISPENSE_Impl(lpDispense, out lpDenominationOut, out FreeResultHandle);

            s = "";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            s = "";
            #region Debug
            try
            {
                if (hRet == XfsGlobalDefine.WFS_SUCCESS)
                {
                    s += "\r\n";
                    foreach (SByte v in lpDenominationOut.cCurrencyID)
                    {
                        strTmp += Convert.ToChar(v);
                    }
                    s += "\tcCurrencyID(" + strTmp + ")-";
                    s += "ulAmount(" + lpDenominationOut.ulAmount + ")-";
                    s += "usCount(" + lpDenominationOut.usCount + ")-";
                    s += "ulValues(<" + lpDenominationOut.usCount;
                    foreach (UInt32 v in lpDenominationOut.lpulValuesDim)
                    {
                        s += " " + v + " ";
                    }
                    s += ">)-";

                    s += "ulCashBox(" + lpDenominationOut.ulCashBox + ")";
                }
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            Trace(s);

            imp.WFSFreeResult_Impl( FreeResultHandle);

            #region S E T  I N I  F I L E
            //[15:57:56] WFSAsyncExecute ( WFS_CMD_CDM_DISPENSE (302) ) completed with WFS_SUCCESS (0) [ReqID: 7]
            //        cCurrencyID: TRY
            //        ulAmount: 20
            //        usCount: 6
            //            ulValues: 0 x 0
            //            ulValues: 0 x 0
            //            ulValues: 0 x 100
            //            ulValues: 0 x 50
            //            ulValues: 1 x 20
            //            ulValues: 0 x 10
            //        ulCashBox: 0

            if (hRet == XfsGlobalDefine.WFS_SUCCESS)
            {
                int slotNum = 0;

                foreach (UInt32 v in lpDenominationOut.lpulValuesDim)
                {
                    slotNum++;

                    if (v > 0)
                    {
                        Utility.SetCassetteCountersIniFile(slotNum, null, (int)v, null, true, null, null, (int)v);
                    }
                    else
                    {
                        //eger kasetten para cikmadiysa, onu sifirla
                        Utility.SetCassetteCountersIniFile(slotNum, null, null, null, true, null, null, 0);
                    }
                }
            }

            #endregion

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.DispenseException(lpDenominationOut, hRet);

            return lpDenominationOut;
        }

        /// <summary>
        /// This command will move items to the exit position for removal by the user.
        /// </summary>
        public void WFS_CMD_CDM_PRESENT()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            string s = "WFS_CMD_CDM_PRESENT: ";
            s += "\r\n\tpresentfwPosition(" + presentfwPosition.ToString() + ")";

            Trace(s);

            int hRet = imp.WFS_CMD_CDM_PRESENT_Impl(PresentfwPosition);

            s = "";
            s += "\r\n";
            s += "return :" + hRet;
            Trace(s);
            
            #region S E T  I N I  F I L E

            if (hRet == XfsGlobalDefine.WFS_SUCCESS)
            {
                Utility.SetCassetteCountersCounterLastDispensedTemp2CounterTotalDispensedAndCounterLastDispensed(cassetteCount);
            }

            #endregion

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// This command will move items from the intermediate stacker and transport to the reject cash unit.
        /// </summary>
        public void WFS_CMD_CDM_REJECT()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            string s = "WFS_CMD_CDM_REJECT:\t\n";

            Trace(s);

            int hRet = imp.WFS_CMD_CDM_REJECT_Impl();

            s = "";
            s += "\r\n";
            s += "return :" + hRet;
            Trace(s);


            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// This command will retract items which may have been in customer access. Retracted items will be moved to either a retract cash unit, the reject cash unit, the transport or the intermediate stacker. After the items are retracted the shutter is closed automatically
        /// </summary>
        public void WFS_CMD_CDM_RETRACT()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            CDMDefinitions.WFSCDMRETRACT lpRetract = new CDMDefinitions.WFSCDMRETRACT();
            lpRetract.fwOutputPosition = retractfwOutputPosition;
            lpRetract.usRetractArea = retractusRetractArea;
            lpRetract.usIndex = retractusIndex;

            string s = "WFS_CMD_CDM_RETRACT: ";
            s += "\r\n";
            s += "\tfwOutputPosition(" + lpRetract.fwOutputPosition.ToString() + ")-";
            s += "usRetractArea(" + lpRetract.usRetractArea.ToString() + ")-";
            s += "usIndex(" + lpRetract.usIndex.ToString() + ")";
            Trace(s);

            int hRet = imp.WFS_CMD_CDM_RETRACT_Impl(lpRetract);

            s = "";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// This command opens the shutter.
        /// </summary>
        public void WFS_CMD_CDM_OPEN_SHUTTER()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            string s = "WFS_CMD_CDM_OPEN_SHUTTER: ";
            s += "\r\n\tshutterfwPosition(" + shutterfwPosition.ToString() + ")";
            Trace(s);

            int hRet = imp.WFS_CMD_CDM_OPEN_SHUTTER_Impl(shutterfwPosition);

            s = "";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// This command closes the shutter.
        /// </summary>
        public void WFS_CMD_CDM_CLOSE_SHUTTER()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            string s = "WFS_CMD_CDM_CLOSE_SHUTTER:\t\n";
            s += "\r\n\tshutterfwPosition(" + shutterfwPosition.ToString() + ")";

            Trace(s);

            int hRet = imp.WFS_CMD_CDM_CLOSE_SHUTTER_Impl(shutterfwPosition);

            s = "";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// This command is used to adjust information regarding the status and contents of the cash units present in the CDM.
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CDM_SET_CASH_UNIT_INFO(CDMDefinitions.WFSCDMCUINFO_Dim lpCashUnitInfo)
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            string s = "WFS_CMD_CDM_SET_CASH_UNIT_INFO: ";
            s += "\r\n";

            Trace(s);

            int hRet = imp.WFS_CMD_CDM_SET_CASH_UNIT_INFO_Impl(lpCashUnitInfo);

            s = "";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet == XfsGlobalDefine.WFS_SUCCESS)
            {
                DebugCashUnitInfo(lpCashUnitInfo);
            }

            #region S E T  I N I  F I L E
            foreach (KT.WOSA.CDM.XfsCdmDefine.WFSCDMCASHUNIT_Dim LogicCu in lpCashUnitInfo.lppListDim)
            {
                //item.counterLoaded = cassette.InitialTotalInCassette;
                int counterDispensed = 0;
                int counterRejected = 0;
                int counterTotalDispensed = 0;
                int counterLastDispensed = 0;
                int counterLastDispensedTemp = 0;

                switch (KTR10_CDM.Xfs30Definitions.CDM.CDMCashUnitTypeConvert(LogicCu.usType))
                {
                    case KTR10_CDM.Xfs30Definitions.CDM.CDMCashUnitType.WFS_CDM_TYPERECYCLING:

                        Utility.SetCassetteCountersIniFile((int)LogicCu.usNumber, (int)LogicCu.ulInitialCount, counterDispensed, counterRejected, false, counterTotalDispensed, counterLastDispensed, counterLastDispensedTemp);

                        break;
                    default:
                        break;
                }
            }

            Utility.SetRejectVaultCountersIniFile(0, 0, false);

            #endregion

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.SetCashUnitException(lpCashUnitInfo, hRet);
        }

        /// <summary>
        /// This command is used by the application to perform a hardware reset which will attempt to return the CDM device to a known good state. This command does not over-ride a lock obtained on another application or service handle, nor can it be performed while the CDM is in the exchange state.
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CDM_RESET()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            CDMDefinitions.WFSCDMITEMPOSITION_Dim lpItemPositon = new CDMDefinitions.WFSCDMITEMPOSITION_Dim();

            lpItemPositon.fwOutputPosition = resetfwOutputPosition;
            lpItemPositon.usNumber = resetusNumber;

            lpItemPositon.lpRetractAreaDim = new List<CDMDefinitions.WFSCDMRETRACT>(1);
            CDMDefinitions.WFSCDMRETRACT Retract = new CDMDefinitions.WFSCDMRETRACT();
            Retract.fwOutputPosition = resetRetractfwOutputPosition;
            Retract.usRetractArea = resetRetractusRetractArea;
            Retract.usIndex = resetRetractusIndex;
            lpItemPositon.lpRetractAreaDim.Add(Retract);

            string s = "WFS_CMD_CDM_RESET: ";
            s += "\r\n";
            s += "\tfwOutputPosition(" + resetfwOutputPosition.ToString() + ")-";
            s += "usNumber(" + resetusNumber.ToString() + ")-";
            s += "Retract.fwOutputPosition(" + resetRetractfwOutputPosition.ToString() + ")-";
            s += "Retract.usRetractArea(" + resetRetractusRetractArea.ToString() + ")-";
            s += "Retract.usIndex(" + resetRetractusIndex.ToString() + ")";
            Trace(s);

            System.Int32 hRet = imp.WFS_CMD_CDM_RESET_Impl(lpItemPositon);

            s = "";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        #endregion

        #region Execute Async Commands
        /// <summary>
        /// This command provides a denomination. A denomination specifies the number of items which are required from each cash unit in order to satisfy a given amount. The denomination depends upon the currency, the mix algorithm and any partial denomination supplied by the application
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CDM_DENOMINATE_Async(uint amount, string currency)
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            CDMDefinitions.WFSCDMDENOMINATE_Dim lpDenominateIn = new CDMDefinitions.WFSCDMDENOMINATE_Dim();
            lpDenominateIn.usTellerID = TellerId;
            lpDenominateIn.usMixNumber = MixNumber;

            #region lpDenominationDim
            lpDenominateIn.lpDenominationDim = new List<CDMDefinitions.WFSCDMDENOMINATION_Dim>();
            lpDenominateIn.lpDenominationDim.Add(FillWFSCDMDENOMINATION_Dim(amount, currency));

            #endregion

            int hRet = imp.WFS_CMD_CDM_DENOMINATE_AsyncImpl(lpDenominateIn);

            String s = "WFS_CMD_CDM_DENOMINATE_Async: ";
            s += "\r\n\t lpDenominateIn.usTellerID(" + lpDenominateIn.usTellerID.ToString() + ")";
            s += "\r\n\t lpDenominateIn.usMixNumber(" + lpDenominateIn.usMixNumber.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command performs the dispensing of items to the customer. The command provides the same functionality as the WFS_CMD_CDM_DENOMINATE command plus the additional functionality of dispensing the items.
        /// </summary>
        public void WFS_CMD_CDM_DISPENSE_Async(uint amount, string currency)
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            CDMDefinitions.WFSCDMDISPENSE_Dim lpDispense = new CDMDefinitions.WFSCDMDISPENSE_Dim();
            lpDispense.bPresent = (uint)(this.AutoPresent ? 1 : 0);
            lpDispense.fwPosition = dispensefwPosition;
            lpDispense.usMixNumber = MixNumber;
            lpDispense.usTellerID = TellerId;

            #region lpDenominationDim
            lpDispense.lpDenominationDim = new List<CDMDefinitions.WFSCDMDENOMINATION_Dim>(1);
            lpDispense.lpDenominationDim.Add(FillWFSCDMDENOMINATION_Dim(amount, currency));

            #endregion

            int hRet = imp.WFS_CMD_CDM_DISPENSE_AsyncImpl(lpDispense);

            string s = "WFS_CMD_CDM_DISPENSE_Async: ";
            s += "\r\n\t lpDispense.bPresent(" + lpDispense.bPresent.ToString() + ")";
            s += "\r\n\t lpDispense.fwPosition(" + lpDispense.fwPosition.ToString() + ")";
            s += "\r\n\t lpDispense.usMixNumber(" + lpDispense.usMixNumber.ToString() + ")";
            s += "\r\n\t lpDispense.usTellerID(" + lpDispense.usTellerID.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command will move items to the exit position for removal by the user.
        /// </summary>
        public void WFS_CMD_CDM_PRESENT_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CDM_PRESENT_AsyncImpl(presentfwPosition);

            string s = "WFS_CMD_CDM_PRESENT_Async: ";
            s += "\r\n\t presentfwPosition(" + presentfwPosition.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command will move items from the intermediate stacker and transport to the reject cash unit.
        /// </summary>
        public void WFS_CMD_CDM_REJECT_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CDM_REJECT_AsyncImpl();

            string s = "WFS_CMD_CDM_REJECT_Async:\t\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command will retract items which may have been in customer access. Retracted items will be moved to either a retract cash unit, the reject cash unit, the transport or the intermediate stacker. After the items are retracted the shutter is closed automatically
        /// </summary>
        public void WFS_CMD_CDM_RETRACT_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            CDMDefinitions.WFSCDMRETRACT lpRetract = new CDMDefinitions.WFSCDMRETRACT();
            lpRetract.fwOutputPosition = retractfwOutputPosition;
            lpRetract.usRetractArea = retractusRetractArea;
            lpRetract.usIndex = retractusIndex;

            int hRet = imp.WFS_CMD_CDM_RETRACT_AsyncImpl(lpRetract);

            string s = "WFS_CMD_CDM_RETRACT_Async: ";
            s += "\r\n\t fwOutputPosition(" + retractfwOutputPosition.ToString() + ")";
            s += "\r\n\t usRetractArea(" + retractusRetractArea.ToString() + ")";
            s += "\r\n\t usIndex(" + retractusIndex.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command opens the shutter.
        /// </summary>
        public void WFS_CMD_CDM_OPEN_SHUTTER_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CDM_OPEN_SHUTTER_AsyncImpl(shutterfwPosition);

            string s = "WFS_CMD_CDM_OPEN_SHUTTER_Async: ";
            s += "\r\n\t shutterfwPosition(" + shutterfwPosition.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command closes the shutter.
        /// </summary>
        public void WFS_CMD_CDM_CLOSE_SHUTTER_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CDM_CLOSE_SHUTTER_AsyncImpl(shutterfwPosition);

            string s = "WFS_CMD_CDM_CLOSE_SHUTTER_Async:\t\n";
            s += "\r\n\t shutterfwPosition(" + shutterfwPosition.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command is used to adjust information regarding the status and contents of the cash units present in the CDM.
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CDM_SET_CASH_UNIT_INFO_Async(CDMDefinitions.WFSCDMCUINFO_Dim lpCashUnitInfo)
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CDM_SET_CASH_UNIT_INFO_AsyncImpl(lpCashUnitInfo);

            string s = "WFS_CMD_CDM_SET_CASH_UNIT_INFO_Async: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command is used by the application to perform a hardware reset which will attempt to return the CDM device to a known good state. This command does not over-ride a lock obtained on another application or service handle, nor can it be performed while the CDM is in the exchange state.
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CDM_RESET_Async()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            CDMDefinitions.WFSCDMITEMPOSITION_Dim lpItemPositon = new CDMDefinitions.WFSCDMITEMPOSITION_Dim();

            lpItemPositon.fwOutputPosition = resetfwOutputPosition;
            lpItemPositon.usNumber = resetusNumber;

            lpItemPositon.lpRetractAreaDim = new List<CDMDefinitions.WFSCDMRETRACT>(1);
            CDMDefinitions.WFSCDMRETRACT Retract = new CDMDefinitions.WFSCDMRETRACT();
            Retract.fwOutputPosition = resetRetractfwOutputPosition;
            Retract.usRetractArea = resetRetractusRetractArea;
            Retract.usIndex = resetRetractusIndex;
            lpItemPositon.lpRetractAreaDim.Add(Retract);

            System.Int32 hRet = imp.WFS_CMD_CDM_RESET_AsyncImpl(lpItemPositon);

            string s = "WFS_CMD_CDM_RESET_Async: ";
            s += "\r\n\t fwOutputPosition(" + resetfwOutputPosition.ToString() + ")";
            s += "\r\n\t usNumber(" + resetusNumber.ToString() + ")";
            s += "\r\n\t Retract.fwOutputPosition(" + resetRetractfwOutputPosition.ToString() + ")";
            s += "\r\n\t Retract.usRetractArea(" + resetRetractusRetractArea.ToString() + ")";
            s += "\r\n\t Retract.usIndex(" + resetRetractusIndex.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        #endregion

        /*
        #region Events
        public int CDMEvent(long eventId, XfsGlobalDefine.WFSRESULT WFSResult)
        {
            this.eventId = eventId;

            string s = "Event (" + eventId + "): ";
            switch (eventId)
            {
                case GDef.WFS_EXECUTE_EVENT:
                    s += "WFS_EXECUTE_EVENT\r\n";
                    ManagerExeEvent(WFSResult);

                    break;

                case GDef.WFS_SERVICE_EVENT:
                    s += "WFS_SERVICE_EVENT\r\n";
                    ManagerSvrEvent(WFSResult);

                    break;

                case GDef.WFS_USER_EVENT:
                    s += "WFS_USER_EVENT\r\n";
                    ManagerUserEvent(WFSResult);

                    break;

                case GDef.WFS_SYSTEM_EVENT:
                    s += "WFS_SYSTEM_EVENT\r\n";
                    ManagerSysEvent(WFSResult);

                    break;

                case GDef.WFS_TIMER_EVENT:
                    s += "WFS_TIMER_EVENT\r\n";

                    break;

                case GDef.WFS_OPEN_COMPLETE:
                    s += "WFS_OPEN_COMPLETE\r\n";

                    break;

                case GDef.WFS_CLOSE_COMPLETE:
                    s += "WFS_CLOSE_COMPLETE\r\n";

                    break;

                case GDef.WFS_LOCK_COMPLETE:
                    s += "WFS_LOCK_COMPLETE\r\n";

                    break;

                case GDef.WFS_UNLOCK_COMPLETE:
                    s += "WFS_UNLOCK_COMPLETE\r\n";

                    break;

                case GDef.WFS_EXECUTE_COMPLETE:
                    s += "WFS_EXECUTE_COMPLETE\r\n";
                    ManagerExeComplete(WFSResult);

                    break;
            }

            Trace(s);
            return 0;
        }

        public void ManagerExeEvent(XfsGlobalDefine.WFSRESULT WFSResult)
        {
            string s = "";
            switch (WFSResult.dwCmdCodeOrEventID)
            {
                case CDM.WFS_EXEE_CDM_CASHUNITERROR:
                    s += "Event:WFS_EXEE_CDM_CASHUNITERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    CDM.WFSCDMCUERROR cuerror = (CDM.WFSCDMCUERROR)Marshal.PtrToStructure(WFSResult.lpBuffer, typeof(CDM.WFSCDMCUERROR));
                    CDM.WFSCDMCASHUNIT lpCashUnit = (CDM.WFSCDMCASHUNIT)Marshal.PtrToStructure(cuerror.lpCashUnit, typeof(CDM.WFSCDMCASHUNIT));
                    switch (cuerror.wFailure)
                    {
                        case CDM.WFS_CDM_CASHUNITEMPTY:
                            s += "\twFailure:WFS_CDM_CASHUNITEMPTY(" + cuerror.wFailure + ")\r\n";

                            break;

                        case CDM.WFS_CDM_CASHUNITERROR:
                            s += "\twFailure:WFS_CDM_CASHUNITERROR(" + cuerror.wFailure + ")\r\n";

                            break;

                        case CDM.WFS_CDM_CASHUNITFULL:
                            s += "\twFailure:WFS_CDM_CASHUNITFULL(" + cuerror.wFailure + ")\r\n";

                            break;

                        case CDM.WFS_CDM_CASHUNITLOCKED:
                            // m_CdmMgr.UpdateCashUnit(lpCashUnit);
                            s += "\twFailure:WFS_CDM_CASHUNITLOCKED(" + cuerror.wFailure + ")\r\n";

                            break;

                        case CDM.WFS_CDM_CASHUNITINVALID:
                            s += "\twFailure:WFS_CDM_CASHUNITINVALID(" + cuerror.wFailure + ")\r\n";

                            break;

                        case CDM.WFS_CDM_CASHUNITCONFIG:
                            s += "\twFailure:WFS_CDM_CASHUNITCONFIG(" + cuerror.wFailure + ")\r\n";

                            break;

                        default:

                            break;
                    }
                    
                    break;

                case CDM.WFS_EXEE_CDM_DELAYEDDISPENSE:
                    s += "Event:WFS_EXEE_CDM_DELAYEDDISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_EXEE_CDM_STARTDISPENSE:
                    s += "Event:WFS_EXEE_CDM_STARTDISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_EXEE_CDM_PARTIALDISPENSE:
                    s += "Event:WFS_EXEE_CDM_PARTIALDISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_EXEE_CDM_SUBDISPENSEOK:
                    s += "Event:WFS_EXEE_CDM_SUBDISPENSEOK(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_EXEE_CDM_INCOMPLETEDISPENSE:
                    s += "Event:WFS_EXEE_CDM_INCOMPLETEDISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_EXEE_CDM_NOTEERROR:
                    s += "Event:WFS_EXEE_CDM_NOTEERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_EXEE_CDM_MEDIADETECTED:
                    s += "Event:WFS_EXEE_CDM_MEDIADETECTED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

            }

            Trace(s);
        }

        public void ManagerSvrEvent(XfsGlobalDefine.WFSRESULT WFSResult)
        {
            string s = "";

            switch (Xfs30Definitions.CDM.CDMMessagesConvert(WFSResult.dwCmdCodeOrEventID))
            {
                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_SRVE_CDM_SAFEDOOROPEN:
                    s += "Event:WFS_SRVE_CDM_SAFEDOOROPEN(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_SRVE_CDM_SAFEDOORCLOSED:
                    s += "Event:WFS_SRVE_CDM_SAFEDOORCLOSED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_USRE_CDM_CASHUNITTHRESHOLD:
                    s += "Event:WFS_USRE_CDM_CASHUNITTHRESHOLD(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_SRVE_CDM_CASHUNITINFOCHANGED:
                    s += "Event:WFS_SRVE_CDM_CASHUNITINFOCHANGED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_SRVE_CDM_TELLERINFOCHANGED:
                    s += "Event:WFS_SRVE_CDM_TELLERINFOCHANGED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_EXEE_CDM_DELAYEDDISPENSE:
                    s += "Event:WFS_EXEE_CDM_DELAYEDDISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_EXEE_CDM_STARTDISPENSE:
                    s += "Event:WFS_EXEE_CDM_STARTDISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_EXEE_CDM_CASHUNITERROR:
                    s += "Event:WFS_EXEE_CDM_CASHUNITERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_SRVE_CDM_ITEMSTAKEN:
                    s += "Event:WFS_SRVE_CDM_ITEMSTAKEN(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    this.GlobalWFSResult = WFSResult;
                    ev.Set();

                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_EXEE_CDM_PARTIALDISPENSE:
                    s += "Event:WFS_EXEE_CDM_PARTIALDISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_EXEE_CDM_SUBDISPENSEOK:
                    s += "Event:WFS_EXEE_CDM_SUBDISPENSEOK(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_SRVE_CDM_ITEMSPRESENTED:
                    s += "Event:WFS_SRVE_CDM_ITEMSPRESENTED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_SRVE_CDM_COUNTS_CHANGED:
                    s += "Event:WFS_SRVE_CDM_COUNTS_CHANGED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_EXEE_CDM_INCOMPLETEDISPENSE:
                    s += "Event:WFS_EXEE_CDM_INCOMPLETEDISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_EXEE_CDM_NOTEERROR:
                    s += "Event:WFS_EXEE_CDM_NOTEERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                case KTR10_CDM.Xfs30Definitions.CDM.CDMMessages.WFS_EXEE_CDM_MEDIADETECTED:
                    s += "Event:WFS_SRVE_CDM_MEDIADETECTED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;

                default:
                    break;
            }

            Trace(s);
        }

        public void ManagerUserEvent(XfsGlobalDefine.WFSRESULT WFSResult)
        {
            string s = "";
            switch (WFSResult.dwCmdCodeOrEventID)
            {
                case CDM.WFS_USRE_CDM_CASHUNITTHRESHOLD:
                    s += "Event:WFS_USRE_CDM_CASHUNITTHRESHOLD(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    CDM.WFSCDMCASHUNIT lpCashUnit = (CDM.WFSCDMCASHUNIT)Marshal.PtrToStructure(WFSResult.lpBuffer, typeof(CDM.WFSCDMCASHUNIT));

                    break;

            }
            Trace(s);
        }

        public void ManagerSysEvent(XfsGlobalDefine.WFSRESULT WFSResult)
        {
            string s = "";
            switch (WFSResult.dwCmdCodeOrEventID)
            {
                case XfsApiPInvoke.WFS_SYSE_DEVICE_STATUS:
                    s += "Event:WFS_SYSE_DEVICE_STATUS(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    XfsApiPInvoke.WFSDEVSTATUS lpWFSDevstatus = (XfsApiPInvoke.WFSDEVSTATUS)Marshal.PtrToStructure(WFSResult.lpBuffer, typeof(XfsApiPInvoke.WFSDEVSTATUS));
                    s += "\tlpszPhysicalName : " + lpWFSDevstatus.lpszPhysicalName + "\r\n";
                    s += "\tlpszWorkstationName:" + lpWFSDevstatus.lpszWorkstationName + "\r\n";
                    s += "\tdwState:" + lpWFSDevstatus.dwState + "\r\n";

                    break;

                case XfsApiPInvoke.WFS_SYSE_HARDWARE_ERROR:
                    s += "Event:WFS_SYSE_HARDWARE_ERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    XfsApiPInvoke.WFSHWERROR lpWFSHardwareError = (XfsApiPInvoke.WFSHWERROR)Marshal.PtrToStructure(WFSResult.lpBuffer, typeof(XfsApiPInvoke.WFSHWERROR));
                    s += "\tlpszLogicalName : " + lpWFSHardwareError.lpszLogicalName + "\r\n";
                    s += "\tlpszPhysicalName : " + lpWFSHardwareError.lpszPhysicalName + "\r\n";
                    s += "\tlpszWorkstationName : " + lpWFSHardwareError.lpszWorkstationName + "\r\n";
                    s += "\tlpszAppID : " + lpWFSHardwareError.lpszAppID + "\r\n";
                    s += "\tdwAction : " + lpWFSHardwareError.dwAction + "\r\n";
                    s += "\tdwSize : " + lpWFSHardwareError.dwSize + "\r\n";
                    s += "\tlpbDescription:";
                    for (int i = 0; i < lpWFSHardwareError.dwSize; i++)
                    {
                        s += Convert.ToChar(lpWFSHardwareError.lpbDescription);
                    }
                    s += "\r\n";

                    break;

                case XfsApiPInvoke.WFS_SYSE_APP_DISCONNECT:
                    s += "Event:WFS_SYSE_APP_DISCONNECT(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case XfsApiPInvoke.WFS_SYSE_SOFTWARE_ERROR:
                    s += "Event:WFS_SYSE_SOFTWARE_ERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case XfsApiPInvoke.WFS_SYSE_USER_ERROR:
                    s += "Event:WFS_SYSE_USER_ERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case XfsApiPInvoke.WFS_SYSE_UNDELIVERABLE_MSG:
                    s += "Event:WFS_SYSE_UNDELIVERABLE_MSG(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case XfsApiPInvoke.WFS_SYSE_VERSION_ERROR:
                    s += "Event:WFS_SYSE_VERSION_ERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case XfsApiPInvoke.WFS_SYSE_LOCK_REQUESTED:
                    s += "Event:WFS_SYSE_LOCK_REQUESTED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

            }
            Trace(s);
        }

        public void ManagerExeComplete(XfsGlobalDefine.WFSRESULT WFSResult)
        {
            string s = "";
            switch (WFSResult.dwCmdCodeOrEventID)
            {
                case CDM.WFS_CMD_CDM_SET_CASH_UNIT_INFO:
                    s += "Event:WFS_CMD_CDM_SET_CASH_UNIT_INFO(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_START_EXCHANGE:
                    s += "Event:WFS_CMD_CDM_START_EXCHANGE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_END_EXCHANGE:
                    s += "Event:WFS_CMD_CDM_END_EXCHANGE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_DENOMINATE:
                    s += "Event:WFS_CMD_CDM_DENOMINATE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_DISPENSE:
                    s += "Event:WFS_CMD_CDM_DISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_REJECT:
                    s += "Event:WFS_CMD_CDM_REJECT(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_PRESENT:
                    s += "Event:WFS_CMD_CDM_PRESENT(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_RETRACT:
                    s += "Event:WFS_CMD_CDM_RETRACT(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_OPEN_SHUTTER:
                    s += "Event:WFS_CMD_CDM_OPEN_SHUTTER(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_CLOSE_SHUTTER:
                    s += "Event:WFS_CMD_CDM_CLOSE_SHUTTER(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_COUNT_EX:
                    s += "Event:WFS_CMD_CDM_COUNT_EX(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;

                case CDM.WFS_CMD_CDM_RESET:
                    s += "Event:WFS_CMD_CDM_RESET(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;
            }
            Trace(s);
        }

        #endregion
        */

        #region Property
        private ushort cassetteCount = 6;
        public ushort CassetteCount
        {
            get { return cassetteCount; }
        }

        private ushort mixNumber = 2;
        public ushort MixNumber
        {
            get { return mixNumber; }
            set { mixNumber = value; }
        }

        private ushort tellerId = 0;
        public ushort TellerId
        {
            get { return tellerId; }
            set { tellerId = value; }
        }

        private bool autoDenom = true;
        public bool AutoDenom
        {
            get { return autoDenom; }
            set { autoDenom = value; }
        }

        private bool autoPresent = false;
        public bool AutoPresent
        {
            get { return autoPresent; }
            set { autoPresent = value; }
        }

        private uint ulCashBox = 0;
        public uint UlCashBox
        {
            get { return ulCashBox; }
            set { ulCashBox = value; }
        }

        private ushort dispensefwPosition = CDMDefinitions.WFS_CDM_POSNULL;
        public ushort DispensefwPosition
        {
            get { return dispensefwPosition; }
            set { dispensefwPosition = value; }
        }

        private ushort presentfwPosition = CDMDefinitions.WFS_CDM_POSNULL;
        public ushort PresentfwPosition
        {
            get { return presentfwPosition; }
            set { presentfwPosition = value; }
        }

        #region retract
        private ushort retractfwOutputPosition = CDMDefinitions.WFS_CDM_POSNULL;
        public ushort RetractfwOutputPosition
        {
            get { return retractfwOutputPosition; }
            set { retractfwOutputPosition = value; }
        }

        private ushort retractusRetractArea = CDMDefinitions.WFS_CDM_RA_RETRACT;
        public ushort RetractusRetractArea
        {
            get { return retractusRetractArea; }
            set { retractusRetractArea = value; }
        }

        private ushort retractusIndex = 0;
        public ushort RetractusIndex
        {
            get { return retractusIndex; }
            set { retractusIndex = value; }
        }

        #endregion

        #region reset
        private ushort resetfwOutputPosition = CDMDefinitions.WFS_CDM_POSNULL;
        public ushort ResetfwOutputPosition
        {
            get { return resetfwOutputPosition; }
            set { resetfwOutputPosition = value; }
        }

        private ushort resetusNumber = 1;
        public ushort ResetusNumber
        {
            get { return resetusNumber; }
            set { resetusNumber = value; }
        }

        private ushort resetRetractfwOutputPosition = CDMDefinitions.WFS_CDM_POSNULL;
        public ushort ResetRetractfwOutputPosition
        {
            get { return resetRetractfwOutputPosition; }
            set { resetRetractfwOutputPosition = value; }
        }

        private ushort resetRetractusRetractArea = CDMDefinitions.WFS_CDM_RA_RETRACT;
        public ushort ResetRetractusRetractArea
        {
            get { return resetRetractusRetractArea; }
            set { resetRetractusRetractArea = value; }
        }

        private ushort resetRetractusIndex = 0;
        public ushort ResetRetractusIndex
        {
            get { return resetRetractusIndex; }
            set { resetRetractusIndex = value; }
        }

        #endregion

        private ushort shutterfwPosition = CDMDefinitions.WFS_CDM_POSNULL;
        public ushort ShutterfwPosition
        {
            get { return shutterfwPosition; }
            set { shutterfwPosition = value; }
        }

        #endregion

        #region Helpers
        private void Trace(string strInfo)
        {
            Utility.DebugMe(strInfo);
        }

        private void DebugCashUnitInfo(KT.WOSA.CDM.XfsCdmDefine.WFSCDMCUINFO_Dim lpCashUnitInfo)
        {
            #region Debug
            try
            {
                string s = "\r\n";
                s += "[";
                s += "\ttellerID(" + lpCashUnitInfo.usTellerID.ToString() + ")-";
                s += "count(" + lpCashUnitInfo.usCount.ToString() + ")";
                s += "\r\n";
                foreach (KT.WOSA.CDM.XfsCdmDefine.WFSCDMCASHUNIT_Dim LogicCu in lpCashUnitInfo.lppListDim)
                {
                    s += "\t";
                    s += "no(" + LogicCu.usNumber + ")";
                    s += "\r\n";
                    s += "\t\t";
                    s += "type(" + KTR10_CDM.Xfs30Definitions.CDM.CDMCashUnitTypeConvert(LogicCu.usType) + "(" + LogicCu.usType.ToString() + ")-";
                    s += "cashUnitName(" + LogicCu.lpszCashUnitName + ")-";

                    s += "unitID(";
                    foreach (var v in LogicCu.cUnitID)
                    {
                        if ('\0' != Convert.ToChar(v))
                            s += Convert.ToChar(v);
                    }

                    s += ")-";

                    String strTmp = "";
                    foreach (var v in LogicCu.cCurrencyID)
                    {
                        strTmp += Convert.ToChar(v);
                    }
                    s += "currency(" + strTmp + ")-";
                    s += "values(" + LogicCu.ulValues + ")-";

                    s += "initCount(" + LogicCu.ulInitialCount + ")-";
                    s += "count(" + LogicCu.ulCount + ")-";
                    s += "rejCount(" + LogicCu.ulRejectCount + ")-";
                    s += "min(" + LogicCu.ulMinimum + ")-";
                    s += "max(" + LogicCu.ulMaximum + ")-";
                    s += "appLock(" + (LogicCu.bAppLock == 1 ? "true" : "false") + ")-";
                    s += "status(" + KTR10_CDM.Xfs30Definitions.CDM.CDMCashUnitStatusConvert(LogicCu.usStatus) + "(" + LogicCu.usStatus.ToString() + ")" + ")";
                    s += "\r\n";
                    s += "\t\t";

                    s += "numPhysicalCUs(" + LogicCu.usNumPhysicalCUs + ")-";
                    s += "lppPhysical(" + LogicCu.lppPhysical.ToString("X") + ")";
                    foreach (KT.WOSA.CDM.XfsCdmDefine.WFSCDMPHCU phCu in LogicCu.lppPhysicalListDim)
                    {
                        s += "\r\n";
                        s += "\t\t\t";
                        s += "phyPosName(" + phCu.lpPhysicalPositionName + ")-";
                        strTmp = "";
                        foreach (SByte v in phCu.cUnitID)
                        {
                            if ('\0' != Convert.ToChar(v))
                                strTmp += Convert.ToChar(v);
                        }
                        s += "unitID(" + strTmp + ")-";
                        s += "initCount(" + phCu.ulInitialCount + ")-";
                        s += "count(" + phCu.ulCount + ")-";
                        s += "rejCount(" + phCu.ulRejectCount + ")-";
                        s += "max(" + phCu.ulMaximum + ")-";
                        s += "pStatus(" + KTR10_CDM.Xfs30Definitions.CDM.CDMCashUnitStatusConvert(phCu.usPStatus) + "(" + phCu.usPStatus.ToString() + ")" + ")-";
                        s += "hardwareSensor(" + phCu.bHardwareSensor + ")";
                    }
                    s += "\r\n";
                }

                s += "]";
                Trace(s);

            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }

            #endregion
        }

        private CDMDefinitions.WFSCDMDENOMINATION_Dim FillWFSCDMDENOMINATION_Dim(uint amount, string currency)
        {
            CDMDefinitions.WFSCDMDENOMINATION_Dim returnObj = new CDMDefinitions.WFSCDMDENOMINATION_Dim();

            #region currency set
            returnObj.cCurrencyID = new Byte[3];

            //1st way
            Buffer.BlockCopy(new System.Text.ASCIIEncoding().GetBytes(currency), 0, returnObj.cCurrencyID, 0, 3);

            //2nd way
            //returnObj.cCurrencyID[0] = Convert.ToByte(currency.ToCharArray()[0]);
            //returnObj.cCurrencyID[1] = Convert.ToByte(currency.ToCharArray()[1]);
            //returnObj.cCurrencyID[2] = Convert.ToByte(currency.ToCharArray()[2]);

            #endregion

            returnObj.ulAmount = amount;
            returnObj.ulCashBox = UlCashBox;

            returnObj.lpulValuesDim = new List<System.UInt32>();
            if (AutoDenom)
            {
                returnObj.usCount = 0;
            }
            else
            {
                returnObj.usCount = CassetteCount;
                returnObj.lpulValuesDim.Add(0);//retract
                returnObj.lpulValuesDim.Add(0);//reject
                returnObj.lpulValuesDim.Add(0);//cassette1 : 100
                returnObj.lpulValuesDim.Add(0);//cassette2 : 50
                returnObj.lpulValuesDim.Add(0);//cassette3 : 20
                returnObj.lpulValuesDim.Add(0);//cassette4 : 10
            }

            return returnObj;
        }

        #endregion
    }
}