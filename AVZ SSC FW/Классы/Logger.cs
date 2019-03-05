using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVZ_SSC_FW
{
    public class Logger
    {
        string path = "";
        string pathFor = "";

        public Logger(SettingApplication setting)
        {
            pathFor = setting.logPath;
            path = setting.logPath + @"\" + DateTime.Now.Month + "_" + DateTime.Now.Year + ".logfile";
            //if (!File.Exists(path))
            //{
            //    File.Create(path);
            //}
        }

        public void OnWriteEvent(LogEvent item)
        {
            string ev =item.id.ToString() + "*" + item.date.ToString() + "*" + item.host + "*" + item.hostEvent;
            StreamWriter stream = new StreamWriter(path, true, Encoding.UTF8);
            stream.WriteLineAsync(ev);
            stream.Close();
        }

        public List<LogEvent> ReadLog(string host, DateTime ot, DateTime po, bool servLog, SettingApplication set)
        {
            if (servLog) pathFor = set.serverLogPath;

            //int resultYear = po.Year - ot.Year;
            int resultMonth = po.Month - ot.Month;
            //MessageBox.Show($"Лет - {resultYear}, месяцев - {resultMonth}");
            List<LogEvent> filter = new List<LogEvent>();
            try
            {
                for (int i = 0; i <= resultMonth; i++)
                {
                    string forlog = pathFor + @"\" + (DateTime.Now.Month - i).ToString() + "_" + DateTime.Now.Year + ".logfile";
                    using (StreamReader sr = new StreamReader(forlog, System.Text.Encoding.UTF8)) //Читаем первоначально файл настроек
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] vs = line.Split('*');
                            LogEvent log = new LogEvent
                            {
                                id = int.Parse(vs[0]),
                                date = DateTime.Parse(vs[1]),
                                host = vs[2],
                                hostEvent = vs[3]
                            };

                            string[] hostec = host.Split('/');
                            string[] eventHost = log.host.Split('/');

                            string hostname = "";
                            string curip = "";

                            string eventHosname = "";
                            string eventIP = "";

                            if (hostec.Length == 4)
                            {
                                hostname = hostec[2];
                                curip = hostec[3];
                            }
                            else
                            {
                                hostname = hostec[1];
                                curip = hostec[2];
                            }

                            if (eventHost.Length == 4)
                            {
                                eventHosname = eventHost[2];
                                eventIP = eventHost[3];
                            }
                            else
                            {
                                eventHosname = eventHost[1];
                                eventIP = eventHost[2];
                            }

                            if (hostname == eventHosname && curip == eventIP && log.date >= ot && log.date <= po)
                            {
                                filter.Add(log);
                            }
                        }
                        sr.Close(); //и закрываем ридер
                    }
                }
            }
            catch
            {
               
            }
            return filter;
        }

    }

    public class LogEvent
    {
        public int id;
        public DateTime date;
        public string host;
        public string hostEvent;
    } 
}
