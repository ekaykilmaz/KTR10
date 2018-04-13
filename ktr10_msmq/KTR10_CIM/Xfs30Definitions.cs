using System;
using System.Collections.Generic;
using KT.WOSA.CIM;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KTR10_CIM.Xfs30Definitions
{
    public class CIM
    {        
        /// <summary>
        /// values of WFSCIMCAPS.wClass
        /// </summary>
        public const int WFS_SERVICE_CLASS_CIM = 13;

        /// <summary>
        /// values of WFSCIMCAPS.wClass
        /// </summary>
        public const int WFS_SERVICE_CLASS_VERSION_CIM = 0x0003;

        /// <summary>
        /// values of WFSCIMCAPS.wClass
        /// </summary>
        public const string WFS_SERVICE_CLASS_NAME_CIM = "CIM";

        /// <summary>
        /// values of WFSCIMCAPS.wClass
        /// #define CIM_SERVICE_OFFSET (WFS_SERVICE_CLASS_CIM * 100)
        /// </summary>
        public const int CIM_SERVICE_OFFSET = 1300;

        /// <summary>
        /// CIM Info Commands
        /// </summary>
        public enum CIMInfoCommands
        {
            WFS_INF_CIM_STATUS = 1301,
            WFS_INF_CIM_CAPABILITIES = 1302,
            WFS_INF_CIM_CASH_UNIT_INFO = 1303,
            WFS_INF_CIM_TELLER_INFO = 1304,
            WFS_INF_CIM_CURRENCY_EXP = 1305,
            WFS_INF_CIM_BANKNOTE_TYPES = 1306,
            WFS_INF_CIM_CASH_IN_STATUS = 1307
        }

        /// <summary>
        /// CIM Execute Commands
        /// </summary>
        public enum CIMExecuteCommands
        {
            WFS_CMD_CIM_CASH_IN_START = 1301,
            WFS_CMD_CIM_CASH_IN = 1302,
            WFS_CMD_CIM_CASH_IN_END = 1303,
            WFS_CMD_CIM_CASH_IN_ROLLBACK = 1304,
            WFS_CMD_CIM_RETRACT = 1305,
            WFS_CMD_CIM_OPEN_SHUTTER = 1306,
            WFS_CMD_CIM_CLOSE_SHUTTER = 1307,
            WFS_CMD_CIM_SET_TELLER_INFO = 1308,
            WFS_CMD_CIM_SET_CASH_UNIT_INFO = 1309,
            WFS_CMD_CIM_START_EXCHANGE = 1310,
            WFS_CMD_CIM_END_EXCHANGE = 1311,
            WFS_CMD_CIM_OPEN_SAFE_DOOR = 1312,
            WFS_CMD_CIM_RESET = 1313,
            WFS_CMD_CIM_CONFIGURE_CASH_IN_UNITS = 1314,
            WFS_CMD_CIM_CONFIGURE_NOTETYPES = 1315
        }

        /// <summary>
        /// CIM Messages
        /// </summary>
        public enum CIMMessages
        {
            WFS_SRVE_CIM_SAFEDOOROPEN = 1301,
            WFS_SRVE_CIM_SAFEDOORCLOSED = 1302,
            WFS_USRE_CIM_CASHUNITTHRESHOLD = 1303,
            WFS_SRVE_CIM_CASHUNITINFOCHANGED = 1304,
            WFS_SRVE_CIM_TELLERINFOCHANGED = 1305,
            WFS_EXEE_CIM_CASHUNITERROR = 1306,
            WFS_SRVE_CIM_ITEMSTAKEN = 1307,
            WFS_SRVE_CIM_COUNTS_CHANGED = 1308,
            WFS_EXEE_CIM_INPUTREFUSE = 1309,
            WFS_SRVE_CIM_ITEMSPRESENTED = 1310,
            WFS_SRVE_CIM_ITEMSINSERTED = 1311,
            WFS_EXEE_CIM_NOTEERROR = 1312,
            WFS_EXEE_CIM_SUBCASHIN = 1313,
            WFS_SRVE_CIM_MEDIADETECTED = 1314
        }

        /// <summary>
        /// values of WFSCIMSTATUS.fwDevice
        /// </summary>
        public enum CIMStatusDevice
        {
            WFS_STAT_DEVONLINE = 0,
            WFS_STAT_DEVOFFLINE = 1,
            WFS_STAT_DEVPOWEROFF = 2,
            WFS_STAT_DEVNODEVICE = 3,
            WFS_STAT_DEVHWERROR = 4,
            WFS_STAT_DEVUSERERROR = 5,
            WFS_STAT_DEVBUSY = 6
        }

        /// <summary>
        /// values of WFSCIMSTATUS.fwSafeDoor
        /// </summary>
        public enum CIMSTATUSSafeDoor
        {
            WFS_CIM_DOORNOTSUPPORTED = 1,
            WFS_CIM_DOOROPEN = 2,
            WFS_CIM_DOORCLOSED = 3,
            WFS_CIM_DOORUNKNOWN = 4
        }

        /// <summary>
        /// values of WFSCIMSTATUS.fwAcceptor
        /// </summary>
        public enum CIMSTATUSAcceptor
        {
            WFS_CIM_ACCOK = 0,
            WFS_CIM_ACCCUSTATE = 1,
            WFS_CIM_ACCCUSTOP = 2,
            WFS_CIM_ACCCUUNKNOWN = 3
        }

        /// <summary>
        /// values of WFSCIMSTATUS.fwIntermediateStacker
        /// </summary>
        public enum CIMSTATUSIntermediateStacker
        {
            WFS_CIM_ISEMPTY = 0,
            WFS_CIM_ISNOTEMPTY = 1,
            WFS_CIM_ISFULL = 2,
            WFS_CIM_ISUNKNOWN = 4,
            WFS_CIM_ISNOTSUPPORTED = 5
        }

        /// <summary>
        /// values of WFSCIMSTATUS.fwStackerItems
        /// </summary>
        public enum CIMSTATUSStackerItems
        {
            WFS_CIM_CUSTOMERACCESS = 0,
            WFS_CIM_NOCUSTOMERACCESS = 1,
            WFS_CIM_ACCESSUNKNOWN = 2,
            WFS_CIM_NOITEMS = 4,
        }

        /// <summary>
        /// values of WFSCIMSTATUS.fwBankNoteReader
        /// </summary>
        public enum CIMSTATUSBankNoteReader
        {
            WFS_CIM_BNROK = 0,
            WFS_CIM_BNRINOP = 1,
            WFS_CIM_BNRUNKNOWN = 2,
            WFS_CIM_BNRNOTSUPPORTED = 3
        }

        /// <summary>
        /// values of WFSCIMSTATUS.fwShutter
        /// </summary>
        public enum CIMSTATUSShutter
        {
            WFS_CIM_SHTCLOSED = 0,
            WFS_CIM_SHTOPEN = 1,
            WFS_CIM_SHTJAMMED = 2,
            WFS_CIM_SHTUNKNOWN = 3,
            WFS_CIM_SHTNOTSUPPORTED = 4
        }

        /// <summary>
        /// values of WFSCIMINPOS.fwPositionStatus
        /// </summary>
        public enum CIMINPOSPositionStatus
        {
            WFS_CIM_PSEMPTY = 0,
            WFS_CIM_PSNOTEMPTY = 1,
            WFS_CIM_PSUNKNOWN = 2,
            WFS_CIM_PSNOTSUPPORTED = 3
        }

        /// <summary>
        /// values of WFSCIMSTATUS.fwTransport
        /// </summary>
        public enum CIMSTATUSTransport
        {
            WFS_CIM_TPOK = 0,
            WFS_CIM_TPINOP = 1,
            WFS_CIM_TPUNKNOWN = 2,
            WFS_CIM_TPNOTSUPPORTED = 3
        }

        /// <summary>
        /// values of WFSCIMINPOS.fwTransportStatus
        /// </summary>
        public enum CIMINPOSTransportStatus
        {
            WFS_CIM_TPSTATEMPTY = 0,
            WFS_CIM_TPSTATNOTEMPTY = 1,
            WFS_CIM_TPSTATNOTEMPTYCUST = 2,
            WFS_CIM_TPSTATNOTEMPTY_UNK = 3,
            WFS_CIM_TPSTATNOTSUPPORTED = 4
        }

        /// <summary>
        /// values of WFSCIMCAPS.fwType
        /// </summary>
        public enum CIMCAPSType
        {
            WFS_CIM_TELLERBILL = 0,
            WFS_CIM_SELFSERVICEBILL = 1,
            WFS_CIM_TELLERCOIN = 2,
            WFS_CIM_SELFSERVICECOIN = 3
        }

        /// <summary>
        /// values of WFSCIMCAPS.fwExchangeType
        /// values of WFSCIMSTARTEX.fwExchangeType
        /// </summary>
        public enum CIMCAPSExchangeType
        {
            WFS_CIM_EXBYHAND = 0x0001,
            WFS_CIM_EXTOCASSETTES = 0x0002,
            WFS_CIM_CLEARRECYCLER = 0x0004,
            WFS_CIM_DEPOSITINTO = 0x0008
        }

        /// <summary>
        /// values of WFSCIMCAPS.fwRetractTransportActions
        /// values of WFSCIMCAPS.fwRetractStackerActions
        /// </summary>
        public enum CIMCAPSRetractTransportActions
        {
            WFS_CIM_PRESENT = 0x0001,
            WFS_CIM_RETRACT = 0x0002,
            WFS_CIM_NOTSUPP = 0x0004
        }

        /// <summary>
        /// values of WFSCIMCASHIN.fwType
        /// </summary>
        public enum CIMCASHINType
        {
            WFS_CIM_TYPERECYCLING = 1,
            WFS_CIM_TYPECASHIN = 2,
            WFS_CIM_TYPEREPCONTAINER = 3,
            WFS_CIM_TYPERETRACTCASSETTE = 4
        }

        /// <summary>
        /// values of WFSCIMCASHIN.fwItemType
        /// values of WFSCIMCASHINTYPE.dwType
        /// </summary>
        public enum CIMCASHINItemType
        {
            WFS_CIM_CITYPALL = 0x0001,
            WFS_CIM_CITYPUNFIT = 0x0002,
            WFS_CIM_CITYPINDIVIDUAL = 0x0004
        }

        /// <summary>
        /// values of WFSCIMCASHIN.usStatus
        /// values of WFSCIMPHCU.usPStatus
        /// </summary>
        public enum CIMCASHINStatus
        {
            WFS_CIM_STATCUOK = 0,
            WFS_CIM_STATCUFULL = 1,
            WFS_CIM_STATCUHIGH = 2,
            WFS_CIM_STATCULOW = 3,
            WFS_CIM_STATCUEMPTY = 4,
            WFS_CIM_STATCUINOP = 5,
            WFS_CIM_STATCUMISSING = 6,
            WFS_CIM_STATCUNOVAL = 7,
            WFS_CIM_STATCUNOREF = 8,
            WFS_CIM_STATCUMANIP = 9
        }

        public enum CIMAllPositions
        {
            WFS_CIM_POSNULL = 0x0000,
            WFS_CIM_POSINLEFT = 0x0001,
            WFS_CIM_POSINRIGHT = 0x0002,
            WFS_CIM_POSINCENTER = 0x0004,
            WFS_CIM_POSINTOP = 0x0008,
            WFS_CIM_POSINBOTTOM = 0x0010,
            WFS_CIM_POSINFRONT = 0x0020,
            WFS_CIM_POSINREAR = 0x0040,
            WFS_CIM_POSOUTLEFT = 0x0080,
            WFS_CIM_POSOUTRIGHT = 0x0100,
            WFS_CIM_POSOUTCENTER = 0x0200,
            WFS_CIM_POSOUTTOP = 0x0400,
            WFS_CIM_POSOUTBOTTOM = 0x0800,
            WFS_CIM_POSOUTFRONT = 0x1000,
            WFS_CIM_POSOUTREAR = 0x2000
        }

        /// <summary>
        /// values of WFSCIMSTATUS.fwPositions
        /// values of WFSCIMCAPS.fwPositions
        /// values of WFSCIMINPOS.fwPosition
        /// values of WFSCIMTELLERDETAILS.fwInputPosition
        /// values of WFSCIMCASHINSTART.fwInputPosition
        /// </summary>
        public enum CIMCAPSInputPositions
        {
            WFS_CIM_POSNULL = 0x0000,
            WFS_CIM_POSINLEFT = 0x0001,
            WFS_CIM_POSINRIGHT = 0x0002,
            WFS_CIM_POSINCENTER = 0x0004,
            WFS_CIM_POSINTOP = 0x0008,
            WFS_CIM_POSINBOTTOM = 0x0010,
            WFS_CIM_POSINFRONT = 0x0020,
            WFS_CIM_POSINREAR = 0x0040
        }
        
        /// <summary>
        /// values of WFSCIMSTATUS.fwPositions
        /// values of WFSCIMCAPS.fwPositions
        /// values of WFSCIMTELLERDETAILS.fwOutputPosition
        /// values of WFSCIMCASHINSTART.fwOutputPosition
        /// values of WFSCIMOUTPUT.fwPosition
        /// </summary>
        public enum CIMCAPSOutputPositions
        {
            WFS_CIM_POSOUTLEFT = 0x0080,
            WFS_CIM_POSOUTRIGHT = 0x0100,
            WFS_CIM_POSOUTCENTER = 0x0200,
            WFS_CIM_POSOUTTOP = 0x0400,
            WFS_CIM_POSOUTBOTTOM = 0x0800,
            WFS_CIM_POSOUTFRONT = 0x1000,
            WFS_CIM_POSOUTREAR = 0x2000
        }

        /// <summary>
        /// values of WFSCIMCASHINSTATUS.wStatus
        /// </summary>
        public enum CIMCASHINSTATUSStatus
        {
            WFS_CIM_CIOK = 0,
            WFS_CIM_CIROLLBACK = 1,
            WFS_CIM_CIACTIVE = 2,
            WFS_CIM_CIRETRACT = 3,
            WFS_CIM_CIUNKNOWN = 4
        }

        /// <summary>
        /// values of WFSCIMCAPS.fwRetractAreas
        /// values of WFSCIMRETRACT.usRetractArea
        /// </summary>
        public enum CIMCAPSRetractAreas
        {
            WFS_CIM_RA_RETRACT = 0x0001,
            WFS_CIM_RA_TRANSPORT = 0x0002,
            WFS_CIM_RA_STACKER = 0x0004,
            WFS_CIM_RA_BILLCASSETTES = 0x0008,
            WFS_CIM_RA_NOTSUPP = 0x0010
        }

        /// <summary>
        /// values of WFSCIMTELLERUPDATE.usAction
        /// </summary>
        public enum CIMTELLERUPDATEAction
        {
            WFS_CIM_CREATE_TELLER = 1,
            WFS_CIM_MODIFY_TELLER = 2,
            WFS_CIM_DELETE_TELLER = 3
        }

        /// <summary>
        /// values of WFSCIMCUERROR.wFailure
        /// </summary>
        public enum CIMCUERRORFailure
        {
            WFS_CIM_CASHUNITEMPTY = 1,
            WFS_CIM_CASHUNITERROR = 2,
            WFS_CIM_CASHUNITFULL = 3,
            WFS_CIM_CASHUNITLOCKED = 4,
            WFS_CIM_CASHUNITNOTCONF = 5,
            WFS_CIM_CASHUNITINVALID = 6,
            WFS_CIM_CASHUNITCONFIG = 7,
            WFS_CIM_FEEDMODULEPROBLEM = 8
        }

        /// <summary>
        /// values of lpusReason in WFS_EXEE_CIM_INPUTREFUSE
        /// </summary>
        public enum WFS_EXEE_CIM_INPUTREFUSEReason
        {
            WFS_CIM_CASHINUNITFULL = 1,
            WFS_CIM_INVALIDBILL = 2,
            WFS_CIM_NOBILLSTODEPOSIT = 3,
            WFS_CIM_DEPOSITFAILURE = 4,
            WFS_CIM_COMMINPCOMPFAILURE = 5,
            WFS_CIM_STACKERFULL = 6
        }

        /// <summary>
        /// values of lpusReason in WFS_EXEE_CIM_NOTESERROR
        /// </summary>
        public enum WFS_EXEE_CIM_NOTESERRORReason
        {
            WFS_CIM_DOUBLENOTEDETECTED = 1,
            WFS_CIM_LONGNOTEDETECTED = 2,
            WFS_CIM_SKEWEDNOTE = 3,
            WFS_CIM_INCORRECTCOUNT = 4,
            WFS_CIM_NOTESTOOCLOSE = 5
        }

        public enum CIMErrorCodes
        {
            WFS_ERR_CIM_INVALIDCURRENCY = -1300,
            WFS_ERR_CIM_INVALIDTELLERID = -1301,
            WFS_ERR_CIM_CASHUNITERROR = -1302,
            WFS_ERR_CIM_TOOMANYITEMS = -1307,
            WFS_ERR_CIM_UNSUPPOSITION = -1308,
            WFS_ERR_CIM_SAFEDOOROPEN = -1310,
            WFS_ERR_CIM_SHUTTERNOTOPEN = -1312,
            WFS_ERR_CIM_SHUTTEROPEN = -1313,
            WFS_ERR_CIM_SHUTTERCLOSED = -1314,
            WFS_ERR_CIM_INVALIDCASHUNIT = -1315,
            WFS_ERR_CIM_NOITEMS = -1316,
            WFS_ERR_CIM_EXCHANGEACTIVE = -1317,
            WFS_ERR_CIM_NOEXCHANGEACTIVE = -1318,
            WFS_ERR_CIM_SHUTTERNOTCLOSED = -1319,
            WFS_ERR_CIM_ITEMSTAKEN = -1323,
            WFS_ERR_CIM_CASHINACTIVE = -1325,
            WFS_ERR_CIM_NOCASHINACTIVE = -1326,
            WFS_ERR_CIM_POSITION_NOT_EMPTY = -1328,
            WFS_ERR_CIM_INVALIDRETRACTPOSITION = -1334,
            WFS_ERR_CIM_NOTRETRACTAREA = -1335
        }



        public static CIMMessages CIMMessagesConvert(uint input)
        {
            return (CIMMessages)Enum.ToObject(typeof(CIMMessages), Convert.ToInt32(input));
        }

        public static CIMStatusDevice CIMStatusDeviceConvert(string input)
        {
            return (CIMStatusDevice)Enum.ToObject(typeof(CIMStatusDevice), Convert.ToInt32(input));
        }

        public static CIMSTATUSSafeDoor CIMSTATUSSafeDoorConvert(string input)
        {
            return (CIMSTATUSSafeDoor)Enum.ToObject(typeof(CIMSTATUSSafeDoor), Convert.ToInt32(input));
        }

        public static CIMSTATUSAcceptor CIMSTATUSAcceptorConvert(string input)
        {
            return (CIMSTATUSAcceptor)Enum.ToObject(typeof(CIMSTATUSAcceptor), Convert.ToInt32(input));
        }

        public static CIMSTATUSIntermediateStacker CIMSTATUSIntermediateStackerConvert(string input)
        {
            return (CIMSTATUSIntermediateStacker)Enum.ToObject(typeof(CIMSTATUSIntermediateStacker), Convert.ToInt32(input));
        }

        public static CIMSTATUSStackerItems CIMSTATUSStackerItemsConvert(ushort input)
        {
            return (CIMSTATUSStackerItems)Enum.ToObject(typeof(CIMSTATUSStackerItems), Convert.ToInt32(input));
        }

        public static CIMSTATUSBankNoteReader CIMSTATUSBankNoteReaderConvert(ushort input)
        {
            return (CIMSTATUSBankNoteReader)Enum.ToObject(typeof(CIMSTATUSBankNoteReader), Convert.ToInt32(input));
        }

        public static CIMSTATUSShutter CIMSTATUSShutterConvert(ushort input)
        {
            return (CIMSTATUSShutter)Enum.ToObject(typeof(CIMSTATUSShutter), Convert.ToInt32(input));
        }

        public static CIMINPOSPositionStatus CIMINPOSPositionStatusConvert(ushort input)
        {
            return (CIMINPOSPositionStatus)Enum.ToObject(typeof(CIMINPOSPositionStatus), Convert.ToInt32(input));
        }

        public static CIMSTATUSTransport CIMSTATUSTransportConvert(ushort input)
        {
            return (CIMSTATUSTransport)Enum.ToObject(typeof(CIMSTATUSTransport), Convert.ToInt32(input));
        }

        public static CIMINPOSTransportStatus CIMINPOSTransportStatusConvert(ushort input)
        {
            return (CIMINPOSTransportStatus)Enum.ToObject(typeof(CIMINPOSTransportStatus), Convert.ToInt32(input));
        }

        public static CIMCAPSType CIMCAPSTypeConvert(uint input)
        {
            return (CIMCAPSType)Enum.ToObject(typeof(CIMCAPSType), Convert.ToInt32(input));
        }

        public static CIMCAPSExchangeType CIMCAPSExchangeTypeConvert(ushort input)
        {
            return (CIMCAPSExchangeType)Enum.ToObject(typeof(CIMCAPSExchangeType), Convert.ToInt32(input));
        }

        public static CIMCAPSRetractTransportActions CIMCAPSRetractTransportActionsConvert(ushort input)
        {
            return (CIMCAPSRetractTransportActions)Enum.ToObject(typeof(CIMCAPSRetractTransportActions), Convert.ToInt32(input));
        }

        public static CIMCASHINType CIMCASHINTypeConvert(uint input)
        {
            return (CIMCASHINType)Enum.ToObject(typeof(CIMCASHINType), Convert.ToInt32(input));
        }

        public static CIMCASHINItemType CIMCASHINItemTypeConvert(uint input)
        {
            return (CIMCASHINItemType)Enum.ToObject(typeof(CIMCASHINItemType), Convert.ToInt32(input));
        }

        public static CIMCASHINStatus CIMCASHINStatusConvert(string input)
        {
            return (CIMCASHINStatus)Enum.ToObject(typeof(CIMCASHINStatus), Convert.ToInt32(input));
        }
        
        public static CIMAllPositions CIMAllPositionsConvert(ushort input)
        {
            return (CIMAllPositions)Enum.ToObject(typeof(CIMAllPositions), Convert.ToInt32(input));
        }

        public static CIMCAPSInputPositions CIMCAPSInputPositionsConvert(ushort input)
        {
            return (CIMCAPSInputPositions)Enum.ToObject(typeof(CIMCAPSInputPositions), Convert.ToInt32(input));
        }

        public static CIMCAPSOutputPositions CIMCAPSOutputPositionsConvert(ushort input)
        {
            return (CIMCAPSOutputPositions)Enum.ToObject(typeof(CIMCAPSOutputPositions), Convert.ToInt32(input));
        }

        public static CIMCASHINSTATUSStatus CIMCASHINSTATUSStatusConvert(ushort input)
        {
            return (CIMCASHINSTATUSStatus)Enum.ToObject(typeof(CIMCASHINSTATUSStatus), Convert.ToInt32(input));
        }

        public static CIMCAPSRetractAreas CIMCAPSRetractAreasConvert(string input)
        {
            return (CIMCAPSRetractAreas)Enum.ToObject(typeof(CIMCAPSRetractAreas), Convert.ToInt32(input));
        }

        public static CIMTELLERUPDATEAction CIMTELLERUPDATEActionConvert(string input)
        {
            return (CIMTELLERUPDATEAction)Enum.ToObject(typeof(CIMTELLERUPDATEAction), Convert.ToInt32(input));
        }

        public static CIMCUERRORFailure CIMCUERRORFailureConvert(string input)
        {
            return (CIMCUERRORFailure)Enum.ToObject(typeof(CIMCUERRORFailure), Convert.ToInt32(input));
        }

        public static WFS_EXEE_CIM_INPUTREFUSEReason WFS_EXEE_CIM_INPUTREFUSEReasonConvert(string input)
        {
            return (WFS_EXEE_CIM_INPUTREFUSEReason)Enum.ToObject(typeof(WFS_EXEE_CIM_INPUTREFUSEReason), Convert.ToInt32(input));
        }

        public static WFS_EXEE_CIM_NOTESERRORReason WFS_EXEE_CIM_NOTESERRORReasonConvert(string input)
        {
            return (WFS_EXEE_CIM_NOTESERRORReason)Enum.ToObject(typeof(WFS_EXEE_CIM_NOTESERRORReason), Convert.ToInt32(input));
        }

        public static CIMErrorCodes CIMErrorCodesConvert(string input)
        {
            return (CIMErrorCodes)Enum.ToObject(typeof(CIMErrorCodes), Convert.ToInt32(input));
        }
    }

    /// <summary>
    /// KTR10_CDM has the same definiton class
    /// </summary>
    public class General
    {
        /// <summary>
        /// String lengths
        /// </summary>
        public const int WFSDDESCRIPTION_LEN = 256;

        /// <summary>
        /// String lengths
        /// </summary>
        public const int WFSDSYSSTATUS_LEN = 256;

        /// <summary>
        /// Values of WFSDEVSTATUS.fwState
        /// </summary>
        public enum WFSDEVSTATUSState
        {
            WFS_STAT_DEVONLINE = 0,
            WFS_STAT_DEVOFFLINE = 1,
            WFS_STAT_DEVPOWEROFF = 2,
            WFS_STAT_DEVNODEVICE = 3,
            WFS_STAT_DEVHWERROR = 4,
            WFS_STAT_DEVUSERERROR = 5,
            WFS_STAT_DEVBUSY = 6
        }

        /// <summary>
        /// Value of WFS_DEFAULT_HAPP
        /// </summary>
        public const int WFS_DEFAULT_HAPP = 0;

        public enum GeneralErrorCodes
        {
            WFS_SUCCESS = 0,
            WFS_ERR_ALREADY_STARTED = -1,
            WFS_ERR_API_VER_TOO_HIGH = -2,
            WFS_ERR_API_VER_TOO_LOW = -3,
            WFS_ERR_CANCELED = -4,
            WFS_ERR_CFG_INVALID_HKEY = -5,
            WFS_ERR_CFG_INVALID_NAME = -6,
            WFS_ERR_CFG_INVALID_SUBKEY = -7,
            WFS_ERR_CFG_INVALID_VALUE = -8,
            WFS_ERR_CFG_KEY_NOT_EMPTY = -9,
            WFS_ERR_CFG_NAME_TOO_LONG = -10,
            WFS_ERR_CFG_NO_MORE_ITEMS = -11,
            WFS_ERR_CFG_VALUE_TOO_LONG = -12,
            WFS_ERR_DEV_NOT_READY = -13,
            WFS_ERR_HARDWARE_ERROR = -14,
            WFS_ERR_INTERNAL_ERROR = -15,
            WFS_ERR_INVALID_ADDRESS = -16,
            WFS_ERR_INVALID_APP_HANDLE = -17,
            WFS_ERR_INVALID_BUFFER = -18,
            WFS_ERR_INVALID_CATEGORY = -19,
            WFS_ERR_INVALID_COMMAND = -20,
            WFS_ERR_INVALID_EVENT_CLASS = -21,
            WFS_ERR_INVALID_HSERVICE = -22,
            WFS_ERR_INVALID_HPROVIDER = -23,
            WFS_ERR_INVALID_HWND = -24,
            WFS_ERR_INVALID_HWNDREG = -25,
            WFS_ERR_INVALID_POINTER = -26,
            WFS_ERR_INVALID_REQ_ID = -27,
            WFS_ERR_INVALID_RESULT = -28,
            WFS_ERR_INVALID_SERVPROV = -29,
            WFS_ERR_INVALID_TIMER = -30,
            WFS_ERR_INVALID_TRACELEVEL = -31,
            WFS_ERR_LOCKED = -32,
            WFS_ERR_NO_BLOCKING_CALL = -33,
            WFS_ERR_NO_SERVPROV = -34,
            WFS_ERR_NO_SUCH_THREAD = -35,
            WFS_ERR_NO_TIMER = -36,
            WFS_ERR_NOT_LOCKED = -37,
            WFS_ERR_NOT_OK_TO_UNLOAD = -38,
            WFS_ERR_NOT_STARTED = -39,
            WFS_ERR_NOT_REGISTERED = -40,
            WFS_ERR_OP_IN_PROGRESS = -41,
            WFS_ERR_OUT_OF_MEMORY = -42,
            WFS_ERR_SERVICE_NOT_FOUND = -43,
            WFS_ERR_SPI_VER_TOO_HIGH = -44,
            WFS_ERR_SPI_VER_TOO_LOW = -45,
            WFS_ERR_SRVC_VER_TOO_HIGH = -46,
            WFS_ERR_SRVC_VER_TOO_LOW = -47,
            WFS_ERR_TIMEOUT = -48,
            WFS_ERR_UNSUPP_CATEGORY = -49,
            WFS_ERR_UNSUPP_COMMAND = -50,
            WFS_ERR_VERSION_ERROR_IN_SRVC = -51,
            WFS_ERR_INVALID_DATA = -52,
            WFS_ERR_SOFTWARE_ERROR = -53,
            WFS_ERR_CONNECTION_LOST = -54,
            WFS_ERR_USER_ERROR = -55,
            WFS_ERR_UNSUPP_DATA = -56,
        }

        public enum Messages
        {
            WFS_OPEN_COMPLETE = 1,
            WFS_CLOSE_COMPLETE = 2,
            WFS_LOCK_COMPLETE = 3,
            WFS_UNLOCK_COMPLETE = 4,
            WFS_REGISTER_COMPLETE = 5,
            WFS_DEREGISTER_COMPLETE = 6,
            WFS_GETINFO_COMPLETE = 7,
            WFS_EXECUTE_COMPLETE = 8,
            WFS_EXECUTE_EVENT = 20,
            WFS_SERVICE_EVENT = 21,
            WFS_USER_EVENT = 22,
            WFS_SYSTEM_EVENT = 23,
            WFS_TIMER_EVENT = 100
        }

        public enum EventClasses
        {
            SERVICE_EVENTS = 1,
            USER_EVENTS = 2,
            SYSTEM_EVENTS = 4,
            EXECUTE_EVENTS = 8
        }

        public enum SystemEventIDs
        {
            WFS_SYSE_UNDELIVERABLE_MSG = 1,
            WFS_SYSE_HARDWARE_ERROR = 2,
            WFS_SYSE_VERSION_ERROR = 3,
            WFS_SYSE_DEVICE_STATUS = 4,
            WFS_SYSE_APP_DISCONNECT = 5,
            WFS_SYSE_SOFTWARE_ERROR = 6,
            WFS_SYSE_USER_ERROR = 7,
            WFS_SYSE_LOCK_REQUESTED = 8
        }

        public enum XFSTraceLevel
        {
            WFS_TRACE_API = 0x00000001,
            WFS_TRACE_ALL_API = 0x00000002,
            WFS_TRACE_SPI = 0x00000004,
            WFS_TRACE_ALL_SPI = 0x00000008,
            WFS_TRACE_MGR = 0x00000010
        }

        public enum XFSErrorActions
        {
            WFS_ERR_ACT_NOACTION = 0x0000,
            WFS_ERR_ACT_RESET = 0x0001,
            WFS_ERR_ACT_SWERROR = 0x0002,
            WFS_ERR_ACT_CONFIG = 0x0004,
            WFS_ERR_ACT_HWCLEAR = 0x0008,
            WFS_ERR_ACT_HWMAINT = 0x0010,
            WFS_ERR_ACT_SUSPEND = 0x0020
        }

        public static WFSDEVSTATUSState WFSDEVSTATUSStateConvert(string input)
        {
            return (WFSDEVSTATUSState)Enum.ToObject(typeof(WFSDEVSTATUSState), Convert.ToInt32(input));
        }

        public static GeneralErrorCodes GeneralErrorCodesConvert(string input)
        {
            return (GeneralErrorCodes)Enum.ToObject(typeof(GeneralErrorCodes), Convert.ToInt32(input));
        }

        public static Messages MessagesConvert(string input)
        {
            return (Messages)Enum.ToObject(typeof(Messages), Convert.ToInt32(input));
        }

        public static EventClasses EventClassesConvert(string input)
        {
            return (EventClasses)Enum.ToObject(typeof(EventClasses), Convert.ToInt32(input));
        }

        public static SystemEventIDs SystemEventIDsConvert(string input)
        {
            return (SystemEventIDs)Enum.ToObject(typeof(SystemEventIDs), Convert.ToInt32(input));
        }

        public static XFSTraceLevel XFSTraceLevelConvert(string input)
        {
            return (XFSTraceLevel)Enum.ToObject(typeof(XFSTraceLevel), Convert.ToInt32(input));
        }

        public static XFSErrorActions XFSErrorActionsConvert(string input)
        {
            return (XFSErrorActions)Enum.ToObject(typeof(XFSErrorActions), Convert.ToInt32(input));
        }
    }
}