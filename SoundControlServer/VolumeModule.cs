using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Media;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Configuration;
using Newtonsoft.Json;

namespace SoundControlServer
{
    public class VolumeModule
    {
        [JsonIgnore]
        public bool checkedInCycle { get; set; } = true;
        public string Type { get; set; } = "New";

        public int PID { get; set; }

        public string Title { get; set; }

        public int? Volume { get; set; }

        public bool? IsMuted { get; set; }

        public VolumeModule()
        { }

        public VolumeModule(MMDevice device)
        {
            PID = -1;
            Title = "Master";
            Volume = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            IsMuted = device.AudioEndpointVolume.Mute;
        }

        public VolumeModule(AudioSessionControl session)
        {
            if (session.IsSystemSoundsSession)
            {
                PID = 0;
                Title = "Systemsounds";
            }
            else
            {
                PID = (int)session.GetProcessID;
                int maxTitleLength = Convert.ToInt16(ConfigurationManager.AppSettings.Get("MaxTitleLength"));
                string processTitle = Process.GetProcessById(PID).MainWindowTitle;

                Title = processTitle.Length > maxTitleLength ? processTitle.Substring(0, maxTitleLength) : processTitle;
            }

            Volume = (int)(session.SimpleAudioVolume.Volume * 100);
            IsMuted = session.SimpleAudioVolume.Mute;
        }
    }
}
