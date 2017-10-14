using System;
using System.Data;
using System.Windows.Forms;

namespace Reminder
{
    public partial class View : Form
    {
        public View()
        {
            InitializeComponent();
        }

        
        private void Form2_Load(object sender, EventArgs e)
        {
            LoadEvents();
        }

        private void LoadEvents()
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
            finally
            {
                dataGridView1.DataSource = Events.eventTable;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].HeaderText = "Дата";
                dataGridView1.Columns[2].HeaderText = "Время";
                dataGridView1.Columns[3].HeaderText = "Событие";
                dataGridView1.Columns[3].Width = dataGridView1.Width - dataGridView1.Columns[1].Width -
                    dataGridView1.Columns[2].Width - 45;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Table Events = new Table();
            Events.Create();
            Events.eventTable = (DataTable)dataGridView1.DataSource;
            Events.eventTable.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
            dataGridView1.DataSource = Events.eventTable;   
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {           
            Table ta = new Table();
            ta.Create();
            ta.eventTable = (DataTable)dataGridView1.DataSource;
            ta.eventTable.WriteXml("Events.xml");
            
        }


    }
}
