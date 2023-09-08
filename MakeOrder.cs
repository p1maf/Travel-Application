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
    public partial class MakeOrder : Form
    {
        int quantity_people = 1;
        string tourID;
        string tourPrice;
        string connectionString = DB.connectionString;
        string nameManager;
        string lastID;
        string idTourist;
        string placeInHotelTour;
        string dateTour;
        Dictionary<int, Person> dicPerson = new Dictionary<int, Person>();
        public MakeOrder(string idTour, string priceTour, string role, string nameUser)
        {
            InitializeComponent();
            tourID = idTour;
            tourPrice = priceTour;
            nameManager = nameUser;
            label2.Visible = false;
            button5.Visible = false;
            label4.Visible = false;
            label7.Visible = false;
            comboBox2.Visible = false;
            textBox1.Visible = false;
            label9.Visible = false;
        }

        // Добавление клиента
        private void Button1_Click(object sender, EventArgs e)
        {
            if(tourID == "")
            {
                MessageBox.Show("Перед добавлением клиента выберите тур");
            }
            else
            {
                try
                {
                    if(dataGridView1.Rows.Count == Convert.ToInt32(placeInHotelTour))
                    {
                        MessageBox.Show($"Нельзя добавить больше {placeInHotelTour} людей в путевку","Сообщение");
                    }
                    else
                    {
                        checkingTourist checkingTourist = new checkingTourist();
                        this.Hide();
                        checkingTourist.ShowDialog();
                        idTourist = checkingTourist.idTourist;
                        string serialTourist = checkingTourist.serial;
                        string numberTourist = checkingTourist.number;
                        this.Show();
                        if (idTourist == "")
                        {
                            workWithTourist workWithTourist = new workWithTourist("Добавить", serialTourist, numberTourist);
                            this.Hide();
                            workWithTourist.ShowDialog();
                            if (string.IsNullOrWhiteSpace(workWithTourist.textBox1.Text) || string.IsNullOrWhiteSpace(workWithTourist.textBox2.Text) || /*string.IsNullOrWhiteSpace(workWithTourist.textBox3.Text) ||*/ string.IsNullOrWhiteSpace(workWithTourist.textBox4.Text) || !workWithTourist.maskedTextBox2.MaskCompleted || !workWithTourist.maskedTextBox3.MaskCompleted ||
                                string.IsNullOrWhiteSpace(workWithTourist.textBox6.Text) || !workWithTourist.maskedTextBox1.MaskCompleted)
                            {
                                this.Show();
                            }
                            else
                            {
                                dataGridView1.Rows.Add(workWithTourist.textBox1.Text, workWithTourist.textBox2.Text, workWithTourist.textBox3.Text, workWithTourist.dateTimePicker1.Text, workWithTourist.maskedTextBox2.Text, workWithTourist.maskedTextBox3.Text, workWithTourist.textBox6.Text, workWithTourist.maskedTextBox1.Text.Replace("-", string.Empty), workWithTourist.textBox4.Text);
                                this.Show();
                                comboBox1.Items.Add($"{workWithTourist.textBox2.Text} {workWithTourist.textBox1.Text} {workWithTourist.textBox3.Text}");
                                Person person = new Person()
                                {
                                    Name = workWithTourist.textBox1.Text,
                                    Surname = workWithTourist.textBox2.Text,
                                    Patronymic = workWithTourist.textBox3.Text,
                                    Birthday = workWithTourist.dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                                    Serial_passport = workWithTourist.maskedTextBox2.Text,
                                    Number_passport = workWithTourist.maskedTextBox3.Text,
                                    Whom_issued = workWithTourist.textBox6.Text,
                                    Number_phone = workWithTourist.maskedTextBox1.Text,
                                    Address = workWithTourist.textBox4.Text
                                };
                                //dicPerson.Add(dataGridView1.Rows.Count - 1, person);
                                if (dataGridView1.Rows.Count != 0)
                                {
                                    quantity_people = dataGridView1.Rows.Count;
                                }
                                if (tourPrice != "")
                                {
                                    quantity_people = dataGridView1.Rows.Count;
                                    label3.Text = $"{Convert.ToInt32(tourPrice)}";
                                    label6.Text = $"{Convert.ToInt32(label3.Text) - Convert.ToInt32(textBox1.Text)}";
                                }
                                button5.Visible = true;
                                button7.Visible = true;
                            }
                        }
                        else if (idTourist != null)
                        {
                            button5.Visible = true;
                            button7.Visible = true;
                            bool have = false;
                            using (MySqlConnection con = new MySqlConnection())
                            {
                                con.ConnectionString = connectionString;
                                con.Open();
                                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM tourist WHERE id = {idTourist}", con);
                                MySqlDataReader rdr = cmd.ExecuteReader();
                                while (rdr.Read())
                                {
                                    foreach (DataGridViewRow row in dataGridView1.Rows)
                                    {
                                        if (row.Cells["serial_passport"].Value.ToString() == rdr[5].ToString() && row.Cells["number_passport"].Value.ToString() == rdr[6].ToString())
                                        {
                                            MessageBox.Show("Данный клиент уже есть в таблице");
                                            have = true;
                                        }
                                    }
                                    if (have == false)
                                    {

                                        dataGridView1.Rows.Add(rdr[1].ToString(), rdr[2].ToString(), rdr[3].ToString(), rdr[4].ToString(), rdr[5].ToString(), rdr[6].ToString(), rdr[7].ToString(), rdr[8].ToString(), rdr[9].ToString());
                                        comboBox1.Items.Add($"{rdr[2].ToString()} {rdr[1].ToString()} {rdr[3].ToString()}");
                                        Person person = new Person()
                                        {
                                            Name = rdr[1].ToString(),
                                            Surname = rdr[2].ToString(),
                                            Patronymic = rdr[3].ToString(),
                                            Birthday = rdr[4].ToString(),
                                            Serial_passport = rdr[5].ToString(),
                                            Number_passport = rdr[6].ToString(),
                                            Whom_issued = rdr[7].ToString(),
                                            Number_phone = rdr[8].ToString(),
                                            Address = rdr[9].ToString()
                                        };
                                        //dicPerson.Add(dataGridView1.Rows.Count - 1, person);
                                        if (dataGridView1.Rows.Count != 0)
                                        {
                                            quantity_people = dataGridView1.Rows.Count;
                                        }
                                        if (tourPrice != "")
                                        {
                                            quantity_people = dataGridView1.Rows.Count;
                                            label3.Text = $"{Convert.ToInt32(tourPrice)}";
                                            textBox1.Text = $"{Convert.ToInt32(Convert.ToInt32(label3.Text) * 0.2)}";
                                            label6.Text = $"{Convert.ToInt32(label3.Text) - Convert.ToInt32(textBox1.Text)}";
                                        }
                                    }
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

        // Оформление путевки
        private void Button2_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(comboBox1.Text) || dataGridView1.RowCount == 0 || tourID == "" || string.IsNullOrWhiteSpace(comboBox2.Text) || dataGridView1.Rows.Count > Convert.ToInt32(placeInHotelTour))
            {
                MessageBox.Show("Не выбран плательщик, не добавлен ни один клиент, не выбран тур или не выбран способ оплаты, клиентов больше чем спальных мест");
            }
            else
            {

                try
                {
                    string name = "";
                    string surname = "";
                    string patronymic = "";
                    string birthday = "";
                    string serial_passport = "";
                    string number_passport = "";
                    string whom_issued = "";
                    string number_phone = "";
                    string hotelName = "";
                    string countryTour = "";
                    string cityDepart = "";
                    string address = "";
                    string numberTravel = "";
                    foreach (DataGridViewRow person in dataGridView1.Rows)
                    {
                        name = person.Cells["name"].Value.ToString();
                        surname = person.Cells["surname"].Value.ToString();
                        patronymic = person.Cells["patronymic"].Value.ToString();
                        birthday = person.Cells["birthday"].Value.ToString();
                        serial_passport = person.Cells["serial_passport"].Value.ToString();
                        number_passport = person.Cells["number_passport"].Value.ToString();
                        whom_issued = person.Cells["whom_issued"].Value.ToString();
                        number_phone = person.Cells["number_phone"].Value.ToString();
                        address = person.Cells["address"].Value.ToString();
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $@"SELECT * FROM tourist WHERE number_passport = '{number_passport}' and serial_passport = '{serial_passport}'";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteScalar() != null)
                            {
                                
                            }
                            else
                            {
                                query = $@"INSERT INTO tourist VALUES (null, '{name.Trim()}', '{surname.Trim()}', '{patronymic.Trim()}', '{DateTime.Parse(birthday).ToString("yyyy-MM-dd")}', '{serial_passport}', '{number_passport}','{whom_issued}', '{number_phone.Replace("-", string.Empty)}', '{address}')";

                                cmd = new MySqlCommand(query, con);
                                if (cmd.ExecuteNonQuery() == 1)
                                {
                                    
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка");
                                }
                            }
                        }
                    }

                    using (MySqlConnection con = new MySqlConnection())
                    {
                        number_passport = "";
                        serial_passport = "";
                        number_passport = dataGridView1.Rows[comboBox1.SelectedIndex].Cells["number_passport"].Value.ToString();
                        serial_passport = dataGridView1.Rows[comboBox1.SelectedIndex].Cells["serial_passport"].Value.ToString();

                        con.ConnectionString = connectionString;
                        con.Open();
                        string query = $@"SELECT id FROM tourist WHERE number_passport = '{number_passport}' and serial_passport = '{serial_passport}'";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        string idPayer = cmd.ExecuteScalar().ToString();
                        query = $@"INSERT INTO travel_package VALUES (null, '{DateTime.Now.ToString("yyyy-MM-dd")}' ,{tourID}, {idPayer}, (SELECT id FROM users WHERE login = '{nameManager}'))";
                        cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            numberTravel = cmd.LastInsertedId.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка");
                        }

                        query = $@"SELECT max(id) FROM travel_package";
                        cmd = new MySqlCommand(query, con);
                        lastID = cmd.ExecuteScalar().ToString();

                        foreach(DataGridViewRow person in dataGridView1.Rows)
                        {
                            string newSerial = "";
                            string newNumber = "";
                            if(person.Cells["number_passport"].Value.ToString() != number_passport && person.Cells["serial_passport"].Value.ToString() != serial_passport)
                            {
                                newSerial = person.Cells["serial_passport"].Value.ToString();
                                newNumber = person.Cells["number_passport"].Value.ToString();
                                query = $@"SELECT id FROM tourist WHERE number_passport = '{newNumber}' and serial_passport = '{newSerial}'";
                                cmd = new MySqlCommand(query, con);
                                string idTourist = cmd.ExecuteScalar().ToString();

                                query = $"INSERT INTO tourist_travel VALUES (null, {lastID}, {idTourist})";
                                cmd = new MySqlCommand(query, con);
                                if (cmd.ExecuteNonQuery() == 1)
                                {

                                }
                                else
                                {
                                    MessageBox.Show("Ошибка");
                                }
                            }
                        }
                    }

                    foreach (DataGridViewRow person in dataGridView1.Rows)
                    {
                        if (person.Index == comboBox1.SelectedIndex)
                        {
                            name = person.Cells["name"].Value.ToString();
                            surname = person.Cells["surname"].Value.ToString();
                            patronymic = person.Cells["patronymic"].Value.ToString();
                            birthday = person.Cells["birthday"].Value.ToString();
                            serial_passport = person.Cells["serial_passport"].Value.ToString();
                            number_passport = person.Cells["number_passport"].Value.ToString();
                            whom_issued = person.Cells["whom_issued"].Value.ToString();
                            number_phone = person.Cells["number_phone"].Value.ToString();
                            address = person.Cells["address"].Value.ToString();
                        }
                    }

                    try
                    {
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $"INSERT INTO payment VALUES (null, '{DateTime.Now.ToString("yyyy-MM-dd")}', {textBox1.Text}, {label6.Text}, (SELECT id FROM type_payment WHERE name = '{comboBox2.Text}'), {lastID})";
                            MySqlCommand cmd = new MySqlCommand(query,con);
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                MessageBox.Show("Путевка успешно оформлена");
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


                    try
                    {
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $@"SELECT countries.name FROM tour JOIN countries ON tour.country = countries.id WHERE tour.id = {tourID}";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            countryTour = cmd.ExecuteScalar().ToString();
                            query = $@"SELECT city.name FROM tour JOIN city ON tour.departure_city = city.id WHERE tour.id = {tourID}";
                            cmd = new MySqlCommand(query,con);
                            cityDepart = cmd.ExecuteScalar().ToString();

                            query = $@"SELECT hotel.name FROM tour JOIN hotel ON tour.hotel = hotel.id WHERE tour.id = {tourID}";
                            cmd = new MySqlCommand(query, con);
                            hotelName = cmd.ExecuteScalar().ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка");
                    }

                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand($"UPDATE tour SET status = 2 WHERE id = {tourID}",con);
                        cmd.ExecuteNonQuery();
                    }

                    WordHelper wordHelper = new WordHelper("blank-dogovora-okazanija-turisticheskih-uslug.docx");

                    var items = new Dictionary<string, string>
                    {
                        { "<DOGOVOR>", $"{numberTravel}" },
                        { "<CITY>", $"{cityDepart}" },
                        { "<DATA>", DateTime.Now.ToString("yyyy-MM-dd") },
                        { "<FIO>", comboBox1.Text },
                        { "<BIRTHDAY>", birthday },
                        { "<ADDRESS>", address },
                        { "<SERIAL>", serial_passport },
                        { "<NUMBER>", number_passport },
                        { "<WHOM_ISSUED>", whom_issued },
                        { "<HOTEL>", hotelName },
                        { "<COUNTRY>", $"{countryTour}" },
                        { "<PRICE>",  tourPrice},
                    };
                    bool pechat = false;
                    DialogResult result = MessageBox.Show(
                    "Вывести документ на печать?",
                    "Сообщение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        pechat = true;
                    }
                    wordHelper.Process(items, numberTravel, "",pechat);
                    this.Close();
            }
                catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        }

        private void Button4_Click(object sender, EventArgs e)
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

        private void Button3_Click(object sender, EventArgs e)
        {
            Tour tour = new Tour("Менеджер");
            this.Hide();
            tour.button5.Visible = false;
            tour.button8.Visible = true;
            tour.label8.Visible = false;
            tour.comboBox7.Visible = false;
            tour.ShowDialog();
            tourID = tour.idTour;
            tourPrice = tour.priceTour;
            placeInHotelTour = tour.placeInHotel;
            dateTour = tour.dateTourStart;
            label2.Visible = true;
            label4.Visible = true;
            label7.Visible = true;
            textBox1.Visible = true;
            label9.Visible = true;
            comboBox2.Visible = true;
            button6.Visible = true;
            if (dataGridView1.Rows.Count != 0)
            {
                quantity_people = dataGridView1.Rows.Count;
            }
            if(DateTime.Now.AddDays(21) < DateTime.Parse(dateTour))
            {
                tourPrice = $"{Convert.ToInt32(tourPrice) - Convert.ToInt32(Convert.ToInt32(tourPrice) * 0.1)}";
            }
            label3.Text = $"{Convert.ToInt32(tourPrice)}";
            textBox1.Text = $"{Convert.ToInt32(Convert.ToInt32(label3.Text) * 0.2)}";
            label6.Text = $"{Convert.ToInt32(label3.Text) - Convert.ToInt32(textBox1.Text)}";
            this.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            workWithTourist workWithTourist = new workWithTourist("Редактировать","","");
            workWithTourist.textBox1.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["name"].Value.ToString();
            workWithTourist.textBox2.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["surname"].Value.ToString();
            workWithTourist.textBox3.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["patronymic"].Value.ToString();
            workWithTourist.dateTimePicker1.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["birthday"].Value.ToString();
            workWithTourist.maskedTextBox2.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["serial_passport"].Value.ToString();
            workWithTourist.maskedTextBox3.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["number_passport"].Value.ToString();
            workWithTourist.textBox6.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["whom_issued"].Value.ToString();
            workWithTourist.maskedTextBox1.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["number_phone"].Value.ToString();
            workWithTourist.textBox4.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["address"].Value.ToString();
            this.Hide();
            workWithTourist.ShowDialog();
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value = workWithTourist.textBox1.Text;
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value = workWithTourist.textBox2.Text;
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value = workWithTourist.textBox3.Text;
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value = workWithTourist.dateTimePicker1.Text;
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value = workWithTourist.maskedTextBox2.Text;
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value = workWithTourist.maskedTextBox3.Text;
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[6].Value = workWithTourist.textBox6.Text;
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[7].Value = workWithTourist.maskedTextBox1.Text;
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[8].Value = workWithTourist.textBox4.Text;
            this.Show();
        }

        private void MakeOrder_Load(object sender, EventArgs e)
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
                        comboBox2.Items.Add(rdr[0].ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            try
            {
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

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                if (Convert.ToInt32(textBox1.Text) > Convert.ToInt32(label3.Text))
                {
                    textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
                    MessageBox.Show("Взнос больше чем цена тура");
                }
                else
                {
                    label6.Text = $"{Convert.ToInt32(label3.Text) - Convert.ToInt32(textBox1.Text)}";
                }
            }
            if(textBox1.Text == "")
            {
                textBox1.Text = $"{Convert.ToInt32(Convert.ToInt32(label3.Text) * 0.2)}";
            }
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox1.Text) < Convert.ToInt32(Convert.ToInt32(label3.Text) * 0.2))
            {
                textBox1.Text = $"{Convert.ToInt32(Convert.ToInt32(label3.Text) * 0.2)}";
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(
                            "Вы точно хотите удалить клиента?",
                            "Сообщение",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    comboBox1.Items.RemoveAt(dataGridView1.CurrentRow.Index);
                    dataGridView1.Rows.Remove(dataGridView1.Rows[dataGridView1.CurrentRow.Index]);
                    if(dataGridView1.Rows.Count == 0)
                    {
                        button7.Visible = false;
                        button5.Visible = false;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
    }

    class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Birthday { get; set; }
        public string Serial_passport { get; set; }
        public string Number_passport { get; set; }
        public string Whom_issued { get; set; }
        public string Number_phone { get; set; }

        public string Address { get; set; }
    }
}
