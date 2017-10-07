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
            //DataTable eventTable = new DataTable("Events");
            Table ta = new Table();
            ta.Create();
            try
            {
                ta.eventTable.ReadXml("Events.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {               
                dataGridView1.DataSource = ta.eventTable;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].HeaderText = "Дата";
                dataGridView1.Columns[2].HeaderText = "Время";
                dataGridView1.Columns[3].HeaderText = "Событие";
                dataGridView1.Columns[3].Width = dataGridView1.Width - dataGridView1.Columns[1].Width -
                    dataGridView1.Columns[2].Width - 45;// -dataGridView1.Columns[2].Width;
                //dataGridView1.AutoResizeColumns();
                //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                //int width = 0;
              //  for (int i = 0; i < dataGridView1.Columns.Count; i++)
              //      width += dataGridView1.Columns[i].Width;
                //dataGridView1.Size = new Size(width, dataGridView1.RowCount * 18);
              //  this.Size = new Size(width + 50, (dataGridView1.RowCount * 20) + 135); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DataTable tabl = new DataTable("Events");
            Table ta = new Table();
            ta.Create();
            ta.eventTable = (DataTable)dataGridView1.DataSource;
            ta.eventTable.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
            dataGridView1.DataSource = ta.eventTable;   
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
           // DataTable table = new DataTable("Events");
            Table ta = new Table();
            ta.Create();

            ta.eventTable = (DataTable)dataGridView1.DataSource;
            ta.eventTable.WriteXml("Events.xml");
            
        }


    }
}
