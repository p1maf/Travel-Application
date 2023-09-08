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
using System.Text.RegularExpressions;
namespace travel
{
    public partial class workWithTourOperator : Form
    {
        string connectionString = DB.connectionString;
        string operatorID, operatorINN;

        private void Button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox5.Text) ||  string.IsNullOrWhiteSpace(textBox5.Text) || string.IsNullOrWhiteSpace(textBox3.Text) || !maskedTextBox1.MaskCompleted || !maskedTextBox2.MaskCompleted)
            {
                MessageBox.Show("Заполните поля");
            }
            else
            {
                if(button1.Text == "Добавить")
                {
                    try
                    {
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $@"SELECT * FROM tour_operator WHERE INN = '{maskedTextBox1.Text}'";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteScalar() != null)
                            {
                                MessageBox.Show("Туроператор уже занесен в БД");
                            }
                            else
                            {
                                query = $@"INSERT INTO tour_operator VALUES (null, '{textBox1.Text.Trim()}', '{textBox2.Text.Trim()}', '{maskedTextBox1.Text}', '{maskedTextBox2.Text}', '{textBox5.Text.Trim()}', '{textBox3.Text.Trim()}')";

                                cmd = new MySqlCommand(query, con);
                                if (cmd.ExecuteNonQuery() == 1)
                                {
                                    MessageBox.Show("Туроператор успешно добавлен");
                                    this.Close();
                                    controlClear();
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка");
                    }
                }
                else
                {
                    try
                    {
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $@"SELECT INN FROM tour_operator WHERE INN = '{operatorINN}'";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteScalar() == null || cmd.ExecuteScalar().ToString() == operatorINN)
                            {
                                query = $@"UPDATE tour_operator SET name = '{textBox1.Text.Trim()}', legal_name = '{textBox2.Text.Trim()}', INN = '{maskedTextBox1.Text}', OGRN = '{maskedTextBox2.Text}',  address = '{textBox5.Text.Trim()}', contract = {textBox3.Text.Trim()} WHERE id = {operatorID}";

                                DialogResult result = MessageBox.Show(
                                "Вы точно хотите отредактировать запись?",
                                "Сообщение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1);
                                if(result == DialogResult.Yes)
                                {
                                    cmd = new MySqlCommand(query, con);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {
                                        MessageBox.Show("Успешное редактирование");
                                        this.Close();
                                        controlClear();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Ошибка");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Туроператор уже занесен в БД");
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

        public workWithTourOperator(string nameButton, string idOperator)
        {
            InitializeComponent();

            if(idOperator != "")
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        string query = $@"SELECT tour_operator.name, legal_name,INN,OGRN,address, contract FROM tour_operator WHERE tour_operator.id = {idOperator};";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            textBox1.Text = rdr[0].ToString();
                            textBox2.Text = rdr[1].ToString();
                            maskedTextBox1.Text = rdr[2].ToString();
                            maskedTextBox2.Text = rdr[3].ToString();
                            textBox5.Text = rdr[4].ToString();
                            textBox3.Text = rdr[5].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
            button1.Text = nameButton;
            operatorID = idOperator;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 1)
            {
                textBox1.Text = textBox1.Text.ToUpper();
            }
            textBox1.Select(textBox1.Text.Length, 0);
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^a-zA-Zа-яА-Я\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^а-яА-Я\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void TextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^а-яА-Я1-9\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                            "Вы точно хотите вернуться, все изменения будут потеряны?",
                            "Сообщение",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            getAddress getAddress = new getAddress();
            this.Hide();
            getAddress.ShowDialog();
            if(getAddress.addrEnd != null)
            {
                textBox5.Text = getAddress.addrEnd;
            }
            this.Show();
        }

        void controlClear()
        {
            foreach (Control c in Controls)
            {
                if (c is TextBox)
                {
                    c.Text = "";
                }
                if(c is MaskedTextBox)
                {
                    c.Text = "";
                }
            }
        }
           
    }
}
