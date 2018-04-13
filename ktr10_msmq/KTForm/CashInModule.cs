using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KT;
using KT.WOSA.CIM;
using System.Runtime.InteropServices;

namespace KTForm.CashInModule
{
    using CIM = XfsCimDefine;
    using KT.WOSA;
    using KT.WOSA.CIM.IMP;
    public partial class MainUIForm : Form
    {
        #region Form Elements
        private TextBox tbTrace;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem mainToolStripMenuItem;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem cIMToolStripMenuItem;
        private ToolStripMenuItem openRegisterToolStripMenuItem;
        private ToolStripMenuItem wFSINFCIMToolStripMenuItem;
        private ToolStripMenuItem cAPABILITIESToolStripMenuItem;
        private ToolStripMenuItem bANKNOTETYPESToolStripMenuItem;
        private ToolStripMenuItem statusToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem wFSCIMToolStripMenuItem;
        private ToolStripMenuItem cASHUNITINFOToolStripMenuItem;
        private ToolStripMenuItem tELLERINFOToolStripMenuItem;
        private ToolStripMenuItem cURRENCYTOOLToolStripMenuItem;
        private ToolStripMenuItem cASHINSTATUSTOOLToolStripMenuItem;
        private ToolStripMenuItem gETP6INFOTOOLToolStripMenuItem;
        private ToolStripMenuItem gETP6TOOLToolStripMenuItem;
        private ToolStripMenuItem rETRACTToolStripMenuItem;
        private ToolStripMenuItem shutterToolStripMenuItem;
        private ToolStripMenuItem openshutterStripMenuItemClickToolStripMenuItem;
        private ToolStripMenuItem cLOSESHUTTERToolStripMenuItem;
        private ToolStripMenuItem rESETToolStripMenuItem;
        private ToolStripMenuItem sETTELLERINFOToolStripMenuItem;
        private ToolStripMenuItem sETCASHUNITINFOToolStripMenuItem;
        private ToolStripMenuItem sTARTEXCHANGEToolStripMenuItem;
        private ToolStripMenuItem eNDEXCHANGEToolStripMenuItem;
        private ToolStripMenuItem oPENSAFEDOORToolStripMenuItem;
        private ToolStripMenuItem cONFIGURECASHUNITsToolStripMenuItem;
        private ToolStripMenuItem cONFIGURENOTETYPESToolStripMenuItem;
        private ToolStripMenuItem cREATEP6SIGNATUREToolStripMenuItem;
        private ToolStripMenuItem cASHINToolStripMenuItem3;
        private ToolStripMenuItem cASHINSTARTToolStripMenuItem;
        private ToolStripMenuItem cASHINToolStripMenuItem;
        private ToolStripMenuItem cASHINENDToolStripMenuItem;
        private ToolStripMenuItem cASHINROLLBACKToolStripMenuItem;
        #endregion

        XfsCimImp imp = new XfsCimImp();

        private Form1 Mainform;
        public MainUIForm(Form1 Mainform)
        {
            InitializeComponent();
            tbTrace.Dock = DockStyle.Fill;

            this.Mainform = Mainform;

            imp.cimEvent = CIMEvent;
        }

        #region Main
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tbTrace.Text = "";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void MainUIForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
        }

        #endregion

        #region CIM
        private void openRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trace("\t -->Begin...");
            string s = "";
            System.String strLogicalName = "CashAcceptor";

            int hr = imp.OpenSP_Impl(strLogicalName);

            if (XfsGlobalDefine.WFS_SUCCESS != hr)
            {
                s += "Failed return " + hr + "\r\n";
                Trace(s);
                return;
            }


            KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION WFSVersion, SrvcVersion, SPIVersion;

            imp.GetSpInfoVersion_Impl(out WFSVersion, out  SrvcVersion, out SPIVersion);
            s = "WFSVersion: \r\n";
            s += "\twVersion: 0x" + WFSVersion.wVersion.ToString("X").PadLeft(4, '0') + "\r\n";
            s += "\twLowVersion: 0x" + WFSVersion.wLowVersion.ToString("X").PadLeft(4, '0') + "\r\n";
            s += "\twHighVersion: 0x" + WFSVersion.wHighVersion.ToString("X").PadLeft(4, '0') + "\r\n";
            s += "\tszDescription: " + WFSVersion.szDescription + "\r\n";
            s += "\tszSystemStatus: " + WFSVersion.szSystemStatus + "\r\n";

            s += "SrvcVersion: \r\n";
            s += "\twVersion: 0x" + SrvcVersion.wVersion.ToString("X").PadLeft(4, '0') + "\r\n";
            s += "\twLowVersion: 0x" + SrvcVersion.wLowVersion.ToString("X").PadLeft(4, '0') + "\r\n";
            s += "\twHighVersion: 0x" + SrvcVersion.wHighVersion.ToString("X").PadLeft(4, '0') + "\r\n";
            s += "\tszDescription: " + SrvcVersion.szDescription + "\r\n";
            s += "\tszSystemStatus: " + SrvcVersion.szSystemStatus + "\r\n";

            s += "SPIVersion: \r\n";
            s += "\twVersion: 0x" + SPIVersion.wVersion.ToString("X").PadLeft(4, '0') + "\r\n";
            s += "\twVersion: 0x" + SPIVersion.wLowVersion.ToString("X").PadLeft(4, '0') + "\r\n";
            s += "\twVersion: 0x" + SPIVersion.wHighVersion.ToString("X").PadLeft(4, '0') + "\r\n";
            s += "\twVersion: " + SPIVersion.szDescription + "\r\n";
            s += "\twVersion: " + SPIVersion.szSystemStatus + "\r\n";

            Trace(s);
        }

        private void statusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String s = "WFS_INF_CIM_STATUS:\r\n";
            KT.WOSA.CIM.XfsCimDefine.WFSCIMSTATUS_dim lpDataOut; 
            imp.WFS_INF_CIM_STATUS_Impl(out lpDataOut); 

            s += "\tfwDevice:" + lpDataOut.fwDevice.ToString().PadLeft(4, '0') + "\r\n";
            s += "\tfwSafeDoor:" + lpDataOut.fwSafeDoor.ToString().PadLeft(4, '0') + "\r\n";
            s += "\tfwDispenser:" + lpDataOut.fwAcceptor.ToString().PadLeft(4, '0') + "\r\n";
            s += "\tfwIntermediateStacker:" + lpDataOut.fwIntermediateStacker.ToString().PadLeft(4, '0') + "\r\n";

            s += "\tbDropBox: \r\n";



            foreach (CIM.WFSCIMINPOS pos in lpDataOut.lppPositionsDim)
            {
                s += "\r\n\tfwPosition=" + pos.fwPosition + 
                    "\r\n\tfwShutter=" + pos.fwShutter + 
                    "\r\n\tfwTransportStatus= " + pos.fwTransportStatus + 
                    "\r\n\tfwTransport=" + pos.fwTransport + "\r\n";
            }

            s += "\tszExtra: \r\n\t";
            if (IntPtr.Zero.Equals(lpDataOut.lpszExtra) == false)
            {
                for (Int32 i = 0; true; i++)
                {
                    System.Byte b1 = Marshal.ReadByte(lpDataOut.lpszExtra, i);
                    System.Byte b2 = Marshal.ReadByte(lpDataOut.lpszExtra, i + 1);
                    if (b1 == 0 && b2 == 0)
                        break;
                    if (b1 == 0)
                        s += "\r\n\t";
                    else
                        s += Convert.ToChar(b1);
                }
            }
            Trace(s);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imp.CloseSP_Impl();
        }

        #endregion

        #region SHUTTER
        private void openshutterStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_OPEN_SHUTTER:\r\n";
            try
            {
                System.UInt16 fwPosition = CIM.WFS_CIM_POSNULL;
                System.Int32 hRet = imp.WFS_CMD_CIM_OPEN_SHUTTER_Impl(fwPosition);
                Console.WriteLine("\r\nWFS_CMD_CDM_OPEN_SHUTTER_Impl return  ", hRet);
                s += "\r\nWFS_CMD_CDM_OPEN_SHUTTER return  " + hRet + "\r\n";

                if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                    throw new KTR10_CIM.Exceptions.GeneralException(hRet);
            }
            catch (KTR10_CIM.Exceptions.BaseException ex)
            {
                if (ex.IsXfsGeneralError)
                {
                    s += "\t\tGeneral Error " + ex.XfsGeneralError + "\r\n";
                }
                else if (ex.IsXfsCIMError)
                {
                    s += "\t\tCIM Error " + ex.XfsCIMError + "\r\n";
                }
                else
                {
                    s += "\t\tOther Error " + ex.Message + "\r\n";
                }
            }
            catch (Exception ex)
            {
                s += "\t\tError " + ex.Message + "\r\n";
            }
            Trace(s);
        }

        private void closeshutterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_CLOSE_SHUTTER\r\n";
            try
            {
                System.UInt16 fwPosition = CIM.WFS_CIM_POSNULL;
                System.Int32 hRet = imp.WFS_CMD_CIM_CLOSE_SHUTTER_Impl(fwPosition);
                Console.WriteLine("\r\nWFS_CMD_CIM_CLOSE_SHUTTER_Impl return  ", hRet);
                s += "\r\nWFS_CMD_CIM_CLOSE_SHUTTER return  " + hRet + "\r\n";

                if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                    throw new KTR10_CIM.Exceptions.GeneralException(hRet);
            }
            catch (KTR10_CIM.Exceptions.BaseException ex)
            {
                if (ex.IsXfsGeneralError)
                {
                    s += "\t\tGeneral Error " + ex.XfsGeneralError + "\r\n";
                }
                else if (ex.IsXfsCIMError)
                {
                    s += "\t\tCIM Error " + ex.XfsCIMError + "\r\n";
                }
                else
                {
                    s += "\t\tOther Error " + ex.Message + "\r\n";
                }
            }
            catch (Exception ex)
            {
                s += "\t\tError " + ex.Message + "\r\n";
            }
            Trace(s);
        }

        #endregion

        #region INFO
        private void capabilitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CIM_CAPABILITIES:\r\n";

            XfsCimDefine.WFSCIMCAPS lpCaps;

            System.Int32 hRet = imp.WFS_INF_CIM_CAPABILITIES_Impl(out lpCaps);
            s += "WFS_INF_CIM_CAPABILITIES return :" + hRet + "\r\n";
            
            if (XfsGlobalDefine.WFS_SUCCESS != hRet)
            {
                Trace(s);
                return;
            }
            s += "\twClass = " + lpCaps.wClass + "\r\n";
            s += "\tfwType = " + lpCaps.fwType + "\r\n";
            s += "\twMaxCashInItems = " + lpCaps.wMaxCashInItems + "\r\n";
            s += "\tbCompound = " + lpCaps.bCompound + "\r\n";
            s += "\tbShutter = " + lpCaps.bShutter + "\r\n";
            s += "\tbShutterControl = " + lpCaps.bShutterControl + "\r\n";
            s += "\tbSafeDoor = " + lpCaps.bSafeDoor + "\r\n";
            s += "\tbCashBox = " + lpCaps.bCashBox + "\r\n";
            s += "\tbRefill = " + lpCaps.bRefill + "\r\n";
            s += "\tfwIntermediateStacker = " + lpCaps.fwIntermediateStacker + "\r\n";
            s += "\tbItemsTakenSensor = " + lpCaps.bItemsTakenSensor + "\r\n";

            s += "\tbItemsInsertedSensor = " + lpCaps.bItemsInsertedSensor + "\r\n";
            s += "\tfwPositions = " + lpCaps.fwPositions + "\r\n";
            s += "\tfwExchangeType = " + lpCaps.fwExchangeType + "\r\n";
            s += "\tfwRetractAreas = " + lpCaps.fwRetractAreas + "\r\n";
            s += "\tfwRetractTransportActions = " + lpCaps.fwRetractTransportActions + "\r\n";
            s += "\tfwRetractStackerActions = " + lpCaps.fwRetractStackerActions + "\r\n";

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
            Trace(s);
        }

        private void bankNoteTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CIM_BANKNOTE_TYPES :\r\n";

            XfsCimDefine.WFSCIMNOTETYPELIST_dim lpCaps;

            System.Int32 hRet = imp.WFS_INF_CIM_BANKNOTE_TYPES_Impl(out lpCaps);
            s += "WFS_INF_CIM_BANKNOTE_TYPES  return :" + hRet + "\r\n";

            if (XfsGlobalDefine.WFS_SUCCESS != hRet)
            {
                Trace(s);
                return;
            }
            s += "\tusNumOfNoteTypes = " + lpCaps.usNumOfNoteTypes + "\r\n";
            List<CIM.WFSCIMNOTETYPE> lppNoteTypesList = lpCaps.lppNoteTypesDim;
            
            foreach (CIM.WFSCIMNOTETYPE lppNoteTypes in lppNoteTypesList)
            {
                String strTmp = "";
                s += "\t\tusNoteID\t= " + lppNoteTypes.usNoteID + "\r\n";

                foreach (SByte v in lppNoteTypes.cCurrencyID)
                {
                    if (Convert.ToChar(v) != '\0')
                    {
                        strTmp += Convert.ToChar(v);
                    }
                }
                
                s += "\t\tcCurrencyID\t= " + strTmp + "\r\n";

                s += "\t\tulValues\t= " + lppNoteTypes.ulValues + "\r\n";
                s += "\t\tusRelease\t= " + lppNoteTypes.usRelease + "\r\n";
                s += "\t\tbConfigured\t= " + lppNoteTypes.bConfigured + "\r\n";
            }

            Trace(s);
        }
        
        private void cashUnitInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CIM_CASH_UNIT_INFO:\r\n";

            XfsCimDefine.WFSCIMCASHINFO_dim lpCashInfo;

            System.Int32 hRet = imp.WFS_INF_CIM_CASH_UNIT_INFO_Impl(out lpCashInfo);
            s += "WFS_INF_CIM_CASH_UNIT_INFO return :" + hRet + "\r\n";

            if (XfsGlobalDefine.WFS_SUCCESS != hRet)
            {
                Trace(s);
                return;
            }
            s += "\tusCount = " + lpCashInfo.usCount + "\r\n";
            s += "\tlppCashIn:\r\n";
            string strTmp = "";
            foreach (CIM.WFSCIMCASHIN_dim pos in lpCashInfo.lppCashInDim)
            {
                s += "\t\tusNumber = " + pos.usNumber + "\r\n";
                s += "\t\tfwType = " + pos.fwType + "\r\n";
                s += "\t\tfwItemType = " + pos.fwItemType + "\r\n";
                foreach (SByte v in pos.cUnitID)
                {
                    if (Convert.ToChar(v) != '\0')
                    {
                        strTmp += Convert.ToChar(v);
                    }
                }
                s += "\t\tcUnitID = " + strTmp + "\r\n";
                strTmp = "";

                foreach (SByte v in pos.cCurrencyID)
                {
                    if (Convert.ToChar(v) != '\0')
                    {
                        strTmp += Convert.ToChar(v);
                    }
                }
                s += "\t\tcCurrencyID = " + strTmp + "\r\n";
                strTmp = "";

                s += "\t\tulValues = " + pos.ulValues + "\r\n";
                s += "\t\tulCashInCount = " + pos.ulCashInCount + "\r\n";
                s += "\t\tulCount = " + pos.ulCount + "\r\n";
                s += "\t\tulMaximum = " + pos.ulMaximum + "\r\n";
                s += "\t\tusStatus = " + pos.usStatus + "\r\n";
                s += "\t\tbAppLock = " + pos.bAppLock + "\r\n";
                s += "\t\tlpNoteNumberList:\r\n";
                //lpNoteNumberList
                s += "\t\t\tusNumOfNoteNumbers = " + pos.lpNoteNumberListDim.usNumOfNoteNumbers + "\r\n";
                
                foreach(CIM.WFSCIMNOTENUMBER v in pos.lpNoteNumberListDim.lppNoteNumberDim)
                {
                    s += "\t\t\t\tusNoteID = " + v.usNoteID + "\r\n";
                    s += "\t\t\t\tulCount = " + v.ulCount + "\r\n";
                }
                s += "\t\tusNumPhysicalCUs = " + pos.usNumPhysicalCUs + "\r\n";
                s += "\t\tlppPhysical:\r\n";

                foreach (CIM.WFSCIMPHCU v in pos.lppPhysicalDim)
                {
                    s += "\t\t\tlpPhysicalPositionName = ";
                    if (IntPtr.Zero.Equals(v.lpPhysicalPositionName) == false)
                    {
                        for (Int32 i = 0; true; i++)
                        {
                            System.Byte b1 = Marshal.ReadByte(v.lpPhysicalPositionName, i);
                            if (b1 == 0 )
                                break;
                            s += Convert.ToChar(b1);
                        }

                        s += "\r\n";
                        s += "\t\t\tcUnitID = ";
                        foreach (SByte v1 in v.cUnitID)
                        {
                            if (Convert.ToChar(v1) != '\0')
                            {
                                s += Convert.ToChar(v1);
                            }
                            
                        }
                        s += "\r\n";

                        s += "\t\t\tulCashInCount = " + v.ulCashInCount + "\r\n";
                        s += "\t\t\tulCount = " + v.ulCount + "\r\n";
                        s += "\t\t\tulMaximum = " + v.ulMaximum + "\r\n";
                        s += "\t\t\tusPStatus = " + v.usPStatus + "\r\n";
                        s += "\t\t\tbHardwareSensors = " + v.bHardwareSensors + "\r\n";
                        s += "\t\t\tlpszExtra:\r\n";
                        if (IntPtr.Zero.Equals(pos.lpszExtra) == false)
                        {
                            for (Int32 i = 0; true; i++)
                            {
                                System.Byte b1 = Marshal.ReadByte(pos.lpszExtra, i);
                                System.Byte b2 = Marshal.ReadByte(pos.lpszExtra, i + 1);
                                if (b1 == 0 && b2 == 0)
                                    break;
                                if (b1 == 0)
                                    s += "\r\n";
                                if (Convert.ToChar(b1) != '\0')
                                {
                                    s += Convert.ToChar(b1);
                                }
                                
                            }
                            s += "\r\n";
                        }
                    }                    
                }

                s += "\t\tlpszExtra:\r\n";
                if (IntPtr.Zero.Equals(pos.lpszExtra) == false)
                {
                    for (Int32 i = 0; true; i++)
                    {
                        System.Byte b1 = Marshal.ReadByte(pos.lpszExtra, i);
                        System.Byte b2 = Marshal.ReadByte(pos.lpszExtra, i + 1);
                        if (b1 == 0 && b2 == 0)
                            break;
                        if (b1 == 0)
                            s += "\r\n";
                        s += Convert.ToChar(b1);
                    }
                }
                s += "\r\n";
            }
            
            Trace(s);
        }

        private void tellerInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_TELLER_INFO:！\r\n";
            s += "Not Implemented\r\n";
            Trace(s);
        }

        private void currencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CIM_CURRENCY_EXP:\r\n";

            List<XfsCimDefine.WFSCIMCURRENCYEXP> lpCurrencyExp;

            System.Int32 hRet = imp.WFS_INF_CIM_CURRENCY_EXP_Impl(out lpCurrencyExp);
            s += "WFS_INF_CIM_CURRENCY_EXP return :" + hRet + "\r\n";
            
            String strTmp = "";
            if (hRet == XfsGlobalDefine.WFS_SUCCESS)
            {
                foreach (XfsCimDefine.WFSCIMCURRENCYEXP CurrencyExp in lpCurrencyExp)
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
            
            Trace(s);
        }

        private void cashinStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CIM_CASH_IN_STATUS:\r\n";

            XfsCimDefine.WFSCIMCASHINSTATUS_dim lpCashInStatus;

            System.Int32 hRet = imp.WFS_INF_CIM_CASH_IN_STATUS_Impl(out lpCashInStatus);
            s += "WFS_INF_CIM_CASH_IN_STATUS return :" + hRet + "\r\n";

            s += "\twStatus = " + lpCashInStatus.wStatus + "\r\n";
            s += "\tusNumOfRefused = " + lpCashInStatus.usNumOfRefused + "\r\n";
            s += "\tlpNoteNumberList:\r\n";
            s += "\t\tusNumOfNoteNumbers = " + lpCashInStatus.lpNoteNumberListDim.usNumOfNoteNumbers + ":\r\n";
            s += "\t\tlppNoteNumber:\r\n";
            foreach(CIM.WFSCIMNOTENUMBER v in lpCashInStatus.lpNoteNumberListDim.lppNoteNumberDim)
            {
                s += "\t\t\tusNoteID = " + v.usNoteID + "\r\n";
                s += "\t\t\tulCount = " + v.ulCount + "\r\n";
            }

            s += "\tlpszExtra:\r\n";
            if (IntPtr.Zero.Equals(lpCashInStatus.lpszExtra) == false)
            {
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
            s += "\r\n";
            
            Trace(s);
        }

        private void getP6InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_INF_CIM_GET_P6_INFO:\r\n";
            s += "Not Implemented\r\n";
            Trace(s);
        }

        private void getP6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String s = "WFS_INF_CIM_GET_P6_SIGNATURE:\r\n";
            s += "Not Implemented\r\n";
            Trace(s);
        }

        #endregion
        
        #region CASHIN
        private void cashInStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_CASH_IN_START:\r\n";
            try
            {
                CIM.WFSCIMCASHINSTART CashInStart = new CIM.WFSCIMCASHINSTART();
                CashInStart.usTellerID = 0;
                CashInStart.bUseRecycleUnits = 1;
                CashInStart.fwOutputPosition = CIM.WFS_CIM_POSNULL;
                CashInStart.fwInputPosition = CIM.WFS_CIM_POSNULL;

                int hRet = imp.WFS_CMD_CIM_CASH_IN_START_Impl(CashInStart);
                s += "WFS_CMD_CIM_CASH_IN_START return " + hRet + "\r\n";

                if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                    throw new KTR10_CIM.Exceptions.GeneralException(hRet);
            }
            catch (KTR10_CIM.Exceptions.BaseException ex)
            {
                if (ex.IsXfsGeneralError)
                {
                    s += "\t\tGeneral Error " + ex.XfsGeneralError + "\r\n";
                }
                else if (ex.IsXfsCIMError)
                {
                    s += "\t\tCIM Error " + ex.XfsCIMError + "\r\n";
                }
                else
                {
                    s += "\t\tOther Error " + ex.Message + "\r\n";
                }
            }
            catch (Exception ex)
            {
                s += "\t\tError " + ex.Message + "\r\n";
            }
            Trace(s);
        }

        private void cashInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_CASH_IN:\r\n";
            try
            {
                int hRet = imp.WFS_CMD_CIM_CASH_IN_Impl();
                s += "WFS_CMD_CIM_CASH_IN return " + hRet + "\r\n";

                if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                    throw new KTR10_CIM.Exceptions.GeneralException(hRet);
            }
            catch (KTR10_CIM.Exceptions.BaseException ex)
            {
                if (ex.IsXfsGeneralError)
                {
                    s += "\t\tGeneral Error " + ex.XfsGeneralError + "\r\n";
                }
                else if (ex.IsXfsCIMError)
                {
                    s += "\t\tCIM Error " + ex.XfsCIMError + "\r\n";
                }
                else
                {
                    s += "\t\tOther Error " + ex.Message + "\r\n";
                }
            }
            catch (Exception ex)
            {
                s += "\t\tError " + ex.Message + "\r\n";
            }
            Trace(s);
        }

        private void cashInEndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_CASH_IN_END:\r\n";
            try
            {
                int hRet = imp.WFS_CMD_CIM_CASH_IN_END_Impl();
                s += "WFS_CMD_CIM_CASH_IN_END return " + hRet + "\r\n";

                if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                    throw new KTR10_CIM.Exceptions.GeneralException(hRet);
            }
            catch (KTR10_CIM.Exceptions.BaseException ex)
            {
                if (ex.IsXfsGeneralError)
                {
                    s += "\t\tGeneral Error " + ex.XfsGeneralError + "\r\n";
                }
                else if (ex.IsXfsCIMError)
                {
                    s += "\t\tCIM Error " + ex.XfsCIMError + "\r\n";
                }
                else
                {
                    s += "\t\tOther Error " + ex.Message + "\r\n";
                }
            }
            catch (Exception ex)
            {
                s += "\t\tError " + ex.Message + "\r\n";
            }
            Trace(s);
        }

        private void cashInRollBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_CASH_IN_ROLLBACK:\r\n";
            try
            {
                CIM.WFSCIMCASHINFO_dim cashInfo;
                int hRet = imp.WFS_CMD_CIM_CASH_IN_ROLLBACK_Impl(out cashInfo);
                s += "WFS_CMD_CIM_CASH_IN_ROLLBACK return " + hRet + "\r\n";

                if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                    throw new KTR10_CIM.Exceptions.GeneralException(hRet);
            }
            catch (KTR10_CIM.Exceptions.BaseException ex)
            {
                if (ex.IsXfsGeneralError)
                {
                    s += "\t\tGeneral Error " + ex.XfsGeneralError + "\r\n";
                }
                else if (ex.IsXfsCIMError)
                {
                    s += "\t\tCIM Error " + ex.XfsCIMError + "\r\n";
                }
                else
                {
                    s += "\t\tOther Error " + ex.Message + "\r\n";
                }
            }
            catch (Exception ex)
            {
                s += "\t\tError " + ex.Message + "\r\n";
            }
            Trace(s);
        }

        #endregion

        #region ACTION

        private void retractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_RETRACT:\r\n";
            try
            {
                CIM.WFSCIMRETRACT retract = new CIM.WFSCIMRETRACT();
                retract.fwOutputPosition = CIM.WFS_CIM_POSNULL;
                retract.usRetractArea = CIM.WFS_CIM_RA_RETRACT;
                retract.usIndex = 0;
                int hRet = imp.WFS_CMD_CIM_RETRACT_Impl(retract);
                s += "WFS_CMD_CIM_RETRACT return " + hRet + "\r\n";

                if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                    throw new KTR10_CIM.Exceptions.GeneralException(hRet);
            }
            catch (KTR10_CIM.Exceptions.BaseException ex)
            {
                if (ex.IsXfsGeneralError)
                {
                    s += "\t\tGeneral Error " + ex.XfsGeneralError + "\r\n";
                }
                else if (ex.IsXfsCIMError)
                {
                    s += "\t\tCIM Error " + ex.XfsCIMError + "\r\n";
                }
                else
                {
                    s += "\t\tOther Error " + ex.Message + "\r\n";
                }
            }
            catch (Exception ex)
            {
                s += "\t\tError " + ex.Message + "\r\n";
            }
            Trace(s);
        }
        
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_RESET\r\n";
            try
            {
                CIM.WFSCIMITEMPOSITION_dim lpItemPos = new CIM.WFSCIMITEMPOSITION_dim();
                lpItemPos.usNumber = 0;
                lpItemPos.fwOutputPosition = 0;

                lpItemPos.lpRetractAreaDim.fwOutputPosition = CIM.WFS_CIM_POSNULL;
                lpItemPos.lpRetractAreaDim.usRetractArea = CIM.WFS_CIM_RA_RETRACT;
                lpItemPos.lpRetractAreaDim.usIndex = 0;

                System.Int32 hRet = imp.WFS_CMD_CIM_RESET_Impl(lpItemPos);
                Console.WriteLine("\r\nWFS_CMD_CIM_RESET_Impl return  ", hRet);
                s += "\r\nWFS_CMD_CIM_RESET return  " + hRet + "\r\n";

                if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                    throw new KTR10_CIM.Exceptions.GeneralException(hRet);
            }
            catch (KTR10_CIM.Exceptions.BaseException ex)
            {
                if (ex.IsXfsGeneralError)
                {
                    s += "\t\tGeneral Error " + ex.XfsGeneralError + "\r\n";
                }
                else if (ex.IsXfsCIMError)
                {
                    s += "\t\tCIM Error " + ex.XfsCIMError + "\r\n";
                }
                else
                {
                    s += "\t\tOther Error " + ex.Message + "\r\n";
                }
            }
            catch (Exception ex)
            {
                s += "\t\tError " + ex.Message + "\r\n";
            }
            Trace(s);
        }

        private void settellerinfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_SET_TELLER_INFO;\r\n";
            s += "Not Implemented\r\n";
           
            Trace(s);
        }

        private void setcashunitinfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_SET_CASH_UNIT_INFO\r\n";
            s += "Not Implemented\r\n";

            Trace(s);
            return;
        }

        private void startexchangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_START_EXCHANGE\r\n";
            s += "Not Implemented\r\n";
            Trace(s);
            return;
        }

        private void endexchangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_END_EXCHANGE";
            s += "Not Implemented\r\n";
            Trace(s);
            return;
        }

        private void opensafedoorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_OPEN_SAFE_DOOR:！\r\n";
            s += "Not Implemented\r\n";

            Trace(s);
        }

        private void wFSCMDCIMCONFIGURECASHINUNITSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_CONFIGURE_CASH_IN_UNITS:\r\n";
            s += "Not Implemented\r\n";

            Trace(s);
        }

        private void configurenotetypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_CONFIGURE_NOTETYPES:\r\n";
            s += "Not Implemented\r\n";
            Trace(s);
            return;
        }

        private void createp6signatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "WFS_CMD_CIM_CREATE_P6_SIGNATURE:！\r\n";
            s += "Not Implemented\r\n";

            Trace(s);
        }

        #endregion
        
        public int CIMEvent(long eventId, KT.WOSA.CIM.XfsGlobalDefine.WFSRESULT WFSResult)
        {
            string s = "Event:\r\n";
            switch (eventId)
            {
                case XfsGlobalDefine.WFS_EXECUTE_EVENT:
                    s += "WFS_EXECUTE_EVENT" + eventId + "\r\n";
                    ManagerExeEvent(WFSResult);
                    break;
                case XfsGlobalDefine.WFS_SERVICE_EVENT:
                    s += "WFS_SERVICE_EVENT" + eventId + "\r\n";
                    ManagerSvrEvent(WFSResult);
                    break;
                case XfsGlobalDefine.WFS_USER_EVENT:
                    s += "WFS_USER_EVENT" + eventId + "\r\n";
                    ManagerUserEvent(WFSResult);
                    break;
                case XfsGlobalDefine.WFS_SYSTEM_EVENT:
                    s += "WFS_SYSTEM_EVENT" + eventId + "\r\n";
                    ManagerSysEvent(WFSResult);
                    break;
                case XfsGlobalDefine.WFS_TIMER_EVENT:
                    s += "WFS_TIMER_EVENT" + eventId + "\r\n";

                    break;
                case XfsGlobalDefine.WFS_OPEN_COMPLETE:
                    s += "WFS_OPEN_COMPLETE" + eventId + "\r\n";
                    
                    break;
                case XfsGlobalDefine.WFS_CLOSE_COMPLETE:
                    s += "WFS_CLOSE_COMPLETE" + eventId + "\r\n";
                    
                    break;
                case XfsGlobalDefine.WFS_LOCK_COMPLETE:
                    s += "WFS_LOCK_COMPLETE(" + eventId + ")\r\n";
                    if (WFSResult.hResult == XfsApiPInvoke.WFS_SUCCESS)
                    {
                        s += "\tLocked\r\n";
                    }
                    else if (WFSResult.hResult == XfsApiPInvoke.WFS_ERR_TIMEOUT)
                    {
                        s += "\tLockTimeout\r\n";
                    }
                    break;
                case XfsGlobalDefine.WFS_UNLOCK_COMPLETE:
                    s += "WFS_UNLOCK_COMPLETE" + eventId + "\r\n";
                    s += "Unlocked\r\n";
                    break;
                case XfsGlobalDefine.WFS_EXECUTE_COMPLETE:
                    s += "WFS_EXECUTE_COMPLETE" + eventId + "\r\n";

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
                case CIM.WFS_EXEE_CIM_INPUTREFUSE:
                    s += "Event:WFS_EXEE_CIM_INPUTREFUSE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;
                case CIM.WFS_EXEE_CIM_INFO_AVAILABLE:
                    s += "Event:WFS_EXEE_CIM_INFO_AVAILABLE(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;
                case CIM.WFS_EXEE_CIM_CASHUNITERROR:
                    s += "Event:WFS_EXEE_CIM_CASHUNITERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;
                case CIM.WFS_EXEE_CIM_NOTEERROR:
                    s += "Event:WFS_EXEE_CIM_NOTEERROR(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;
            }

            Trace(s);
        }

        public void ManagerSvrEvent(XfsGlobalDefine.WFSRESULT WFSResult)
        {
            string s = "";
            switch (WFSResult.dwCmdCodeOrEventID)
            {
                case CIM.WFS_SRVE_CIM_ITEMSINSERTED:
                    s += "Event:WFS_SRVE_CIM_ITEMSINSERTED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    
                    break;
                case CIM.WFS_SRVE_CIM_ITEMSTAKEN:
                    s += "Event:WFS_SRVE_CIM_ITEMSTAKEN(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    
                    break;
                case CIM.WFS_SRVE_CIM_CASHUNITINFOCHANGED:
                    s += "Event:WFS_SRVE_CIM_CASHUNITINFOCHANGED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    //20151014做到此处
                    CIM.WFSCIMCASHIN cashUnit = (CIM.WFSCIMCASHIN)Marshal.PtrToStructure(WFSResult.lpBuffer, typeof(CIM.WFSCIMCASHIN));
                    
                    CIM.WFSCIMCASHIN_dim cashInfoPosDim = new CIM.WFSCIMCASHIN_dim();
                    cashInfoPosDim.lpNoteNumberListDim.lppNoteNumberDim = new List<CIM.WFSCIMNOTENUMBER>();
                    cashInfoPosDim.Copy(cashUnit);

                    IntPtr pAddressVal = IntPtr.Zero;
                    CIM.WFSCIMNOTENUMBERLIST noteNumberListPos = (CIM.WFSCIMNOTENUMBERLIST)Marshal.PtrToStructure(cashUnit.lpNoteNumberList, typeof(CIM.WFSCIMNOTENUMBERLIST));
                    cashInfoPosDim.lpNoteNumberListDim.Copy(noteNumberListPos);
                    for (int k = 0; k < noteNumberListPos.usNumOfNoteNumbers; k++)
                    {
                        pAddressVal = Marshal.ReadIntPtr((IntPtr)((int)noteNumberListPos.lppNoteNumber + IntPtr.Size * k), 0);
                        CIM.WFSCIMNOTENUMBER noteNumberPos = (CIM.WFSCIMNOTENUMBER)Marshal.PtrToStructure(pAddressVal, typeof(CIM.WFSCIMNOTENUMBER));
                        cashInfoPosDim.lpNoteNumberListDim.lppNoteNumberDim.Add(noteNumberPos);
                    }

                    cashInfoPosDim.lppPhysicalDim = new List<CIM.WFSCIMPHCU>();

                    for (int j = 0; j < cashUnit.usNumPhysicalCUs; j++)
                    {
                        pAddressVal = Marshal.ReadIntPtr((IntPtr)((int)cashUnit.lppPhysical + IntPtr.Size * j), 0);
                        CIM.WFSCIMPHCU phCuPos = (CIM.WFSCIMPHCU)Marshal.PtrToStructure(pAddressVal, typeof(CIM.WFSCIMPHCU));
                        cashInfoPosDim.lppPhysicalDim.Add(phCuPos);
                    }

                    string strTmp = "";
                    s += "\t\tusNumber = " + cashInfoPosDim.usNumber + "\r\n";
                    s += "\t\tfwType = " + cashInfoPosDim.fwType + "\r\n";
                    s += "\t\tfwItemType = " + cashInfoPosDim.fwItemType + "\r\n";
                    foreach (SByte v in cashInfoPosDim.cUnitID)
                    {
                        if (Convert.ToChar(v) != '\0')
                        {
                            strTmp += Convert.ToChar(v);
                        }
                    }
                    s += "\t\tcUnitID = " + strTmp + "\r\n";
                    strTmp = "";

                    foreach (SByte v in cashInfoPosDim.cCurrencyID)
                    {
                        if (Convert.ToChar(v) != '\0')
                        {
                            strTmp += Convert.ToChar(v);
                        }
                    }
                    s += "\t\tcCurrencyID = " + strTmp + "\r\n";
                    strTmp = "";

                    s += "\t\tulValues = " + cashInfoPosDim.ulValues + "\r\n";
                    s += "\t\tulCashInCount = " + cashInfoPosDim.ulCashInCount + "\r\n";
                    s += "\t\tulCount = " + cashInfoPosDim.ulCount + "\r\n";
                    s += "\t\tulMaximum = " + cashInfoPosDim.ulMaximum + "\r\n";
                    s += "\t\tusStatus = " + cashInfoPosDim.usStatus + "\r\n";
                    s += "\t\tbAppLock = " + cashInfoPosDim.bAppLock + "\r\n";
                    s += "\t\tlpNoteNumberList:\r\n";
                    //lpNoteNumberList
                    s += "\t\t\tusNumOfNoteNumbers = " + cashInfoPosDim.lpNoteNumberListDim.usNumOfNoteNumbers + "\r\n";

                    foreach (CIM.WFSCIMNOTENUMBER v in cashInfoPosDim.lpNoteNumberListDim.lppNoteNumberDim)
                    {
                        s += "\t\t\t\tusNoteID = " + v.usNoteID + "\r\n";
                        s += "\t\t\t\tulCount = " + v.ulCount + "\r\n";
                    }
                    s += "\t\tusNumPhysicalCUs = " + cashInfoPosDim.usNumPhysicalCUs + "\r\n";
                    s += "\t\tlppPhysical:\r\n";

                    foreach (CIM.WFSCIMPHCU v in cashInfoPosDim.lppPhysicalDim)
                    {
                        s += "\t\t\tlpPhysicalPositionName = ";
                        if (IntPtr.Zero.Equals(v.lpPhysicalPositionName) == false)
                        {
                            for (Int32 i = 0; true; i++)
                            {
                                System.Byte b1 = Marshal.ReadByte(v.lpPhysicalPositionName, i);
                                if (b1 == 0)
                                    break;
                                s += Convert.ToChar(b1);
                            }

                            s += "\r\n";
                            s += "\t\t\tcUnitID = ";
                            foreach (SByte v1 in v.cUnitID)
                            {
                                if (Convert.ToChar(v1) != '\0')
                                {
                                    s += Convert.ToChar(v1);
                                }

                            }
                            s += "\r\n";

                            s += "\t\t\tulCashInCount = " + v.ulCashInCount + "\r\n";
                            s += "\t\t\tulCount = " + v.ulCount + "\r\n";
                            s += "\t\t\tulMaximum = " + v.ulMaximum + "\r\n";
                            s += "\t\t\tusPStatus = " + v.usPStatus + "\r\n";
                            s += "\t\t\tbHardwareSensors = " + v.bHardwareSensors + "\r\n";
                            s += "\t\t\tlpszExtra:\r\n";
                            if (IntPtr.Zero.Equals(cashInfoPosDim.lpszExtra) == false)
                            {
                                for (Int32 i = 0; true; i++)
                                {
                                    System.Byte b1 = Marshal.ReadByte(cashInfoPosDim.lpszExtra, i);
                                    System.Byte b2 = Marshal.ReadByte(cashInfoPosDim.lpszExtra, i + 1);
                                    if (b1 == 0 && b2 == 0)
                                        break;
                                    if (b1 == 0)
                                        s += "\r\n";
                                    if (Convert.ToChar(b1) != '\0')
                                    {
                                        s += Convert.ToChar(b1);
                                    }
                                }
                                s += "\r\n";
                            }
                        }
                    }

                    s += "\t\tlpszExtra:\r\n";
                    if (IntPtr.Zero.Equals(cashInfoPosDim.lpszExtra) == false)
                    {
                        for (Int32 i = 0; true; i++)
                        {
                            System.Byte b1 = Marshal.ReadByte(cashInfoPosDim.lpszExtra, i);
                            System.Byte b2 = Marshal.ReadByte(cashInfoPosDim.lpszExtra, i + 1);
                            if (b1 == 0 && b2 == 0)
                                break;
                            if (b1 == 0)
                                s += "\r\n";
                            s += Convert.ToChar(b1);
                        }
                    }
                    s += "\r\n";
                    
                    break;
                case CIM.WFS_SRVE_CIM_ITEMSPRESENTED:
                    s += "Event:WFS_SRVE_CIM_ITEMSPRESENTED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;
                case CIM.WFS_SRVE_CIM_COUNTS_CHANGED:
                    s += "Event:WFS_SRVE_CIM_COUNTS_CHANGED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";

                    break;
                case CIM.WFS_SRVE_CIM_MEDIADETECTED:
                    s += "Event:WFS_SRVE_CIM_MEDIADETECTED(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    break;
            }
            Trace(s);
        }

        public void ManagerUserEvent(XfsGlobalDefine.WFSRESULT WFSResult)
        {
            string s = "";
            switch (WFSResult.dwCmdCodeOrEventID)
            {
                case CIM.WFS_USRE_CIM_CASHUNITTHRESHOLD:
                    s += "Event:WFS_USRE_CIM_CASHUNITTHRESHOLD(" + WFSResult.dwCmdCodeOrEventID + ")\r\n";
                    CIM.WFSCIMCASHIN cashUnit = (CIM.WFSCIMCASHIN)Marshal.PtrToStructure(WFSResult.lpBuffer, typeof(CIM.WFSCIMCASHIN));
                    
                    CIM.WFSCIMCASHIN_dim cashInfoPosDim = new CIM.WFSCIMCASHIN_dim();
                    cashInfoPosDim.lpNoteNumberListDim.lppNoteNumberDim = new List<CIM.WFSCIMNOTENUMBER>();
                    cashInfoPosDim.Copy(cashUnit);

                    IntPtr pAddressVal = IntPtr.Zero;
                    CIM.WFSCIMNOTENUMBERLIST noteNumberListPos = (CIM.WFSCIMNOTENUMBERLIST)Marshal.PtrToStructure(cashUnit.lpNoteNumberList, typeof(CIM.WFSCIMNOTENUMBERLIST));
                    cashInfoPosDim.lpNoteNumberListDim.Copy(noteNumberListPos);
                    for (int k = 0; k < noteNumberListPos.usNumOfNoteNumbers; k++)
                    {
                        pAddressVal = Marshal.ReadIntPtr((IntPtr)((int)noteNumberListPos.lppNoteNumber + IntPtr.Size * k), 0);
                        CIM.WFSCIMNOTENUMBER noteNumberPos = (CIM.WFSCIMNOTENUMBER)Marshal.PtrToStructure(pAddressVal, typeof(CIM.WFSCIMNOTENUMBER));
                        cashInfoPosDim.lpNoteNumberListDim.lppNoteNumberDim.Add(noteNumberPos);
                    }

                    cashInfoPosDim.lppPhysicalDim = new List<CIM.WFSCIMPHCU>();

                    for (int j = 0; j < cashUnit.usNumPhysicalCUs; j++)
                    {
                        pAddressVal = Marshal.ReadIntPtr((IntPtr)((int)cashUnit.lppPhysical + IntPtr.Size * j), 0);
                        CIM.WFSCIMPHCU phCuPos = (CIM.WFSCIMPHCU)Marshal.PtrToStructure(pAddressVal, typeof(CIM.WFSCIMPHCU));
                        cashInfoPosDim.lppPhysicalDim.Add(phCuPos);
                    }

                    string strTmp = "";
                    s += "\t\tusNumber = " + cashInfoPosDim.usNumber + "\r\n";
                    s += "\t\tfwType = " + cashInfoPosDim.fwType + "\r\n";
                    s += "\t\tfwItemType = " + cashInfoPosDim.fwItemType + "\r\n";
                    foreach (SByte v in cashInfoPosDim.cUnitID)
                    {
                        if (Convert.ToChar(v) != '\0')
                        {
                            strTmp += Convert.ToChar(v);
                        }
                    }
                    s += "\t\tcUnitID = " + strTmp + "\r\n";
                    strTmp = "";

                    foreach (SByte v in cashInfoPosDim.cCurrencyID)
                    {
                        if (Convert.ToChar(v) != '\0')
                        {
                            strTmp += Convert.ToChar(v);
                        }
                    }
                    s += "\t\tcCurrencyID = " + strTmp + "\r\n";
                    strTmp = "";

                    s += "\t\tulValues = " + cashInfoPosDim.ulValues + "\r\n";
                    s += "\t\tulCashInCount = " + cashInfoPosDim.ulCashInCount + "\r\n";
                    s += "\t\tulCount = " + cashInfoPosDim.ulCount + "\r\n";
                    s += "\t\tulMaximum = " + cashInfoPosDim.ulMaximum + "\r\n";
                    s += "\t\tusStatus = " + cashInfoPosDim.usStatus + "\r\n";
                    s += "\t\tbAppLock = " + cashInfoPosDim.bAppLock + "\r\n";
                    s += "\t\tlpNoteNumberList:\r\n";
                    //lpNoteNumberList
                    s += "\t\t\tusNumOfNoteNumbers = " + cashInfoPosDim.lpNoteNumberListDim.usNumOfNoteNumbers + "\r\n";

                    foreach (CIM.WFSCIMNOTENUMBER v in cashInfoPosDim.lpNoteNumberListDim.lppNoteNumberDim)
                    {
                        s += "\t\t\t\tusNoteID = " + v.usNoteID + "\r\n";
                        s += "\t\t\t\tulCount = " + v.ulCount + "\r\n";
                    }
                    s += "\t\tusNumPhysicalCUs = " + cashInfoPosDim.usNumPhysicalCUs + "\r\n";
                    s += "\t\tlppPhysical:\r\n";

                    foreach (CIM.WFSCIMPHCU v in cashInfoPosDim.lppPhysicalDim)
                    {
                        s += "\t\t\tlpPhysicalPositionName = ";
                        if (IntPtr.Zero.Equals(v.lpPhysicalPositionName) == false)
                        {
                            for (Int32 i = 0; true; i++)
                            {
                                System.Byte b1 = Marshal.ReadByte(v.lpPhysicalPositionName, i);
                                if (b1 == 0)
                                    break;
                                s += Convert.ToChar(b1);
                            }

                            s += "\r\n";
                            s += "\t\t\tcUnitID = ";
                            foreach (SByte v1 in v.cUnitID)
                            {
                                if (Convert.ToChar(v1) != '\0')
                                {
                                    s += Convert.ToChar(v1);
                                }

                            }
                            s += "\r\n";

                            s += "\t\t\tulCashInCount = " + v.ulCashInCount + "\r\n";
                            s += "\t\t\tulCount = " + v.ulCount + "\r\n";
                            s += "\t\t\tulMaximum = " + v.ulMaximum + "\r\n";
                            s += "\t\t\tusPStatus = " + v.usPStatus + "\r\n";
                            s += "\t\t\tbHardwareSensors = " + v.bHardwareSensors + "\r\n";
                            s += "\t\t\tlpszExtra:\r\n";
                            if (IntPtr.Zero.Equals(cashInfoPosDim.lpszExtra) == false)
                            {
                                for (Int32 i = 0; true; i++)
                                {
                                    System.Byte b1 = Marshal.ReadByte(cashInfoPosDim.lpszExtra, i);
                                    System.Byte b2 = Marshal.ReadByte(cashInfoPosDim.lpszExtra, i + 1);
                                    if (b1 == 0 && b2 == 0)
                                        break;
                                    if (b1 == 0)
                                        s += "\r\n";
                                    if (Convert.ToChar(b1) != '\0')
                                    {
                                        s += Convert.ToChar(b1);
                                    }

                                }
                                s += "\r\n";
                            }
                        }

                    }

                    s += "\t\tlpszExtra:\r\n";
                    if (IntPtr.Zero.Equals(cashInfoPosDim.lpszExtra) == false)
                    {
                        for (Int32 i = 0; true; i++)
                        {
                            System.Byte b1 = Marshal.ReadByte(cashInfoPosDim.lpszExtra, i);
                            System.Byte b2 = Marshal.ReadByte(cashInfoPosDim.lpszExtra, i + 1);
                            if (b1 == 0 && b2 == 0)
                                break;
                            if (b1 == 0)
                                s += "\r\n";
                            s += Convert.ToChar(b1);
                        }
                    }
                    s += "\r\n";
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainUIForm));
            this.tbTrace = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cIMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRegisterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSINFCIMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cAPABILITIESToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bANKNOTETYPESToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cASHUNITINFOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tELLERINFOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cURRENCYTOOLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cASHINSTATUSTOOLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gETP6INFOTOOLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gETP6TOOLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cASHINToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.cASHINSTARTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cASHINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cASHINENDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cASHINROLLBACKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wFSCIMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rETRACTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rESETToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sETTELLERINFOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sETCASHUNITINFOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sTARTEXCHANGEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eNDEXCHANGEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oPENSAFEDOORToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cONFIGURECASHUNITsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cONFIGURENOTETYPESToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cREATEP6SIGNATUREToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shutterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openshutterStripMenuItemClickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cLOSESHUTTERToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbTrace
            // 
            this.tbTrace.Location = new System.Drawing.Point(12, 27);
            this.tbTrace.Multiline = true;
            this.tbTrace.Name = "tbTrace";
            this.tbTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTrace.Size = new System.Drawing.Size(772, 364);
            this.tbTrace.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripMenuItem,
            this.cIMToolStripMenuItem,
            this.wFSINFCIMToolStripMenuItem,
            this.cASHINToolStripMenuItem3,
            this.wFSCIMToolStripMenuItem,
            this.shutterToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(796, 33);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mainToolStripMenuItem
            // 
            this.mainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.mainToolStripMenuItem.Name = "mainToolStripMenuItem";
            this.mainToolStripMenuItem.Size = new System.Drawing.Size(63, 29);
            this.mainToolStripMenuItem.Text = "Main";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(120, 30);
            this.clearToolStripMenuItem.Text = "clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(120, 30);
            this.exitToolStripMenuItem.Text = "exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // cIMToolStripMenuItem
            // 
            this.cIMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openRegisterToolStripMenuItem,
            this.statusToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.cIMToolStripMenuItem.Name = "cIMToolStripMenuItem";
            this.cIMToolStripMenuItem.Size = new System.Drawing.Size(56, 29);
            this.cIMToolStripMenuItem.Text = "CIM";
            // 
            // openRegisterToolStripMenuItem
            // 
            this.openRegisterToolStripMenuItem.Name = "openRegisterToolStripMenuItem";
            this.openRegisterToolStripMenuItem.Size = new System.Drawing.Size(189, 30);
            this.openRegisterToolStripMenuItem.Text = "openRegister";
            this.openRegisterToolStripMenuItem.Click += new System.EventHandler(this.openRegisterToolStripMenuItem_Click);
            // 
            // statusToolStripMenuItem
            // 
            this.statusToolStripMenuItem.Name = "statusToolStripMenuItem";
            this.statusToolStripMenuItem.Size = new System.Drawing.Size(189, 30);
            this.statusToolStripMenuItem.Text = "status";
            this.statusToolStripMenuItem.Click += new System.EventHandler(this.statusToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(189, 30);
            this.closeToolStripMenuItem.Text = "close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // wFSINFCIMToolStripMenuItem
            // 
            this.wFSINFCIMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cAPABILITIESToolStripMenuItem,
            this.bANKNOTETYPESToolStripMenuItem,
            this.cASHUNITINFOToolStripMenuItem,
            this.tELLERINFOToolStripMenuItem,
            this.cURRENCYTOOLToolStripMenuItem,
            this.cASHINSTATUSTOOLToolStripMenuItem,
            this.gETP6INFOTOOLToolStripMenuItem,
            this.gETP6TOOLToolStripMenuItem});
            this.wFSINFCIMToolStripMenuItem.Name = "wFSINFCIMToolStripMenuItem";
            this.wFSINFCIMToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.wFSINFCIMToolStripMenuItem.Text = "INFO";
            this.wFSINFCIMToolStripMenuItem.Click += new System.EventHandler(this.bankNoteTypesToolStripMenuItem_Click);
            // 
            // cAPABILITIESToolStripMenuItem
            // 
            this.cAPABILITIESToolStripMenuItem.Name = "cAPABILITIESToolStripMenuItem";
            this.cAPABILITIESToolStripMenuItem.Size = new System.Drawing.Size(227, 30);
            this.cAPABILITIESToolStripMenuItem.Text = "CAPABILITIES";
            this.cAPABILITIESToolStripMenuItem.Click += new System.EventHandler(this.capabilitiesToolStripMenuItem_Click);
            // 
            // bANKNOTETYPESToolStripMenuItem
            // 
            this.bANKNOTETYPESToolStripMenuItem.Name = "bANKNOTETYPESToolStripMenuItem";
            this.bANKNOTETYPESToolStripMenuItem.Size = new System.Drawing.Size(227, 30);
            this.bANKNOTETYPESToolStripMenuItem.Text = "BANKNOTE TYPES";
            this.bANKNOTETYPESToolStripMenuItem.Click += new System.EventHandler(this.bankNoteTypesToolStripMenuItem_Click);
            // 
            // cASHUNITINFOToolStripMenuItem
            // 
            this.cASHUNITINFOToolStripMenuItem.Name = "cASHUNITINFOToolStripMenuItem";
            this.cASHUNITINFOToolStripMenuItem.Size = new System.Drawing.Size(227, 30);
            this.cASHUNITINFOToolStripMenuItem.Text = "CASHUNIT INFO";
            this.cASHUNITINFOToolStripMenuItem.Click += new System.EventHandler(this.cashUnitInfoToolStripMenuItem_Click);
            // 
            // tELLERINFOToolStripMenuItem
            // 
            this.tELLERINFOToolStripMenuItem.Name = "tELLERINFOToolStripMenuItem";
            this.tELLERINFOToolStripMenuItem.Size = new System.Drawing.Size(227, 30);
            this.tELLERINFOToolStripMenuItem.Text = "TELLER INFO";
            this.tELLERINFOToolStripMenuItem.Click += new System.EventHandler(this.tellerInfoToolStripMenuItem_Click);
            // 
            // cURRENCYTOOLToolStripMenuItem
            // 
            this.cURRENCYTOOLToolStripMenuItem.Name = "cURRENCYTOOLToolStripMenuItem";
            this.cURRENCYTOOLToolStripMenuItem.Size = new System.Drawing.Size(227, 30);
            this.cURRENCYTOOLToolStripMenuItem.Text = "CURRENCY";
            this.cURRENCYTOOLToolStripMenuItem.Click += new System.EventHandler(this.currencyToolStripMenuItem_Click);
            // 
            // cASHINSTATUSTOOLToolStripMenuItem
            // 
            this.cASHINSTATUSTOOLToolStripMenuItem.Name = "cASHINSTATUSTOOLToolStripMenuItem";
            this.cASHINSTATUSTOOLToolStripMenuItem.Size = new System.Drawing.Size(227, 30);
            this.cASHINSTATUSTOOLToolStripMenuItem.Text = "CASHIN STATUS";
            this.cASHINSTATUSTOOLToolStripMenuItem.Click += new System.EventHandler(this.cashinStatusToolStripMenuItem_Click);
            // 
            // gETP6INFOTOOLToolStripMenuItem
            // 
            this.gETP6INFOTOOLToolStripMenuItem.Name = "gETP6INFOTOOLToolStripMenuItem";
            this.gETP6INFOTOOLToolStripMenuItem.Size = new System.Drawing.Size(227, 30);
            this.gETP6INFOTOOLToolStripMenuItem.Text = "GET P6 INFO";
            this.gETP6INFOTOOLToolStripMenuItem.Click += new System.EventHandler(this.getP6InfoToolStripMenuItem_Click);
            // 
            // gETP6TOOLToolStripMenuItem
            // 
            this.gETP6TOOLToolStripMenuItem.Name = "gETP6TOOLToolStripMenuItem";
            this.gETP6TOOLToolStripMenuItem.Size = new System.Drawing.Size(227, 30);
            this.gETP6TOOLToolStripMenuItem.Text = "GET P6";
            this.gETP6TOOLToolStripMenuItem.Click += new System.EventHandler(this.getP6ToolStripMenuItem_Click);
            // 
            // cASHINToolStripMenuItem3
            // 
            this.cASHINToolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cASHINSTARTToolStripMenuItem,
            this.cASHINToolStripMenuItem,
            this.cASHINENDToolStripMenuItem,
            this.cASHINROLLBACKToolStripMenuItem});
            this.cASHINToolStripMenuItem3.Name = "cASHINToolStripMenuItem3";
            this.cASHINToolStripMenuItem3.Size = new System.Drawing.Size(93, 29);
            this.cASHINToolStripMenuItem3.Text = "CASH IN";
            // 
            // cASHINSTARTToolStripMenuItem
            // 
            this.cASHINSTARTToolStripMenuItem.Name = "cASHINSTARTToolStripMenuItem";
            this.cASHINSTARTToolStripMenuItem.Size = new System.Drawing.Size(242, 30);
            this.cASHINSTARTToolStripMenuItem.Text = "CASH IN START";
            this.cASHINSTARTToolStripMenuItem.Click += new System.EventHandler(this.cashInStartToolStripMenuItem_Click);
            // 
            // cASHINToolStripMenuItem
            // 
            this.cASHINToolStripMenuItem.Name = "cASHINToolStripMenuItem";
            this.cASHINToolStripMenuItem.Size = new System.Drawing.Size(242, 30);
            this.cASHINToolStripMenuItem.Text = "CASH IN";
            this.cASHINToolStripMenuItem.Click += new System.EventHandler(this.cashInToolStripMenuItem_Click);
            // 
            // cASHINENDToolStripMenuItem
            // 
            this.cASHINENDToolStripMenuItem.Name = "cASHINENDToolStripMenuItem";
            this.cASHINENDToolStripMenuItem.Size = new System.Drawing.Size(242, 30);
            this.cASHINENDToolStripMenuItem.Text = "CASH IN END";
            this.cASHINENDToolStripMenuItem.Click += new System.EventHandler(this.cashInEndToolStripMenuItem_Click);
            // 
            // cASHINROLLBACKToolStripMenuItem
            // 
            this.cASHINROLLBACKToolStripMenuItem.Name = "cASHINROLLBACKToolStripMenuItem";
            this.cASHINROLLBACKToolStripMenuItem.Size = new System.Drawing.Size(242, 30);
            this.cASHINROLLBACKToolStripMenuItem.Text = "CASH IN ROLLBACK";
            this.cASHINROLLBACKToolStripMenuItem.Click += new System.EventHandler(this.cashInRollBackToolStripMenuItem_Click);
            // 
            // wFSCIMToolStripMenuItem
            // 
            this.wFSCIMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rETRACTToolStripMenuItem,
            this.rESETToolStripMenuItem,
            this.sETTELLERINFOToolStripMenuItem,
            this.sETCASHUNITINFOToolStripMenuItem,
            this.sTARTEXCHANGEToolStripMenuItem,
            this.eNDEXCHANGEToolStripMenuItem,
            this.oPENSAFEDOORToolStripMenuItem,
            this.cONFIGURECASHUNITsToolStripMenuItem,
            this.cONFIGURENOTETYPESToolStripMenuItem,
            this.cREATEP6SIGNATUREToolStripMenuItem});
            this.wFSCIMToolStripMenuItem.Name = "wFSCIMToolStripMenuItem";
            this.wFSCIMToolStripMenuItem.Size = new System.Drawing.Size(88, 29);
            this.wFSCIMToolStripMenuItem.Text = "ACTION";
            // 
            // rETRACTToolStripMenuItem
            // 
            this.rETRACTToolStripMenuItem.Name = "rETRACTToolStripMenuItem";
            this.rETRACTToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.rETRACTToolStripMenuItem.Text = "RETRACT";
            this.rETRACTToolStripMenuItem.Click += new System.EventHandler(this.retractToolStripMenuItem_Click);
            // 
            // rESETToolStripMenuItem
            // 
            this.rESETToolStripMenuItem.Name = "rESETToolStripMenuItem";
            this.rESETToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.rESETToolStripMenuItem.Text = "RESET";
            this.rESETToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // sETTELLERINFOToolStripMenuItem
            // 
            this.sETTELLERINFOToolStripMenuItem.Name = "sETTELLERINFOToolStripMenuItem";
            this.sETTELLERINFOToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.sETTELLERINFOToolStripMenuItem.Text = "SET TELLER INFO";
            this.sETTELLERINFOToolStripMenuItem.Click += new System.EventHandler(this.settellerinfoToolStripMenuItem_Click);
            // 
            // sETCASHUNITINFOToolStripMenuItem
            // 
            this.sETCASHUNITINFOToolStripMenuItem.Name = "sETCASHUNITINFOToolStripMenuItem";
            this.sETCASHUNITINFOToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.sETCASHUNITINFOToolStripMenuItem.Text = "SET CASH UNIT INFO";
            this.sETCASHUNITINFOToolStripMenuItem.Click += new System.EventHandler(this.setcashunitinfoToolStripMenuItem_Click);
            // 
            // sTARTEXCHANGEToolStripMenuItem
            // 
            this.sTARTEXCHANGEToolStripMenuItem.Name = "sTARTEXCHANGEToolStripMenuItem";
            this.sTARTEXCHANGEToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.sTARTEXCHANGEToolStripMenuItem.Text = "START EXCHANGE";
            this.sTARTEXCHANGEToolStripMenuItem.Click += new System.EventHandler(this.startexchangeToolStripMenuItem_Click);
            // 
            // eNDEXCHANGEToolStripMenuItem
            // 
            this.eNDEXCHANGEToolStripMenuItem.Name = "eNDEXCHANGEToolStripMenuItem";
            this.eNDEXCHANGEToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.eNDEXCHANGEToolStripMenuItem.Text = "END EXCHANGE";
            this.eNDEXCHANGEToolStripMenuItem.Click += new System.EventHandler(this.endexchangeToolStripMenuItem_Click);
            // 
            // oPENSAFEDOORToolStripMenuItem
            // 
            this.oPENSAFEDOORToolStripMenuItem.Name = "oPENSAFEDOORToolStripMenuItem";
            this.oPENSAFEDOORToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.oPENSAFEDOORToolStripMenuItem.Text = "OPEN SAFE DOOR";
            this.oPENSAFEDOORToolStripMenuItem.Click += new System.EventHandler(this.opensafedoorToolStripMenuItem_Click);
            // 
            // cONFIGURECASHUNITsToolStripMenuItem
            // 
            this.cONFIGURECASHUNITsToolStripMenuItem.Name = "cONFIGURECASHUNITsToolStripMenuItem";
            this.cONFIGURECASHUNITsToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.cONFIGURECASHUNITsToolStripMenuItem.Text = "CONFIGURE CASH UNITs";
            this.cONFIGURECASHUNITsToolStripMenuItem.Click += new System.EventHandler(this.wFSCMDCIMCONFIGURECASHINUNITSToolStripMenuItem_Click);
            // 
            // cONFIGURENOTETYPESToolStripMenuItem
            // 
            this.cONFIGURENOTETYPESToolStripMenuItem.Name = "cONFIGURENOTETYPESToolStripMenuItem";
            this.cONFIGURENOTETYPESToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.cONFIGURENOTETYPESToolStripMenuItem.Text = "CONFIGURE NOTE TYPES";
            this.cONFIGURENOTETYPESToolStripMenuItem.Click += new System.EventHandler(this.configurenotetypesToolStripMenuItem_Click);
            // 
            // cREATEP6SIGNATUREToolStripMenuItem
            // 
            this.cREATEP6SIGNATUREToolStripMenuItem.Name = "cREATEP6SIGNATUREToolStripMenuItem";
            this.cREATEP6SIGNATUREToolStripMenuItem.Size = new System.Drawing.Size(283, 30);
            this.cREATEP6SIGNATUREToolStripMenuItem.Text = "CREATE P6 SIGNATURE";
            this.cREATEP6SIGNATUREToolStripMenuItem.Click += new System.EventHandler(this.createp6signatureToolStripMenuItem_Click);
            // 
            // shutterToolStripMenuItem
            // 
            this.shutterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openshutterStripMenuItemClickToolStripMenuItem,
            this.cLOSESHUTTERToolStripMenuItem});
            this.shutterToolStripMenuItem.Name = "shutterToolStripMenuItem";
            this.shutterToolStripMenuItem.Size = new System.Drawing.Size(97, 29);
            this.shutterToolStripMenuItem.Text = "SHUTTER";
            // 
            // openshutterStripMenuItemClickToolStripMenuItem
            // 
            this.openshutterStripMenuItemClickToolStripMenuItem.Name = "openshutterStripMenuItemClickToolStripMenuItem";
            this.openshutterStripMenuItemClickToolStripMenuItem.Size = new System.Drawing.Size(136, 30);
            this.openshutterStripMenuItemClickToolStripMenuItem.Text = "OPEN";
            this.openshutterStripMenuItemClickToolStripMenuItem.Click += new System.EventHandler(this.openshutterStripMenuItem_Click);
            // 
            // cLOSESHUTTERToolStripMenuItem
            // 
            this.cLOSESHUTTERToolStripMenuItem.Name = "cLOSESHUTTERToolStripMenuItem";
            this.cLOSESHUTTERToolStripMenuItem.Size = new System.Drawing.Size(136, 30);
            this.cLOSESHUTTERToolStripMenuItem.Text = "CLOSE";
            this.cLOSESHUTTERToolStripMenuItem.Click += new System.EventHandler(this.closeshutterToolStripMenuItem_Click);
            // 
            // MainUIForm
            // 
            this.ClientSize = new System.Drawing.Size(796, 403);
            this.Controls.Add(this.tbTrace);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainUIForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cash ‌In";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainUIForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #region Helpers
        private void Trace(string strInfo)
        {
            try
            {
                DateTime dt = DateTime.Now;
                System.String sTime = "";

                sTime += dt.Hour.ToString("D02") + ":" + dt.Minute.ToString("D02") + ":" + dt.Second.ToString("D02") + "." + dt.Millisecond.ToString("D03");
                sTime = "[ " + sTime + " ]";

                CrossThreadUI.SetText(tbTrace, sTime + "\r\n" + strInfo + "\r\n", true);
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

        public void ScrollToCurrent()
        {
            tbTrace.Select(tbTrace.Text.Length, 0);
            tbTrace.ScrollToCaret();
        }

        #endregion
    }
}
