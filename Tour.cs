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
    public partial class Tour : Form
    {
        string roleName;
        public string idTour;
        public string priceTour;
        public string placeInHotel;
        public string dateTourStart;
        string connectionString = DB.connectionString;
        int clickBtn;
        string zapisey;
        public Tour(string role)
        {
            InitializeComponent();
            roleName = role;
        }

        private void Tour_Load(object sender, EventArgs e)
        {
            switch(roleName)
            {
                case "Менеджер":
                    button3.Visible = false;
                    button4.Visible = false;
                    break;
                case "Администратор":
                    button2.Visible = false;
                    break;
            }
            update("");
            textComobox();
            comboBox6.SelectedIndex = 0;
        }

        void update(string filter)
        {
            try
            {
                string query =@"SELECT tour.id, city.name as 'Город отправления', countries.name as 'Страна тура', start_date as 'Дата начала', end_date as 'Дата окончания',
                                    quantity_night as 'Количество ночей', hotel.name as 'Отель', type_food.description as 'Тип питания', type_room.name as 'Тип номера',
                                    placeInRoom as 'Спальных мест',air_travel.name as 'Авиаперелет', tour_operator.name as 'Туроператор', price as 'Цена', status_tour.name as 'Статус тура' FROM tour
                                    JOIN tour_operator ON tour.tour_operator = tour_operator.id
                                    JOIN type_food ON tour.type_food = type_food.id
                                    JOIN type_room ON tour.type_room = type_room.id
                                    JOIN air_travel ON tour.air_travel = air_travel.id
                                    JOIN status_tour ON tour.status = status_tour.id
                                    JOIN hotel ON tour.hotel = hotel.id
                                    JOIN city ON tour.departure_city = city.id
                                    JOIN countries ON tour.country = countries.id";
                if(filter != "")
                {
                    query += filter;
                    if (roleName == "Менеджер")
                    {
                        query += " AND status_tour.name != 'Забронирован' AND status_tour.name != 'Отменен'";
                    }
                }
                else
                {
                    if(roleName == "Менеджер")
                    {
                        query += " WHERE status_tour.name != 'Забронирован' AND status_tour.name != 'Отменен'";
                    }
                }
                if (comboBox6.Text == "По возрастанию")
                {
                    query += " ORDER BY price ASC LIMIT 15";
                }
                else if (comboBox6.Text == "По убыванию")
                {
                    query += " ORDER BY price DESC LIMIT 15";
                }
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns[0].Visible = false;
                    cmd = new MySqlCommand("SELECT count(*) FROM tour", con);
                    label7.Text = $"Количество записей: {dataGridView1.Rows.Count}/{cmd.ExecuteScalar().ToString()}";
                    if(dataGridView1.Rows.Count < 15)
                    {
                        button7.Visible = false;
                    }
                    else if(dataGridView1.Rows.Count == 15)
                    {
                        button7.Visible = true;
                    }
                    button6.Visible = false;
                }
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if(row.Cells["Статус тура"].Value.ToString() == "Отменен")
                    {
                        row.DefaultCellStyle.BackColor = Color.Aquamarine;
                    }
                    if(row.Cells["Статус тура"].Value.ToString() == "Забронирован")
                    {
                        row.DefaultCellStyle.BackColor = Color.CadetBlue;
                    }
                }
                if (dataGridView1.RowCount == 0)
                {
                    MessageBox.Show("По вашим фильтрам ничего не найдено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            clickBtn = 0;
            update(filtraion());
        }

        public string filtraion()
        {
            string filtration = " WHERE";
            List<string> fil = new List<string>();
            if (comboBox1.Text != "Все" && comboBox1.Text != "")
            {
                fil.Add($" city.name = '{comboBox1.Text}'");
            }
            if(comboBox2.Text != "Все" && comboBox2.Text != "")
            {
                fil.Add($" countries.name = '{comboBox2.Text}'");
            }
            if (comboBox3.Text != "Все" && comboBox3.Text != "")
            {
                fil.Add($" type_food.description = '{comboBox3.Text}'");
            }
            if (comboBox4.Text != "Все" && comboBox4.Text != "")
            {
                fil.Add($" type_room.name = '{comboBox4.Text}'");
            }
            if (comboBox5.Text != "Все" && comboBox5.Text != "")
            {
                fil.Add($" tour_operator.name = '{comboBox5.Text}'");
            }
            if(comboBox7.Text != "Все" && comboBox7.Text != "")
            {
                fil.Add($" status_tour.name = '{comboBox7.Text}'");
            }
            if (fil.Count == 0)
            {
                return "";
            }
            else if(fil.Count == 1)
            {
                filtration += fil[0];
                return filtration;
            }
            else
            {
                foreach(var item in fil)
                {
                    if(item == fil[fil.Count-1])
                    {
                        filtration += item;
                    }
                    else
                    {
                        filtration += $"{item} AND";
                    }
                }
                return filtration;
            }
        }

        void textComobox()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = "SELECT distinct city.name FROM tour JOIN city ON tour.departure_city = city.id";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox1.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    query = "SELECT distinct countries.name FROM tour JOIN countries ON tour.country = countries.id";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox2.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    query = "SELECT description FROM type_food";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox3.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    query = "SELECT name FROM type_room";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox4.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    query = "SELECT name FROM tour_operator";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox5.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();

                    query = "SELECT name FROM status_tour";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while(rdr.Read())
                    {
                        comboBox7.Items.Add(rdr[0].ToString());
                    }

                }
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
                comboBox4.SelectedIndex = 0;
                comboBox5.SelectedIndex = 0;
                comboBox7.SelectedIndex = 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            clickBtn = 0;
            update(filtraion());
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {
                update("");
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            clickBtn = 0;
        }

        private void ComboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            clickBtn = 0;
            //if (dataGridView1.Rows.Count != 0)
            //{
            //    update(filtraion());
            //}
            update(filtraion());
        }

        private void ComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            clickBtn = 0;
            update(filtraion());
        }

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            clickBtn = 0;
            update(filtraion());
        }

        private void ComboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            clickBtn = 0;
            update(filtraion());
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Статус тура"].Value.ToString() == "Забронирован")
                {
                    MessageBox.Show("Данный тур забронирован, выберите другой");
                }
                else if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Статус тура"].Value.ToString() == "Отменен")
                {
                    MessageBox.Show("Данный тур отменен, выберите другой");
                }
                else
                {
                    idTour = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["id"].Value.ToString();
                    priceTour = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Цена"].Value.ToString();
                    placeInHotel = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Спальных мест"].Value.ToString();
                    dateTourStart = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Дата начала"].Value.ToString();
                    this.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                workWithTour workWithTour = new workWithTour("", "Добавить", "", "", "", "", "", "", "", "", "", "", "", "", "");
                this.Hide();
                workWithTour.ShowDialog();
                update(filtraion());
                this.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentRow is null)
            {
                MessageBox.Show("Выберите тур");
            }
            else
            {
                string idTour = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["id"].Value.ToString();
                string departureCityTour = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Город отправления"].Value.ToString();
                string countryTour = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Страна тура"].Value.ToString();
                string start_date = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Дата начала"].Value.ToString();
                string end_date = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Дата окончания"].Value.ToString();
                string quantity_night = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Количество ночей"].Value.ToString();
                string type_food = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Тип питания"].Value.ToString();
                string type_room = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Тип номера"].Value.ToString();
                string air_travel = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Авиаперелет"].Value.ToString();
                string tour_operator = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Туроператор"].Value.ToString();
                string price = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Цена"].Value.ToString();
                string status_tour = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Статус тура"].Value.ToString();
                string hotel = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Отель"].Value.ToString();
                string placeInHot = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Спальных мест"].Value.ToString();
                workWithTour workWithTour = new workWithTour(idTour, "Редактировать", departureCityTour, countryTour, start_date, end_date, quantity_night, type_food, type_room, air_travel, tour_operator, price, status_tour, hotel, placeInHot);
                this.Hide();
                workWithTour.ShowDialog();
                update(filtraion());
                this.Show();
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow is null)
            {
                MessageBox.Show("Выберите тур");
            }
            else
            {
                string idTour = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["id"].Value.ToString();
                string departureCityTour = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Город отправления"].Value.ToString();
                string countryTour = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Страна тура"].Value.ToString();
                string start_date = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Дата начала"].Value.ToString();
                string end_date = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Дата окончания"].Value.ToString();
                string quantity_night = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Количество ночей"].Value.ToString();
                string type_food = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Тип питания"].Value.ToString();
                string type_room = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Тип номера"].Value.ToString();
                string air_travel = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Авиаперелет"].Value.ToString();
                string tour_operator = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Туроператор"].Value.ToString();
                string price = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Цена"].Value.ToString();
                string status_tour = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Статус тура"].Value.ToString();
                string hotel = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Отель"].Value.ToString();
                string placeInHot = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Спальных мест"].Value.ToString();
                workWithTour workWithTour = new workWithTour(idTour, "", departureCityTour, countryTour, start_date, end_date, quantity_night, type_food, type_room, air_travel, tour_operator, price, status_tour, hotel, placeInHot);
                this.Hide();
                workWithTour.button1.Visible = false;
                workWithTour.button3.Visible = false;
                workWithTour.button6.Visible = false;
                workWithTour.dateTimePicker1.Enabled = false;
                workWithTour.dateTimePicker2.Enabled = false;
                workWithTour.numericUpDown1.Enabled = false;
                workWithTour.numericUpDown2.Enabled = false;
                workWithTour.comboBox1.Enabled = false;
                workWithTour.comboBox2.Enabled = false;
                workWithTour.comboBox3.Enabled = false;
                workWithTour.comboBox4.Enabled = false;
                workWithTour.textBox4.Enabled = false;
                workWithTour.comboBox5.Enabled = false;
                workWithTour.comboBox6.Enabled = false;
                workWithTour.comboBox7.Enabled = false;
                workWithTour.comboBox8.Enabled = false;
                workWithTour.ShowDialog();
                update(filtraion());
                this.Show();
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($"SELECT max(id) FROM tour", con);
                    zapisey = cmd.ExecuteScalar().ToString();
                }
                if (Convert.ToInt32(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value.ToString()) != Convert.ToInt32(zapisey))
                {
                    clickBtn++;
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        string query = $@"SELECT tour.id, city.name as 'Город отправления', countries.name as 'Страна тура', start_date as 'Дата начала', end_date as 'Дата окончания',
                                    quantity_night as 'Количество ночей', hotel.name as 'Отель', type_food.description as 'Тип питания', type_room.name as 'Тип номера',
                                    placeInRoom as 'Спальных мест',air_travel.name as 'Авиаперелет', tour_operator.name as 'Туроператор', price as 'Цена', status_tour.name as 'Статус тура' FROM tour
                                    JOIN tour_operator ON tour.tour_operator = tour_operator.id
                                    JOIN type_food ON tour.type_food = type_food.id
                                    JOIN type_room ON tour.type_room = type_room.id
                                    JOIN air_travel ON tour.air_travel = air_travel.id
                                    JOIN status_tour ON tour.status = status_tour.id
                                    JOIN hotel ON tour.hotel = hotel.id
                                    JOIN city ON tour.departure_city = city.id
                                    JOIN countries ON tour.country = countries.id";

                        if (filtraion() != "")
                        {
                            query += filtraion();
                            if (roleName == "Менеджер")
                            {
                                query += " AND status_tour.name != 'Забронирован' AND status_tour.name != 'Отменен'";
                            }
                        }
                        else
                        {
                            if (roleName == "Менеджер")
                            {
                                query += " WHERE status_tour.name != 'Забронирован' AND status_tour.name != 'Отменен'";
                            }
                        }
                        if (comboBox6.Text == "По возрастанию")
                        {
                            query += $" ORDER BY price ASC LIMIT {15 * clickBtn},15";
                        }
                        else if (comboBox6.Text == "По убыванию")
                        {
                            query += $" ORDER BY price DESC LIMIT {15 * clickBtn},15";
                        }
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteScalar() != null)
                        {
                            cmd.ExecuteNonQuery();
                            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;
                            if (dataGridView1.Rows.Count != 15)
                            {
                                button7.Visible = false;
                            }
                            dataGridView1.Columns[0].Visible = false;
                            button6.Visible = true;
                            cmd = new MySqlCommand("SELECT count(*) FROM tour", con);
                            label7.Text = $"Количество записей: {dataGridView1.Rows.Count}/{cmd.ExecuteScalar().ToString()}";
                        }
                    }
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["Статус тура"].Value.ToString() == "Отменен")
                        {
                            row.DefaultCellStyle.BackColor = Color.Aquamarine;
                        }
                        if (row.Cells["Статус тура"].Value.ToString() == "Забронирован")
                        {
                            row.DefaultCellStyle.BackColor = Color.CadetBlue;
                        }
                    }

                    if (dataGridView1.RowCount == 0)
                    {
                        MessageBox.Show("По вашим фильтрам ничего не найдено");
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
                if (clickBtn > 0)
                {
                    clickBtn--;
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        string query = $@"SELECT tour.id, city.name as 'Город отправления', countries.name as 'Страна тура', start_date as 'Дата начала', end_date as 'Дата окончания',
                                    quantity_night as 'Количество ночей', hotel.name as 'Отель', type_food.description as 'Тип питания', type_room.name as 'Тип номера',
                                    placeInRoom as 'Спальных мест',air_travel.name as 'Авиаперелет', tour_operator.name as 'Туроператор', price as 'Цена', status_tour.name as 'Статус тура' FROM tour
                                    JOIN tour_operator ON tour.tour_operator = tour_operator.id
                                    JOIN type_food ON tour.type_food = type_food.id
                                    JOIN type_room ON tour.type_room = type_room.id
                                    JOIN air_travel ON tour.air_travel = air_travel.id
                                    JOIN status_tour ON tour.status = status_tour.id
                                    JOIN hotel ON tour.hotel = hotel.id
                                    JOIN city ON tour.departure_city = city.id
                                    JOIN countries ON tour.country = countries.id";
                        if (filtraion() != "")
                        {
                            query += filtraion();
                            if (roleName == "Менеджер")
                            {
                                query += " AND status_tour.name != 'Забронирован' AND status_tour.name != 'Отменен'";
                            }
                        }
                        else
                        {
                            if (roleName == "Менеджер")
                            {
                                query += " WHERE status_tour.name != 'Забронирован' AND status_tour.name != 'Отменен'";
                            }
                        }
                        if (comboBox6.Text == "По возрастанию")
                        {
                            query += $" ORDER BY price ASC LIMIT {15 * clickBtn},15";
                        }
                        else if (comboBox6.Text == "По убыванию")
                        {
                            query += $" ORDER BY price DESC LIMIT {15 * clickBtn},15";
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
                            dataGridView1.Columns[0].Visible = false;
                            cmd = new MySqlCommand("SELECT count(*) FROM tour", con);
                            label7.Text = $"Количество записей: {dataGridView1.Rows.Count}/{cmd.ExecuteScalar().ToString()}";
                        }
                    }
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["Статус тура"].Value.ToString() == "Отменен")
                        {
                            row.DefaultCellStyle.BackColor = Color.Aquamarine;
                        }
                        if (row.Cells["Статус тура"].Value.ToString() == "Забронирован")
                        {
                            row.DefaultCellStyle.BackColor = Color.CadetBlue;
                        }
                    }

                    if (dataGridView1.RowCount == 0)
                    {
                        MessageBox.Show("По вашим фильтрам ничего не найдено");
                    }
                }
                if(clickBtn == 0)
                {
                    button6.Visible = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void ComboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            clickBtn = 0;
            update(filtraion());
        }
    }
}
