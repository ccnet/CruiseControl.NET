using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
    /// <summary>
    /// Encapsulates a CM11 message.
    /// </summary>
    internal class Cm11Message
    {
        private byte[] buffer;
        private int count;

        /// <summary>
        /// Creates an empty Cm11Message
        /// </summary>
        public Cm11Message()
        {
        }

        /// <summary>
        /// Creates a Cm11Message with the supplied content
        /// </summary>
        /// <param name="buffer">Message byte array, as message should be sent to CM11 Interface</param>
        /// <param name="count">Length of message</param>
        public Cm11Message(byte[] buffer, int count)
        {
            this.Buffer = buffer;
            this.Count = count;
        }

        /// <summary>
        /// Message byte array, as message should be sent to CM11 Interface
        /// </summary>
        public byte[] Buffer
        {
            get { return buffer; }
            set 
            { 
                // Make sure we make a COPY of the buffer
                // we do not want to pass a reference to the command
                // processor.. on another thread.
                buffer = new byte[value.Length];
                for (int i = 0; i < value.Length; i++ )
                    buffer[i] = value[i];
            }
        }

        /// <summary>
        /// Length of message
        /// </summary>
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }
}
