using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using System.Windows;

namespace AVZ_SSC_FW
{
    public partial class Main : Form
    {
        public  SettingApplication setting = new SettingApplication();
        public  List<Items> items = new List<Items>();  //Лист хранения 
        private List<Items> deploy = new List<Items>(); //Лист для деплоя

        private List<Items> offline = new List<Items>(); //Лист для обработки пропавших хостов
        private List<Items> online = new List<Items>(); //Лист ждя обработки появившихся хостов

        public  List<ListViewGroup> group = new List<ListViewGroup>();

        ItemComparer itemComparer = new ItemComparer();

        bool noError = true;
        bool firstStart = true;
        bool Exit = false;

        public bool refresh = true;

        public int selectedItem;

        NotifyIcon notify;

        public Logger logger;

        DateTime dateTime;

        public Main()
        {
            InitializeComponent();
            this.listView1.Items.Clear();

            notify = notifyIcon1; //Иконка по умолчанию
            timer1.Interval = setting.timeRefresh; //Задаем время рефреша
            timer1.Start(); //Запускаем таймер

            firstStart = true;

            //Таймер обновления вивера
            timer2.Interval = 5000;
            timer2.Start();

            switch (setting.startView)
            {
                case "LargeIcon":
                    listView1.Visible = true;
                    listView2.Visible = false;
                    break;
                case "Table":
                    listView1.Visible = false;
                    listView2.Visible = true;
                    break;
            }

            setting.main = this;

            this.Size = new Size(setting.sizeX, setting.sizeY);
            listView2.Columns[0].Width = setting.column1size;
            listView2.Columns[1].Width = setting.column2size;
            listView2.Columns[2].Width = setting.column3size;
            listView2.Columns[3].Width = setting.column4size;
            ;listView2.ListViewItemSorter = itemComparer;
            this.Location = new Point(setting.positionX, setting.positionY);


            //Для записи логов
            logger = new Logger(setting);
            dateTime = DateTime.Now; 

            this.FormClosing += MainForm_Closing; //Подписываемся на события
        }

        public void TimerPreset()
        {
            timer1.Stop();
            timer1.Interval = setting.timeRefresh;
            timer1.Start();
        }

        //////////////////////////////////////////////////////////////////////           Действия на кнопки в программе              ////////////////////////////////////////////////

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                //backgroundWorker1.CancelAsync();
                backgroundWorker1.RunWorkerAsync();
            }
            catch { }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            RefreshItemsList();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            RefreshViewList();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Owner = this;
            about.ShowDialog(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddNew add = new AddNew();
            add.Owner = this;
            add.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1.RunWorkerAsync();
                RefreshViewList();
            }
            catch { }
            //RefreshList();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit = true;
            Application.Exit();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Owner = this;
            settings.set = setting;
            settings.ShowDialog(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddNewGroup();
        }

        private void группуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewGroup();
        }

        private void свернутьВТрейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void крупныеЗначкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setting.startView = "LargeIcon";
            listView1.Visible = true;
            listView2.Visible = false;
            setting.OnWriteSettings();
        }

        private void таблицейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setting.startView = "Table";
            listView1.Visible = false;
            listView2.Visible = true;
            setting.OnWriteSettings();
        }


        ////////////////////////////////////////////////////////////////////      Методы событий в форме       //////////////////////////////////////////////////////////////////////


        private void Main_Load(object sender, EventArgs e)
        {
            // Создаем подсказку при наведении
            ToolTip toolTip1 = new ToolTip();

            // Задаем время появления и работы подсказки
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            // Разрешаем работать ему
            toolTip1.ShowAlways = true;

            // делаем невидимой нашу иконку в трее
            //notify.Visible = false;
            //notifyIcon1.Visible = false;
            notifyIcon2.Visible = false;

            //Проверяем, как запускается программа
            if (setting.startInTray) WindowState = FormWindowState.Minimized;
            else WindowState = FormWindowState.Normal;


            LoadList();


        }

        private void Main_Resize(object sender, EventArgs e)
        {

            // проверяем наше окно, и если оно было свернуто, делаем событие        
            if (WindowState == FormWindowState.Minimized)
            {
                // прячем наше окно из панели
                this.ShowInTaskbar = false;
                this.Hide();
                // делаем нашу иконку в трее активной
                notify.Visible = true;
            }
            else
            {
                this.Show();
                this.ShowInTaskbar = true;
                //notify.Visible = false;
            }
        }

        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            setting.sizeX = this.Size.Width;
            setting.sizeY = this.Size.Height;
            setting.OnWriteSettings();
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e) //Сворачиваем вместо закрытия
        {
            if (!Exit)
            {
                e.Cancel = true; // кнопка больше не закрывает форму
                                 // а тут теперь указываем что она делает    
                WindowState = FormWindowState.Minimized;
                //this.ShowInTaskbar = false;
                //notify.Visible = true;
            }
        }

        private void Main_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                int size = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

                setting.positionX = this.Location.X;

                if (this.Location.Y < size && this.Location.Y > 0)
                {
                    setting.positionY = this.Location.Y;
                }
                setting.OnWriteSettings();
            }
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {

        }

        /////////////////////////////////////////////////////////////////////       События по правому клику в трее     ////////////////////////////////////////////////////////////

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit = true;
            Application.Exit();
        }

        private void развернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        private void notifyIcon2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        private void закрытьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Exit = true;
            Application.Exit();
        }

        private void развернутьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        private void отметитьИзмененияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notify.Visible = false;
            notify = notifyIcon1;
            notify.Visible = true;
        }

        /////////////////////////////////////////////////////////////////////   Работа с элементами коллекции     //////////////////////////////////////////////////////////////////

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                    {
                        ItemContexMenu.Show(Cursor.Position);
                        selectedItem = listView1.SelectedIndices[0];
                        //MessageBox.Show($"{selectedItem} = {items[selectedItem].hostName}");
                    }
                    else
                    {
                        //if(listView1.)
                        ListViewMenu.Show(Cursor.Position);
                    }
                }
                catch
                {
                    ListViewMenu.Show(Cursor.Position);
                }
            }
        }

        private void AddHost_Click(object sender, EventArgs e)
        {
            AddNew add = new AddNew();
            add.Owner = this;
            add.ShowDialog(this);
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { backgroundWorker1.RunWorkerAsync(); }
            catch { }
            
        }

        private void DeleteItemList(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                $"Удалить хост {items[selectedItem].displayName} из списка?",
                "Удаление хоста",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
                );

            if (result == DialogResult.Yes)
            {
                try
                {
                    items.RemoveAt(selectedItem);
                    refresh = true;
                    RefreshViewList();
                    OnWriteItems();
                }
                catch { }
            }
        }

        private void свойстваToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Info info = new Info();
            info.Owner = this;
            info.ShowDialog(this);
        }

        private void группуToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteGroup delete = new DeleteGroup();
            delete.Owner = this;
            delete.ShowDialog(this);
        }

        private void listView2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    if (listView2.FocusedItem.Bounds.Contains(e.Location) == true)
                    {
                        ItemContexMenu.Show(Cursor.Position);
                        //selectedItem = listView2.SelectedItems[0].Index;
                        foreach (var item in items)
                        {
                            if(item.displayName == listView2.SelectedItems[0].Text)
                            {
                                selectedItem = item.id;
                            }
                        }
                        //MessageBox.Show($"{selectedItem} = {items[selectedItem].hostName}");
                    }
                    else
                    {
                        //if(listView1.)
                        ListViewMenu.Show(Cursor.Position);
                    }
                }
                catch
                {
                    ListViewMenu.Show(Cursor.Position);
                }
            }
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 0)
                return;
            else
            {
                ItemContexMenu.Show(Cursor.Position);
                //selectedItem = listView2.SelectedItems[0].Index;
                foreach (var item in items)
                {
                    if (item.displayName != "Unknown" && item.displayName != "")
                    {
                        if (item.displayName == listView2.SelectedItems[0].Text)
                        {
                            selectedItem = item.id;
                        }
                    }
                    else
                    {
                        if (item.ip.ToString() == listView2.SelectedItems[0].SubItems[1].Text)
                        {
                            selectedItem = item.id;
                        }
                    }
                }
                Info info = new Info();
                info.Owner = this;
                info.ShowDialog(this);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            else
            {
                selectedItem = listView1.SelectedIndices[0];
                Info info = new Info();
                info.Owner = this;
                info.ShowDialog(this);
            }
        }

        private void listView2_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            setting.column1size = listView2.Columns[0].Width;
            setting.column2size = listView2.Columns[1].Width;
            setting.column3size = listView2.Columns[2].Width;
            setting.column4size = listView2.Columns[3].Width;
            setting.OnWriteSettings();
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //Указываем сортируемую колонку
            itemComparer.ColumnIndex = e.Column;
            //Непосредственно инициируем сортировку
            ((ListView)sender).Sort();
        }

        /////////////////////////////////////////////////////////////////////    Логика       ///////////////////////////////////////////////////////////////////////////////////////

        private void AddNewGroup()
        {
            AddGroupNew addGroup = new AddGroupNew();
            addGroup.Owner = this;
            addGroup.ShowDialog(this);
        }

        ////////////////////////////////////////////////////////////////////     Загружаем наш лист с компьютерами из файла    //////////////////////////////////////////////////////
        private void LoadList()
        {
            try
            {
                string path = setting.computersPath + @"\data.ssc";
                if (File.Exists(path))
                {   //Считываем наши сохраненные компьютеры
                    using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8)) //Читаем первоначально файл настроек
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] read = line.Split(';');

                            if(read[0] == "group") //Проверяем наличие записей о группах
                            {
                                ListViewGroup viewGroup = new ListViewGroup
                                {
                                    Name = read[1],
                                    Header = read[1]
                                };
                                group.Add(viewGroup);
                            }

                            if (read[0] == "host") //Проверяем наличие записей о хостах
                            {
                                Items item = new Items
                                {
                                    id = int.Parse(read[1]),
                                    ip = IPAddress.Parse(read[2]),
                                    displayName = read[3],
                                    hostName = read[4],
                                    description = read[5],
                                    status = 2,
                                    group = read[6],
                                    lastView = DateTime.Parse(read[7]),
                                    ignoreNotification = bool.Parse(read[8]),
                                    logWrite = bool.Parse(read[9])
                                };
                                items.Add(item);
                            }
                        }
                        sr.Close(); //и закрываем ридер
                    }
                    if(group.Count == 0)
                    {
                        ListViewGroup viewGroup = new ListViewGroup
                        {
                            Name = "Без группы",
                            Header = "Без группы"
                        };
                        group.Add(viewGroup);
                    }
                }
                else
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                    MessageBox.Show($"Отсутcтвует файл с базой данных! Cоздан новый по пути: \n {path}", "Не найден файл");
                    ListViewGroup viewGroup = new ListViewGroup
                    {
                        Name = "Без группы",
                        Header = "Без группы"
                    };
                    group.Add(viewGroup);
                }
                RefreshViewList();
            }
            catch
            {

            }
        }

        /////////////////////////////////////////////////////////////////      Обновляем файл коллекции            //////////////////////////////////////////////////////////////////

        public void RefreshItemsList() //Обновляем нашу коллекцию с файлами
        {
            offline = new List<Items>(); //Лист для обработки пропавших хостов
            online = new List<Items>();
            
            noError = true;
            int id = 0;
            try
            {
                foreach (var item in items)
                {   
                    string name = "";
                    //Проводим проверку на доступность
                    try
                    {
                        Ping pingSender = new Ping();
                        PingReply pingReply;
                        bool access = false;

                        for (int i = 0; i < 4; i++)
                        {                        
                            if (item.hostName != "Unknown" && item.hostName != "") //Если же нам до этого обнаружить имя компьютера удалось
                            {
                                name = item.hostName;
                                pingReply = pingSender.Send(item.hostName, 250); //Пингуем приоритетно по нему
                                if (item.ip != pingReply.Address) //И проверяем, не изменился ли адресс
                                {
                                    item.ip = pingReply.Address;
                                }
                            }
                            else
                            {
                                name = item.ip.ToString();
                                pingReply = pingSender.Send(item.ip, 250); //Если имя неизвестно, то пингуем по АйПи
                            }

                            //Доступен ли вообще он?
                            if (pingReply.Status == IPStatus.Success)
                            {
                                access = true;
                                item.lastPing = pingReply.RoundtripTime;
                            }
                            
                        }
                        //Доступен ли вообще он?
                        if (access)
                        {
                            if (item.report)
                            {
                                item.report = false;
                                online.Add(item);
                            }
                            item.status = 1;
                            item.lastView = DateTime.Now;
                            
                        }
                        else
                        {
                            item.status = 0;
                            if (!item.report && !item.ignoreNotification)
                            {
                                item.report = true;
                                offline.Add(item);
                            }
                        }
                    }
                    catch
                    {
                        item.status = 0;
                        if (!item.report && !item.ignoreNotification)
                        {
                            item.report = true;
                            offline.Add(item);
                        }
                    }

                    if (item.displayName != "") name = item.displayName;
                    else item.displayName = item.hostName;
                    item.id = id;
                    id++;

                    //Собираем лог-запись

                    //Проверяем, не сменился ли месяц проверки? Полезно в ночь с 31 на 1
                    if (item.logWrite)
                    {
                        DateTime newDateTime = DateTime.Now;
                        if (dateTime.Month != newDateTime.Month) logger = new Logger(setting);

                        string hostNameLog = "";
                        if (item.group != "Без группы") hostNameLog = item.group + "/" + item.displayName + "/" + item.hostName + "/" + item.ip;
                        else hostNameLog = item.displayName + "/" + item.hostName + "/" + item.ip;

                        string eventLog = "";
                        if (item.status == 0) eventLog = "Хост недоступен";
                        else eventLog = "Хост доступен, пинг = " + item.lastPing;

                        LogEvent logEvent = new LogEvent
                        {
                            id = item.id,
                            date = newDateTime,
                            host = hostNameLog,
                            hostEvent = eventLog
                        };

                        logger.OnWriteEvent(logEvent);
                    }
                }
            }
            catch
            {

            }


            //Если же были ошибки доступа, то выводим текст сообщения
            if (offline.Count > 0)
            {
                //Создаем текст сообщения
                noError = false;
                string errorComp = "";
                foreach (var obj in offline)
                {
                    errorComp += obj.displayName + "\n";
                }

                //Сообщение при нормальном режиме
                if (WindowState == FormWindowState.Normal)
                {
                    //MessageBox.Show("Обнаружены недоступные компьютеры! \nСписок компьютеров: \n" + errorComp, "Недоступные компьютеры");
                }

                //При сворачивании в трей
                if (WindowState == FormWindowState.Minimized)
                {
                    notify.BalloonTipIcon = ToolTipIcon.Warning;
                    notify.BalloonTipTitle = "Обнаружены недоступные компьютеры!";
                    notify.BalloonTipText = errorComp;
                    notify.ShowBalloonTip(2000);
                }
            }

            if (noError)
            {
                notifyIcon2.Visible = false;
                notifyIcon1.Visible = true;
                notify = notifyIcon1;
            }
            else
            {
                notifyIcon1.Visible = false;
                notifyIcon2.Visible = true;
                notify = notifyIcon2;
            }
            //Появились компьютеры
            if (online.Count > 0)
            {
                //Создаем текст сообщения
                string onlineComp = "";
                foreach (var obj in online)
                {
                    onlineComp += obj.displayName + "\n";
                }

                //Сообщение при нормальном режиме
                if (WindowState == FormWindowState.Normal)
                {
                    //MessageBox.Show("Offline хосты снова online! \nСписок компьютеров: \n" + onlineComp, "Появление хостов");
                }

                //При сворачивании в трей
                if (WindowState == FormWindowState.Minimized)
                {
                    notify.BalloonTipIcon = ToolTipIcon.Info;
                    notify.BalloonTipTitle = "Offline хосты снова online!";
                    notify.BalloonTipText = onlineComp;
                    notify.ShowBalloonTip(2000);
                }
            }

            refresh = true;
            OnWriteItems();
        }

        public void RefreshViewList()
        {
            if (items.Count != 0)
            {
                if (refresh) //Если флаг реестра говорит нам о том, что надо обновится
                {
                    refresh = false;
                    //Очищаем наш лист
                    try
                    {
                        listView1.Clear();
                        listView1.Groups.Clear();
                        listView2.Items.Clear();
                        listView2.Groups.Clear();
                    }
                    catch
                    {
                        notify.BalloonTipIcon = ToolTipIcon.Warning;
                        notify.BalloonTipTitle = "Невозможно очистить коллекцию!";
                        notify.BalloonTipText = "Ох беда!!!";
                        notify.ShowBalloonTip(1000);
                    }

                    //обновляем все группы
                    foreach (var item in group) 
                    {
                        listView1.Groups.Add(item);
                        listView2.Groups.Add(item);
                    }                    

                    //Обновляем все элементы коллекции
                    foreach (var item in items) 
                    {
                        //Определяем статус
                        string status = "";
                        string ping = "";
                        Color color = new Color();
                        Font font = null;
                        string date = "";
                        if (item.lastView == new DateTime()) date = "Никогда не был доступен";
                        else date = $"Недоступен с {item.lastView}";
                        switch (item.status)
                        {
                            case 0:
                                status = date;
                                ping = "---";
                                color = Color.OrangeRed;
                                font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
                                break;
                            case 1:
                                status = "Доступен";
                                color = Color.ForestGreen;
                                if (item.lastPing < 1) { ping = "< 1 мс"; }
                                else ping = item.lastPing + " мс";
                                font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
                                break;
                            case 2:
                                status = "Информация обновляется";
                                ping = "Неизвестно";
                                color = Color.Black;
                                font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
                                break;
                        }                   

                        //Создаем элементы для коллекции с картинками
                        ListViewItem listViewItem = new ListViewItem
                        {
                            Text = item.displayName,
                            ImageIndex = item.status,
                            ToolTipText = $"Имя хоста: {item.hostName}" + "\n" + $"IP хоста: {item.ip}" + "\n" + $"Описание: {item.description}" + "\n" + "\n" + $"Статус: {status}" + "\n" + $"Ping: {ping}"
                        };
                        try
                        {
                            listViewItem.Group = listView1.Groups[item.group];
                        }
                        catch
                        {
                            listViewItem.Group = listView1.Groups["Без группы"];
                        }


                        //Создаем для таблицы
                        ListViewItem viewItem = new ListViewItem
                        {
                            Text = item.displayName,
                            UseItemStyleForSubItems = false,
                            ToolTipText = $"Имя хоста: {item.hostName}" + "\n" + $"IP хоста: {item.ip}" + "\n" + $"Описание: {item.description}" + "\n" + "\n" + $"Статус: {status}" + "\n" + $"Ping: {ping}"
                        };

                        try
                        {
                            viewItem.Group = listView1.Groups[item.group];
                        }
                        catch
                        {
                            viewItem.Group = listView1.Groups["Без группы"];
                        }

                        string ipst = "";
                        try
                        {
                            ipst = item.ip.ToString();
                        }
                        catch
                        {
                            ipst = "0.0.0.0";
                        }

                        ListViewItem.ListViewSubItem ip = new ListViewItem.ListViewSubItem
                        {
                            Text = ipst,
                            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold)
                        };

                        ListViewItem.ListViewSubItem itemStatus = new ListViewItem.ListViewSubItem
                        {
                            Text = status,
                            Font = font,
                            ForeColor = color
                        };

                        viewItem.SubItems.Add(ip);
                        viewItem.SubItems.Add(itemStatus);
                        viewItem.SubItems.Add(ping);
                        //Добавляем элемент
                        listView1.Items.Add(listViewItem);
                        listView2.Items.Add(viewItem);
                    }

                    //Проверяем, есть ли пустые группы
                    for (int i = 0; i < listView1.Groups.Count; i++)
                    {
                        if (listView1.Groups[i].Items.Count == 0 && listView1.Groups[i].Name != "Без группы")
                        {
                            ListViewItem item = new ListViewItem //И добавляем им пустой элемент
                            {
                                Text = "",
                                Group = listView1.Groups[i]
                            };

                            ListViewItem item2 = new ListViewItem //И добавляем им пустой элемент
                            {
                                Text = "",
                                Group = listView2.Groups[i]
                            };

                            listView1.Items.Add(item);
                            listView2.Items.Add(item2);
                        }
                    }
                }
            }
            else //Если же у нас пустая коллекция, то и отображать ничего не надо
            {
                try
                {
                    listView1.Clear();
                    listView1.Items.Clear();
                }
                catch { }
            }

            if (firstStart)
            {
                try { backgroundWorker1.RunWorkerAsync();  firstStart = false; }
                catch { }
            }
        }

        /////////////////////////////////////////////////////////////////      Добавляем новый экземпляр компьютера в коллекцию     /////////////////////////////////////////////////

        public void AddItemList(Items item)
        {
            item.id = items.Count;

            if (item.hostName == "")
            {
                try
                {
                    IPHostEntry entry = Dns.GetHostEntry(item.ip);
                    item.hostName = entry.HostName;
                }
                catch(Exception ex)
                {
                    item.hostName = "Unknown";
                    MessageBox.Show("Возникла ошибка обнаружения компьютера! Компьютер будет добавлен в список по IP адресу, но это не гарантирует доступность его по сети. " +
                        $"Проблема может возникать с некоторыми Linux-подобными ОС. Сообщение ошибки:{ex.Message}",
                        "Ошибка поиска",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            if (item.ip == null)
            {
                Ping pingSender = new Ping();
                try
                {
                    PingReply reply = pingSender.Send(item.hostName);
                    item.ip = reply.Address;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Возникла ошибка обнаружения компьютера! Проверьте правильность набранного имени хоста или уже убедитесь, " +
                        $"что брендмауер целевого компьютера пропускает ICMP пакеты. Компьютер будет добавлен в список, но это не гарантирует его доступность. Сообщение ошибки:{ex.Message}",
                        "Ошибка поиска",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            if (item.displayName == "") item.displayName = item.hostName;
            item.status = 2; //По умолчанию ставится статус *Информация обновляется*
            items.Add(item);
            refresh = true;
            try
            {
                backgroundWorker1.RunWorkerAsync();
            }
            catch { }
            OnWriteItems();
            RefreshViewList();
        }

        ////////////////////////////////////////////////////////////////     Записываем данные о компьютере в файл    ///////////////////////////////////////////////////////////////
        
        public void OnWriteItems()
        {
            File.Copy(setting.computersPath + @"\data.ssc", setting.computersPath + @"\dataBackup.ssc", true);
            File.Delete(setting.computersPath + @"\data.ssc");
            string[] write = new string[items.Count + group.Count];

            for (int i = 0; i < group.Count; i++)
            {
                write[i] = "group;" + group[i].Name;
            }

            for (int i = 0 ; i < items.Count; i++)
            {
                string groupes = "Без группы";
                for (int j = 0; j < group.Count; j++)
                {
                    if (items[i].group == group[j].Name) groupes = items[i].group;
                }

                if (items[i].ip != null)
                    write[i + group.Count] = "host;" + items[i].id.ToString() + ";" + items[i].ip.ToString() + ";" + items[i].displayName + ";" + items[i].hostName + ";" + items[i].description + ";" + groupes + ";" + items[i].lastView + ";" + items[i].ignoreNotification.ToString() + ";" + items[i].logWrite.ToString();
                else
                    write[i + group.Count] = "host;" + items[i].id.ToString() + ";" + "0.0.0.0" + ";" + items[i].displayName + ";" + items[i].hostName + ";" + items[i].description + ";" + groupes + ";" + items[i].lastView + ";" + items[i].ignoreNotification.ToString() + ";" + items[i].logWrite.ToString();
            }

            File.WriteAllLines(setting.computersPath + @"\data.ssc", write, Encoding.UTF8);
        }

        private void среднееВремяОткликаУХостаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DiagramForOneHost diagram = new DiagramForOneHost();
            diagram.Owner = this;
            diagram.ShowDialog(this);
        }
    }

    class ItemComparer : IComparer
    {
        int columnIndex = 0;
        bool sortAscending = true;
        //Это свойство инициализируется при каждом клике на column header'e
        public int ColumnIndex
        {
            set
            {
                //предыдущий клик был на этой же колонке?
                if (columnIndex == value)
                    //да - меняем направление сортировки
                    sortAscending = !sortAscending;
                else
                {
                    columnIndex = value;
                    sortAscending = true;
                }
            }
        }
        //здесь непосредственно производится сравнение
        //возвращаемые значения:
        // < 0 если x < y
        // 0 если x = y
        // > 0 если x > y
        public int Compare(object x, object y)
        {
            try
            {
                string value1 = ((ListViewItem)x).SubItems[columnIndex].Text;
                string value2 = ((ListViewItem)y).SubItems[columnIndex].Text;
                return String.Compare(value1, value2) * (sortAscending ? 1 : -1);
            }
            catch
            {
                return 1;
            }
        }
    }
}
