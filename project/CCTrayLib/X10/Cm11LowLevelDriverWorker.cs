using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
    public partial class Cm11LowLevelDriver
    {
        /// <summary>
        /// Private classs run in separate thread to actually send commands on the wire
        /// </summary>
        private class Cm11LowLevelDriverWorker
        {
            /// <summary>
            /// Event is raised when an communication error occurrs. Message contains description of error.
            /// </summary>
            public event System.EventHandler<Cm11LowLevelDriverError> Error;

            private System.Collections.Generic.Queue<Cm11Message> queue = new System.Collections.Generic.Queue<Cm11Message>();
            private object queuePadLock = new object(); // Used to synclock queue interaction.
            private volatile bool stopWorker = false;  // Used to signal thread stop
            private SerialPort comm; //Serial Port CM11a Controller is connected to
            private CM11aHouseCode x10HouseCode; //House code to control
            private int transmissionRetries = 5; //Number of times to retry transmissions
            static readonly byte[] okBuffer = new byte[1] { 0x00 }; //Const for the OK command
            static readonly int comRetryInterval = 5000; // Milliseconds between trying to retrying to establish com communications

            private bool supressComErrors = false;    // Flag used to make sure comError events only are sent once
            private DateTime lastComTry = DateTime.Now.AddSeconds(-10); // Used to make sure we only retry ComCreation every 5 seconds

            private string comPort;
            private int baudRate;
            private Parity parity;
            private int dataBits;
            private StopBits stopBits;

            public Cm11LowLevelDriverWorker(CM11aHouseCode houseCode, string comPort, int baudRate, Parity parity, int dataBits,
                                  StopBits stopBits)
            {
                x10HouseCode = houseCode;
                // We defer creation of comPort until first communication attempt
                // until then, store settings, that way we will also be able to 
                // restore a comPort if configured ComPort is on a USB device that is
                // temporarily disconnected.
                this.comPort = comPort;
                this.baudRate = baudRate;
                this.parity = parity;
                this.dataBits = dataBits;
                this.stopBits = stopBits;
            }

            /// <summary>
            /// This method is the main method of the worker class.
            /// it polls the Queue every second and sends them to the
            /// CM11 interface.
            /// </summary>
            public void StartProcessing()
            {
                // This is the processing loop
                while (!stopWorker)
                {       
                    ProcessCommand();
                    
                    // Wait a half a second before checking the queue again.
                    System.Threading.Thread.Sleep(500);
                }

                // Close down Comms and quit thread
                this.CloseDriver();
                return;
            }

            /// <summary>
            /// Closes down communications and releases resources.
            /// </summary>
            public void CloseDriver()
            {
                if (comm != null)
                {
                    if (comm.IsOpen)
                        try
                        {
                            comm.Close();
                        }
                        catch (System.IO.IOException ex)
                        {
                            // Cannot shutwdown IO port
                            // doesnt really matter now, we are shutting down.
                        }
                    comm = null;
                }
            }

            /// <summary>
            /// Adds a message to the queue.
            /// </summary>
            /// <param name="message">Message to be added to the queue</param>
            public void AddMessage(Cm11Message message)
            {
                lock (queuePadLock)
                {
                    if (this.queue.Count < 100)
                    {
                        this.queue.Enqueue(message);
                    }
                    else
                    {
                        // We already have 100 commands waiting. 
                        // Just drop further commands. Comms are probably
                        // down. 
                        RaiseErrorEvent(new Cm11LowLevelDriverError("Communication to CM11 interface is probably down and message queue is full."));
                    }
                }
            }

            /// <summary>
            /// Call this method to stop the Worker.
            /// </summary>
            public void StopProcessing()
            {
                stopWorker = true;
            }

            /// <summary>
            /// Returns or Sets the number of communication retries.
            /// </summary>
            public int TransmissionRetries
            {
                get { return (transmissionRetries); }
                set { transmissionRetries = value; }
            }

            /// <summary>
            /// If commands are found in the queue it sends commands and removes processed commands from queue
            /// </summary>
            private void ProcessCommand()
            {
                // Check if we have any commands in the queue to process.
                if (queue.Count > 0)
                {
                    Cm11Message command = queue.Peek();
                    if (command != null)
                    {
                        // Try to send command to CM11 device
                        try
                        {
                            if (Send(command.Buffer, command.Count))
                            {
                                if (SendOk())
                                {
                                    lock (queuePadLock)
                                    {
                                        queue.Dequeue();
                                    }
                                }
                            }
                        }
                        catch (System.TimeoutException)
                        {
                            RaiseErrorEvent(new Cm11LowLevelDriverError("Sending command to CM11 interace failed. Is the CM11 interface still plugged in to the AC outlet?"));
                        }
                    }
                }
            }
            
            /// <summary>
            /// Checks if ComPort is created, if not it will try to create one.
            /// </summary>
            private bool ComPortCreated()
            {
                if (comm == null && DateTime.Now.Subtract(lastComTry).TotalMilliseconds> comRetryInterval)
                {                    
                    lastComTry = DateTime.Now;
                    try
                    {
                        comm = new SerialPort(comPort, baudRate, parity, dataBits, stopBits);
                        
                        // reset flag
                        supressComErrors = false;
                        lastComTry = DateTime.Now.AddSeconds(-10);
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        if(!supressComErrors)
                            RaiseErrorEvent(new Cm11LowLevelDriverError("Access error. " + comPort + " is probably already in use by another application. Will not send this message again."));

                        supressComErrors = true;
                        return false;
                    }
                    catch (Exception ex)
                    {
                        if(!supressComErrors)
                            RaiseErrorEvent(new Cm11LowLevelDriverError("An error occurred trying to initiate communications through " + comPort + " Message: " + ex.Message + " Will not send this message again"));

                        supressComErrors = true;
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Checks if ComPort is open, if it is not will try to open it.
            /// </summary>
            /// <returns>True if ComPort is open for communication</returns>
            private bool CommsOpen()
            {
                // Can't proceed if ComPort is not created.
                if (!ComPortCreated())
                    return false;

                if (!comm.IsOpen && DateTime.Now.Subtract(lastComTry).TotalMilliseconds > comRetryInterval)
                {
                    lastComTry = DateTime.Now;
                    try
                    {
                        // Open Communications.
                        comm.ReadTimeout = 3000;
                        comm.Open();
                        // Reset com errors flag
                        supressComErrors = false;
                    }
                    catch (System.IO.IOException ioEx)
                    {
                        if(!supressComErrors)
                            RaiseErrorEvent(new Cm11LowLevelDriverError("Could not open communications: " + ioEx.Message));

                        supressComErrors = true;
                        
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        if (!supressComErrors)
                            RaiseErrorEvent(new Cm11LowLevelDriverError("Access error. " + comPort + " is probably already in use by another application."));

                        supressComErrors = true;
                    }
                    catch (System.TimeoutException)
                    {
                        if (!supressComErrors)
                            RaiseErrorEvent(new Cm11LowLevelDriverError("Time out trying to open " + comPort));

                        supressComErrors = true;
                    }
                }

                return comm.IsOpen;
            }

            /// <summary>
            /// Sends the X10 Command to the CM11a
            /// </summary>
            /// <param name="buffer">Command to send in an array of bytes</param>
            /// <param name="count">size of command array</param>
            private bool Send(byte[] buffer, int count)
            {
                if (!CommsOpen())
                {
                    return false;
                }
                int retVal = 0;
                int x = 0;
                do
                {
                    comm.DiscardInBuffer(); //Get rid of anything in the buffer
                    comm.Write(buffer, 0, count); //send transmission to device
                    retVal = comm.ReadByte();
                    if (0x5a == retVal)
                    { //If status messages are being transmitted cmd will not go through
                        byte[] sendpollack = new byte[1];
                        sendpollack[0] = 0xc3;
                        comm.Write(sendpollack, 0, 1); //Send cmd to acknowledged status command.
                    }
                    if (0xa5 == retVal)
                    { // Detected a Time Request message from interface and this is not in response 
                        // to us sending a timer download header message.
                        byte[] timerDownload = TimerDownloadMessage(DateTime.Now);
                        comm.Write(timerDownload, 0, timerDownload.Length);
                        int ignoreNum = comm.ReadByte();  // Ignore checksum
                        comm.Write(okBuffer, 0, 1);
                    }
                    x++;
                } while ((Checksum(buffer, count) != retVal) && this.TransmissionRetries > x);
                //Try to resend transmission of it did not go through

                //If too many retried raise error event
                if (this.TransmissionRetries <= x)
                {
                    RaiseErrorEvent(new Cm11LowLevelDriverError("Failed to communicate with X10 controller device"));
                    return false;
                }

                // Message sent!
                return true;
            }

            /// <summary>
            /// Sends Ok to CM11 interface
            /// </summary>
            /// <returns></returns>
            private bool SendOk()
            {
                return Send(okBuffer , 1);
            }

            /// <summary>
            /// Returns a complete Timer Download Message in response to a Time Request
            /// </summary>
            private byte[] TimerDownloadMessage(DateTime date)
            {
                // If Power-fail some interfaces will ask for date time update.
                // we will have to give them a new date and time or
                // transmitted cmds will not go through

                // Snippet from: http://www.heyu.org/docs/protocol.txt
                // For a CM11, the time request is from the interface is: 0xa5.
                // The PC must then respond with the following transmission
                // Bit range	Description
                // 55 to 48	timer download header (0x9b)			        (byte 0)
                // 47 to 40	Current time (seconds)				            (byte 1)
                // 39 to 32	Current time (minutes ranging from 0 to 119)    (byte 2)
                // 31 to 24	Current time (hours/2, ranging from 0 to 11)	(byte 3)
                // 23 to 15	Current year day (MSB is bit 15)		        (byte 4+.1)
                // 14 to 8	Day mask (SMTWTFS)				                (byte 5-.1)
                // 7 to 4	Monitored house code				            (byte 6...)
                // 3		Reserved
                // 2		Timer purge flag 		
                // 1		Battery timer clear flag
                // 0		Monitored status clear flag
                // End of snippet

                int minute = date.Minute;
                int hour = date.Hour / 2;
                if (Math.IEEERemainder(date.Hour, 2) > 0)
                { // Add remaining minutes 
                    minute += 60;
                }

                int wday = Convert.ToInt16(Math.Pow(2, (int)date.DayOfWeek));
                int yearDay = date.DayOfYear - 1;
                if (yearDay > 255)
                {
                    yearDay = yearDay - 256;
                    // Set current yearDay flag in wday's 7:th bit, since yearDay overflowed...
                    wday = wday + Convert.ToInt16(Math.Pow(2, 7));
                }

                // Build message
                byte[] message = new byte[7];
                message[0] = 0x9b;   // Time download header
                message[1] = Convert.ToByte(date.Second);
                message[2] = Convert.ToByte(minute);
                message[3] = Convert.ToByte(hour);
                message[4] = Convert.ToByte(yearDay);
                message[5] = Convert.ToByte(wday);
                message[6] = Convert.ToByte(0x03 + (int)this.x10HouseCode); // Send timer purgeflag,Monitored status clear flag, monitored house code.

                return message;
            }

            /// <summary>
            /// Computes Checksum of transmission to determine if the controller received command
            /// properly
            /// </summary>
            /// <param name="buffer">Command to send in the form of a byte array.</param>
            /// <param name="count">length of command array</param>
            /// <returns></returns>
            private int Checksum(byte[] buffer, int count)
            {
                if (1 < count)
                {
                    byte iRetVal = 0;
                    foreach (byte element in buffer)
                    {
                        iRetVal += element;
                    }
                    return (iRetVal & (byte)0xFF);
                }
                else if (1 == count)
                {
                    if (0 == buffer[0])
                    {
                        return (0x55);
                    }
                }
                return (0x00);
            }

            /// <summary>
            /// Raises the Error event
            /// </summary>
            /// <param name="error"></param>
            private void RaiseErrorEvent(Cm11LowLevelDriverError error)
            {
                // If we have any subscribers, raise event.
                if (this.Error != null)
                    this.Error(this, error);

            }
        }
    }
}
