using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ATM_FINAL
{
   
    public partial class Form5 : Form
    {
        string connectionString = "User Id=QUIZ;Password=1234;Data Source=localhost:1521/xe;";
        string roll;
        public Form5()
        {
            InitializeComponent();
            this.roll=roll;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
           
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {

            string roll = textBox1.Text.Trim();
            string subject1 = textBox2.Text.Trim();
            string subject2 = textBox3.Text.Trim();
            string subject3 = textBox4.Text.Trim();



            if (roll == "" || subject1 == "" || subject2 == "" || subject3 == "")
            {
                MessageBox.Show("Please enter all details");
                return;
            }

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string insertQuery = "INSERT INTO Marks(roll,subject1,subject2,subject3) VALUES (:textbox1, :textbox2,:textbox3,:textbox4)";
                    OracleCommand cmd = new OracleCommand(insertQuery, con);
                    cmd.Parameters.Add(":textbox1", roll);
                    cmd.Parameters.Add(":textbox2", subject1);
                    cmd.Parameters.Add(":textbox3", subject2);
                    cmd.Parameters.Add(":textbox4", subject3);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                        MessageBox.Show("Added Successful!");
                    else
                        MessageBox.Show("Failed to Add");
                }
                catch (OracleException ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ROLL,SUBJECT1,SUBJECT2,SUBJECT3,TOTAL,PERCENTAGE FROM MARKS WHERE ROLL=:roll";
                                   
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("ROLL",roll);
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading transaction history: " + ex.Message);
                }
            }
        }
    }
}
