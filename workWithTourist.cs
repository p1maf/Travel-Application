using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
namespace travel
{
    public partial class workWithTourist : Form
    {
        string connectionString = DB.connectionString;
        public string address;
        public workWithTourist(string nameBtn, string serial, string number)
        {
            InitializeComponent();
            button1.Text = nameBtn;
            maskedTextBox2.Text = serial;
            maskedTextBox3.Text = number;
            maskedTextBox2.Enabled = false;
            maskedTextBox3.Enabled = false;
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

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^а-яА-Я\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^а-яА-Я\b\s-]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^а-яА-Я\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 1)
            {
                textBox1.Text = textBox1.Text.ToUpper();
            }
            textBox1.Select(textBox1.Text.Length, 0);
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length == 1)
            {
                textBox2.Text = textBox2.Text.ToUpper();
            }
            textBox2.Select(textBox2.Text.Length, 0);
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length == 1)
            {
                textBox3.Text = textBox3.Text.ToUpper();
            }
            textBox3.Select(textBox3.Text.Length, 0);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || !maskedTextBox1.MaskCompleted || !maskedTextBox2.MaskCompleted || !maskedTextBox3.MaskCompleted || string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Заполните все поля");
            }
            else
            {
                this.Close();
            }
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

        private void TextBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length == 1)
            {
                textBox4.Text = textBox4.Text.ToUpper();
            }
            textBox4.Select(textBox4.Text.Length, 0);
        }

        private void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^а-яА-Я\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void WorkWithTourist_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MaxDate = new DateTime(DateTime.Now.AddYears(-14).Year, 12, 31);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            getAddress getAddress = new getAddress();
            this.Hide();
            getAddress.ShowDialog();
            if(getAddress.addrEnd != null)
            {
                textBox4.Text = getAddress.addrEnd;
            }
            this.Show();
        }
    }
}
