using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ATM_FINAL
{
    public partial class Form2 : Form
    {
        string connectionString = "User Id=ATM_FINAL;Password=1234;Data Source=localhost:1521/xe;";
        string cardNo;

        public Form2(string cardNo)
        {
            InitializeComponent();
            this.cardNo = cardNo;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT USER_IMAGE FROM BANK_USER WHERE CARD_NO = :cardNo";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(":cardNo", OracleDbType.Varchar2).Value = cardNo;

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            byte[] imgBytes = (byte[])reader["USER_IMAGE"];
                            using (MemoryStream ms = new MemoryStream(imgBytes))
                            {
                                pictureBox1.Image = Image.FromStream(ms);
                            }
                        }
                    }
                }
            }
            LoadBalance();
            LoadTransactionHistory();
        }
      
        private void LoadBalance()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT BALANCE FROM BANK_USER WHERE CARD_NO = :cardNo";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("cardNo", cardNo);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            double balance = Convert.ToDouble(result);
                            textBox1.Text = balance.ToString();
                        }
                        else
                        {
                            textBox1.Text = "0";
                            MessageBox.Show("Account not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading balance: " + ex.Message);
                }
            }
        }

        private void LoadTransactionHistory()
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT TYPE, AMOUNT, TO_CHAR(TRANS_DATE, 'DD-MON-YYYY HH:MI:SS AM') AS DATE_TIME " +
                                   "FROM TRANSACTIONS WHERE CARD_NO = :cardNo ORDER BY TRANS_DATE DESC";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("cardNo", cardNo);
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

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            if (!ValidateAmount(textBox2.Text)) return;

            double withdrawAmount = Convert.ToDouble(textBox2.Text);
            double currentBalance = Convert.ToDouble(textBox1.Text);

            if (withdrawAmount > currentBalance)
            {
                MessageBox.Show("Insufficient Balance!");
                return;
            }

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Update balance
                    string query = "UPDATE BANK_USER SET BALANCE = BALANCE - :amount WHERE CARD_NO = :cardNo";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("amount", withdrawAmount);
                        cmd.Parameters.Add("cardNo", cardNo);
                        cmd.ExecuteNonQuery();
                    }

                    // Log transaction
                    string insertTxn = "INSERT INTO TRANSACTIONS (CARD_NO, TYPE, AMOUNT) VALUES (:cardNo, 'Withdraw', :amount)";
                    using (OracleCommand cmd = new OracleCommand(insertTxn, conn))
                    {
                        cmd.Parameters.Add("cardNo", cardNo);
                        cmd.Parameters.Add("amount", withdrawAmount);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    // Update UI
                    double newBalance = currentBalance - withdrawAmount;
                    textBox1.Text = newBalance.ToString();
                    textBox2.Clear();
                    LoadTransactionHistory();

                    MessageBox.Show("Withdrawal successful!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            if (!ValidateAmount(textBox3.Text)) return;

            double depositAmount = Convert.ToDouble(textBox3.Text);
            double currentBalance = Convert.ToDouble(textBox1.Text);

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Update balance
                    string query = "UPDATE BANK_USER SET BALANCE = BALANCE + :amount WHERE CARD_NO = :cardNo";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("amount", depositAmount);
                        cmd.Parameters.Add("cardNo", cardNo);
                        cmd.ExecuteNonQuery();
                    }

                    // Log transaction
                    string insertTxn = "INSERT INTO TRANSACTIONS (CARD_NO, TYPE, AMOUNT) VALUES (:cardNo, 'Deposit', :amount)";
                    using (OracleCommand cmd = new OracleCommand(insertTxn, conn))
                    {
                        cmd.Parameters.Add("cardNo", cardNo);
                        cmd.Parameters.Add("amount", depositAmount);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    // Update UI
                    double newBalance = currentBalance + depositAmount;
                    textBox1.Text = newBalance.ToString();
                    textBox3.Clear();
                    LoadTransactionHistory();

                    MessageBox.Show("Deposit successful!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private bool ValidateAmount(string input)
        {
            if (!double.TryParse(input, out double value) || value <= 0)
            {
                MessageBox.Show("Please enter a valid positive amount.");
                return false;
            }
            return true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(open.FileName);
            }
        }
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    byte[] imgBytes = ms.ToArray();

                    // Use your connection string here
                    using (OracleConnection conn = new OracleConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "UPDATE BANK_USER SET USER_IMAGE = :img WHERE CARD_NO = :cardNo";
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Parameters.Add(":img", OracleDbType.Blob).Value = imgBytes;
                            cmd.Parameters.Add(":cardNo", OracleDbType.Varchar2).Value = cardNo; 

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Image saved to database!");
                        }
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
