using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVZ_SSC_FW
{
    public class ViewBox
    {
        public delegate void ViewBoxControl(ListView list);

        public event ViewBoxControl Clear;

        public void ClearList(ListView list, NotifyIcon notify)
        {
            if (Clear != null)
            {
                try
                {
                    list.Clear();  //Очищаем наш текущий список
                }
                catch
                {
                    notify.BalloonTipIcon = ToolTipIcon.Warning;
                    notify.BalloonTipTitle = "Невозможно очистить коллекцию!";
                    notify.BalloonTipText = "Ох пиздеец!";
                    notify.ShowBalloonTip(1000);
                }
            }
        }
    }
}
