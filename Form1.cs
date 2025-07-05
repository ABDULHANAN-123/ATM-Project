using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ATM_FINAL
{
    public partial class Form1 : Form
    {
        string connectionString = "User Id=ATM_FINAL;Password=1234;Data Source=localhost:1521/xe;";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            string cardNo = txtCardNo.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (cardNo == "" || password == "")
            {
                MessageBox.Show("Please enter both Card Number and Password.");
                return;
            }

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string insertQuery = "INSERT INTO BANK_USER (CARD_NO, PASSWORD) VALUES (:card, :pass)";
                    OracleCommand cmd = new OracleCommand(insertQuery, con);
                    cmd.Parameters.Add(":card", cardNo);
                    cmd.Parameters.Add(":pass", password);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                        MessageBox.Show("Sign Up Successful!");
                    else
                        MessageBox.Show("Failed to sign up.");
                }
                catch (OracleException ex)
                {
                    if (ex.Number == 1) // unique constraint violation
                        MessageBox.Show("Card number already exists.");
                    else
                        MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string cardNo = txtCardNo.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (cardNo == "" || password == "")
            {
                MessageBox.Show("Please enter both Card Number and Password.");
                return;
            }

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string loginQuery = "SELECT * FROM BANK_USER WHERE CARD_NO = :card AND PASSWORD = :pass";
                    OracleCommand cmd = new OracleCommand(loginQuery, con);
                    cmd.Parameters.Add(":card", cardNo);
                    cmd.Parameters.Add(":pass", password);

                    OracleDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        MessageBox.Show("Login successful!");
                        Form2 f2 = new Form2(cardNo); // optional: pass card number to Form2
                        f2.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Card Number or Password.");
                    }
                }
                catch (OracleException ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }
        private void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            string cardNo = txtCardNo.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (cardNo == "" || password == "")
            {
                MessageBox.Show("Please enter both Card Number and Password.");
                return;
            }

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM BANK_USER WHERE CARD_NO = :card AND PASSWORD = :pass";
                    OracleCommand cmd = new OracleCommand(query, con);
                    cmd.Parameters.Add(":card", cardNo);
                    cmd.Parameters.Add(":pass", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        // Open Form3 to confirm current password again
                        Form3 form3 = new Form3(cardNo);
                        form3.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Card Number or Password.");
                    }
                }
                catch (OracleException ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }

        private void btnUpdatePassword_Click_1(object sender, EventArgs e)
        {

        }
    }
}

