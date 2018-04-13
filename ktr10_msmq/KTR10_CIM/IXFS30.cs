
using KT.WOSA.CIM;
using System.Collections.Generic;

namespace KTR10_CIM
{
    public interface IXfs3
    {
        #region Basic Commands
        void OpenRegister();
        void GetSpInfoVersion(out KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION WFSVersion, out KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION SrvcVersion, out KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION SPIVersion);
        void Close();

        #endregion

        #region Info Commands
        XfsCimDefine.WFSCIMSTATUS_dim WFS_INF_CIM_STATUS();
        XfsCimDefine.WFSCIMCAPS WFS_INF_CIM_CAPABILITIES();
        XfsCimDefine.WFSCIMCASHINFO_dim WFS_INF_CIM_CASH_UNIT_INFO();
        //void WFS_INF_CIM_TELLER_INFO();
        List<XfsCimDefine.WFSCIMCURRENCYEXP>  WFS_INF_CIM_CURRENCY_EXP();
        XfsCimDefine.WFSCIMNOTETYPELIST_dim WFS_INF_CIM_BANKNOTE_TYPES();
        XfsCimDefine.WFSCIMCASHINSTATUS_dim WFS_INF_CIM_CASH_IN_STATUS();

        #endregion

        #region Execute Commands
        void WFS_CMD_CIM_CASH_IN_START();
        void WFS_CMD_CIM_CASH_IN();
        void WFS_CMD_CIM_CASH_IN_END();
        XfsCimDefine.WFSCIMCASHINFO_dim WFS_CMD_CIM_CASH_IN_ROLLBACK();

        void WFS_CMD_CIM_RETRACT();
        void WFS_CMD_CIM_OPEN_SHUTTER();
        void WFS_CMD_CIM_CLOSE_SHUTTER();

        //void WFS_CMD_CIM_SET_TELLER_INFO();
        void WFS_CMD_CIM_SET_CASH_UNIT_INFO(XfsCimDefine.WFSCIMCASHINFO_dim lpCashUnitInfo);

        //void WFS_CMD_CIM_START_EXCHANGE();
        //void WFS_CMD_CIM_END_EXCHANGE();

        //void WFS_CMD_CIM_OPEN_SAFE_DOOR();
        void WFS_CMD_CIM_RESET(ushort fwOutputPosition, ushort usNumber, ushort retractfwOutputPosition, ushort retractusRetractArea, ushort retractusIndex);
        //void WFS_CMD_CIM_CONFIGURE_CASH_IN_UNITS();
        //void WFS_CMD_CIM_CONFIGURE_NOTETYPES();

        #endregion

        #region Execute Async Commands
        void WFS_CMD_CIM_CASH_IN_START_Async();
        void WFS_CMD_CIM_CASH_IN_Async();
        void WFS_CMD_CIM_CASH_IN_END_Async();
        void WFS_CMD_CIM_CASH_IN_ROLLBACK_Async();

        void WFS_CMD_CIM_RETRACT_Async();
        void WFS_CMD_CIM_OPEN_SHUTTER_Async();
        void WFS_CMD_CIM_CLOSE_SHUTTER_Async();

        void WFS_CMD_CIM_SET_CASH_UNIT_INFO_Async(XfsCimDefine.WFSCIMCASHINFO_dim lpCashUnitInfo);

        void WFS_CMD_CIM_RESET_Async(ushort fwOutputPosition, ushort usNumber, ushort retractfwOutputPosition, ushort retractusRetractArea, ushort retractusIndex);

        #endregion
                
        #region Property
        ushort TellerId { get; set; }

        int MaxEscrowCapacity { get; }
        int MaxItems { get; }

        int CashInbUseRecycleUnits { get; set; }
        ushort CashInfwOutputPosition { get; set; }
        ushort CashInfwInputPosition { get; set; }

        ushort ShutterfwPosition { get; set; }        

        #endregion
    }
}