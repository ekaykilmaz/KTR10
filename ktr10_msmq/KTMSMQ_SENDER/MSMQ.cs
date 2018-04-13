using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace KTMSMQ_SENDER
{
    public class MSMQ
    { 
        static public MessageQueue createMSMQ(string queueName, out string error)
        {
            MessageQueue mq = null;
            error = "";
            try
            {
                if (MessageQueue.Exists(queueName))
                {
                    mq = new MessageQueue(queueName);
                    if (!mq.Transactional)
                    {
                        error = "Message queue is not transactional.";
                        mq.Dispose();
                        mq = null;
                    }
                    
                }
                else
                {
                    error = "Message queue does not exist.";
                }
            }
            catch (Exception e)
            {
                error = "Message Queue Exception: " + e.Message;
                mq = null;
            }
            return mq;
        }

        static public MessageQueue MSMQPURGE(string queueName, out string error)
        {
            MessageQueue mq = null;
            error = "";
            try
            {
                if (MessageQueue.Exists(queueName))
                {
                    mq = new MessageQueue(queueName);
                    mq.Purge();
                }
                else
                {
                    error = "Message queue does not exist.";
                }
            }
            catch (Exception e)
            {
                error = "Message Queue Exception: " + e.Message;
                mq = null;
            }
            return mq;
        }

        static public bool Receive_msmq_message<T>(int timeoutInMs, out T convertedObject, out string MessageLabel, MessageQueue queue ,out string error )
        {
            convertedObject = default(T);
            error = "";
            MessageLabel = string.Empty;
            bool retValue = false;
                      

            if (queue != null)
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(timeoutInMs);

                MessageQueueTransaction tr = new MessageQueueTransaction();
                tr.Begin();
                try
                {
                    System.Messaging.Message message = queue.Receive(ts, tr);
                    message.Formatter = new XmlMessageFormatter(new Type[] { typeof(T) });

                    MessageLabel = message.Label;
                    convertedObject = (T)message.Body;
                    tr.Commit();
                    retValue = true;
                }
                catch (Exception e)
                {
                    error = e.Message;
                    tr.Abort();
                }
            }

           
            return retValue;
        }


        public static string GetString (string strSource , string strStart , string strEnd)
        {
            int start, end;

            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                start = strSource.IndexOf(strStart, 0) + strStart.Length;
                end = strSource.IndexOf(strEnd, start);

                return  strSource.Substring(start, end - start);
            }
            else
            {
                return null;
            }
             
        }

         

    }


}
