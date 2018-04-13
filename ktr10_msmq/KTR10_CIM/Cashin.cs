using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using Des.Pascal.Nouma;
using System.IO;
using Microsoft.Win32;

namespace Des
{
    /// <summary>
    /// Des model ATM'de para yatırma ünitesine yönelik işlemleri gerçekleştiren class.
    /// </summary>

    public class CashIn : DeviceInterface.ICashIn
    {

        #region Problem Determination
        private const string responsibility = "Des.Turkey";
        private const string owner = "Nouma.XFSCashIn";
        private const string className = "XFSCashIn";
        private Des.Pascal.PDCollection.StdSupplyPoint sp = null;
        #endregion

        #region internal Class Definition
        internal class NoumaDeviceReference
        {
            public Des.Pascal.Nouma.XFSCashInServiceClass deviceRef;
            public int refCount;
            public string devName;
            public System.UInt32 activeXFSWML;
        }
        #endregion

        #region Member Variables
        private long lMediaInCashInStartID;
        private long lMediaInCashInID;
        private static System.Collections.ArrayList deviceList = new System.Collections.ArrayList();
        private string devName = "";
        private Des.Pascal.Nouma.XFSCashInServiceClass xfsDevice = null;
        private XFSCashInResourceC3 cIM = null;

        private int xfsTimeOut = 0;
        private bool xfsExceptionEvent = true;
        private string xfsApplicationName = "";
        internal string workstationID = "";
        internal bool devicePresent = true;
        internal Des.Pascal.Nouma.XFSStatus lastAvailabilityState = Des.Pascal.Nouma.XFSStatus.XFS_STATUS_SESSION_CLOSED;
        internal bool fireManualAvailability = false;
        internal bool isSetDeviceCalled = false;
        private const string xfsClass = "CIM";
        private const string xfsType = "";
        private Thread th = null;
        private int iRetVal = -1;
        private int tmpTimeout = 0;
        private int PresentNoteTimeout = 0;
        private int rejectedNoteCount;
        private int lastRejectedNoteCount;
        private XFSStatus CIMDeviceStatus;
        private bool isNoteDictInitialised = false;
        private bool isStackerFull = false;

        private bool isResetActionPresent = false;
        private const int WFS_ERR_ACT_NOACTION = 0;
        private const int WFS_ERR_ACT_RESET = 1;
        //private const int WFS_ERR_ACT_SWERROR = 2;
        //private const int WFS_ERR_ACT_CONFIG = 4;
        //private const int WFS_ERR_ACT_HWCLEAR = 8;
        //private const int WFS_ERR_ACT_HWMAINT = 10;
        //private const int WFS_ERR_ACT_SUSPEND = 20;



        private int previousRetractedCount = 0;

        private string CashInStatusData = "";
        private string CashInCassetteData = "";
        private string CashInXFSErrorData = "";
        private string XFSErrorDataDevice = "";
        private string XFSErrorDataError = "";
        private string XFSErrorDataErrorCategory = "";
        private string XFSErrorDataSource = "";
        private string XFSErrorDataWOSACommand = "";
        private string XFSErrorDataWOSAError = "";
        private string XFSErrorDataWOSADescription = "";
        private string XFSErrorDataResult = "";

        private const string errSetDevice = "XFSCashInResourceC3- SetDevice8not called";
        private const string errNoDevice = "XFSCashInResourceC3 - Device is not configured or not responding";
        private System.Threading.AutoResetEvent initialisedEvent = new System.Threading.AutoResetEvent(false);
        internal System.Threading.AutoResetEvent CIMEvent = new System.Threading.AutoResetEvent(false);
        internal System.Threading.AutoResetEvent CIMPresentEvent = new System.Threading.AutoResetEvent(false);
        internal System.Threading.AutoResetEvent CIMCancelMediaInEvent = new System.Threading.AutoResetEvent(false);
        internal System.Threading.AutoResetEvent CIMP6Sig = new System.Threading.AutoResetEvent(false);
        /// <summary>
        /// InputRefused Timer.
        /// </summary>
        protected System.Timers.Timer InputRefusedTimer;

        /// <summary>
        /// Money Entry Timer.
        /// </summary>
        protected System.Timers.Timer moneyEntryTimer;
        /// <summary>
        /// Rollback Timer.
        /// </summary>
        protected System.Timers.Timer RollBackTimer;
        /// <summary>
        /// Rollback Timer.
        /// </summary>
        protected System.Timers.Timer ForceRollBackTimer;
        /// <summary>
        /// Money Removal Timer.
        /// </summary>
        protected System.Timers.Timer moneyRemovalTimer;

        //bayro
        //protected System.Timers.Timer cassetteCheckTimer;
        //private Thread cassetteCheckTimerThread = null;
        //private bool cassetteCheckTimerThreadOn;
        private bool resetMediaDetected;
        private bool resetMovedSuccessfully;
        private bool retractItemsTaken = false;
        private bool retractNoItems = false;
        private bool notesInserted = false;
        private bool useRecycle = false;
        private bool mediaInProcessRunning = false;
        private bool mediaInCompleted = false;

        private bool useRetractedDict = false;
        protected XFSDictionary retractedDict;


        private XFSStatus RcDeviceStatus = XFSStatus.XFS_STATUS_GOOD;
        //private XFSStatus cassetteCheckDeviceStatus = XFSStatus.XFS_STATUS_GOOD;
        //private XFSAvailability cassetteCheckDeviceAvailability = XFSAvailability.XFS_AVAILABLE;
        //private CIMCassetteStatus cassetteCheckCassetteStatus = CIMCassetteStatus.CIM_CASSETTE_AVAILABLE;
        //private CIMCassetteRepStatus cassetteCheckCassetteRepStatus = CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_OK;
        //bayro

        /// <summary>
        /// NoteTypeDictionary
        /// </summary>
        protected XFSDictionary noteTypesDict;
        XFSDictionary TmpNoteEntered;
        Des.Pascal.ConfigNotesSrvCore.NoteDefinition noteDict = null;
        private DeviceInterface.CashInIdentifiedNotes tmpidentifiedNotes = null;
        private DeviceInterface.CashInIdentifiedNotes LatestDepositedNotes = null;
        /// <summary>
        /// MediaIn_Inserted eventi alındığında uygulamayı bilgilendiren eventtir.
        /// </summary>
        public event DeviceInterface.CashInNoteInsertedDelegate CashInNoteInserted;
        /// <summary>
        /// Recycle üniteler için kullanılacak note inserted alındığında uygulamayı bilgilendiren eventtir.
        /// </summary>
        public event DeviceInterface.CashInRMNoteInsertedDelegate CashInRMNoteInserted;
        /// <summary>
        /// MediaInCashIn_InputRefused eventi alındığında uygulamayı bilgilendiren eventtir.
        /// </summary>
        public event DeviceInterface.CashInInputRefusedDelegate CashInInputRefused;
        /// <summary>
        /// MediaInCashIn_Presented eventi alındığında uygulamayı bilgilendiren eventtir.
        /// </summary>
        public event DeviceInterface.CashInRollbackPresentedDelegate CashInRollbackPresented;

        /// <summary>
        /// Para yaturma işleminin müşteri tarafından iptal edildiğini uygulamaya bildiren evettir.
        /// </summary>
        public delegate void CanceledByUserDelegate();
        /// <summary>
        /// Para yaturma işleminin müşteri tarafından iptal edildiğini uygulamaya bildiren evettir.
        /// </summary>
        public event CanceledByUserDelegate CanceledByUser;

        ATMLight Light;
        private bool RecyclingPresent = false;
        private SIUXFSDevice SiuDev = SIUXFSDevice.SIU_ENVELOPE_DEPOSITORY;
        private bool RecycleWaitTillTaken = false;
        private bool IsAtmDiebold;
        private const int WAIT_P6_TIME = 1000;
        #endregion

        # region CashIn - Ctor
        /// <summary>
        /// CashIn Constructor
        /// Obje kullanılmak üzere yaratıldığında, Service Provider üzerinden CashInModule1 modülüne erişilir.
        /// Yaratılmış olan bu obje kullanıldığı sürece bu erişim üzerinden işlemler gerçekleştirilir.
        /// Objenin yaratılmasından sonra ayrıca ünitenin tanıyacağı banknote tipleri belirlenir.
        /// </summary>
        public CashIn()
        {
            #region Problem Determination
            sp = new Des.Pascal.PDCollection.StdSupplyPoint(responsibility, owner, className);
            sp.InterfaceEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashIn - CTOR, Entry", Logger.Logger.LogTypes.Development);
            #endregion
            try
            {
                if (GetRegistryParameter("SstType") == "Des_RM")
                {
                    RecyclingPresent = true;
                    useRecycle = true;
                    SiuDev = SIUXFSDevice.SIU_BILL_ACCEPTOR;
                }
                else
                {
                    RecyclingPresent = false;
                    useRecycle = false;
                    SiuDev = SIUXFSDevice.SIU_ENVELOPE_DEPOSITORY;
                }
                if (GetRegistryParameter("SstMarka") == "DIEBOLD")
                {
                    IsAtmDiebold = true;
                }
                else
                {
                    IsAtmDiebold = false;

                    string SourcePath = @"c:\DesTemplate";
                    if (Directory.GetFiles(SourcePath, "pcbna.ini", SearchOption.TopDirectoryOnly).Length != 0)
                    {
                        DateTime lastWriteTime1 = File.GetLastWriteTime(@"c:\DesTemplate\pcbna.ini");
                        DateTime lastWriteTime2 = File.GetLastWriteTime(@"c:\Program Files\Des Pascal\RS232BunchNoteAcceptor\pcbna.ini");
                        if (DateTime.Compare(lastWriteTime1, lastWriteTime2) > 0)
                        {
                            Logger.Logger.LogWrite("DesCashIn - CashIn , update pcbna.ini", Logger.Logger.LogTypes.Development);
                            File.Copy(Path.Combine(@"c:\DesTemplate", "pcbna.ini"), Path.Combine(@"c:\Program Files\Des Pascal\RS232BunchNoteAcceptor", "pcbna.ini"), true);
                        }
                    }
                }

                initialisedEvent.Reset();
                System.Threading.Thread winThread = new System.Threading.Thread(new System.Threading.ThreadStart(XFSCashInResourceWindowThread));
                winThread.IsBackground = true;
                winThread.Name = "XFSCashInResourceC3 WML";
                winThread.Start();
                initialisedEvent.WaitOne();
                Light = new ATMLight();
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - CashIn - CTOR1, Exit", Logger.Logger.LogTypes.Exceptions);
                #endregion
                Des.Pascal.CEH.RaiseException(ex);
            }
            #region Problem Determination
            sp.InterfaceExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashIn - CTOR2, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        private void XFSCashInResourceWindowThread()
        {
            try
            {
                #region Problem Determination
                sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - XFSCashInResourceWindowThread, Entry", Logger.Logger.LogTypes.Development);
                #endregion

                // Create instances of the timers
                moneyEntryTimer = new System.Timers.Timer();
                moneyEntryTimer.AutoReset = false;
                RollBackTimer = new System.Timers.Timer();
                RollBackTimer.AutoReset = false;
                ForceRollBackTimer = new System.Timers.Timer();
                ForceRollBackTimer.AutoReset = false;
                InputRefusedTimer = new System.Timers.Timer();
                InputRefusedTimer.AutoReset = false;
                LatestDepositedNotes = new DeviceInterface.CashInIdentifiedNotes();
                tmpidentifiedNotes = new DeviceInterface.CashInIdentifiedNotes();
                retractedDict = new XFSDictionary();

                System.UInt32 activeXFSWML = Des.Pascal.Nouma.ActiveXFSWML.WMLInitialise();
                xfsDevice = new Des.Pascal.Nouma.XFSCashInServiceClass();
                devName = "CashInModule1";
                AddExistingXFSDevice(devName, xfsDevice, activeXFSWML);

                xfsDevice.timeOut = xfsTimeOut;

                if (xfsApplicationName != "")
                    xfsDevice.ApplicationName = xfsApplicationName;

                xfsDevice.ExceptionEventRequired = xfsExceptionEvent;
                xfsDevice.AvailabilityChanged += new IXFSCashInServiceEvents_AvailabilityChangedEventHandler(CashIn_AvailabilityChanged);
                xfsDevice.XFSErrorEvent += new IXFSCashInServiceEvents_XFSErrorEventEventHandler(XFSErrorEventLog);
                xfsDevice.XFSSystemErrorEvent += new IXFSCashInServiceEvents_XFSSystemErrorEventEventHandler(XFSSystemErrorEventLog);
                xfsDevice.ExceptionEvent += new IXFSCashInServiceEvents_ExceptionEventEventHandler(Cashin_ExceptionEvent);

                xfsDevice.SetDevice(devName);

                Des.Pascal.Nouma.ActiveXFSWML.WMLProcessMessage(activeXFSWML);
                Des.Pascal.Nouma.ActiveXFSWML.WMLCloseWindow(activeXFSWML);

                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - XFSCashInResourceWindowThread1, Exit", Logger.Logger.LogTypes.Development);
                #endregion
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - XFSCashInResourceWindowThread2, Exit", Logger.Logger.LogTypes.Exceptions);
                #endregion

                Logger.Logger.LogWrite("DesCashIn - Reset system ", Logger.Logger.LogTypes.Development);
                Logger.Logger.LogResetInfo("DesCashIn Exception HEART");
                System.Diagnostics.Process.Start("C:\\ISBANKASI\\ExcReset.exe");
            }
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - XFSCashInResourceWindowThread3, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void CassetteCheckEventLog(string message)
        {
            DateTime dt = DateTime.Now;
            string fileName = @"C:\WEBATMLOGS\JOURNAL\" + dt.Year.ToString() + dt.Month.ToString().PadLeft(2, '0') + dt.Day.ToString().PadLeft(2, '0') + ".jrn";
            string sLogInfo = message;
            sLogInfo = CreateLogTimeStr() + sLogInfo;
            StreamWriter sWriter = null;
            try
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                sWriter = new StreamWriter(fileStream);
                sWriter.WriteLine(sLogInfo);
            }
            finally
            {
                if (sWriter != null) sWriter.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Current date time in string (GG.AA.YYYY - SS:DD:SS:MMMM)</returns>
        private string CreateLogTimeStr()
        {
            try
            {
                DateTime dt = DateTime.Now;

                return (dt.Day.ToString().PadLeft(2, '0') + "." +
                        dt.Month.ToString().PadLeft(2, '0') + "." +
                        dt.Year.ToString() + " - " +
                        dt.Hour.ToString().PadLeft(2, '0') + ":" +
                        dt.Minute.ToString().PadLeft(2, '0') + ":" +
                        dt.Second.ToString().PadLeft(2, '0') + ":" +
                        dt.Millisecond.ToString().PadLeft(3, '0'));
            }
            catch
            {
                return "GG.AA.YYYY - SS:DD:SS:MMMM";
            }
        }
        //bayro
        private void AddExistingXFSDevice(string devName, Des.Pascal.Nouma.XFSCashInServiceClass XFSCashIn, System.UInt32 activeXFSWML)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - AddExistingXFSDevice, Entry", Logger.Logger.LogTypes.Development);
            #endregion
            NoumaDeviceReference device = new NoumaDeviceReference();
            device.devName = devName;
            device.deviceRef = XFSCashIn;
            device.refCount = 1;
            device.activeXFSWML = activeXFSWML;
            deviceList.Add(device);
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - AddExistingXFSDevice, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        private void CashIn_AvailabilityChanged(XFSStatus status, string workstationID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.EventArrived(MethodBase.GetCurrentMethod().Name, status, workstationID, timeStamp);
            Logger.Logger.LogWrite("DesCashIn - CashIn_AvailabilityChanged, Entry Status :{0}", Logger.Logger.LogTypes.Development, status.ToString());
            #endregion

            Logger.Logger.LogWrite("DesCashIn - CashIn_AvailabilityChanged, " + status.ToString(), Logger.Logger.LogTypes.Development);

            try
            {
                CIMAcceptorStatus cIMAcceptorStatus = xfsDevice.AcceptorStatus;

                RcDeviceStatus = status;

                Logger.Logger.LogWrite("DesCashIn - AcceptorStatus, " + cIMAcceptorStatus.ToString(), Logger.Logger.LogTypes.Development);

                DeviceInterface.CashInInitTransactionResults initStatus = InitTransaction();

                if (status == XFSStatus.XFS_STATUS_NO_DEVICE_PRESENT)
                {
                    string SysRestartDateNow = DateTime.Now.ToString("yyyyMMdd");
                    Logger.Logger.LogWrite("DesCashIn - SysRestartDateNow :({0})", Logger.Logger.LogTypes.Development, SysRestartDateNow.ToString());
                    string SysRestartDatePrev = GetRegistryParameter("SysRestartDate");
                    Logger.Logger.LogWrite("DesCashIn - SysRestartDatePrev :({0})", Logger.Logger.LogTypes.Development, SysRestartDatePrev.ToString());

                    if (Convert.ToInt64(SysRestartDatePrev) < Convert.ToInt64(SysRestartDateNow))
                    {
                        SetRegistryParameter("SysRestartDate", SysRestartDateNow);
                        System.Threading.Thread.Sleep(15000);
                        Logger.Logger.LogResetInfo("DeviceNotFound Exception CashIn HEART");
                        System.Diagnostics.Process.Start("C:\\ISBANKASI\\ExcReset.exe");
                    }
                    else
                    {
                        initialisedEvent.Set();
                    }
                }
                else if (initStatus != DeviceInterface.CashInInitTransactionResults.OK)
                {
                    initialisedEvent.Set();
                }
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - CashIn_AvailabilityChanged, Exception :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion

                initialisedEvent.Set();
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashIn_AvailabilityChanged, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        private void SetRegistryParameter(string name, string Value)
        {
            RegistryKey rKey = Registry.LocalMachine;
            RegistryKey rk = rKey;
            RegistryKey sk1 = rk.CreateSubKey(@"SOFTWARE\SelfServiceTerminal\parameters");
            if (sk1 != null)
                sk1.SetValue(name, Value);
        }

        private void initialiseNoteDict()
        {
            Logger.Logger.LogWrite("DesCashIn - initialiseNoteDict, Entry", Logger.Logger.LogTypes.Development);
            cIM = new XFSCashInResourceC3();
            cIM.ExceptionEventRequired = true;
            cIM.SessionModel = XFSModel.XFS_PROCESS_BASED;
            cIM.XFSErrorEvent += new _IXFSCashInEvents_XFSErrorEventEventHandler(XFSErrorEventLog);
            cIM.XFSSystemErrorEvent += new _IXFSCashInEvents_XFSSystemErrorEventEventHandler(XFSSystemErrorEventLog);
            cIM.AvailabilityChanged += new _IXFSCashInEvents_AvailabilityChangedEventHandler(cIM_AvailabilityChanged);
            cIM.ExceptionEvent += new _IXFSCashInEvents_ExceptionEventEventHandler(cIM_ExceptionEvent);
            cIM.SetDevice("CashInModule1");
            Logger.Logger.LogWrite("DesCashIn - initialiseNoteDict, Exit", Logger.Logger.LogTypes.Development);
        }

        private void cIM_AvailabilityChanged(XFSStatus status, string workstationID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.EventArrived(MethodBase.GetCurrentMethod().Name, status, workstationID, timeStamp);
            Logger.Logger.LogWrite("DesCashIn - cIM_AvailabilityChanged, Entry Status:{0}", Logger.Logger.LogTypes.Development, status.ToString());
            #endregion

            try
            {

                cIM.XFSErrorEvent -= new _IXFSCashInEvents_XFSErrorEventEventHandler(XFSErrorEventLog);
                cIM.XFSSystemErrorEvent -= new _IXFSCashInEvents_XFSSystemErrorEventEventHandler(XFSSystemErrorEventLog);
                cIM.AvailabilityChanged -= new _IXFSCashInEvents_AvailabilityChangedEventHandler(cIM_AvailabilityChanged);
                cIM.ExceptionEvent -= new _IXFSCashInEvents_ExceptionEventEventHandler(cIM_ExceptionEvent);

                noteDict = new Des.Pascal.ConfigNotesSrvCore.NoteDefinition();
                noteDict.Resource = cIM;
                noteTypesDict = new XFSDictionary();
                noteTypesDict = noteDict.GetConfiguredNotes("");
                Logger.Logger.LogWrite("DesCashIn - noteTypesDict, :({0})", Logger.Logger.LogTypes.Critical, noteTypesDict.Count.ToString());
                AcceptableNotes(noteTypesDict);

                isNoteDictInitialised = true;
            }
            catch (System.Exception ex)
            {
                isNoteDictInitialised = false;
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - cIM_AvailabilityChanged, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - cIM_AvailabilityChanged, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.ExceptionEventRequired = false;
            initialisedEvent.Set();

        }

        void cIM_ExceptionEvent(int requestID, XFSErrorStatus error, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name, error.ToString(), workstationID, timeStamp);
            Logger.Logger.LogWrite("DesCashin - cIM_ExceptionEvent, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            cIM.XFSErrorEvent -= new _IXFSCashInEvents_XFSErrorEventEventHandler(XFSErrorEventLog);
            cIM.XFSSystemErrorEvent -= new _IXFSCashInEvents_XFSSystemErrorEventEventHandler(XFSSystemErrorEventLog);
            cIM.AvailabilityChanged -= new _IXFSCashInEvents_AvailabilityChangedEventHandler(cIM_AvailabilityChanged);
            cIM.ExceptionEvent -= new _IXFSCashInEvents_ExceptionEventEventHandler(cIM_ExceptionEvent);

            Logger.Logger.LogWrite("DesCashin - cIM_ExceptionEvent, XFSErrorStatus : " + error.ToString(), Logger.Logger.LogTypes.Development);
            Logger.Logger.LogWrite("DesCashin - cIM_ExceptionEvent, DeviceStatus : " + xfsDevice.DeviceStatus.ToString(), Logger.Logger.LogTypes.Development);

            isNoteDictInitialised = false;
            xfsDevice.ExceptionEventRequired = false;
            initialisedEvent.Set();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCard - cIM_ExceptionEvent, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        #endregion

        #region ICashIn Members

        #region InitTransaction
        /// <summary>
        /// Bu fonksiyonda AcceptorStatus durumuna bakılarak uygulamaya para yatirma işleminin yapılıp yapılamayacağı
        /// bilgisi gönderir.
        /// </summary>
        /// <returns>
        /// DeviceInterface.CashInInitTransactionResults.OK
        /// DeviceInterface.CashInInitTransactionResults.Error
        /// </returns>
        public DeviceInterface.CashInInitTransactionResults InitTransaction()
        {
            #region Problem Determination
            sp.InterfaceEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - InitTransaction, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            DeviceInterface.CashInInitTransactionResults TmpStatus;
            TmpStatus = DeviceInterface.CashInInitTransactionResults.Error;

            try
            {
                CIMGetDeviceStatus returnCode = xfsDevice.GetDeviceStatus();

                Logger.Logger.LogWrite("DesCashIn - InitTransaction, GetDeviceStatus,{0}", Logger.Logger.LogTypes.Development, returnCode.ToString());
                if (returnCode == CIMGetDeviceStatus.CIM_GET_DEVICE_STATUS_OK)
                {
                    CIMDeviceStatus = xfsDevice.DeviceStatusObject.DeviceStatus;
                    CIMAcceptorStatus cashInAcceptorStatus = xfsDevice.DeviceStatusObject.AcceptorStatus;
                    Logger.Logger.LogWrite("DesCashIn - InitTransaction, DeviceStatus :({0})", Logger.Logger.LogTypes.Development, CIMDeviceStatus.ToString());
                    if (CIMDeviceStatus == XFSStatus.XFS_STATUS_GOOD || CIMDeviceStatus == XFSStatus.XFS_STATUS_ERROR ||
                       (RecyclingPresent == true && CIMDeviceStatus == XFSStatus.XFS_STATUS_DEVICE_BUSY))
                    {
                        if (cashInAcceptorStatus == CIMAcceptorStatus.CIM_ACCEPTOR_CASH_UNIT_STOP ||
                            cashInAcceptorStatus == CIMAcceptorStatus.CIM_ACCEPTOR_CASH_UNIT_UNKNOWN)
                        {
                            Logger.Logger.LogWrite("DesCashIn - InitTransaction:AcceptorStatus,{0}", Logger.Logger.LogTypes.Development, cashInAcceptorStatus.ToString());
                            CIMDeviceStatus = XFSStatus.XFS_STATUS_DEVICE_INOPERATIVE;
                            TmpStatus = DeviceInterface.CashInInitTransactionResults.Error;
                        }
                        //12.08.2015 03:18:02 12.08.15 03:18:02 Des 3305 03102 8516
                        else if (RecyclingPresent == true && cashInAcceptorStatus == CIMAcceptorStatus.CIM_ACCEPTOR_CASH_UNIT_STATE)
                        {
                            TmpStatus = DeviceInterface.CashInInitTransactionResults.OK;

                            XFSStatus cashInDeviceStatus = xfsDevice.DeviceStatusObject.DeviceStatus;
                            Logger.Logger.LogWrite("DesCashIn - InitTransaction:DeviceStatus,{0}", Logger.Logger.LogTypes.Development, cashInDeviceStatus.ToString());
                            XFSCashInCassette cashUnitCassetteType;
                            XFSDictionary dict;
                            dict = xfsDevice.CashUnits;
                            object keys = dict.Keys();
                            System.Array keyArray = (System.Array)keys;
                            foreach (object key in keyArray)
                            {
                                // Get an item from the dictionary
                                object refKey = key;
                                cashUnitCassetteType = (XFSCashInCassette)dict.get_Item(ref refKey);
                                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:CashUnits.UnitID :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.UnitID.ToString());
                                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:CashUnits.UnitType :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.UnitType.ToString());
                                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:CashUnits.CassetteStatus :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.CassetteStatus.ToString());
                                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:CashUnits.UnitCurrentCount :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.UnitCurrentCount.ToString());
                                if (cashUnitCassetteType.UnitType == CIMCassetteType.CIM_CASH_IN)
                                {
                                    if (cashUnitCassetteType.CassetteStatus == CIMCassetteStatus.CIM_CASSETTE_NOT_AVAILABLE)
                                    {
                                        Logger.Logger.LogJournal("!!! CIM_CASH_IN : CIM_CASSETTE_NOT_AVAILABLE");
                                        Logger.Logger.LogJournal("!!! DEPOSIT BULUNAMADI PARA YATIRMAS DEVRE DISI");
                                        TmpStatus = DeviceInterface.CashInInitTransactionResults.Error;
                                    }
                                }
                            }

                        }
                        else
                        {
                            TmpStatus = DeviceInterface.CashInInitTransactionResults.OK;
                        }
                    }
                    else
                    {
                        TmpStatus = DeviceInterface.CashInInitTransactionResults.Error;
                    }
                }
                else
                {
                    TmpStatus = DeviceInterface.CashInInitTransactionResults.Error;
                }
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - InitTransaction, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn InitTransaction, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }

            if (!isNoteDictInitialised && TmpStatus == DeviceInterface.CashInInitTransactionResults.OK)
            {
                initialiseNoteDict();
                if (!isNoteDictInitialised)
                {
                    TmpStatus = DeviceInterface.CashInInitTransactionResults.Error;

                }
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - InitTransaction, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            return (TmpStatus);
        }
        #endregion

        #region MediaIn
        /// <summary>
        /// Para yatırma işleminin başlaması durumunda çağırılan fonksiyondur. Bu fonksiyon çağırıldığında Des tarafında
        /// iki method çalıştırılır. Bu metodlar CashInStart ve CashIn metodlarıdır. CashInStart metodunun başarılı bitmesi
        /// durumunda CashIn metodu çalıştırılır. Bu nedenle her iki metodta thread açılarak ayrı ayrı degerlendirilmektedir.
        /// Metodların detaylarını Pascal Documentation Active XFS Controls altında bulabilirsiniz. 
        /// </summary> 
        /// <param name="timeout"></param>
        /// <returns>
        /// DeviceInterface.CashInMediaInresults
        /// DeviceInterface.DeviceInterface.CashInMediaInresults.CancelledByUser
        /// DeviceInterface.CashInMediaInresults.Error
        /// DeviceInterface.CashInMediaInresults.MetalDetected
        /// DeviceInterface.CashInMediaInresults.ShutterCloseError
        /// DeviceInterface.CashInMediaInresults.ShutterOpenError
        /// DeviceInterface.CashInMediaInresults.Successfull
        /// DeviceInterface.CashInMediaInresults.Timeout
        /// DeviceInterface.CashInMediaInresults.TooManyNotes
        /// </returns>
        public DeviceInterface.CashInMediaInresults MediaIn(int timeout, string currency)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaIn, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            RecycleWaitTillTaken = false;
            notesInserted = false;
            mediaInCompleted = false;
            useRetractedDict = false;
            resetMediaDetected = false;
            isStackerFull = false;
            isResetActionPresent = false;

            iRetVal = -1;
            tmpTimeout = timeout * 1000;
            if (xfsDevice.LastCashInStatus.TransactionStatus != CIMTransactionStatus.CIM_TRANSACTION_ACTIVE)
            {
                try
                {

                    Logger.Logger.LogWrite("DesCashIn - MediaIn, (false) Step1", Logger.Logger.LogTypes.Development);

                    rejectedNoteCount = 0;
                    lastRejectedNoteCount = 0;

                    CashInStatusData = "";
                    CashInCassetteData = "";
                    CashInXFSErrorData = "";
                    XFSErrorDataDevice = "";
                    XFSErrorDataError = "";
                    XFSErrorDataErrorCategory = "";
                    XFSErrorDataSource = "";
                    XFSErrorDataWOSACommand = "";
                    XFSErrorDataWOSAError = "";
                    XFSErrorDataWOSADescription = "";
                    XFSErrorDataResult = "";

                    CIMEvent.Reset();
                    th = new Thread(new ThreadStart(MediaInCashInStartProcess));
                    th.IsBackground = true;
                    th.Start();
                    CIMEvent.WaitOne();

                    if (iRetVal == 0)
                    {
                        LatestDepositedNotes.CurrentEscrowCapacity = xfsDevice.MaxItemsOnStacker;
                        tmpidentifiedNotes.CurrentEscrowCapacity = 0;
                        Logger.Logger.LogWrite("DesCashIn - MediaIn, (false) Step2", Logger.Logger.LogTypes.Development);
                        CIMEvent.Reset();
                        mediaInProcessRunning = true;
                        th = new Thread(new ThreadStart(MediaInCashInProcess));
                        th.IsBackground = true;
                        th.Start();
                        CIMEvent.WaitOne();
                        mediaInProcessRunning = false;

                        xfsDevice.CashInFailed -= new IXFSCashInServiceEvents_CashInFailedEventHandler(MediaIn_CashInFailed);
                        xfsDevice.CashInOK -= new IXFSCashInServiceEvents_CashInOKEventHandler(MediaIn_CashInOK);
                        xfsDevice.InputRefused -= new IXFSCashInServiceEvents_InputRefusedEventHandler(MediaInCashIn_InputRefused);
                        xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(MediaInCashIn_CashUnitError);
                        xfsDevice.NoteError -= new IXFSCashInServiceEvents_NoteErrorEventHandler(MediaInCashIn_NoteError);
                        xfsDevice.Inserted -= new IXFSCashInServiceEvents_InsertedEventHandler(MediaIn_Inserted);
                        xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(MediaInCashIn_Taken);
                        xfsDevice.Presented -= new IXFSCashInServiceEvents_PresentedEventHandler(MediaInCashIn_Presented);
                        this.CanceledByUser -= new CanceledByUserDelegate(CashIn_CanceledByUser);

                        StopInputRefusedTimer();
                        StopMoneyEntryTimer();
                        Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);

                        if (iRetVal == 0 && RecyclingPresent)
                            GetSignature();
                    }

                    #region Problem Determination
                    sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                    Logger.Logger.LogWrite("DesCashIn - MediaIn, Exit (false) {0}", Logger.Logger.LogTypes.Development, iRetVal.ToString());
                    #endregion

                    return (DeviceInterface.CashInMediaInresults)iRetVal;
                }
                catch (System.Exception ex)
                {
                    #region Problem Determination
                    sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                    Logger.Logger.LogWrite("DesCashIn - MediaIn, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                    #endregion
                    Logger.Logger.LogResetInfo("DesCashIn MediaIn, Exception");
                    //Des.Pascal.CEH.RaiseException(ex);
                    return DeviceInterface.CashInMediaInresults.Error;
                }
            }
            else
            {
                try
                {

                    Logger.Logger.LogWrite("DesCashIn - MediaIn, (true)", Logger.Logger.LogTypes.Development);

                    lastRejectedNoteCount = 0;
                    CIMEvent.Reset();
                    mediaInProcessRunning = true;
                    th = new Thread(new ThreadStart(MediaInCashInProcess));
                    th.IsBackground = true;
                    th.Start();
                    CIMEvent.WaitOne();
                    mediaInProcessRunning = false;

                    xfsDevice.CashInFailed -= new IXFSCashInServiceEvents_CashInFailedEventHandler(MediaIn_CashInFailed);
                    xfsDevice.CashInOK -= new IXFSCashInServiceEvents_CashInOKEventHandler(MediaIn_CashInOK);
                    xfsDevice.InputRefused -= new IXFSCashInServiceEvents_InputRefusedEventHandler(MediaInCashIn_InputRefused);
                    xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(MediaInCashIn_CashUnitError);
                    xfsDevice.NoteError -= new IXFSCashInServiceEvents_NoteErrorEventHandler(MediaInCashIn_NoteError);
                    xfsDevice.Inserted -= new IXFSCashInServiceEvents_InsertedEventHandler(MediaIn_Inserted);
                    xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(MediaInCashIn_Taken);
                    xfsDevice.Presented -= new IXFSCashInServiceEvents_PresentedEventHandler(MediaInCashIn_Presented);
                    this.CanceledByUser -= new CanceledByUserDelegate(CashIn_CanceledByUser);

                    StopInputRefusedTimer();
                    StopMoneyEntryTimer();
                    Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);

                    if (iRetVal == 0 && RecyclingPresent)
                        GetSignature();

                    #region Problem Determination
                    sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                    Logger.Logger.LogWrite("DesCashIn - MediaIn, Exit (true),{0}", Logger.Logger.LogTypes.Development, iRetVal.ToString());
                    #endregion

                    return (DeviceInterface.CashInMediaInresults)iRetVal;
                }
                catch (System.Exception ex)
                {
                    #region Problem Determination
                    sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                    Logger.Logger.LogWrite("DesCashIn - MediaIn, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                    #endregion
                    Logger.Logger.LogResetInfo("DesCashIn MediaIn, Exception");
                    //Des.Pascal.CEH.RaiseException(ex);
                    return DeviceInterface.CashInMediaInresults.Error;
                }
            }
        }

        #region MediaInCashInStartProcess (Step 1)
        private void MediaInCashInStartProcess()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashInStartProcess, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            CIMPosition inputPosition;
            CIMPosition outputPosition;

            inputPosition = CIMPosition.CIM_POSITION_DEFAULT;
            outputPosition = CIMPosition.CIM_POSITION_DEFAULT;

            Logger.Logger.LogWrite("DesCashIn - MediaInCashInStartProcess, useRecycle : " + useRecycle.ToString(), Logger.Logger.LogTypes.Development);

            try
            {
                xfsDevice.CashInStartOK += new IXFSCashInServiceEvents_CashInStartOKEventHandler(MediaIn_CashInStartOK);
                xfsDevice.CashInStartFailed += new IXFSCashInServiceEvents_CashInStartFailedEventHandler(MediaIn_CashInStartFailed);
                lMediaInCashInStartID = xfsDevice.CashInStart(inputPosition, outputPosition, useRecycle);
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - MediaInCashInStartProcess, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn MediaInCashInStartProcess, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashInStartProcess, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void MediaIn_CashInStartFailed(int requestID, CIMCashInStartError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaIn_CashInStartFailed, Critical :({0})", Logger.Logger.LogTypes.Critical, reason.ToString());
            #endregion

            xfsDevice.CashInStartOK -= new IXFSCashInServiceEvents_CashInStartOKEventHandler(MediaIn_CashInStartOK);
            xfsDevice.CashInStartFailed -= new IXFSCashInServiceEvents_CashInStartFailedEventHandler(MediaIn_CashInStartFailed);

            iRetVal = 12;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaIn_CashInStartFailed, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        void MediaIn_CashInStartOK(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaIn_CashInStartOK, Entry", Logger.Logger.LogTypes.Development);
            #endregion


            xfsDevice.CashInStartOK -= new IXFSCashInServiceEvents_CashInStartOKEventHandler(MediaIn_CashInStartOK);
            xfsDevice.CashInStartFailed -= new IXFSCashInServiceEvents_CashInStartFailedEventHandler(MediaIn_CashInStartFailed);

            iRetVal = 0;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaIn_CashInStartOK, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        private string GetRegistryParameter(string param)
        {
            string REG_PARAMS = @"SOFTWARE\SelfServiceTerminal\parameters";

            RegistryKey rKey = Registry.LocalMachine;
            RegistryKey sk = rKey.OpenSubKey(REG_PARAMS);
            if (sk != null)
            {
                return (string)sk.GetValue(param);
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region MediaInCashInProcess (Step 2)
        private void MediaInCashInProcess()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashInProcess, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            StartMoneyEntryTimer(tmpTimeout);

            try
            {
                xfsDevice.CashInFailed += new IXFSCashInServiceEvents_CashInFailedEventHandler(MediaIn_CashInFailed);
                xfsDevice.CashInOK += new IXFSCashInServiceEvents_CashInOKEventHandler(MediaIn_CashInOK);
                xfsDevice.InputRefused += new IXFSCashInServiceEvents_InputRefusedEventHandler(MediaInCashIn_InputRefused);
                xfsDevice.CashUnitError += new IXFSCashInServiceEvents_CashUnitErrorEventHandler(MediaInCashIn_CashUnitError);
                xfsDevice.NoteError += new IXFSCashInServiceEvents_NoteErrorEventHandler(MediaInCashIn_NoteError);
                xfsDevice.Inserted += new IXFSCashInServiceEvents_InsertedEventHandler(MediaIn_Inserted);
                xfsDevice.Taken += new IXFSCashInServiceEvents_TakenEventHandler(MediaInCashIn_Taken);
                xfsDevice.Presented += new IXFSCashInServiceEvents_PresentedEventHandler(MediaInCashIn_Presented);
                this.CanceledByUser += new CanceledByUserDelegate(CashIn_CanceledByUser);

                lMediaInCashInID = xfsDevice.CashIn();

                Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_MEDIUMFLASH, SiuDev);
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - MediaInCashInProcess, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn MediaInCashInProcess, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashInProcess, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void MediaInCashIn_NoteError(int requestID, CIMNoteError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_NoteError, Entry :({0})", Logger.Logger.LogTypes.Exceptions, reason.ToString());
            #endregion

            Logger.Logger.LogJournal("HATA KODU   : " + reason.ToString());
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_NoteError, reason :({0})", Logger.Logger.LogTypes.Development, reason.ToString());

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_NoteError, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void MediaInCashIn_CashUnitError(int requestID, XFSCashInCassette cashUnit, CIMCashUnitFailure reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_CashUnitError, Entry :({0})", Logger.Logger.LogTypes.Exceptions, reason.ToString());
            #endregion

            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_NoteError, cashUnit UnitID :({0})", Logger.Logger.LogTypes.Development, cashUnit.UnitID.ToString());
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_NoteError, reason :({0})", Logger.Logger.LogTypes.Development, reason.ToString());

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_CashUnitError, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void MediaInCashIn_Taken(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_Taken, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);

            if (lastRejectedNoteCount == 0)
            {
                Logger.Logger.LogJournal("KUPURLER ALINDI");
            }

            //Recycle cihazlarda taken eventi veya timeout alana kadar MediaIn fonksiyonunun return ettirme (Input refused case'i için)
            if (RecycleWaitTillTaken)
            {
                Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_Taken, RecycleWaitTillTaken", Logger.Logger.LogTypes.Development);
                xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(MediaInCashIn_Taken);
                RecycleWaitTillTaken = false;
                mediaInCompleted = true;
                CIMEvent.Set();
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_Taken, Exit", Logger.Logger.LogTypes.Development);
            #endregion

        }

        void MediaInCashIn_Presented(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_Presented, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            if (RecyclingPresent)
                RecycleWaitTillTaken = true;
            if (notesInserted == false)
                StopMoneyEntryTimer();

            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_MEDIUMFLASH, SiuDev);
            StartInputRefusedTimer(45000);

            if (CashInRollbackPresented != null)
            {
                CashInRollbackPresented();
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_Presented, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void MediaInCashIn_InputRefused(int requestID, CIMInputRefusedError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_InputRefused, Entry", Logger.Logger.LogTypes.Development);
            Logger.Logger.LogWrite("InputRefuse Reason: {0}", Logger.Logger.LogTypes.Development, reason.ToString());
            #endregion

            int refusedCount = xfsDevice.LastCashInStatus.NumberRefused;
            lastRejectedNoteCount = refusedCount - rejectedNoteCount;
            rejectedNoteCount = refusedCount;
            Logger.Logger.LogWrite("NumberRefused: {0}", Logger.Logger.LogTypes.Development, refusedCount.ToString());
            Logger.Logger.LogWrite("lastRejectedNoteCount: {0}", Logger.Logger.LogTypes.Development, lastRejectedNoteCount.ToString());

            if (lastRejectedNoteCount == 0)
            {
                Logger.Logger.LogJournal("IADE NEDENI : " + reason.ToString());
            }
            if (CashInInputRefused != null)
            {
                CashInInputRefused(lastRejectedNoteCount);
            }
            if (reason == CIMInputRefusedError.CIM_REFUSED_STACKER_FULL ||
                reason == CIMInputRefusedError.CIM_REFUSED_CASH_UNIT_FULL)
            {
                isStackerFull = true;
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaInCashIn_InputRefused, Exit", Logger.Logger.LogTypes.Development);
            #endregion

        }

        void MediaIn_Inserted(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaIn_Inserted, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            notesInserted = true;

            StopMoneyEntryTimer();
            StopInputRefusedTimer();
            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);

            this.CanceledByUser -= new CanceledByUserDelegate(CashIn_CanceledByUser);

            if (CashInNoteInserted != null)
            {
                CashInNoteInserted();
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaIn_Inserted, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void CashIn_CanceledByUser()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashIn_CanceledByUser, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);
            StopMoneyEntryTimer();
            StopInputRefusedTimer();

            xfsDevice.CashInFailed -= new IXFSCashInServiceEvents_CashInFailedEventHandler(MediaIn_CashInFailed);
            xfsDevice.CashInOK -= new IXFSCashInServiceEvents_CashInOKEventHandler(MediaIn_CashInOK);
            xfsDevice.InputRefused -= new IXFSCashInServiceEvents_InputRefusedEventHandler(MediaInCashIn_InputRefused);
            xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(MediaInCashIn_CashUnitError);
            xfsDevice.NoteError -= new IXFSCashInServiceEvents_NoteErrorEventHandler(MediaInCashIn_NoteError);
            xfsDevice.Inserted -= new IXFSCashInServiceEvents_InsertedEventHandler(MediaIn_Inserted);
            xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(MediaInCashIn_Taken);
            xfsDevice.Presented -= new IXFSCashInServiceEvents_PresentedEventHandler(MediaInCashIn_Presented);
            this.CanceledByUser -= new CanceledByUserDelegate(CashIn_CanceledByUser);

            iRetVal = 3;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashIn_CanceledByUser, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        void MediaIn_CashInFailed(int requestID, CIMCashInError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - MediaIn_CashInFailed, Critical :({0})", Logger.Logger.LogTypes.Critical, reason.ToString());
            #endregion

            StopMoneyEntryTimer();

            xfsDevice.CashInFailed -= new IXFSCashInServiceEvents_CashInFailedEventHandler(MediaIn_CashInFailed);
            xfsDevice.CashInOK -= new IXFSCashInServiceEvents_CashInOKEventHandler(MediaIn_CashInOK);
            xfsDevice.InputRefused -= new IXFSCashInServiceEvents_InputRefusedEventHandler(MediaInCashIn_InputRefused);
            xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(MediaInCashIn_CashUnitError);
            xfsDevice.NoteError -= new IXFSCashInServiceEvents_NoteErrorEventHandler(MediaInCashIn_NoteError);
            xfsDevice.Inserted -= new IXFSCashInServiceEvents_InsertedEventHandler(MediaIn_Inserted);
            //xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(MediaInCashIn_Taken);
            xfsDevice.Presented -= new IXFSCashInServiceEvents_PresentedEventHandler(MediaInCashIn_Presented);
            this.CanceledByUser -= new CanceledByUserDelegate(CashIn_CanceledByUser);

            if (RecycleWaitTillTaken)
            {
                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - MediaIn_CashInFailed, Exit", Logger.Logger.LogTypes.Development);
                #endregion
                mediaInCompleted = true;

                switch (reason)
                {
                    case CIMCashInError.CIM_CASHIN_NO_ITEMS:
                        iRetVal = 0;
                        break;
                    case CIMCashInError.CIM_CASHIN_TOO_MANY_ITEMS:
                        isStackerFull = true;
                        iRetVal = 0;
                        break;
                    default:
                        iRetVal = 1;
                        break;
                }

                return;
            }
            else
            {
                Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);
                StopInputRefusedTimer();
                xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(MediaInCashIn_Taken);
                mediaInCompleted = true;

                switch (reason)
                {
                    case CIMCashInError.CIM_CASHIN_CANCELLED:
                        //zaman aşımı durumlarında canceloperations sonrasında buraya giriliyor
                        Logger.Logger.LogWrite("DesCashIn - OnTimedOut CancelOperation, Entry", Logger.Logger.LogTypes.Development);
                        if (iRetVal != 2)
                        {
                            iRetVal = 3;
                        }
                        else
                        {
                            Logger.Logger.LogWrite("DesCashIn - OnTimedOut CancelOperation, Exit", Logger.Logger.LogTypes.Development);
                        }
                        break;
                    case CIMCashInError.CIM_CASHIN_NO_ITEMS:
                        iRetVal = 0;
                        break;
                    case CIMCashInError.CIM_CASHIN_TOO_MANY_ITEMS:
                        isStackerFull = true;
                        iRetVal = 0;
                        break;
                    default:
                        iRetVal = 1;
                        break;
                }

                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - xfsDevice_CashInFailed, Exit", Logger.Logger.LogTypes.Development);
                #endregion

                CIMEvent.Set();
            }
        }

        void MediaIn_CashInOK(int requestID, XFSDictionary notesEntered, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_CashInOK, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            iRetVal = 0;

            StopMoneyEntryTimer();

            xfsDevice.CashInFailed -= new IXFSCashInServiceEvents_CashInFailedEventHandler(MediaIn_CashInFailed);
            xfsDevice.CashInOK -= new IXFSCashInServiceEvents_CashInOKEventHandler(MediaIn_CashInOK);
            xfsDevice.InputRefused -= new IXFSCashInServiceEvents_InputRefusedEventHandler(MediaInCashIn_InputRefused);
            xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(MediaInCashIn_CashUnitError);
            xfsDevice.NoteError -= new IXFSCashInServiceEvents_NoteErrorEventHandler(MediaInCashIn_NoteError);
            xfsDevice.Inserted -= new IXFSCashInServiceEvents_InsertedEventHandler(MediaIn_Inserted);
            xfsDevice.Presented -= new IXFSCashInServiceEvents_PresentedEventHandler(MediaInCashIn_Presented);
            this.CanceledByUser -= new CanceledByUserDelegate(CashIn_CanceledByUser);

            TmpNoteEntered = notesEntered;

            //Recycle cihazlarda taken eventi veya timeout alana kadar MediaIn fonksiyonunun return ettirme (Input refused case'i için)
            if (RecycleWaitTillTaken)
            {
                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - xfsDevice_CashInOK RecycleWaitTillTaken, Exit", Logger.Logger.LogTypes.Development);
                #endregion
                mediaInCompleted = true;
                return;
            }
            else
            {
                StopInputRefusedTimer();
                xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(MediaInCashIn_Taken);
                mediaInCompleted = true;

                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - xfsDevice_CashInOK, Exit", Logger.Logger.LogTypes.Development);
                #endregion

                CIMEvent.Set();
            }
        }

        #endregion

        #region MediaIn Functions

        public void OnTimedOutInputRefused(object sender, System.Timers.ElapsedEventArgs e)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - OnTimedOutInputRefused, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);
            StopInputRefusedTimer();
            this.CanceledByUser -= new CanceledByUserDelegate(CashIn_CanceledByUser);

            iRetVal = 2;

            if (RecyclingPresent == false)
            {
                xfsDevice.CancelOperation((int)lMediaInCashInStartID);
                xfsDevice.CancelOperation((int)lMediaInCashInID);
                Logger.Logger.LogWrite("DesCashIn -OnTimedOutInputRefused, CancelAllOperations Wait ", Logger.Logger.LogTypes.Development);

                //Cancel çalışmazsa diye kondu
                int i = 0;
                while (i <= 120 && mediaInCompleted == false)
                {
                    System.Threading.Thread.Sleep(1000);
                    i++;
                }
                if (mediaInCompleted == false)
                {
                    Logger.Logger.LogWrite("DesCashIn - OnTimedOutInputRefused, CancelAllOperations Error", Logger.Logger.LogTypes.Development);
                }
                CIMEvent.Set();
            }
            else
            {
                CIMEvent.Set();
            }
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - OnTimedOutInputRefused, Exit", Logger.Logger.LogTypes.Development);
            #endregion

        }

        private void OnTimedOutEntry(object sender, System.Timers.ElapsedEventArgs e)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - OnTimedOutEntry, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);
            StopMoneyEntryTimer();
            this.CanceledByUser -= new CanceledByUserDelegate(CashIn_CanceledByUser);

            iRetVal = 2;

            xfsDevice.CancelOperation((int)lMediaInCashInStartID);
            xfsDevice.CancelOperation((int)lMediaInCashInID);
            Logger.Logger.LogWrite("DesCashIn - OnTimedOutEntry, CancelAllOperations Wait ", Logger.Logger.LogTypes.Development);

            //Cancel çalışmazsa diye kondu
            int i = 0;
            while (i <= 120 && mediaInCompleted == false)
            {
                System.Threading.Thread.Sleep(1000);
                i++;
            }
            if (mediaInCompleted == false)
            {
                Logger.Logger.LogWrite("DesCashIn - OnTimedOutEntry, CancelAllOperations Error", Logger.Logger.LogTypes.Development);
                iRetVal = 2;
                CIMEvent.Set();
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - OnTimedOutEntry, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        /// <summary>Starts the money entry timer</summary>
        /// <param name="dueTime">time interval</param>
        protected virtual void StartInputRefusedTimer(long dueTime)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name, dueTime);
            Logger.Logger.LogWrite("DesCashIn - StartInputRefusedTimer, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            InputRefusedTimer.Interval = dueTime;
            InputRefusedTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedOutInputRefused);
            InputRefusedTimer.Start();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StartInputRefusedTimer, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        /// <summary>Stops the money entry timer</summary>
        protected virtual void StopInputRefusedTimer()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StopInputRefusedTimer, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            InputRefusedTimer.Elapsed -= new System.Timers.ElapsedEventHandler(OnTimedOutInputRefused);

            InputRefusedTimer.Stop();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StopInputRefusedTimer, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        /// <summary>Starts the money entry timer</summary>
        /// <param name="dueTime">time interval</param>
        protected virtual void StartMoneyEntryTimer(long dueTime)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name, dueTime);
            Logger.Logger.LogWrite("DesCashIn - StartMoneyEntryTimer, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            moneyEntryTimer.Interval = dueTime;
            moneyEntryTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedOutEntry);
            moneyEntryTimer.Start();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StartMoneyEntryTimer, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        /// <summary>Stops the money entry timer</summary>
        protected virtual void StopMoneyEntryTimer()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StopMoneyEntryTimer, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            moneyEntryTimer.Elapsed -= new System.Timers.ElapsedEventHandler(OnTimedOutEntry);
            moneyEntryTimer.Stop();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StopMoneyEntryTimer, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        #endregion

        #endregion

        #region CancelMediaIn
        /// <summary>
        /// Bu fonksiyon başlatılan bir MediaIn fonksiyonunu iptal etmek için kullanılır. Bu iş için genel olarak 
        /// kullanılan CancelAllOperations metodu kullanılmaktadır.
        /// </summary>
        /// <returns>
        /// DeviceInterface.CashInGeneralResults.OK
        /// DeviceInterface.CashInGeneralResults.Error
        /// </returns>
        public DeviceInterface.CashInGeneralResults CancelMediaIn()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CancelMediaIn, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            iRetVal = 0;

            StopInputRefusedTimer();
            StopMoneyEntryTimer();

            xfsDevice.CancelAllOperations();


            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CancelMediaIn, Exit", Logger.Logger.LogTypes.Development);
            #endregion
            if (CanceledByUser != null)
            {
                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - CancelMediaIn-CanceledByUser", Logger.Logger.LogTypes.Development);
                #endregion
                CanceledByUser();
            }
            return (DeviceInterface.CashInGeneralResults)iRetVal;
        }
        #endregion

        #region GetIdentifiednotes
        /// <summary>
        /// Yatırılan para adetlerini DeviceInterface.CashInIdentifiedNotes yapısında uygulamaya aktaran fonksiyondur.
        /// </summary>
        /// <param name="identifiedNotes"></param>
        /// <returns>
        /// DeviceInterface.CashInGeneralResults.OK
        /// DeviceInterface.CashInGeneralResults.Error
        /// </returns>
        public DeviceInterface.CashInGeneralResults GetIdentifiedNotes(DeviceInterface.CashInIdentifiedNotes identifiedNotes)
        {
            #region Problem Determination
            sp.InterfaceEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetIdentifiedNotes, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            try
            {

                StopInputRefusedTimer();
                StopMoneyEntryTimer();

                tmpidentifiedNotes = identifiedNotes;

                Logger.Logger.LogWrite("DesCashIn - VendorData : {0}", Logger.Logger.LogTypes.Development, identifiedNotes.VendorData);
                if (identifiedNotes.VendorData == "CashIn")
                {
                    if (useRetractedDict == true)
                    {
                        Logger.Logger.LogWrite("DesCashIn - GetIdentifiedNotes, useRetractedDict", Logger.Logger.LogTypes.Development);
                        CalculateNotesEntered(retractedDict);
                        identifiedNotes.NumberOfIdentifiedNoteTypes = retractedDict.Count;
                    }
                    else
                    {
                        CalculateNotesEntered(xfsDevice.LastCashInStatus.NoteNumberList);
                        identifiedNotes.NumberOfIdentifiedNoteTypes = xfsDevice.LastCashInStatus.NoteNumberList.Count;
                    }

                    CalculateCIMCassetteRepStatus(TmpNoteEntered);
                    identifiedNotes.CurrentEscrowCapacity = xfsDevice.MaxItemsOnStacker - identifiedNotes.CurrentEscrowCapacity;
                    if (isStackerFull == true)
                    {
                        identifiedNotes.CurrentEscrowCapacity = 0;
                    }
                    identifiedNotes.NumberOfUnIdentifiedNoteCount = lastRejectedNoteCount;
                    Logger.Logger.LogWrite("DesCashIn - identifiedNotes.NumberOfUnIdentifiedNoteCount: {0}", Logger.Logger.LogTypes.Development, identifiedNotes.NumberOfUnIdentifiedNoteCount.ToString());
                }
                else if (identifiedNotes.VendorData.Substring(0, 7) == "CashOut")
                {
                    retractedDict.RemoveAll();
                    string[] retractVendorSpecificArray = (identifiedNotes.VendorData.Substring(7)).Split(':');
                    int retractedKey = 0;
                    lastRejectedNoteCount = Convert.ToInt16(retractVendorSpecificArray[0]);
                    for (int i = 1; i < retractVendorSpecificArray.Length; i++)
                    {
                        XFSCashInNoteTypesCount retractedItem = new XFSCashInNoteTypesCount();

                        Logger.Logger.LogWrite("DesCashIn - GetIdentifiedNotes retractVendorSpecificArray OK", Logger.Logger.LogTypes.Development);
                        string[] retractVendorSpecificCountArray = retractVendorSpecificArray[i].Split(',');
                        int retractedCount = 0;
                        short retractedNoteID = 0;

                        retractedNoteID = Convert.ToInt16(retractVendorSpecificCountArray[0]);
                        Logger.Logger.LogWrite("DesCashIn - GetIdentifiedNotes retractVendorSpecificArray retractedNoteID : " + retractedNoteID.ToString(), Logger.Logger.LogTypes.Development);
                        for (int j = 1; j < retractVendorSpecificCountArray.Length; j++)
                        {
                            retractedCount = retractedCount + Convert.ToInt32(retractVendorSpecificCountArray[j]);
                        }
                        Logger.Logger.LogWrite("DesCashIn - GetIdentifiedNotes retractVendorSpecificArray retractedCount : " + retractedCount.ToString(), Logger.Logger.LogTypes.Development);
                        retractedKey++;

                        object retractedUnitKey = retractedKey;
                        Logger.Logger.LogWrite("DesCashIn - GetIdentifiedNotes retractVendorSpecificArray retractedKey : " + retractedKey.ToString(), Logger.Logger.LogTypes.Development);
                        retractedItem.Count = retractedCount;
                        retractedItem.NoteID = retractedNoteID;
                        object retractedUnitItem = retractedItem;
                        retractedDict.Add(ref retractedUnitKey, ref retractedUnitItem);
                    }
                    Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, CashOut", Logger.Logger.LogTypes.Development);
                    CalculateNotesEntered(retractedDict);
                    identifiedNotes.NumberOfIdentifiedNoteTypes = retractedDict.Count;
                    identifiedNotes.NumberOfUnIdentifiedNoteCount = lastRejectedNoteCount;
                    Logger.Logger.LogWrite("DesCashIn - identifiedNotes.NumberOfUnIdentifiedNoteCount: {0}", Logger.Logger.LogTypes.Development, identifiedNotes.NumberOfUnIdentifiedNoteCount.ToString());
                }
                else
                {
                    Logger.Logger.LogWrite("DesCashIn - Type Error VendorData", Logger.Logger.LogTypes.Development);
                    return DeviceInterface.CashInGeneralResults.Error;
                }
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - GetIdentifiedNotes, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                return DeviceInterface.CashInGeneralResults.Error;
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetIdentifiedNotes, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            return DeviceInterface.CashInGeneralResults.OK;
        }
        /// <summary>
        /// Para yatırma ünitesinin kabul edeceği banknot tiplerini konfigür eden fonksiyondur.
        /// </summary>
        /// <param name="dictionary">
        /// XFSCashInNoteTypes
        /// </param>
        protected void AcceptableNotes(XFSDictionary dictionary)
        {
            #region Problem Determination
            sp.InterfaceEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - AcceptableNotes, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            int position = 0;
            XFSCashInNoteTypes noteTypes;
            XFSDictionary newDict = new XFSDictionary();
            // Extract the necessary information from the the XFSDictionary to parse its contents    
            int counter = dictionary.Count - 1;
            object dictKeys = dictionary.Keys();
            System.Array keyArray = (System.Array)dictKeys;
            // Loop through all the notes that can be accepted by the CIM hardware    
            for (int iDesemented = 0; iDesemented <= counter; iDesemented++)
            {
                object dicKey = keyArray.GetValue(iDesemented);
                noteTypes = (XFSCashInNoteTypes)(dictionary.get_Item(ref dicKey));

                //if (((noteTypes.CashUnitCurrency == "TRL" || noteTypes.CashUnitCurrency == "TRY" || noteTypes.CashUnitCurrency == "YTL") && noteTypes.CashUnitValue != 200) ||
                if (noteTypes.CashUnitCurrency == "TRL" || noteTypes.CashUnitCurrency == "TRY" || noteTypes.CashUnitCurrency == "YTL" ||
                   (noteTypes.CashUnitCurrency == "EUR" && (noteTypes.CashUnitValue != 1 && noteTypes.CashUnitValue != 2 && noteTypes.CashUnitValue != 5 && noteTypes.CashUnitValue != 10 && noteTypes.CashUnitValue != 500)) ||
                   (noteTypes.CashUnitCurrency == "USD" && (noteTypes.CashUnitValue != 1 && noteTypes.CashUnitValue != 2 && noteTypes.CashUnitValue != 5 && noteTypes.CashUnitValue != 10 && noteTypes.CashUnitValue != 500)) ||
                   (noteTypes.CashUnitCurrency == "GBP" && (noteTypes.CashUnitValue != 1 && noteTypes.CashUnitValue != 2 && noteTypes.CashUnitValue != 5 && noteTypes.CashUnitValue != 10 && noteTypes.CashUnitValue != 500)))
                //if (noteTypes.CashUnitCurrency == "TRL" || noteTypes.CashUnitCurrency == "TRY" || noteTypes.CashUnitCurrency == "YTL")
                {
                    if (noteTypes.Configured && noteTypes.NoteID != 19205 && noteTypes.NoteID != 2822) //Eski 20 ve 50 Pound
                    {
                        position++;
                        dicKey = (object)position;
                        object item = (object)noteTypes.NoteID;
                        newDict.Add(ref dicKey, ref item);
                        Logger.Logger.LogWrite("DesCashIn - AcceptableNotes noteTypes, :({0},{1},{2},{3})", Logger.Logger.LogTypes.Critical, noteTypes.Configured.ToString(), noteTypes.CashUnitValue.ToString(), noteTypes.CashUnitCurrency.ToString(), noteTypes.NoteID.ToString());
                    }
                    else
                    {
                        #region Problem Determination
                        string debug = String.Format("CashUnitCurrency = {0}, CashUnitValue = {1}", "Reason = {2}",
                                       noteTypes.CashUnitCurrency, noteTypes.CashUnitValue, "Is not configured but is the current CurrencyType");
                        Logger.Logger.LogWrite("DesCashIn - AcceptableNotes, Critical :({0})", Logger.Logger.LogTypes.Critical, debug);
                        #endregion
                    }
                }
                else
                {
                    #region Problem Determination
                    string debug = String.Format("CashUnitCurrency = {0}, CashUnitValue = {1}", "Reason = {2}",
                                   noteTypes.CashUnitCurrency, noteTypes.CashUnitValue, "Is not of the current CurrencyType");
                    Logger.Logger.LogWrite("DesCashIn - AcceptableNotes, Critical :({0})", Logger.Logger.LogTypes.Critical, debug);
                    #endregion
                }
            }
            // MF - newDict.Count 0 olunca ConfigureNoteTypes fonksiyonu Exception uretiyordu.
            // asagidaki satir kapatildi, sadece if-else kisminda bu fonksiyon calisacak
            // xfsDevice.ConfigureNoteTypes(newDict);

            //Configure the device if atleast one note is configure to be accepted
            if (newDict.Count == 0)
            {
                #region Problem Determination
                //No notes have been configured via supervisor, the transaction is not available
                sp.Warning(MethodBase.GetCurrentMethod().Name, "No notes have been configured, the transaction is unavailable");
                Logger.Logger.LogWrite("DesCashIn - AcceptableNotes, No notes have been configured, the transaction is unavailable", Logger.Logger.LogTypes.Development);
                #endregion
            }
            else
            {
                #region Problem Determination
                sp.Warning(MethodBase.GetCurrentMethod().Name, "ConfigureNoteTypes command called on CIM Device");
                Logger.Logger.LogWrite("DesCashIn - AcceptableNotes, ConfigureNoteTypes command called on CIM Device", Logger.Logger.LogTypes.Development);
                #endregion
                //cIM.ConfigureNoteTypes(newDict, out requestID);
                xfsDevice.ConfigureNoteTypes(newDict);
            }

            newDict = null;
            noteTypes = null;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - AcceptableNotes, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        private void CalculateCIMCassetteRepStatus(XFSDictionary dict)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CalculateCIMCassetteRepStatus, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            XFSDictionary bins = xfsDevice.CashUnits;

            object keys = bins.Keys();
            System.Array keyArray = (System.Array)keys;
            int binNum = 0;


            if (bins != null)
            {
                XFSCashInCassette bin;

                // Get cassette bin number
                binNum = 0;
                foreach (object key in keyArray)
                {
                    object refKey = key;

                    bin = (XFSCashInCassette)bins.get_Item(ref refKey);
                    binNum++;
                    if (bin.UnitType == CIMCassetteType.CIM_CASH_IN && bin.UnitID == "CI2")
                    {
                        switch (bin.CassetteReplenishmentStatus)
                        {
                            case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_EMPTY:
                                break;
                            case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_FULL:
                                break;
                            case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_HIGH:
                                break;
                            case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_INOPERATIVE:
                                break;
                            case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_NOT_PRESENT:
                                break;
                            case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_NO_VALUE:
                                break;
                            case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_OK:
                                break;
                            default:
                                break;
                        }
                    }

                }

            }
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CalculateCIMCassetteRepStatus, Exit", Logger.Logger.LogTypes.Development);
            #endregion

        }
        private void CalculateNotesRejected(XFSDictionary dict)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name, dict);
            #endregion

            XFSCashInCassette cashUnitCassetteType;
            object dictKeys = dict.Keys();
            System.Array keyArray = (System.Array)dictKeys;

            foreach (object key in keyArray)
            {
                object refKey = key;
                // Get an item from the dictionary
                cashUnitCassetteType = (XFSCashInCassette)dict.get_Item(ref refKey);

                // Check if the cassette type is the Reject Cassette
                if (cashUnitCassetteType.UnitType == CIMCassetteType.CIM_TYPE_RETRACT_CASSETTE)
                {
                    // Check count of Reject cassette
                    if (cashUnitCassetteType.UnitInCount > 0)
                    {
                        #region Problem Determination
                        string debug = String.Format("CashUnitCassetteType Type: {0} CashUnitCassetteType count: {1}", cashUnitCassetteType.UnitType,
                            cashUnitCassetteType.UnitInCount);
                        sp.GeneralDebug(MethodBase.GetCurrentMethod().Name, debug);
                        #endregion

                        // set cassete count to reject count
                        rejectedNoteCount = cashUnitCassetteType.UnitInCount;

                    }
                }
            }
        }
        private void CalculateNotesEntered(XFSDictionary dict)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Entry", Logger.Logger.LogTypes.Development);
            #endregion
            int cashInNoteTypesCount;
            int acceptableNotesCount;
            int iDesemented;
            object dictNoteTypeCountKeys;
            object dictNoteTypeKeys;
            XFSCashInNoteTypesCount noteTypesCount = new XFSCashInNoteTypesCount();
            XFSCashInNoteTypes noteTypes = new XFSCashInNoteTypes();

            int total;
            int subTotal;
            total = 0;
            // Dictionary of XFSCashInNoteTypesCount
            cashInNoteTypesCount = dict.Count - 1;
            dictNoteTypeCountKeys = dict.Keys();
            System.Array keyArrayNoteTypesCount = (System.Array)dictNoteTypeCountKeys;
            // Dictionary of XFSCashInNoteTypes
            acceptableNotesCount = noteTypesDict.Count - 1;
            dictNoteTypeKeys = noteTypesDict.Keys();
            System.Array keyArrayNoteTypes = (System.Array)dictNoteTypeKeys;

            #region Problem Determination
            string debug = String.Format("cashInNoteTypesCount: {0} acceptableNotesCount: {1}", cashInNoteTypesCount, acceptableNotesCount);
            //Logger.Logger.LogWrite("DesCashIn - AcceptableNotes, Critical :({0})", Logger.Logger.LogTypes.Critical, debug);
            #endregion

            // IDesement through the whole of the XFSCashInNoteTypesCount dictionary - entered notes
            foreach (object keyNoteTypeCount in keyArrayNoteTypesCount)
            {
                object refKeyNoteTypeCount = keyNoteTypeCount;
                // Get an item from the dictionary

                noteTypesCount = (XFSCashInNoteTypesCount)dict.get_Item(ref refKeyNoteTypeCount);

                #region Problem Determination
                debug = String.Format("noteTypesCount Count: {0} noteTypesCount NoteID: {1}", noteTypesCount.Count, noteTypesCount.NoteID);
                Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Critical :({0})", Logger.Logger.LogTypes.Critical, debug);
                #endregion

                // Check if the count is equal to zero, only add none zero counts to the dictionary
                if (noteTypesCount.Count != 0)
                {
                    // IDesement through the XFSCashInNoteTypes dictionary until a match 
                    // of 'NoteID' is found
                    // This is the complete dictionary of acceptable notes
                    for (iDesemented = 0; iDesemented <= acceptableNotesCount; iDesemented++)
                    {
                        #region Problem Determination
                        debug = String.Format("iDesemented: {0} ", iDesemented);
                        sp.GeneralDebug(MethodBase.GetCurrentMethod().Name, debug);
                        #endregion

                        object dicKey = keyArrayNoteTypes.GetValue(iDesemented);
                        noteTypes = (XFSCashInNoteTypes)noteTypesDict.get_Item(ref dicKey);

                        #region Problem Determination
                        debug = String.Format("noteTypes CashUnitCurrency: {0} noteTypes CashUnitValue: {1} noteTypes NoteID: {2}",
                            noteTypes.CashUnitCurrency, noteTypes.CashUnitValue, noteTypes.NoteID);
                        //Logger.Logger.LogWrite("DesCashIn - AcceptableNotes, Critical :({0})", Logger.Logger.LogTypes.Critical, debug);
                        sp.GeneralDebug(MethodBase.GetCurrentMethod().Name, debug);
                        #endregion



                        if (noteTypes.NoteID == noteTypesCount.NoteID)
                        {
                            // Total the value of notes added
                            subTotal = noteTypesCount.Count;
                            total = total + subTotal;
                            tmpidentifiedNotes.IdentifiedNote[(int)keyNoteTypeCount - 1].Count += noteTypesCount.Count;
                            tmpidentifiedNotes.IdentifiedNote[(int)keyNoteTypeCount - 1].Currency += noteTypes.CashUnitCurrency;
                            tmpidentifiedNotes.IdentifiedNote[(int)keyNoteTypeCount - 1].Denom += noteTypes.CashUnitValue;

                            #region Problem Determination
                            debug = String.Format("subTotal: {0} " + "total: {1}", subTotal, total);
                            sp.GeneralDebug(MethodBase.GetCurrentMethod().Name, debug);
                            Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Critical :({0})", Logger.Logger.LogTypes.Critical, debug);
                            #endregion

                        }
                    } //for
                }
                else
                {
                    // Add log to PD
                    #region Problem Determination
                    sp.GeneralDebug(MethodBase.GetCurrentMethod().Name,
                        "A Note type of count zero has been passed in the dictionary");
                    #endregion
                }
            }
            DeviceInterface.CashInIdentifiedNotes sortednotelist = null;
            sortednotelist = new DeviceInterface.CashInIdentifiedNotes();
            //sortednotelist.IdentifiedNote = tmpidentifiedNotes.IdentifiedNote;
            int i;
            int Denom5TRLcount, Denom10TRLcount, Denom20TRLcount, Denom50TRLcount, Denom100TRLcount, Denom200TRLcount;
            int Denom5EURcount, Denom10EURcount, Denom20EURcount, Denom50EURcount, Denom100EURcount, Denom200EURcount;
            int Denom5USDcount, Denom10USDcount, Denom20USDcount, Denom50USDcount, Denom100USDcount, Denom200USDcount;
            int Denom5GBPcount, Denom10GBPcount, Denom20GBPcount, Denom50GBPcount, Denom100GBPcount, Denom200GBPcount;

            int numofrecognizednotetype;

            numofrecognizednotetype = 0;
            Denom5TRLcount = Denom10TRLcount = Denom20TRLcount = Denom50TRLcount = Denom100TRLcount = Denom200TRLcount = 0;
            Denom5EURcount = Denom10EURcount = Denom20EURcount = Denom50EURcount = Denom100EURcount = Denom200EURcount = 0;
            Denom5USDcount = Denom10USDcount = Denom20USDcount = Denom50USDcount = Denom100USDcount = Denom200USDcount = 0;
            Denom5GBPcount = Denom10GBPcount = Denom20GBPcount = Denom50GBPcount = Denom100GBPcount = Denom200GBPcount = 0;

            Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, dict.Count({0})", Logger.Logger.LogTypes.Critical, dict.Count.ToString());
            tmpidentifiedNotes.NumberOfIdentifiedNoteTypes = 1;
            if (dict.Count > 1)
            {
                for (i = 0; i <= dict.Count - 1; i++)
                {
                    if (tmpidentifiedNotes.IdentifiedNote[i].Currency == "TRL" || tmpidentifiedNotes.IdentifiedNote[i].Currency == "TRY")
                    {
                        switch (tmpidentifiedNotes.IdentifiedNote[i].Denom)
                        {
                            case 5:
                                Denom5TRLcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 10:
                                Denom10TRLcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 20:
                                Denom20TRLcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 50:
                                Denom50TRLcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 100:
                                Denom100TRLcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 200:
                                Denom200TRLcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                        }
                    }
                    else if (tmpidentifiedNotes.IdentifiedNote[i].Currency == "EUR")
                    {
                        switch (tmpidentifiedNotes.IdentifiedNote[i].Denom)
                        {
                            case 5:
                                Denom5EURcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 10:
                                Denom10EURcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 20:
                                Denom20EURcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 50:
                                Denom50EURcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 100:
                                Denom100EURcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 200:
                                Denom200EURcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                        }
                    }
                    else if (tmpidentifiedNotes.IdentifiedNote[i].Currency == "USD")
                    {
                        switch (tmpidentifiedNotes.IdentifiedNote[i].Denom)
                        {
                            case 5:
                                Denom5USDcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 10:
                                Denom10USDcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 20:
                                Denom20USDcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 50:
                                Denom50USDcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 100:
                                Denom100USDcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 200:
                                Denom200USDcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                        }
                    }
                    else if (tmpidentifiedNotes.IdentifiedNote[i].Currency == "GBP")
                    {
                        switch (tmpidentifiedNotes.IdentifiedNote[i].Denom)
                        {
                            case 5:
                                Denom5GBPcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 10:
                                Denom10GBPcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 20:
                                Denom20GBPcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 50:
                                Denom50GBPcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 100:
                                Denom100GBPcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                            case 200:
                                Denom200GBPcount += tmpidentifiedNotes.IdentifiedNote[i].Count;
                                break;
                        }
                    }
                }
                for (int k = 0; k < 36; k++)
                {
                    if (Denom5TRLcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom5TRLcount;
                        sortednotelist.IdentifiedNote[k].Currency = "TRL";
                        sortednotelist.IdentifiedNote[k].Denom = 5;
                        Denom5TRLcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom5TRLcount:({0})", Logger.Logger.LogTypes.Critical, Denom5TRLcount.ToString());
                    }
                    else if (Denom10TRLcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom10TRLcount;
                        sortednotelist.IdentifiedNote[k].Currency = "TRL";
                        sortednotelist.IdentifiedNote[k].Denom = 10;
                        Denom10TRLcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom10TRLcount:({0})", Logger.Logger.LogTypes.Critical, Denom10TRLcount.ToString());
                    }
                    else if (Denom20TRLcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom20TRLcount;
                        sortednotelist.IdentifiedNote[k].Currency = "TRL";
                        sortednotelist.IdentifiedNote[k].Denom = 20;
                        Denom20TRLcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom20TRLcount:({0})", Logger.Logger.LogTypes.Critical, Denom20TRLcount.ToString());
                    }
                    else if (Denom50TRLcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom50TRLcount;
                        sortednotelist.IdentifiedNote[k].Currency = "TRL";
                        sortednotelist.IdentifiedNote[k].Denom = 50;
                        Denom50TRLcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom50TRLcount:({0})", Logger.Logger.LogTypes.Critical, Denom50TRLcount.ToString());
                    }
                    else if (Denom100TRLcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom100TRLcount;
                        sortednotelist.IdentifiedNote[k].Currency = "TRL";
                        sortednotelist.IdentifiedNote[k].Denom = 100;
                        Denom100TRLcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom100TRLcount:({0})", Logger.Logger.LogTypes.Critical, Denom100TRLcount.ToString());
                    }
                    else if (Denom200TRLcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom200TRLcount;
                        sortednotelist.IdentifiedNote[k].Currency = "TRL";
                        sortednotelist.IdentifiedNote[k].Denom = 200;
                        Denom200TRLcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom200TRLcount:({0})", Logger.Logger.LogTypes.Critical, Denom200TRLcount.ToString());
                    }
                    else if (Denom5EURcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom5EURcount;
                        sortednotelist.IdentifiedNote[k].Currency = "EUR";
                        sortednotelist.IdentifiedNote[k].Denom = 5;
                        Denom5EURcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom5EURcount:({0})", Logger.Logger.LogTypes.Critical, Denom5EURcount.ToString());
                    }
                    else if (Denom10EURcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom10EURcount;
                        sortednotelist.IdentifiedNote[k].Currency = "EUR";
                        sortednotelist.IdentifiedNote[k].Denom = 10;
                        Denom10EURcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom10EURcount:({0})", Logger.Logger.LogTypes.Critical, Denom10EURcount.ToString());
                    }
                    else if (Denom20EURcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom20EURcount;
                        sortednotelist.IdentifiedNote[k].Currency = "EUR";
                        sortednotelist.IdentifiedNote[k].Denom = 20;
                        Denom20EURcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom20EURcount:({0})", Logger.Logger.LogTypes.Critical, Denom20EURcount.ToString());
                    }
                    else if (Denom50EURcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom50EURcount;
                        sortednotelist.IdentifiedNote[k].Currency = "EUR";
                        sortednotelist.IdentifiedNote[k].Denom = 50;
                        Denom50EURcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom50EURcount:({0})", Logger.Logger.LogTypes.Critical, Denom50EURcount.ToString());
                    }
                    else if (Denom100EURcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom100EURcount;
                        sortednotelist.IdentifiedNote[k].Currency = "EUR";
                        sortednotelist.IdentifiedNote[k].Denom = 100;
                        Denom100EURcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom100EURcount:({0})", Logger.Logger.LogTypes.Critical, Denom100EURcount.ToString());
                    }
                    else if (Denom200EURcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom200EURcount;
                        sortednotelist.IdentifiedNote[k].Currency = "EUR";
                        sortednotelist.IdentifiedNote[k].Denom = 200;
                        Denom200EURcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom200EURcount:({0})", Logger.Logger.LogTypes.Critical, Denom200EURcount.ToString());
                    }
                    else if (Denom5USDcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom5USDcount;
                        sortednotelist.IdentifiedNote[k].Currency = "USD";
                        sortednotelist.IdentifiedNote[k].Denom = 5;
                        Denom5USDcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom5USDcount:({0})", Logger.Logger.LogTypes.Critical, Denom5USDcount.ToString());
                    }
                    else if (Denom10USDcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom10USDcount;
                        sortednotelist.IdentifiedNote[k].Currency = "USD";
                        sortednotelist.IdentifiedNote[k].Denom = 10;
                        Denom10USDcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom10USDcount:({0})", Logger.Logger.LogTypes.Critical, Denom10USDcount.ToString());
                    }
                    else if (Denom20USDcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom20USDcount;
                        sortednotelist.IdentifiedNote[k].Currency = "USD";
                        sortednotelist.IdentifiedNote[k].Denom = 20;
                        Denom20USDcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom20USDcount:({0})", Logger.Logger.LogTypes.Critical, Denom20USDcount.ToString());
                    }
                    else if (Denom50USDcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom50USDcount;
                        sortednotelist.IdentifiedNote[k].Currency = "USD";
                        sortednotelist.IdentifiedNote[k].Denom = 50;
                        Denom50USDcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom50USDcount:({0})", Logger.Logger.LogTypes.Critical, Denom50USDcount.ToString());
                    }
                    else if (Denom100USDcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom100USDcount;
                        sortednotelist.IdentifiedNote[k].Currency = "USD";
                        sortednotelist.IdentifiedNote[k].Denom = 100;
                        Denom100USDcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom100USDcount:({0})", Logger.Logger.LogTypes.Critical, Denom100USDcount.ToString());
                    }
                    else if (Denom200USDcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom200USDcount;
                        sortednotelist.IdentifiedNote[k].Currency = "USD";
                        sortednotelist.IdentifiedNote[k].Denom = 200;
                        Denom200USDcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom200USDcount:({0})", Logger.Logger.LogTypes.Critical, Denom200USDcount.ToString());
                    }
                    else if (Denom5GBPcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom5GBPcount;
                        sortednotelist.IdentifiedNote[k].Currency = "GBP";
                        sortednotelist.IdentifiedNote[k].Denom = 5;
                        Denom5GBPcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom5GBPcount:({0})", Logger.Logger.LogTypes.Critical, Denom5GBPcount.ToString());
                    }
                    else if (Denom10GBPcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom10GBPcount;
                        sortednotelist.IdentifiedNote[k].Currency = "GBP";
                        sortednotelist.IdentifiedNote[k].Denom = 10;
                        Denom10GBPcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom10GBPcount:({0})", Logger.Logger.LogTypes.Critical, Denom10GBPcount.ToString());
                    }
                    else if (Denom20GBPcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom20GBPcount;
                        sortednotelist.IdentifiedNote[k].Currency = "GBP";
                        sortednotelist.IdentifiedNote[k].Denom = 20;
                        Denom20GBPcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom20GBPcount:({0})", Logger.Logger.LogTypes.Critical, Denom20GBPcount.ToString());
                    }
                    else if (Denom50GBPcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom50GBPcount;
                        sortednotelist.IdentifiedNote[k].Currency = "GBP";
                        sortednotelist.IdentifiedNote[k].Denom = 50;
                        Denom50GBPcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom50GBPcount:({0})", Logger.Logger.LogTypes.Critical, Denom50GBPcount.ToString());
                    }
                    else if (Denom100GBPcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom100GBPcount;
                        sortednotelist.IdentifiedNote[k].Currency = "GBP";
                        sortednotelist.IdentifiedNote[k].Denom = 100;
                        Denom100GBPcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom100GBPcount:({0})", Logger.Logger.LogTypes.Critical, Denom100GBPcount.ToString());
                    }
                    else if (Denom200GBPcount != 0)
                    {
                        sortednotelist.IdentifiedNote[k].Count = Denom200GBPcount;
                        sortednotelist.IdentifiedNote[k].Currency = "GBP";
                        sortednotelist.IdentifiedNote[k].Denom = 200;
                        Denom200GBPcount = 0;
                        numofrecognizednotetype += 1;
                        Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, Denom200GBPcount:({0})", Logger.Logger.LogTypes.Critical, Denom200GBPcount.ToString());
                    }
                }
                tmpidentifiedNotes.IdentifiedNote = sortednotelist.IdentifiedNote;
                tmpidentifiedNotes.NumberOfIdentifiedNoteTypes = numofrecognizednotetype;
            }
            Logger.Logger.LogWrite("DesCashIn - CalculateNotesEntered, numofrecognizednotetype:({0})", Logger.Logger.LogTypes.Critical, numofrecognizednotetype.ToString());
            //tmpidentifiedNotes.MaxEscrowCapacity = xfsDevice.MaxItems;
            tmpidentifiedNotes.MaxEscrowCapacity = xfsDevice.MaxItemsOnStacker;
            tmpidentifiedNotes.CurrentEscrowCapacity = total;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            #endregion

        }
        #endregion

        #region PresentNotes
        /// <summary>
        /// Yatırma işleminde tanınan paraların iade edilmesi için kullanılan fonksiyondur. RollBack Methodu çağırılmıştır.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="tray"></param>
        /// <returns>
        /// DeviceInterface.CashInPresentResults.Error;
        /// DeviceInterface.CashInPresentResults.MetalDetectedAfterPresent
        /// DeviceInterface.CashInPresentResults.ShutterAlreadyClosed
        /// DeviceInterface.CashInPresentResults.ShutterAlreadyOpen
        /// DeviceInterface.CashInPresentResults.ShutterNotClosed
        /// DeviceInterface.CashInPresentResults.ShutterOpenError
        /// DeviceInterface.CashInPresentResults.Successfull
        /// DeviceInterface.CashInPresentResults.Timeout
        /// </returns>
        public DeviceInterface.CashInPresentResults PresentNotes(int timeout, int tray)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotes, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            iRetVal = 1;

            if (isResetActionPresent == true)
            {
                Logger.Logger.LogWrite("DesCashIn - PresentNotes, isResetActionPresent = true", Logger.Logger.LogTypes.Development);
                return DeviceInterface.CashInPresentResults.Error;
            }
            if (xfsDevice.LastCashInStatus.TransactionStatus != CIMTransactionStatus.CIM_TRANSACTION_ACTIVE)
            {
                Logger.Logger.LogWrite("DesCashIn - PresentNotes, LastCashInStatus.TransactionStatus Error:{0}", Logger.Logger.LogTypes.Development, xfsDevice.LastCashInStatus.TransactionStatus.ToString());
                return DeviceInterface.CashInPresentResults.Error;
            }

            Logger.Logger.LogWrite("DesCashIn - PresentNotes, CurrentEscrowCapacity:{0}", Logger.Logger.LogTypes.Development, tmpidentifiedNotes.CurrentEscrowCapacity.ToString());
            if (xfsDevice.MaxItemsOnStacker - tmpidentifiedNotes.CurrentEscrowCapacity <= xfsDevice.MaxItems)
            {
                PresentNoteTimeout = timeout * 1000;
            }
            else
            {
                PresentNoteTimeout = 2 * timeout * 1000;
                Logger.Logger.LogWrite("DesCashIn - PresentNotes, PresentNoteTimeout:{0}", Logger.Logger.LogTypes.Development, PresentNoteTimeout.ToString());
            }

            try
            {
                CIMPresentEvent.Reset();
                th = new Thread(new ThreadStart(PresentNotesProcess));
                th.IsBackground = true;
                th.Start();
                CIMPresentEvent.WaitOne();

                xfsDevice.RollbackOK -= new IXFSCashInServiceEvents_RollbackOKEventHandler(PresentNotesProcess_RollbackOK);
                xfsDevice.RollbackFailed -= new IXFSCashInServiceEvents_RollbackFailedEventHandler(PresentNotesProcess_RollbackFailed);
                xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(PresentNotesProcess_Taken);
                xfsDevice.Presented -= new IXFSCashInServiceEvents_PresentedEventHandler(PresentNotesProcess_Presented);
                xfsDevice.Inserted -= new IXFSCashInServiceEvents_InsertedEventHandler(MediaIn_Inserted);
                xfsDevice.InputRefused -= new IXFSCashInServiceEvents_InputRefusedEventHandler(MediaInCashIn_InputRefused);
                xfsDevice.Presented -= new IXFSCashInServiceEvents_PresentedEventHandler(MediaInCashIn_Presented);
                xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(MediaInCashIn_Taken);
                this.CanceledByUser -= new CanceledByUserDelegate(CashIn_CanceledByUser);
                xfsDevice.CashInFailed -= new IXFSCashInServiceEvents_CashInFailedEventHandler(MediaIn_CashInFailed);
                xfsDevice.CashInOK -= new IXFSCashInServiceEvents_CashInOKEventHandler(MediaIn_CashInOK);
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - PresentNotes, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn PresentNotes, Exception");
                //Des.Pascal.CEH.RaiseException(ex);
                iRetVal = 1;
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotes, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            return (DeviceInterface.CashInPresentResults)iRetVal;
        }

        private void PresentNotesProcess()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            try
            {
                xfsDevice.RollbackOK += new IXFSCashInServiceEvents_RollbackOKEventHandler(PresentNotesProcess_RollbackOK);
                xfsDevice.RollbackFailed += new IXFSCashInServiceEvents_RollbackFailedEventHandler(PresentNotesProcess_RollbackFailed);
                xfsDevice.Taken += new IXFSCashInServiceEvents_TakenEventHandler(PresentNotesProcess_Taken);
                xfsDevice.Presented += new IXFSCashInServiceEvents_PresentedEventHandler(PresentNotesProcess_Presented);

                xfsDevice.Rollback();
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn PresentNotesProcess, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void PresentNotesProcess_Presented(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_Presented, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            StartRollbackTimer(PresentNoteTimeout);
            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_MEDIUMFLASH, SiuDev);

            if (xfsDevice.MaxItemsOnStacker - tmpidentifiedNotes.CurrentEscrowCapacity <= xfsDevice.MaxItems)
            {
                if (CashInRollbackPresented != null)
                {
                    CashInRollbackPresented();
                }
            }

            xfsDevice.Presented -= new IXFSCashInServiceEvents_PresentedEventHandler(PresentNotesProcess_Presented);

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_Presented, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        void PresentNotesProcess_Taken(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_Taken, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);
            StopRollbackTimer();
            xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(PresentNotesProcess_Taken);
            iRetVal = 0;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_Taken, Exit", Logger.Logger.LogTypes.Development);
            #endregion
            CIMPresentEvent.Set();
        }

        void PresentNotesProcess_RollbackFailed(int requestID, CIMRollbackError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackFailed, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackFailed, reason : " + reason.ToString(), Logger.Logger.LogTypes.Development);

            xfsDevice.RollbackOK -= new IXFSCashInServiceEvents_RollbackOKEventHandler(PresentNotesProcess_RollbackOK);
            xfsDevice.RollbackFailed -= new IXFSCashInServiceEvents_RollbackFailedEventHandler(PresentNotesProcess_RollbackFailed);
            xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(PresentNotesProcess_Taken);
            xfsDevice.Presented -= new IXFSCashInServiceEvents_PresentedEventHandler(PresentNotesProcess_Presented);

            if (reason == CIMRollbackError.CIM_ROLLBACK_NO_ITEMS)
                iRetVal = 4;
            else
                iRetVal = 1;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackFailed, Exit", Logger.Logger.LogTypes.Development);
            #endregion
            CIMPresentEvent.Set();
        }
        void PresentNotesProcess_RollbackOK(int requestID, XFSDictionary cashInfo, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackOK, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.RollbackOK -= new IXFSCashInServiceEvents_RollbackOKEventHandler(PresentNotesProcess_RollbackOK);
            xfsDevice.RollbackFailed -= new IXFSCashInServiceEvents_RollbackFailedEventHandler(PresentNotesProcess_RollbackFailed);

            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackOK, TransactionStatus : " + xfsDevice.LastCashInStatus.TransactionStatus.ToString(), Logger.Logger.LogTypes.Development);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackOK, NumberRefused : " + xfsDevice.LastCashInStatus.NumberRefused.ToString(), Logger.Logger.LogTypes.Development);
            XFSCashInNoteTypesCount noteTypesCount = new XFSCashInNoteTypesCount();
            XFSDictionary rollbackNoteNumberList = xfsDevice.LastCashInStatus.NoteNumberList;
            object keys = rollbackNoteNumberList.Keys();
            System.Array keyArray = (System.Array)keys;
            int binNum = 0;
            foreach (object keyNoteTypeCount in keyArray)
            {
                Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackOK, NoteNumberList Key : {0}", Logger.Logger.LogTypes.Development, keyNoteTypeCount.ToString());
                object refKeyNoteTypeCount = keyNoteTypeCount;
                noteTypesCount = (XFSCashInNoteTypesCount)rollbackNoteNumberList.get_Item(ref refKeyNoteTypeCount);
                string debug = String.Format("noteTypesCount Count: {0} " + "noteTypesCount NoteID: {1}", noteTypesCount.Count, noteTypesCount.NoteID);
                Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackOK, Critical :({0})", Logger.Logger.LogTypes.Critical, debug);
                binNum++;
            }
            keys = xfsDevice.LastCashInStatus.VendorSpecific.Keys();
            keyArray = (System.Array)keys;
            binNum = 0;
            foreach (object key in keyArray)
            {
                object refKey = key;
                Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackOK, VendorSpecific Key : {0}", Logger.Logger.LogTypes.Development, key.ToString());
                Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackOK, VendorSpecific Item : {0}", Logger.Logger.LogTypes.Development, xfsDevice.LastCashInStatus.VendorSpecific.get_Item(ref refKey).ToString());
                binNum++;
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - PresentNotesProcess_RollbackOK, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        //bayro
        protected virtual void StartRollbackTimer(long dueTime)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name, dueTime);
            Logger.Logger.LogWrite("DesCashIn - StartRollbackTimer, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            RollBackTimer.Interval = dueTime;
            RollBackTimer.Elapsed += new System.Timers.ElapsedEventHandler(RollbackTimedOut);
            RollBackTimer.Start();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StartRollbackTimer, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        protected virtual void StopRollbackTimer()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StopMoneyEntryTimer, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            RollBackTimer.Elapsed -= new System.Timers.ElapsedEventHandler(RollbackTimedOut);
            RollBackTimer.Stop();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StopMoneyEntryTimer, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        public void RollbackTimedOut(object sender, System.Timers.ElapsedEventArgs e)
        {
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RollbackTimedOut, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            #region AlwaysOn PD
            sp.AlwaysOn(MethodBase.GetCurrentMethod().Name, "", Des.Pascal.PDCollection.AlwaysOnCategory.ConsumerAction);
            #endregion

            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_MEDIUMFLASH, SiuDev);
            StopRollbackTimer();
            iRetVal = (int)DeviceInterface.CashInPresentResults.Timeout;


            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RollbackTimedOut, Exit", Logger.Logger.LogTypes.Development);
            #endregion
            CIMPresentEvent.Set();
        }

        #endregion

        #region CashInNotes
        /// <summary>
        /// Geçici kasa bölmesindeki tanınmış paraları kasetleri atmak ve para yatırma işlemini sonlandırmak için 
        /// kullanılan fonksiyondur. CashInEnd methodu thread vasıtası ile çağırılır.
        /// </summary>
        /// <returns>
        /// DeviceInterface.CashInNotesResults.
        /// DeviceInterface.CashInNotesResults.Error
        /// DeviceInterface.CashInNotesResults.NoNotesFound
        /// DeviceInterface.CashInNotesResults.Successfull
        /// </returns>
        public DeviceInterface.CashInNotesResults CashInNotes()
        {
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotes, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            iRetVal = -1;

            try
            {
                CIMEvent.Reset();
                th = new Thread(new ThreadStart(CashInNotesProcess));
                th.IsBackground = true;
                th.Start();
                CIMEvent.WaitOne();

                xfsDevice.CashInEndFailed -= new IXFSCashInServiceEvents_CashInEndFailedEventHandler(CashInNotes_CashInEndFailed);
                xfsDevice.CashInEndOK -= new IXFSCashInServiceEvents_CashInEndOKEventHandler(CashInNotes_CashInEndOK);
                xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(CashInNotes_CashUnitError);
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - CashInNotes, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn CashInNotes, Exception");
                iRetVal = 1;
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotes, Exit", Logger.Logger.LogTypes.Development);
            #endregion
            return (DeviceInterface.CashInNotesResults)iRetVal;
        }

        private void CashInNotesProcess()
        {
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotesProcess, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            try
            {
                xfsDevice.CashInEndFailed += new IXFSCashInServiceEvents_CashInEndFailedEventHandler(CashInNotes_CashInEndFailed);
                xfsDevice.CashInEndOK += new IXFSCashInServiceEvents_CashInEndOKEventHandler(CashInNotes_CashInEndOK);
                xfsDevice.CashUnitError += new IXFSCashInServiceEvents_CashUnitErrorEventHandler(CashInNotes_CashUnitError);
                xfsDevice.CashInEnd();
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - CashInNotesProcess, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn CashInNotesProcess, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotesProcess, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        private int GetDepositedAmountTotal()
        {
            int totalAmount = 0;
            DeviceInterface.CashInIdentifiedNotes cashInIdentifiedNotes = new DeviceInterface.CashInIdentifiedNotes();

            try
            {
                //tür bazında yatırılan küpür adetlerini alır
                GetDepositedNoteTypeCount(cashInIdentifiedNotes);
                Logger.Logger.LogWrite("GetIdentifiedNotes Length=" + cashInIdentifiedNotes.IdentifiedNote.Length.ToString(), Logger.Logger.LogTypes.Development);
                Logger.Logger.LogWrite("GetIdentifiedNotes NumberOfIdentifiedNoteTypes=" + cashInIdentifiedNotes.NumberOfIdentifiedNoteTypes.ToString(), Logger.Logger.LogTypes.Development);
                for (int i = 0; i < cashInIdentifiedNotes.NumberOfIdentifiedNoteTypes; i++)
                {
                    Logger.Logger.LogWrite("count=" + i.ToString() + " ******************************************************", Logger.Logger.LogTypes.Development);
                    Logger.Logger.LogWrite("GetIdentifiedNotes Count=" + cashInIdentifiedNotes.IdentifiedNote[i].Count.ToString(), Logger.Logger.LogTypes.Development);
                    Logger.Logger.LogWrite("GetIdentifiedNotes Currency=" + cashInIdentifiedNotes.IdentifiedNote[i].Currency, Logger.Logger.LogTypes.Development);
                    Logger.Logger.LogWrite("GetIdentifiedNotes Denom=" + cashInIdentifiedNotes.IdentifiedNote[i].Denom.ToString(), Logger.Logger.LogTypes.Development);

                    totalAmount += cashInIdentifiedNotes.IdentifiedNote[i].Count * cashInIdentifiedNotes.IdentifiedNote[i].Denom;
                }

                return totalAmount;
            }
            catch (Exception ex)
            {
                Logger.Logger.LogWrite("DesCashIn - (Exception) GetDepositedAmountTotal exception occured :" + ex.ToString(), Logger.Logger.LogTypes.Exceptions);
                return totalAmount;
            }
        }

        void CashInNotes_CashUnitError(int requestID, XFSCashInCassette cashUnit, CIMCashUnitFailure reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashUnitError, Entry :({0})", Logger.Logger.LogTypes.Exceptions, reason.ToString());
            #endregion

            Logger.Logger.LogWrite("DesCashIn - CashInNotes_NoteError, cashUnit UnitID :({0})", Logger.Logger.LogTypes.Development, cashUnit.UnitID.ToString());
            Logger.Logger.LogWrite("DesCashIn - CashInNotes_NoteError, reason :({0})", Logger.Logger.LogTypes.Development, reason.ToString());

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashUnitError, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void CashInNotes_CashInEndOK(int requestID, XFSDictionary cashInfo, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndOK, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.CashInEndFailed -= new IXFSCashInServiceEvents_CashInEndFailedEventHandler(CashInNotes_CashInEndFailed);
            xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(CashInNotes_CashUnitError);

            iRetVal = 0;
            Logger.Logger.LogWrite("DesCashIn - DEPOSIT TOTAL:" + GetDepositedAmountTotal().ToString(), Logger.Logger.LogTypes.Development);

            Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndOK, TransactionStatus : " + xfsDevice.LastCashInStatus.TransactionStatus.ToString(), Logger.Logger.LogTypes.Development);
            Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndOK, NumberRefused : " + xfsDevice.LastCashInStatus.NumberRefused.ToString(), Logger.Logger.LogTypes.Development);
            XFSCashInNoteTypesCount noteTypesCount = new XFSCashInNoteTypesCount();
            XFSDictionary rollbackNoteNumberList = xfsDevice.LastCashInStatus.NoteNumberList;
            object keys = rollbackNoteNumberList.Keys();
            System.Array keyArray = (System.Array)keys;
            int binNum = 0;
            foreach (object keyNoteTypeCount in keyArray)
            {
                Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndOK, NoteNumberList Key : {0}", Logger.Logger.LogTypes.Development, keyNoteTypeCount.ToString());
                object refKeyNoteTypeCount = keyNoteTypeCount;
                noteTypesCount = (XFSCashInNoteTypesCount)rollbackNoteNumberList.get_Item(ref refKeyNoteTypeCount);
                string debug = String.Format("noteTypesCount Count: {0} " + "noteTypesCount NoteID: {1}", noteTypesCount.Count, noteTypesCount.NoteID);
                Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndOK, Critical :({0})", Logger.Logger.LogTypes.Critical, debug);
                binNum++;
            }
            keys = xfsDevice.LastCashInStatus.VendorSpecific.Keys();
            keyArray = (System.Array)keys;
            binNum = 0;
            foreach (object key in keyArray)
            {
                object refKey = key;
                Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndOK, VendorSpecific Key : {0}", Logger.Logger.LogTypes.Development, key.ToString());
                Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndOK, VendorSpecific Item : {0}", Logger.Logger.LogTypes.Development, xfsDevice.LastCashInStatus.VendorSpecific.get_Item(ref refKey).ToString());
                binNum++;
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndOK, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        void CashInNotes_CashInEndFailed(int requestID, CIMCashInEndError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndFailed, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.CashInEndOK -= new IXFSCashInServiceEvents_CashInEndOKEventHandler(CashInNotes_CashInEndOK);
            xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(CashInNotes_CashUnitError);

            Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndFailed, reason : " + reason.ToString(), Logger.Logger.LogTypes.Development);

            if (reason == CIMCashInEndError.CIM_CASHINEND_NO_ITEMS)
            {
                iRetVal = 4;
            }
            else
            {
                iRetVal = 1;
            }
            Logger.Logger.LogWrite("DesCashIn - (Failed)DEPOSIT TOTAL:" + GetDepositedAmountTotal().ToString(), Logger.Logger.LogTypes.Development);

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashInNotes_CashInEndFailed, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        #endregion

        #region RollBackNotes
        /// <summary>
        /// Paraları müşteriye geri vermek için kullanılan fonksiyondur.
        /// </summary>
        /// <returns>
        /// DeviceInterface.CashInRollbackResults.Error
        /// DeviceInterface.CashInRollbackResults.NoNotesFound
        /// DeviceInterface.CashInRollbackResults.PresentTrayNotEmpty
        /// DeviceInterface.CashInRollbackResults.Successfull
        /// </returns>
        public DeviceInterface.CashInRollbackResults RollBackNotes()
        {
            iRetVal = -1;
            return (DeviceInterface.CashInRollbackResults)iRetVal;
        }
        #endregion

        #region RetractNotes
        /// <summary>
        /// Para yatırma bölmesi girişinde alınmayan paraları capture etmek için kullanılan fonksiyondur.
        /// </summary>
        /// <returns>
        /// DeviceInterface.CashInRetractResults.Error
        /// DeviceInterface.CashInRetractResults.MaximumRetractCountReached
        /// DeviceInterface.CashInRetractResults.MetalDetected
        /// DeviceInterface.CashInRetractResults.NoNotesFound
        /// DeviceInterface.CashInRetractResults.NotesTakenByCustomer
        /// DeviceInterface.CashInRetractResults.ShutterNotClosed
        /// DeviceInterface.CashInRetractResults.Successfull
        /// </returns>
        public DeviceInterface.CashInRetractResults RetractNotes()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            iRetVal = 1;

            if (xfsDevice.LastCashInStatus.TransactionStatus == CIMTransactionStatus.CIM_TRANSACTION_OK ||
                xfsDevice.LastCashInStatus.TransactionStatus == CIMTransactionStatus.CIM_TRANSACTION_RETRACTED)
            {
                Logger.Logger.LogWrite("DesCashIn - RetractNotes, LastCashInStatus.TransactionStatus Error:{0}", Logger.Logger.LogTypes.Development, xfsDevice.LastCashInStatus.TransactionStatus.ToString());
                return DeviceInterface.CashInRetractResults.Error;
            }

            if (xfsDevice.DeviceStatusObject.DeviceStatus == XFSStatus.XFS_STATUS_USER_INTERFERENCE)
            {
                Logger.Logger.LogWrite("DesCashIn - RetractNotes, DeviceStatus = " + xfsDevice.DeviceStatusObject.DeviceStatus.ToString(), Logger.Logger.LogTypes.Development);
                return DeviceInterface.CashInRetractResults.Error;
            }

            retractItemsTaken = false;
            retractNoItems = false;

            try
            {
                CIMEvent.Reset();
                th = new Thread(new ThreadStart(RetractNotesProcess));
                th.IsBackground = true;
                th.Start();
                CIMEvent.WaitOne();

                xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(RetractNotes_CashUnitError);
                xfsDevice.NoteError -= new IXFSCashInServiceEvents_NoteErrorEventHandler(RetractNotes_NoteError);
                xfsDevice.RetractOK -= new IXFSCashInServiceEvents_RetractOKEventHandler(RetractNotes_RetractOK);
                xfsDevice.RetractFailed -= new IXFSCashInServiceEvents_RetractFailedEventHandler(RetractNotes_RetractFailed);
                xfsDevice.XFSErrorEvent -= new IXFSCashInServiceEvents_XFSErrorEventEventHandler(RetractNotes_XFSErrorEvent);
                xfsDevice.MediaDetected -= new IXFSCashInServiceEvents_MediaDetectedEventHandler(Retract_MediaDetected);

            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - RetractNotes, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn RetractNotes, Exception");
                iRetVal = 1;
            }
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes, Exit", Logger.Logger.LogTypes.Development);
            #endregion
            return (DeviceInterface.CashInRetractResults)iRetVal;
        }
        private void RetractNotesProcess()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotesProcess, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            XFSCashInRetract cashInRetract = new XFSCashInRetract();

            // Find the first Retract cassette
            XFSDictionary bins = xfsDevice.CashUnits;

            try
            {

                if (bins != null)
                {
                    int retractBinNum;
                    XFSCashInCassette bin;

                    retractBinNum = -1;

                    object keys = bins.Keys();
                    System.Array keyArray = (System.Array)keys;
                    int binNum = 0;

                    // Get retract bin number
                    previousRetractedCount = 0;
                    foreach (object key in keyArray)
                    {
                        object refKey = key;

                        bin = (XFSCashInCassette)bins.get_Item(ref refKey);
                        binNum++;
                        if (bin.UnitType == CIMCassetteType.CIM_TYPE_RETRACT_CASSETTE)
                        {
                            retractBinNum = binNum;
                            if (IsAtmDiebold == false && RecyclingPresent == false)
                            {
                                previousRetractedCount = bin.UnitInCount;
                                Logger.Logger.LogWrite("DesCashIn - cashInRetract, previousReractedCount :({0})", Logger.Logger.LogTypes.Development, previousRetractedCount.ToString());
                            }
                            break;
                        }

                    }

                    //cashInRetract.Index = (short)retractBinNum;
                    //cashInRetract.OutputPosition = CIMPosition.CIM_POSITION_DEFAULT;
                    //cashInRetract.RetractArea = CIMRetractArea.CIM_TO_RETRACT;

                    cashInRetract.Index = 0;
                    cashInRetract.OutputPosition = CIMPosition.CIM_POSITION_DEFAULT;
                    cashInRetract.RetractArea = CIMRetractArea.CIM_TO_CASSETTE;

                    if (IsAtmDiebold)
                    {
                        cashInRetract.Index = 1;
                        cashInRetract.OutputPosition = (CIMPosition)(-1);
                        cashInRetract.RetractArea = CIMRetractArea.CIM_TO_RETRACT;
                    }
                }

                xfsDevice.CashUnitError += new IXFSCashInServiceEvents_CashUnitErrorEventHandler(RetractNotes_CashUnitError);
                xfsDevice.NoteError += new IXFSCashInServiceEvents_NoteErrorEventHandler(RetractNotes_NoteError);
                xfsDevice.RetractOK += new IXFSCashInServiceEvents_RetractOKEventHandler(RetractNotes_RetractOK);
                xfsDevice.RetractFailed += new IXFSCashInServiceEvents_RetractFailedEventHandler(RetractNotes_RetractFailed);
                xfsDevice.XFSErrorEvent += new IXFSCashInServiceEvents_XFSErrorEventEventHandler(RetractNotes_XFSErrorEvent);
                xfsDevice.MediaDetected += new IXFSCashInServiceEvents_MediaDetectedEventHandler(Retract_MediaDetected);

                Logger.Logger.LogWrite("DesCashIn - cashInRetract, RetractArea :({0})", Logger.Logger.LogTypes.Development, cashInRetract.RetractArea.ToString());
                Logger.Logger.LogWrite("DesCashIn - cashInRetract, Index :({0})", Logger.Logger.LogTypes.Development, cashInRetract.Index.ToString());
                Logger.Logger.LogWrite("DesCashIn - cashInRetract, OutputPosition :({0})", Logger.Logger.LogTypes.Development, cashInRetract.OutputPosition.ToString());


                xfsDevice.Retract(cashInRetract);
                Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);

                Logger.Logger.LogWrite("DesCashIn - cashInRetract, Command", Logger.Logger.LogTypes.Development);

            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - RetractNotesProcess, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn RetractNotesProcess, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotesProcess, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        void RetractNotes_XFSErrorEvent(IXFSError error, DateTime timeStamp)
        {

            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_XFSErrorEvent, Entry", Logger.Logger.LogTypes.Development);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_XFSErrorEvent, Error :({0})", Logger.Logger.LogTypes.Exceptions, error.XFSError.ToString());
            #endregion

            switch (error.XFSError)
            {
                case XFSErrorStatus.XFS_ERR_CIM_ITEMSTAKEN:
                    retractItemsTaken = true;
                    break;
                case XFSErrorStatus.XFS_ERR_CIM_NOITEMS:
                    retractNoItems = true;
                    break;
                default:
                    break;
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_XFSErrorEvent, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void RetractNotes_NoteError(int requestID, CIMNoteError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_NoteError, Entry :({0})", Logger.Logger.LogTypes.Exceptions, reason.ToString());
            #endregion

            resetMediaDetected = true;

            Logger.Logger.LogWrite("DesCashIn - RetractNotes_NoteError, reason :({0})", Logger.Logger.LogTypes.Development, reason.ToString());

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_NoteError, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void RetractNotes_CashUnitError(int requestID, XFSCashInCassette cashUnit, CIMCashUnitFailure reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_CashUnitError, Entry :({0})", Logger.Logger.LogTypes.Exceptions, reason.ToString());
            #endregion

            resetMediaDetected = true;

            Logger.Logger.LogWrite("DesCashIn - RetractNotes_NoteError, cashUnit UnitID :({0})", Logger.Logger.LogTypes.Development, cashUnit.UnitID.ToString());
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_NoteError, reason :({0})", Logger.Logger.LogTypes.Development, reason.ToString());

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_CashUnitError, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void Retract_MediaDetected(int requestID, bool movedSuccessfully, short cashUnitID, XFSCashInRetract retractInfo, CIMPosition Position, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Retract_MediaDetected, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            resetMediaDetected = true;

            Logger.Logger.LogWrite("DesCashIn - Retract_MediaDetected, movedSuccessfully :({0})", Logger.Logger.LogTypes.Development, movedSuccessfully.ToString());
            Logger.Logger.LogWrite("DesCashIn - Retract_MediaDetected, cashUnitID :({0})", Logger.Logger.LogTypes.Development, cashUnitID.ToString());
            Logger.Logger.LogWrite("DesCashIn - Retract_MediaDetected, Position :({0})", Logger.Logger.LogTypes.Development, Position.ToString());
            Logger.Logger.LogWrite("DesCashIn - Retract_MediaDetected, RetractArea :({0})", Logger.Logger.LogTypes.Development, retractInfo.RetractArea.ToString());
            Logger.Logger.LogWrite("DesCashIn - Retract_MediaDetected, Index :({0})", Logger.Logger.LogTypes.Development, retractInfo.Index.ToString());
            Logger.Logger.LogWrite("DesCashIn - Retract_MediaDetected, OutputPosition :({0})", Logger.Logger.LogTypes.Development, retractInfo.OutputPosition.ToString());


            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Retract_MediaDetected, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void RetractNotes_RetractFailed(int requestID, CIMRetractError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_RetractFailed, Entry :({0})", Logger.Logger.LogTypes.Exceptions, reason.ToString());
            #endregion

            xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(RetractNotes_CashUnitError);
            xfsDevice.NoteError -= new IXFSCashInServiceEvents_NoteErrorEventHandler(RetractNotes_NoteError);
            xfsDevice.RetractOK -= new IXFSCashInServiceEvents_RetractOKEventHandler(RetractNotes_RetractOK);
            xfsDevice.RetractFailed -= new IXFSCashInServiceEvents_RetractFailedEventHandler(RetractNotes_RetractFailed);
            xfsDevice.XFSErrorEvent -= new IXFSCashInServiceEvents_XFSErrorEventEventHandler(RetractNotes_XFSErrorEvent);
            xfsDevice.MediaDetected -= new IXFSCashInServiceEvents_MediaDetectedEventHandler(Retract_MediaDetected);

            Logger.Logger.LogWrite("DesCashIn - RetractNotes_RetractFailed, reason :({0})", Logger.Logger.LogTypes.Development, reason.ToString());

            switch (reason)
            {
                case CIMRetractError.CIM_RETRACT_ITEMS_TAKEN:
                    retractItemsTaken = true;
                    break;
                case CIMRetractError.CIM_RETRACT_NO_ITEMS:
                    retractNoItems = true;
                    break;
                default:
                    break;
            }

            if (retractNoItems)
                iRetVal = 4;
            else if (retractItemsTaken)
                iRetVal = 10;
            else
                iRetVal = 1;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_RetractFailed, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        void RetractNotes_RetractOK(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_RetractOK, Entry", Logger.Logger.LogTypes.Development);
            #endregion


            xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(RetractNotes_CashUnitError);
            xfsDevice.NoteError -= new IXFSCashInServiceEvents_NoteErrorEventHandler(RetractNotes_NoteError);
            xfsDevice.RetractOK -= new IXFSCashInServiceEvents_RetractOKEventHandler(RetractNotes_RetractOK);
            xfsDevice.RetractFailed -= new IXFSCashInServiceEvents_RetractFailedEventHandler(RetractNotes_RetractFailed);
            xfsDevice.XFSErrorEvent -= new IXFSCashInServiceEvents_XFSErrorEventEventHandler(RetractNotes_XFSErrorEvent);
            xfsDevice.MediaDetected -= new IXFSCashInServiceEvents_MediaDetectedEventHandler(Retract_MediaDetected);

            if (retractNoItems)
                iRetVal = 4;
            else if (retractItemsTaken)
                iRetVal = 10;
            else
                iRetVal = 0;

            try
            {
                if (iRetVal == 0 && IsAtmDiebold == false && xfsDevice.LastCashInStatus.TransactionStatus == CIMTransactionStatus.CIM_TRANSACTION_RETRACTED)
                {
                    Logger.Logger.LogWrite("DesCashIn - RetractNotes CIM_TRANSACTION_RETRACTED, Entry", Logger.Logger.LogTypes.Development);
                    retractedDict.RemoveAll();
                    XFSDictionary retractVendorSpecific = xfsDevice.LastCashInStatus.VendorSpecific;
                    object keys = retractVendorSpecific.Keys();
                    System.Array keyArray = (System.Array)keys;
                    if (RecyclingPresent == true && retractVendorSpecific != null) //Des Recycle
                    {
                        Logger.Logger.LogWrite("DesCashIn - RetractNotes retractVendorSpecific OK", Logger.Logger.LogTypes.Development);

                        int binNum = 0;
                        string retractVendorSpecificItem = "";
                        string retractVendorSpecificKey = "";
                        foreach (object key in keyArray)
                        {
                            object refKey = key;
                            Logger.Logger.LogWrite("DesCashIn - RetractNotes, VendorSpecific Key : {0}", Logger.Logger.LogTypes.Development, key.ToString());
                            retractVendorSpecificKey = key.ToString();
                            retractVendorSpecificItem = retractVendorSpecific.get_Item(ref refKey).ToString();
                            Logger.Logger.LogWrite("DesCashIn - RetractNotes, VendorSpecific Item : {0}", Logger.Logger.LogTypes.Development, retractVendorSpecificItem);
                            binNum++;
                        }

                        if (retractVendorSpecificKey == "Last Retract")
                        {
                            Logger.Logger.LogWrite("DesCashIn - RetractNotes Last Retract OK", Logger.Logger.LogTypes.Development);

                            if (retractVendorSpecificItem == "0")
                            {
                                Logger.Logger.LogWrite("DesCashIn - RetractNotes Last Retract No notes found", Logger.Logger.LogTypes.Development);
                                iRetVal = 4;
                            }
                            else
                            {
                                string[] retractVendorSpecificArray = retractVendorSpecificItem.Split(':');
                                int retractedKey = 0;
                                lastRejectedNoteCount = Convert.ToInt16(retractVendorSpecificArray[0]);
                                for (int i = 1; i < retractVendorSpecificArray.Length; i++)
                                {
                                    XFSCashInNoteTypesCount retractedItem = new XFSCashInNoteTypesCount();

                                    Logger.Logger.LogWrite("DesCashIn - RetractNotes retractVendorSpecificArray OK", Logger.Logger.LogTypes.Development);
                                    string[] retractVendorSpecificCountArray = retractVendorSpecificArray[i].Split(',');
                                    int retractedCount = 0;
                                    short retractedNoteID = 0;

                                    retractedNoteID = Convert.ToInt16(retractVendorSpecificCountArray[0]);
                                    Logger.Logger.LogWrite("DesCashIn - RetractNotes retractVendorSpecificArray retractedNoteID : " + retractedNoteID.ToString(), Logger.Logger.LogTypes.Development);
                                    for (int j = 1; j < retractVendorSpecificCountArray.Length; j++)
                                    {
                                        retractedCount = retractedCount + Convert.ToInt32(retractVendorSpecificCountArray[j]);
                                    }
                                    Logger.Logger.LogWrite("DesCashIn - RetractNotes retractVendorSpecificArray retractedCount : " + retractedCount.ToString(), Logger.Logger.LogTypes.Development);
                                    retractedKey++;

                                    object retractedUnitKey = retractedKey;
                                    Logger.Logger.LogWrite("DesCashIn - RetractNotes retractVendorSpecificArray retractedKey : " + retractedKey.ToString(), Logger.Logger.LogTypes.Development);
                                    retractedItem.Count = retractedCount;
                                    retractedItem.NoteID = retractedNoteID;
                                    object retractedUnitItem = retractedItem;
                                    retractedDict.Add(ref retractedUnitKey, ref retractedUnitItem);
                                }
                                useRetractedDict = true;
                            }
                        }
                        else
                        {
                            Logger.Logger.LogWrite("DesCashIn - RetractNotes No Last Retract found", Logger.Logger.LogTypes.Development);
                            iRetVal = 1;
                        }
                    }
                    else //Des BNA
                    {
                        retractedDict = xfsDevice.LastCashInStatus.NoteNumberList;

                        int totalRetractedCount = 0;
                        XFSCashInCassette cashUnitCassetteType;
                        XFSDictionary retractCashUnitsDict = xfsDevice.CashUnits;
                        object retractCashUnitsKeys = retractCashUnitsDict.Keys();
                        System.Array keyArrayRetractCashUnitsKeys = (System.Array)retractCashUnitsKeys;
                        foreach (object retractCashUnits in keyArrayRetractCashUnitsKeys)
                        {
                            object refRetractCashUnits = retractCashUnits;
                            cashUnitCassetteType = (XFSCashInCassette)retractCashUnitsDict.get_Item(ref refRetractCashUnits);
                            Logger.Logger.LogWrite("DesCashIn - RetractNotes:CashUnits.UnitType :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.UnitType.ToString());
                            if (cashUnitCassetteType.UnitType == CIMCassetteType.CIM_TYPE_RETRACT_CASSETTE)
                            {
                                Logger.Logger.LogWrite("DesCashIn - RetractNotes UnitInCount :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.UnitInCount.ToString());
                                totalRetractedCount = totalRetractedCount + cashUnitCassetteType.UnitInCount;
                                break;
                            }
                        }
                        Logger.Logger.LogWrite("DesCashIn - RetractNotes RetractedNoteTypeCount retractedTotalCount : " + totalRetractedCount.ToString(), Logger.Logger.LogTypes.Development);

                        lastRejectedNoteCount = totalRetractedCount - previousRetractedCount;
                        Logger.Logger.LogWrite("DesCashIn - RetractNotes RetractedNoteTypeCount UnIdentifiedCount : " + lastRejectedNoteCount.ToString(), Logger.Logger.LogTypes.Development);

                        useRetractedDict = true;
                    }
                    Logger.Logger.LogWrite("DesCashIn - RetractNotes CIM_TRANSACTION_RETRACTED, Exit", Logger.Logger.LogTypes.Development);
                }
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - RetractNotes_RetractOk, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion

                iRetVal = 1;
                CIMEvent.Set();
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - RetractNotes_RetractOK, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        #endregion

        #region Reset
        /// <summary>
        /// Para yatırma ünitesine Reset veya Clear komutu göndermek için kullanılan fonksiyondur.
        /// </summary>
        /// DeviceInterface.CashInGeneralResults.Error
        /// DeviceInterface.CashInGeneralResults.OK
        /// <returns></returns>
        public DeviceInterface.CashInResetResults Reset()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Reset, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            if (xfsDevice.LastCashInStatus.TransactionStatus == CIMTransactionStatus.CIM_TRANSACTION_ACTIVE)
            {
                Logger.Logger.LogWrite("DesCashIn - Reset, LastCashInStatus.TransactionStatus Error:{0}", Logger.Logger.LogTypes.Development, xfsDevice.LastCashInStatus.TransactionStatus.ToString());
                return DeviceInterface.CashInResetResults.Error;
            }

            iRetVal = -1;

            try
            {
                CIMEvent.Reset();
                th = new Thread(new ThreadStart(ResetProcess));
                th.IsBackground = true;
                th.Start();
                CIMEvent.WaitOne();

                xfsDevice.ResetOK -= new IXFSCashInServiceEvents_ResetOKEventHandler(xfsDevice_ResetOK);
                xfsDevice.ResetFailed -= new IXFSCashInServiceEvents_ResetFailedEventHandler(xfsDevice_ResetFailed);
                xfsDevice.MediaDetected -= new IXFSCashInServiceEvents_MediaDetectedEventHandler(Reset_MediaDetected);
                xfsDevice.CashUnitThreshold -= new IXFSCashInServiceEvents_CashUnitThresholdEventHandler(Reset_CashUnitThreshold);
                xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(Reset_CashUnitError);
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - Reset, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn Reset, Exception");
                iRetVal = 1;
            }
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Reset, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            return (DeviceInterface.CashInResetResults)iRetVal;
        }
        private void ResetProcess()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ResetProcess, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            try
            {
                XFSCashInRetract cashInRetract = new XFSCashInRetract();
                CIMRetractArea notesRetractArea;
                int cashUnitNum = 0;

                // Find the first Retract cassette
                XFSDictionary bins = xfsDevice.CashUnits;

                if (bins != null)
                {
                    int retractBinNum;
                    XFSCashInCassette bin;

                    retractBinNum = -1;

                    object keys = bins.Keys();
                    System.Array keyArray = (System.Array)keys;
                    int binNum = 0;

                    // Get retract bin number
                    foreach (object key in keyArray)
                    {
                        object refKey = key;

                        bin = (XFSCashInCassette)bins.get_Item(ref refKey);
                        binNum++;
                        if (bin.UnitType == CIMCassetteType.CIM_TYPE_RETRACT_CASSETTE)
                        {
                            retractBinNum = binNum;
                            break;
                        }

                    }

                    // Assume default position is Retract Bin
                    notesRetractArea = CIMRetractArea.CIM_TO_RETRACT;
                    cashUnitNum = retractBinNum;

                    cashInRetract.Index = (short)cashUnitNum;
                    cashInRetract.OutputPosition = CIMPosition.CIM_POSITION_DEFAULT;
                    cashInRetract.RetractArea = notesRetractArea;

                    if (IsAtmDiebold)
                    {
                        int position = -1;
                        cashInRetract.Index = 1;
                        cashInRetract.OutputPosition = (CIMPosition)position;
                        cashInRetract.RetractArea = CIMRetractArea.CIM_TO_RETRACT;
                        cashUnitNum = 2;
                    }
                }
                xfsDevice.ResetOK += new IXFSCashInServiceEvents_ResetOKEventHandler(xfsDevice_ResetOK);
                xfsDevice.ResetFailed += new IXFSCashInServiceEvents_ResetFailedEventHandler(xfsDevice_ResetFailed);
                xfsDevice.MediaDetected += new IXFSCashInServiceEvents_MediaDetectedEventHandler(Reset_MediaDetected);
                xfsDevice.CashUnitThreshold += new IXFSCashInServiceEvents_CashUnitThresholdEventHandler(Reset_CashUnitThreshold);
                xfsDevice.CashUnitError += new IXFSCashInServiceEvents_CashUnitErrorEventHandler(Reset_CashUnitError);

                Logger.Logger.LogWrite("DesCashIn - Reset, RetractArea :({0})", Logger.Logger.LogTypes.Development, cashInRetract.RetractArea.ToString());
                Logger.Logger.LogWrite("DesCashIn - Reset, Index :({0})", Logger.Logger.LogTypes.Development, cashInRetract.Index.ToString());
                Logger.Logger.LogWrite("DesCashIn - Reset, OutputPosition :({0})", Logger.Logger.LogTypes.Development, cashInRetract.OutputPosition.ToString());

                xfsDevice.Reset((short)cashUnitNum, cashInRetract, cashInRetract.OutputPosition);

            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - ResetProcess, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn ResetProcess, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ResetProcess, Exit", Logger.Logger.LogTypes.Development);
            #endregion

        }

        void Reset_CashUnitThreshold(XFSCashInCassette cassette, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Reset_CashUnitThreshold, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            resetMediaDetected = true;

            Logger.Logger.LogWrite("DesCashIn - Reset_CashUnitThreshold, CassetteStatus :({0})", Logger.Logger.LogTypes.Development, cassette.CassetteStatus.ToString());
            Logger.Logger.LogWrite("DesCashIn - Resete_CashUnitThreshold, CassetteReplenishmentStatus :({0})", Logger.Logger.LogTypes.Development, cassette.CassetteReplenishmentStatus.ToString());

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Reset_CashUnitThreshold, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void Reset_CashUnitError(int requestID, XFSCashInCassette cashUnit, CIMCashUnitFailure reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Reset_CashUnitError, Entry :({0})", Logger.Logger.LogTypes.Exceptions, reason.ToString());
            #endregion

            resetMediaDetected = true;

            Logger.Logger.LogWrite("DesCashIn - Reset_CashUnitError, cashUnit UnitID :({0})", Logger.Logger.LogTypes.Development, cashUnit.UnitID.ToString());
            Logger.Logger.LogWrite("DesCashIn - Reset_CashUnitError, reason :({0})", Logger.Logger.LogTypes.Development, reason.ToString());

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Reset_CashUnitError, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void Reset_MediaDetected(int requestID, bool movedSuccessfully, short cashUnitID, XFSCashInRetract retractInfo, CIMPosition Position, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Reset_MediaDetected, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            resetMediaDetected = true;

            Logger.Logger.LogWrite("DesCashIn - Reset_MediaDetected, movedSuccessfully :({0})", Logger.Logger.LogTypes.Development, movedSuccessfully.ToString());
            Logger.Logger.LogWrite("DesCashIn - Reset_MediaDetected, cashUnitID :({0})", Logger.Logger.LogTypes.Development, cashUnitID.ToString());
            Logger.Logger.LogWrite("DesCashIn - Reset_MediaDetected, Position :({0})", Logger.Logger.LogTypes.Development, Position.ToString());
            Logger.Logger.LogWrite("DesCashIn - Reset_MediaDetected, RetractArea :({0})", Logger.Logger.LogTypes.Development, retractInfo.RetractArea.ToString());
            Logger.Logger.LogWrite("DesCashIn - Reset_MediaDetected, Index :({0})", Logger.Logger.LogTypes.Development, retractInfo.Index.ToString());
            Logger.Logger.LogWrite("DesCashIn - Reset_MediaDetected, OutputPosition :({0})", Logger.Logger.LogTypes.Development, retractInfo.OutputPosition.ToString());


            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - Reset_MediaDetected, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void xfsDevice_ResetFailed(int requestID, CIMResetError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ResetFailed, Entry", Logger.Logger.LogTypes.Development);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ResetFailed, Reason :({0})", Logger.Logger.LogTypes.Exceptions, reason.ToString());
            #endregion

            xfsDevice.ResetOK -= new IXFSCashInServiceEvents_ResetOKEventHandler(xfsDevice_ResetOK);
            xfsDevice.ResetFailed -= new IXFSCashInServiceEvents_ResetFailedEventHandler(xfsDevice_ResetFailed);
            xfsDevice.MediaDetected -= new IXFSCashInServiceEvents_MediaDetectedEventHandler(Reset_MediaDetected);
            xfsDevice.CashUnitThreshold -= new IXFSCashInServiceEvents_CashUnitThresholdEventHandler(Reset_CashUnitThreshold);
            xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(Reset_CashUnitError);

            iRetVal = (int)DeviceInterface.CashInResetResults.Error;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ResetFailed, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        void xfsDevice_ResetOK(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ResetOK, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.ResetOK -= new IXFSCashInServiceEvents_ResetOKEventHandler(xfsDevice_ResetOK);
            xfsDevice.ResetFailed -= new IXFSCashInServiceEvents_ResetFailedEventHandler(xfsDevice_ResetFailed);
            xfsDevice.MediaDetected -= new IXFSCashInServiceEvents_MediaDetectedEventHandler(Reset_MediaDetected);
            xfsDevice.CashUnitThreshold -= new IXFSCashInServiceEvents_CashUnitThresholdEventHandler(Reset_CashUnitThreshold);
            xfsDevice.CashUnitError -= new IXFSCashInServiceEvents_CashUnitErrorEventHandler(Reset_CashUnitError);

            if (resetMediaDetected)
                iRetVal = (int)DeviceInterface.CashInResetResults.Successfull;
            else
                iRetVal = (int)DeviceInterface.CashInResetResults.NoNotesFound;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ResetOK, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();

        }
        #endregion

        /***************************/
        #region ForceRollBackNotes
        /// <summary>
        /// Para yatırma ünitesine Reset veya Clear komutu göndermek için kullanılan fonksiyondur.
        /// </summary>
        /// DeviceInterface.CashInForceRollBackNotesResults.Error
        /// DeviceInterface.CashInForceRollBackNotesResults.Successfull
        /// DeviceInterface.CashInForceRollBackNotesResults.Successfull
        /// <returns></returns>
        /// 
        public DeviceInterface.CashInResetResults ForceRollBackNotes(int timeout, int tray)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ForceRollBack, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            if (isResetActionPresent == false)
            {
                if (xfsDevice.LastCashInStatus.TransactionStatus == CIMTransactionStatus.CIM_TRANSACTION_ACTIVE)
                {
                    Logger.Logger.LogWrite("DesCashIn - ForceRollBack, LastCashInStatus.TransactionStatus Error:{0}", Logger.Logger.LogTypes.Development, xfsDevice.LastCashInStatus.TransactionStatus.ToString());
                    return DeviceInterface.CashInResetResults.Error;
                }
            }

            xfsDevice.CancelAllOperations();
            resetMovedSuccessfully = false;

            iRetVal = 1;

            try
            {
                CIMEvent.Reset();
                th = new Thread(new ThreadStart(ForceRollBackProcess));
                th.IsBackground = true;
                th.Start();
                CIMEvent.WaitOne();

                xfsDevice.ResetOK -= new IXFSCashInServiceEvents_ResetOKEventHandler(xfsDevice_ForceRollBackOK);
                xfsDevice.ResetFailed -= new IXFSCashInServiceEvents_ResetFailedEventHandler(xfsDevice_ForceRollBackFailed);
                xfsDevice.MediaDetected -= new IXFSCashInServiceEvents_MediaDetectedEventHandler(xfsDevice_ForceRollBackMediaDetected);
                xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(ForceRollBack_Taken);
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - ForceRollBack, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn ForceRollBack, Exception");
                iRetVal = 1;
            }
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ForceRollBack, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.CancelAllOperations();

            return (DeviceInterface.CashInResetResults)iRetVal;
        }
        private void ForceRollBackProcess()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ForceRollBackProcess, Entry", Logger.Logger.LogTypes.Development);
            #endregion
            try
            {
                xfsDevice.ResetOK += new IXFSCashInServiceEvents_ResetOKEventHandler(xfsDevice_ForceRollBackOK);
                xfsDevice.ResetFailed += new IXFSCashInServiceEvents_ResetFailedEventHandler(xfsDevice_ForceRollBackFailed);
                xfsDevice.MediaDetected += new IXFSCashInServiceEvents_MediaDetectedEventHandler(xfsDevice_ForceRollBackMediaDetected);
                xfsDevice.Taken += new IXFSCashInServiceEvents_TakenEventHandler(ForceRollBack_Taken);

                XFSCashInRetract cashInRetract = new XFSCashInRetract();
                cashInRetract.RetractArea = CIMRetractArea.CIM_TO_TRANSPORT;
                cashInRetract.Index = 0;
                cashInRetract.OutputPosition = CIMPosition.CIM_POSITION_DEFAULT;

                Logger.Logger.LogWrite("DesCashIn - ForceRollBack_XFSErrorEvent, RetractArea :({0})", Logger.Logger.LogTypes.Development, cashInRetract.RetractArea.ToString());
                Logger.Logger.LogWrite("DesCashIn - ForceRollBack_XFSErrorEvent, Index :({0})", Logger.Logger.LogTypes.Development, cashInRetract.Index.ToString());
                Logger.Logger.LogWrite("DesCashIn - ForceRollBack_XFSErrorEvent, OutputPosition :({0})", Logger.Logger.LogTypes.Development, cashInRetract.OutputPosition.ToString());

                xfsDevice.Reset(0, cashInRetract, CIMPosition.CIM_POSITION_FRONT);
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - ForceRollBackProcess, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn ForceRollBackProcess, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ForceRollBackProcess, Exit", Logger.Logger.LogTypes.Development);
            #endregion

        }

        void xfsDevice_ForceRollBackMediaDetected(int requestID, bool movedSuccessfully, short cashUnitID, XFSCashInRetract retractInfo, CIMPosition Position, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackMediaDetected, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            if (movedSuccessfully)
            {
                resetMovedSuccessfully = true;
            }
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackMediaDetected, movedSuccessfully :({0})", Logger.Logger.LogTypes.Development, movedSuccessfully.ToString());
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackMediaDetected, cashUnitID :({0})", Logger.Logger.LogTypes.Development, cashUnitID.ToString());
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackMediaDetected, Position :({0})", Logger.Logger.LogTypes.Development, Position.ToString());
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackMediaDetected, RetractArea :({0})", Logger.Logger.LogTypes.Development, retractInfo.RetractArea.ToString());
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackMediaDetected, Index :({0})", Logger.Logger.LogTypes.Development, retractInfo.Index.ToString());
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackMediaDetected, OutputPosition :({0})", Logger.Logger.LogTypes.Development, retractInfo.OutputPosition.ToString());


            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackMediaDetected, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        void ForceRollBack_Taken(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ForceRollBack_Taken, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_OFF, SiuDev);
            StopForceRollbackTimer();
            xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(ForceRollBack_Taken);

            iRetVal = iRetVal = (int)DeviceInterface.CashInResetResults.Successfull;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ForceRollBack_Taken, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        void xfsDevice_ForceRollBackFailed(int requestID, CIMResetError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackFailed, Entry", Logger.Logger.LogTypes.Development);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackFailed, Reason :({0})", Logger.Logger.LogTypes.Exceptions, reason.ToString());
            #endregion

            xfsDevice.ResetOK -= new IXFSCashInServiceEvents_ResetOKEventHandler(xfsDevice_ForceRollBackOK);
            xfsDevice.ResetFailed -= new IXFSCashInServiceEvents_ResetFailedEventHandler(xfsDevice_ForceRollBackFailed);
            xfsDevice.MediaDetected -= new IXFSCashInServiceEvents_MediaDetectedEventHandler(xfsDevice_ForceRollBackMediaDetected);
            xfsDevice.Taken -= new IXFSCashInServiceEvents_TakenEventHandler(ForceRollBack_Taken);

            iRetVal = iRetVal = (int)DeviceInterface.CashInResetResults.Error;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackFailed, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        void xfsDevice_ForceRollBackOK(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackOK, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.ResetOK -= new IXFSCashInServiceEvents_ResetOKEventHandler(xfsDevice_ForceRollBackOK);
            xfsDevice.ResetFailed -= new IXFSCashInServiceEvents_ResetFailedEventHandler(xfsDevice_ForceRollBackFailed);
            xfsDevice.MediaDetected -= new IXFSCashInServiceEvents_MediaDetectedEventHandler(xfsDevice_ForceRollBackMediaDetected);

            if (resetMovedSuccessfully)
            {
                if (CashInRollbackPresented != null)
                {
                    CashInRollbackPresented();
                }

                StartForceRollbackTimer(45000);
                Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_MEDIUMFLASH, SiuDev);

                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackOK, Exit", Logger.Logger.LogTypes.Development);
                #endregion
            }
            else
            {
                iRetVal = (int)DeviceInterface.CashInResetResults.NoNotesFound;

                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - xfsDevice_ForceRollBackOK, Exit", Logger.Logger.LogTypes.Development);
                #endregion

                CIMEvent.Set();
            }
        }

        protected virtual void StartForceRollbackTimer(long dueTime)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name, dueTime);
            Logger.Logger.LogWrite("DesCashIn - StartForceRollbackTimer, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            ForceRollBackTimer.Interval = dueTime;
            ForceRollBackTimer.Elapsed += new System.Timers.ElapsedEventHandler(ForceRollbackTimedOut);
            ForceRollBackTimer.Start();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StartForceRollbackTimer, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        protected virtual void StopForceRollbackTimer()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StopForceRollbackTimer, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            ForceRollBackTimer.Elapsed -= new System.Timers.ElapsedEventHandler(ForceRollbackTimedOut);
            ForceRollBackTimer.Stop();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - StopForceRollbackTimer, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        public void ForceRollbackTimedOut(object sender, System.Timers.ElapsedEventArgs e)
        {
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ForceRollbackTimedOut, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            #region AlwaysOn PD
            sp.AlwaysOn(MethodBase.GetCurrentMethod().Name, "", Des.Pascal.PDCollection.AlwaysOnCategory.ConsumerAction);
            #endregion

            Light.LightSet(SIUSetGuidance.SIU_SET_GUIDELIGHT_MEDIUMFLASH, SiuDev);
            StopForceRollbackTimer();
            iRetVal = (int)DeviceInterface.CashInResetResults.Timeout;


            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - ForceRollbackTimedOut, Exit", Logger.Logger.LogTypes.Development);
            #endregion
            CIMEvent.Set();
        }

        #endregion
        /***************************/

        #region GetMaxEscrowCapacity
        /// <summary>
        /// Para yatırma ünitesinin geçici kasa bölmesinin kapasitesini almak için kullanılan fonksiyondur.
        /// </summary>
        /// <returns>
        /// xfsDevice.MaxItems
        /// </returns>
        public int GetMaxEscrowCapacity()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetMaxEscrowCapacity, Entry", Logger.Logger.LogTypes.Development);
            #endregion
            return (xfsDevice.MaxItemsOnStacker);
        }
        #endregion

        #region GetSystemStatus
        /// <summary>
        /// Para yatırma ünitesinin statü bilgilerini sorgulamak için kullanılan fonksiyondur.
        /// </summary>
        /// <param name="cashInStatus">
        /// cashInStatus.BIM
        /// cashInStatus.Cassettes
        /// cashInStatus.Device
        /// cashInStatus.TransactionStatus
        /// cashInStatus.InputStacker
        /// cashInStatus.InputTransportStatus
        /// cashInStatus.InputTransportWay
        /// cashInStatus.InputTray
        /// cashInStatus.InputTrayShutter
        /// cashInStatus.IsCrashBoxEmpty
        /// cashInStatus.NumberOfAllCassettes
        /// cashInStatus.OutputStacker
        /// cashInStatus.OutputTransportStatus
        /// cashInStatus.OutputTransportWay
        /// cashInStatus.OutputTray
        /// cashInStatus.OutputTrayShutter
        /// cashInStatus.RetractAct
        /// cashInStatus.RetractMax
        /// cashInStatus.SafeDoor
        /// cashInStatus.StackerItems
        /// </param>
        /// <returns>
        /// DeviceInterface.CashInGeneralResults.Error
        /// DeviceInterface.CashInGeneralResults.OK
        /// </returns>
        public DeviceInterface.CashInGeneralResults GetSystemStatus(DeviceInterface.CashInSystemStatus cashInStatus, ref string errorData)
        {
            try
            {
                #region Problem Determination
                sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus, Entry", Logger.Logger.LogTypes.Development);
                #endregion

                errorData = "";

                cashInStatus.RetractAct = 0;
                cashInStatus.RetractMax = 0;

                XFSStatus cashInDeviceStatus = xfsDevice.DeviceStatusObject.DeviceStatus;
                cashInStatus.Device = (int)cashInDeviceStatus;
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:DeviceStatus,{0}", Logger.Logger.LogTypes.Development, cashInDeviceStatus.ToString());

                CIMAcceptorStatus cashInAcceptorStatus = xfsDevice.DeviceStatusObject.AcceptorStatus;
                cashInStatus.Cassettes = (int)cashInAcceptorStatus;
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:AcceptorStatus,{0}", Logger.Logger.LogTypes.Development, cashInAcceptorStatus.ToString());

                CIMTransactionStatus cashInTransactionStatus = xfsDevice.LastCashInStatus.TransactionStatus;
                cashInStatus.TransactionStatus = Convert2MofType(cashInTransactionStatus);
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:TransactionStatus,{0}", Logger.Logger.LogTypes.Development, cashInTransactionStatus.ToString());

                cashInStatus.NumberOfAllCassettes = (int)xfsDevice.NumberOfCashUnits;
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:CashUnits.Count,{0}", Logger.Logger.LogTypes.Development, cashInStatus.NumberOfAllCassettes.ToString());
                XFSCashInCassette cashUnitCassetteType;
                XFSDictionary dict;
                dict = xfsDevice.CashUnits;
                object keys = dict.Keys();
                System.Array keyArray = (System.Array)keys;
                foreach (object key in keyArray)
                {
                    // Get an item from the dictionary
                    object refKey = key;
                    cashUnitCassetteType = (XFSCashInCassette)dict.get_Item(ref refKey);
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:CashUnits.UnitID :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.UnitID.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:CashUnits.UnitType :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.UnitType.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:CashUnits.CassetteStatus :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.CassetteStatus.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:CashUnits.UnitCurrentCount :({0})", Logger.Logger.LogTypes.Development, cashUnitCassetteType.UnitCurrentCount.ToString());
                    if (cashUnitCassetteType.UnitType == CIMCassetteType.CIM_TYPE_RETRACT_CASSETTE)
                    {
                        cashInStatus.RetractAct = cashInStatus.RetractAct + cashUnitCassetteType.UnitCurrentCount;
                    }
                }

                CIMStackerItemsStatus cashInStackerItemsStatus = xfsDevice.DeviceStatusObject.StackerItemStatus;
                cashInStatus.StackerItems = (int)cashInStackerItemsStatus;
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:StackerItemStatus,{0}", Logger.Logger.LogTypes.Development, cashInStackerItemsStatus.ToString());

                CIMStackerStatus cashInStackerStatus = xfsDevice.DeviceStatusObject.StackerStatus;
                switch (cashInStackerStatus)
                {
                    case CIMStackerStatus.CIM_STACKER_EMPTY:
                        cashInStatus.InputStacker = 0;
                        break;
                    case CIMStackerStatus.CIM_STACKER_FULL:
                        cashInStatus.InputStacker = 1;
                        break;
                    case CIMStackerStatus.CIM_STACKER_NOT_EMPTY:
                        cashInStatus.InputStacker = 1;
                        break;
                    case CIMStackerStatus.CIM_STACKER_NOT_SUPPORTED:
                        cashInStatus.InputStacker = 3;
                        break;
                    case CIMStackerStatus.CIM_STACKER_UNKNOWN:
                        cashInStatus.InputStacker = 2;
                        break;
                    default:
                        cashInStatus.InputStacker = 2;
                        break;
                }
                cashInStatus.OutputStacker = cashInStatus.InputStacker;
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:StackerStatus,{0}", Logger.Logger.LogTypes.Development, cashInStackerStatus.ToString());

                CIMNoteReaderStatus cashInNoteReaderStatus = xfsDevice.DeviceStatusObject.NoteReaderStatus;
                cashInStatus.BIM = (int)cashInNoteReaderStatus;
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:NoteReaderStatus,{0}", Logger.Logger.LogTypes.Development, cashInNoteReaderStatus.ToString());

                CIMSafeDoorStatus cashInSafeDoorStatus = xfsDevice.DeviceStatusObject.SafeDoorStatus;
                cashInStatus.SafeDoor = (int)cashInSafeDoorStatus;
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:SafeDoorStatus,{0}", Logger.Logger.LogTypes.Development, cashInSafeDoorStatus.ToString());

                bool cashinDropBoxStatus = xfsDevice.DeviceStatusObject.DropBoxStatus;
                if (cashinDropBoxStatus)
                    cashInStatus.IsCrashBoxEmpty = 1;
                else
                    cashInStatus.IsCrashBoxEmpty = 0;
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:DropBoxStatus,{0}", Logger.Logger.LogTypes.Development, cashinDropBoxStatus.ToString());


                CashInStatusData = "";

                if (!(CIMDeviceStatus == XFSStatus.XFS_STATUS_GOOD || CIMDeviceStatus == XFSStatus.XFS_STATUS_ERROR ||
                    (RecyclingPresent == true && CIMDeviceStatus == XFSStatus.XFS_STATUS_DEVICE_BUSY)))
                {
                    CashInStatusData = "<CashInStatusData>";
                    Logger.Logger.LogJournal("DeviceStatus              : " + cashInDeviceStatus.ToString());
                    CashInStatusData = CashInStatusData + "<DeviceStatus>" + cashInDeviceStatus.ToString() + "</DeviceStatus>";
                    Logger.Logger.LogJournal("AcceptorStatus            : " + cashInAcceptorStatus.ToString());
                    CashInStatusData = CashInStatusData + "<AcceptorStatus>" + cashInAcceptorStatus.ToString() + "</AcceptorStatus>";
                    Logger.Logger.LogJournal("TransactionStatus         : " + cashInTransactionStatus.ToString());
                    CashInStatusData = CashInStatusData + "<TransactionStatus>" + cashInTransactionStatus.ToString() + "</TransactionStatus>";
                    CashInCassetteData = "<CashInCassetteData>";
                    CashInCassetteData = CashInCassetteData + "<CassetteNumber>" + cashInStatus.NumberOfAllCassettes.ToString() + "</CassetteNumber>";
                    int cassIndex = 1;
                    foreach (object key in keyArray)
                    {
                        object refKey = key;
                        cashUnitCassetteType = (XFSCashInCassette)dict.get_Item(ref refKey);
                        Logger.Logger.LogJournal("UnitID                    : " + cashUnitCassetteType.UnitID.ToString());
                        Logger.Logger.LogJournal("UnitType                  : " + cashUnitCassetteType.UnitType.ToString());
                        Logger.Logger.LogJournal("CassetteStatus            : " + cashUnitCassetteType.CassetteStatus.ToString());
                        CashInCassetteData = CashInCassetteData + "<Cassette" + cassIndex.ToString() + ">";
                        CashInCassetteData = CashInCassetteData + "<UnitID>" + cashUnitCassetteType.UnitID.ToString() + "</UnitID>";
                        CashInCassetteData = CashInCassetteData + "<UnitType>" + cashUnitCassetteType.UnitType.ToString() + "</UnitType>";
                        CashInCassetteData = CashInCassetteData + "<CassetteStatus>" + cashUnitCassetteType.CassetteStatus.ToString() + "</CassetteStatus>";
                        CashInCassetteData = CashInCassetteData + "</Cassette" + cassIndex.ToString() + ">";
                        cassIndex = cassIndex + 1;
                    }
                    CashInCassetteData = CashInCassetteData + "</CashInCassetteData>";
                    CashInStatusData = CashInStatusData + "<StackerItemStatus>" + cashInStackerItemsStatus.ToString() + "</StackerItemStatus>";
                    Logger.Logger.LogJournal("StackerItemsStatus        : " + cashInStackerItemsStatus.ToString());
                    CashInStatusData = CashInStatusData + "<StackerStatus>" + cashInStackerStatus.ToString() + "</StackerStatus>";
                    Logger.Logger.LogJournal("StackerStatus             : " + cashInStackerStatus.ToString());
                    CashInStatusData = CashInStatusData + "<NoteReaderStatus>" + cashInNoteReaderStatus.ToString() + "</NoteReaderStatus>";
                    Logger.Logger.LogJournal("NoteReaderStatus          : " + cashInNoteReaderStatus.ToString());
                    CashInStatusData = CashInStatusData + "<SafeDoorStatus>" + cashInSafeDoorStatus.ToString() + "</SafeDoorStatus>";
                    Logger.Logger.LogJournal("SafeDoorStatus            : " + cashInSafeDoorStatus.ToString());
                    CashInStatusData = CashInStatusData + "<DropBoxStatus>" + cashinDropBoxStatus.ToString() + "</DropBoxStatus>";
                    Logger.Logger.LogJournal("DropBoxStatus             : " + cashinDropBoxStatus.ToString());
                }

                XFSDictionary inputDict = xfsDevice.DeviceStatusObject.InputPositionStatus;// deviceStatusObject.InputPositionStatus;
                XFSDictionary outputDict = xfsDevice.DeviceStatusObject.OutputPositionStatus;// deviceStatusObject.OutputPositionStatus;
                object[] inputItems = (object[])inputDict.Items();
                object[] outputItems = (object[])outputDict.Items();
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:InputItems", Logger.Logger.LogTypes.Development);
                for (int i = 0; i < inputItems.Length; i++)
                {
                    IXFSPositionStatus posStatus = (IXFSPositionStatus)inputItems[i];
                    cashInStatus.InputTray = Convert2MofType(posStatus.PositionStatus);
                    cashInStatus.InputTrayShutter = Convert2MofType(posStatus.ShutterStatus);
                    cashInStatus.InputTransportWay = Convert2MofType(posStatus.TransportItemStatus);
                    cashInStatus.InputTransportStatus = (int)posStatus.TransportStatus;
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:InputPosition,{0}", Logger.Logger.LogTypes.Development, posStatus.Position.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:InputPositionStatus,{0}", Logger.Logger.LogTypes.Development, posStatus.PositionStatus.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:InputShutterStatus,{0}", Logger.Logger.LogTypes.Development, posStatus.ShutterStatus.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:InputTransportStatus,{0}", Logger.Logger.LogTypes.Development, posStatus.TransportStatus.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:InputTransportItemStatus,{0}", Logger.Logger.LogTypes.Development, posStatus.TransportItemStatus.ToString());
                    if (!(CIMDeviceStatus == XFSStatus.XFS_STATUS_GOOD || CIMDeviceStatus == XFSStatus.XFS_STATUS_ERROR ||
                        (RecyclingPresent == true && CIMDeviceStatus == XFSStatus.XFS_STATUS_DEVICE_BUSY)))
                    {
                        CashInStatusData = CashInStatusData + "<InputPosition>" + posStatus.Position.ToString() + "</InputPosition>";
                        CashInStatusData = CashInStatusData + "<InputPositionStatus>" + posStatus.PositionStatus.ToString() + "</InputPositionStatus>";
                        CashInStatusData = CashInStatusData + "<InputShutterStatus>" + posStatus.ShutterStatus.ToString() + "</InputShutterStatus>";
                        CashInStatusData = CashInStatusData + "<InputTransportStatus>" + posStatus.TransportStatus.ToString() + "</InputTransportStatus>";
                        CashInStatusData = CashInStatusData + "<InputTransportItemStatus>" + posStatus.TransportItemStatus.ToString() + "</InputTransportItemStatus>";
                        Logger.Logger.LogJournal("InputPosition             : " + posStatus.Position.ToString());
                        Logger.Logger.LogJournal("InputPositionStatus       : " + posStatus.PositionStatus.ToString());
                        Logger.Logger.LogJournal("InputShutterStatus        : " + posStatus.ShutterStatus.ToString());
                        Logger.Logger.LogJournal("InputTransportStatus      : " + posStatus.TransportStatus.ToString());
                        Logger.Logger.LogJournal("InputTransportItemStatus  : " + posStatus.TransportItemStatus.ToString());
                    }
                }

                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:OutputItems", Logger.Logger.LogTypes.Development);
                for (int i = 0; i < outputItems.Length; i++)
                {
                    IXFSPositionStatus posStatus = (IXFSPositionStatus)outputItems[i];
                    cashInStatus.OutputTray = Convert2MofType(posStatus.PositionStatus);
                    cashInStatus.OutputTrayShutter = Convert2MofType(posStatus.ShutterStatus);
                    cashInStatus.OutputTransportWay = Convert2MofType(posStatus.TransportItemStatus);
                    cashInStatus.OutputTransportStatus = (int)posStatus.TransportStatus;
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:OutputPosition,{0}", Logger.Logger.LogTypes.Development, posStatus.Position.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:OutputPositionStatus,{0}", Logger.Logger.LogTypes.Development, posStatus.PositionStatus.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:OutputShutterStatus,{0}", Logger.Logger.LogTypes.Development, posStatus.ShutterStatus.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:OutputTransportStatus,{0}", Logger.Logger.LogTypes.Development, posStatus.TransportStatus.ToString());
                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus:OutputTransportItemStatus,{0}", Logger.Logger.LogTypes.Development, posStatus.TransportItemStatus.ToString());
                    if (!(CIMDeviceStatus == XFSStatus.XFS_STATUS_GOOD || CIMDeviceStatus == XFSStatus.XFS_STATUS_ERROR ||
                        (RecyclingPresent == true && CIMDeviceStatus == XFSStatus.XFS_STATUS_DEVICE_BUSY)))
                    {
                        CashInStatusData = CashInStatusData + "<OutputPosition>" + posStatus.Position.ToString() + "</OutputPosition>";
                        CashInStatusData = CashInStatusData + "<OutputPositionStatus>" + posStatus.PositionStatus.ToString() + "</OutputPositionStatus>";
                        CashInStatusData = CashInStatusData + "<OutputShutterStatus>" + posStatus.ShutterStatus.ToString() + "</OutputShutterStatus>";
                        CashInStatusData = CashInStatusData + "<OutputTransportStatus>" + posStatus.TransportStatus.ToString() + "</OutputTransportStatus>";
                        CashInStatusData = CashInStatusData + "<OutputTransportItemStatus>" + posStatus.TransportItemStatus.ToString() + "</OutputTransportItemStatus>";
                        Logger.Logger.LogJournal("OutputPosition            : " + posStatus.Position.ToString());
                        Logger.Logger.LogJournal("OutputPositionStatus      : " + posStatus.PositionStatus.ToString());
                        Logger.Logger.LogJournal("OutputShutterStatus       : " + posStatus.ShutterStatus.ToString());
                        Logger.Logger.LogJournal("OutputTransportStatus     : " + posStatus.TransportStatus.ToString());
                        Logger.Logger.LogJournal("OutputTransportItemStatus : " + posStatus.TransportItemStatus.ToString());
                    }
                }

                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus, Exit", Logger.Logger.LogTypes.Development);
                #endregion

                if (CashInStatusData != "")
                {
                    CashInStatusData = CashInStatusData + "<RetractAct>0</RetractAct>";
                    CashInStatusData = CashInStatusData + "<RetractMax>0</RetractMax>";
                    CashInStatusData = CashInStatusData + "</CashInStatusData>";
                    CashInXFSErrorData = "<CashInXFSErrorData>";
                    CashInXFSErrorData = CashInXFSErrorData + "<XFSErrorDataDevice>" + XFSErrorDataDevice + "</XFSErrorDataDevice>";
                    CashInXFSErrorData = CashInXFSErrorData + "<XFSErrorDataError>" + XFSErrorDataError + "</XFSErrorDataError>";
                    CashInXFSErrorData = CashInXFSErrorData + "<XFSErrorDataErrorCategory>" + XFSErrorDataErrorCategory + "</XFSErrorDataErrorCategory>";
                    CashInXFSErrorData = CashInXFSErrorData + "<XFSErrorDataSource>" + XFSErrorDataSource + "</XFSErrorDataSource>";
                    CashInXFSErrorData = CashInXFSErrorData + "<XFSErrorDataWOSACommand>" + XFSErrorDataWOSACommand + "</XFSErrorDataWOSACommand>";
                    CashInXFSErrorData = CashInXFSErrorData + "<XFSErrorDataWOSAError>" + XFSErrorDataWOSAError + "</XFSErrorDataWOSAError>";
                    CashInXFSErrorData = CashInXFSErrorData + "<XFSErrorDataWOSADescription>" + XFSErrorDataWOSADescription + "</XFSErrorDataWOSADescription>";
                    CashInXFSErrorData = CashInXFSErrorData + "<XFSErrorDataResult>" + XFSErrorDataResult + "</XFSErrorDataResult>";
                    CashInXFSErrorData = CashInXFSErrorData + "</CashInXFSErrorData>";

                    Logger.Logger.LogWrite("DesCashIn - GetSystemStatus, ErrorXML : " + CashInStatusData + CashInCassetteData + CashInXFSErrorData, Logger.Logger.LogTypes.Development);
                }
                errorData = CashInStatusData + CashInCassetteData + CashInXFSErrorData;
                return DeviceInterface.CashInGeneralResults.OK;
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - GetSystemStatus, Exception:{0}", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                errorData = "";
                return DeviceInterface.CashInGeneralResults.Error;
            }


        }
        #endregion

        #region GetCashUnitInfo
        /// <summary>
        /// Para yatırma ünitesindeki count bilgilerini almak için kullanılan fonksiyondur.
        /// </summary>
        /// <param name="cashInCashUnitInfo">
        /// 
        /// cashInCashUnitInfo.CashUnitInfo[].AppLock
        /// cashInCashUnitInfo.CashUnitInfo[].CassetteID
        /// cashInCashUnitInfo.CashUnitInfo[].Count
        /// cashInCashUnitInfo.CashUnitInfo[].CurrencyID
        /// cashInCashUnitInfo.CashUnitInfo[].Denomination
        /// cashInCashUnitInfo.CashUnitInfo[].DevLock
        /// cashInCashUnitInfo.CashUnitInfo[].InitialCount
        /// cashInCashUnitInfo.CashUnitInfo[].LogicalNumber
        /// cashInCashUnitInfo.CashUnitInfo[].Maximum
        /// cashInCashUnitInfo.CashUnitInfo[].Minimum
        /// cashInCashUnitInfo.CashUnitInfo[].NumberOfNoteTypes
        /// cashInCashUnitInfo.CashUnitInfo[].Status
        /// cashInCashUnitInfo.CashUnitInfo[].Type
        /// cashInCashUnitInfo.CashUnitInfo[].Unfit
        /// cashInCashUnitInfo.NumberOfCassettes
        /// </param>
        /// <returns>
        /// DeviceInterface.CashInGeneralResults.Error
        /// DeviceInterface.CashInGeneralResults.OK
        /// </returns>
        public DeviceInterface.CashInGeneralResults GetCashUnitInfo(DeviceInterface.CashInCashUnitInfo cashInCashUnitInfo)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetCashUnitInfo, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            XFSCashInCassette cashUnitCassetteType;
            IXFSC302ItemType cassetteItemType;
            CIMCassetteStatus cassetteStatus;
            CIMCassetteRepStatus cassetteReplenishmentStatus;
            CIMCassetteType unitType;

            int cassetteCounter = 0;
            String unitID;

            XFSDictionary dict;
            dict = xfsDevice.CashUnits;
            cashInCashUnitInfo.NumberOfCassettes = xfsDevice.CashUnits.Count;

            Logger.Logger.LogWrite("DesCashIn - GetCashUnitInfo, cashInCashUnitInfo.NumberOfCassettes=({0})", Logger.Logger.LogTypes.Development, cashInCashUnitInfo.NumberOfCassettes.ToString());

            object keys = dict.Keys();
            System.Array keyArray = (System.Array)keys;
            int count = 0;
            int tmpCount = 0;
            foreach (object key in keyArray)
            {
                if (IsAtmDiebold)
                    tmpCount = count;
                else
                    tmpCount = (short)key - 1;

                Logger.Logger.LogWrite("DesCashIn - GetCashUnitInfo, keyArray.Length=({0})", Logger.Logger.LogTypes.Development, keyArray.Length.ToString());
                // Get an item from the dictionary
                object refKey = key;
                cashUnitCassetteType = (XFSCashInCassette)dict.get_Item(ref refKey);
                cassetteCounter++;
                unitID = cashUnitCassetteType.UnitID;
                Logger.Logger.LogWrite("DesCashIn - GetCashUnitInfo, unitID=({0})", Logger.Logger.LogTypes.Development, unitID.ToString());

                cassetteStatus = cashUnitCassetteType.CassetteStatus;
                cassetteReplenishmentStatus = cashUnitCassetteType.CassetteReplenishmentStatus;
                unitType = cashUnitCassetteType.UnitType;
                cassetteItemType = (IXFSC302ItemType)cashUnitCassetteType.UnitItemType;
                Logger.Logger.LogWrite("DesCashIn - GetCashUnitInfo, unitType=({0})", Logger.Logger.LogTypes.Development, unitType.ToString());

                #region Problem Determination
                sp.GeneralDebug(MethodBase.GetCurrentMethod().Name, "Cassette :" + cassetteCounter,
                    unitID, cassetteStatus, cassetteReplenishmentStatus, unitType, cassetteItemType);
                #endregion
                switch (xfsDevice.Locked)
                {
                    case XFSLocked.XFS_LOCK_PENDING:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].AppLock = 2;
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].DevLock = 2;
                        break;
                    case XFSLocked.XFS_LOCKED:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].AppLock = 1;
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].DevLock = 1;
                        break;
                    case XFSLocked.XFS_NOT_LOCKED:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].AppLock = 0;
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].DevLock = 0;
                        break;
                    default:
                        break;
                }

                Logger.Logger.LogWrite("DesCashIn - GetCashUnitInfo, cassetteStatus=({0})", Logger.Logger.LogTypes.Development, cassetteStatus.ToString());

                cashInCashUnitInfo.CashUnitInfo[tmpCount].CassetteID = cashUnitCassetteType.UnitID;
                cashInCashUnitInfo.CashUnitInfo[tmpCount].Count = cashUnitCassetteType.UnitCurrentCount;
                cashInCashUnitInfo.CashUnitInfo[tmpCount].CurrencyID = cashUnitCassetteType.CashUnitCurrency;
                cashInCashUnitInfo.CashUnitInfo[tmpCount].Denomination = cashUnitCassetteType.CashUnitValue;
                cashInCashUnitInfo.CashUnitInfo[tmpCount].InitialCount = 0;
                cashInCashUnitInfo.CashUnitInfo[tmpCount].LogicalNumber = 0;
                cashInCashUnitInfo.CashUnitInfo[tmpCount].Maximum = cashUnitCassetteType.MaximumItems;
                cashInCashUnitInfo.CashUnitInfo[tmpCount].Minimum = 0;
                cashInCashUnitInfo.CashUnitInfo[tmpCount].NumberOfNoteTypes = cashUnitCassetteType.NoteTypesCount.Count;
                switch (cashUnitCassetteType.CassetteReplenishmentStatus)
                {
                    case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_EMPTY:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Status = 4; break;
                    case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_FULL:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Status = 1; break;
                    case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_HIGH:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Status = 2; break;
                    case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_INOPERATIVE:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Status = 6; break;
                    case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_NO_VALUE:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Status = 7; break;
                    case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_NOT_PRESENT:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Status = 5; break;
                    case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_OK:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Status = 0; break;
                }
                switch (cashUnitCassetteType.UnitType)
                {
                    case CIMCassetteType.CIM_CASH_IN:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Type = 10; break;
                    case CIMCassetteType.CIM_RECYCLING:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Type = 10; break;
                    case CIMCassetteType.CIM_REPLENISHMENT_CONTAINER:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Type = 3; break;
                    case CIMCassetteType.CIM_TYPE_RETRACT_CASSETTE:
                        cashInCashUnitInfo.CashUnitInfo[tmpCount].Type = 6; break;
                }
                cashInCashUnitInfo.CashUnitInfo[tmpCount].Unfit = cashUnitCassetteType.UnitInCount;
                count++;
                Logger.Logger.LogWrite("**********************************************=({0})", Logger.Logger.LogTypes.Development, cassetteStatus.ToString());
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetCashUnitInfo, Exit", Logger.Logger.LogTypes.Development);
            #endregion
            return DeviceInterface.CashInGeneralResults.OK;
        }
        #endregion

        #region GetExtraSystemStatus
        /// <summary>
        /// Para yatırma ünitesinde meydana gelen XFSError hatalarının loglanması amacı ile kullanılan fonksiyondur.
        /// </summary>
        /// <param name="infoKey"></param>
        /// <param name="info"></param>
        /// <returns>
        /// DeviceInterface.CashInGeneralResults.Error
        /// DeviceInterface.CashInGeneralResults.OK
        /// </returns>
        public DeviceInterface.CashInGeneralResults GetExtraSystemStatus(string infoKey, ref string info)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetExtraSystemStatus, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            StreamReader sReader = null;
            string contents = null;
            string fileName = @"C:\WEBATMLOGS\XFSErrorLogs\CIMXFSErrorLog.txt";

            if (System.IO.File.Exists(fileName))
            {
                try
                {
                    FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    sReader = new StreamReader(fileStream);
                    contents = sReader.ReadToEnd();
                    info = contents;
                }
                finally
                {
                    if (sReader != null) sReader.Close();
                }

                // Delete file
                File.Delete(fileName);

                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - GetExtraSystemStatus, Exit", Logger.Logger.LogTypes.Development);
                #endregion

                return DeviceInterface.CashInGeneralResults.OK;
            }
            else
            {
                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - GetExtraSystemStatus, Exit", Logger.Logger.LogTypes.Development);
                #endregion

                return DeviceInterface.CashInGeneralResults.Error;
            }

        }

        void Cashin_ExceptionEvent(int requestID, XFSErrorStatus error, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name, error.ToString(), workstationID, timeStamp);
            Logger.Logger.LogWrite("DesCashin - Cashin_ExceptionEvent, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            Logger.Logger.LogWrite("DesCashin - Cashin_ExceptionEvent, XFSErrorStatus : " + error.ToString(), Logger.Logger.LogTypes.Development);
            Logger.Logger.LogWrite("DesCashin - Cashin_ExceptionEvent, DeviceStatus : " + xfsDevice.DeviceStatus.ToString(), Logger.Logger.LogTypes.Development);

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCard - Cashin_ExceptionEvent, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }

        private string FindXFSReasonData(string stringToSearch, string searchPattern1, string searchPattern2)
        {
            try
            {
                string XFSReasonData = "";
                int index1, index2;
                if (stringToSearch.Contains(searchPattern1) && stringToSearch.Contains(searchPattern2))
                {
                    index1 = stringToSearch.IndexOf(searchPattern1);
                    index2 = stringToSearch.IndexOf(searchPattern2);
                    XFSReasonData = stringToSearch.Substring(index1 + searchPattern1.Length, index2 - index1 - searchPattern1.Length).Trim();
                }
                return XFSReasonData;
            }
            catch (System.Exception ex)
            {
                Logger.Logger.LogWrite("DesCashIn - FindXFSReasonData, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                return "";
            }
        }

        private string SplitXFSReasonData(string stringToSplit)
        {
            try
            {
                string XFSReasonData = "";
                string[] tmpXFSReasonData = stringToSplit.Split(' ');
                int value = 0;

                if (tmpXFSReasonData.Length == 1)
                    return "";
                for (int i = 1; i < tmpXFSReasonData.Length; i++)
                {
                    value = Convert.ToInt32(tmpXFSReasonData[i], 16);
                    XFSReasonData = XFSReasonData + Char.ConvertFromUtf32(value).PadLeft(2, '0');
                }
                return XFSReasonData;
            }
            catch (System.Exception ex)
            {
                Logger.Logger.LogWrite("DesCashIn - FindXFSReasonData, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                return "";
            }
        }

        void XFSErrorEventLog(IXFSError error, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name, error.Result, workstationID, timeStamp);
            Logger.Logger.LogWrite("DesCashIn - XFSErrorEventLog, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            string tmpResult = error.Result;
            string fileName = @"C:\WEBATMLOGS\XFSErrorLogs\CIMXFSErrorLog.txt";
            string sLogInfo = "CIM," + DateTime.Now.ToString() + "," + tmpResult;
            StreamWriter sWriter = null;
            try
            {
                if (sLogInfo.IndexOf("SLen") > 0)
                {
                    FileStream fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                    sWriter = new StreamWriter(fileStream);
                    sWriter.Write(sLogInfo);

                    XFSErrorDataResult = tmpResult;
                    XFSErrorDataDevice = error.Device;
                    XFSErrorDataError = error.XFSError.ToString();
                    XFSErrorDataErrorCategory = error.ErrorCategory.ToString();
                    XFSErrorDataSource = error.Source;
                    XFSErrorDataWOSACommand = error.WOSACommand.ToString();
                    XFSErrorDataWOSAError = error.WOSAError.ToString();
                    XFSErrorDataWOSADescription = ConvertHexString(error.WOSADescription);
                    //XFSErrorDataResult = XFSErrorDataResult + "<SrvcAddr>" + FindXFSReasonData(tmpResult, "SrvcAddr", "Class") + "</SrvcAddr>";
                    //XFSErrorDataResult = XFSErrorDataResult + "<Class>" + FindXFSReasonData(tmpResult, "Class", "TCode") + "</Class>";
                    //XFSErrorDataResult = XFSErrorDataResult + "<TCode>" + FindXFSReasonData(tmpResult, "TCode", "TLen") + "</TCode>";
                    //XFSErrorDataResult = XFSErrorDataResult + "<TData>" + FindXFSReasonData(tmpResult, "TLen", "SLen") + "</TData>";
                    //XFSErrorDataResult = XFSErrorDataResult + "<SData>" + FindXFSReasonData(tmpResult, "SLen", "MCode")+ "</SData>";
                    //XFSErrorDataResult = XFSErrorDataResult + "<MCode>" + FindXFSReasonData(tmpResult, "MCode", "MStatus") + "</MCode>";
                    //XFSErrorDataResult = XFSErrorDataResult + "<MStatus>" + FindXFSReasonData(tmpResult, "MStatus", "MLen") + "</MStatus>";
                    //XFSErrorDataResult = XFSErrorDataResult + "<MData>" + FindXFSReasonData(tmpResult, "MLen", "RSLen") + "</MData>";
                    //XFSErrorDataResult = XFSErrorDataResult + "<RSData>" + SplitXFSReasonData(FindXFSReasonData(tmpResult, "RSLen", "RCLen")) + "</RSData>";
                    //XFSErrorDataResult = XFSErrorDataResult + "<RCData>" + SplitXFSReasonData(FindXFSReasonData(tmpResult, "RCLen", "*")) + "</RCData>";

                    Logger.Logger.LogWrite("DesCashIn - XFSError [{0}]", Logger.Logger.LogTypes.Exceptions, sLogInfo);
                }
                else if (XFSErrorDataResult == "")
                {
                    XFSErrorDataDevice = error.Device;
                    XFSErrorDataError = error.XFSError.ToString();
                    XFSErrorDataErrorCategory = error.ErrorCategory.ToString();
                    XFSErrorDataSource = error.Source;
                    XFSErrorDataWOSACommand = error.WOSACommand.ToString();
                    XFSErrorDataWOSAError = error.WOSAError.ToString();
                    XFSErrorDataWOSADescription = ConvertHexString(error.WOSADescription);
                }
            }
            finally
            {
                if (sWriter != null) sWriter.Close();
            }

            Logger.Logger.LogWrite("DesCashIn - XFSError, requestID : [{0}]", Logger.Logger.LogTypes.Development, error.requestID.ToString());
            Logger.Logger.LogWrite("DesCashIn - XFSError, Device : [{0}]", Logger.Logger.LogTypes.Development, XFSErrorDataDevice);
            Logger.Logger.LogWrite("DesCashIn - XFSError, XFSError : [{0}]", Logger.Logger.LogTypes.Development, error.XFSError.ToString());
            Logger.Logger.LogWrite("DesCashIn - XFSError, ErrorCategory : [{0}]", Logger.Logger.LogTypes.Development, XFSErrorDataErrorCategory);
            Logger.Logger.LogWrite("DesCashIn - XFSError, Source : [{0}]", Logger.Logger.LogTypes.Development, XFSErrorDataSource);
            Logger.Logger.LogWrite("DesCashIn - XFSError, WOSACommand : [{0}]", Logger.Logger.LogTypes.Development, XFSErrorDataWOSACommand);
            Logger.Logger.LogWrite("DesCashIn - XFSError, WOSAError : [{0}]", Logger.Logger.LogTypes.Development, XFSErrorDataWOSAError);
            Logger.Logger.LogWrite("DesCashIn - XFSError, WOSADescription : [{0}]", Logger.Logger.LogTypes.Development, ConvertHexString(error.WOSADescription));

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - XFSErrorEventLog, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        private string ConvertHexString(byte[] arr)
        {
            try
            {
                StringBuilder strHex = new StringBuilder();
                string tmp = "";
                for (int i = 0; i < arr.Length; i++)
                {
                    tmp = String.Format("{0:X}", arr[i]);
                    strHex.Append(tmp.PadLeft(2, '0'));
                }
                return strHex.ToString();
            }
            catch
            {
                return "";
            }
        }

        void XFSSystemErrorEventLog(XFSSystemErrorType errorType, string logicalName, string physicalName, string WorkStationName, string appID, int action, ref byte[] vendorErrorDescription)
        {
            Logger.Logger.LogWrite("DesCashIn - XFSSystemErrorEventLog, Entry", Logger.Logger.LogTypes.Development);

            Logger.Logger.LogWrite("DesCashIn - XFSSystemErrorEventLog, errorType : [{0}]", Logger.Logger.LogTypes.Development, errorType.ToString());
            Logger.Logger.LogWrite("DesCashIn - XFSSystemErrorEventLog, logicalName : [{0}]", Logger.Logger.LogTypes.Development, logicalName);
            Logger.Logger.LogWrite("DesCashIn - XFSSystemErrorEventLog, physicalName : [{0}]", Logger.Logger.LogTypes.Development, physicalName);
            Logger.Logger.LogWrite("DesCashIn - XFSSystemErrorEventLog, WorkStationName : [{0}]", Logger.Logger.LogTypes.Development, WorkStationName);
            Logger.Logger.LogWrite("DesCashIn - XFSSystemErrorEventLog, appID : [{0}]", Logger.Logger.LogTypes.Development, appID);
            Logger.Logger.LogWrite("DesCashIn - XFSSystemErrorEventLog, action : [{0}]", Logger.Logger.LogTypes.Development, action.ToString());
            if (action == WFS_ERR_ACT_RESET)
            {
                isResetActionPresent = false;
            }
            Logger.Logger.LogWrite("DesCashIn - XFSSystemErrorEventLog, vendorErrorDescription : [{0}]", Logger.Logger.LogTypes.Development, ConvertHexString(vendorErrorDescription));
            Logger.Logger.LogWrite("DesCashIn - XFSSystemErrorEventLog, Exit", Logger.Logger.LogTypes.Development);
            Logger.Logger.LogJournal("XFS SYSTEM ERROR. TYPE = " + errorType.ToString());
            Logger.Logger.LogJournal("XFS SYSTEM ERROR. ACTION = " + action.ToString());
        }
        #endregion

        #region CIM-Specific

        private static int Convert2MofType(CIMShutterState status)
        {
            switch (status)
            {
                case CIMShutterState.CIM_SHUTTER_STATE_CLOSED:
                    return 0;
                case CIMShutterState.CIM_SHUTTER_STATE_OPEN:
                    return 1;
                case CIMShutterState.CIM_SHUTTER_STATE_JAMMED:
                    return 3;
                case CIMShutterState.CIM_SHUTTER_STATE_UNKNOWN:
                    return 2;
                case CIMShutterState.CIM_SHUTTER_STATE_NOT_SUPPORTED:
                    return 4;
                default:
                    return 2;
            }
        }

        private int Convert2MofType(CIMPosition status, CIMPositionType positionType)
        {
            if (positionType == CIMPositionType.Input)
            {
                switch (status)
                {
                    case CIMPosition.CIM_POSITION_LEFT:
                        return 1;
                    case CIMPosition.CIM_POSITION_RIGHT:
                        return 2;
                    case CIMPosition.CIM_POSITION_CENTER:
                        return 4;
                    case CIMPosition.CIM_POSITION_TOP:
                        return 8;
                    case CIMPosition.CIM_POSITION_BOTTOM:
                        return 16;
                    case CIMPosition.CIM_POSITION_FRONT:
                        return 32;
                    case CIMPosition.CIM_POSITION_REAR:
                        return 64;
                }
            }
            else if (positionType == CIMPositionType.Output)
            {
                switch (status)
                {
                    case CIMPosition.CIM_POSITION_LEFT:
                        return 128;
                    case CIMPosition.CIM_POSITION_RIGHT:
                        return 256;
                    case CIMPosition.CIM_POSITION_CENTER:
                        return 512;
                    case CIMPosition.CIM_POSITION_TOP:
                        return 1024;
                    case CIMPosition.CIM_POSITION_BOTTOM:
                        return 2048;
                    case CIMPosition.CIM_POSITION_FRONT:
                        return 4096;
                    case CIMPosition.CIM_POSITION_REAR:
                        return 8192;
                }
            }
            return 0;
        }

        private static int Convert2MofType(CIMPositionState status)
        {
            switch (status)
            {
                case CIMPositionState.CIM_POSITION_STATE_EMPTY:
                    return 0;
                case CIMPositionState.CIM_POSITION_STATE_NOT_EMPTY:
                    return 1;
                case CIMPositionState.CIM_POSITION_STATE_UNKNOWN:
                    return 2;
                case CIMPositionState.CIM_POSITION_STATE_NOT_SUPPORTED:
                    return 3;
                default:
                    return 2;
            }
        }

        private static int Convert2MofType(CIMTransportState status)
        {
            switch (status)
            {

                case CIMTransportState.CIM_TRANSPORT_STATE_OK:
                    return 0;
                case CIMTransportState.CIM_TRANSPORT_STATE_INOPERATIVE:
                    return 1;
                case CIMTransportState.CIM_TRANSPORT_STATE_UNKNOWN:
                    return 2;
                case CIMTransportState.CIM_TRANSPORT_STATE_NOT_SUPPORTED:
                    return 3;
                default:
                    return 3;
            }
        }

        private static int Convert2MofType(CIMTransportItemState status)
        {
            switch (status)
            {
                case CIMTransportItemState.CIM_TRANSPORT_ITEM_STATE_EMPTY:
                    return 0;
                case CIMTransportItemState.CIM_TRANSPORT_ITEM_STATE_NOT_EMPTY:
                    return 1;
                case CIMTransportItemState.CIM_TRANSPORT_ITEM_STATE_NOT_EMPTY_CUSTOMER:
                    return 1;
                case CIMTransportItemState.CIM_TRANSPORT_ITEM_STATE_NOT_EMPTY_UNKNOWN:
                    return 2;
                case CIMTransportItemState.CIM_TRANSPORT_ITEM_STATE_NOT_SUPPORTED:
                    return 3;
                default:
                    return 2;
            }
        }

        private static int Convert2MofType(CIMCassetteStatus cIMCassetteStatus, CIMCassetteRepStatus cIMCassetteRepStatus)
        {
            switch (cIMCassetteStatus)
            {
                case CIMCassetteStatus.CIM_CASSETTE_NOT_PRESENT:
                    return 6;
                case CIMCassetteStatus.CIM_CASSETTE_NO_REFERENCE:
                    return 8;
                case CIMCassetteStatus.CIM_CASSETTE_MANIPULATION:
                    return 9;
            }

            switch (cIMCassetteRepStatus)
            {
                case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_OK:
                    {
                        switch (cIMCassetteStatus)
                        {
                            case CIMCassetteStatus.CIM_CASSETTE_AVAILABLE:
                                return 0;
                            case CIMCassetteStatus.CIM_CASSETTE_NOT_AVAILABLE:
                                return 7;
                        }
                    }
                    break;
                case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_FULL:
                    return 1;
                case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_HIGH:
                    return 2;
                case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_EMPTY:
                    return 4;
                case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_INOPERATIVE:
                    return 5;
                case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_NOT_PRESENT:
                    return 6;
                case CIMCassetteRepStatus.CIM_CASSETTE_REPLENISH_NO_VALUE:
                    return 7;
            }

            return 7;
        }


        private static int Convert2MofType(CIMTransactionStatus status)
        {
            switch (status)
            {
                case CIMTransactionStatus.CIM_TRANSACTION_OK:
                    return 0;
                case CIMTransactionStatus.CIM_TRANSACTION_ROLLBACK:
                    return 1;
                case CIMTransactionStatus.CIM_TRANSACTION_ACTIVE:
                    return 2;
                case CIMTransactionStatus.CIM_TRANSACTION_RETRACTED:
                    return 4;
                case CIMTransactionStatus.CIM_TRANSACTION_UNKNOWN:
                    return 3;
                default:
                    return 3;
            }
        }

        internal enum CIMPositionType
        {
            Input,
            Output
        }

        #endregion

        #region SetZeroToCashUnit
        /// <summary>
        /// CashIn componentinde kaset sifirlamak icin kullanilacak olan fonksiyondur.
        /// CashOut gibi GetCashUnitInfo arkasindan cagrilmasi gerekmiyor cunku CashIn'de sadece kasetleri sifirlama
        /// islemi olabilir, degerleri degistirmek diye bir islem olamaz, bu yuzden bu fonksiyon parametresiz olarak 
        /// calisacak sekilde ayarlandi ve kasetlerdeki UnitInCount ve UnitCurrentCount degerleri sifirlandi.
        /// Exchange komutu sirasinda alinabilecek hata degerleri kodun icine Comment olarak eklendi ve islem sonucunda
        /// geriye donduruluyor. Bu hata degerleri gerekiyor ise CashOut ve CashIn olarak DeviceInterface'e tanimlanabilir.
        /// </summary>
        /// <returns>
        /// DeviceInterface.CashInGeneralResults.Error
        /// DeviceInterface.CashInGeneralResults.OK
        /// </returns>
        public DeviceInterface.CashInGeneralResults SetZeroToCashUnitInfo()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - SetZeroToCashUnitInfo, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            if (RecyclingPresent && RcDeviceStatus != XFSStatus.XFS_STATUS_GOOD)
                return DeviceInterface.CashInGeneralResults.Error;

            iRetVal = -1;
            try
            {
                CIMEvent.Reset();
                th = new Thread(new ThreadStart(SetZeroToCashUnitInfoProcess));
                th.IsBackground = true;
                th.Start();
                CIMEvent.WaitOne();
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - SetZeroToCashUnitInfo, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn SetZeroToCashUnitInfo, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - SetZeroToCashUnitInfo, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            return (DeviceInterface.CashInGeneralResults)iRetVal;
        }

        private void SetZeroToCashUnitInfoProcess()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - SetZeroToCashUnitInfoProcess, Entry", Logger.Logger.LogTypes.Development);
            #endregion
            try
            {
                Logger.Logger.LogWrite("DesCashIn - SetZeroToCashUnitInfoProcess, TransactionStatus=" + xfsDevice.LastCashInStatus.TransactionStatus.ToString(), Logger.Logger.LogTypes.Development);
                //iptal tuşuyla çıkıldığında cashin_active kaldığı için, reset komutu eklendi.
                if (IsAtmDiebold && xfsDevice.LastCashInStatus.TransactionStatus == CIMTransactionStatus.CIM_TRANSACTION_ACTIVE)
                {
                    Reset();
                }

                xfsDevice.ExchangeStarted += new IXFSCashInServiceEvents_ExchangeStartedEventHandler(CIM_ExchangeStarted);
                xfsDevice.StartExchangeFailed += new IXFSCashInServiceEvents_StartExchangeFailedEventHandler(CIM_StartExchangeFailed);

                XFSDictionary StartExcDict = new XFSDictionary();
                StartExcDict = GetCashUnitLogicalIds();
                xfsDevice.StartExchange(CIMExchangeType.CIM_EXCHANGE_BY_HAND, null, StartExcDict);
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - SetZeroToCashUnitInfoProcess, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
                Logger.Logger.LogResetInfo("DesCashIn SetZeroToCashUnitInfoProcess, Exception");
                Des.Pascal.CEH.RaiseException(ex);
            }
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - SetZeroToCashUnitInfoProcess, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        /// <summary>
        /// ErrorInfo
        /// </summary>
        /// <remarks>
        /// CIM_EXCHANGE_START_CASH_UNIT_ERROR = 0,
        /// CIM_EXCHANGE_START_TOO_MANY_ITEMS = 1,
        /// CIM_EXCHANGE_START_EXCHANGE_ACTIVE = 2,
        /// CIM_EXCHANGE_START_TIME_OUT = 100,
        /// CIM_EXCHANGE_START_HARDWARE_ERROR = 101,
        /// CIM_EXCHANGE_START_DEVICE_LOCKED = 102,
        /// CIM_EXCHANGE_START_UNSUPPORTED_METHOD = 103,
        /// CIM_EXCHANGE_START_CANCELLED = 104,
        /// CIM_EXCHANGE_START_UNSUPPORTED_CATEGORY = 105,
        /// CIM_EXCHANGE_START_CONNECTION_LOST = 106,
        /// CIM_EXCHANGE_START_DEVICE_NOT_READY = 107,
        /// CIM_EXCHANGE_START_EXCEPTION_OCCURRED = 108,
        /// CIM_EXCHANGE_START_UNSUPPORTED_DATA = 109,
        /// CIM_EXCHANGE_START_INVALID_DATA = 110,
        /// CIM_EXCHANGE_START_USER_ERROR = 111,
        /// </remarks>
        void CIM_StartExchangeFailed(int requestID, CIMExchangeStartError errorInfo, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CIM_StartExchangeFailed, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            iRetVal = (int)errorInfo;

            xfsDevice.ExchangeStarted -= new IXFSCashInServiceEvents_ExchangeStartedEventHandler(CIM_ExchangeStarted);
            xfsDevice.StartExchangeFailed -= new IXFSCashInServiceEvents_StartExchangeFailedEventHandler(CIM_StartExchangeFailed);

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CIM_StartExchangeFailed, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        void CIM_ExchangeStarted(int requestID, XFSDictionary cashInfo, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CIM_ExchangeStarted, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.ExchangeStarted -= new IXFSCashInServiceEvents_ExchangeStartedEventHandler(CIM_ExchangeStarted);
            xfsDevice.StartExchangeFailed -= new IXFSCashInServiceEvents_StartExchangeFailedEventHandler(CIM_StartExchangeFailed);
            xfsDevice.EndExchangeOK += new IXFSCashInServiceEvents_EndExchangeOKEventHandler(CIM_EndExchangeOK);
            xfsDevice.EndExchangeFailed += new IXFSCashInServiceEvents_EndExchangeFailedEventHandler(CIM_EndExchangeFailed);

            XFSDictionary ExchangeDictionary = new XFSDictionary();
            ExchangeDictionary = SetAllCashUnits();
            Logger.Logger.LogWrite("DesCashIn - CIM_ExchangeStarted, ExchangeDictionary.Count:{0}", Logger.Logger.LogTypes.Development, ExchangeDictionary.Count.ToString());
            if (ExchangeDictionary.Count > 0)
                xfsDevice.EndExchange(ExchangeDictionary);
            else
            {
                Logger.Logger.LogWrite("DesCashIn - CIM_ExchangeStarted, ExchangeDictionary.Count < 1", Logger.Logger.LogTypes.Development);
                iRetVal = (int)DeviceInterface.CashInGeneralResults.Error;
                CIMEvent.Set();
            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CIM_ExchangeStarted, Exit", Logger.Logger.LogTypes.Development);
            #endregion
        }
        /// <summary>
        /// ErrorInfo
        /// </summary>
        /// <remarks>
        /// CIM_EXCHANGE_END_CASH_UNIT_ERROR = 1,
        /// CIM_EXCHANGE_END_INACTIVE = 2,
        /// CIM_EXCHANGE_END_TIME_OUT = 100,
        /// CIM_EXCHANGE_END_HARDWARE_ERROR = 101,
        /// CIM_EXCHANGE_END_DEVICE_LOCKED = 102,
        /// CIM_EXCHANGE_END_UNSUPPORTED_METHOD = 103,
        /// CIM_EXCHANGE_END_CANCELLED = 104,
        /// CIM_EXCHANGE_END_UNSUPPORTED_CATEGORY = 105,
        /// CIM_EXCHANGE_END_CONNECTION_LOST = 106,
        /// CIM_EXCHANGE_END_DEVICE_NOT_READY = 107,
        /// CIM_EXCHANGE_END_EXCEPTION_OCCURRED = 108,
        /// CIM_EXCHANGE_END_UNSUPPORTED_DATA = 109,
        /// CIM_EXCHANGE_END_INVALID_DATA = 110,
        /// CIM_EXCHANGE_END_USER_ERROR = 111,
        /// </remarks>
        void CIM_EndExchangeFailed(int requestID, CIMExchangeEndError errorInfo, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CIM_EndExchangeFailed, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            iRetVal = (int)errorInfo;

            xfsDevice.EndExchangeOK -= new IXFSCashInServiceEvents_EndExchangeOKEventHandler(CIM_EndExchangeOK);
            xfsDevice.EndExchangeFailed -= new IXFSCashInServiceEvents_EndExchangeFailedEventHandler(CIM_EndExchangeFailed);

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CIM_EndExchangeFailed, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        void CIM_EndExchangeOK(int requestID, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CIM_EndExchangeOK, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.EndExchangeOK -= new IXFSCashInServiceEvents_EndExchangeOKEventHandler(CIM_EndExchangeOK);
            xfsDevice.EndExchangeFailed -= new IXFSCashInServiceEvents_EndExchangeFailedEventHandler(CIM_EndExchangeFailed);

            iRetVal = (int)DeviceInterface.CashInGeneralResults.OK;

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CIM_EndExchangeOK, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            CIMEvent.Set();
        }

        /// <summary>
        /// Gets a dictionary containg the logical ID's of the CashUnits
        /// </summary>
        protected virtual XFSDictionary GetCashUnitLogicalIds()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetCashUnitLogicalIds, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            XFSDictionary unitIDs = new XFSDictionary();
            XFSDictionary localCashUnits = new XFSDictionary();

            localCashUnits = xfsDevice.CashUnits;

            Array cashUnitKeys = localCashUnits.Keys() as Array;
            Array cashUnitItems = localCashUnits.Items() as Array;

            Logger.Logger.LogWrite("DesCashIn - GetCashUnitLogicalIds, cashUnitKeys.Length :{0}", Logger.Logger.LogTypes.Development, cashUnitKeys.Length.ToString());

            int cashUnit = 0;
            for (int i = 0; i < cashUnitKeys.Length; i++)
            {
                IXFSCashInCassette cashUnitItem = cashUnitItems.GetValue(i) as IXFSCashInCassette;

                if (CashUnitConfigured(cashUnitItem))
                {
                    cashUnit++;

                    object cashUnitKey = cashUnit;
                    object logicalId = cashUnit; //have to shuffle up all cash unit numbers

                    unitIDs.Add(ref cashUnitKey, ref logicalId);
                    Logger.Logger.LogWrite("DesCashIn - GetCashUnitLogicalIds, cashUnit:{0}", Logger.Logger.LogTypes.Development, cashUnit.ToString());
                }
            }
            Logger.Logger.LogWrite("DesCashIn - GetCashUnitLogicalIds, unitIDs.Count:{0}", Logger.Logger.LogTypes.Development, unitIDs.Count.ToString());

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetCashUnitLogicalIds, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            return unitIDs;
        }

        protected virtual bool CashUnitConfigured(IXFSCashInCassette cashUnit)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashUnitConfigured, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            bool configured = true;

            if (cashUnit != null)
            {
                if (cashUnit.UnitType == CIMCassetteType.CIM_RECYCLING)
                {
                    if ((cashUnit.CashUnitCurrency == string.Empty) || (cashUnit.CashUnitValue == 0))
                        configured = false;
                }
            }
            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - CashUnitConfigured, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            return configured;
        }

        protected virtual XFSDictionary SetAllCashUnits()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - SetAllCashUnits, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            XFSDictionary newCashUnits = new XFSDictionary();
            XFSDictionary localCashUnits = new XFSDictionary();

            localCashUnits = xfsDevice.CashUnits;

            Array cashUnitKeys = localCashUnits.Keys() as Array;
            Array cashUnitItems = localCashUnits.Items() as Array;

            Logger.Logger.LogWrite("DesCashIn - SetAllCashUnits, cashUnitKeys.Length :{0}", Logger.Logger.LogTypes.Development, cashUnitKeys.Length.ToString());

            int cashUnit = 0;
            for (int i = 0; i < cashUnitKeys.Length; i++)
            {
                IXFSCashInCassette cashUnitItem = cashUnitItems.GetValue(i) as IXFSCashInCassette;

                //do not pass in the logical id for a cassette this is not configured
                if (CashUnitConfigured(cashUnitItem))
                {
                    cashUnitItem.UnitInCount = 0;
                    cashUnitItem.UnitCurrentCount = 0;
                    cashUnitItem.NoteTypesCount.RemoveAll();


                    Logger.Logger.LogWrite("***************************************************", Logger.Logger.LogTypes.Development);
                    Logger.Logger.LogWrite("--------, CashUnitCurrency:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.CashUnitCurrency.ToString());
                    Logger.Logger.LogWrite("--------, CashUnitValue:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.CashUnitValue.ToString());
                    Logger.Logger.LogWrite("--------, CassetteReplenishmentStatus:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.CassetteReplenishmentStatus.ToString());
                    Logger.Logger.LogWrite("--------, CassetteStatus:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.CassetteStatus.ToString());
                    Logger.Logger.LogWrite("--------, MaximumItems:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.MaximumItems.ToString());
                    Logger.Logger.LogWrite("--------, NumberOfPhysicalUnits:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.NumberOfPhysicalUnits.ToString());
                    Logger.Logger.LogWrite("--------, UnitApplicationLock:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.UnitApplicationLock.ToString());
                    Logger.Logger.LogWrite("--------, UnitCurrentCount:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.UnitCurrentCount.ToString());
                    Logger.Logger.LogWrite("--------, UnitID:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.UnitID.ToString());
                    Logger.Logger.LogWrite("--------, UnitInCount:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.UnitInCount.ToString());
                    Logger.Logger.LogWrite("--------, UnitItemType:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.UnitItemType.ToString());
                    Logger.Logger.LogWrite("--------, UnitType:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.UnitType.ToString());

                    if (IsAtmDiebold)
                    {
                        if (cashUnitItem.NumberOfPhysicalUnits > 0)
                        {
                            XFSDictionary ddPUnits = cashUnitItem.PhysicalUnits;
                            object[] PUnits = ddPUnits.Items() as object[];
                            for (int k = 0; k < ddPUnits.Count; k++)
                            {
                                IXFSCashInPhysicalCassette tmpPUnit = PUnits[k] as IXFSCashInPhysicalCassette;
                                Logger.Logger.LogWrite("--------, 2tmpPUnit.InitialCount:{0}", Logger.Logger.LogTypes.Development, tmpPUnit.UnitCurrentCount.ToString());
                                Logger.Logger.LogWrite("--------, 2tmpPUnit.RejectCount:{0}", Logger.Logger.LogTypes.Development, tmpPUnit.UnitInCount.ToString());
                                tmpPUnit.UnitCurrentCount = 0;
                                tmpPUnit.UnitInCount = 0;
                            }
                        }
                    }

                    cashUnit++;
                    object cashUnitKey = cashUnit;
                    object UnitItem = cashUnitItem;

                    newCashUnits.Add(ref cashUnitKey, ref UnitItem);

                    Logger.Logger.LogWrite("DesCashIn - SetAllCashUnits, cashUnit                   :{0}", Logger.Logger.LogTypes.Development, cashUnit.ToString());
                    Logger.Logger.LogWrite("DesCashIn - SetAllCashUnits, cashUnitItem.CassetteStatus:{0}", Logger.Logger.LogTypes.Development, cashUnitItem.CassetteStatus.ToString());
                }
            }
            Logger.Logger.LogWrite("DesCashIn - SetAllCashUnits, newCashUnits.Count:{0}", Logger.Logger.LogTypes.Development, newCashUnits.Count.ToString());

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name, newCashUnits);
            Logger.Logger.LogWrite("DesCashIn - SetAllCashUnits, Exit", Logger.Logger.LogTypes.Development);
            #endregion

            return newCashUnits;
        }

        #endregion

        #region GetDepositedNoteTypeCount
        public void GetDepositedNoteTypeCount(DeviceInterface.CashInIdentifiedNotes totalIdentifiedNotes)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetDepositedNoteTypeCount, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            int cashInNoteTypesCount;
            int acceptableNotesCount;
            object dictNoteTypeCountKeys;
            object dictNoteTypeKeys;

            // Dictionary of XFSCashInNoteTypesCount
            cashInNoteTypesCount = noteTypesDict.Count - 1;
            dictNoteTypeCountKeys = noteTypesDict.Keys();
            System.Array keyArrayNoteTypesCount = (System.Array)dictNoteTypeCountKeys;
            // Dictionary of XFSCashInNoteTypes
            acceptableNotesCount = noteTypesDict.Count - 1;
            dictNoteTypeKeys = noteTypesDict.Keys();
            System.Array keyArrayNoteTypes = (System.Array)dictNoteTypeKeys;
            XFSDictionary cashUnits = null;
            cashUnits = xfsDevice.CashUnits;


            totalIdentifiedNotes.NumberOfIdentifiedNoteTypes = 0;
            if (cashUnits != null)
            {
                Array items = cashUnits.Items() as Array;

                for (int j = 1; j <= items.Length; j++)
                {
                    Des.Pascal.Nouma.IXFSCashInCassette item = items.GetValue(j - 1) as Des.Pascal.Nouma.IXFSCashInCassette;

                    if (item != null && item.UnitType == CIMCassetteType.CIM_CASH_IN)
                    {

                        Logger.Logger.LogWrite("item.NoteTypesCount:{0}", Logger.Logger.LogTypes.Development, item.NoteTypesCount.Count.ToString());

                        GetNoteTypesCounts(item.NoteTypesCount, j, totalIdentifiedNotes); //per cassette
                    }
                }
            }
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetDepositedNoteTypeCount, Exit", Logger.Logger.LogTypes.Development);
            #endregion

        }
        protected virtual void GetNoteTypesCounts(XFSDictionary dictionary, int numCassette, DeviceInterface.CashInIdentifiedNotes totalIdentifiedNotes)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name, dictionary.ToString());
            Logger.Logger.LogWrite("DesCashIn - GetNoteTypesCounts, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            if (dictionary != null)
            {
                Array items = dictionary.Items() as Array;
                Array allNoteTypes = xfsDevice.ItemNoteTypes.Items() as Array;

                for (int i = 0; i < items.Length; i++)
                {
                    Des.Pascal.Nouma.IXFSCashInNoteTypesCount item = items.GetValue(i) as Des.Pascal.Nouma.IXFSCashInNoteTypesCount;

                    if (item != null)
                    {
                        //find note type to get currency type and value
                        for (int j = 0; j < allNoteTypes.Length; j++)
                        {
                            Des.Pascal.Nouma.IXFSCashInNoteTypes nType = allNoteTypes.GetValue(j) as Des.Pascal.Nouma.IXFSCashInNoteTypes;

                            if (nType != null)
                            {
                                if (nType.NoteID == item.NoteID)
                                {
                                    totalIdentifiedNotes.IdentifiedNote[totalIdentifiedNotes.NumberOfIdentifiedNoteTypes].Count = item.Count;
                                    totalIdentifiedNotes.IdentifiedNote[totalIdentifiedNotes.NumberOfIdentifiedNoteTypes].Currency = nType.CashUnitCurrency;
                                    totalIdentifiedNotes.IdentifiedNote[totalIdentifiedNotes.NumberOfIdentifiedNoteTypes].Denom = nType.CashUnitValue;
                                    Logger.Logger.LogWrite("item.NoteID, item.CountnType.CashUnitCurrency, nType.CashUnitValue:{0},{1},{2},{3}", Logger.Logger.LogTypes.Development, item.NoteID.ToString(), item.Count.ToString(), nType.CashUnitCurrency.ToString(), nType.CashUnitValue.ToString());
                                    //IDesement when new type added (0->1->2)
                                    totalIdentifiedNotes.NumberOfIdentifiedNoteTypes++;
                                    break;
                                }
                            }
                        }
                    }
                }

            }

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetNoteTypesCounts, EXIT", Logger.Logger.LogTypes.Development);
            #endregion
        }

        #endregion

        #region ConfigureCashUnits
        public DeviceInterface.CashInGeneralResults ConfigureCashUnits()
        {
            return DeviceInterface.CashInGeneralResults.OK;
        }
        #endregion

        #region Handle Sessions
        public void CloseSession()
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashout - CloseSession, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            xfsDevice.SyncCloseSession();

            Logger.Logger.LogWrite("DesCashout - CloseSession result : " + xfsDevice.DeviceStatus.ToString(), Logger.Logger.LogTypes.Development);

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashout - CloseSession, Exit", Logger.Logger.LogTypes.Development);
            #endregion

        }

        public void OpenSession()
        {

            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashout - OpenSession, Entry", Logger.Logger.LogTypes.Development);
            #endregion

            XFSOpenSessionStatus ret;
            ret = xfsDevice.SyncOpenSession();

            Logger.Logger.LogWrite("DesCashout - OpenSession result : " + ret.ToString(), Logger.Logger.LogTypes.Development);

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashout - OpenSession, Exit", Logger.Logger.LogTypes.Development);
            #endregion

        }
        #endregion

        #region GetP6Signature
        public void GetSignature()
        {
            try
            {
                #region Problem Determination
                sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - GetSignature, Entry", Logger.Logger.LogTypes.Development);
                #endregion Problem Determination

                IXFSDictionary p6Info = ((Des.Pascal.Nouma.IXFSC302CashInSpecific)xfsDevice).P6Info as IXFSDictionary;
                Logger.Logger.LogWrite("DesCashIn - GetSignature, p6Info.Count :{0}", Logger.Logger.LogTypes.Development, p6Info.Count.ToString());


                object keys = p6Info.Keys();
                System.Array keyArray = (System.Array)keys;

                int Level2NoteCount = 0;
                int Level3NoteCount = 0;
                int Level4NoteCount = 0;
                foreach (object key in keyArray)
                {
                    object refKey = key;

                    XFSP6Info P6InfoStructure = (XFSP6Info)p6Info.get_Item(ref refKey);
                    Logger.Logger.LogWrite("GetSignature, P6InfoStructure.level :{0}", Logger.Logger.LogTypes.Development, P6InfoStructure.level.ToString());
                    //Logger.Logger.LogWrite("GetSignature, P6InfoStructure.NoteNumberList :{0}", Logger.Logger.LogTypes.Development, P6InfoStructure.NoteNumberList.ToString());
                    object notelist = P6InfoStructure.NoteNumberList.Keys();
                    System.Array notelistarray = (System.Array)notelist;
                    XFSCashInNoteTypesCount notetypes = null;
                    foreach (object note in notelistarray)
                    {
                        object refnote = note;
                        notetypes = (XFSCashInNoteTypesCount)P6InfoStructure.NoteNumberList.get_Item(ref refnote);
                        Logger.Logger.LogWrite("GetSignature, notetypes.Count :{0}", Logger.Logger.LogTypes.Development, notetypes.Count.ToString());
                        Logger.Logger.LogWrite("GetSignature, notetypes.NoteID :{0}", Logger.Logger.LogTypes.Development, notetypes.NoteID.ToString());
                    }
                    Logger.Logger.LogWrite("GetSignature, P6InfoStructure.NumberOfSignatures :{0}", Logger.Logger.LogTypes.Development, P6InfoStructure.NumberOfSignatures.ToString());
                    if (P6InfoStructure.level == CIMP6Level.CIM_LEVEL_2)
                    {
                        Level2NoteCount = (int)P6InfoStructure.NumberOfSignatures;
                    }
                    if (P6InfoStructure.level == CIMP6Level.CIM_LEVEL_3)
                    {
                        Level3NoteCount = (int)P6InfoStructure.NumberOfSignatures;
                    }
                    if (P6InfoStructure.level == CIMP6Level.CIM_LEVEL_4)
                    {
                        Level4NoteCount = (int)P6InfoStructure.NumberOfSignatures;
                    }
                }
                Logger.Logger.LogWrite("Level 2 Signature : {0}", Logger.Logger.LogTypes.Development, Level2NoteCount.ToString());
                if (Level2NoteCount > 0)
                {
                    for (int i = 0; i < Level2NoteCount - 1; i++)
                    {
                        CIMP6Sig.Reset();
                        xfsDevice.GetP6SignatureFailed += new IXFSCashInServiceEvents_GetP6SignatureFailedEventHandler(xfsDevice_GetP6SignatureFailed);
                        xfsDevice.GetP6SignatureOK += new IXFSCashInServiceEvents_GetP6SignatureOKEventHandler(xfsDevice_GetP6SignatureOK);
                        xfsDevice.GetP6Signature(CIMP6Level.CIM_LEVEL_2, (short)(i));
                        CIMP6Sig.WaitOne(WAIT_P6_TIME);
                    }
                }
                Logger.Logger.LogWrite("Level 3 Signature : {0}", Logger.Logger.LogTypes.Development, Level3NoteCount.ToString());
                if (Level3NoteCount > 0)
                {
                    for (int i = 0; i < Level3NoteCount - 1; i++)
                    {
                        CIMP6Sig.Reset();
                        xfsDevice.GetP6SignatureFailed += new IXFSCashInServiceEvents_GetP6SignatureFailedEventHandler(xfsDevice_GetP6SignatureFailed);
                        xfsDevice.GetP6SignatureOK += new IXFSCashInServiceEvents_GetP6SignatureOKEventHandler(xfsDevice_GetP6SignatureOK);
                        xfsDevice.GetP6Signature(CIMP6Level.CIM_LEVEL_3, (short)(i));
                        CIMP6Sig.WaitOne(WAIT_P6_TIME);
                    }
                }
                Logger.Logger.LogWrite("Level 4 Signature : {0}", Logger.Logger.LogTypes.Development, Level4NoteCount.ToString());
                if (Level4NoteCount > 0)
                {
                    for (short i = 0; i < Level4NoteCount; i++)
                    {
                        CIMP6Sig.Reset();
                        xfsDevice.GetP6SignatureFailed += new IXFSCashInServiceEvents_GetP6SignatureFailedEventHandler(xfsDevice_GetP6SignatureFailed);
                        xfsDevice.GetP6SignatureOK += new IXFSCashInServiceEvents_GetP6SignatureOKEventHandler(xfsDevice_GetP6SignatureOK);
                        xfsDevice.GetP6Signature(CIMP6Level.CIM_LEVEL_4, i);
                        CIMP6Sig.WaitOne(WAIT_P6_TIME);
                    }
                }


                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - GetSignature, Exit", Logger.Logger.LogTypes.Development);
                #endregion Problem Determination
            }
            catch (System.Exception ex)
            {
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - GetSignature, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
            }

        }

        void xfsDevice_GetP6SignatureOK(int requestID, XFSP6Signature signature, DateTime timeStamp)
        {
            try
            {
                #region Problem Determination
                sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - GetP6SignatureOK, Entry", Logger.Logger.LogTypes.Development);
                #endregion Problem Determination


                xfsDevice.GetP6SignatureOK -= new IXFSCashInServiceEvents_GetP6SignatureOKEventHandler(xfsDevice_GetP6SignatureOK);
                xfsDevice.GetP6SignatureFailed -= new IXFSCashInServiceEvents_GetP6SignatureFailedEventHandler(xfsDevice_GetP6SignatureFailed);
                StringBuilder serialNo = new StringBuilder();

                foreach (byte data in signature.signature)
                {

                    serialNo.Append(System.Convert.ToChar(data));

                }

                String srnl = Encoding.ASCII.GetString(signature.signature).ToString();

                int iPos = 0;
                StringBuilder sNumber = new StringBuilder();

                Logger.Logger.LogWrite("srnl =" + srnl, Logger.Logger.LogTypes.Development);

                iPos = srnl.IndexOf("SNUML", 0);
                srnl = srnl.Substring(iPos + 8).Trim();
                srnl = srnl.Substring(0, 47);


                sNumber = new StringBuilder(srnl.Substring(0, 1));
                for (int i = 0; i + 5 < srnl.Length; )
                {
                    sNumber.Append(srnl.Substring(i + 5, 1));
                    i += 5;
                }

                Logger.Logger.LogWrite("P6 DATA NoteID      =" + signature.NoteID.ToString(), Logger.Logger.LogTypes.Development);
                Logger.Logger.LogWrite("P6 DATA Orientation =" + signature.Orientation.ToString(), Logger.Logger.LogTypes.Development);
                Logger.Logger.LogWrite("Serial Number   =" + sNumber.ToString(), Logger.Logger.LogTypes.Development);

                #region Problem Determination
                sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
                Logger.Logger.LogWrite("DesCashIn - GetP6SignatureOK, Exit", Logger.Logger.LogTypes.Development);
                #endregion Problem Determination
                CIMP6Sig.Set();
            }
            catch (System.Exception ex)
            {
                CIMP6Sig.Set();
                #region Problem Determination
                sp.Exception(MethodBase.GetCurrentMethod().Name, ex);
                Logger.Logger.LogWrite("DesCashIn - GetP6SignatureOK, Exceptions :({0})", Logger.Logger.LogTypes.Exceptions, ex.ToString());
                #endregion
            }
        }

        void xfsDevice_GetP6SignatureFailed(int requestID, CIMGetP6SignatureError reason, DateTime timeStamp)
        {
            #region Problem Determination
            sp.ImplementationEntry(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetP6SignatureFailed, Entry", Logger.Logger.LogTypes.Development);
            #endregion Problem Determination

            xfsDevice.GetP6SignatureOK -= new IXFSCashInServiceEvents_GetP6SignatureOKEventHandler(xfsDevice_GetP6SignatureOK);
            xfsDevice.GetP6SignatureFailed -= new IXFSCashInServiceEvents_GetP6SignatureFailedEventHandler(xfsDevice_GetP6SignatureFailed);
            CIMP6Sig.Set();

            #region Problem Determination
            sp.ImplementationExit(MethodBase.GetCurrentMethod().Name);
            Logger.Logger.LogWrite("DesCashIn - GetP6SignatureFailed, Exit", Logger.Logger.LogTypes.Development);
            #endregion Problem Determination

        }
        #endregion

        public DeviceInterface.CashInGeneralResults GetBimNoteTypeInfo(ref List<DeviceInterface.BIMNoteTypes> noteTypeList)
        {
            throw new NotImplementedException();
        }

        public DeviceInterface.CashInGeneralResults ConfigureSelectedCashUnits(List<DeviceInterface.BIMNoteTypes> selectedNoteTypeList)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
