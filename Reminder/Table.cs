using System.Data;


namespace Reminder
{
    public class Table
    {

        public Table()
        {
            
        }
        public DataTable eventTable;
       
        public void Create()
        {
            //table Events
            eventTable = new DataTable("Events");

            // events' columns
            DataColumn eventIDColumn = new DataColumn("EventID", typeof(int));
            eventIDColumn.Caption = "Event ID";
            eventIDColumn.ReadOnly = true;
            eventIDColumn.Unique = true;
            eventIDColumn.AllowDBNull = false;
            eventIDColumn.AutoIncrement = true;
            eventIDColumn.AutoIncrementSeed = 0;
            eventIDColumn.AutoIncrementStep = 1;
            DataColumn EventDateColumn = new DataColumn("EventDate", typeof(string));
            EventDateColumn.Caption = "Event Date";
            DataColumn EventTimeColumn = new DataColumn("EventTime", typeof(string));
            EventTimeColumn.Caption = "Event Time";
            DataColumn EventDescColumn = new DataColumn("EventDesc", typeof(string));
            EventDescColumn.Caption = "Event Description";

            //adding columns into table events                
            eventTable.Columns.AddRange(new DataColumn[] {eventIDColumn,
                EventDateColumn, EventTimeColumn, EventDescColumn});
        }
    }
}
