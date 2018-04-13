using System;
using System.Collections.Generic;
using KT.WOSA.CIM;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KTR10_CIM
{
    using CIM = XfsCimDefine;
    using GDef = KT.WOSA.CIM.XfsGlobalDefine;
    using KT.WOSA.CIM.IMP;

    public class Xfs30Api : IXfs3
    {
        public XfsCimImp imp = new XfsCimImp();
        
        public Xfs30Api()
        {
            Utility.DebugMe("KTR10_CIM.Xfs30Api Constructed!");
        }

        #region Basic Commands
        public void OpenRegister()
        {
            string strLogicalName = "CashAcceptor";
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

            KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION WFSVersion;
            KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION SrvcVersion;
            KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION SPIVersion;
            GetSpInfoVersion(out WFSVersion, out SrvcVersion, out SPIVersion);
        }

        public void GetSpInfoVersion(out KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION WFSVersion, out KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION SrvcVersion, out KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION SPIVersion)
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

        private void CheckIfNoRegisterSoRegisterOneMoreTime()
        {
            XfsCimDefine.WFSCIMSTATUS_dim lpDataOut;

            int hRet = imp.WFS_INF_CIM_STATUS_Impl(out lpDataOut);

            String s = "<Check> WFS_INF_CIM_STATUS_Impl: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
            {
                OpenRegister();
            }
        }

        private int CheckIfNoRegisterSoRegisterOneMoreTime(out XfsCimDefine.WFSCIMSTATUS_dim lpDataOut)
        {
            int hRet = imp.WFS_INF_CIM_STATUS_Impl(out lpDataOut);

            String s = "<Check> WFS_INF_CIM_STATUS_Impl: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
            {
                OpenRegister();
            }

            return hRet;
        }

        #region Info Commands
        /// <summary>
        /// This command is used to obtain the status of the CIM. It may also return vendor-specific status information
        /// </summary>
        public XfsCimDefine.WFSCIMSTATUS_dim WFS_INF_CIM_STATUS()
        {
            XfsCimDefine.WFSCIMSTATUS_dim lpDataOut;
            int hRet = CheckIfNoRegisterSoRegisterOneMoreTime(out lpDataOut);

            string s = "";

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
            {
                hRet = imp.WFS_INF_CIM_STATUS_Impl(out lpDataOut);

                s = "WFS_INF_CIM_STATUS: ";
                s += "return :" + hRet;
            }

            #region Debug
            try
            {
                string fwDevice = lpDataOut.fwDevice.ToString().PadLeft(4, '0');
                string fwSafeDoor = lpDataOut.fwSafeDoor.ToString().PadLeft(4, '0');
                string fwAcceptor = lpDataOut.fwAcceptor.ToString().PadLeft(4, '0');
                string fwIntermediateStacker = lpDataOut.fwIntermediateStacker.ToString().PadLeft(4, '0');

                s += "\r\n";
                s += "\t";
                s += "fwDevice(" + KTR10_CIM.Xfs30Definitions.CIM.CIMStatusDeviceConvert(fwDevice) + "(" + fwDevice + "))-";
                s += "fwSafeDoor(" + KTR10_CIM.Xfs30Definitions.CIM.CIMSTATUSSafeDoorConvert(fwSafeDoor) + "(" + fwSafeDoor + "))-";
                s += "fwAcceptor(" + KTR10_CIM.Xfs30Definitions.CIM.CIMSTATUSAcceptorConvert(fwAcceptor) + "(" + fwAcceptor + "))-";
                s += "fwIntermediateStacker(" + KTR10_CIM.Xfs30Definitions.CIM.CIMSTATUSIntermediateStackerConvert(fwIntermediateStacker) + "(" + fwIntermediateStacker + "))-";
                s += "fwStackerItems(" + KTR10_CIM.Xfs30Definitions.CIM.CIMSTATUSStackerItemsConvert(lpDataOut.fwStackerItems) + "(" + fwAcceptor + "))-";
                s += "fwBanknoteReader(" + KTR10_CIM.Xfs30Definitions.CIM.CIMSTATUSBankNoteReaderConvert(lpDataOut.fwBanknoteReader) + "(" + fwAcceptor + "))-";
                s += "bDropBox(" + lpDataOut.bDropBox.ToString() + "))";

                s += "\r\n";
                s += "\t";
                s += "lppPositions";
                foreach (CIM.WFSCIMINPOS pos in lpDataOut.lppPositionsDim)
                {
                    s += "\r\n";
                    s += "\t\t";
                    s += "fwPstn(" + pos.fwPosition +
                        ")-fwPstnStts(" + KTR10_CIM.Xfs30Definitions.CIM.CIMINPOSPositionStatusConvert(pos.fwPositionStatus) + "(" + pos.fwPositionStatus + "))-" +
                        ")-fwShttr(" + KTR10_CIM.Xfs30Definitions.CIM.CIMSTATUSShutterConvert(pos.fwShutter) + "(" + pos.fwShutter + "))-" +
                        ")-fwTrnsprt(" + KTR10_CIM.Xfs30Definitions.CIM.CIMSTATUSTransportConvert(pos.fwTransport) + "(" + pos.fwTransport + "))-" +
                        ")-fwTrnsprtStts(" + KTR10_CIM.Xfs30Definitions.CIM.CIMINPOSTransportStatusConvert(pos.fwTransportStatus) + "(" + pos.fwTransportStatus + "))";
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
                throw new Exceptions.StatusException(lpDataOut, hRet);

            return lpDataOut;
        }

        /// <summary>
        /// This command is used to retrieve the capabilities of the cash acceptor.
        /// </summary>
        /// <returns></returns>
        public XfsCimDefine.WFSCIMCAPS WFS_INF_CIM_CAPABILITIES()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            XfsCimDefine.WFSCIMCAPS lpCaps;

            int hRet = imp.WFS_INF_CIM_CAPABILITIES_Impl(out lpCaps);

            string s = "WFS_INF_CIM_CAPABILITIES: ";
            s += "return :" + hRet;

            #region Debug
            try
            {
                if (XfsGlobalDefine.WFS_SUCCESS == hRet)
                {
                    s += "\r\n";
                    s += "\twClass(" + lpCaps.wClass + ")";
                    s += "fwType(" + KTR10_CIM.Xfs30Definitions.CIM.CIMCAPSTypeConvert(lpCaps.fwType) + "(" + lpCaps.fwType.ToString() + ")-";
                    s += "wMaxCashInItems(" + lpCaps.wMaxCashInItems + ")-";
                    s += "bCompound(" + (lpCaps.bCompound == 1 ? "true" : "false") + ")-";
                    s += "bShutter(" + (lpCaps.bShutter == 1 ? "true" : "false") + ")-";
                    s += "bShutterControl(" + lpCaps.bShutterControl + ")-";
                    s += "\r\n";

                    s += "\tbSafeDoor(" + (lpCaps.bSafeDoor == 1 ? "true" : "false") + ")-"; ;
                    s += "bCashBox(" + (lpCaps.bCashBox == 1 ? "true" : "false") + ")-";
                    s += "bRefill(" + lpCaps.bRefill + ")-";
                    s += "fwIntermediateStacker(" + (lpCaps.fwIntermediateStacker == 1 ? "true" : "false") + ")-";
                    s += "bItemsTakenSensor(" + (lpCaps.bItemsTakenSensor == 1 ? "true" : "false") + ")-";
                    s += "bItemsInsertedSensor(" + (lpCaps.bItemsInsertedSensor == 1 ? "true" : "false") + ")";
                    s += "\r\n";

                    s += "\tfwPositions(" + lpCaps.fwPositions + ")-";
                    s += "fwExchangeType(" + lpCaps.fwExchangeType + ")-";
                    s += "fwRetractAreas(" + lpCaps.fwRetractAreas + ")-";
                    s += "fwRetractTransportActions(" + lpCaps.fwRetractTransportActions + ")-";
                    s += "fwRetractStackerActions(" + lpCaps.fwRetractStackerActions + ")-";

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
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.CapabilityException(lpCaps, hRet);

            return lpCaps;
        }

        /// <summary>
        /// This command is used to obtain information about the status and contents of the cash in units and recycle units in the CIM. 
        /// </summary>
        /// <returns></returns>
        public XfsCimDefine.WFSCIMCASHINFO_dim WFS_INF_CIM_CASH_UNIT_INFO()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            XfsCimDefine.WFSCIMCASHINFO_dim lpCashInfo;

            int hRet = imp.WFS_INF_CIM_CASH_UNIT_INFO_Impl(out lpCashInfo);

            String s = "WFS_INF_CIM_CASH_UNIT_INFO_Impl: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet == XfsGlobalDefine.WFS_SUCCESS)
            {
                DebugCashUnitInfo(lpCashInfo);
            }

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.CashUnitInfoException(lpCashInfo, hRet);

            return lpCashInfo;
        }
        
        /// <summary>
        /// This command returns each exponent assigned to each currency known to the service provider
        /// </summary>
        /// <returns></returns>
        public List<XfsCimDefine.WFSCIMCURRENCYEXP> WFS_INF_CIM_CURRENCY_EXP()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            List<XfsCimDefine.WFSCIMCURRENCYEXP> CurrencyExpList;

            int hRet = imp.WFS_INF_CIM_CURRENCY_EXP_Impl(out CurrencyExpList);
            string s = "WFS_INF_CIM_CURRENCY_EXP: ";
            s += "return :" + hRet;

            #region Debug
            try
            {
                String strTmp = "";
                if (hRet == XfsGlobalDefine.WFS_SUCCESS)
                {
                    foreach (XfsCimDefine.WFSCIMCURRENCYEXP CurrencyExp in CurrencyExpList)
                    {
                        foreach (SByte v in CurrencyExp.cCurrencyID)
                        {
                            strTmp += Convert.ToChar(v);

                        }

                        Console.WriteLine("cCurrencyID\t= {0}", strTmp);
                        s += "\tcCurrencyID\t= " + strTmp + "\r\n";
                        Console.WriteLine("sExponent\t= {0}", CurrencyExp.sExponent);
                        s += "\tsExponent\t= " + CurrencyExp.sExponent + "\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.CurrencyExponentException(CurrencyExpList, hRet);

            return CurrencyExpList;
        }

        /// <summary>
        /// This command is used to obtain information about the banknote types that can be detected by the banknote reader
        /// </summary>
        /// <returns></returns>
        public XfsCimDefine.WFSCIMNOTETYPELIST_dim WFS_INF_CIM_BANKNOTE_TYPES()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            XfsCimDefine.WFSCIMNOTETYPELIST_dim lpCaps;

            int hRet = imp.WFS_INF_CIM_BANKNOTE_TYPES_Impl(out lpCaps);

            string s = "WFS_INF_CIM_BANKNOTE_TYPES: ";
            s += "return :" + hRet;

            #region Debug
            try
            {
                if (XfsGlobalDefine.WFS_SUCCESS == hRet)
                {
                    s += "\r\n";
                    s += "\tusNumOfNoteTypes = " + lpCaps.usNumOfNoteTypes;
                    List<CIM.WFSCIMNOTETYPE> lppNoteTypesList = lpCaps.lppNoteTypesDim;

                    foreach (CIM.WFSCIMNOTETYPE lppNoteTypes in lppNoteTypesList)
                    {
                        s += "\r\n";
                        String strTmp = "";
                        s += "\t\tusNoteID(" + lppNoteTypes.usNoteID + ")-";

                        foreach (SByte v in lppNoteTypes.cCurrencyID)
                        {
                            if (Convert.ToChar(v) != '\0')
                                strTmp += Convert.ToChar(v);
                        }

                        s += "cCurrencyID(" + strTmp + ")-";

                        s += "ulValues(" + lppNoteTypes.ulValues + ")-";
                        s += "usRelease(" + lppNoteTypes.usRelease + ")-";
                        s += "bConfigured(" + lppNoteTypes.bConfigured + ")";
                    }
                }
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.BankNoteTypeException(lpCaps, hRet);

            return lpCaps;
        }

        /// <summary>
        /// This command is used to get information about the status of the last cash in transaction. This value is persistent and is valid until the next WFS_CMD_CIM_CASH_IN_START
        /// </summary>
        /// <returns></returns>
        public XfsCimDefine.WFSCIMCASHINSTATUS_dim WFS_INF_CIM_CASH_IN_STATUS()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            XfsCimDefine.WFSCIMCASHINSTATUS_dim lpCashInStatus;
            
            int hRet = imp.WFS_INF_CIM_CASH_IN_STATUS_Impl(out lpCashInStatus);

            string s = "WFS_INF_CIM_CASH_IN_STATUS: ";
            s += "return :" + hRet;

            #region Debug
            try
            {
                int _cntr = 0;
                if (hRet == XfsGlobalDefine.WFS_SUCCESS)
                {
                    s += "\r\n";
                    s += "\twStatus(" + KTR10_CIM.Xfs30Definitions.CIM.CIMCASHINSTATUSStatusConvert(lpCashInStatus.wStatus) + "(" + lpCashInStatus.wStatus.ToString() + ")-";
                    s += "usNumOfRefused(" + lpCashInStatus.usNumOfRefused + ")\r\n";
                    s += "\tlpNoteNumberList:\r\n";
                    s += "\t\tusNumOfNoteNumbers(" + lpCashInStatus.lpNoteNumberListDim.usNumOfNoteNumbers + ") - ";
                    s += "lppNoteNumber List";

                    _cntr = 0;
                    foreach (CIM.WFSCIMNOTENUMBER v in lpCashInStatus.lpNoteNumberListDim.lppNoteNumberDim)
                    {
                        _cntr++;
                        if (_cntr == 1)
                            s += " || ";
                        else
                            s += "\r\n\t\t\t";

                        s += "usNoteID(" + v.usNoteID + ")-";
                        s += "ulCount(" + v.ulCount + ")";
                    }

                    s += "\r\n";
                    s += "\tlpszExtra[";
                    if (IntPtr.Zero.Equals(lpCashInStatus.lpszExtra) == false)
                    {
                        s += "\r\n";
                        for (Int32 i = 0; true; i++)
                        {
                            System.Byte b1 = Marshal.ReadByte(lpCashInStatus.lpszExtra, i);
                            System.Byte b2 = Marshal.ReadByte(lpCashInStatus.lpszExtra, i + 1);
                            if (b1 == 0 && b2 == 0)
                                break;
                            if (b1 == 0)
                                s += "\r\n";
                            s += Convert.ToChar(b1);
                        }
                    }
                    s += "]";
                }
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            
            #endregion

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.CashInStatusException(lpCashInStatus, hRet);

            return lpCashInStatus;
        }

        /// <summary>
        /// This command is used to retrieve the information detected for the items processed during the last command that could move notes. 
        /// The data is non-cumulative and is only available until the next command that could move notes is executed 
        /// (including commands on the CDM interface on recycling devices) or a new cash-in transaction is started. 
        /// This command can be used both within and out with a cash-in transaction.
        /// </summary>
        public XfsCimDefine.WFSCIMITEMSINFO_dim WFS_INF_CIM_GET_ITEMS_INFO()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            XfsCimDefine.WFSCIMITEMSINFO_dim lpDataOut;

            int hRet = imp.WFS_INF_CIM_GET_ITEMS_INFO_Impl(out lpDataOut);

            string s = "WFS_INF_CIM_GET_ITEMS_INFO: ";
            s += "return :" + hRet;

                #region Debug
                try
                {
                    string usCount = lpDataOut.usCount.ToString();
                    s += "\r\n";
                    s += "\tusCount(" + usCount.ToString() + ")-";

                    s += "\r\n";

                    s += "\tItemsList[";
                    foreach (var pos in lpDataOut.lppOneItemListDim)
                    {
                        s += "\r\n";
                        s += "\tusLevel(" + pos.usLevel.ToString() +
                            ")-usNoteID(" + pos.usNoteID.ToString() +
                            ")-lpszSerialNumber(" + pos.strSerialNumber +
                            ")-lpszImageFileName(" + pos.strImageFileName + ")";
                    }
                    s += "]";
                }
                catch (Exception ex)
                {
                    Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
                }
                #endregion
                Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GetItemsInfoException(lpDataOut, hRet);

            return lpDataOut;
        }

        #endregion

        #region Execute Commands
        /// <summary>
        /// Before initiating a Cash-In operation, an application must issue the WFS_CMD_CIM_CASH_IN_START command to begin a Cash-In Transaction. During a Cash-In Transaction any number of WFS_CMD_CIM_CASH_IN commands may be issued. The transaction is ended when either a WFS_CMD_CIM_ROLLBACK or WFS_CMD_CIM_CASH_IN_END command is sent. 
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_CASH_IN_START()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            CIM.WFSCIMCASHINSTART CashInStart = new CIM.WFSCIMCASHINSTART();
            CashInStart.usTellerID = tellerId;
            CashInStart.bUseRecycleUnits = cashInbUseRecycleUnits;
            CashInStart.fwOutputPosition = cashInfwOutputPosition;
            CashInStart.fwInputPosition = cashInfwInputPosition;

            int hRet = imp.WFS_CMD_CIM_CASH_IN_START_Impl(CashInStart);

            string s = "WFS_CMD_CIM_CASH_IN_START: ";
            s += "\r\n\t usTellerID(" + tellerId.ToString() + ")";
            s += "\r\n\t bUseRecycleUnits(" + cashInbUseRecycleUnits.ToString() + ")";
            s += "\r\n\t fwOutputPosition(" + cashInfwOutputPosition.ToString() + ")";
            s += "\r\n\t fwInputPosition(" + cashInfwInputPosition.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// This command moves items into the CIM from an input position.  
        /// <para>The items may pass through the banknote reader for identification. Failure to identify items does not mean that the command has failed - even if some or all of the items are rejected by the banknote reader, the command may return WFS_SUCCESS. In this case a WFS_EXEE_CIM_INPUTREFUSE event will be sent to report the rejection.  </para>
        /// <para>If the device does not have a banknote reader then the output parameter will be NULL. </para>
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_CASH_IN()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_CASH_IN_Impl();

            string s = "WFS_CMD_CIM_CASH_IN: ";
            s += "return :" + hRet;

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// This command ends a Cash-In Transaction. If items are on the stacker as a result of a WFS_CMD_CIM_CASH_IN command, these items are moved into the cash-in cash units or the recycle units.
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_CASH_IN_END()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_CASH_IN_END_Impl();

            string s = "WFS_CMD_CIM_CASH_IN_END: ";
            s += "return :" + hRet;

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// A Cash-In operation has to be handled as a transaction that can be rolled back if a difference occurs between the amount counted by the CIM and the amount inserted. This command is used to roll back a Cash-In transaction. It causes all the notes cashed in since the last WFS_CMD_CIM_CASH_IN_START command to be returned to the customer.  
        /// <para>This command ends the current Cash-In Transaction. The Cash-In transaction is ended even if this command does not complete successfully. </para>
        /// </summary>
        /// <returns></returns>
        public CIM.WFSCIMCASHINFO_dim WFS_CMD_CIM_CASH_IN_ROLLBACK()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            CIM.WFSCIMCASHINFO_dim cashInfo;

            int hRet = imp.WFS_CMD_CIM_CASH_IN_ROLLBACK_Impl(out cashInfo);

            string s = "WFS_CMD_CIM_CASH_IN_ROLLBACK: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.CashInRollbackException(cashInfo, hRet);

            return cashInfo;
        }

        /// <summary>
        /// This command retracts items from an output position. Retracted items will be moved to either a retract bin, the transport or an intermediate stacker area. After the items are retracted the shutter is closed automatically. 
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_RETRACT()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            CIM.WFSCIMRETRACT retract = new CIM.WFSCIMRETRACT();
            retract.fwOutputPosition = retractfwOutputPosition;
            retract.usRetractArea = retractusRetractArea;
            retract.usIndex = retractusIndex;

            int hRet = imp.WFS_CMD_CIM_RETRACT_Impl(retract);

            string s = "WFS_CMD_CIM_RETRACT: ";
            s += "\r\n\t fwOutputPosition(" + retractfwOutputPosition.ToString() + ")";
            s += "\r\n\t usRetractArea(" + retractusRetractArea.ToString() + ")";
            s += "\r\n\t usIndex(" + retractusIndex.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// This command opens the shutter
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_OPEN_SHUTTER()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_OPEN_SHUTTER_Impl(shutterfwPosition);

            string s = "WFS_CMD_CIM_OPEN_SHUTTER: ";
            s += "\r\n\t shutterfwPosition(" + shutterfwPosition.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        /// <summary>
        /// This command closes the shutter. 
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_CLOSE_SHUTTER()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_CLOSE_SHUTTER_Impl(shutterfwPosition);

            string s = "WFS_CMD_CIM_CLOSE_SHUTTER: ";
            s += "\r\n\t shutterfwPosition(" + shutterfwPosition.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }
        
        /// <summary>
        /// This command is used to adjust information about the status and contents of the cash units present in the CIM.  
        /// <para>This command generates the service event WFS_SRVE_CIM_CASHUNITINFOCHANGED to inform applications that cash unit information has been changed</para>
        /// <para>This command can only be used to change software counters, thresholds and the application lock. All other fields in the input structure will be ignore</para>
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_SET_CASH_UNIT_INFO(CIM.WFSCIMCASHINFO_dim lpCashUnitInfo)
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_SET_CASH_UNIT_INFO_Impl(lpCashUnitInfo);

            string s = "WFS_CMD_CIM_SET_CASH_UNIT_INFO: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet == XfsGlobalDefine.WFS_SUCCESS)
            {
                DebugCashUnitInfo(lpCashUnitInfo);
            }

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }
                
        /// <summary>
        /// This command is used by the application to perform a hardware reset which will attempt to return the CIM device to a known good state. This command does not over-ride a lock obtained on another application or service handle nor can it be performed while the CIM is in the exchange state. This command does not end a cash in transaction, the CIM remains in the cash in state.  
        /// <para>Persistent values, such as counts and configuration information are not cleared by this command</para>
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_RESET(ushort fwOutputPosition, ushort usNumber, ushort retractfwOutputPosition, ushort retractusRetractArea, ushort retractusIndex)
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            CIM.WFSCIMITEMPOSITION_dim lpItemPos = new CIM.WFSCIMITEMPOSITION_dim();
            lpItemPos.fwOutputPosition = fwOutputPosition;
            lpItemPos.usNumber = usNumber;

            lpItemPos.lpRetractAreaDim.fwOutputPosition = retractfwOutputPosition;
            lpItemPos.lpRetractAreaDim.usRetractArea = retractusRetractArea;
            lpItemPos.lpRetractAreaDim.usIndex = retractusIndex;

            int hRet = imp.WFS_CMD_CIM_RESET_Impl(lpItemPos);

            string s = "WFS_CMD_CIM_RESET: ";
            s += "\r\n\t fwOutputPosition(" + fwOutputPosition.ToString() + ")";
            s += "\r\n\t usNumber(" + usNumber.ToString() + ")";
            s += "\r\n\t lpRetractAreaDim.fwOutputPosition(" + retractfwOutputPosition.ToString() + ")";
            s += "\r\n\t lpRetractAreaDim.usRetractArea(" + retractusRetractArea.ToString() + ")";
            s += "\r\n\t lpRetractAreaDim.usIndex(" + retractusIndex.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.GeneralException(hRet);
        }

        #endregion

        #region Execute Async Commands
        /// <summary>
        /// Before initiating a Cash-In operation, an application must issue the WFS_CMD_CIM_CASH_IN_START command to begin a Cash-In Transaction. During a Cash-In Transaction any number of WFS_CMD_CIM_CASH_IN commands may be issued. The transaction is ended when either a WFS_CMD_CIM_ROLLBACK or WFS_CMD_CIM_CASH_IN_END command is sent. 
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_CASH_IN_START_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            CIM.WFSCIMCASHINSTART CashInStart = new CIM.WFSCIMCASHINSTART();
            CashInStart.usTellerID = tellerId;
            CashInStart.bUseRecycleUnits = cashInbUseRecycleUnits;
            CashInStart.fwOutputPosition = cashInfwOutputPosition;
            CashInStart.fwInputPosition = cashInfwInputPosition;

            int hRet = imp.WFS_CMD_CIM_CASH_IN_START_AsyncImpl(CashInStart);

            string s = "WFS_CMD_CIM_CASH_IN_START_Async: ";
            s += "\r\n\t usTellerID(" + tellerId.ToString() + ")";
            s += "\r\n\t bUseRecycleUnits(" + cashInbUseRecycleUnits.ToString() + ")";
            s += "\r\n\t fwOutputPosition(" + cashInfwOutputPosition.ToString() + ")";
            s += "\r\n\t fwInputPosition(" + cashInfwInputPosition.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command moves items into the CIM from an input position.  
        /// <para>The items may pass through the banknote reader for identification. Failure to identify items does not mean that the command has failed - even if some or all of the items are rejected by the banknote reader, the command may return WFS_SUCCESS. In this case a WFS_EXEE_CIM_INPUTREFUSE event will be sent to report the rejection.  </para>
        /// <para>If the device does not have a banknote reader then the output parameter will be NULL. </para>
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_CASH_IN_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_CASH_IN_AsyncImpl();

            string s = "WFS_CMD_CIM_CASH_IN_Async: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command ends a Cash-In Transaction. If items are on the stacker as a result of a WFS_CMD_CIM_CASH_IN command, these items are moved into the cash-in cash units or the recycle units.
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_CASH_IN_END_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_CASH_IN_END_AsyncImpl();

            string s = "WFS_CMD_CIM_CASH_IN_END_Async: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// A Cash-In operation has to be handled as a transaction that can be rolled back if a difference occurs between the amount counted by the CIM and the amount inserted. This command is used to roll back a Cash-In transaction. It causes all the notes cashed in since the last WFS_CMD_CIM_CASH_IN_START command to be returned to the customer.  
        /// <para>This command ends the current Cash-In Transaction. The Cash-In transaction is ended even if this command does not complete successfully. </para>
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_CASH_IN_ROLLBACK_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_CASH_IN_ROLLBACK_AsyncImpl();

            string s = "WFS_CMD_CIM_CASH_IN_ROLLBACK_Async: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command retracts items from an output position. Retracted items will be moved to either a retract bin, the transport or an intermediate stacker area. After the items are retracted the shutter is closed automatically. 
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_RETRACT_Async()
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            CIM.WFSCIMRETRACT retract = new CIM.WFSCIMRETRACT();
            retract.fwOutputPosition = retractfwOutputPosition;
            retract.usRetractArea = retractusRetractArea;
            retract.usIndex = retractusIndex;

            int hRet = imp.WFS_CMD_CIM_RETRACT_AsyncImpl(retract);

            string s = "WFS_CMD_CIM_RETRACT_Async: ";
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
        /// This command opens the shutter
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_OPEN_SHUTTER_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_OPEN_SHUTTER_AsyncImpl(shutterfwPosition);

            string s = "WFS_CMD_CIM_OPEN_SHUTTER_Async: ";
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
        /// <returns></returns>
        public void WFS_CMD_CIM_CLOSE_SHUTTER_Async()
        {
            //CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_CLOSE_SHUTTER_AsyncImpl(shutterfwPosition);

            string s = "WFS_CMD_CIM_CLOSE_SHUTTER_Async: ";
            s += "\r\n\t shutterfwPosition(" + shutterfwPosition.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command is used to adjust information about the status and contents of the cash units present in the CIM.  
        /// <para>This command generates the service event WFS_SRVE_CIM_CASHUNITINFOCHANGED to inform applications that cash unit information has been changed</para>
        /// <para>This command can only be used to change software counters, thresholds and the application lock. All other fields in the input structure will be ignore</para>
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_SET_CASH_UNIT_INFO_Async(CIM.WFSCIMCASHINFO_dim lpCashUnitInfo)
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            int hRet = imp.WFS_CMD_CIM_SET_CASH_UNIT_INFO_AsyncImpl(lpCashUnitInfo);

            string s = "WFS_CMD_CIM_SET_CASH_UNIT_INFO_Async: ";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        /// <summary>
        /// This command is used by the application to perform a hardware reset which will attempt to return the CIM device to a known good state. This command does not over-ride a lock obtained on another application or service handle nor can it be performed while the CIM is in the exchange state. This command does not end a cash in transaction, the CIM remains in the cash in state.  
        /// <para>Persistent values, such as counts and configuration information are not cleared by this command</para>
        /// </summary>
        /// <returns></returns>
        public void WFS_CMD_CIM_RESET_Async(ushort fwOutputPosition, ushort usNumber, ushort retractfwOutputPosition, ushort retractusRetractArea, ushort retractusIndex)
        {
            CheckIfNoRegisterSoRegisterOneMoreTime();

            CIM.WFSCIMITEMPOSITION_dim lpItemPos = new CIM.WFSCIMITEMPOSITION_dim();
            lpItemPos.fwOutputPosition = fwOutputPosition;
            lpItemPos.usNumber = usNumber;

            lpItemPos.lpRetractAreaDim.fwOutputPosition = retractfwOutputPosition;
            lpItemPos.lpRetractAreaDim.usRetractArea = retractusRetractArea;
            lpItemPos.lpRetractAreaDim.usIndex = retractusIndex;

            int hRet = imp.WFS_CMD_CIM_RESET_AsyncImpl(lpItemPos);

            string s = "WFS_CMD_CIM_RESET_Async: ";
            s += "\r\n\t fwOutputPosition(" + fwOutputPosition.ToString() + ")";
            s += "\r\n\t usNumber(" + usNumber.ToString() + ")";
            s += "\r\n\t lpRetractAreaDim.fwOutputPosition(" + retractfwOutputPosition.ToString() + ")";
            s += "\r\n\t lpRetractAreaDim.usRetractArea(" + retractusRetractArea.ToString() + ")";
            s += "\r\n\t lpRetractAreaDim.usIndex(" + retractusIndex.ToString() + ")";
            s += "\r\n";
            s += "return :" + hRet;

            Trace(s);

            if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                throw new Exceptions.AsyncMethodCallException(hRet);
        }

        #endregion
        
        #region Property
        private ushort tellerId = 0;
        public ushort TellerId
        {
            get { return tellerId; }
            set { tellerId = value; }
        }

        public int MaxEscrowCapacity
        {
            get
            {
                XfsCimDefine.WFSCIMCAPS? _capability = null;
                
                try
                {
                    _capability = WFS_INF_CIM_CAPABILITIES();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("WFS_INF_CIM_CAPABILITIES : get problem {0}", ex.Message);
                }

                return _capability.HasValue ? (int)_capability.Value.wMaxCashInItems : 300;
            }
        }

        public int MaxItems
        {
            get { return 300; }
        }

        private int cashInbUseRecycleUnits = 1;
        public int CashInbUseRecycleUnits
        {
            get { return cashInbUseRecycleUnits; }
            set { cashInbUseRecycleUnits = value; }
        }

        private ushort cashInfwOutputPosition = CIM.WFS_CIM_POSNULL;
        public ushort CashInfwOutputPosition 
        {
            get { return cashInfwOutputPosition; }
            set { cashInfwOutputPosition = value; }
        }
        
        private ushort cashInfwInputPosition = CIM.WFS_CIM_POSNULL;
        public ushort CashInfwInputPosition
        {
            get { return cashInfwInputPosition; }
            set { cashInfwInputPosition = value; }
        }

        #region retract
        private ushort retractfwOutputPosition = CIM.WFS_CIM_POSNULL;
        public ushort RetractfwOutputPosition
        {
            get { return retractfwOutputPosition; }
            set { retractfwOutputPosition = value; }
        }

        private ushort retractusRetractArea = CIM.WFS_CIM_RA_RETRACT;
        public ushort RetractusRetractArea
        {
            get { return retractusRetractArea; }
            set { retractusRetractArea = value; }
        }

        private ushort retractusIndex = 1;
        public ushort RetractusIndex
        {
            get { return retractusIndex; }
            set { retractusIndex = value; }
        }

        #endregion
        
        private ushort shutterfwPosition = CIM.WFS_CIM_POSNULL;
        public ushort ShutterfwPosition
        {
            get { return shutterfwPosition; }
            set { shutterfwPosition = value; }
        }
        
        public struct BankNoteDetail
        {
            public int bConfigured;
            public string cCurrencyID;
            public uint ulValues;
            public ushort usNoteID;
            public ushort usRelease;
        }

        #endregion

        #region Helpers
        private void Trace(string strInfo)
        {
            Utility.DebugMe(strInfo);
        }

        private void DebugCashUnitInfo(KT.WOSA.CIM.XfsCimDefine.WFSCIMCASHINFO_dim lpCashInfo)
        {
            #region Debug
            try
            {
                int _cntr = 0;

                string s = "";
                s += "count(" + lpCashInfo.usCount.ToString() + ")";

                string strTmp = "";
                foreach (CIM.WFSCIMCASHIN_dim pos in lpCashInfo.lppCashInDim)
                {
                    s += "\r\n";
                    s += "\t";
                    s += "no(" + pos.usNumber + ")";
                    s += "\r\n";
                    s += "\t\t";
                    s += "type(" + KTR10_CIM.Xfs30Definitions.CIM.CIMCASHINTypeConvert(pos.fwType) + "(" + pos.fwType.ToString() + "))-";
                    s += "itemType(" + KTR10_CIM.Xfs30Definitions.CIM.CIMCASHINItemTypeConvert(pos.fwItemType) + "(" + pos.fwItemType.ToString() + "))-";

                    foreach (SByte v in pos.cUnitID)
                    {
                        if (Convert.ToChar(v) != '\0')
                        {
                            strTmp += Convert.ToChar(v);
                        }
                    }

                    s += "unitID(" + strTmp + ")-";
                    strTmp = "";

                    foreach (SByte v in pos.cCurrencyID)
                    {
                        if (Convert.ToChar(v) != '\0')
                        {
                            strTmp += Convert.ToChar(v);
                        }
                    }
                    s += "currency(" + strTmp + ")-";
                    strTmp = "";

                    s += "values(" + pos.ulValues + ")-";

                    s += "cashInCount(" + pos.ulCashInCount + ")-";
                    s += "count(" + pos.ulCount + ")-";
                    s += "max(" + pos.ulMaximum + ")-";
                    s += "status(" + pos.usStatus + ")-";
                    s += "appLock(" + pos.bAppLock + ")";

                    s += "\r\n";
                    s += "\t\t";
                    s += "noteNumberList: ";
                    //lpNoteNumberList
                    s += "numOfNoteNumbers(" + pos.lpNoteNumberListDim.usNumOfNoteNumbers + ")";
                    
                    _cntr = 0;
                    foreach (CIM.WFSCIMNOTENUMBER v in pos.lpNoteNumberListDim.lppNoteNumberDim)
                    {
                        _cntr++;
                        if(_cntr == 1) 
                            s += " || ";
                        else
                            s += "\r\n\t\t\t";
                        s += "noteID(" + v.usNoteID + ")-";
                        s += "count(" + v.ulCount + ")";
                        
                    }

                    s += "\r\n";
                    s += "\t\t";
                    s += "numPhysicalCUs(" + pos.usNumPhysicalCUs + ")";

                    foreach (CIM.WFSCIMPHCU v in pos.lppPhysicalDim)
                    {
                        s += "\r\n";
                        s += "\t\t\t";
                        s += "phyPosName(";
                        if (IntPtr.Zero.Equals(v.lpPhysicalPositionName) == false)
                        {
                            for (Int32 i = 0; true; i++)
                            {
                                System.Byte b1 = Marshal.ReadByte(v.lpPhysicalPositionName, i);
                                if (b1 == 0)
                                    break;
                                s += Convert.ToChar(b1);
                            }

                            s += ")-";
                            s += "unitID(";
                            foreach (SByte v1 in v.cUnitID)
                            {
                                if (Convert.ToChar(v1) != '\0')
                                {
                                    s += Convert.ToChar(v1);
                                }

                            }
                            s += ")-";

                            s += "cashInCount(" + v.ulCashInCount + ")-";
                            s += "count(" + v.ulCount + ")-";
                            s += "max(" + v.ulMaximum + ")-";
                            s += "pStatus(" + v.usPStatus + ")-";
                            s += "hardwareSensors(" + v.bHardwareSensors + ")";
                        }
                    }
                }
                Trace(s);
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion
        }

        #endregion
    }
}