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
using System.Threading;
using System.Security.Cryptography;
namespace travel
{
    public partial class Form1 : Form
    {
        string text, userPassword, userRole, userName, checkDel;
        int countTryIn;
        bool captcha = true;
        string connectionString = DB.connectionString;

        private void Button2_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = this.CreateImage(pictureBox2.Width, pictureBox2.Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = DB.emptyString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(@"SELECT SCHEMA_NAME
                                                         FROM INFORMATION_SCHEMA.SCHEMATA
                                                         WHERE SCHEMA_NAME = 'db_tourism'", con);
                    if(cmd.ExecuteScalar() == null)
                    {
                     
                        //Заполнения переменных и выполнение запроса
                        string fileName = $"{Application.StartupPath}\\Резервная копия базы данных\\empty.sql";


                        using (MySqlConnection connnection = new MySqlConnection())
                        {
                            connnection.ConnectionString = DB.emptyString;
                            connnection.Open();
                            MySqlCommand cmd1 = new MySqlCommand($@"CREATE DATABASE IF NOT EXISTS `db_tourism`;", connnection);
                            if (cmd1.ExecuteNonQuery() == 1)
                            {

                            }
                            else
                            {

                            }
                        }

                        using (MySqlConnection conn = new MySqlConnection())
                        {
                            using (MySqlCommand cmd2 = new MySqlCommand())
                            {
                                //Создание подлкючени
                                using (MySqlBackup mb = new MySqlBackup(cmd2))
                                {
                                    conn.ConnectionString = connectionString;
                                    conn.Open();
                                    cmd2.Connection = conn;
                                    mb.ImportFromFile(fileName);
                                    MessageBox.Show("База данных успешно восстановлена, зарегестрируйте пользователя с правами админа");
                                    addUser addUser = new addUser("","","","","","Администратор","Добавить","");
                                    this.Hide();
                                    addUser.button2.Visible = false;
                                    addUser.comboBox1.Enabled = false;
                                    addUser.ShowDialog();
                                    this.Show();
                                }
                                conn.Close();
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

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text == "Логин")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void TextBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Пароль")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Логин";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void TextBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Пароль";
                textBox2.UseSystemPasswordChar = false;
                textBox2.ForeColor = Color.Gray;
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Visible == true)
            {
                if (text == textBox3.Text)
                {
                    captcha = true;
                    textBox3.Clear();
                    pictureBox2.Visible = false;
                    textBox3.Visible = false;
                    button2.Visible = false;
                    this.Enabled = true;
                }
                else
                {
                    captcha = false;
                }
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || textBox1.Text == "Логин" || textBox2.Text == "Пароль")
            {
                if (countTryIn >= 2)
                {
                    MessageBox.Show("Защита от спама. Приложение заблокировано на 10 секунд");
                    this.Enabled = false;
                    Thread.Sleep(10000);
                    this.Enabled = true;

                }
                MessageBox.Show("Заполните поля");
                countTryIn++;
            }
            else
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand($"SELECT password, concat(surname, ' ', users.name,' ', patronymic), role.name, delUser FROM users JOIN role ON users.role = role.id WHERE login = '{textBox1.Text}';", con);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            userPassword = rdr[0].ToString();
                            userName = rdr[1].ToString();
                            userRole = rdr[2].ToString();
                            checkDel = rdr[3].ToString();
                        }
                    }
                    if (md5.hashPassword(textBox2.Text.Trim()) == userPassword && captcha == true)
                    {
                        if(checkDel == "0")
                        {
                            MessageBox.Show("Вы успешно вошли");
                            MainMenu mainMenu = new MainMenu(userName, userRole, textBox1.Text.Trim());
                            this.Hide();
                            mainMenu.ShowDialog();
                            this.Show();
                            textBox1.Clear();
                            textBox2.Clear();
                            countTryIn = 0;
                            pictureBox2.Visible = false;
                            button2.Visible = false;
                            textBox3.Visible = false;
                        }
                        else
                        {
                            MessageBox.Show("Ваша учетная запись заблокирована");
                        }
                    }
                    else if (captcha == false)
                    {
                        MessageBox.Show("Попробуйте еще раз");
                        textBox3.Clear();
                        pictureBox2.Image = this.CreateImage(pictureBox2.Width, pictureBox2.Height);
                    }
                    else
                    {
                        if(countTryIn >= 2)
                        {
                            MessageBox.Show("Защита от спама. Приложение заблокировано на 10 секунд");
                            this.Enabled = false;
                            Thread.Sleep(10000);
                            this.Enabled = true;
                            MessageBox.Show("Логин или пароль введен, подтвердите что вы не робот");
                            pictureBox2.Visible = true;
                            textBox3.Visible = true;
                            button2.Visible = true;
                            pictureBox2.Image = this.CreateImage(pictureBox2.Width, pictureBox2.Height);
                        }
                        else if(countTryIn >= 1)
                        {
                            MessageBox.Show("Логин или пароль введен, подтвердите что вы не робот");
                            pictureBox2.Visible = true;
                            textBox3.Visible = true;
                            button2.Visible = true;
                            pictureBox2.Image = this.CreateImage(pictureBox2.Width, pictureBox2.Height);
                        }
                        else
                        {
                            MessageBox.Show("Попробуйте еще раз");
                        }

                    }
                    countTryIn++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }
        private Bitmap CreateImage(int Width, int Height)
        {

            Random rnd = new Random();

            //Создадим изображение
            Bitmap result = new Bitmap(Width, Height);

            //Вычислим позицию текста
            int Xpos = Width / 10; // примерно по центру
            int Ypos = Height / 18;

            //Добавим различные цвета ддя текста
            Brush[] colors = {
                                Brushes.Black,
                                Brushes.Red,
                                Brushes.RoyalBlue,
                                Brushes.Green,
                                Brushes.Yellow,
                                Brushes.White,
                                Brushes.Tomato,
                                Brushes.Sienna,
                                Brushes.Pink };

            //Добавим различные цвета линий
            Pen[] colorpens = {
                                Pens.Black,
                                Pens.Red,
                                Pens.RoyalBlue,
                                Pens.Green,
                                Pens.Yellow,
                                Pens.White,
                                Pens.Tomato,
                                Pens.Sienna,
                                Pens.Pink };

            //Делаем случайный стиль текста
            FontStyle[] fontstyle = {
                                FontStyle.Bold,
                                FontStyle.Italic,
                                FontStyle.Regular,
                                FontStyle.Strikeout,
                                FontStyle.Underline};

            //Добавим различные углы поворота текста
            Int16[] rotate = { 1, -1, 2, -2, 3, -3, 4, -4, 5, -5, 6, -6 };

            //Укажем где рисовать
            Graphics g = Graphics.FromImage((Image)result);

            //Пусть фон картинки будет серым
            g.Clear(Color.Gray);

            //Делаем случайный угол поворота текста
            g.RotateTransform(rnd.Next(rotate.Length));

            //Генерируем текст
            text = String.Empty;
            string ALF = "1234567890qwertyuiopasdfghjklzxcvbnm";
            for (int i = 0; i < 6; ++i)
            {
                text += ALF[rnd.Next(ALF.Length)];
            }

            //Нарисуем сгенирируемый текст
            g.DrawString(text,
                new Font("Arial", 26, fontstyle[rnd.Next(fontstyle.Length)]),
                colors[rnd.Next(colors.Length)],
                new PointF(Xpos, Ypos));

            //Добавим немного помех
            //Линии из углов
            g.DrawLine(colorpens[rnd.Next(colorpens.Length)],
                       new Point(0, 0),
                       new Point(Width - 1, Height - 1));

            g.DrawLine(colorpens[rnd.Next(colorpens.Length)],
                       new Point(0, Height - 1),
                       new Point(Width - 1, 0));


            //Белые точки
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    if (rnd.Next() % 20 == 0)
                        result.SetPixel(i, j, Color.White);

            return result;
        }
        public Form1()
        {
            InitializeComponent();
            pictureBox2.Visible = false;
            textBox3.Visible = false;
            button2.Visible = false;
        }
    }
}
