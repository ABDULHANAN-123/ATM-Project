using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ATM_FINAL
{
    public partial class Form3 : Form
    {
        private string connectionString = "User Id=ATM_FINAL;Password=1234;Data Source=localhost:1521/xe;";
        private string cardNo;

        public Form3(string cardNo)
        {
            InitializeComponent();
            this. cardNo = cardNo;
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            string cardNumber = txtCardNo.Text.Trim();
            string currentPassword = txtCurrentPassword.Text.Trim();

            if (string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(currentPassword))
            {
                MessageBox.Show("Please enter both Card Number and Current Password.");
                return;
            }

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM BANK_USER WHERE CARD_NO = :cardNo AND PASSWORD = :currPass";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add(":cardNo", cardNumber);
                    cmd.Parameters.Add(":currPass", currentPassword);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        // Password verified, open Form4
                        Form4 f4 = new Form4(cardNumber); // Pass the card number to Form4
                        f4.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect card number or password.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
           
        }
        
    }
}
