using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace travel
{
    public partial class getAddress : Form
    {
        string connectionString = "Data Source=./kladr.db";
        string newOcatd;
        string code;
        public string addrEnd;
        public getAddress()
        {
            InitializeComponent();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SQLiteConnection con = new SQLiteConnection())
            {
                con.ConnectionString = connectionString;
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand($"SELECT code FROM region WHERE name = '{comboBox1.Text}'", con);
                code = cmd.ExecuteScalar().ToString();

                cmd = new SQLiteCommand($"SELECT name FROM kladr WHERE code LIKE '{code}%' AND socr = '{comboBox2.Text.ToLower()[0]}'", con);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                var values = new AutoCompleteStringCollection();
                while (rdr.Read())
                {
                    values.Add($"{rdr[0].ToString()}");

                }
                comboBox3.AutoCompleteCustomSource = values;
                comboBox3.AutoCompleteMode = AutoCompleteMode.Suggest;
                comboBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
        }

        private void GetAddress_Load(object sender, EventArgs e)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT name FROM region", con);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    var values = new AutoCompleteStringCollection();
                    while (rdr.Read())
                    {
                        values.Add(rdr[0].ToString());
                    }
                    comboBox1.AutoCompleteCustomSource = values;
                    comboBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
                    comboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        

        

        private void Button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(comboBox1.Text) || string.IsNullOrWhiteSpace(comboBox2.Text) || string.IsNullOrWhiteSpace(comboBox3.Text) || string.IsNullOrWhiteSpace(comboBox4.Text) || string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Заполните поля");
            }
            else
            {
                addrEnd = $"Россия, {comboBox1.Text}, {comboBox2.Text.ToLower()[0]}. {comboBox3.Text}, ул. {comboBox4.Text}, д.{textBox3.Text}, кв.{textBox1.Text}";
                this.Close();
            }
        }

        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Text = "";
            using (SQLiteConnection con = new SQLiteConnection())
            {
                con.ConnectionString = connectionString;
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand($"SELECT code FROM region WHERE name = '{comboBox1.Text}'", con);
                code = cmd.ExecuteScalar().ToString();

                cmd = new SQLiteCommand($"SELECT name FROM kladr WHERE code LIKE '{code}%' AND socr = '{comboBox2.Text.ToLower()[0]}'", con);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                var values = new AutoCompleteStringCollection();
                while (rdr.Read())
                {
                    values.Add($"{rdr[0].ToString()}");
                    comboBox3.Items.Add(rdr[0].ToString());
                }
                comboBox3.AutoCompleteCustomSource = values;
                comboBox3.AutoCompleteMode = AutoCompleteMode.Suggest;
                comboBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;
                comboBox3.Enabled = true;
            }
        }

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.Text.Length > 3)
                {
                    string ocatd = "";
                    using (SQLiteConnection con = new SQLiteConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        SQLiteCommand cmd = new SQLiteCommand($"SELECT ocatd FROM kladr WHERE name = '{comboBox3.Text}' AND socr = '{comboBox2.Text.ToLower()[0]}' AND code LIKE '{code[0]}{code[1]}%'", con);
                        SQLiteDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            ocatd = rdr[0].ToString();
                        }
                    }
                    newOcatd = ocatd.TrimEnd('0');

                    using (SQLiteConnection con = new SQLiteConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        SQLiteCommand cmd = new SQLiteCommand($"SELECT name FROM street WHERE (ocatd LIKE '{newOcatd}%') AND (socr = 'ул' OR socr = 'ш')", con);
                        SQLiteDataReader rdr = cmd.ExecuteReader();
                        var values = new AutoCompleteStringCollection();
                        while (rdr.Read())
                        {
                            values.Add(rdr[0].ToString());
                            comboBox4.Items.Add(rdr[0].ToString());
                        }
                        comboBox4.AutoCompleteCustomSource = values;
                        comboBox4.AutoCompleteMode = AutoCompleteMode.Suggest;
                        comboBox4.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        comboBox4.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void ComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string codeStreet = "";
                string ocatdStreet = "";
                if (comboBox4.Text.Length >= 3)
                {
                    using (SQLiteConnection con = new SQLiteConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        SQLiteCommand cmd = new SQLiteCommand($"SELECT code,ocatd FROM street WHERE ocatd LIKE '{newOcatd}%' AND socr = 'ул' AND name = '{comboBox4.Text.Trim()}'", con);
                        SQLiteDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            codeStreet = rdr[0].ToString();
                            ocatdStreet = rdr[1].ToString();
                        }

                        rdr.Close();
                        cmd = new SQLiteCommand($"SELECT name FROM doma WHERE ocatd = '{ocatdStreet}' AND code LIKE '{codeStreet}%'", con);
                        rdr = cmd.ExecuteReader();
                        var values = new AutoCompleteStringCollection();
                        while (rdr.Read())
                        {
                            var words = rdr[0].ToString().Split(',');
                            foreach (var item in words)
                            {
                                values.Add(item);
                            }
                        }
                        textBox3.AutoCompleteCustomSource = values;
                        textBox3.AutoCompleteMode = AutoCompleteMode.Suggest;
                        textBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        textBox3.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        private void ComboBox1_TextUpdate(object sender, EventArgs e)
        {
            if(comboBox1.Text == "")
            {
                comboBox3.Enabled = false;
                comboBox3.Text = "";
                comboBox4.Enabled = false;
                comboBox4.Text = "";
                textBox3.Enabled = false;
                textBox3.Text = "";
                textBox1.Enabled = false;
                textBox1.Text = "";
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

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
        }
    }
}
