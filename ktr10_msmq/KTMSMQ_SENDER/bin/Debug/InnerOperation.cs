using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Verisoft.Logging;
using Dgiworks.Kiosk.Messages;
using Verisoft;
using Microsoft.Win32;
using KT.WOSA.CIM.IMP;
using KT.WOSA.CIM;


namespace KTMSMQ_SENDER
{

    using CIM = XfsCimDefine;
    using KT.WOSA;
    using KT.WOSA.CIM.IMP;
    using System.Runtime.InteropServices;



    class InnerOperation
    {

        public const string ProgramName = "Kiosk KTR10 Service";


        private static volatile bool serviceRunning;
        private string logName = ProgramName + " Log";
        private Logger logger = null;

        private string productKEY = "ABCDEF0123456789";
        private string localQueueNameForReading = "";
        private string localQueueNameForWriting = "";
        private string localQueueNameForLogging = "";
        public static string msmqLabel = "KTR10";
        private string SNRIniFileName = "";
        private string DeviceID = "0";
        private string KIOSK_ID = "0";
        private string MERCHANT_ID = "0";


        private int listenerThreadSleepTimeInMillisecond;
        private int innerMainThreadSleepTime;
        private int receiveTimeout;
        private string comPort;


        public static string XFSPicSavePath = "";
        public static string XFSIniSavePath = "";

        public static int IsCreateDATFile = 0;
        public static int IsCreateFSNFile = 0;
        public static int IsCreateJPGFile = 0;
        public static int IsCreateOCRFile = 0;
        public static int IsCreateTXTFile = 0;

        bool getRegOk = false;
        RegistryKey key = null;


        XfsCimImp imp = new XfsCimImp();
        SNRInfo.CSNRInfo dd = new SNRInfo.CSNRInfo();




        public InnerOperation()
        {
            imp.cimEvent = CIMEvent;


        }
        private static object startStopObject = new object();
        private static bool started = false;
        private object mainLock = new object();




        public void Start()
        {
            lock (startStopObject)
            {
                if (!started) // with lock and if, InnerOperation instance can be started only once
                {


                    Thread innerMainThread = null;
                    System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
                    innerMainThread = new Thread(new ThreadStart(InnerMainThreadProcess));
                    serviceRunning = true;
                    innerMainThread.Start();
                    started = true;
                }
            }
        }
        public void Stop()
        {
            serviceRunning = false;
        }


        private void InnerMainThreadProcess()
        {
            lock (mainLock) //InnerOperation instance is one-time-startable. But still we use this as a measurement
            {

                int errorCount = 0;

                bool openRet = false;


                string errors = "";


                if (!InitLogger())
                {
                    errorCount++;
                    errors += "InitLogger, ";
                }
                if (!CheckEventLog())
                {
                    errorCount++;
                    errors += "CheckEventLog, ";
                }

                if (!RetrievePropertiesFromREgedit())
                {
                    errorCount++;
                    errors += "RetrievePropertiesFromRegedit, ";
                }


                if (!RetrieveFromRegeditSFX())
                {
                    errorCount++;
                    errors += "RetrieveFromRegeditSFX() ";
                }






                if (!CheckQueues())
                {
                    errorCount++;
                    errors += "CheckQueues, ";
                }



                //if (!initKTR10())
                //{
                //    errorCount++;
                //    errors += "initKTR10, ";
                //}

                if (errorCount > 0)
                {
                    Log("BNA " + errors + " process steps checking error", EventLogEntryType.Error);
                    Environment.Exit(Environment.ExitCode);
                }

                // if (serviceRunning)
                // openRet = openBNA();   
                Log("KTR10 Open First Check Attempt.", EventLogEntryType.Information);
                Thread.Sleep(1000); // check KTR10 devices
                                    //if (BnaMeiSCN83.IsTurnedOn() == false)
                                    //{
                                    //    Log("BNA Open Second Check Attempt.", EventLogEntryType.Information);
                                    //    Thread.Sleep(4000);
                                    //    if (BnaMeiSCN83.IsTurnedOn() == false)
                                    //    {
                                    //        Log("Cannot open " + comPort + " Port, but BNA Service Ready", EventLogEntryType.Error);
                                    //    }

                
                while (serviceRunning)//this loop will check if threads are alive once in innerMainThreadSleepTime milliseconds
                {
                    try
                    {
                        // CheckThreads(); 
                        ListenQueue();
                        if (serviceRunning)
                            try
                            {
                                Thread.Sleep(innerMainThreadSleepTime);
                                Console.WriteLine("Sleep.........");
                            }
                            catch { }
                    }
                    catch (Exception e)
                    {
                        Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + e.Message, EventLogEntryType.Error);
                    }
                }
                DisposeElements();

            }
        }

        private void CheckThreads()
        {
            StartListenerThreadProcess();
        }
        private Thread listenerThread = null;
        private void StartListenerThreadProcess()
        {
            if (listenerThread == null || !listenerThread.IsAlive)
            {
                listenerThread = new Thread(new ThreadStart(CIMStatus));
                listenerThread.Start();
                // Log("Msmq Listener Thread started.", EventLogEntryType.Information);
            }
        }
        private bool isListening = false;
        private object listenLock = new object();





        private void CIMStatus()
        {
            try
            {

                string s = "WFS_INF_CIM_STATUS:\r\n";
                KT.WOSA.CIM.XfsCimDefine.WFSCIMSTATUS_dim lpDataOut;
                imp.WFS_INF_CIM_STATUS_Impl(out lpDataOut);

                s += "\tfwDevice:" + lpDataOut.fwDevice.ToString().PadLeft(4, '0') + "\r\n";
                s += "\tfwSafeDoor:" + lpDataOut.fwSafeDoor.ToString().PadLeft(4, '0') + "\r\n";
                s += "\tfwDispenser:" + lpDataOut.fwAcceptor.ToString().PadLeft(4, '0') + "\r\n";
                s += "\tfwIntermediateStacker:" + lpDataOut.fwIntermediateStacker.ToString().PadLeft(4, '0') + "\r\n";

                s += "\tbDropBox: \r\n";

                Console.WriteLine("CIMStatus............." + s);
            }
            catch (Exception)
            {


            }
        }



        private void ListenQueue()
        {

            Console.WriteLine("ListenQueue.........");

            if (!isListening) //prevent more than one thread to come in and run this method (Not completely effective but a pre-measurement)
            {
                isListening = true;

                //but if more than one thread comes in
                //make them wait here
                lock (listenLock)
                {
                    MessageQueueTransaction transaction = new MessageQueueTransaction();
                    bool rollBack = false;


                    //Step 2: create the queues
                    string error;
                    bool queuesCreated = true;
                    MessageQueue messageQueueForReading = null;
                    MessageQueue messageQueueForWriting = null;
                    MessageQueue messageQueueForLogging = null;
                    try
                    {
                        //Recreate the queues
                        #region queues



                        messageQueueForReading = MSMQ.createMSMQ(localQueueNameForReading, out error);
                        messageQueueForWriting = MSMQ.createMSMQ(localQueueNameForWriting, out error);
                        messageQueueForLogging = MSMQ.createMSMQ(localQueueNameForLogging, out error);

                    }
                    catch (Exception e)
                    {
                        Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + e.Message, System.Diagnostics.EventLogEntryType.Error);
                        queuesCreated = false;
                    }
                    #endregion

                    //Step 3: If everything goes ok, then begin the transaction
                    if (queuesCreated)
                    {
                        transaction.Begin();
                        try
                        {
                            //timeout will cause an exception if expires.
                            TimeSpan timeSpan = TimeSpan.FromMilliseconds(receiveTimeout);
                            System.Messaging.Message message = messageQueueForReading.Receive(timeSpan, transaction);
                            //during the waiting, the service can be stopped. Check it..
                            if (!serviceRunning)
                                throw new Exception("Service was about to read a message while service is not running.");

                            //process the message, get a log and an answer message. Print the log and send the message to msmq
                            message.Formatter = new XmlMessageFormatter(new Type[] { typeof(Ktr10) });
                            Ktr10 ktrMessage = (Ktr10)message.Body;
                            Ktr10 response = new Ktr10(); // burası update edilecek

                            // erkan burası KTR10 Methodlarına gidecek
                            // int ret = bnaMessageProcessor.Process(comPort, ktrMessage, out response, out logMessage);

                            EventLogEntryType entryType = EventLogEntryType.Error;

                            string s = "";
                            bool XmlMessageOK = false;


                            #region CashAcceptor



                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMOpenRegister)
                            {
                                XmlMessageOK = true;


                                System.String strLogicalName = "CashAcceptor";
                                int hr = imp.OpenSP_Impl(strLogicalName);
                                if (XfsGlobalDefine.WFS_SUCCESS != hr)
                                {
                                    s += "Failed return " + hr + "\r\n";
                                    Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + s, entryType);

                                    response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                    ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;

                                }
                                else
                                {
                                    response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;

                                    #region WFSVersion


                                    KT.WOSA.CIM.XfsGlobalDefine.WFSVERSION WFSVersion, SrvcVersion, SPIVersion;

                                    imp.GetSpInfoVersion_Impl(out WFSVersion, out SrvcVersion, out SPIVersion);
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


                                    response.DESCRIPTION = s;

                                    #endregion

                                    entryType = EventLogEntryType.Information;
                                    Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + s, entryType);
                                }

                                response.ID = ktrMessage.ID;
                            }

                            #endregion 
                            #region CashAcceptorStatus



                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMStatusRegister)
                            {
                                XmlMessageOK = true;
                                s = "WFS_INF_CIM_STATUS:\r\n";
                                KT.WOSA.CIM.XfsCimDefine.WFSCIMSTATUS_dim lpDataOut;

                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + s, entryType);

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
                                        //   Console.WriteLine(s);
                                    }
                                }



                                response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;

                                ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                ktrMessage.DESCRIPTION = s;

                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + s, entryType);

                            }

                            #endregion 
                            #region WFS_CMD_CIM_RETRACT
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMRetrack)
                            {
                                XmlMessageOK = true;
                                s = "WFS_CMD_CIM_RETRACT:\r\n";
                                try
                                {
                                    CIM.WFSCIMRETRACT retract = new CIM.WFSCIMRETRACT();
                                 

                                    retract.fwOutputPosition = CIM.WFS_CIM_POSNULL;
                                   retract.usRetractArea = CIM.WFS_CIM_RA_RETRACT ; 
                                    retract.usIndex = 0;
                                    int hRet = imp.WFS_CMD_CIM_RETRACT_Impl(retract);
                                    s += "return " + hRet + "\r\n";

                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);
                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;

                                    }

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



                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;

                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);


                            }
                            #endregion 
                            #region SHUTTER OPEN
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMShutterOpen)
                            {
                                XmlMessageOK = true;
                                s = "WFS_CMD_CIM_OPEN_SHUTTER:\r\n";
                                try
                                {
                                    System.UInt16 fwPosition = CIM.WFS_CIM_POSNULL;
                                    System.Int32 hRet = imp.WFS_CMD_CIM_OPEN_SHUTTER_Impl(fwPosition);
                                    Console.WriteLine("\r\nWFS_CMD_CDM_OPEN_SHUTTER_Impl return  ", hRet);
                                    s += "return  " + hRet + "\r\n";

                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;

                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);


                            }
                            #endregion
                            #region SHUTTER CLOSE
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMShutterClose)
                            {
                                XmlMessageOK = true;
                                s = " WFS_CMD_CIM_CLOSE_SHUTTER:\r\n";
                                try
                                {
                                    System.UInt16 fwPosition = CIM.WFS_CIM_POSNULL;
                                    System.Int32 hRet = imp.WFS_CMD_CIM_CLOSE_SHUTTER_Impl(fwPosition);
                                    Console.WriteLine("\r\nWFS_CMD_CIM_CLOSE_SHUTTER_Impl return  ", hRet);
                                    s += "return " + hRet + "\r\n";

                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;

                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion 
                            #region SHUTTER STATUS
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMShutterStatus)
                            {
                                XmlMessageOK = true;
                                s = "WFS_CMD_CIM_OPEN_SHUTTER:\r\n";
                                try
                                {
                                    System.UInt16 fwPosition = CIM.WFS_CIM_POSNULL;
                                    System.Int32 hRet = imp.WFS_CMD_CIM_CLOSE_SHUTTER_Impl(fwPosition);
                                    Console.WriteLine("\r\nWFS_CMD_CIM_CLOSE_SHUTTER_Impl return  ", hRet);
                                    s += "return  " + hRet + "\r\n";

                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion 
                            #region cash ın start
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMCashInStart)
                            {
                                XmlMessageOK = true;
                                s = "WFS_CMD_CIM_CASH_IN_START:\r\n";
                                try
                                {
                                    CIM.WFSCIMCASHINSTART CashInStart = new CIM.WFSCIMCASHINSTART();
                                    CashInStart.usTellerID = 0;
                                    CashInStart.bUseRecycleUnits = 1;
                                    CashInStart.fwOutputPosition = CIM.WFS_CIM_POSNULL;
                                    CashInStart.fwInputPosition = CIM.WFS_CIM_POSNULL;

                                    int hRet = imp.WFS_CMD_CIM_CASH_IN_START_Impl(CashInStart);
                                    s += "return " + hRet + "\r\n";


                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion
                            #region cashIN
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMCashIn)
                            {
                                XmlMessageOK = true;
                                s = "WFS_CMD_CIM_CASH_IN:\r\n";
                                try
                                {
                                    int hRet = imp.WFS_CMD_CIM_CASH_IN_Impl();
                                    s += "return " + hRet + "\r\n";


                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion
                            #region cash END
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMCashInEnd)
                            {
                                XmlMessageOK = true;
                                s = "WFS_CMD_CIM_CASH_IN_END:\r\n";
                                try
                                {
                                    int hRet = imp.WFS_CMD_CIM_CASH_IN_END_Impl();
                                    s += "return " + hRet + "\r\n";


                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion
                            #region cash Roolback
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMCashInRollBack)
                            {
                                XmlMessageOK = true;
                                s = "WFS_CMD_CIM_CASH_IN_ROLLBACK:\r\n";

                                try
                                {
                                    CIM.WFSCIMCASHINFO_dim cashInfo;
                                    int hRet = imp.WFS_CMD_CIM_CASH_IN_ROLLBACK_Impl(out cashInfo);
                                    s += "return " + hRet + "\r\n";


                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion
                            #region cash Retrack
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMRetrack)
                            {
                                XmlMessageOK = true;
                                s = "WFS_CMD_CIM_RETRACT:\r\n";

                                try
                                {
                                    CIM.WFSCIMRETRACT retract = new CIM.WFSCIMRETRACT();
                                    retract.fwOutputPosition = CIM.WFS_CIM_POSNULL;
                                    retract.usRetractArea = CIM.WFS_CIM_RA_RETRACT;
                                    retract.usIndex = 0;
                                    int hRet = imp.WFS_CMD_CIM_RETRACT_Impl(retract);
                                    s += "return " + hRet + "\r\n";


                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion
                            #region cash Reset
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMReset)
                            {
                                XmlMessageOK = true;
                                s = "WFS_CMD_CIM_RESET\r\n";

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
                                    s += "return  " + hRet + "\r\n";
                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {

                                        #region cihaz acılıp kapandığında gerekiyor.

                                        XmlMessageOK = true; 
                                        System.String strLogicalName = "CashAcceptor";
                                        int hr = imp.OpenSP_Impl(strLogicalName);

                                        #endregion

                                        //tekrar denemede kurtarıyor..
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);

                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion 
                            #region cash In Status
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMCashInStatus)
                            {
                                XmlMessageOK = true;
                                #region status 
                                s = "WFS_INF_CIM_CASH_IN_STATUS:\r\n";

                                try
                                {
                                    XfsCimDefine.WFSCIMCASHINSTATUS_dim lpCashInStatus;

                                    System.Int32 hRet = imp.WFS_INF_CIM_CASH_IN_STATUS_Impl(out lpCashInStatus);
                                    s += "return :" + hRet + "\r\n";

                                    s += "\twStatus = " + lpCashInStatus.wStatus + "\r\n";
                                    s += "\tusNumOfRefused = " + lpCashInStatus.usNumOfRefused + "\r\n";
                                    s += "\tlpNoteNumberList:\r\n";
                                    s += "\t\tusNumOfNoteNumbers = " + lpCashInStatus.lpNoteNumberListDim.usNumOfNoteNumbers + ":\r\n";
                                    s += "\t\tlppNoteNumber:\r\n";
                                    foreach (CIM.WFSCIMNOTENUMBER v in lpCashInStatus.lpNoteNumberListDim.lppNoteNumberDim)
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

                                    #endregion



                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);

                                    }
                                    else
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion 
                            #region CIM Capabilites
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMCapaBilites)
                            {

                                XmlMessageOK = true;
                                s = "WFS_INF_CIM_CAPABILITIES:\r\n";

                                try
                                {

                                    XfsCimDefine.WFSCIMCAPS lpCaps;

                                    System.Int32 hRet = imp.WFS_INF_CIM_CAPABILITIES_Impl(out lpCaps);
                                    s += "WFS_INF_CIM_CAPABILITIES return :" + hRet + "\r\n";





                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);

                                    }
                                    else
                                    {


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

                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion 
                            #region  BankNoteTypes
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMBankNoteTypes)
                            {
                                XmlMessageOK = true;

                                s = "WFS_INF_CIM_BANKNOTE_TYPES :\r\n";

                                try
                                {
                                    XfsCimDefine.WFSCIMNOTETYPELIST_dim lpCaps;

                                    System.Int32 hRet = imp.WFS_INF_CIM_BANKNOTE_TYPES_Impl(out lpCaps);
                                    s += " return :" + hRet + "\r\n";



                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);

                                    }
                                    else
                                    {

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


                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion
                            #region  CashUnitInfo
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMCashUnitInfo)
                            {
                                XmlMessageOK = true;
                                s = "WFS_INF_CIM_CASH_UNIT_INFO:\r\n";

                                try
                                {
                                    XfsCimDefine.WFSCIMCASHINFO_dim lpCashInfo;

                                    System.Int32 hRet = imp.WFS_INF_CIM_CASH_UNIT_INFO_Impl(out lpCashInfo);
                                    s += "return :" + hRet + "\r\n";


                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {
                                        #region lpCashInfo


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

                                            foreach (CIM.WFSCIMNOTENUMBER v in pos.lpNoteNumberListDim.lppNoteNumberDim)
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

                                        #endregion

                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion 
                            #region  CIMCurrencyExport
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMCurrencyExport)
                            {

                                XmlMessageOK = true;
                                s = "WFS_INF_CIM_CURRENCY_EXP:\r\n";

                                try
                                {
                                    List<XfsCimDefine.WFSCIMCURRENCYEXP> lpCurrencyExp;

                                    System.Int32 hRet = imp.WFS_INF_CIM_CURRENCY_EXP_Impl(out lpCurrencyExp);
                                    s += "WFS_INF_CIM_CURRENCY_EXP return :" + hRet + "\r\n";


                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
                                    {

                                        #region MyRegion
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
                                        #endregion

                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;
                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion
                            #region  WFS_INF_CIM_GET_ITEMS_INFO_Impl
                            if (ktrMessage.OPERATION_CODE == Ktr10.CIMGetItemsInfo)
                            {
                                XmlMessageOK = true;

                                try
                                {
                                    XfsCimDefine.WFSCIMITEMSINFO_dim lpDataOut;

                                    int hRet = imp.WFS_INF_CIM_GET_ITEMS_INFO_Impl(out lpDataOut);

                                    s = "WFS_INF_CIM_GET_ITEMS_INFO: ";
                                    s += "return :" + hRet;



                                    if (hRet != XfsGlobalDefine.WFS_SUCCESS)
                                    {
                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.FAILED;
                                        throw new KTR10_CIM.Exceptions.GeneralException(hRet);


                                    }
                                    else
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


                                        response.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                        ktrMessage.RESPONSE_CODE = ktrMessage.OPERATION_CODE + Ktr10.SUCCESS;
                                    }
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

                                response.DESCRIPTION = s;

                                response.ID = ktrMessage.ID;
                                ktrMessage.DESCRIPTION = s;
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.RESPONSE_CODE, entryType);
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + ktrMessage.DESCRIPTION, entryType);

                            }
                            #endregion 

                            ///////////////////////////////////////

                            #region msm xml message write

                            if (XmlMessageOK)
                            {
                                        message.Label = msmqLabel;
                                        messageQueueForWriting.Send(message, transaction);
                                        LogMessage lMessage = new LogMessage();
                                        lMessage.DATE = DateTime.Now.ToString(); 
                                        message = new System.Messaging.Message(response);
                                        message.Label = msmqLabel;
                                        lMessage.DEVICE_ID = DeviceID;
                                        lMessage.KIOSK_ID = KIOSK_ID;
                                        lMessage.MERCHANT_ID = MERCHANT_ID; 
                                        lMessage.MESSAGE_TYPE = (int)entryType;
                                        lMessage.MESSAGE_BODY = response.Serialize();
                                        message = new System.Messaging.Message(lMessage);
                                        message.Label = msmqLabel;
                                        messageQueueForLogging.Send(message, transaction);
                                        transaction.Commit();
                            }
                        }
                        catch (Exception e)
                        {
                            EventLogEntryType et = EventLogEntryType.Error;
                            if (e is MessageQueueException)
                            {
                                //if timeout, call it "warning" not "error"
                                if (((MessageQueueException)e).MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                                    et = EventLogEntryType.Warning;
                            }
                            else //timeout eventları gitmesin die else koydum  
                                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + e.Message, et);

                            rollBack = true;
                        }

                        if (rollBack)
                            transaction.Abort();

                        messageQueueForReading.Dispose();
                        messageQueueForWriting.Dispose();

                        #endregion

                    }


                }

            }

            isListening = false;
        }
    





        private void DisposeElements()
        {
            //int ret = BnaMeiSCN83.ClosePort();
            //if (ret == BnaMeiSCN83.SUCCESS)
            //{
            //    Log("Close " + comPort + " Port  Success", EventLogEntryType.Information);

            //}
            //else if (ret == BnaMeiSCN83.NOTTURNEDONERROR)
            //{
            //    Log(comPort + " Port  is not open", EventLogEntryType.Information);

            //}
            //else
            //{
            //    Log("Cannot Close " + comPort + " Port BNA.", EventLogEntryType.Error);

            //}
            //if (bnaMessageProcessor != null)
            //    bnaMessageProcessor = null;

            Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + "Service stopped.", EventLogEntryType.Information);
            if (logger != null)
                logger = null;

            started = false;

        }

        private bool InitLogger()
        {
            try
            {
                logger = new Logger(logName);
                Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + "Logger created.", EventLogEntryType.Information);
                return true;
            }
            catch (Exception e)
            {
                SimpleLogger.Write("Exception occured during logger creation:" + e.Message);
                return false;
            }
        }

        private bool CheckEventLog()
        {
            try
            {
                if (!EventLog.Exists(logName))
                {
                    EventLog.CreateEventSource(logName, logName);
                    Log(DateTime.Now.ToString("hh:mm:ss") + ">>" + "Event log created.", EventLogEntryType.Information);
                    return true;
                }
                else
                {
                    //  Log("Event log already exists. No need to create.", EventLogEntryType.Information);
                    return true;
                }
            }
            catch (Exception e1)
            {
                Log("Cannot check and create event log: " + e1.Message, EventLogEntryType.Error);
                return false;
            }
        }

        private bool CheckQueues()
        {
            if (localQueueNameForReading.Equals(""))
            {
                Log("Local queue name for reading is not valid.", EventLogEntryType.Error);
                return false;
            }
            try
            {
                if (!MessageQueue.Exists(localQueueNameForReading))
                {
                    MessageQueue queue = MessageQueue.Create(localQueueNameForReading, true);
                    if (queue != null)
                    {
                        queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                        queue.UseJournalQueue = true;
                    }
                    Log("Local queue Name:  " + localQueueNameForReading + " for reading created successfully.", EventLogEntryType.Information);
                }
                else
                {
                    Log("Local queue Name:  " + localQueueNameForReading + " for reading already exists. No need to create.", EventLogEntryType.Information);
                }
            }
            catch (Exception e)
            {
                Log("Cannot check and create local queue for reading: " + e.Message, EventLogEntryType.Error);
                return false;
            }

            if (localQueueNameForWriting.Equals(""))
            {
                Log("Local queue name for writing is not valid.", EventLogEntryType.Error);
                return false;
            }
            try
            {
                if (!MessageQueue.Exists(localQueueNameForWriting))
                {
                    MessageQueue queue = MessageQueue.Create(localQueueNameForWriting, true);
                    if (queue != null)
                    {
                        queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                        queue.UseJournalQueue = true;
                    }
                    Log("Local queue  Name:  " + localQueueNameForWriting + " for writing created successfully.", EventLogEntryType.Information);
                }
                else
                {
                    Log("Local queue  Name:  " + localQueueNameForWriting + " for writing already exists. No need to create.", EventLogEntryType.Information);
                }
            }
            catch (Exception e)
            {
                Log("Cannot check and create local queue for writing: " + e.Message, EventLogEntryType.Error);
                return false;
            }

            if (localQueueNameForLogging.Equals(""))
            {
                Log("Local queue name for logging is not valid.", EventLogEntryType.Error);
                return false;
            }
            try
            {
                if (!MessageQueue.Exists(localQueueNameForLogging))
                {
                    MessageQueue queue = MessageQueue.Create(localQueueNameForLogging, true);
                    if (queue != null)
                    {
                        queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                        queue.UseJournalQueue = true;
                    }
                    Log("Local queue  Name: " + localQueueNameForLogging + " for logging created successfully.", EventLogEntryType.Information);
                }
                else
                {
                    Log("Local queue  Name: " + localQueueNameForLogging + " for logging already exists. No need to create.", EventLogEntryType.Information);
                }
            }
            catch (Exception e)
            {
                Log("Cannot check and create local queue for logging: " + e.Message, EventLogEntryType.Error);
                return false;
            }
            return true;
        }

        private static string strSectionCashInfo= "LEVEL1_COUNT";
       

        private bool CheckIniFile()
        {


             

            try
            {

                var MyIni = new SNRInfo.IniFile(XFSIniSavePath +"\\"+ SNRIniFileName); 

                int CassetteCount = 4;
                int cassetteNo = 0;
                string _key = "";
                for (int i = 0; i < CassetteCount; i++)
                {
                    cassetteNo = i + 1;
                    //  _key = String.Format("LEVEL1_COUNT=", cassetteNo);

                    _key = "LEVEL4_COUNT";
                    if (!MyIni.KeyExists(_key, "Cash_Info"))
                    {

                        string a = MyIni.Read(_key, "Cash_Info"); 
                         
                    }
                   
                }



            }
            catch (Exception e)
            {
                Log("Cannot check and create local queue for reading: " + e.Message, EventLogEntryType.Error);
                return false;
            }

            return true;

        }




        private void Log(string message, EventLogEntryType type)
        {
            if (logger != null)
                logger.Log(DateTime.Now.ToString("hh:mm:ss") + " -> " + message, type);
        }

        private bool RetrievePropertiesFromREgedit()
        {
            string logMessage = "";
            string error;


            //SETTINGS GET FROM REG//


            try
            {
                key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Dgiworks\\KTR10");
                if (key != null)
                    getRegOk = true;

            }
            catch (Exception)
            {

            }



            if (!getRegOk)
                logMessage += "KTR settings cannot be found in the registry. ";
            else
            {
                localQueueNameForReading = key.GetValue("QREADNAME").ToString();
                localQueueNameForWriting = key.GetValue("QWRITENAME").ToString();
                localQueueNameForLogging = key.GetValue("QLOGNAME").ToString();
                msmqLabel = key.GetValue("MessageLabelName").ToString();
                SNRIniFileName = key.GetValue("SNRiniFile").ToString();
                DeviceID =   key.GetValue("DEVICE_ID").ToString() ;
                KIOSK_ID = key.GetValue("KIOSK_ID").ToString();
                MERCHANT_ID = key.GetValue("MERCHANT_ID").ToString();



                logger.IsTextLogEnabled = bool.Parse(key.GetValue("TEXTLOGENABLED").ToString());
                logger.IsEventLogEnabled = bool.Parse(key.GetValue("EVENTLOGENABLED").ToString());
                int eventTraceLevel = int.Parse(key.GetValue("EVENTTRACELEVEL").ToString());


                Logger.EventTraceLevel level = Logger.EventTraceLevel.OFF;
                if (eventTraceLevel == 1)
                    level = Logger.EventTraceLevel.ERROR;
                else if (eventTraceLevel == 2)
                    level = Logger.EventTraceLevel.ERRORANDWARNING;
                else if (eventTraceLevel == 3)
                    level = Logger.EventTraceLevel.ALL;

                logger.TraceLevelForEventMessage = level;

                listenerThreadSleepTimeInMillisecond = int.Parse(key.GetValue("LISTENERTHREADSLEEPTIMEINMILLISECOND").ToString());
                innerMainThreadSleepTime = int.Parse(key.GetValue("INNERMAINTHREADSLEEPTIMEINMILLISECOND").ToString());
                receiveTimeout = int.Parse(key.GetValue("RECEIVETIMEOUT").ToString());
                comPort = key.GetValue("COMPORT").ToString();
                productKEY = key.GetValue("PRODUCTKEY").ToString();
            }

            if (logMessage.Length == 0)
                Log("Properties successfully retrieved from the registry.", EventLogEntryType.Information);
            else
                Log(logMessage, EventLogEntryType.Error);

            return logMessage.Length == 0;
        }

        private bool RetrieveFromRegeditSFX()
        {
            string logMessage = "";
            string error;


            //SETTINGS GET FROM REG//
            bool getRegOk = false;
            RegistryKey key = null;

            try
            {
                key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\XFS\\SERVICE_PROVIDERS\\ktR10");
                if (key != null)
                    getRegOk = true;

            }
            catch (Exception)
            {
                getRegOk = false;
            }



            if (!getRegOk)
            {
                logMessage += "XFS\\SERVICE_PROVIDERS\\ktR10 settings cannot be found in the registry. ";

                getRegOk = false;
            }
            else
            {

                XFSPicSavePath = key.GetValue("PicSavePath").ToString();
                XFSIniSavePath = key.GetValue("IniSavePath").ToString();
                IsCreateDATFile = (int)key.GetValue("IsCreateDATFile");
                IsCreateFSNFile = (int)key.GetValue("IsCreateFSNFile");
                IsCreateJPGFile = (int)key.GetValue("IsCreateJPGFile");
                IsCreateOCRFile = (int)key.GetValue("IsCreateOCRFile");
                IsCreateTXTFile = (int)key.GetValue("IsCreateTXTFile");



                getRegOk = true;
            }

            return getRegOk;


        }

        private void msmq_send_event_message(string s, int eventId)
        {
            #region msm xml messagecreate

            if (eventId > 0)
            {

                try
                {

                    string error = string.Empty;
                    MessageQueue messageQueueForWriting = MSMQ.createMSMQ(localQueueNameForWriting, out error);
                    MessageQueueTransaction transaction = new MessageQueueTransaction();
                    transaction.Begin();

                    if (messageQueueForWriting != null || error != null)
                    {

                        Ktr10 ktrMessage = new Ktr10();
                        ktrMessage.RESPONSE_CODE = eventId.ToString();
                        ktrMessage.DESCRIPTION = s;


                        System.Messaging.Message m = new System.Messaging.Message(ktrMessage);
                        m.Label = "CIMEVENT";
                        messageQueueForWriting.Send(m, transaction);
                        transaction.Commit();


                    }

                }
                catch (Exception ex)
                {

                    Log(DateTime.Now.ToString("hh:mm:ss") + ">>>" + "CIMEVENT_xml message " + ex.Message, EventLogEntryType.Error);

                }

            }
            #endregion


        }

        public int CIMEvent(long eventId, XfsGlobalDefine.WFSRESULT WFSResult)
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



            // Trace(s);



            Console.WriteLine(s);
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

            msmq_send_event_message(s, (int)WFSResult.dwCmdCodeOrEventID);
            Console.WriteLine(s);
            //  Trace(s);
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

            msmq_send_event_message(s, (int)WFSResult.dwCmdCodeOrEventID);
            // Console.WriteLine(s);
            //Trace(s);
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
            // Trace(s);

            msmq_send_event_message(s, (int)WFSResult.dwCmdCodeOrEventID);
            Console.WriteLine(s);
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

            msmq_send_event_message(s, (int)WFSResult.dwCmdCodeOrEventID);
            Console.WriteLine(s);
            // Trace(s);
        }
    }
}

   
   
