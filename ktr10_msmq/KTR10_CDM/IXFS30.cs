
using KT.WOSA.CDM;
using System.Collections.Generic;

namespace KTR10_CDM
{
    public interface IXfs3
    {
        #region Basic Commands
        void OpenRegister();
        void GetSpInfoVersion(out KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION WFSVersion, out KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION SrvcVersion, out KT.WOSA.CDM.XfsGlobalDefine.WFSVERSION SPIVersion);
        void Close();
        #endregion

        #region Info Commands
        XfsCdmDefine.WFSCDMSTATUS_Dim WFS_INF_CDM_STATUS();
        XfsCdmDefine.WFSCDMCAPS WFS_INF_CDM_CAPABILITIES();
        XfsCdmDefine.WFSCDMCUINFO_Dim WFS_INF_CDM_CASH_UNIT_INFO();
        //void WFS_INF_CDM_TELLER_INFO();
        List<XfsCdmDefine.WFSCDMCURRENCYEXP> WFS_INF_CDM_CURRENCY_EXP();
        List<XfsCdmDefine.WFSCDMMIXTYPE> WFS_INF_CDM_MIX_TYPES();
        XfsCdmDefine.WFSCDMMIXTABLE_Dim WFS_INF_CDM_MIX_TABLE();
        XfsCdmDefine.WFSCDMPRESENTSTATUS_Dim WFS_INF_CDM_PRESENT_STATUS();

        #endregion

        #region Execute Commands
        XfsCdmDefine.WFSCDMDENOMINATION_Dim WFS_CMD_CDM_DENOMINATE(uint amount, string currency, bool log = true);
        XfsCdmDefine.WFSCDMDENOMINATION_Dim WFS_CMD_CDM_DISPENSE(uint amount, string currency);
        //void WFS_CMD_CDM_COUNT();
        void WFS_CMD_CDM_PRESENT();
        void WFS_CMD_CDM_REJECT();
        void WFS_CMD_CDM_RETRACT();
        void WFS_CMD_CDM_OPEN_SHUTTER();
        void WFS_CMD_CDM_CLOSE_SHUTTER();
        //void WFS_CMD_CDM_SET_TELLER_INFO();
        void WFS_CMD_CDM_SET_CASH_UNIT_INFO(XfsCdmDefine.WFSCDMCUINFO_Dim lpCashUnitInfo);
        //XfsCdmDefine.WFSCDMCUINFO_Dim WFS_CMD_CDM_START_EXCHANGE();
        //XfsCdmDefine.WFSCDMCUINFO_Dim WFS_CMD_CDM_END_EXCHANGE();
        //void WFS_CMD_CDM_OPEN_SAFE_DOOR();
        //void WFS_CMD_CDM_CALIBRATE_CASH_UNIT();
        //void WFS_CMD_CDM_SET_MIX_TABLE();
        void WFS_CMD_CDM_RESET();
        //XfsCdmDefine.WFSCDMCUINFO_Dim WFS_CMD_CDM_TEST_CASH_UNITS();
        
        #endregion

        #region Execute Async Commands
        void WFS_CMD_CDM_DENOMINATE_Async(uint amount, string currency);
        void WFS_CMD_CDM_DISPENSE_Async(uint amount, string currency);
        void WFS_CMD_CDM_PRESENT_Async();
        void WFS_CMD_CDM_REJECT_Async();
        void WFS_CMD_CDM_RETRACT_Async();
        void WFS_CMD_CDM_OPEN_SHUTTER_Async();
        void WFS_CMD_CDM_CLOSE_SHUTTER_Async();
        void WFS_CMD_CDM_SET_CASH_UNIT_INFO_Async(XfsCdmDefine.WFSCDMCUINFO_Dim lpCashUnitInfo);
        void WFS_CMD_CDM_RESET_Async();

        #endregion

        #region Events

        //int CDMEvent(long eventId, XfsGlobalDefine.WFSRESULT WFSResult);
        
        //void ManagerExeEvent(XfsGlobalDefine.WFSRESULT WFSResult);

        //void ManagerSvrEvent(XfsGlobalDefine.WFSRESULT WFSResult);

        //void ManagerUserEvent(XfsGlobalDefine.WFSRESULT WFSResult);

        //void ManagerSysEvent(XfsGlobalDefine.WFSRESULT WFSResult);

        //void ManagerExeComplete(XfsGlobalDefine.WFSRESULT WFSResult);


        //WFS_SRVE_CDM_SAFEDOOROPEN
        //WFS_SRVE_CDM_SAFEDOORCLOSED
        //WFS_USRE_CDM_CASHUNITTHRESHOLD
        //WFS_SRVE_CDM_CASHUNITINFOCHANGED
        //WFS_SRVE_CDM_TELLERINFOCHANGED
        //WFS_EXEE_CDM_DELAYEDDISPENSE
        //WFS_EXEE_CDM_STARTDISPENSE
        //WFS_EXEE_CDM_CASHUNITERROR
        //WFS_SRVE_CDM_ITEMSTAKEN
        //WFS_SRVE_CDM_COUNTS_CHANGED
        //WFS_EXEE_CDM_PARTIALDISPENSE
        //WFS_EXEE_CDM_SUBDISPENSEOK
        //WFS_EXEE_CDM_INCOMPLETEDISPENSE
        //WFS_EXEE_CDM_NOTEERROR
        //WFS_SRVE_CDM_ITEMSPRESENTED
        //WFS_SRVE_CDM_MEDIADETECTED 

        #endregion

        #region Property
        ushort CassetteCount { get;  }
        ushort MixNumber { get; set; }
        ushort TellerId { get; set; }
        bool AutoPresent { get; set; }
        bool AutoDenom { get; set; }
        uint UlCashBox { get; set; }

        ushort DispensefwPosition { get; set; }

        ushort ShutterfwPosition { get; set; }

        ushort PresentfwPosition { get; set; }
        
        ushort RetractfwOutputPosition  { get; set; }
        ushort RetractusRetractArea  { get; set; }
        ushort RetractusIndex { get; set; }

        ushort ResetfwOutputPosition { get; set; }
        ushort ResetusNumber { get; set; }
        ushort ResetRetractfwOutputPosition { get; set; }
        ushort ResetRetractusRetractArea { get; set; }
        ushort ResetRetractusIndex { get; set; }

        #endregion
    }
}