using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AVZ_SSC_FW
{
    public class Items
    {
        public int id;
        public int status;
        public IPAddress ip;
        public string displayName = "";
        public string hostName;
        public string description = "Описание хоста";
        public bool report = false;

        public string group = "Без группы";
        //Для табличного вида
        public DateTime lastView = new DateTime();
        public float lastPing = 0f;
        public bool ignoreNotification = false;
        public bool logWrite = true;
    }
}
