using System;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using KTR10_CIM;

class Program
{

    static void Main()
    {

        int num = 4128;

        List<int> listOfInts = new List<int>();
        while (num > 0)
        {
            listOfInts.Add(num % 10);
            num = num / 10;
        }
        listOfInts.Reverse();


        Console.WriteLine(num);
        foreach (var item in listOfInts)
        {
            Console.WriteLine(item);
        }



        Console.ReadLine();

        return;


        Thread th = new Thread(() => Func1(1));
        th.Name = "_th " + "Func1_1";
        th.IsBackground = true;
        th.Start();

        Thread.Sleep(1200);
        Console.WriteLine("");

        th = new Thread(() => Func1(2));
        th.Name = "_th " + "Func1_2";
        th.IsBackground = true;
        th.Start();

        Thread.Sleep(1200);
        Console.WriteLine("");

        Console.WriteLine("press enter for release one thread!");
        Console.ReadLine();
        CIMEvent.Set();

        Thread.Sleep(1200);
        Console.WriteLine("");

        Console.WriteLine("press enter for release another thread!");
        Console.ReadLine();
        CIMEvent.Set();

        Thread.Sleep(1200);
        Console.WriteLine("");

        Console.WriteLine("press enter for exit!");
        Console.ReadLine();
    }

    struct WaitOneDefiniton
    {
        public static int MaxWait = 50;
        public static TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(-1);
    }

    static System.Threading.AutoResetEvent CIMEvent = new System.Threading.AutoResetEvent(false);

    static void Func1(int counter)
    {
        Console.WriteLine("thread ({0}({1})) entered", "Func1", counter);

        CIMEvent.Reset();
        //CIMEvent.WaitOne();
        WaitOne((WaitHandle)CIMEvent, -1);

        Console.WriteLine("thread ({0}({1})) released", "Func1", counter);
    }

    public static bool WaitOne(WaitHandle handle, int timeoutInMs)
    {
        string strDatetimeFormat = "HH:mm:ss.fff";

        Console.WriteLine("Wait Entry");
        Console.WriteLine("Wait Timeout : {0} ", timeoutInMs);
        Console.WriteLine("Wait Start : {0}", DateTime.Now.ToString(strDatetimeFormat));

        TimeSpan timeout = new TimeSpan(0, 0, 0, 0, timeoutInMs);

        int expireTicks = 0, waitTime = 0; ;
        bool signaled = false, exitLoop = false;

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
                signaled = true;
            }
            else
            {
                if (Application.MessageLoop)
                {
                    Application.DoEvents();
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }
        while (!exitLoop);

        #endregion

        Console.WriteLine("Wait End : {0}", DateTime.Now.ToString(strDatetimeFormat));
        Console.WriteLine("Wait Return Object : Signaled ({0})", signaled.ToString());

        Console.WriteLine("Wait Exit");
        return signaled;
    }
}

#region Acceptable Nots
/*

    static void Main()
    {
        List<Xfs30Api.BankNoteDetail>  noteTypesDict = new List<Xfs30Api.BankNoteDetail>();
        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 1, cCurrencyID = "YTL", ulValues = 1, usNoteID = 1234, usRelease = 0 });
        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 0, cCurrencyID = "TRY", ulValues = 2, usNoteID = 1234, usRelease = 0 });
        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 0, cCurrencyID = "TRY", ulValues = 3, usNoteID = 1234, usRelease = 0 });
        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 1, cCurrencyID = "TRY", ulValues = 4, usNoteID = 1234, usRelease = 0 });


        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 1, cCurrencyID = "EUR", ulValues = 5, usNoteID = 1234, usRelease = 0 });
        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 0, cCurrencyID = "GBP", ulValues = 6, usNoteID = 1234, usRelease = 0 });
        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 1, cCurrencyID = "GBP", ulValues = 7, usNoteID = 1234, usRelease = 0 });
        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 1, cCurrencyID = "USD", ulValues = 8, usNoteID = 1234, usRelease = 0 });
        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 0, cCurrencyID = "USD", ulValues = 9, usNoteID = 1234, usRelease = 0 });
        noteTypesDict.Add(new Xfs30Api.BankNoteDetail() { bConfigured = 1, cCurrencyID = "USDS", ulValues = 10, usNoteID = 1234, usRelease = 0 });

        foreach (var item in noteTypesDict)
        {
            Console.WriteLine("{0} {1} {2}", item.ulValues, item.cCurrencyID, item.bConfigured);
        }
        Console.WriteLine("----------------------------------");

        noteTypesDict.RemoveAll(x =>
            (x.cCurrencyID != "TRL" &&
            x.cCurrencyID != "TRY" &&
            x.cCurrencyID != "YTL" &&
            x.cCurrencyID != "EUR" &&
            x.cCurrencyID != "USD" &&
            x.cCurrencyID != "GBP") ||
            x.bConfigured == 0);

        foreach (var item in noteTypesDict)
        {
            Console.WriteLine("{0} {1} {2}", item.ulValues, item.cCurrencyID, item.bConfigured);
        }
        Console.WriteLine("----------------------------------");

        Console.ReadLine();
    }
*/
#endregion

#region Indetified Notes Array Sorting
/*
class Program
{
    static CashInIdentifiedNotes tmpidentifiedNotes = new CashInIdentifiedNotes();
    static int counter = 0;

    static void Main()
    {
        Set(10, "GBP", 10);
        Set(1, "GBP", 1);
        Set(100, "GBP", 100);

        Set(50, "USD", 50);
        Set(20, "USD", 20);
        Set(10, "USD", 10);

        Set(5, "EUR", 5);
        Set(200, "EUR", 200);
        Set(100, "EUR", 100);


        Set(20, "TRY", 20);
        Set(5, "TRY", 5);
        //Set(50, "TRY", 50);

        foreach (var item in tmpidentifiedNotes.IdentifiedNote)
        {
            //if (item.Denom > 0)
            Console.WriteLine("{0} {1} => #{2}", item.Denom, item.Currency, item.Count);
        }

        Console.WriteLine("....Sorting......");
        Console.WriteLine("");
        Array.Sort(tmpidentifiedNotes.IdentifiedNote, delegate(IdentifiedNoteInfo note1, IdentifiedNoteInfo note2)
        {
            //"10 GBP" <> "100 GBP"  ==> "-1"

            //TRY <> EUR ==> -1
            //TRY <> USD ==> -1
            //TRY <> GBP ==> -1

            //EUR <> TRY ==> 1
            //EUR <> USD ==> -1
            //EUR <> GBP ==> -1

            //USD <> TRY ==> 1
            //USD <> EUR ==> 1
            //USD <> GBP ==> -1

            //GBP <> TRY ==> 1
            //GBP <> EUR ==> 1
            //GBP <> USD ==> 1

            int _result = note1.Denom.CompareTo(note2.Denom);

            string note1c = note1.Currency;
            string note2c = note2.Currency;

            //Console.WriteLine("NORMAL : {0} {1} <> {2} {3} = {4}", note1.Denom, note1.Currency, note2.Denom, note2.Currency, _result);
            if (note1c != note2c)
            {

                //if (note1c == "TRY" && note2c == "EUR")
                //    _result = -1;
                //else if (note1c == "TRY" && note2c == "USD")
                //    _result = -1;
                //else if (note1c == "TRY" && note2c == "GBP")
                //    _result = -1;

                //else if (note1c == "EUR" && note2c == "TRY")
                //    _result = 1;
                //else if (note1c == "EUR" && note2c == "USD")
                //    _result = -1;
                //else if (note1c == "EUR" && note2c == "GBP")
                //    _result = -1;

                //else if (note1c == "USD" && note2c == "TRY")
                //    _result = 1;
                //else if (note1c == "USD" && note2c == "EUR")
                //    _result = 1;
                //else if (note1c == "USD" && note2c == "GBP")
                //    _result = -1;

                //else if (note1c == "GBP" && note2c == "TRY")
                //    _result = 1;
                //else if (note1c == "GBP" && note2c == "EUR")
                //    _result = 1;
                //else if (note1c == "GBP" && note2c == "USD")
                //    _result = 1;

                if (note1c == "TRY" || note1c == "TRL")
                {
                    _result = -1;
                }
                else if (note1c == "EUR")
                {
                    if (note2c == "TRY" || note2c == "TRL")
                        _result = 1;
                    else
                        _result = -1;
                }
                else if (note1c == "USD")
                {
                    if (note2c == "TRY" || note2c == "TRL")
                        _result = 1;
                    else if (note2c == "EUR")
                        _result = 1;
                    else
                        _result = -1;
                }
                else if (note1c == "GBP")
                {
                    if (note2c == "TRY" || note2c == "TRL")
                        _result = 1;
                    else if (note2c == "EUR")
                        _result = 1;
                    else if (note2c == "USD")
                        _result = 1;
                    else
                        _result = -1;
                }
            }

            #region Denom 0 case
            if (note1.Denom == 0 && note2.Denom > 0)
            {
                _result = 1;
            }
            else if (note2.Denom == 0 && note1.Denom > 0)
            {
                _result = -1;
            }
            #endregion

            //Console.WriteLine("MINE : {0} {1} <> {2} {3} = {4}", note1.Denom, note1.Currency, note2.Denom, note2.Currency, _result);

            //Console.WriteLine("");
            return _result;
        });
        Console.WriteLine("....//Sorting......");

        Console.WriteLine("..........");
        foreach (var item in tmpidentifiedNotes.IdentifiedNote)
        {
            //if (item.Denom > 0)
            Console.WriteLine("{0} {1} => #{2}", item.Denom, item.Currency, item.Count);
        }

        Console.ReadLine();
    }

    static void Set(int count, string currency, int denom)
    {
        tmpidentifiedNotes.IdentifiedNote[counter].Count = count;
        tmpidentifiedNotes.IdentifiedNote[counter].Currency = currency;
        tmpidentifiedNotes.IdentifiedNote[counter].Denom = denom;
        counter++;
    }
}




///
/// <summary>
/// Provides the detailed information about note types which are identified by system.
/// </summary>
/// <history>
/// Initial Version : 06.02.2009 Tolga Çakıroğlu
/// </history>
public class CashInIdentifiedNotes
{
    ///
    /// <summary>
    /// Maximum number of note types which can be detected by system.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    protected const int MAX_IDENTIFIED_NOTE = 20;

    private int m_NumberOfIdentifiedNoteTypes = 0;
    ///
    /// <summary>
    /// Shows the number of identified note types.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    public virtual int NumberOfIdentifiedNoteTypes
    {
        get
        {
            return m_NumberOfIdentifiedNoteTypes;
        }
        set
        {
            m_NumberOfIdentifiedNoteTypes = value;
        }
    }

    private int m_NumberOfUnIdentifiedNoteCount = 0;
    ///
    /// <summary>
    /// Shows the number of unidentified note types.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    public virtual int NumberOfUnIdentifiedNoteCount
    {
        get
        {
            return m_NumberOfUnIdentifiedNoteCount;
        }
        set
        {
            m_NumberOfUnIdentifiedNoteCount = value;
        }
    }

    private IdentifiedNoteInfo[] m_IdentifiedNote;
    ///
    /// <summary>
    /// Shows the detailed information about identified notes.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    public virtual IdentifiedNoteInfo[] IdentifiedNote
    {
        get
        {
            return m_IdentifiedNote;
        }
        set
        {
            m_IdentifiedNote = value;
        }
    }

    private int m_MaxEscrowCapacity;
    ///
    /// <summary>
    /// Maximum number of the notes that can be stored in the escrow unit.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    public int MaxEscrowCapacity
    {
        get
        {
            return m_MaxEscrowCapacity;
        }
        set
        {
            m_MaxEscrowCapacity = value;
        }
    }

    private int m_CurrentEscrowCapacity;
    ///
    /// <summary>
    /// Current note capacity of the escrow unit.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    public int CurrentEscrowCapacity
    {
        get
        {
            return m_CurrentEscrowCapacity;
        }
        set
        {
            m_CurrentEscrowCapacity = value;
        }
    }

    private string m_VendorData;
    ///
    /// <summary>
    /// Vendor dependent data about identified notes.
    /// </summary>
    /// <history>
    /// Initial Version : 17.10.2012 Bayram Özgür
    /// </history>
    public string VendorData
    {
        get
        {
            return m_VendorData;
        }
        set
        {
            m_VendorData = value;
        }
    }

    ///
    /// <summary>
    /// Contructors of the CashInIdentifiedNotes. 
    /// It is creates and initialise IdentifiedNote array with size of <see cref="DeviceInterface.CashInIdentifiedNotes.MAX_IDENTIFIED_NOTE">MAX_IDENTIFIED_NOTE</see>.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    public CashInIdentifiedNotes()
    {
        NumberOfIdentifiedNoteTypes = 0;
        IdentifiedNote = new IdentifiedNoteInfo[MAX_IDENTIFIED_NOTE];
        for (int i = 0; i < MAX_IDENTIFIED_NOTE; i++)
        {
            IdentifiedNote[i] = new IdentifiedNoteInfo();
        }
    }
}
///
/// <summary>
/// Provides detailed information about one identified note type.
/// </summary>
/// <history>
/// Initial Version : 06.02.2009 Tolga Çakıroğlu
/// </history>
public class IdentifiedNoteInfo
{
    private string m_Currency = "";
    ///
    /// <summary>
    /// Currency name of the identified note type.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    public string Currency
    {
        get
        {
            return m_Currency;
        }
        set
        {
            m_Currency = value;
        }
    }

    private int m_Denom = 0;
    ///
    /// <summary>
    /// Denomination name of the identified note type.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    public int Denom
    {
        get
        {
            return m_Denom;
        }
        set
        {
            m_Denom = value;
        }
    }

    private int m_Count = 0;
    ///
    /// <summary>
    /// Identified note count of the note type.
    /// </summary>
    /// <history>
    /// Initial Version : 06.02.2009 Tolga Çakıroğlu
    /// </history>
    public int Count
    {
        get
        {
            return m_Count;
        }
        set
        {
            m_Count = value;
        }
    }
}
*/
#endregion

#region WaitOne Test
/*
static void Main()
    {
        Thread th = new Thread(() => Func1(1));
        th.Name = "_th " + "Func1_1";
        th.IsBackground = true;
        th.Start();

        Thread.Sleep(1200);
        Console.WriteLine("");

        th = new Thread(() => Func1(2));
        th.Name = "_th " + "Func1_2";
        th.IsBackground = true;
        th.Start();

        Thread.Sleep(1200);
        Console.WriteLine("");

        Console.WriteLine("press enter for release one thread!");
        Console.ReadLine();
        CIMEvent.Set();

        Thread.Sleep(1200);
        Console.WriteLine("");

        Console.WriteLine("press enter for release another thread!");
        Console.ReadLine();
        CIMEvent.Set();

        Thread.Sleep(1200);
        Console.WriteLine("");

        Console.WriteLine("press enter for exit!");
        Console.ReadLine();
    }

    struct WaitOneDefiniton
    {
        public static int MaxWait = 50;
        public static TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(-1);
    }

    static System.Threading.AutoResetEvent CIMEvent = new System.Threading.AutoResetEvent(false);

    static void Func1(int counter)
    {
        Console.WriteLine("thread ({0}({1})) entered", "Func1", counter);

        //CIMEvent.WaitOne();
        WaitOne((WaitHandle)CIMEvent, -1);

        Console.WriteLine("thread ({0}({1})) released", "Func1", counter);
    }

    public static bool WaitOne(WaitHandle handle, int timeoutInMs)
    {
        string strDatetimeFormat = "HH:mm:ss.fff";

        Console.WriteLine("Wait Entry");
        Console.WriteLine("Wait Timeout : {0} ", timeoutInMs);
        Console.WriteLine("Wait Start : {0}", DateTime.Now.ToString(strDatetimeFormat));

        TimeSpan timeout = new TimeSpan(0, 0, 0, 0, timeoutInMs);

        int expireTicks = 0, waitTime = 0; ;
        bool signaled = false, exitLoop = false;

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
                signaled = true;
            }
            else
            {
                if (Application.MessageLoop)
                {
                    Application.DoEvents();
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }
        while (!exitLoop);

        #endregion

        Console.WriteLine("Wait End : {0}", DateTime.Now.ToString(strDatetimeFormat));
        Console.WriteLine("Wait Return Object : Signaled ({0})", signaled.ToString());

        Console.WriteLine("Wait Exit");
        return signaled;
    }
*/
#endregion

#region Async Test
/*
class Program
{
    static AutoResetEvent InitialEvent = new AutoResetEvent(false);
    static AutoResetEvent DenominateEvent = new AutoResetEvent(false);
        
    public static void _Main()
    {
        InitialEvent.Reset();

        Thread th = new Thread(() => SubMain(1,"",true));
        th.IsBackground = false;
        th.Start();

        InitialEvent.WaitOne();
        
        Console.WriteLine(" ** E N D **");
        Console.ReadLine();
    }

    static void SubMain(int param1, string param2, bool param3)
    {
        Xfs30Api cashOut = new Xfs30Api();
        cashOut.AutoDenom = true;
        cashOut.MixNumber = 2;

        cashOut.AutoPresent = false;
        cashOut.CassetteCount = 6;
        cashOut.TellerId = 0;
        cashOut.imp.cdmEvent += new KT.WOSA.CDM.IMP.Event(CDMEvent);

        Console.WriteLine("Welcome STM Demonstration");
        Console.WriteLine("");

        Console.WriteLine("1: Status; 2. OpenRegister; 3: Denominate; 0: Close; ");

        string readStr = "initial";
        while (!String.IsNullOrEmpty(readStr))
        {
            readStr = Console.ReadLine();

            switch (readStr)
            {
                case "1":
                    #region Status
                    try
                    {
                        cashOut.WFS_INF_CDM_STATUS();
                        Console.WriteLine("WFS_INF_CDM_STATUS done!");
                    }
                    catch (KTR10_CDM.Exceptions.BaseException ex)
                    {
                        Console.WriteLine("WFS_CMD_CDM_DENOMINATE ex {0} - {1} - {2}", ex.ExceptionCode, ex.Message, ex.StackTrace);
                    }

                    #endregion
                    break;

                case "2":
                    #region OpenRegister
                    try
                    {
                        cashOut.OpenRegister();
                        Console.WriteLine("OpenRegister done!");
                    }
                    catch (KTR10_CDM.Exceptions.BaseException ex)
                    {
                        Console.WriteLine("WFS_CMD_CDM_DENOMINATE ex {0} - {1} - {2}", ex.ExceptionCode, ex.Message, ex.StackTrace);
                    }

                    #endregion
                    break;

                case "3":
                    #region Denominate
                    try
                    {
                        DenominateEvent.Reset();
                        cashOut.WFS_CMD_CDM_DENOMINATE_Async((uint)100, "TRY");

                        DenominateEvent.WaitOne(15000);

                        Console.WriteLine("WFS_CMD_CDM_DENOMINATE done!");
                    }
                    catch (KTR10_CDM.Exceptions.BaseException ex)
                    {
                        Console.WriteLine("WFS_CMD_CDM_DENOMINATE ex {0} - {1} - {2}", ex.ExceptionCode, ex.Message, ex.StackTrace);
                    }

                    #endregion
                    break;


                case "0":
                    #region Close
                    try
                    {
                        cashOut.Close();
                        Console.WriteLine("Close done!");
                    }
                    catch (KTR10_CDM.Exceptions.BaseException ex)
                    {
                        Console.WriteLine("WFS_CMD_CDM_DENOMINATE ex {0} - {1} - {2}", ex.ExceptionCode, ex.Message, ex.StackTrace);
                    }

                    #endregion
                    break;


                default:
                    readStr = "";
                    break;
            }
        }

        InitialEvent.Set();
    }

    #region Events

    static int CDMEvent(long eventId, XfsGlobalDefine.WFSRESULT WFSResult)
    {
        string s = "Event (" + eventId + "):\r\n";
        
        DenominateEvent.Set();

        Console.WriteLine(s);

        return 0;
    }

    #endregion





    private static AutoResetEvent event_1 = new AutoResetEvent(true);
    private static AutoResetEvent event_2 = new AutoResetEvent(false);

    static void Main()
    {
        Console.WriteLine("Press Enter to create three threads and start them.\r\n" +
                          "The threads wait on AutoResetEvent #1, which was created\r\n" +
                          "in the signaled state, so the first thread is released.\r\n" +
                          "This puts AutoResetEvent #1 into the unsignaled state.");
        Console.ReadLine();

        for (int i = 1; i < 4; i++)
        {
            Thread t = new Thread(ThreadProc);
            t.Name = "Thread_" + i;
            t.Start();
        }
        Thread.Sleep(250);

        for (int i = 0; i < 2; i++)
        {
            Console.WriteLine("Press Enter to release another thread.");
            Console.ReadLine();
            event_1.Set();
            Thread.Sleep(250);
        }

        Console.WriteLine("\r\nAll threads are now waiting on AutoResetEvent #2.");
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine("Press Enter to release a thread.");
            Console.ReadLine();
            event_2.Set();
            Thread.Sleep(250);
        }

        // Visual Studio: Uncomment the following line.
        //Console.Readline();
    }

    static void ThreadProc()
    {
        string name = Thread.CurrentThread.Name;

        Console.WriteLine("{0} waits on AutoResetEvent #1.", name);
        event_1.WaitOne();
        Console.WriteLine("{0} is released from AutoResetEvent #1.", name);

        Console.WriteLine("{0} waits on AutoResetEvent #2.", name);
        event_2.WaitOne();
        Console.WriteLine("{0} is released from AutoResetEvent #2.", name);

        Console.WriteLine("{0} ends.", name);
    }
}

*/

#endregion