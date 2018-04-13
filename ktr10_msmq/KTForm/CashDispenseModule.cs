using System;
using System.ComponentModel;
using System.Windows.Forms;
using KT.WOSA.CDM;
using KTR10_CDM;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GDef = KT.WOSA.CDM.XfsGlobalDefine;
using CDM = KT.WOSA.CDM.XfsCdmDefine;
using System.Runtime.Remoting.Messaging;

namespace KTForm.CashDispenseModule
{

    public partial class MainUIForm : Form
    {
        #region Form Elements
        private TextBox TBTrace;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem cDMToolStripMenuItem;
        private ToolStripMenuItem registerToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem wFSINFCDMToolStripMenuItem;
        private ToolStripMenuItem wFSINFCDMCAPABILITIESToolStripMenuItem;
        private ToolStripMenuItem wFSINFCDMCASHUNITINFOToolStripMenuItem;
        private ToolStripMenuItem wFSINFCDMTELLERINFOToolStripMenuItem;
        private ToolStripMenuItem wFSINFCDMCURRENCYEXPToolStripMenuItem;
        private ToolStripMenuItem wFSINFCDMMIXTYPESToolStripMenuItem;
        private ToolStripMenuItem wFSINFCDMMIXTABLEToolStripMenuItem;
        private ToolStripMenuItem wFSINFCDMPRESENTSTATUSToolStripMenuItem;
        private ToolStripMenuItem wFSCMDToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMSETTELLERINFOToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMSETCASHUNITINFOToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMSTARTEXCHANGEToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        private IContainer components;
        private ToolStripMenuItem wFSCMDCDMENDEXCHANGEToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMOPENSAFEDOORToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMSETMIXTABLEToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMRESETToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMTESTCASHUNITSToolStripMenuItem;
        private ToolStripMenuItem cASHOUTToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMDISPENSEToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMPRESENTToolStripMenuItem;
        private ToolStripMenuItem wFSCMDCDMDENOMINATEToolStripMenuItem;
        private ToolStripMenuItem countToolStripMenuItem;
        private ToolStripMenuItem rejectToolStripMenuItem;
        private ToolStripMenuItem retractToolStripMenuItem;
        private ToolStripMenuItem statusToolStripMenuItem1;
        private ToolStripMenuItem openShutterToolStripMenuItem;
        private ToolStripMenuItem closeShutterToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem wFSCMDCDMDENOMINATEToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMCOUNTToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMDISPENSEToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMPRESENTToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMREJECTToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMRETRACTToolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem wFSCMDCDMOPENSHUTTERToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMCLOSESHUTTERToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMSETTELLERINFOToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMENDEXCHANGEToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMOPENSAFEDOORToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMSETMIXTABLEToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMRESETToolStripMenuItem1;
        private ToolStripMenuItem wFSCMDCDMTESTCASHUNITSToolStripMenuItem1;
        #endregion

        #region Variables
        private Form1 Mainform;

        struct WaitOneDefiniton
        {
            public static int MaxWait = 10;
            public static TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(-1);
        }

        private static bool signaled = false;

        private Xfs30Api cashOut = null;

        private string amount = "", currency = "";

        private long eventId;
        private XfsGlobalDefine.WFSRESULT? GlobalWFSResult = null;

        #endregion

        #region volatile Boolean Completed Properties
        private volatile Boolean WFS_CMD_CDM_SET_CASH_UNIT_INFOCompleted = false;
        private volatile Boolean WFS_CMD_CDM_START_EXCHANGECompleted = false;
        private volatile Boolean WFS_CMD_CDM_END_EXCHANGECompleted = false;
        private volatile Boolean WFS_CMD_CDM_DENOMINATEComplete = false;
        private volatile Boolean WFS_CMD_CDM_DISPENSECompleted = false;
        private volatile Boolean WFS_CMD_CDM_REJECTCompleted = false;
        private volatile Boolean WFS_CMD_CDM_PRESENTCompleted = false;
        private volatile Boolean WFS_CMD_CDM_RETRACTCompleted = false;
        private volatile Boolean WFS_CMD_CDM_OPEN_SHUTTERCompleted = false;
        private volatile Boolean WFS_CMD_CDM_CLOSE_SHUTTERCompleted = false;
        private volatile Boolean WFS_CMD_CDM_COUNT_EXCompleted = false;
        private volatile Boolean WFS_CMD_CDM_RESETCompleted = false;

        #endregion

        #region volatile Boolean Completed Properties
        private volatile bool SERVICEEVENT_ITEMSTAKEN = false;
        #endregion

        public MainUIForm(Form1 Mainform)
        {
            InitializeComponent();
            TBTrace.Dock = DockStyle.Fill;

            this.Mainform = Mainform;

            cashOut = new Xfs30Api(6);

            cashOut.AutoDenom = true;
            cashOut.AutoPresent = false;
            cashOut.MixNumber = 2;
            cashOut.TellerId = 0;

            cashOut.imp.cdmEvent += new KT.WOSA.CDM.IMP.Event(this.CDMEvent);
        }

        #region Main
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TBTrace.Text = "";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Mainform != null)
                Mainform.CDMIsOpen = false;
            this.Close();
        }

        private void MainUIForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Mainform != null)
                Mainform.CDMIsOpen = false;
            this.Close();
        }

        #endregion

        #region CDM
        private void openRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("\t -->Begin...");

            string s = "OpenRegister\r\n";

            try
            {
                cashOut.OpenRegister();
                s = "(OpenRegister) --> Success!";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "(OpenRegister) --> Failed! Return: " + ex.ExceptionCode;
                Trace(s);
                return;
            }

            s += "\r\n";
            
            KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION WFSVersion, SrvcVersion, SPIVersion;

            cashOut.GetSpInfoVersion(out WFSVersion, out  SrvcVersion, out SPIVersion);

            s += "GetSpInfoVersion\r\n";

            s += "WFSVersion[ ";
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
                
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "Close\r\n";
            cashOut.Close();
            s += "Succeed \r\n";
            Trace(s);
        }

        #endregion

        #region Get Info
        private void statusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CDM_STATUS\r\n";

            KT.WOSA.CDM.XfsCdmDefine.WFSCDMSTATUS_Dim lpDataOut;

            try
            {
                lpDataOut = cashOut.WFS_INF_CDM_STATUS();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace("\r\n\r\nWFS_INF_CDM_STATUS = " + ex.ExceptionCode);
                return;
            }

            s = "WFSCDMStatus:\r\n";

            string fwDevice = lpDataOut.fwDevice.ToString().PadLeft(4, '0');
            string fwSafeDoor = lpDataOut.fwSafeDoor.ToString().PadLeft(4, '0');
            string fwDispenser = lpDataOut.fwDispenser.ToString().PadLeft(4, '0');
            string fwIntermediateStacker = lpDataOut.fwIntermediateStacker.ToString().PadLeft(4, '0');
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
            
            Trace(s);
        }        

        private void wFSINFCDMCAPABILITIESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CDM_CAPABILITIES\r\n";

            XfsCdmDefine.WFSCDMCAPS lpCaps;

            try
            {
                lpCaps = cashOut.WFS_INF_CDM_CAPABILITIES();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }

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

            Trace(s);
        }

        private void wFSINFCDMCASHUNITINFOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CDM_CASH_UNIT_INFO\r\n";

            KT.WOSA.CDM.XfsCdmDefine.WFSCDMCUINFO_Dim lpCashUnitInfo;
            try
            {
                lpCashUnitInfo = cashOut.WFS_INF_CDM_CASH_UNIT_INFO();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }

            DebugCashUnitInfo(lpCashUnitInfo);

            Trace(s);
        }

        private void wFSINFCDMTELLERINFOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("not implemented");
        }

        private void wFSINFCDMCURRENCYEXPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CDM_CURRENCY_EXP\r\n";

            List<KT.WOSA.CDM.XfsCdmDefine.WFSCDMCURRENCYEXP> CurrencyExpList;
            try
            {
                CurrencyExpList = cashOut.WFS_INF_CDM_CURRENCY_EXP();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }

            #region Debug
            try
            {
                String strTmp = "";
                foreach (var CurrencyExp in CurrencyExpList)
                {
                    strTmp = "";
                    foreach (SByte v in CurrencyExp.cCurrencyID)
                    {
                        strTmp += Convert.ToChar(v);
                    }

                    s += "\tcCurrencyID(" + strTmp + ")-";
                    s += "sExponent(" + CurrencyExp.sExponent + ")";
                }

            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            Trace(s);
        }

        private void wFSINFCDMMIXTYPESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CDM_MIX_TYPES\r\n";

            List<KT.WOSA.CDM.XfsCdmDefine.WFSCDMMIXTYPE> MixTypesList;
            try
            {
                MixTypesList = cashOut.WFS_INF_CDM_MIX_TYPES();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }

            #region Debug
            try
            {
                foreach (var MixTypes in MixTypesList)
                {
                    s += "\tusMixNumber(" + MixTypes.usMixNumber + ")-usMixType(" + MixTypes.usMixType + ")-usSubType(" + MixTypes.usSubType + ")-lpszName(" + MixTypes.lpszName + ")";
                }
            }
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            Trace(s);
        }

        private void wFSINFCDMMIXTABLEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CDM_MIX_TABLE\r\n";

            KT.WOSA.CDM.XfsCdmDefine.WFSCDMMIXTABLE_Dim MixTable;
            try
            {
                MixTable = cashOut.WFS_INF_CDM_MIX_TABLE();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }

            #region Debug
            try
            {
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
                foreach (var v in MixTable.lppMixRowsDim)
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
            catch (Exception ex)
            {
                Trace(String.Format(Utility.TraceDebugStr, System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex.Message));
            }
            #endregion

            Trace(s);
        }

        private void wFSINFCDMPRESENTSTATUSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CDM_PRESENT_STATUS\r\n";

            KT.WOSA.CDM.XfsCdmDefine.WFSCDMPRESENTSTATUS_Dim PresentStatus;
            try
            {
                PresentStatus = cashOut.WFS_INF_CDM_PRESENT_STATUS();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }

            #region Debug
            try
            {
                do
                {
                    if (IntPtr.Zero.Equals(PresentStatus.lpDenomination) == false)
                    {
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
        }
        
        #endregion
        
        #region Execute
        private void wFSCMDCDMDENOMINATEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new FormDenominate())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    amount = form.Amount;
                    currency = form.Currency;
                }
                else
                {
                    Trace("Cancelled!");
                    return;
                }
            }

            String s = "WFS_CMD_CDM_DENOMINATE:\r\n";

            KT.WOSA.CDM.XfsCdmDefine.WFSCDMDENOMINATION_Dim lpDenominationOut = new KT.WOSA.CDM.XfsCdmDefine.WFSCDMDENOMINATION_Dim();
            try
            {
                s += "\t Amount " + amount + "\r\n";
                s += "\t Currency " + currency + "\r\n";
                lpDenominationOut = cashOut.WFS_CMD_CDM_DENOMINATE(Convert.ToUInt32(amount), currency);
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }           

            Trace(s);
        }

        private void wFSCMDCDMDISPENSEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new FormDenominate())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    amount = form.Amount;
                    currency = form.Currency;
                }
                else
                {
                    Trace("Cancelled!");
                    return;
                }
            }

            string s = "WFS_CMD_CDM_DISPENSE:\r\n";
            String strTmp = "";

            KT.WOSA.CDM.XfsCdmDefine.WFSCDMDENOMINATION_Dim lpDenominationOut;

            try
            {
                s += "\t Amount " + amount + "\r\n";
                s += "\t Currency " + currency + "\r\n";
                lpDenominationOut = cashOut.WFS_CMD_CDM_DISPENSE(Convert.ToUInt32(amount), currency);
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }

            #region 
            foreach (SByte v in lpDenominationOut.cCurrencyID)
            {
                strTmp += Convert.ToChar(v);
            }

            s += "cCurrencyID\t= " + strTmp;
            s += "\r\nulAmount\t= " + lpDenominationOut.ulAmount;
            s += "\r\nusCount\t= " + lpDenominationOut.usCount;
            foreach (UInt32 v in lpDenominationOut.lpulValuesDim)
            {
                s += "\t\tulValues\t= " + v;
            }

            s += "\r\nulCashBox\t= " + lpDenominationOut.ulCashBox;

            #endregion

            Trace(s);
        }

        private void wFSCMDCDMCOUNTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("not implemented");
        }

        private void wFSCMDCDMPRESENTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SERVICEEVENT_ITEMSTAKEN = false;

            string s = "WFS_CMD_CDM_PRESENT:\r\n";
            try
            {
                cashOut.WFS_CMD_CDM_PRESENT();
                s = "\tSucceed";
                
                Trace(s);
                
                #region Check The Money
                DateTime startTime, endTime;
                int elapsedMillisecs = 0;
                startTime = DateTime.Now;

                Int32 counter = 0;
                while (!SERVICEEVENT_ITEMSTAKEN
                    && elapsedMillisecs < 20000
                    && counter < 20) //for security reasons. this is the maximum check count
                {
                    endTime = DateTime.Now;
                    elapsedMillisecs = (int)((TimeSpan)(endTime - startTime)).TotalMilliseconds;

                    Thread.Sleep(1000);
                    Application.DoEvents();

                    if (SERVICEEVENT_ITEMSTAKEN)
                    {
                        Trace("get the money#" + counter.ToString() + ", elapsed time (ms) : " + elapsedMillisecs.ToString());
                    }
                    else
                    {
                        Trace("money still there #" + counter.ToString() + ", elapsed time (ms) : " + elapsedMillisecs.ToString());
                    }


                    counter++;
                }

                if (!SERVICEEVENT_ITEMSTAKEN)
                {
                    Trace("retracting...");
                    wFSCMDCDMRETRACTToolStripMenuItem_Click(sender, EventArgs.Empty);
                    Trace("retracted...");
                }

                #endregion


                Trace("Event Reached! DenominateResult : " + this.GlobalWFSResult.Value.hResult.ToString());

            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }
            Trace(s);
        }

        private void wFSCMDCDMREJECTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CDM_REJECT\t\n";
            try
            {
                cashOut.WFS_CMD_CDM_REJECT();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }
            Trace(s);
        }

        private void wFSCMDCDMRETRACTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CDM_RETRACT\r\n";
            try
            {
                cashOut.WFS_CMD_CDM_RETRACT();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }
            Trace(s);
        }
        #endregion

        #region Execute Admin
        private void wFSCMDCDMOPENSHUTTERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CDM_OPEN_SHUTTER\r\n"; try
            {
                cashOut.WFS_CMD_CDM_OPEN_SHUTTER();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }
            Trace(s);
        }

        private void wFSCMDCDMCLOSESHUTTERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CDM_CLOSE_SHUTTER:\t\n";
            try
            {
                cashOut.WFS_CMD_CDM_CLOSE_SHUTTER();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }
            Trace(s);
        }
        
        private void wFSCMDCDMSETTELLERINFOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("not implemented");
        }

        private void wFSCMDCDMSETCASHUNITINFOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("not implemented");
            //string s = "wFSCMDCDMSETCASHUNITINFO\t\n";
            //try
            //{

            //    XfsCdmDefine.WFSCDMCUINFO_Dim lpCashUnitInfo = cashOut.WFS_INF_CDM_CASH_UNIT_INFO();


            //    List<XfsCdmDefine.WFSCDMCASHUNIT_Dim> cashUnitList = lpCashUnitInfo.lppListDim;

            //    for (int counter = 0; counter < lpCashUnitInfo.usCount; counter++)
            //    {
            //        KT.WOSA.CDM.XfsCdmDefine.WFSCDMCASHUNIT_Dim cashUnit = lpCashUnitInfo.lppListDim[counter];
                    
            //        string currency = "";
            //        foreach (var v in cashUnit.cCurrencyID)
            //        {
            //            currency += Convert.ToChar(v);
            //        }
            //        if (currency == "TRY")
            //            currency = "TRL";

            //        switch (KTR10_CDM.Xfs30Definitions.CDM.CDMCashUnitTypeConvert(cashUnit.usType))
            //        {
            //            case KTR10_CDM.Xfs30Definitions.CDM.CDMCashUnitType.WFS_CDM_TYPEREJECTCASSETTE:
            //                #region REJECT
            //                cashUnit.ulInitialCount = 0;
            //                cashUnit.ulCount = 0;
            //                cashUnit.ulRejectCount = 0;
            //                #endregion
            //                break;
            //            case KTR10_CDM.Xfs30Definitions.CDM.CDMCashUnitType.WFS_CDM_TYPERETRACTCASSETTE:
            //                #region RETRACT
            //                cashUnit.ulInitialCount = 0;
            //                cashUnit.ulCount = 0;
            //                cashUnit.ulRejectCount = 0;
            //                #endregion
            //                break;
            //            case KTR10_CDM.Xfs30Definitions.CDM.CDMCashUnitType.WFS_CDM_TYPERECYCLING:
            //                #region RECYCLER
            //                var loaderUnit = (from l in ExchangeInfo.Cassette
            //                                  where l.Values == cashUnit.ulValues &&
            //                                           l.Currency == currency
            //                                  select l).ToList<DeviceInterface.CashUnitData>();

            //                if (loaderUnit != null && loaderUnit.Count > 0)
            //                {
            //                    cashUnit.ulInitialCount = (uint)loaderUnit[0].InitialTotalInCassette;
            //                    cashUnit.ulCount = cashUnit.ulInitialCount;
            //                    cashUnit.ulRejectCount = 0;
            //                }
            //                #endregion
            //                break;
            //            default:
            //                Trace("for loop continue");
            //                continue;
            //        }

            //        lpCashUnitInfo.lppListDim[counter] = cashUnit;
            //    }
                
            //    cashOut.WFS_CMD_CDM_SET_CASH_UNIT_INFO(lpCashUnitInfo);

            //    s += "Succeed \r\n";
            //}
            //catch (KTR10_CDM.Exceptions.BaseException ex)
            //{
            //    s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
            //    Trace(s);
            //    return;
            //}            
            //Trace(s);
        }

        private void wFSCMDCDMSTARTEXCHANGEToolStripMenuItem_Click(object sender, EventArgs e)
        {   
            Trace("not implemented");
        }

        private void wFSCMDCDMENDEXCHANGEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("not implemented");
        }

        private void wFSCMDCDMOPENSAFEDOORToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("not implemented");
        }

        private void wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("not implemented");
        }

        private void wFSCMDCDMSETMIXTABLEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("not implemented");
        }

        private void wFSCMDCDMRESETToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CDM_RESET\r\n";
            try
            {
                cashOut.WFS_CMD_CDM_RESET();
                s += "Succeed \r\n";
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tFailed! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }
            Trace(s);
        }

        private void wFSCMDCDMTESTCASHUNITSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("not implemented");
        }

        #endregion

        #region Execute Async
        private void wFSCMDCDMDENOMINATEToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            WFS_CMD_CDM_DENOMINATEComplete = false;

            using (var form = new FormDenominate())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    amount = form.Amount;
                    currency = form.Currency;
                }
                else
                {
                    Trace("Cancelled!");
                    return;
                }
            }

            String s = "WFS_CMD_CDM_DENOMINATE_Async:\r\n";
            try
            {
                s += "\t Amount " + amount + "\r\n";
                s += "\t Currency " + currency + "\r\n";
                Trace(s);

                cashOut.WFS_CMD_CDM_DENOMINATE_Async(Convert.ToUInt32(amount), currency);
            }
            catch (KTR10_CDM.Exceptions.AsyncMethodCallException ex)
            {
                s += "\tAsyncMethodCallException! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }
            catch (KTR10_CDM.Exceptions.BaseException ex)
            {
                s += "\tBaseException! Return " + ex.ExceptionCode + "\r\n";
                Trace(s);
                return;
            }
            catch (Exception ex)
            {
                s += "\tException! Return " + ex.Message + "\r\n";
                Trace(s);
                return;
            }
        }

        private void wFSCMDCDMDISPENSEToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMCOUNTToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMPRESENTToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
        }

        private void wFSCMDCDMREJECTToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMRETRACTToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region Execute Admin Async
        private void wFSCMDCDMOPENSHUTTERToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMCLOSESHUTTERToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMSETTELLERINFOToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMENDEXCHANGEToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMOPENSAFEDOORToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMSETMIXTABLEToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMRESETToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void wFSCMDCDMTESTCASHUNITSToolStripMenuItem1_Click(object sender, EventArgs e)
        {            
        }

        #endregion

        #region Events
        public int CDMEvent(long eventId, XfsGlobalDefine.WFSRESULT WFSResult)
        {
            this.eventId = eventId;
            this.GlobalWFSResult = WFSResult;
            
            string s = "Event (" + eventId + "):\r\n";
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

            switch (KTR10_CDM.Xfs30Definitions.CDM.CDMMessagesConvert(WFSResult.dwCmdCodeOrEventID))
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

                    SERVICEEVENT_ITEMSTAKEN = true;

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
                    try
                    {
                        for (int i = 0; i < lpWFSHardwareError.dwSize; i++)
                        {
                            s += Convert.ToChar(lpWFSHardwareError.lpbDescription);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ManagerSysEvent exception : " + ex.Message);
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
                    WFS_CMD_CDM_SET_CASH_UNIT_INFOCompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_START_EXCHANGE:
                    s += "Event:WFS_CMD_CDM_START_EXCHANGE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_START_EXCHANGECompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_END_EXCHANGE:
                    s += "Event:WFS_CMD_CDM_END_EXCHANGE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_END_EXCHANGECompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_DENOMINATE:
                    s += "Event:WFS_CMD_CDM_DENOMINATE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_DENOMINATEComplete = true;
                    
                    break;

                case CDM.WFS_CMD_CDM_DISPENSE:
                    s += "Event:WFS_CMD_CDM_DISPENSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_DISPENSECompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_REJECT:
                    s += "Event:WFS_CMD_CDM_REJECT(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_REJECTCompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_PRESENT:
                    s += "Event:WFS_CMD_CDM_PRESENT(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_PRESENTCompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_RETRACT:
                    s += "Event:WFS_CMD_CDM_RETRACT(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_RETRACTCompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_OPEN_SHUTTER:
                    s += "Event:WFS_CMD_CDM_OPEN_SHUTTER(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_OPEN_SHUTTERCompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_CLOSE_SHUTTER:
                    s += "Event:WFS_CMD_CDM_CLOSE_SHUTTER(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_CLOSE_SHUTTERCompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_COUNT_EX:
                    s += "Event:WFS_CMD_CDM_COUNT_EX(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_COUNT_EXCompleted = true;

                    break;

                case CDM.WFS_CMD_CDM_RESET:
                    s += "Event:WFS_CMD_CDM_RESET(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    WFS_CMD_CDM_RESETCompleted = true;

                    break;
            }
            Trace(s);
        }

        #endregion

        #region Helpers
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

        private void Trace(string strInfo)
        {
            try
            {
                DateTime dt = DateTime.Now;
                System.String sTime = "";

                sTime += dt.Hour.ToString("D02") + ":" + dt.Minute.ToString("D02") + ":" + dt.Second.ToString("D02") + "." + dt.Millisecond.ToString("D03");
                sTime = "[ " + sTime + " ]";

                CrossThreadUI.SetText(TBTrace, sTime + "\r\n" + strInfo + "\r\n", true);
            }
            catch (Exception ex)
            {
                AutoClosingMessageBox.Show("<Trace> exception occurred , here is the detail! " + ex.Message, "Error", 1000);
            }
            finally
            {
                ScrollToCurrent();
            }
        }

        public static bool WaitOne(WaitHandle handle, int timeoutInMs)
        {
            string strDatetimeFormat = "HH:mm:ss.fff";
            Console.WriteLine("Wait Entry");
            Console.WriteLine("Wait Timeout : {0} ", timeoutInMs);
            Console.WriteLine("Wait Start : {0}", DateTime.Now.ToString(strDatetimeFormat));

            TimeSpan timeout = new TimeSpan(0, 0, 0, 0, timeoutInMs);

            int expireTicks = 0, waitTime = 0; ;
            bool _signaled = false, exitLoop = false;

            #region Check The Inputs...
            if (handle == null)
                throw new ArgumentException("handle is null");
            else if ((handle.SafeWaitHandle.IsClosed))
                throw new ArgumentException("closed wait handle", "handle");
            else if ((handle.SafeWaitHandle.IsInvalid))
                throw new ArgumentException("invalid wait handle", "handle");
            else if ((timeout < WaitOneDefiniton.InfiniteTimeout))
                throw new ArgumentException("invalid timeout < -1", "timeout");

            #endregion

            #region Wait For The Signal...
            expireTicks = (int)Environment.TickCount + (int)timeout.TotalMilliseconds;

            do
            {
                if (timeout.Equals(WaitOneDefiniton.InfiniteTimeout))
                {
                    waitTime = WaitOneDefiniton.MaxWait;
                }
                else
                {
                    waitTime = (expireTicks - Environment.TickCount);
                    if (waitTime <= 0)
                    {
                        exitLoop = true;
                        waitTime = 0;
                    }
                    else if (waitTime > WaitOneDefiniton.MaxWait)
                    {
                        waitTime = WaitOneDefiniton.MaxWait;
                    }
                }

                if ((handle.SafeWaitHandle.IsClosed))
                {
                    exitLoop = true;
                }
                else if (handle.WaitOne(waitTime, false))
                {
                    exitLoop = true;
                    _signaled = true;
                }
                else
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }
            while (!exitLoop);

            #endregion

            Console.WriteLine("Wait End : {0}", DateTime.Now.ToString(strDatetimeFormat));
            Console.WriteLine("Wait Return Object : Signaled ({0})", signaled.ToString());

            Console.WriteLine("Wait Exit");
            return _signaled;
        }

        public void ScrollToCurrent()
        {
            TBTrace.Select(TBTrace.Text.Length, 0);
            TBTrace.ScrollToCaret(); 
        }

        #endregion

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TBTrace = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cDMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSINFCDMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSINFCDMCAPABILITIESToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSINFCDMCASHUNITINFOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSINFCDMTELLERINFOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSINFCDMCURRENCYEXPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSINFCDMMIXTYPESToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSINFCDMMIXTABLEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSINFCDMPRESENTSTATUSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cASHOUTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMDENOMINATEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.countToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMDISPENSEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMPRESENTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rejectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openShutterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeShutterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMRESETToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMDENOMINATEToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMCOUNTToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMDISPENSEToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMPRESENTToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMREJECTToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMRETRACTToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMOPENSHUTTERToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMCLOSESHUTTERToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMRESETToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TBTrace
            // 
            this.TBTrace.Location = new System.Drawing.Point(0, 54);
            this.TBTrace.Multiline = true;
            this.TBTrace.Name = "TBTrace";
            this.TBTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TBTrace.Size = new System.Drawing.Size(668, 290);
            this.TBTrace.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.cDMToolStripMenuItem,
            this.wFSINFCDMToolStripMenuItem,
            this.cASHOUTToolStripMenuItem,
            this.wFSCMDToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(668, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(41, 20);
            this.toolStripMenuItem1.Text = "Main";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.clearToolStripMenuItem.Text = "1. Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exitToolStripMenuItem.Text = "2. Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // cDMToolStripMenuItem
            // 
            this.cDMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registerToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.cDMToolStripMenuItem.Name = "cDMToolStripMenuItem";
            this.cDMToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.cDMToolStripMenuItem.Text = "Service";
            // 
            // registerToolStripMenuItem
            // 
            this.registerToolStripMenuItem.Name = "registerToolStripMenuItem";
            this.registerToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.registerToolStripMenuItem.Text = "1. Open + Register";
            this.registerToolStripMenuItem.Click += new System.EventHandler(this.openRegisterToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.closeToolStripMenuItem.Text = "2. Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // wFSINFCDMToolStripMenuItem
            // 
            this.wFSINFCDMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusToolStripMenuItem1,
            this.wFSINFCDMCAPABILITIESToolStripMenuItem,
            this.wFSINFCDMCASHUNITINFOToolStripMenuItem,
            this.wFSINFCDMTELLERINFOToolStripMenuItem,
            this.wFSINFCDMCURRENCYEXPToolStripMenuItem,
            this.wFSINFCDMMIXTYPESToolStripMenuItem,
            this.wFSINFCDMMIXTABLEToolStripMenuItem,
            this.wFSINFCDMPRESENTSTATUSToolStripMenuItem});
            this.wFSINFCDMToolStripMenuItem.Name = "wFSINFCDMToolStripMenuItem";
            this.wFSINFCDMToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.wFSINFCDMToolStripMenuItem.Text = "GetInfo";
            // 
            // statusToolStripMenuItem1
            // 
            this.statusToolStripMenuItem1.Name = "statusToolStripMenuItem1";
            this.statusToolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
            this.statusToolStripMenuItem1.Text = "1. Status";
            this.statusToolStripMenuItem1.Click += new System.EventHandler(this.statusToolStripMenuItem_Click);
            // 
            // wFSINFCDMCAPABILITIESToolStripMenuItem
            // 
            this.wFSINFCDMCAPABILITIESToolStripMenuItem.Name = "wFSINFCDMCAPABILITIESToolStripMenuItem";
            this.wFSINFCDMCAPABILITIESToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.wFSINFCDMCAPABILITIESToolStripMenuItem.Text = "2. Capabilities";
            this.wFSINFCDMCAPABILITIESToolStripMenuItem.Click += new System.EventHandler(this.wFSINFCDMCAPABILITIESToolStripMenuItem_Click);
            // 
            // wFSINFCDMCASHUNITINFOToolStripMenuItem
            // 
            this.wFSINFCDMCASHUNITINFOToolStripMenuItem.Name = "wFSINFCDMCASHUNITINFOToolStripMenuItem";
            this.wFSINFCDMCASHUNITINFOToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.wFSINFCDMCASHUNITINFOToolStripMenuItem.Text = "3. CashUnitInfo";
            this.wFSINFCDMCASHUNITINFOToolStripMenuItem.Click += new System.EventHandler(this.wFSINFCDMCASHUNITINFOToolStripMenuItem_Click);
            // 
            // wFSINFCDMTELLERINFOToolStripMenuItem
            // 
            this.wFSINFCDMTELLERINFOToolStripMenuItem.Name = "wFSINFCDMTELLERINFOToolStripMenuItem";
            this.wFSINFCDMTELLERINFOToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.wFSINFCDMTELLERINFOToolStripMenuItem.Text = "4. TellerInfo";
            this.wFSINFCDMTELLERINFOToolStripMenuItem.Click += new System.EventHandler(this.wFSINFCDMTELLERINFOToolStripMenuItem_Click);
            // 
            // wFSINFCDMCURRENCYEXPToolStripMenuItem
            // 
            this.wFSINFCDMCURRENCYEXPToolStripMenuItem.Name = "wFSINFCDMCURRENCYEXPToolStripMenuItem";
            this.wFSINFCDMCURRENCYEXPToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.wFSINFCDMCURRENCYEXPToolStripMenuItem.Text = "5. CurrencyExp";
            this.wFSINFCDMCURRENCYEXPToolStripMenuItem.Click += new System.EventHandler(this.wFSINFCDMCURRENCYEXPToolStripMenuItem_Click);
            // 
            // wFSINFCDMMIXTYPESToolStripMenuItem
            // 
            this.wFSINFCDMMIXTYPESToolStripMenuItem.Name = "wFSINFCDMMIXTYPESToolStripMenuItem";
            this.wFSINFCDMMIXTYPESToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.wFSINFCDMMIXTYPESToolStripMenuItem.Text = "6. MixTypes";
            this.wFSINFCDMMIXTYPESToolStripMenuItem.Click += new System.EventHandler(this.wFSINFCDMMIXTYPESToolStripMenuItem_Click);
            // 
            // wFSINFCDMMIXTABLEToolStripMenuItem
            // 
            this.wFSINFCDMMIXTABLEToolStripMenuItem.Name = "wFSINFCDMMIXTABLEToolStripMenuItem";
            this.wFSINFCDMMIXTABLEToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.wFSINFCDMMIXTABLEToolStripMenuItem.Text = "7. MixTable";
            this.wFSINFCDMMIXTABLEToolStripMenuItem.Click += new System.EventHandler(this.wFSINFCDMMIXTABLEToolStripMenuItem_Click);
            // 
            // wFSINFCDMPRESENTSTATUSToolStripMenuItem
            // 
            this.wFSINFCDMPRESENTSTATUSToolStripMenuItem.Name = "wFSINFCDMPRESENTSTATUSToolStripMenuItem";
            this.wFSINFCDMPRESENTSTATUSToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.wFSINFCDMPRESENTSTATUSToolStripMenuItem.Text = "8. PresentStatus";
            this.wFSINFCDMPRESENTSTATUSToolStripMenuItem.Click += new System.EventHandler(this.wFSINFCDMPRESENTSTATUSToolStripMenuItem_Click);
            // 
            // cASHOUTToolStripMenuItem
            // 
            this.cASHOUTToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wFSCMDCDMDENOMINATEToolStripMenuItem,
            this.countToolStripMenuItem,
            this.wFSCMDCDMDISPENSEToolStripMenuItem,
            this.wFSCMDCDMPRESENTToolStripMenuItem,
            this.rejectToolStripMenuItem,
            this.retractToolStripMenuItem});
            this.cASHOUTToolStripMenuItem.Name = "cASHOUTToolStripMenuItem";
            this.cASHOUTToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.cASHOUTToolStripMenuItem.Text = "Execute";
            // 
            // wFSCMDCDMDENOMINATEToolStripMenuItem
            // 
            this.wFSCMDCDMDENOMINATEToolStripMenuItem.Name = "wFSCMDCDMDENOMINATEToolStripMenuItem";
            this.wFSCMDCDMDENOMINATEToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.wFSCMDCDMDENOMINATEToolStripMenuItem.Text = "1. Denominate";
            this.wFSCMDCDMDENOMINATEToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMDENOMINATEToolStripMenuItem_Click);
            // 
            // countToolStripMenuItem
            // 
            this.countToolStripMenuItem.Name = "countToolStripMenuItem";
            this.countToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.countToolStripMenuItem.Text = "2. Count";
            this.countToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMCOUNTToolStripMenuItem_Click);
            // 
            // wFSCMDCDMDISPENSEToolStripMenuItem
            // 
            this.wFSCMDCDMDISPENSEToolStripMenuItem.Name = "wFSCMDCDMDISPENSEToolStripMenuItem";
            this.wFSCMDCDMDISPENSEToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.wFSCMDCDMDISPENSEToolStripMenuItem.Text = "3. Dispense";
            this.wFSCMDCDMDISPENSEToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMDISPENSEToolStripMenuItem_Click);
            // 
            // wFSCMDCDMPRESENTToolStripMenuItem
            // 
            this.wFSCMDCDMPRESENTToolStripMenuItem.Name = "wFSCMDCDMPRESENTToolStripMenuItem";
            this.wFSCMDCDMPRESENTToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.wFSCMDCDMPRESENTToolStripMenuItem.Text = "4. Present";
            this.wFSCMDCDMPRESENTToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMPRESENTToolStripMenuItem_Click);
            // 
            // rejectToolStripMenuItem
            // 
            this.rejectToolStripMenuItem.Name = "rejectToolStripMenuItem";
            this.rejectToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.rejectToolStripMenuItem.Text = "5. Reject";
            this.rejectToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMREJECTToolStripMenuItem_Click);
            // 
            // retractToolStripMenuItem
            // 
            this.retractToolStripMenuItem.Name = "retractToolStripMenuItem";
            this.retractToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.retractToolStripMenuItem.Text = "6. Retract";
            this.retractToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMRETRACTToolStripMenuItem_Click);
            // 
            // wFSCMDToolStripMenuItem
            // 
            this.wFSCMDToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openShutterToolStripMenuItem,
            this.closeShutterToolStripMenuItem,
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem,
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem,
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem,
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem,
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem,
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem,
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem,
            this.wFSCMDCDMRESETToolStripMenuItem,
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem});
            this.wFSCMDToolStripMenuItem.Name = "wFSCMDToolStripMenuItem";
            this.wFSCMDToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.wFSCMDToolStripMenuItem.Text = "ExecuteAdmin";
            // 
            // openShutterToolStripMenuItem
            // 
            this.openShutterToolStripMenuItem.Name = "openShutterToolStripMenuItem";
            this.openShutterToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.openShutterToolStripMenuItem.Text = "1. Open Shutter";
            this.openShutterToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMOPENSHUTTERToolStripMenuItem_Click);
            // 
            // closeShutterToolStripMenuItem
            // 
            this.closeShutterToolStripMenuItem.Name = "closeShutterToolStripMenuItem";
            this.closeShutterToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.closeShutterToolStripMenuItem.Text = "2. Close Shutter";
            this.closeShutterToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMCLOSESHUTTERToolStripMenuItem_Click);
            // 
            // wFSCMDCDMSETTELLERINFOToolStripMenuItem
            // 
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem.Name = "wFSCMDCDMSETTELLERINFOToolStripMenuItem";
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem.Text = "3. SetTellerInfo";
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMSETTELLERINFOToolStripMenuItem_Click);
            // 
            // wFSCMDCDMSETCASHUNITINFOToolStripMenuItem
            // 
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem.Name = "wFSCMDCDMSETCASHUNITINFOToolStripMenuItem";
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem.Text = "4. SetCashUnitInfo";
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem_Click);
            // 
            // wFSCMDCDMSTARTEXCHANGEToolStripMenuItem
            // 
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem.Name = "wFSCMDCDMSTARTEXCHANGEToolStripMenuItem";
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem.Text = "5. StartExchange";
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem_Click);
            // 
            // wFSCMDCDMENDEXCHANGEToolStripMenuItem
            // 
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem.Name = "wFSCMDCDMENDEXCHANGEToolStripMenuItem";
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem.Text = "6. EndExchange";
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMENDEXCHANGEToolStripMenuItem_Click);
            // 
            // wFSCMDCDMOPENSAFEDOORToolStripMenuItem
            // 
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem.Name = "wFSCMDCDMOPENSAFEDOORToolStripMenuItem";
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem.Text = "7. OpenSafeDoor";
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem_Click);
            // 
            // wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem
            // 
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem.Name = "wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem";
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem.Text = "8. CalibrateCashUnit";
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem_Click);
            // 
            // wFSCMDCDMSETMIXTABLEToolStripMenuItem
            // 
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem.Name = "wFSCMDCDMSETMIXTABLEToolStripMenuItem";
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem.Text = "9. SetMixTable";
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMSETMIXTABLEToolStripMenuItem_Click);
            // 
            // wFSCMDCDMRESETToolStripMenuItem
            // 
            this.wFSCMDCDMRESETToolStripMenuItem.Name = "wFSCMDCDMRESETToolStripMenuItem";
            this.wFSCMDCDMRESETToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMRESETToolStripMenuItem.Text = "0. Reset";
            this.wFSCMDCDMRESETToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMRESETToolStripMenuItem_Click);
            // 
            // wFSCMDCDMTESTCASHUNITSToolStripMenuItem
            // 
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem.Name = "wFSCMDCDMTESTCASHUNITSToolStripMenuItem";
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem.Text = "A. TestCashUnits";
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wFSCMDCDMDENOMINATEToolStripMenuItem1,
            this.wFSCMDCDMCOUNTToolStripMenuItem1,
            this.wFSCMDCDMDISPENSEToolStripMenuItem1,
            this.wFSCMDCDMPRESENTToolStripMenuItem1,
            this.wFSCMDCDMREJECTToolStripMenuItem1,
            this.wFSCMDCDMRETRACTToolStripMenuItem1});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(90, 20);
            this.toolStripMenuItem2.Text = "Execute Async";
            // 
            // wFSCMDCDMDENOMINATEToolStripMenuItem1
            // 
            this.wFSCMDCDMDENOMINATEToolStripMenuItem1.Name = "wFSCMDCDMDENOMINATEToolStripMenuItem1";
            this.wFSCMDCDMDENOMINATEToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.wFSCMDCDMDENOMINATEToolStripMenuItem1.Text = "1. Denominate";
            this.wFSCMDCDMDENOMINATEToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMDENOMINATEToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMCOUNTToolStripMenuItem1
            // 
            this.wFSCMDCDMCOUNTToolStripMenuItem1.Name = "wFSCMDCDMCOUNTToolStripMenuItem1";
            this.wFSCMDCDMCOUNTToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.wFSCMDCDMCOUNTToolStripMenuItem1.Text = "2. Count";
            this.wFSCMDCDMCOUNTToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMCOUNTToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMDISPENSEToolStripMenuItem1
            // 
            this.wFSCMDCDMDISPENSEToolStripMenuItem1.Name = "wFSCMDCDMDISPENSEToolStripMenuItem1";
            this.wFSCMDCDMDISPENSEToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.wFSCMDCDMDISPENSEToolStripMenuItem1.Text = "3. Dispense";
            this.wFSCMDCDMDISPENSEToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMDISPENSEToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMPRESENTToolStripMenuItem1
            // 
            this.wFSCMDCDMPRESENTToolStripMenuItem1.Name = "wFSCMDCDMPRESENTToolStripMenuItem1";
            this.wFSCMDCDMPRESENTToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.wFSCMDCDMPRESENTToolStripMenuItem1.Text = "4. Present";
            this.wFSCMDCDMPRESENTToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMPRESENTToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMREJECTToolStripMenuItem1
            // 
            this.wFSCMDCDMREJECTToolStripMenuItem1.Name = "wFSCMDCDMREJECTToolStripMenuItem1";
            this.wFSCMDCDMREJECTToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.wFSCMDCDMREJECTToolStripMenuItem1.Text = "5. Reject";
            this.wFSCMDCDMREJECTToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMREJECTToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMRETRACTToolStripMenuItem1
            // 
            this.wFSCMDCDMRETRACTToolStripMenuItem1.Name = "wFSCMDCDMRETRACTToolStripMenuItem1";
            this.wFSCMDCDMRETRACTToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.wFSCMDCDMRETRACTToolStripMenuItem1.Text = "6. Retract";
            this.wFSCMDCDMRETRACTToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMRETRACTToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wFSCMDCDMOPENSHUTTERToolStripMenuItem1,
            this.wFSCMDCDMCLOSESHUTTERToolStripMenuItem1,
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem1,
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1,
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1,
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem1,
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem1,
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1,
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem1,
            this.wFSCMDCDMRESETToolStripMenuItem1,
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem1});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(119, 20);
            this.toolStripMenuItem3.Text = "ExecuteAdmin Async";
            // 
            // wFSCMDCDMOPENSHUTTERToolStripMenuItem1
            // 
            this.wFSCMDCDMOPENSHUTTERToolStripMenuItem1.Name = "wFSCMDCDMOPENSHUTTERToolStripMenuItem1";
            this.wFSCMDCDMOPENSHUTTERToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMOPENSHUTTERToolStripMenuItem1.Text = "1. Open Shutter";
            this.wFSCMDCDMOPENSHUTTERToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMOPENSHUTTERToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMCLOSESHUTTERToolStripMenuItem1
            // 
            this.wFSCMDCDMCLOSESHUTTERToolStripMenuItem1.Name = "wFSCMDCDMCLOSESHUTTERToolStripMenuItem1";
            this.wFSCMDCDMCLOSESHUTTERToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMCLOSESHUTTERToolStripMenuItem1.Text = "2. Close Shutter";
            this.wFSCMDCDMCLOSESHUTTERToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMCLOSESHUTTERToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMSETTELLERINFOToolStripMenuItem1
            // 
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem1.Name = "wFSCMDCDMSETTELLERINFOToolStripMenuItem1";
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem1.Text = "3. SetTellerInfo";
            this.wFSCMDCDMSETTELLERINFOToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMSETTELLERINFOToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1
            // 
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1.Name = "wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1";
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1.Text = "4. SetCashUnitInfo";
            this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMSETCASHUNITINFOToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1
            // 
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1.Name = "wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1";
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1.Text = "5. StartExchange";
            this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMSTARTEXCHANGEToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMENDEXCHANGEToolStripMenuItem1
            // 
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem1.Name = "wFSCMDCDMENDEXCHANGEToolStripMenuItem1";
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem1.Text = "6. EndExchange";
            this.wFSCMDCDMENDEXCHANGEToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMENDEXCHANGEToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMOPENSAFEDOORToolStripMenuItem1
            // 
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem1.Name = "wFSCMDCDMOPENSAFEDOORToolStripMenuItem1";
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem1.Text = "7. OpenSafeDoor";
            this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMOPENSAFEDOORToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1
            // 
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1.Name = "wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1";
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1.Text = "8. CalibrateCashUnit";
            this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMCALIBRATECASHUNITToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMSETMIXTABLEToolStripMenuItem1
            // 
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem1.Name = "wFSCMDCDMSETMIXTABLEToolStripMenuItem1";
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem1.Text = "9. SetMixTable";
            this.wFSCMDCDMSETMIXTABLEToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMSETMIXTABLEToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMRESETToolStripMenuItem1
            // 
            this.wFSCMDCDMRESETToolStripMenuItem1.Name = "wFSCMDCDMRESETToolStripMenuItem1";
            this.wFSCMDCDMRESETToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMRESETToolStripMenuItem1.Text = "0. Reset";
            this.wFSCMDCDMRESETToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMRESETToolStripMenuItem1_Click);
            // 
            // wFSCMDCDMTESTCASHUNITSToolStripMenuItem1
            // 
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem1.Name = "wFSCMDCDMTESTCASHUNITSToolStripMenuItem1";
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem1.Text = "A. TestCashUnits";
            this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem1.Click += new System.EventHandler(this.wFSCMDCDMTESTCASHUNITSToolStripMenuItem1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // MainUIForm
            // 
            this.ClientSize = new System.Drawing.Size(668, 348);
            this.Controls.Add(this.TBTrace);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainUIForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
