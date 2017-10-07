using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;
using System.IO;
using System.Xml.Serialization;

namespace Reminder
{

    public partial class MainForm : Form
    {
        private ContextMenu contextMenu1;
        private MenuItem menuItem1;
        private MenuItem menuItem2;
        private IContainer components1;
        Thread t ;
        
        public MainForm()
        {
            InitializeComponent();
            
            CreateNotifyIcon();

            t = new Thread(CheckTime);
            t.Start();                     
        }
        public class saveData 
        {
            public string backColor { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public Point location { get; set; }
            
        }
        private void CheckTime()
        {
            //DataTable eventTable = new DataTable("Events");
            Table ta = new Table();
            ta.Create();
            while (true)
            {
                Thread.Sleep(60000);
                
                try
                {
                    ta.eventTable.ReadXml("Events.xml");

                }
                catch (Exception )
                {
                }
                for (int i = 0; i < ta.eventTable.Rows.Count; i++ )
                {

                    object[] itemsArray = ta.eventTable.Rows[i].ItemArray;
                    string date = DateTime.Now.Date.ToString();
                    string time = DateTime.Now.TimeOfDay.ToString();
                    if (itemsArray[1].ToString() == date.Substring(0, 10) &&
                        itemsArray[2].ToString() == time.Substring(0, 5))
                        MessageBox.Show("Наступило событие: " + itemsArray[3] + "\n" +
                    "Дата: " + itemsArray[1] + " " + itemsArray[2], "Время пришло!");
                }
                

            }
        }
        
        private void CreateNotifyIcon()
        {           
            components1 = new Container();
            contextMenu1 = new ContextMenu();
            menuItem1 = new MenuItem();
            menuItem2 = new MenuItem();
            //initialize contextmenu
            this.contextMenu1.MenuItems.AddRange(new MenuItem[] { menuItem1, menuItem2 });
            //initialize menuitem
            menuItem1.Index = 0;
            menuItem1.Text = "Открыть";
            menuItem2.Index = 1;
            menuItem2.Text = "Выход";
            menuItem1.Click += new EventHandler(this.menuItem1_Click);
            menuItem2.Click += new EventHandler(this.menuItem2_Click);

            //initialize notifyicon
            notifyIcon1.ContextMenu = contextMenu1;
            notifyIcon1.Text = "Reminder";
            notifyIcon1.Visible = false;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            
        }


        private void menuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            DialogResult dlg = MessageBox.Show("Вы уверены?", "Выход", MessageBoxButtons.YesNo);
            if (dlg == DialogResult.No)
                e.Cancel = true;
            else
            {
                SaveSettings();

            }


        }

        private void SaveSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(saveData));

            TextWriter writer = null;
            try
            {
                writer = new StreamWriter("Settings.xml");

                saveData myData = new saveData();
                myData.backColor = ColorTranslator.ToHtml(this.BackColor);
                myData.width = this.Size.Width;
                myData.height = this.Size.Height;
                myData.location = this.Location;
                serializer.Serialize(writer, myData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                writer.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == string.Empty)
            {
                MessageBox.Show("Введите описание для события!");
            }
            else
            {
                AddEventToTable();

            }

        }

        private void AddEventToTable()
        {
            //table Events
            Table Events = new Table();
            Events.Create();

            //read table from file if exist
            try
            {
                Events.eventTable.ReadXml("Events.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //fill table row
            DataRow eventRow = Events.eventTable.NewRow();
            eventRow["EventDate"] = monthCalendar1.SelectionRange.Start.ToShortDateString();
            eventRow["EventTime"] = comboBox1.Text + ":" + comboBox2.Text;
            eventRow["EventDesc"] = richTextBox1.Text;
            Events.eventTable.Rows.Add(eventRow);
            //write to file
            Events.eventTable.WriteXml("Events.xml");

            MessageBox.Show("Событие дабавлено!");
            richTextBox1.Text = "";
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            menuItem1_Click(sender, e);
         
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadSettings();
            for (int i = 0; i < 60; i++)
            {
                if (i < 24)
                    comboBox1.Items.Add(i);
                comboBox2.Items.Add(i);
            }
            MissedEvents();

        }

        private static void MissedEvents()
        {
            Table Events = new Table();
            Events.Create();
            try
            {
                Events.eventTable.ReadXml("Events.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            string allPastEvents = string.Empty;
            for (int i = 0; i < Events.eventTable.Rows.Count; i++)
            {
                object[] itemsArray = Events.eventTable.Rows[i].ItemArray;
                string s1 = itemsArray[1].ToString() + " " + itemsArray[2].ToString();
                if (DateTime.Now.CompareTo(Convert.ToDateTime(s1)) > 0)
                    allPastEvents += itemsArray[3].ToString() + " Дата: " + s1 + "\n";
            }
            if (allPastEvents != string.Empty)
            {
                MessageBox.Show("Пропущенные события: \n" + allPastEvents, "Уведомление");
            }
            else
            {
                MessageBox.Show("Вы не пропустили никаких событий.", "Уведомление");
            }
        }

        private void ReadSettings()
        {
            RegistryKey myKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            myKey.SetValue("Reminder", Application.ExecutablePath);
            myKey.Close();

            XmlSerializer serializer = new XmlSerializer(typeof(saveData));
            saveData myData;
            FileStream fs = null;
            try
            {
                fs = new FileStream("Settings.xml", FileMode.Open);
                myData = (saveData)serializer.Deserialize(fs);
                //myData.setData();
                this.BackColor = ColorTranslator.FromHtml(myData.backColor);
                this.Size = new Size(myData.width, myData.height);
                this.Location = myData.location;
                //fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                fs.Close();
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View frm2 = new View();
            frm2.Owner = this;
            frm2.Show();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Red;
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Orange;
        }

        private void goldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Gold;
        }

        private void lightGreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.LightGreen;
        }

        private void greenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Green;
        }

        private void aquaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Aqua;
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Blue;
        }

        private void indigoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Indigo;
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.LightGray;
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
        }       

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            t.Abort();         
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Owner = this;
            about.Show();

        }

        private void помощьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Owner = this;
            help.Show();
        }
    }

}
