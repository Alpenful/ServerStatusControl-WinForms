using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVZ_SSC_FW
{
    public class SettingApplication
    {
        public Main main;

        private int settingSize = 19;

        public bool startWithWindows = false; //Запускать при старте винды

        public bool startInTray = false; //Запускать в трее

        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true); //Место для записи информации об автозапуске в винде

        public string computersPath = Application.StartupPath;

        public string reportPath = Application.StartupPath + @"\report";

        public string logPath = Application.StartupPath + @"\logs";

        public string serverLogPath = @"O:\AVZ ServerStatusControl\logs";

        public int hourPeriod = 0;
        public int minutePeriod = 1;
        public int secondPeriod = 0;

        public int timeRefresh = 60000;

        public string startView = "LargeIcon";

        public int sizeX = 608;
        public int sizeY = 539;

        public int positionX = 0;
        public int positionY = 0;

        public int column1size = 139;
        public int column2size = 126;
        public int column3size = 232;
        public int column4size = 76;


        public SettingApplication()
        {
            /////////////////////////////////////////////////////////Работаем с файлом настроек при запуске/////////////////////////////////////////////////////////
            //Проверяем, есть ли такой файл
            if (File.Exists(Application.StartupPath + @"\settings_0.8b.ini"))  /////// И если есть, то
            {
                try
                {
                    string[] readSetting = new string[settingSize];
                    string[] applySettings = new string[settingSize];

                    using (StreamReader sr = new StreamReader(Application.StartupPath + @"\settings_0.8b.ini", System.Text.Encoding.Default)) //Читаем первоначально файл настроек
                    {
                        int check = 0;
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            readSetting[check] = line;
                            check++;
                        }
                        sr.Close(); //и закрываем ридер
                    }

                    for (int i = 0; i < settingSize; i++) //Разбиваем массив строк с настройками на переменные и применяем их
                    {
                        applySettings = readSetting[i].Split('=');
                        switch (applySettings[0])
                        {
                            //case "startWithWindows":
                            //    startWithWindows = bool.Parse(applySettings[1]);
                            //    break;
                            case "startInTray":
                                startInTray = bool.Parse(applySettings[1]);
                                break;
                            case "computersPath":
                                computersPath = applySettings[1].ToString();
                                break;
                            case "reportPath":
                                reportPath = applySettings[1].ToString();
                                break;
                            case "hourPeriod":
                                hourPeriod = int.Parse(applySettings[1]);
                                break;
                            case "minutePeriod":
                                minutePeriod = int.Parse(applySettings[1]);
                                break;
                            case "secondPeriod":
                                secondPeriod = int.Parse(applySettings[1]);
                                break;
                            case "timeRefresh":
                                timeRefresh = int.Parse(applySettings[1]);
                                break;
                            case "startView":
                                startView = applySettings[1];
                                break;
                            case "sizeX":
                                sizeX = int.Parse(applySettings[1]);
                                break;
                            case "sizeY":
                                sizeY = int.Parse(applySettings[1]);
                                break;
                            case "column1size":
                                column1size = int.Parse(applySettings[1]);
                                break;
                            case "column2size":
                                column2size = int.Parse(applySettings[1]);
                                break;
                            case "column3size":
                                column3size = int.Parse(applySettings[1]);
                                break;
                            case "column4size":
                                column4size = int.Parse(applySettings[1]);
                                break;
                            case "logPath":
                                logPath = applySettings[1];
                                break;
                            case "positionX":
                                positionX = int.Parse(applySettings[1]);
                                break;
                            case "positionY":
                                positionY = int.Parse(applySettings[1]);
                                break;
                            case "serverLogPath":
                                serverLogPath = applySettings[1];
                                break;
                        }
                    }
                    if (rkApp.GetValue("AVZSSC") == null)
                    {
                        startWithWindows = false;
                    }
                    else
                    {
                        startWithWindows = true;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Возникла ошибка чтения файла настроек! {ex.Message}", "Ошибка сохранения настроек");
                }
            }
            else //Если же нет, то создаем с базовыми настройками.
            {
                try
                {                 
                    OnWriteSettings();
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Возникла ошибка создания файла настроек! {ex.Message}", "Ошибка сохранения настроек"); 
                }
            }
            FolderExist();
        }

        public void FolderExist()
        {
            /////////////////////////////////////////////////////////Стандартные папки для логов и отчетов///////////////////////////////////////////////////////////
            ///поверяем папку по умолчанию
            //Отчеты
            if (!Directory.Exists(reportPath))
            {
                Directory.CreateDirectory(reportPath);
            }
            //Логи
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }

        ///////////////////////////////////////////////////////////////////////   Создаем массив для записи в файл настроек   ////////////////////////////////////////////////

        public void OnWriteSettings()
        {
            try
            {
                string[] write = new string[settingSize];
                for (int i = 0; i < settingSize; i++) //Создаем массив из текущих настроек
                {                   
                    switch (i)
                    {
                        case 0:
                            write[i] = "startWithWindows=" + startWithWindows;
                            break;
                        case 1:
                            write[i] = "startInTray=" + startInTray;
                            break;
                        case 2:
                            write[i] = "computersPath=" + computersPath;
                            break;
                        case 3:
                            write[i] = "reportPath=" + reportPath;
                            break;
                        case 4:
                            write[i] = "hourPeriod=" + hourPeriod;
                            break;
                        case 5:
                            write[i] = "minutePeriod=" + minutePeriod;
                            break;
                        case 6:
                            write[i] = "secondPeriod=" + secondPeriod;
                            break;
                        case 7:
                            write[i] = "timeRefresh=" + timeRefresh;
                            break;
                        case 8:
                            write[i] = "startView=" + startView;
                            break;
                        case 9:
                            write[i] = "sizeX=" + sizeX;
                            break;
                        case 10:
                            write[i] = "sizeY=" + sizeY;
                            break;
                        case 11:
                            write[i] = "column1size=" + column1size;
                            break;
                        case 12:
                            write[i] = "column2size=" + column2size;
                            break;
                        case 13:
                            write[i] = "column3size=" + column3size;
                            break;
                        case 14:
                            write[i] = "column4size=" + column4size;
                            break;
                        case 15:
                            write[i] = "logPath=" + logPath;
                            break;
                        case 16:
                            write[i] = "positionX=" + positionX;
                            break;
                        case 17:
                            write[i] = "positionY=" + positionY;
                            break;
                        case 18:
                            write[i] = "serverLogPath=" + serverLogPath;
                            break;
                    }
                }
                File.WriteAllLines(Application.StartupPath + @"\settings_0.8b.ini", write);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Возникла ошибка записи в файл настроек! {ex.Message}", "Ошибка сохранения настроек");
            }
        }


        //////////////////////////////////////////////////////////////////////   Применяем новые настройки   ////////////////////////////////////////////////////////////////
        
        public void ApplySettings(bool auto, bool tray, string comPath, string repPath, string lPath, string slPath, int hour, int minute, int seconds)
        {
            // Работаем с реестром для автозапуска
            if (!startWithWindows && auto)
            {
                startWithWindows = auto;
                rkApp.SetValue("AVZSSC", Application.ExecutablePath);
            }
            if(startWithWindows && !auto)
            {
                startWithWindows = auto;
                rkApp.DeleteValue("AVZSSC", false);
            }
            
            startInTray = tray;
            computersPath = comPath;
            reportPath = repPath;
            hourPeriod = hour;
            minutePeriod = minute;
            secondPeriod = seconds;
            timeRefresh = (hour * 3600000) + (minute * 60000) + (seconds * 1000);
            logPath = lPath;
            serverLogPath = slPath;
            File.Delete(Application.StartupPath + @"\settings_0.8b.ini");
            OnWriteSettings();
        }

        /////////////////////////////////////////////////////////////////////   Получаем сохраненный список компьютеров   //////////////////////////////////////////////////
        
       

    }

    

}
