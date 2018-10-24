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
        private static MMDevice _device;
        private static List<VolumeModule> _volumeModules;
        private static bool _newCycle;
        private static Serial _serial;

        private static void Main(string[] args)
        {
            _serial = new Serial(ConfigurationManager.AppSettings.Get("COMPort"), Convert.ToInt32(ConfigurationManager.AppSettings.Get("BaudRate")));
            _volumeModules = new List<VolumeModule>();

            try
            {
                if (_serial.Open())
                {
                    while (true)
                    {
                        MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
                        _device = DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

                        var sessions = _device.AudioSessionManager.Sessions;
                        if (sessions == null) return;

                        checkModule(new VolumeModule(_device));

                        _newCycle = true;

                        for (int i = 0; i < sessions.Count; i++)
                        {
                            AudioSessionControl session = sessions[i];

                            if (session != null)
                            {
                                VolumeModule module = new VolumeModule(session);

                                if (!string.IsNullOrEmpty(module.Title))
                                {
                                    checkModule(module);
                                    _newCycle = false;
                                }
                            }
                        }

                        Console.CursorVisible = false;
                        Thread.Sleep(Convert.ToInt16(ConfigurationManager.AppSettings.Get("RefreshRate")));
                    }
                }
                else
                {
                    Console.WriteLine("Failed to open COM port");
                }

                Console.ReadLine();
            }
            finally
            {
                _serial.Close();
                if (_device != null)
                {
                    _device.Dispose();
                }
            }
        }

        private static void checkModule(VolumeModule module)
        {
            if (_newCycle)
            {
                for (int i = _volumeModules.Count - 1; i > 0; i--)
                {
                    if (_volumeModules[i].checkedInCycle)
                    {
                        _volumeModules[i].checkedInCycle = false;
                    }
                    else
                    {
                        _serial.Write(new VolumeModule() { PID = _volumeModules[i].PID, Type = "Delete" });
                        _volumeModules.Remove(_volumeModules[i]);
                    }
                }
            }

            VolumeModule existingModule = _volumeModules.FirstOrDefault(o => o.PID == module.PID);

            if (existingModule != null)
            {
                existingModule.checkedInCycle = true;

                if (existingModule.Title != module.Title)
                {
                    _serial.Write(new VolumeModule() { PID = module.PID, Title = module.Title, Type = "Change" });
                    existingModule.Title = module.Title;
                }

                if (existingModule.Volume != module.Volume)
                {
                    _serial.Write(new VolumeModule() { PID = module.PID, Volume = module.Volume, Type = "Change" });
                    existingModule.Volume = module.Volume;
                }

                if (existingModule.IsMuted != module.IsMuted)
                {
                    _serial.Write(new VolumeModule() { PID = module.PID, IsMuted = module.IsMuted, Type = "Change" });
                    existingModule.IsMuted = module.IsMuted;
                }
            }
            else
            {
                _volumeModules.Add(module);
                _serial.Write(module);
            }
        }
    }
}
