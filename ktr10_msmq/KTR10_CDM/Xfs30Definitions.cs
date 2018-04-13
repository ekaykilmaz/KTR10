using System;
using System.Collections.Generic;
using KT.WOSA.CDM;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KTR10_CDM.Xfs30Definitions
{
    public class CDM
    {
        /// <summary>
        /// values of WFSCDMCAPS.wClass
        /// </summary>
        public const int WFS_SERVICE_CLASS_CDM = 3;

        /// <summary>
        /// values of WFSCDMCAPS.wClass
        /// </summary>
        public const int WFS_SERVICE_CLASS_VERSION_CDM = 0x0003;

        /// <summary>
        /// values of WFSCDMCAPS.wClass
        /// </summary>
        public const string WFS_SERVICE_CLASS_NAME_CDM = "CDM";

        /// <summary>
        /// values of WFSCDMCAPS.wClass
        /// #define CDM_SERVICE_OFFSET (WFS_SERVICE_CLASS_CDM * 100)
        /// </summary>
        public const int CDM_SERVICE_OFFSET = 300;

        /// <summary>
        /// CDM Info Commands
        /// </summary>
        public enum CDMInfoCommands
        {
            WFS_INF_CDM_STATUS = 301,
            WFS_INF_CDM_CAPABILITIES = 302,
            WFS_INF_CDM_CASH_UNIT_INFO = 303,
            WFS_INF_CDM_TELLER_INFO = 304,
            WFS_INF_CDM_CURRENCY_EXP = 306,
            WFS_INF_CDM_MIX_TYPES = 307,
            WFS_INF_CDM_MIX_TABLE = 308,
            WFS_INF_CDM_PRESENT_STATUS = 309
        }

        /// <summary>
        /// CDM Execute Commands
        /// </summary>
        public enum CDMExecuteCommands
        {
            WFS_CMD_CDM_DENOMINATE = 301,
            WFS_CMD_CDM_DISPENSE = 302,
            WFS_CMD_CDM_PRESENT = 303,
            WFS_CMD_CDM_REJECT = 304,
            WFS_CMD_CDM_RETRACT = 305,
            WFS_CMD_CDM_OPEN_SHUTTER = 307,
            WFS_CMD_CDM_CLOSE_SHUTTER = 308,
            WFS_CMD_CDM_SET_TELLER_INFO = 309,
            WFS_CMD_CDM_SET_CASH_UNIT_INFO = 310,
            WFS_CMD_CDM_START_EXCHANGE = 311,
            WFS_CMD_CDM_END_EXCHANGE = 312,
            WFS_CMD_CDM_OPEN_SAFE_DOOR = 313,
            WFS_CMD_CDM_CALIBRATE_CASH_UNIT = 315,
            WFS_CMD_CDM_SET_MIX_TABLE = 320,
            WFS_CMD_CDM_RESET = 321,
            WFS_CMD_CDM_TEST_CASH_UNITS = 322,
            WFS_CMD_CDM_COUNT = 323
        }

        /// <summary>
        /// CDM Messages
        /// </summary>
        public enum CDMMessages
        {
            UNKNOWN = 0,
            WFS_SRVE_CDM_SAFEDOOROPEN = 301,
            WFS_SRVE_CDM_SAFEDOORCLOSED = 302,
            WFS_USRE_CDM_CASHUNITTHRESHOLD = 303,
            WFS_SRVE_CDM_CASHUNITINFOCHANGED = 304,
            WFS_SRVE_CDM_TELLERINFOCHANGED = 305,
            WFS_EXEE_CDM_DELAYEDDISPENSE = 306,
            WFS_EXEE_CDM_STARTDISPENSE = 307,
            WFS_EXEE_CDM_CASHUNITERROR = 308,
            WFS_SRVE_CDM_ITEMSTAKEN = 309,
            WFS_EXEE_CDM_PARTIALDISPENSE = 310,
            WFS_EXEE_CDM_SUBDISPENSEOK = 311,
            WFS_SRVE_CDM_ITEMSPRESENTED = 313,
            WFS_SRVE_CDM_COUNTS_CHANGED = 314,
            WFS_EXEE_CDM_INCOMPLETEDISPENSE = 315,
            WFS_EXEE_CDM_NOTEERROR = 316,
            WFS_EXEE_CDM_MEDIADETECTED = 317
        }

        /// <summary>
        /// values of WFSCDMSTATUS.fwDevice
        /// </summary>
        public enum CDMStatusDevice
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
        /// values of WFSCDMSTATUS.fwSafeDoor
        /// </summary>
        public enum CDMStatusSafeDoor
        {
            WFS_CDM_DOORNOTSUPPORTED = 1,
            WFS_CDM_DOOROPEN = 2,
            WFS_CDM_DOORCLOSED = 3,
            WFS_CDM_DOORUNKNOWN = 5
        }

        /// <summary>
        /// values of WFSCDMSTATUS.fwDispenser
        /// </summary>
        public enum CDMStatusDispenser
        {
            WFS_CDM_DISPOK = 0,
            WFS_CDM_DISPCUSTATE = 1,
            WFS_CDM_DISPCUSTOP = 2,
            WFS_CDM_DISPCUUNKNOWN = 3
        }

        /// <summary>
        /// values of WFSCDMSTATUS.fwIntermediateStacker
        /// </summary>
        public enum CDMStatusIntermediateStacker
        {
            WFS_CDM_ISEMPTY = 0,
            WFS_CDM_ISNOTEMPTY = 1,
            WFS_CDM_ISNOTEMPTYCUST = 2,
            WFS_CDM_ISNOTEMPTYUNK = 3,
            WFS_CDM_ISUNKNOWN = 4,
            WFS_CDM_ISNOTSUPPORTED = 5
        }

        /// <summary>
        /// values of WFSCDMOUTPOS.fwShutter
        /// </summary>
        public enum CDMOUTPosShutter
        {
            WFS_CDM_SHTCLOSED = 0,
            WFS_CDM_SHTOPEN = 1,
            WFS_CDM_SHTJAMMED = 2,
            WFS_CDM_SHTUNKNOWN = 3,
            WFS_CDM_SHTNOTSUPPORTED = 4
        }
        
        /// <summary>
        /// values of WFSCDMOUTPOS.fwPositionStatus
        /// </summary>
        public enum CDMOUTPosPositionStatus
        {
            WFS_CDM_PSEMPTY = 0,
            WFS_CDM_PSNOTEMPTY = 1,
            WFS_CDM_PSUNKNOWN = 2,
            WFS_CDM_PSNOTSUPPORTED = 3,
        }

        /// <summary>
        /// values of WFSCDMOUTPOS.fwTransport
        /// </summary>
        public enum CDMOUTPosTransport
        {
            WFS_CDM_TPOK = 0,
            WFS_CDM_TPINOP = 1,
            WFS_CDM_TPUNKNOWN = 2,
            WFS_CDM_TPNOTSUPPORTED = 3,
        }

        /// <summary>
        /// values of WFSCDMOUTPOS.fwTransportStatus
        /// </summary>
        public enum CDMOUTPosTransportStatus
        {
            WFS_CDM_TPSTATEMPTY = 0,
            WFS_CDM_TPSTATNOTEMPTY = 1,
            WFS_CDM_TPSTATNOTEMPTYCUST = 2,
            WFS_CDM_TPSTATNOTEMPTY_UNK = 3,
            WFS_CDM_TPSTATNOTSUPPORTED = 4
        }
        
        /// <summary>
        /// values of WFSCDMCAPS.fwType
        /// </summary>
        public enum CDMCAPSType
        {
            WFS_CDM_TELLERBILL = 0,
            WFS_CDM_SELFSERVICEBILL = 1,
            WFS_CDM_TELLERCOIN = 2,
            WFS_CDM_SELFSERVICECOIN = 3
        }

        /// <summary>
        /// values of WFSCDMCAPS.fwRetractAreas
        /// values of WFSCDMRETRACT.usRetractArea
        /// </summary>
        public enum CDMCAPSRetractArea
        {
            WFS_CDM_RA_RETRACT = 0x0001,
            WFS_CDM_RA_TRANSPORT = 0x0002,
            WFS_CDM_RA_STACKER = 0x0004,
            WFS_CDM_RA_REJECT = 0x0008,
            WFS_CDM_RA_NOTSUPP = 0x0010
        }

        /// <summary>
        /// values of WFSCDMCAPS.fwRetractTransportActions
        /// values of WFSCDMCAPS.fwRetractStackerActions
        /// </summary>
        public enum CDMCAPSRetractActions
        {
            WFS_CDM_PRESENT = 0x0001,
            WFS_CDM_RETRACT = 0x0002,
            WFS_CDM_REJECT = 0x0004,
            WFS_CDM_NOTSUPP = 0x0008
        }

        /// <summary>
        /// values of WFSCDMCAPS.fwMoveItems
        /// </summary>
        public enum CDMCAPSMoveItems
        {
            WFS_CDM_FROMCU = 0x0001,
            WFS_CDM_TOCU = 0x0002,
            WFS_CDM_TOTRANSPORT = 0x0004
        }

        /// <summary>
        /// values of WFSCDMCASHUNIT.usType
        /// </summary>
        public enum CDMCashUnitType
        {
            WFS_CDM_TYPENA = 1,
            WFS_CDM_TYPEREJECTCASSETTE = 2,
            WFS_CDM_TYPEBILLCASSETTE = 3,
            WFS_CDM_TYPECOINCYLINDER = 4,
            WFS_CDM_TYPECOINDISPENSER = 5,
            WFS_CDM_TYPERETRACTCASSETTE = 6,
            WFS_CDM_TYPECOUPON = 7,
            WFS_CDM_TYPEDOCUMENT = 8,
            WFS_CDM_TYPEREPCONTAINER = 11,
            WFS_CDM_TYPERECYCLING = 12
        }

        /// <summary>
        /// values of WFSCDMCASHUNIT.usStatus
        /// </summary>
        public enum CDMCashUnitStatus
        {
            WFS_CDM_STATCUOK = 0,
            WFS_CDM_STATCUFULL = 1,
            WFS_CDM_STATCUHIGH = 2,
            WFS_CDM_STATCULOW = 3,
            WFS_CDM_STATCUEMPTY = 4,
            WFS_CDM_STATCUINOP = 5,
            WFS_CDM_STATCUMISSING = 6,
            WFS_CDM_STATCUNOVAL = 7,
            WFS_CDM_STATCUNOREF = 8,
            WFS_CDM_STATCUMANIP = 9
        }

        /// <summary>
        /// values of WFSCDMMIXTYPE.usMixType
        /// </summary>
        public enum CDMMIXTYPEMixType
        {
            WFS_CDM_MIXALGORITHM = 1,
            WFS_CDM_MIXTABLE = 2
        }

        /// <summary>
        /// values of WFSCDMMIXTYPE.usMixNumber
        /// </summary>
        public enum CDMMIXTYPEMixNumber
        {
            WFS_CDM_INDIVIDUAL = 0,
            //values of WFSCDMMIXTYPE.usSubType (predefined mix algorithms)
            WFS_CDM_MIX_MINIMUM_NUMBER_OF_BILLS = 1,
            WFS_CDM_MIX_EQUAL_EMPTYING_OF_CASH_UNITS = 2,
            WFS_CDM_MIX_MAXIMUM_NUMBER_OF_CASH_UNITS = 3
        }

        /// <summary>
        /// values of WFSCDMPRESENTSTATUS.wPresentState
        /// </summary>
        public enum CDMPRESENTSTATUSPresentState
        {
            WFS_CDM_PRESENTED = 1,
            WFS_CDM_NOTPRESENTED = 2,
            WFS_CDM_UNKNOWN = 3
        }

        /// <summary>
        /// values of WFSCDMDISPENSE.fwPosition
        /// values of WFSCDMCAPS.fwPositions
        /// values of WFSCDMOUTPOS.fwPosition
        /// values of WFSCDMTELLERPOS.fwPosition
        /// values of WFSCDMTELLERDETAILS.fwOutputPosition
        /// values of WFSCDMPHYSICALCU.fwPosition
        /// </summary>
        public enum CDMPosition
        {
            WFS_CDM_POSNULL = 0x0000,
            WFS_CDM_POSLEFT = 0x0001,
            WFS_CDM_POSRIGHT = 0x0002,
            WFS_CDM_POSCENTER = 0x0004,
            WFS_CDM_POSTOP = 0x0040,
            WFS_CDM_POSBOTTOM = 0x0080,
            WFS_CDM_POSREJECT = 0x0100,
            WFS_CDM_POSFRONT = 0x0800,
            WFS_CDM_POSREAR = 0x1000
        }

        /// <summary>
        /// values of WFSCDMTELLERDETAILS.ulInputPosition
        /// </summary>
        public enum CDMTELLERDETAILSInputPosition
        {
            WFS_CDM_POSINLEFT = 0x0001,
            WFS_CDM_POSINRIGHT = 0x0002,
            WFS_CDM_POSINCENTER = 0x0004,
            WFS_CDM_POSINTOP = 0x0008,
            WFS_CDM_POSINBOTTOM = 0x0010,
            WFS_CDM_POSINFRONT = 0x0020,
            WFS_CDM_POSINREAR = 0x0040
        }

        /// <summary>
        /// values of fwExchangeType
        /// </summary>
        public enum ExchangeType
        {
            WFS_CDM_EXBYHAND = 0x0001,
            WFS_CDM_EXTOCASSETTES = 0x0002
        }

        /// <summary>
        /// values of WFSCDMTELLERUPDATE.usAction
        /// </summary>
        public enum CDMTELLERUPDATEAction
        {
            WFS_CDM_CREATE_TELLER = 1,
            WFS_CDM_MODIFY_TELLER = 2,
            WFS_CDM_DELETE_TELLER = 3
        }

        /// <summary>
        /// values of WFSCDMCUERROR.wFailure
        /// </summary>
        public enum CDMCUERRORFailure
        {
            WFS_CDM_CASHUNITEMPTY = 1,
            WFS_CDM_CASHUNITERROR = 2,
            WFS_CDM_CASHUNITFULL = 4,
            WFS_CDM_CASHUNITLOCKED = 5,
            WFS_CDM_CASHUNITINVALID = 6,
            WFS_CDM_CASHUNITCONFIG = 7
        }

        /// <summary>
        /// values of lpusReason in WFS_EXEE_CDM_NOTESERROR
        /// </summary>
        public enum WFS_EXEE_CDM_NOTESERRORReason
        {
            WFS_CDM_DOUBLENOTEDETECTED = 1,
            WFS_CDM_LONGNOTEDETECTED = 2,
            WFS_CDM_SKEWEDNOTE = 3,
            WFS_CDM_INCORRECTCOUNT = 4,
            WFS_CDM_NOTESTOOCLOSE = 5
        }

        /// <summary>
        /// WOSA/XFS CDM Errors
        /// </summary>
        public enum CDMErrorCodes
        {
            WFS_ERR_CDM_INVALIDCURRENCY = -300,
            WFS_ERR_CDM_INVALIDTELLERID = -301,
            WFS_ERR_CDM_CASHUNITERROR = -302,
            WFS_ERR_CDM_INVALIDDENOMINATION = -303,
            WFS_ERR_CDM_INVALIDMIXNUMBER = -304,
            WFS_ERR_CDM_NOCURRENCYMIX = -305,
            WFS_ERR_CDM_NOTDISPENSABLE = -306,
            WFS_ERR_CDM_TOOMANYITEMS = -307,
            WFS_ERR_CDM_UNSUPPOSITION = -308,
            WFS_ERR_CDM_SAFEDOOROPEN = -310,
            WFS_ERR_CDM_SHUTTERNOTOPEN = -312,
            WFS_ERR_CDM_SHUTTEROPEN = -313,
            WFS_ERR_CDM_SHUTTERCLOSED = -314,
            WFS_ERR_CDM_INVALIDCASHUNIT = -315,
            WFS_ERR_CDM_NOITEMS = -316,
            WFS_ERR_CDM_EXCHANGEACTIVE = -317,
            WFS_ERR_CDM_NOEXCHANGEACTIVE = -318,
            WFS_ERR_CDM_SHUTTERNOTCLOSED = -319,
            WFS_ERR_CDM_PRERRORNOITEMS = -320,
            WFS_ERR_CDM_PRERRORITEMS = -321,
            WFS_ERR_CDM_PRERRORUNKNOWN = -322,
            WFS_ERR_CDM_ITEMSTAKEN = -323,
            WFS_ERR_CDM_INVALIDMIXTABLE = -327,
            WFS_ERR_CDM_OUTPUTPOS_NOT_EMPTY = -328,
            WFS_ERR_CDM_INVALIDRETRACTPOSITION = -329,
            WFS_ERR_CDM_NOTRETRACTAREA = -330,
            WFS_ERR_CDM_NOCASHBOXPRESENT = -333,
            WFS_ERR_CDM_AMOUNTNOTINMIXTABLE = -334,
            WFS_ERR_CDM_ITEMSNOTTAKEN = -335,
            WFS_ERR_CDM_ITEMSLEFT = -336
        }

        public static CDMMessages CDMMessagesConvert(uint input)
        {
            return (CDMMessages)Enum.ToObject(typeof(CDMMessages), Convert.ToInt32(input));
        }

        public static CDMStatusDevice CDMStatusDeviceConvert(string input)
        {
            return (CDMStatusDevice)Enum.ToObject(typeof(CDMStatusDevice), Convert.ToInt32(input));
        }

        public static CDMStatusSafeDoor CDMStatusSafeDoorConvert(string input)
        {
            return (CDMStatusSafeDoor)Enum.ToObject(typeof(CDMStatusSafeDoor), Convert.ToInt32(input));
        }

        public static CDMStatusDispenser CDMStatusDispenserConvert(string input)
        {
            return (CDMStatusDispenser)Enum.ToObject(typeof(CDMStatusDispenser), Convert.ToInt32(input));
        }

        public static CDMStatusIntermediateStacker CDMStatusIntermediateStackerConvert(string input)
        {
            return (CDMStatusIntermediateStacker)Enum.ToObject(typeof(CDMStatusIntermediateStacker), Convert.ToInt32(input));
        }
        
        public static CDMOUTPosShutter CDMOUTPosShutterConvert(ushort input)
        {
            return (CDMOUTPosShutter)Enum.ToObject(typeof(CDMOUTPosShutter), Convert.ToInt32(input));
        }

        public static CDMOUTPosPositionStatus CDMOUTPosPositionStatusConvert(ushort input)
        {
            return (CDMOUTPosPositionStatus)Enum.ToObject(typeof(CDMOUTPosPositionStatus), Convert.ToInt32(input));
        }

        public static CDMOUTPosTransport CDMOUTPosTransportConvert(ushort input)
        {
            return (CDMOUTPosTransport)Enum.ToObject(typeof(CDMOUTPosTransport), Convert.ToInt32(input));
        }

        public static CDMOUTPosTransportStatus CDMOUTPosTransportStatusConvert(ushort input)
        {
            return (CDMOUTPosTransportStatus)Enum.ToObject(typeof(CDMOUTPosTransportStatus), Convert.ToInt32(input));
        }
        
        public static CDMCAPSType CDMCAPSTypeConvert(ushort input)
        {
            return (CDMCAPSType)Enum.ToObject(typeof(CDMCAPSType), Convert.ToInt32(input));
        }

        public static CDMCAPSRetractArea CDMCAPSRetractAreaConvert(ushort input)
        {
            return (CDMCAPSRetractArea)Enum.ToObject(typeof(CDMCAPSRetractArea), Convert.ToInt32(input));
        }

        public static CDMCAPSRetractActions CDMCAPSRetractActionsConvert(ushort input)
        {
            return (CDMCAPSRetractActions)Enum.ToObject(typeof(CDMCAPSRetractActions), Convert.ToInt32(input));
        }

        public static CDMCAPSMoveItems CDMCAPSMoveItemsConvert(string input)
        {
            return (CDMCAPSMoveItems)Enum.ToObject(typeof(CDMCAPSMoveItems), Convert.ToInt32(input));
        }

        public static CDMCashUnitType CDMCashUnitTypeConvert(ushort input)
        {
            return (CDMCashUnitType)Enum.ToObject(typeof(CDMCashUnitType), Convert.ToInt32(input));
        }

        public static CDMCashUnitStatus CDMCashUnitStatusConvert(ushort input)
        {
            return (CDMCashUnitStatus)Enum.ToObject(typeof(CDMCashUnitStatus), Convert.ToInt32(input));
        }

        public static CDMMIXTYPEMixType CDMMIXTYPEMixTypeConvert(string input)
        {
            return (CDMMIXTYPEMixType)Enum.ToObject(typeof(CDMMIXTYPEMixType), Convert.ToInt32(input));
        }

        public static CDMMIXTYPEMixNumber CDMMIXTYPEMixNumberConvert(string input)
        {
            return (CDMMIXTYPEMixNumber)Enum.ToObject(typeof(CDMMIXTYPEMixNumber), Convert.ToInt32(input));
        }

        public static CDMPRESENTSTATUSPresentState CDMPRESENTSTATUSPresentStateConvert(string input)
        {
            return (CDMPRESENTSTATUSPresentState)Enum.ToObject(typeof(CDMPRESENTSTATUSPresentState), Convert.ToInt32(input));
        }

        public static CDMPosition CDMPositionConvert(ushort input)
        {
            return (CDMPosition)Enum.ToObject(typeof(CDMPosition), Convert.ToInt32(input));
        }

        public static CDMTELLERDETAILSInputPosition CDMTELLERDETAILSInputPositionConvert(string input)
        {
            return (CDMTELLERDETAILSInputPosition)Enum.ToObject(typeof(CDMTELLERDETAILSInputPosition), Convert.ToInt32(input));
        }

        public static ExchangeType ExchangeTypeConvert(string input)
        {
            return (ExchangeType)Enum.ToObject(typeof(ExchangeType), Convert.ToInt32(input));
        }

        public static CDMTELLERUPDATEAction CDMTELLERUPDATEActionConvert(string input)
        {
            return (CDMTELLERUPDATEAction)Enum.ToObject(typeof(CDMTELLERUPDATEAction), Convert.ToInt32(input));
        }

        public static CDMCUERRORFailure CDMCUERRORFailureConvert(string input)
        {
            return (CDMCUERRORFailure)Enum.ToObject(typeof(CDMCUERRORFailure), Convert.ToInt32(input));
        }

        public static WFS_EXEE_CDM_NOTESERRORReason WFS_EXEE_CDM_NOTESERRORReasonConvert(string input)
        {
            return (WFS_EXEE_CDM_NOTESERRORReason)Enum.ToObject(typeof(WFS_EXEE_CDM_NOTESERRORReason), Convert.ToInt32(input));
        }

        public static CDMErrorCodes CDMErrorCodesConvert(string input)
        {
            return (CDMErrorCodes)Enum.ToObject(typeof(CDMErrorCodes), Convert.ToInt32(input));
        }
    }

    /// <summary>
    /// KTR10_CIM has the same definiton class
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