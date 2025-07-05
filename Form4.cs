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
    public partial class Form4 : Form
    {
        private string connectionString = "User Id=ATM_FINAL;Password=1234;Data Source=localhost:1521/xe;";
        private string cardNo;

        public Form4(string cardNum)
        {
            InitializeComponent();
            cardNo = cardNum;
        }
        private void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in both fields.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE BANK_USER SET PASSWORD = :newPass WHERE CARD_NO = :cardNo";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add("newPass", OracleDbType.Varchar2).Value = newPassword;
                    cmd.Parameters.Add("cardNo", OracleDbType.Varchar2).Value = cardNo;

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Password updated successfully!");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error: Card number not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
           
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3(cardNo);
            f3.Show();
            this.Hide();
        }

        
    }
}
