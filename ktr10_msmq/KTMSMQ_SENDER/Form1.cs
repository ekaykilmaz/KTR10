using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dgiworks.Kiosk.Messages;
using System.Threading;
using KT.WOSA.CIM.IMP;
using KT.WOSA.CIM;

namespace KTMSMQ_SENDER
{
    public partial class Form1 : Form
    {

        bool th_read__msmq;
        private int receiveTimeout; 
        XfsCimImp imp = new XfsCimImp();


        public Form1()
        {
            InitializeComponent();
        }


   

        public void GetPrivateQueues()
        {
            // Get a list of queues with the specified category.
            MessageQueue[] QueueList =
                MessageQueue.GetPrivateQueuesByMachine(".");
            comboBox_Read_Qname.Items.Clear();
            comboBox_Write_Qname.Items.Clear();
            // Display the paths of the queues in the list.
            foreach (MessageQueue queueItem in QueueList)
            {
                 
                comboBox_Read_Qname.Items.Add(".\\" + queueItem.Label);
                comboBox_Write_Qname.Items.Add(".\\" + queueItem.Label);
                comboBox_Read_Qname.SelectedIndex = 0;
                comboBox_Write_Qname.SelectedIndex = 0; 
                Console.WriteLine(queueItem.Path);
            }

            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                GetPrivateQueues();
 

            }
            catch (Exception)
            {
            }

        }

        



        private bool send_msmq_message(Ktr10 KTR_Message)
        {

            String error = string.Empty;
            bool returnvalue = false;

            try
            {
                #region MyRegion



                MessageQueue queue = MSMQ.createMSMQ( comboBox_Read_Qname.SelectedItem.ToString(), out error);
                if (queue != null)
                {

                    KTR_Message.DATE = DateTime.Now.ToString();
                    KTR_Message.ID = RandomString(8);
                    System.Messaging.Message m = new System.Messaging.Message(KTR_Message);
                    m.Label = tbLabel.Text;

                    MessageQueueTransaction tr = new MessageQueueTransaction();
                    tr.Begin();
                    try
                    {
                        queue.Send(m, tr);
                        tr.Commit();
                        returnvalue = true;
                    }
                    catch
                    {
                        returnvalue = false;
                        tr.Abort();
                    }

                    queue.Dispose();


                }
                else
                {
                    textBox_read_log.AppendText(error + Environment.NewLine);
                }

                #endregion

            }
            catch (Exception)
            {


            }

            return returnvalue;
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }




        private void Register()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMOpenRegister;
                KTR_Message.DATE = DateTime.Now.ToString();
               

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    tbTrace.AppendText(DateTime.Now.ToString("dd/mm/yyyy hh:mm:ss") + "-->Sending Register XML Message Command" + Environment.NewLine);
                }
                else
                {
                    // write msmq fail
                    tbTrace.AppendText(DateTime.Now.ToString("dd/mm/yyyy hh:mm:ss") + "-->XML Message Fail Send Command" + Environment.NewLine);
                }
            }


        }


        private void CIMStatus()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMStatusRegister;
                KTR_Message.DATE = DateTime.Now.ToString();
           

                if (send_msmq_message(KTR_Message))
                {
                    // send ok

                    Trace(KTR_Message.OPERATION_CODE.ToString());

                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }


        }

        private void CIMClose()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ( comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMCloseRegister;
                KTR_Message.DATE = DateTime.Now.ToString();
               



                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());


                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }


        }

        private void ShutterOpen()

        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMShutterOpen;
                KTR_Message.DATE = DateTime.Now.ToString();
               
                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }


        }

        private void ShutterClose()

        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMShutterClose;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }


        }

        private void ShutterStatus()

        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMShutterStatus;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }


        }

        private void CashINStart()

        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMCashInStart;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }


        }
        private void CashIN()

        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMCashIn;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }


        }

        private void CashINEnd()

        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMCashInEnd;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }


        }

        private void CashINRollback()

        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMCashInRollBack;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }


        }

        private void radioButton1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {

                if (radioButton1.Checked) Register();

            }
            catch (Exception ex)
            {
                tbTrace.AppendText(ex.Message.ToString() + Environment.NewLine);



            }
        }

        private void radioButton_CIMClose_Click(object sender, EventArgs e)
        {
            if (radioButton_CIMClose.Checked) CIMClose();

        }

        private void radioButton_CIMstatus_Click(object sender, EventArgs e)
        {
            if (radioButton_CIMstatus.Checked) CIMStatus();
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked)
            {
                StartOperation();
                tbTrace.AppendText(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "KTR MSMQ Service Started.." + Environment.NewLine);


                try
                {
                    Thread.Sleep(3000);
                    textBox_picsavefile.Text = InnerOperation.XFSPicSavePath;
                    textBox_inisavefile.Text = InnerOperation.XFSIniSavePath;

                    if (InnerOperation.IsCreateDATFile == 1) checkBox_IsCreateDATFile.Checked = true;
                    if (InnerOperation.IsCreateFSNFile == 1) checkBox_IsCreateFSNFile.Checked = true;
                    if (InnerOperation.IsCreateJPGFile == 1) checkBox_IsCreateJPGFile.Checked = true;
                    if (InnerOperation.IsCreateOCRFile == 1) checkBox_IsCreateOCRFile.Checked = true;
                    if (InnerOperation.IsCreateTXTFile == 1) checkBox_IsCreateTXTFile.Checked = true;
                }
                catch (Exception)
                {

                   
                }
               

            }

        }

        private void StopOperation()
        {
            if (operation != null)
            {
                operation.Stop();
                operation = null;

                comboBox_Read_Qname.Enabled = true;
                comboBox_Write_Qname.Enabled = true;
                tbLabel.Enabled = true;

                th_read__msmq = false;

            }
        }

        private InnerOperation operation = null;
        private void StartOperation()
        {

            comboBox_Read_Qname.Enabled = false;
            comboBox_Write_Qname.Enabled = false;
            tbLabel.Enabled = false;

            operation = new InnerOperation();
            operation.Start();


            //th_read__msmq = true;
            //Thread listenerThread = new Thread(new ThreadStart(PEEK_MSMQ));
            //listenerThread.Start();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            // SERVİCES STOP



            if (radioButton6.Checked)
            {
                StopOperation();
                tbTrace.AppendText(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " KTR MSMQ Service Stoped.." + Environment.NewLine);

            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (tbTrace.Text.Length > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Sure", "CIM LOG CLEAR", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                    tbTrace.Text = string.Empty;
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            string queueName = comboBox_Write_Qname.SelectedItem.ToString();

            if (radioButton_write_journal.Checked) queueName = comboBox_Write_Qname.SelectedItem.ToString() + ";" + "JOURNAL";

            DialogResult dialogResult = MessageBox.Show("MSMQ PURGE SURE ? ", comboBox_Write_Qname.SelectedItem.ToString() , MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                PURGEMSMQ(queueName);
                PEEK_MSMQ(queueName, textBox_writelog);
            }




        }


    
        private void PURGEMSMQ(string msmqname)
        {
            string error = string.Empty;
            //write msmq purge
            if (msmqname != null)
            {
                //do something
                MSMQ.MSMQPURGE(msmqname.ToString(), out error);

                if (error != string.Empty)
                {
                    tbTrace.AppendText(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + error.ToString() + Environment.NewLine);
                }
                else
                {
                    tbTrace.AppendText(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + msmqname.ToString() + "-->Purge OK " + Environment.NewLine);
                }
            }

        }

        private void button23_Click(object sender, EventArgs e)
        {

            string queueName = comboBox_Read_Qname.SelectedItem.ToString();

            if (radioButton_read_journal.Checked) queueName = comboBox_Read_Qname.SelectedItem.ToString() + ";" + "JOURNAL";


            DialogResult dialogResult = MessageBox.Show("MSMQ PURGE SURE ? ", queueName, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                PURGEMSMQ(queueName);
                PEEK_MSMQ(queueName, textBox_read_log);
            }
        }


        private void PEEK_MSMQ(string msmqname , TextBox txb)
        {

            txb.Clear();
            // PEEK_MSMQ(); 
            MessageQueue mq = new MessageQueue(msmqname);
            MessageEnumerator Enum = mq.GetMessageEnumerator2();

            int i = 0;
            try
            {

            
                    while (Enum.MoveNext())
                    {
                        i++;
                        System.Messaging.Message mes = new System.Messaging.Message();
                        mes = Enum.Current;
                        mes.Formatter = new XmlMessageFormatter(new Type[] { typeof(Ktr10) });
                        Ktr10 ktrm = new Ktr10();
                        ktrm = (Ktr10)mes.Body;
                        if (ktrm.OPERATION_CODE != null)
                        {
                            txb.AppendText(i.ToString() + ":" + ktrm.DATE.ToString() + " >>>" + ktrm.OPERATION_CODE.ToString() + Environment.NewLine);
                        }
                        else
                        {
                            txb.AppendText(i.ToString() + ":"  + Enum.Current.Label.ToString() + Environment.NewLine);
                        }


                    }
            }
            catch (Exception)
            {


            }


        }

        private   void MyPeekCompleted(Object source,
            PeekCompletedEventArgs asyncResult)
        {
            // Connect to the queue.
            MessageQueue mq = (MessageQueue)source;

            // End the asynchronous peek operation.
            System.Messaging.Message m = mq.EndPeek(asyncResult.AsyncResult);

            // Display message information on the screen. 

          

            m.Formatter = new XmlMessageFormatter(new Type[] { typeof(Ktr10) });
            Ktr10 ktrms = (Ktr10)m.Body;

          
            textBox_read_log.AppendText(ktrms.DATE.ToString() + " " + ktrms.OPERATION_CODE.ToString() +  Environment.NewLine);
            // Restart the asynchronous peek operation.
          //  mq.BeginPeek();

            return;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            string queueName = comboBox_Read_Qname.SelectedItem.ToString();

            if (radioButton_read_journal.Checked) queueName = comboBox_Read_Qname.SelectedItem.ToString() + ";" + "JOURNAL";
            PEEK_MSMQ(queueName, textBox_read_log);

        }

        private void button25_Click(object sender, EventArgs e)
        {

           
            string queueName = comboBox_Write_Qname.SelectedItem.ToString();
            if (radioButton_write_journal.Checked) queueName = comboBox_Write_Qname.SelectedItem.ToString() + ";" + "JOURNAL";
           

            PEEK_MSMQ(queueName, textBox_writelog);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopOperation();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
             
        }


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
                CrossThreadUI.SetText(tbTrace ,"<Trace> exception occurred , here is the detail! " + ex.Message, true);

                //AutoClosingMessageBox.Show("<Trace> exception occurred , here is the detail! " + ex.Message, "Error", 1000);
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

        private void radioButton8_Click(object sender, EventArgs e)
        {
            // SHUTTER ON
            if (radioButton8.Checked) ShutterOpen();
            
        }

        private void radioButton7_Click(object sender, EventArgs e)
        {

            if (radioButton7.Checked) ShutterClose();
            // SHUTTER OFF
        }

        private void button16_Click(object sender, EventArgs e)
        {
          

            try
            {
                CashIN();
            }
            catch (Exception)
            {


            }

        }

        private void button15_Click(object sender, EventArgs e)
        {

            try
            {
                CashINRollback();
            }
            catch (Exception)
            {

                 
            }
           
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {

            try
            {
                CashINEnd();
            }
            catch (Exception)
            {


            }
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            try
            {
                CashINStart();
            }
            catch (Exception)
            {


            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                CIMReset();
            }
            catch (Exception)
            {


            }
        }

        private void CIMReset()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMReset;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }
        }

        private void CIMRetrack()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMRetrack;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }
        }

        private void CIMCapaBilites()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMCapaBilites;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }
        }

        private void CIMBankNoteTypes()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMBankNoteTypes;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                CashINStatus();
            }
            catch (Exception)
            {


            }
        }

        private void CashINStatus()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMCashInStatus;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }
        }
        private void CashUnitInfo()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMCashUnitInfo;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }
        }

        private void CIMCurrencyExport()
        {
            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMCurrencyExport;
                KTR_Message.DATE = DateTime.Now.ToString();

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    Trace(KTR_Message.OPERATION_CODE.ToString());
                }
                else
                {
                    // write msmq fail
                    Trace("Fail -->" + KTR_Message.OPERATION_CODE.ToString());
                }
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                CIMBankNoteTypes();
            }
            catch (Exception)
            {


            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            //RETRACK
            try
            {
                CIMRetrack();
            }
            catch (Exception)
            {


            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CIMCapaBilites();
            }
            catch (Exception)
            {


            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                CashUnitInfo();
            }
            catch (Exception)
            {


            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                CIMCurrencyExport();
            }
            catch (Exception)
            {


            }
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            // start
            if (radioButton3.Checked || !th_read__msmq)
            {
                th_read__msmq = true; 
                Thread innerMainThread = null;
                System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
                innerMainThread = new Thread(new ThreadStart(ListenQueue));
                innerMainThread.Start();
            }
           
        }

        private void ListenQueue()
        {

            string error = "";
            receiveTimeout = 1000;
            string MessageLabel = string.Empty;
            MessageQueue messageQueueForWriting = MSMQ.createMSMQ(comboBox_Write_Qname.SelectedItem.ToString(), out error);

            if (messageQueueForWriting != null)
            {


                while (th_read__msmq)
                {
 
                    try
                    {

                        Ktr10 ktrMessage;


                        if (MSMQ.Receive_msmq_message<Ktr10>(10000, out ktrMessage, out MessageLabel, messageQueueForWriting, out error))
                        {
                            tbTrace.AppendText(MessageLabel + " :" + Environment.NewLine);
                            tbTrace.AppendText(ktrMessage.RESPONSE_CODE + " :" + Environment.NewLine);
                            tbTrace.AppendText(ktrMessage.DESCRIPTION + " :" + Environment.NewLine);

                            #region CIMStatusRegister



                            if (ktrMessage.RESPONSE_CODE == Ktr10.CIMStatusRegister + Ktr10.SUCCESS ||
                                ktrMessage.RESPONSE_CODE == Ktr10.CIMStatusRegister + Ktr10.FAILED)
                            {
                                if ((label_CIMSTATUS.Text = MSMQ.GetString(ktrMessage.DESCRIPTION, "Level=", "ErrorCode")) != null)
                                {
                                    label_ErrorCode.Text = MSMQ.GetString(ktrMessage.DESCRIPTION, "ErrorCode=", "Desc=");
                                    label_CIMSTATUS.ForeColor = Color.DarkGreen;
                                }
                                else
                                {
                                    label_CIMSTATUS.Text = "CIM OFFLINE";
                                    label_CIMSTATUS.ForeColor = Color.DarkRed;
                                }






                            }

                            #endregion
                                                   
                            
                            if (MessageLabel == InnerOperation.msmqLabel)
                            {

                                #region CIMCashInStatus
                                if (ktrMessage.RESPONSE_CODE == Ktr10.CIMCashInStatus + Ktr10.SUCCESS)
                                {
                                    textBox_Note.Clear();
                                    SetBankNoteID_CashIN(ktrMessage.DESCRIPTION);
                                }
                                #endregion
                                #region CIMBankNoteTypes

                                if (ktrMessage.RESPONSE_CODE == Ktr10.CIMBankNoteTypes + Ktr10.SUCCESS)
                                {
                                    SetBankNoteTypeFromCIM(ktrMessage.DESCRIPTION);
                                }
                                else
                                {

                                    //   textBox_Note.AppendText(ktrMessage.DESCRIPTION + " :" + Environment.NewLine);
                                }

                                #endregion                                
                                #region CIMBankNoteserial

                                if (ktrMessage.RESPONSE_CODE == Ktr10.CIMGetItemsInfo + Ktr10.SUCCESS)
                                {
                                   ReadSerialsNumber(ktrMessage.DESCRIPTION);
                                }
                                else { /* error read serial number;*/ }

                                #endregion
                                #region CIMCashEnd

                                if (ktrMessage.RESPONSE_CODE == Ktr10.CIMCashInEnd + Ktr10.SUCCESS)
                                {
                                    // cash and success;
                                    textBox_Note.Clear();
                                }
                                else { /* Cash and error;*/ }

                                #endregion
                                #region CIMCashRoolback

                                if (ktrMessage.RESPONSE_CODE == Ktr10.CIMCashInRollBack + Ktr10.SUCCESS)
                                {
                                    // roolback success;
                                }
                                else { /* roolback error;*/ }

                                #endregion
                               
                            }
                            
                            else if (MessageLabel == "CIMEVENT")
                            {
                                #region CIMEVENT


                                // event trigerlar

                                if (ktrMessage.RESPONSE_CODE == Ktr10.CIMITEMSTAKEN)
                                {
                                    //ItemActivation take
                                    textBox_Note.Clear();
                                }

                                if (ktrMessage.RESPONSE_CODE == Ktr10.CIMITEMSPRESENTED)
                                {
                                    //refund
                                    ShutterOpen();
                                }

                                if (ktrMessage.RESPONSE_CODE == Ktr10.CIMITEMSINSERTED)
                                {

                                }


                                if (ktrMessage.RESPONSE_CODE == Ktr10.CASHUNITINFOCHANGED)
                                {

                                }

                                if (ktrMessage.RESPONSE_CODE == Ktr10.CIMINPUTREFUSE)
                                {

                                }

                                #endregion
                            }

                        }

                    }
                    catch (Exception)
                    {


                    }


                   
                    Thread.Sleep(100);
                    Console.WriteLine("Read msmq message...........");

                }

                Console.WriteLine("STOP Read msmq ...........");

            }

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            th_read__msmq = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
             
            /////////////


            String error = string.Empty;

            MessageQueue queue = MSMQ.createMSMQ(comboBox_Read_Qname.SelectedItem.ToString(), out error);
            if (queue != null)
            {
                Ktr10 KTR_Message = new Ktr10();
                KTR_Message.OPERATION_CODE = Ktr10.CIMGetItemsInfo;
                KTR_Message.DATE = DateTime.Now.ToString();
                

                if (send_msmq_message(KTR_Message))
                {
                    // send ok
                    tbTrace.AppendText(DateTime.Now.ToString("dd/mm/yyyy hh:mm:ss") + "-->Sending Register XML Message Command" + Environment.NewLine);
                }
                else
                {
                    // write msmq fail
                    tbTrace.AppendText(DateTime.Now.ToString("dd/mm/yyyy hh:mm:ss") + "-->XML Message Fail Send Command" + Environment.NewLine);
                }

               


                }
        }

        private SNRInfo.BANKNOTE_TYPES bnknotes;
        private SNRInfo.lppNoteNumber lppNoteNumber;
        private SNRInfo.ImageAndSerialNumber NoteSerials;

        private SNRInfo.SETBANKNOTE_TYPES BANKNOTE_TYPE;
        private void button7_Click(object sender, EventArgs e)
        {
              

            #region BanknoteType


            #region usNumOfNoteTypes
            string BanknoteType = @"usNumOfNoteTypes = 27 
                                                usNoteID = 5153
                                                cCurrencyID = TRY
                                                ulValues = 1
                                                usRelease = 2005
                                                bConfigured = 0
                                                usNoteID = 5155
                                                cCurrencyID = TRY
                                                ulValues = 5
                                                usRelease = 2005
                                                bConfigured = 0
                                                usNoteID = 5156
                                                cCurrencyID = TRY
                                                ulValues = 10
                                                usRelease = 2005
                                                bConfigured = 1
                                                usNoteID = 5157
                                                cCurrencyID = TRY
                                                ulValues = 20
                                                usRelease = 2005
                                                bConfigured = 1
                                                usNoteID = 5158
                                                cCurrencyID = TRY
                                                ulValues = 50
                                                usRelease = 2005
                                                bConfigured = 1
                                                usNoteID = 5159
                                                cCurrencyID = TRY
                                                ulValues = 100
                                                usRelease = 2005
                                                bConfigured = 1
                                                usNoteID = 5187
                                                cCurrencyID = TRY
                                                ulValues = 5
                                                usRelease = 2009
                                                bConfigured = 0
                                                usNoteID = 5188
                                                cCurrencyID = TRY
                                                ulValues = 10
                                                usRelease = 2009
                                                bConfigured = 1
                                                usNoteID = 5189
                                                cCurrencyID = TRY
                                                ulValues = 20
                                                usRelease = 2009
                                                bConfigured = 1
                                                usNoteID = 5190
                                                cCurrencyID = TRY
                                                ulValues = 50
                                                usRelease = 2009
                                                bConfigured = 1
                                                usNoteID = 5191
                                                cCurrencyID = TRY
                                                ulValues = 100
                                                usRelease = 2009
                                                bConfigured = 1
                                                usNoteID = 5192
                                                cCurrencyID = TRY
                                                ulValues = 200
                                                usRelease = 2009
                                                bConfigured = 0
                                                usNoteID = 4204
                                                cCurrencyID = IRR
                                                ulValues = 5000
                                                usRelease = 2013
                                                bConfigured = 1
                                                usNoteID = 4206
                                                cCurrencyID = IRR
                                                ulValues = 20000
                                                usRelease = 2015
                                                bConfigured = 1
                                                usNoteID = 4207
                                                cCurrencyID = IRR
                                                ulValues = 50000
                                                usRelease = 2015
                                                bConfigured = 1
                                                usNoteID = 4210
                                                cCurrencyID = IRR
                                                ulValues = 500000
                                                usRelease = 2015
                                                bConfigured = 1
                                                usNoteID = 4142
                                                cCurrencyID = IRR
                                                ulValues = 1000
                                                usRelease = 1992
                                                bConfigured = 1
                                                usNoteID = 4139
                                                cCurrencyID = IRR
                                                ulValues = 2000
                                                usRelease = 2000
                                                bConfigured = 1
                                                usNoteID = 4140
                                                cCurrencyID = IRR
                                                ulValues = 5000
                                                usRelease = 1993
                                                bConfigured = 1
                                                usNoteID = 4141
                                                cCurrencyID = IRR
                                                ulValues = 10000
                                                usRelease = 1992
                                                bConfigured = 1
                                                usNoteID = 4142
                                                cCurrencyID = IRR
                                                ulValues = 20000
                                                usRelease = 2005
                                                bConfigured = 1
                                                usNoteID = 4172
                                                cCurrencyID = IRR
                                                ulValues = 5000
                                                usRelease = 2009
                                                bConfigured = 1
                                                usNoteID = 4174
                                                cCurrencyID = IRR
                                                ulValues = 20000
                                                usRelease = 2009
                                                bConfigured = 1
                                                usNoteID = 4175
                                                cCurrencyID = IRR
                                                ulValues = 50000
                                                usRelease = 2007
                                                bConfigured = 1
                                                usNoteID = 4176
                                                cCurrencyID = IRR
                                                ulValues = 100000
                                                usRelease = 2010
                                                bConfigured = 1
                                                usNoteID = 4178
                                                cCurrencyID = IRR
                                                ulValues = 500000
                                                usRelease = 2008
                                                bConfigured = 1
                                                usNoteID = 4179
                                                cCurrencyID = IRR
                                                ulValues = 1000000
                                                usRelease = 2010
                                                bConfigured = 1";
            #endregion
             
         
            if (BanknoteType != null)
            {

                int NumOfNote =Convert.ToInt32( MSMQ.GetString(BanknoteType, "usNumOfNoteTypes = ", "usNoteID"));

                if (NumOfNote > 0)
                {
                    BANKNOTE_TYPE = new SNRInfo.SETBANKNOTE_TYPES();
                    BANKNOTE_TYPE.usNumOfNoteTypes = NumOfNote;
                    BANKNOTE_TYPE.OFBANKNOTE = new SNRInfo.BANKNOTE_TYPES[NumOfNote];                                        
                    int i = 0;
                    foreach (var myString in BanknoteType.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    {
                                    try
                                    { 
                                                  #region MyRegion

                        
                                    if (myString.IndexOf("usNumOfNoteTypes") > 0)
                                    {
                            
                                    }
                                    else if (myString.IndexOf("usNoteID") > 0)
                                    {
                                        bnknotes = new SNRInfo.BANKNOTE_TYPES(); 
                            
                                        BANKNOTE_TYPE.OFBANKNOTE[i] = bnknotes;
                                        bnknotes.UsNoteID = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));
                                        i++;
                                    }
                                    else if (myString.IndexOf("cCurrencyID") > 0)
                                    {

                                        bnknotes.cCurrencyID =  (myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));
                                    }
                                    else if (myString.IndexOf("ulValues") > 0)
                                    {
                                        bnknotes.ulValues =Convert.ToInt32 (myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));

                                    }
                                    else if (myString.IndexOf("usRelease") > 0)
                                    {

                                        bnknotes.usRelease = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));
                                    }
                                    else if (myString.IndexOf("bConfigured") > 0)
                                    {
                                        bnknotes.BConfigured = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));
                                    }

                                        #endregion

                                    }
                                    catch (Exception)
                                    {


                                    }

                    };
                }


                

            }
            #endregion

            #region cim_CASHINSTATUS


            #region MyRegion
            string cim_CASHINSTATUS = @"WFS_INF_CIM_CASH_IN_STATUS:
                                        return :0

                                            wStatus = 0

                                            usNumOfRefused = 0

                                            lpNoteNumberList:
                                                    usNumOfNoteNumbers = 4:
		                                        lppNoteNumber:
                                                    usNoteID = 5189

                                                    ulCount = 50

                                                    usNoteID = 5188

                                                    ulCount = 25

                                                    usNoteID = 5190

                                                    ulCount = 25

                                                    usNoteID = 5191

                                                    ulCount = 200



                                            lpszExtra:";




            #endregion


            if (cim_CASHINSTATUS != null)
            {
                int NumOfNote = Convert.ToInt32(MSMQ.GetString(cim_CASHINSTATUS, "usNumOfNoteNumbers =", ":"));


                if (NumOfNote > 0)
                {
                    SNRInfo.CIM_CASHINSTATUS BankNotStatus = new SNRInfo.CIM_CASHINSTATUS();
                    BankNotStatus.usNumOfNoteNumbers = NumOfNote;
                    BankNotStatus.NoteNumber = new SNRInfo.lppNoteNumber[NumOfNote];
                    int b = 0;

                    foreach (var myString in cim_CASHINSTATUS.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            #region MyRegion


                            if (myString.IndexOf("usNoteID") > 0)
                            {
                                lppNoteNumber = new SNRInfo.lppNoteNumber();

                                BankNotStatus.NoteNumber[b] = lppNoteNumber;
                                lppNoteNumber.usNoteID = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));

                                SNRInfo.NoteIDValue findValueNoteID = FindObject(lppNoteNumber.usNoteID, BANKNOTE_TYPE);
                                lppNoteNumber.Currency = findValueNoteID.Currency;
                                lppNoteNumber.Value = findValueNoteID.Value;
                                lppNoteNumber.BConfigured = findValueNoteID.BConfigured;



                                b++;
                            }
                            else if (myString.IndexOf("ulCount") > 0)
                            {
                                lppNoteNumber.ulCount = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));

                            }

                            else if (myString.IndexOf("usNumOfRefused") > 0)
                            {
                                BankNotStatus.usNumOfRefused = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));

                            }
                            else if (myString.IndexOf("wStatus") > 0)
                            {
                                BankNotStatus.wStatus = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));

                            }




                            #endregion

                        }
                        catch (Exception ex)
                        {


                        }

                    }
                }

            }

            #endregion
             
        }


        private SNRInfo.NoteIDValue FindObject(int FindNoteID, SNRInfo.SETBANKNOTE_TYPES array)
        {
            SNRInfo.NoteIDValue NoteIDValue = new SNRInfo.NoteIDValue();

            for (int i = 0; i < array.OFBANKNOTE.Length; i++)
            {
                if (FindNoteID == array.OFBANKNOTE[i].UsNoteID)
                {
                    NoteIDValue.Value = array.OFBANKNOTE[i].ulValues;
                    NoteIDValue.Currency = array.OFBANKNOTE[i].cCurrencyID;
                    NoteIDValue.usNoteID = array.OFBANKNOTE[i].UsNoteID;
                    NoteIDValue.BConfigured = array.OFBANKNOTE[i].BConfigured;
                }
            }


            return NoteIDValue;
        }

        private bool SetBankNoteTypeFromCIM(string BanknoteTypes)
        {
            bool _response = false;


            #region BanknoteType
             
            if (BanknoteTypes != null)
            { 
                int NumOfNote = Convert.ToInt32(MSMQ.GetString(BanknoteTypes, "usNumOfNoteTypes = ", "usNoteID")); 

                if (NumOfNote > 0)
                {
                    BANKNOTE_TYPE = new SNRInfo.SETBANKNOTE_TYPES();
                    BANKNOTE_TYPE.usNumOfNoteTypes = NumOfNote;
                    BANKNOTE_TYPE.OFBANKNOTE = new SNRInfo.BANKNOTE_TYPES[NumOfNote];
                    int i = 0;
                    foreach (var myString in BanknoteTypes.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            #region MyRegion


                            if (myString.IndexOf("usNumOfNoteTypes") > 0)
                            {

                            }
                            else if (myString.IndexOf("usNoteID") > 0)
                            {
                                bnknotes = new SNRInfo.BANKNOTE_TYPES();

                                BANKNOTE_TYPE.OFBANKNOTE[i] = bnknotes;
                                bnknotes.UsNoteID = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));
                                i++;
                            }
                            else if (myString.IndexOf("cCurrencyID") > 0)
                            {

                                bnknotes.cCurrencyID = (myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));
                            }
                            else if (myString.IndexOf("ulValues") > 0)
                            {
                                bnknotes.ulValues = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));

                            }
                            else if (myString.IndexOf("usRelease") > 0)
                            {

                                bnknotes.usRelease = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));
                            }
                            else if (myString.IndexOf("bConfigured") > 0)
                            {
                                bnknotes.BConfigured = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));

                                if (bnknotes.BConfigured ==1)
                                {
                                    //     textBox_Note.AppendText(bnknotes.cCurrencyID + "(" + bnknotes.ulValues +")" + bnknotes.UsNoteID + Environment.NewLine);
                                }
                            }

                            #endregion
                            _response = true;
                        }
                        catch (Exception)
                        {
                            _response = false;

                        }

                    };
                }




            }
            #endregion

            return _response;




        }

        private bool SetBankNoteID_CashIN(string CashInNotsID)
        {
            bool _returnValue = false;

            #region cim_CASHINSTATUS
             

            if (CashInNotsID != null)
            {
                int NumOfNote = Convert.ToInt32(MSMQ.GetString(CashInNotsID, "usNumOfNoteNumbers =", ":"));


                if (NumOfNote > 0)
                {
                    SNRInfo.CIM_CASHINSTATUS BankNotStatus = new SNRInfo.CIM_CASHINSTATUS();
                    BankNotStatus.usNumOfNoteNumbers = NumOfNote;
                    BankNotStatus.NoteNumber = new SNRInfo.lppNoteNumber[NumOfNote];
                    int b = 0;

                    foreach (var myString in CashInNotsID.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            #region MyRegion


                            if (myString.IndexOf("usNoteID") > 0)
                            {
                                lppNoteNumber = new SNRInfo.lppNoteNumber();

                                BankNotStatus.NoteNumber[b] = lppNoteNumber;
                                lppNoteNumber.usNoteID = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));

                              
                                SNRInfo.NoteIDValue findValueNoteID = FindObject(lppNoteNumber.usNoteID, BANKNOTE_TYPE);
                                lppNoteNumber.Currency = findValueNoteID.Currency;
                                lppNoteNumber.Value = findValueNoteID.Value;
                                lppNoteNumber.BConfigured = findValueNoteID.BConfigured;

                                 

                                b++;
                            }
                            else if (myString.IndexOf("ulCount") > 0)
                            {
                                lppNoteNumber.ulCount = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));
                                textBox_Note.AppendText(lppNoteNumber.Value + " ( " + lppNoteNumber.Currency + ") X" + lppNoteNumber.ulCount + Environment.NewLine);
                            }

                            else if (myString.IndexOf("usNumOfRefused") > 0)
                            {
                                BankNotStatus.usNumOfRefused = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));
                                textBox_Note.AppendText("BankNot Refused" + "X" + BankNotStatus.usNumOfRefused + Environment.NewLine);
                            }
                            else if (myString.IndexOf("wStatus") > 0)
                            {
                                BankNotStatus.wStatus = Convert.ToInt32(myString.Substring(myString.IndexOf("=") + 1, (myString.Length - myString.IndexOf("=") - 1)));

                            }
                             
                            #endregion

                        }
                        catch (Exception ex)
                        {


                        }

                    }
                }

            }

            #endregion


            return _returnValue;



        }

        private void ReadSerialsNumber(string NoteSerialsNumber)
        {

            #region NoteSerialsNumber
            //    NoteSerialsNumber = @"WFS_INF_CIM_GET_ITEMS_INFO: return :0
            //         usCount(2)-
            //          ItemsList[
            //          usLevel(4)-usNoteID(5189)-lpszSerialNumber(B320210418)-lpszImageFileName(C:\R10\Image\20180327\104804\B320210418_20180327104804_00001.jpg)
            //         usLevel(4)-usNoteID(5188)-lpszSerialNumber(C169123063)-lpszImageFileName(C:\R10\Image\20180327\104804\C169123063_20180327104804_00002.jpg)]";
            //            string NoteSerialsNumber = @"WFS_INF_CIM_GET_ITEMS_INFO: return :0
            //usCount(0) -
            //ItemsList[] :";


            if (NoteSerialsNumber != null)
            {

                SNRInfo.NoteSerialNumberINFO NotSerialsNumber = new SNRInfo.NoteSerialNumberINFO();
                
                int i = 0;

                foreach (var myString in NoteSerialsNumber.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {

                 if (myString.IndexOf("return ") > 0)
                    {
                        try { NotSerialsNumber.usreturn = Convert.ToInt32(myString.Substring(myString.Length - 1)); } catch (Exception) { }
                    }



                    // ıf return 0
                    foreach (var myString1 in myString.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        #region usCount


                        if (myString1.IndexOf("usCount") > 0)
                        {
                            try
                            {
                                NotSerialsNumber.usCount = Convert.ToInt32(MSMQ.GetString(myString1, "(", ")"));
                                NotSerialsNumber.NoteItemsSerialsImage = new SNRInfo.ImageAndSerialNumber[NotSerialsNumber.usCount];
                            }
                            catch (Exception) { }

                        }
                        #endregion

                        #region usLevel
                        if (myString1.IndexOf("usLevel") > 0)
                        {
                            try
                            {

                                NoteSerials = new SNRInfo.ImageAndSerialNumber();
                                NoteSerials.usLevel = Convert.ToInt32(MSMQ.GetString(myString1, "(", ")"));
                            }
                            catch (Exception ex)
                            {

                            }



                        }
                        #endregion

                        #region usNoteID
                        if (myString1.IndexOf("NoteID") > 0)
                        {
                            try { NoteSerials.usNoteID = Convert.ToInt32(MSMQ.GetString(myString1, "(", ")")); } catch (Exception) { }
                        }

                        #endregion

                        #region lpszSerialNumber
                        if (myString1.IndexOf("SerialNumber") > 0)
                        {
                            try { NoteSerials.lpszSerialNumber = (MSMQ.GetString(myString1, "(", ")")); } catch (Exception) { }
                        }

                        #endregion

                        #region lpszImageFileName
                        if (myString1.IndexOf("ImageFileName") > 0)
                        {
                            try
                            {
                                NoteSerials.lpszImageFileName = (MSMQ.GetString(myString1, "(", ")"));
                          
                                NotSerialsNumber.NoteItemsSerialsImage[i] = NoteSerials;
                                i++;
                            }
                            catch (Exception) { }
                        }

                        #endregion

                    }
                    

                }

            }

            #endregion
        }

        private void button17_Click(object sender, EventArgs e)
        {
           
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            try
            {
                ShutterStatus();
            }
            catch (Exception)
            {


            }
        }
    }
}
