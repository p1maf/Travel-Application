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
    public partial class DetailInfo : Form
    {
        public string idTravelPackage;
        public string tourID;
        public string nameClient;
        public string numberPhoneClient;
        string connectionString = DB.connectionString;
        public DetailInfo()
        {
            InitializeComponent();
        }

        private void DetailInfo_Load(object sender, EventArgs e)
        {
            update();
            touristInTravel();
            typePay();
            label7.Text += $" {idTravelPackage}";
            label8.Text += $" {nameClient}";
            label10.Text += $" {numberPhoneClient}";
        }

        void update()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT payment.id, payment.date as 'Дата платежа', amount as 'Платеж', type_payment.name as 'Тип платежа'
                                    FROM payment
                                    JOIN type_payment ON payment.type = type_payment.id
                                    JOIN travel_package ON payment.id_travel = travel_package.id
                                    WHERE travel_package.id = {idTravelPackage}";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns[0].Visible = false;

                    query = $@"SELECT remains FROM payment
                            JOIN travel_package ON payment.id_travel = travel_package.id
                            WHERE travel_package.id = {idTravelPackage} ORDER BY payment.id DESC LIMIT 1";
                    cmd = new MySqlCommand(query, con);
                    if(cmd.ExecuteScalar() != null)
                    {
                        label4.Text = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        void touristInTravel()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT tourist.surname as 'Фамилия', tourist.name as 'Имя', tourist.patronymic as 'Отчество', tourist.birthday as 'Дата рождения'
                                    FROM db_tourism.tourist_travel
                                    JOIN travel_package ON tourist_travel.idtravel = travel_package.id
                                    JOIN tourist ON tourist_travel.idtourist = tourist.id
                                    WHERE idtravel = {idTravelPackage}";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView2.DataSource = dt;
                    dataGridView2.Columns[0].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }


        void typePay()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT name FROM type_payment", con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while(rdr.Read())
                    {
                        comboBox1.Items.Add(rdr[0].ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string idTour = "";
                string departureCityTour = "";
                string countryTour = "";
                string start_date = "";
                string end_date = "";
                string quantity_night = "";
                string type_food = "";
                string type_room = "";
                string air_travel = "";
                string tour_operator = "";
                string price = "";
                string status_tour = "";
                string hotel = "";
                string placeInHot = "";

                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT city.name as 'Город отправления', countries.name as 'Страна тура', start_date as 'Дата начала', end_date as 'Дата окончания',
                                    quantity_night as 'Количество ночей', hotel.name as 'Отель', type_food.description as 'Тип питания', type_room.name as 'Тип номера',
                                    placeInRoom as 'Спальных мест',air_travel.name as 'Авиаперелет', tour_operator.name as 'Туроператор', price as 'Цена', status_tour.name as 'Статус тура' FROM tour
                                    JOIN tour_operator ON tour.tour_operator = tour_operator.id
                                    JOIN type_food ON tour.type_food = type_food.id
                                    JOIN type_room ON tour.type_room = type_room.id
                                    JOIN air_travel ON tour.air_travel = air_travel.id
                                    JOIN status_tour ON tour.status = status_tour.id
                                    JOIN hotel ON tour.hotel = hotel.id
                                    JOIN city ON tour.departure_city = city.id
                                    JOIN countries ON tour.country = countries.id
                                    WHERE tour.id = {tourID}";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        departureCityTour = rdr[0].ToString();
                        countryTour = rdr[1].ToString();
                        start_date = rdr[2].ToString();
                        end_date = rdr[3].ToString();
                        quantity_night = rdr[4].ToString();
                        hotel = rdr[5].ToString();
                        type_food = rdr[6].ToString();
                        type_room = rdr[7].ToString();
                        placeInHot = rdr[8].ToString();
                        air_travel = rdr[9].ToString();
                        tour_operator = rdr[10].ToString();
                        price = rdr[11].ToString();
                        status_tour = rdr[12].ToString();
                    }
                }
                workWithTour workWithTour = new workWithTour("", "", departureCityTour, countryTour, start_date, end_date, quantity_night, type_food, type_room, air_travel, tour_operator, price, status_tour, hotel, placeInHot);
                this.Hide();
                workWithTour.button1.Visible = false;
                workWithTour.button3.Visible = false;
                workWithTour.button5.Visible = true;
                workWithTour.dateTimePicker1.Enabled = false;
                workWithTour.dateTimePicker2.Enabled = false;
                workWithTour.numericUpDown1.Enabled = false;
                workWithTour.numericUpDown2.Enabled = false;
                workWithTour.comboBox1.Enabled = false;
                workWithTour.comboBox2.Enabled = false;
                workWithTour.comboBox3.Enabled = false;
                workWithTour.comboBox4.Enabled = false;
                workWithTour.textBox4.Enabled = false;
                workWithTour.comboBox6.Enabled = false;
                workWithTour.comboBox7.Enabled = false;
                workWithTour.comboBox8.Enabled = false;
                workWithTour.comboBox5.Enabled = false;
                workWithTour.button6.Visible = false;
                workWithTour.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
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

        private void Button2_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(comboBox1.Text) || textBox1.Text == "Сумма платежа")
            {
                MessageBox.Show("Заполните данные");
            }
            else
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        string query = $"INSERT INTO payment VALUES(null, '{DateTime.Now.ToString("yyyy-MM-dd")}', '{textBox1.Text}', '{Convert.ToInt32(label4.Text) - Convert.ToInt32(textBox1.Text)}', (SELECT id FROM type_payment WHERE name = '{comboBox1.Text}'), {idTravelPackage})";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            MessageBox.Show("Оплата успешно добавлена");
                            update();
                            comboBox1.SelectedIndex = -1;
                            textBox1.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка");
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text != "Сумма платежа" && label4.Text != "" && textBox1.Text != "")
            {
                if (Convert.ToInt32(textBox1.Text) > Convert.ToInt32(label4.Text))
                {
                    textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
                    MessageBox.Show("Сумма больше чем остаток");
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text == "Сумма платежа")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                textBox1.Text = "Сумма платежа";
                textBox1.ForeColor = Color.Gray;
            }
        }
    }
}
