using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
namespace travel
{
    public partial class addUser : Form
    {
        public string connectionString = DB.connectionString;
        string userId, userLogin;
        public addUser(string nameUser, string surnameUser, string patronymicUser, string loginUser, string passwordUser, string roleUser, string nameButton, string idUser)
        {
            InitializeComponent();
            if(nameButton != "Добавить")
            {
                textBox1.Text = nameUser;
                textBox2.Text = surnameUser;
                textBox3.Text = patronymicUser;
                textBox4.Text = loginUser;
                textBox5.Text = passwordUser;
                userId = idUser;
                userLogin = loginUser;
                textBox1.ForeColor = Color.Black;
                textBox2.ForeColor = Color.Black;
                textBox3.ForeColor = Color.Black;
                textBox4.ForeColor = Color.Black;
                textBox5.ForeColor = Color.Black;
            }
            comboBox1.Text = roleUser;
            button1.Text = nameButton;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                            "Вы точно хотите вернуться, все изменения будут потеряны?",
                            "Сообщение",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1);
            if(result == DialogResult.Yes)
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
            if(textBox1.Text != "")
            {
                if(textBox1.Text.Length == 1)
                {
                textBox1.Text = textBox1.Text.ToUpper();
                }
                textBox1.Select(textBox1.Text.Length, 0);
            }
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
            if (button1.Text == "Добавить")
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox5.Text) || string.IsNullOrWhiteSpace(comboBox1.Text) || textBox4.Text.Length < 4 || textBox5.Text.Length < 4 || string.IsNullOrWhiteSpace(textBox6.Text) || textBox1.Text == "Имя" || textBox2.Text == "Фамилия" || textBox4.Text == "Логин" || textBox5.Text == "Пароль" || textBox6.Text == "Подтвердите пароль")
                {
                    MessageBox.Show("Поля не заполнены или пароль, логин меньше 4 символов");
                }
                else
                {
                    try
                    {
                        if(textBox5.Text != textBox6.Text)
                        {
                            MessageBox.Show("Подтвердите пароль");
                            textBox6.Clear();
                        }
                        else
                        {
                            using (MySqlConnection con = new MySqlConnection())
                            {
                                con.ConnectionString = connectionString;
                                con.Open();
                                string query = $@"SELECT * FROM users WHERE login = '{textBox4.Text.Trim()}'";
                                MySqlCommand cmd = new MySqlCommand(query, con);
                                if (cmd.ExecuteScalar() != null)
                                {
                                    MessageBox.Show("Пользователь с таким логином уже существует");
                                    textBox4.Clear();
                                }
                                else
                                {

                                    if(textBox3.Text == "Отчество")
                                    {
                                        query = $@"INSERT INTO users VALUES (null, '{textBox1.Text.Trim()}', '{textBox2.Text.Trim()}', '', '{textBox4.Text.Trim()}', '{md5.hashPassword(textBox5.Text.Trim())}', '{comboBox1.SelectedIndex + 1}', 0)";
                                    }
                                    else
                                    {
                                        query = $@"INSERT INTO users VALUES (null, '{textBox1.Text.Trim()}', '{textBox2.Text.Trim()}', '{textBox3.Text.Trim()}', '{textBox4.Text.Trim()}', '{md5.hashPassword(textBox5.Text.Trim())}', '{comboBox1.SelectedIndex + 1}', 0)";

                                    }


                                    cmd = new MySqlCommand(query, con);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {
                                        MessageBox.Show("Пользователь успешно добавлен");
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка");
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox5.Text) || string.IsNullOrWhiteSpace(comboBox1.Text) || textBox4.Text.Length < 4 || textBox5.Text.Length < 4 || string.IsNullOrWhiteSpace(textBox6.Text) || textBox1.Text == "Имя" || textBox2.Text == "Фамилия" || textBox4.Text == "Логин" || textBox5.Text == "Пароль" || textBox6.Text == "Подтвердите пароль")
                {
                    MessageBox.Show("Поля не заполнены или пароль, логин меньше 4 символов");
                }
                else
                {
                    try
                    {
                        if (textBox5.Text != textBox6.Text)
                        {
                            MessageBox.Show("Подтвердите пароль");
                            textBox6.Clear();
                        }
                        else
                        {
                            using (MySqlConnection con = new MySqlConnection())
                            {
                                con.ConnectionString = connectionString;
                                con.Open();
                                string query = $@"SELECT login FROM users WHERE login = '{textBox4.Text.Trim()}'";
                                MySqlCommand cmd = new MySqlCommand(query, con);
                                if (cmd.ExecuteScalar() == null || cmd.ExecuteScalar().ToString() == userLogin)
                                {
                                    DialogResult result = MessageBox.Show(
                                    "Вы точно хотите отредактировать запись?",
                                    "Сообщение",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information,
                                    MessageBoxDefaultButton.Button1);
                                    if(result == DialogResult.Yes)
                                    {
                                        if(textBox3.Text == "Отчество")
                                        {
                                            query = $@"UPDATE users SET name = '{textBox1.Text.Trim()}', surname = '{textBox2.Text.Trim()}', patronymic = '', login = '{textBox4.Text.Trim()}',  password = '{md5.hashPassword(textBox5.Text.Trim())}', role = {comboBox1.SelectedIndex + 1} WHERE id = {userId}";
                                        }
                                        else
                                        {
                                            query = $@"UPDATE users SET name = '{textBox1.Text.Trim()}', surname = '{textBox2.Text.Trim()}', patronymic = '{textBox3.Text.Trim()}', login = '{textBox4.Text.Trim()}',  password = '{md5.hashPassword(textBox5.Text.Trim())}', role = {comboBox1.SelectedIndex + 1} WHERE id = {userId}";

                                        }

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
                                    MessageBox.Show("Пользователь с таким логином уже существует");
                                }
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

        void controlClear()
        {
            foreach(Control c in Controls)
            {
                if (c is TextBox)
                {
                    c.Text = "";
                }
            }
            comboBox1.SelectedIndex = -1;
        }

        private void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^a-zA-Z1-9\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox5.UseSystemPasswordChar = false;
            }
            else
            {
                textBox5.UseSystemPasswordChar = true;
            }
        }

        private void TextBox6_TextChanged(object sender, EventArgs e)
        {
            textBox6.UseSystemPasswordChar = true;
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                textBox6.UseSystemPasswordChar = false;
            }
            else
            {
                textBox6.UseSystemPasswordChar = true;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            int length = 8;
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            textBox5.Text = res.ToString();
            textBox6.Text = res.ToString();
        }

        private void TextBox5_TextChanged(object sender, EventArgs e)
        {
            textBox5.UseSystemPasswordChar = true;
            if (textBox5.Text != "")
            {
                if (Convert.ToInt32(textBox5.Text.Length) > 16)
                {
                    textBox5.Text = textBox5.Text.Remove(textBox5.Text.Length - 1);
                    MessageBox.Show("Длинна пароля не может быть выше 16 символов");
                }
            }
        }

        private void TextBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                if (Convert.ToInt32(textBox4.Text.Length) > 12)
                {
                    textBox4.Text = textBox4.Text.Remove(textBox5.Text.Length - 1);
                    MessageBox.Show("Лоигн не может быть выше 12 символов");
                }
            }
        }

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Имя")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Имя";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void TextBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Фамилия")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void TextBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Фамилия";
                textBox2.ForeColor = Color.Gray;
            }
        }

        private void TextBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "Отчество")
            {
                textBox3.Text = "";
                textBox3.ForeColor = Color.Black;
            }
        }

        private void TextBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                textBox3.Text = "Отчество";
                textBox3.ForeColor = Color.Gray;
            }
        }

        private void TextBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text == "Логин")
            {
                textBox4.Text = "";
                textBox4.ForeColor = Color.Black;
            }
        }

        private void TextBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                textBox4.Text = "Логин";
                textBox4.ForeColor = Color.Gray;
            }
        }

        private void TextBox5_Enter(object sender, EventArgs e)
        {
            if (textBox5.Text == "Пароль")
            {
                textBox5.Text = "";
                textBox5.ForeColor = Color.Black;
            }
        }

        private void TextBox5_Leave(object sender, EventArgs e)
        {
            if (textBox5.Text == "")
            {
                textBox5.Text = "Пароль";
                textBox5.ForeColor = Color.Gray;
                textBox5.UseSystemPasswordChar = false;
            }
        }

        private void TextBox6_Enter(object sender, EventArgs e)
        {
            if (textBox6.Text == "Подтвердите пароль")
            {
                textBox6.Text = "";
                textBox6.ForeColor = Color.Black;
            }
        }

        private void TextBox6_Leave(object sender, EventArgs e)
        {
            if (textBox6.Text == "Подтвердите пароль")
            {
                textBox6.Text = "Пароль";
                textBox6.ForeColor = Color.Gray;
            }
        }

        private void AddUser_Load(object sender, EventArgs e)
        {
            if(userId != null)
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        string query = $"SELECT surname as 'Фамилия', users.name as 'Имя', patronymic as 'Отчество', login as 'Логин', role.name as 'Роль' FROM users INNER JOIN role ON users.role = role.id WHERE users.id = {userId}";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            textBox2.Text = rdr[0].ToString();
                            textBox1.Text = rdr[1].ToString();
                            textBox3.Text = rdr[2].ToString();
                            textBox4.Text = rdr[3].ToString();
                            comboBox1.Text = rdr[4].ToString();
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }

        private void TextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^a-zA-Z1-9\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
    }
}
