using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace travel
{
    public partial class checkingTourist : Form
    {
        string connectionString = DB.connectionString;
        public string idTourist;
        public string serial, number;
        public checkingTourist()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(!maskedTextBox1.MaskCompleted || !maskedTextBox2.MaskCompleted)
            {
                MessageBox.Show("Заполните поля");
            }
            else
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        string query = $"SELECT id FROM tourist WHERE serial_passport = '{maskedTextBox1.Text}' AND number_passport = '{maskedTextBox2.Text}'";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteScalar() != null)
                        {
                            idTourist = cmd.ExecuteScalar().ToString();
                            this.Close();
                        }
                        else
                        {
                            idTourist = "";
                            serial = maskedTextBox1.Text;
                            number = maskedTextBox2.Text;
                            this.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }
    }
}
