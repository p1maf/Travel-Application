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
namespace travel
{
    public partial class ContractLog : Form
    {
        string connectionString = DB.connectionString;
        int clickBtn;
        string zapisey;
        public ContractLog()
        {
            InitializeComponent();
        }

        private void ContractLog_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            update("","", "");
            dateTimePicker1.MinDate = new DateTime(DateTime.Now.Year, 1, 01);
            dateTimePicker1.MaxDate = new DateTime(DateTime.Now.Year, 12, 31);
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DetailInfo detailInfo = new DetailInfo();
            this.Hide();
            detailInfo.idTravelPackage = dataGridView1.Rows[e.RowIndex].Cells["Номер договора"].Value.ToString();
            detailInfo.tourID = dataGridView1.Rows[e.RowIndex].Cells["Тур"].Value.ToString();
            detailInfo.nameClient = dataGridView1.Rows[e.RowIndex].Cells["ФИО клиента"].Value.ToString();
            detailInfo.numberPhoneClient = dataGridView1.Rows[e.RowIndex].Cells["Номер телефона"].Value.ToString();
            detailInfo.ShowDialog();
            this.Show();
        }
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(comboBox1.SelectedIndex == 0)
            {
                var regex = new Regex(@"[^0-9\b]+");
                if (regex.IsMatch(e.KeyChar.ToString()))
                {
                    e.Handled = true;
                }
            }
            else
            {
                var regex = new Regex(@"[^а-яА-Я\b]+");
                if (regex.IsMatch(e.KeyChar.ToString()))
                {
                    e.Handled = true;
                }

                
            }
        }

        void update(string idContract, string surnameClient, string dateChange)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT travel_package.id as 'Номер договора', tour.id as 'Тур', tour.price as 'Цена путевки ₽', concat(tourist.surname, ' ', LEFT(tourist.name, 1), '.', LEFT(tourist.patronymic, 1)) as 'ФИО клиента', tourist.number_phone as 'Номер телефона', concat(users.surname, ' ', users.name, ' ', users.patronymic) as 'ФИО менеджера',
                                    reg_date as 'Дата оформления'
                                    FROM travel_package
                                    JOIN tour ON travel_package.idtour = tour.id
                                    JOIN tourist ON travel_package.idpayer = tourist.id
                                    JOIN users ON travel_package.idmanager = users.id";
                    if(idContract != "")
                    {
                        query += $" WHERE travel_package.id = {idContract}";
                        if(dateChange != "" && checkBox1.Checked == false)
                        {
                            query += $" AND reg_date = '{dateChange}'";
                        }
                    }
                    else if(surnameClient != "")
                    {
                        query += $" WHERE tourist.surname LIKE '{surnameClient}%'";
                        if (dateChange != "" && checkBox1.Checked == false)
                        {
                            query += $" AND reg_date = '{dateChange}'";
                        }
                    }
                    else if(dateChange != "")
                    {
                        query += $" WHERE reg_date = '{dateChange}'";
                    }
                    if(comboBox6.SelectedIndex == 0)
                    {
                        query += " ORDER BY price LIMIT 15";
                    }
                    else if(comboBox6.SelectedIndex == 1)
                    {
                        query += " ORDER BY price DESC LIMIT 15";
                    }
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns[1].Visible = false;
                    label2.Text = $"Количество записей: {dataGridView1.Rows.Count}";
                    if (dataGridView1.Rows.Count <= 14)
                    {
                        button6.Visible = false;
                        button7.Visible = false;
                    }
                    else 
                    {
                        button6.Visible = true;
                        button7.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    update(textBox1.Text, "", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    update("", textBox1.Text, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    update("", "", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
            }
            else
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    update(textBox1.Text, "", "");
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    update("", textBox1.Text, "");
                }
            }
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == false)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    update(textBox1.Text, "", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    update("", textBox1.Text, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    update("", "", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                update(textBox1.Text, "", "");
            }
            else
            {
                update("", textBox1.Text, "");
            }
        }

        private void ComboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    update(textBox1.Text, "", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    update("", textBox1.Text, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    update("", "", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
            }
            else
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    update(textBox1.Text, "", "");
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    update("", textBox1.Text, "");
                }
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == false)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    update(textBox1.Text, "", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    update("", textBox1.Text, dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    update("", "", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                }
            }
            else
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    update(textBox1.Text, "", "");
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    update("", textBox1.Text, "");
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = connectionString;
                con.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT max(id) FROM travel_package", con);
                zapisey = cmd.ExecuteScalar().ToString();
            }
            if (Convert.ToInt32(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Номер договора"].Value.ToString()) != Convert.ToInt32(zapisey))
            {
                clickBtn++;
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT travel_package.id as 'Номер договора', tour.id as 'Тур', tour.price as 'Цена', concat(tourist.surname, ' ', tourist.name, ' ', tourist.patronymic) as 'ФИО клиента', concat(users.surname, ' ', users.name, ' ', users.patronymic) as 'ФИО менеджера',
                                    reg_date as 'Дата оформления'
                                    FROM travel_package
                                    JOIN tour ON travel_package.idtour = tour.id
                                    JOIN tourist ON travel_package.idpayer = tourist.id
                                    JOIN users ON travel_package.idmanager = users.id 
                                    ORDER BY price";

                    if (comboBox6.SelectedIndex == 0)
                    {
                        query += $" ASC LIMIT {15 * clickBtn},15";
                    }
                    else if (comboBox6.SelectedIndex == 1)
                    {
                        query += $" DESC LIMIT {15 * clickBtn},15";
                    }
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    if (cmd.ExecuteScalar() != null)
                    {
                        cmd.ExecuteNonQuery();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                        if(dataGridView1.Rows.Count != 15)
                        {
                            button7.Visible = false;
                        }
                    }
                }
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if (clickBtn > 0)
            {
                clickBtn--;
                using (MySqlConnection con = new MySqlConnection())
                {
                    string query = $@"SELECT travel_package.id as 'Номер договора', tour.id as 'Тур', tour.price as 'Цена', concat(tourist.surname, ' ', tourist.name, ' ', tourist.patronymic) as 'ФИО клиента', concat(users.surname, ' ', users.name, ' ', users.patronymic) as 'ФИО менеджера',
                                    reg_date as 'Дата оформления'
                                    FROM travel_package
                                    JOIN tour ON travel_package.idtour = tour.id
                                    JOIN tourist ON travel_package.idpayer = tourist.id
                                    JOIN users ON travel_package.idmanager = users.id
                                    ORDER BY price";
                    if (comboBox6.SelectedIndex == 0)
                    {
                        query += $" ASC LIMIT {15 * clickBtn},15";
                    }
                    else if (comboBox6.SelectedIndex == 1)
                    {
                        query += $" DESC LIMIT {15 * clickBtn},15";
                    }
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    if (cmd.ExecuteScalar() != null)
                    {
                        cmd.ExecuteNonQuery();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                        if (dataGridView1.Rows.Count == 15)
                        {
                            button7.Visible = true;
                        }
                    }
                }
            }
        }
    }
}
