using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Configuration;

namespace SoundControlServer
{
    public class Program
    {
        private static MMDevice device;
        private static List<VolumeModule> volumeModules;
        private static bool newCycle;

        private static void Main(string[] args)
        {
            volumeModules = new List<VolumeModule>();

            while (true)
            {
                MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
                device = DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

                var sessions = device.AudioSessionManager.Sessions;
                if (sessions == null) return;

                checkModule(new VolumeModule(device));

                newCycle = true;

                for (int i = 0; i < sessions.Count; i++)
                {
                    AudioSessionControl session = sessions[i];

                    if (session != null)
                    {
                        VolumeModule module = new VolumeModule(session);

                        if (!string.IsNullOrEmpty(module.Title))
                        {
                            checkModule(module);
                            newCycle = false;
                        }
                    }
                }

                Console.CursorVisible = false;
                Thread.Sleep(Convert.ToInt16(ConfigurationManager.AppSettings.Get("RefreshRate")));
            }
        }

        private static void checkModule(VolumeModule module)
        {
            if (newCycle)
            {
                for (int i = volumeModules.Count - 1; i > 0; i--)
                {
                    if (volumeModules[i].checkedInCycle)
                    {
                        volumeModules[i].checkedInCycle = false;
                    }
                    else
                    {
                        Serial.Write(new VolumeModule() { PID = volumeModules[i].PID, Type = "Delete" });
                        volumeModules.Remove(volumeModules[i]);
                    }
                }
            }

            VolumeModule existingModule = volumeModules.FirstOrDefault(o => o.PID == module.PID);            

            if (existingModule != null)
            {
                existingModule.checkedInCycle = true;

                if (existingModule.Title != module.Title)
                {
                    Serial.Write(new VolumeModule() { PID = module.PID, Title = module.Title, Type = "Change" });
                    existingModule.Title = module.Title;
                }

                if (existingModule.Volume != module.Volume)
                {
                    Serial.Write(new VolumeModule() { PID = module.PID, Volume = module.Volume, Type = "Change" });
                    existingModule.Volume = module.Volume;
                }

                if (existingModule.IsMuted != module.IsMuted)
                {
                    Serial.Write(new VolumeModule() { PID = module.PID, IsMuted = module.IsMuted, Type = "Change" });
                    existingModule.IsMuted = module.IsMuted;
                }
            }
            else
            {
                volumeModules.Add(module);
                Serial.Write(module);
            }
        }
    }
}
