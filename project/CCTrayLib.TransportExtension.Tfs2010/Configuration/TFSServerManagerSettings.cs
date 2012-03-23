/*
 * TFSServerManagerSettings.cs
 *
 * Created by <ben@virtual-olympus.com>.
 *
 * Copyright (c) 2010 Benjamin Gavin
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 *
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution.
 *
 * Neither the name of the project's author nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 */

using System;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace CCTray.TransportExtension.Tfs2010.Configuration
{
    [Serializable]
    public enum TfsServerVersion { NotSet,Tfs2010 }

    [XmlRoot(ElementName = "TFSServerManagerSettings")]
    public class TFSServerManagerSettings
    {
        [XmlElement(ElementName = "Version")]
        public TfsServerVersion ServerVersion { get; set; }

        [XmlElement(ElementName = "Url")]
        public string ServerUrl { get; set; }

        [XmlElement(ElementName = "Project")]
        public string TeamProject { get; set; }

        public TFSServerManagerSettings()
        {
            // NOTE: only provide support for 2010.
            ServerVersion = TfsServerVersion.Tfs2010;
        }

        public static TFSServerManagerSettings GetSettings(string settingsString)
        {
            if (String.IsNullOrEmpty(settingsString))
            {
                return new TFSServerManagerSettings();
            }
            else
            {
                XmlSerializer ser = new XmlSerializer(typeof(TFSServerManagerSettings));
                using (StringReader rdr = new StringReader(settingsString))
                {
                    return ser.Deserialize(rdr) as TFSServerManagerSettings;
                }
            }
        }

        public override string ToString()
        {
            XmlSerializer ser = new XmlSerializer(typeof(TFSServerManagerSettings));
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                ser.Serialize(writer, this);
            }

            return sb.ToString();
        }
    }
}
