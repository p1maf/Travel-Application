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
using System.IO;
namespace travel
{
    public partial class Special_Features : Form
    {
        string connectionString = DB.connectionString;
        public Special_Features()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            try
            {
                DialogResult dialogResult = MessageBox.Show("Создать резервную копию БД?", "Сообщение пользователю", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                // если ответ - "ДА"
                if (dialogResult == DialogResult.Yes)
                {

                    using (MySqlConnection conn = new MySqlConnection())
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            conn.ConnectionString = connectionString;
                            //Создание подлкючения
                            conn.Open();
                            cmd.Connection = conn;
                            DataTable dt = GetTable(cmd, "show databases LIKE '%tourism';");

                            //Заполнение таблицы
                            foreach (DataRow dr in dt.Rows)
                            {
                                string db = dr[0] + "";
                                string time2 = DateTime.Now.ToString("yyyy.MM.dd_HH.mm");
                                cmd.CommandText = $"use `{db}`";
                                cmd.ExecuteNonQuery();
                                saveFileDialog1.FileName = $"{db}-{time2}.sql";
                                saveFileDialog1.Filter = "SQL Text File (*.sql)|*.sql";
                                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                                {
                                    MessageBox.Show("Резервная копия успешно создана в: " + saveFileDialog1.FileName, "Резервная копия", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                using (MySqlBackup mb = new MySqlBackup(cmd))
                                {
                                    //Функция импорта
                                    mb.ExportInfo.ExportTableStructure = true;
                                    mb.ExportInfo.ExportRows = true;
                                    mb.ExportToFile(saveFileDialog1.FileName);
                                }
                            }
                            conn.Close();
                        }
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    // сообщение пользователю
                    MessageBox.Show("Создание резервной копии отменено!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        DataTable GetTable(MySqlCommand cmd, string sql)
        {
            //Функция получения таблицы
            cmd.CommandText = sql;
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Восстановить базу данных?", "Сообщение пользователю", MessageBoxButtons.YesNo, MessageBoxIcon.Question); //сообщение пользователю
                if (dialogResult == DialogResult.Yes)
                {
                    //Установки типа файла
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Filter = " SQL Text File (*.sql)|*.sql";
                    openFileDialog1.Title = "Выберите файл";
                    //Установка директории
                    openFileDialog1.InitialDirectory = Environment.CurrentDirectory;

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        //Заполнения переменных и выполнение запроса
                        string fileName = openFileDialog1.FileName;


                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = DB.emptyString;
                            con.Open();
                            MySqlCommand cmd = new MySqlCommand($@"CREATE DATABASE IF NOT EXISTS `db_tourism`;", con);
                            if (cmd.ExecuteNonQuery() == 1)
                            {

                            }
                            else
                            {
                                
                            }
                        }

                        using (MySqlConnection conn = new MySqlConnection())
                        {
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                //Создание подлкючени
                                using (MySqlBackup mb = new MySqlBackup(cmd))
                                {
                                    conn.ConnectionString = connectionString;
                                    conn.Open();
                                    cmd.Connection = conn;
                                    mb.ImportFromFile(fileName);
                                }
                                conn.Close();
                            }
                        }
                        MessageBox.Show("Восстановление бд успешно!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Special_Features_Load(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT table_name FROM information_schema.tables WHERE table_schema = 'db_tourism';";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox1.Items.Add(rdr[0].ToString());  
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Выберите таблицу");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Составить экспорт?", "Сообщение пользователю", MessageBoxButtons.YesNo, MessageBoxIcon.Question); //сообщение пользователю
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        //Тип файла
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.FileName = "";
                        saveFileDialog1.Filter = "Файл Microsoft Excel, содержащий значения, разделенные запятыми (*.csv)|*.csv";

                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            string filesCVS = "";
                            using (MySqlConnection con = new MySqlConnection())
                            {
                                con.ConnectionString = connectionString;
                                con.Open();
                                string query = $@"SELECT * FROM {comboBox1.Text};";
                                MySqlCommand cmd = new MySqlCommand(query, con);
                                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                da.Fill(dt);

                                query = $@"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{comboBox1.Text}' AND TABLE_SCHEMA = 'db_tourism' ORDER BY ORDINAL_POSITION ASC";
                                cmd = new MySqlCommand(query, con);
                                MySqlDataReader rdr = cmd.ExecuteReader();
                                while (rdr.Read())
                                {
                                    filesCVS += $"{rdr[0].ToString()};";
                                }
                                filesCVS += "\n";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {

                                    for (int a = 0; a < dt.Columns.Count; a++)
                                    {
                                        string str = dt.Rows[i][a].ToString() + ";";
                                        filesCVS += str;
                                    }

                                    filesCVS += "\t\n";
                                }
                                StreamWriter wr = new StreamWriter(saveFileDialog1.FileName, false, Encoding.GetEncoding("UTF-8"));
                                //Вывод документа
                                wr.Write(filesCVS);
                                wr.Close();

                                MessageBox.Show("Успешно!");
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

        private void Button5_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Выберите таблицу");
            }
            else
            {
                DialogResult dialogResult1 = MessageBox.Show("Составить импорт?", "Сообщение пользователю", MessageBoxButtons.YesNo, MessageBoxIcon.Question); //сообщение пользователю
                if (dialogResult1 == DialogResult.Yes)
                {
                    openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();

                    try
                    {
                        OpenFileDialog openFileDialog1 = new OpenFileDialog();
                        openFileDialog1.DefaultExt = ".csv";
                        openFileDialog1.Filter = "Файл Microsoft Excel, содержащий значения, разделенные запятыми (*.csv)|*.csv";

                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            //Путь до и название выбранного файла
                            string pathToFile = openFileDialog1.FileName; // full path
                            string fileName = openFileDialog1.SafeFileName.Split('.')[0];

                            DialogResult dialogResult = MessageBox.Show($"Импортировать данные в таблицу {comboBox1.Text}?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            string[] readText = File.ReadAllLines(pathToFile); // array lines from files
                            string[] valField;
                            string[] words = { "" };
                            string[] titleField = readText[0].Split(';'); // first line - Title
                            titleField = titleField.Except(words).ToArray();
                            string strCmd = $"INSERT INTO {comboBox1.Text}({String.Join(",", titleField)}) VALUES";
                            foreach (string str in readText.Skip(1).ToArray())// skip first row in CSV files
                            {
                                strCmd += "(";
                                valField = str.Split(';');
                                for(int i = 0; i < valField.Length; i++)
                                {
                                    DateTime temp;
                                    if (DateTime.TryParse(valField[i], out temp))
                                    {
                                        strCmd += $" '{Convert.ToDateTime(valField[i]).ToString("yyyy-MM-dd")}',";
                                    }
                                    else if (valField[i] == "\t")
                                    {
                                        strCmd = strCmd.Substring(0, strCmd.Length - 1) + "),";
                                    }
                                    else
                                    {
                                        strCmd += $" '{valField[i]}',";
                                    }
                                }
                            }
                            strCmd = strCmd.Substring(0, strCmd.Length - 1) + ";";
                            using (MySqlConnection con = new MySqlConnection())
                            {
                                con.ConnectionString = connectionString;
                                con.Open();       
                                MySqlCommand cmd = new MySqlCommand(strCmd, con);
                                if(cmd.ExecuteScalar() == null)
                                {
                                    MessageBox.Show("Таблица была успешно импортирована");
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
    }
}
