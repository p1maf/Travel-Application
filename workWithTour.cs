using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace travel
{
    public partial class workWithTour : Form
    {
        int idSelectPhoto = 1;
        string connectionString = DB.connectionString;
        public string tourId;
        string nameHotel, statusTour, placeHotel;
        public workWithTour(string idTour = "",string nameButton = "", string departure_city = "", string country = "", string start_date = "", string end_date = "", string quantity_night = "", string type_food = "", string type_room = "", string air_travel = "", string tour_operator = "", string price = "", string status = "", string hotel = "", string placeHotel = "")
        {
            InitializeComponent();
            comboboxFill();
            button1.Text = nameButton;
            statusTour = status;
            if (nameButton == "Добавить")
            {
                comboBox5.SelectedIndex = 0;
                comboBox5.Enabled = false;
                dateTimePicker1.MinDate = DateTime.Now;
            }
            else if(nameButton == "Редактировать")
            {
                if(statusTour == "Забронирован")
                {
                    comboBox5.Items.RemoveAt(0);
                    dateTimePicker1.Enabled = false;
                    dateTimePicker2.Enabled = false;
                    numericUpDown1.Enabled = false;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    textBox4.Enabled = false;
                    comboBox6.Enabled = false;
                    comboBox7.Enabled = false;
                    comboBox8.Enabled = false;
                }
                else if(statusTour == "Новый")
                {
                    comboBox5.Items.RemoveAt(1);
                }
                else if(statusTour == "Отменен")
                {
                    comboBox5.Items.RemoveAt(1);
                }
            }
            tourId = idTour;
            comboBox6.Text = departure_city;
            comboBox7.Text = country;
            comboBox8.Text = hotel;
            comboBox1.Text = type_food;
            comboBox2.Text = type_room;
            dateTimePicker1.Text = start_date;
            dateTimePicker2.Text = end_date;
            nameHotel = hotel;
            if(quantity_night == "")
            {
                numericUpDown1.Value = numericUpDown1.Minimum;
            }
            else
            {
                numericUpDown1.Value = Convert.ToDecimal(quantity_night);
            }
            if(placeHotel == "")
            {
                numericUpDown1.Value = numericUpDown1.Minimum;
            }
            else
            {
                numericUpDown2.Value = Convert.ToDecimal(placeHotel);
            }
            comboBox3.Text = air_travel;
            comboBox4.Text = tour_operator;
            textBox4.Text = price;
            comboBox5.Text = status;
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void WorkWithTour_Load(object sender, EventArgs e)
        {
            try
            {
                if(Directory.Exists($"{Application.StartupPath}\\TourImage\\{nameHotel}"))
                {
                    DirectoryInfo di = new DirectoryInfo($"{Application.StartupPath}\\TourImage\\{nameHotel}");
                    int fcount = di.GetFiles($"{nameHotel}_*", SearchOption.TopDirectoryOnly).Length;
                    if (fcount == 0)
                    {
                        using (var stream = new FileStream($"{Application.StartupPath}\\TourImage\\picture.png", FileMode.Open))
                        {
                            pictureBox1.Image = Image.FromStream(stream);
                        }
                        button2.Visible = false;
                        button6.Visible = false;
                        button4.Visible = false;
                    }
                    else if (fcount == 1)
                    {
                        button2.Visible = false;
                        button4.Visible = false;
                        foreach (var item in di.GetFiles($"{nameHotel}_1*", SearchOption.TopDirectoryOnly))
                        {
                            using (var stream = new FileStream($"{Application.StartupPath}\\TourImage\\{nameHotel}\\{item}", FileMode.Open))
                            {
                                pictureBox1.Image = Image.FromStream(stream);
                            }
                        }
                    }
                    else
                    {
                        button2.Visible = false;
                        foreach (var item in di.GetFiles($"{nameHotel}_1*", SearchOption.TopDirectoryOnly))
                        {
                            using (var stream = new FileStream($"{Application.StartupPath}\\TourImage\\{nameHotel}\\{item}", FileMode.Open))
                            {
                                pictureBox1.Image = Image.FromStream(stream);
                            }
                        }
                    }
                }
                else
                {
                    using (var stream = new FileStream($"{Application.StartupPath}\\TourImage\\picture.png", FileMode.Open))
                    {
                        pictureBox1.Image = Image.FromStream(stream);
                    }
                    button2.Visible = false;
                    button4.Visible = false;
                    button6.Visible = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        void comboboxFill()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = "SELECT name FROM air_travel";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox3.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    query = "SELECT name FROM tour_operator";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox4.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();
                    query = "SELECT name FROM status_tour";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox5.Items.Add(rdr[0].ToString());
                    }

                    rdr.Close();
                    var values = new AutoCompleteStringCollection();
                    query = "SELECT name FROM city";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox6.Items.Add(rdr[0].ToString());
                        values.Add(rdr[0].ToString());
                    }
                    comboBox6.AutoCompleteCustomSource = values;
                    comboBox6.AutoCompleteMode = AutoCompleteMode.Suggest;
                    comboBox6.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    rdr.Close();
                    values = new AutoCompleteStringCollection();
                    query = "SELECT name FROM countries";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox7.Items.Add(rdr[0].ToString());
                        values.Add(rdr[0].ToString());
                    }
                    comboBox7.AutoCompleteCustomSource = values;
                    comboBox7.AutoCompleteMode = AutoCompleteMode.Suggest;
                    comboBox7.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            bool filledTextBox = this.Controls.OfType<TextBox>().All(textBox => textBox.Text != "");
            bool filledComboBox = this.Controls.OfType<ComboBox>().All(comboBox => comboBox.Text != "");
            bool filledNumeric = this.Controls.OfType<NumericUpDown>().All(NumericUpDown => NumericUpDown.Text != "");

            if(filledTextBox == false || filledComboBox == false || filledNumeric == false)
            {
                MessageBox.Show("Заполните поля");
            }
            else
            {
                if (button1.Text == "Добавить")
                {
                    try
                    {
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $@"SELECT id FROM city WHERE name = '{comboBox6.Text.Trim()}'";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            var idCity = cmd.ExecuteScalar();
                            if(idCity != null)
                            {
                                query = $@"SELECT id FROM countries WHERE name = '{comboBox7.Text.Trim()}'";
                                cmd = new MySqlCommand(query, con);
                                var idCountry = cmd.ExecuteScalar();
                                if(idCountry != null)
                                {
                                    query = $"INSERT INTO tour VALUES(null, '{idCity}', '{idCountry}', '{dateTimePicker1.Value.ToString("yyyy-MM-dd")}', '{dateTimePicker2.Value.ToString("yyyy-MM-dd")}', '{numericUpDown1.Value}', (SELECT id FROM hotel WHERE name = '{comboBox8.Text.Trim()}'), (SELECT id FROM type_food WHERE description = '{comboBox1.Text}'), (SELECT id FROM type_room WHERE name = '{comboBox2.Text}'), '{numericUpDown2.Value}', (SELECT id FROM air_travel WHERE name = '{comboBox3.Text}'), (SELECT id FROM tour_operator WHERE name = '{comboBox4.Text}'), {textBox4.Text}, 1)";
                                    cmd = new MySqlCommand(query, con);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {
                                        MessageBox.Show("Тур успешно добавлен");
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
                    catch(Exception ex)
                    {
                        MessageBox.Show("Ошибка");
                    }
                }
                else if(button1.Text == "Редактировать")
                {
                    try
                    {
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $@"SELECT id FROM city WHERE name = '{comboBox6.Text.Trim()}'";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            var idCity = cmd.ExecuteScalar();
                            if (idCity != null)
                            {
                                query = $@"SELECT id FROM countries WHERE name = '{comboBox7.Text.Trim()}'";
                                cmd = new MySqlCommand(query, con);
                                var idCountry = cmd.ExecuteScalar();
                                if (idCountry != null)
                                {
                                    DialogResult result = MessageBox.Show(
                                    "Вы точно хотите редактировать запись?",
                                    "Сообщение",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information,
                                    MessageBoxDefaultButton.Button1);
                                    if(result == DialogResult.Yes)
                                    {
                                        query = $"UPDATE tour SET departure_city = '{idCity}', country = '{idCountry}', start_date = '{dateTimePicker1.Value.ToString("yyyy-MM-dd")}', end_date = '{dateTimePicker2.Value.ToString("yyyy-MM-dd")}', quantity_night = '{numericUpDown1.Value}',hotel = (SELECT id FROM hotel WHERE name = '{comboBox8.Text.Trim()}'), type_food = (SELECT id FROM type_food WHERE description = '{comboBox1.Text}'), type_room = (SELECT id FROM type_room WHERE name = '{comboBox2.Text}'), placeInRoom = '{numericUpDown2.Value}' ,air_travel = (SELECT id FROM air_travel WHERE name = '{comboBox3.Text}'), tour_operator = (SELECT id FROM tour_operator WHERE name = '{comboBox4.Text}'), price = {textBox4.Text}, status = (SELECT id FROM status_tour WHERE name = '{comboBox5.Text}') WHERE id = {tourId}";
                                        cmd = new MySqlCommand(query, con);
                                        if (cmd.ExecuteNonQuery() == 1)
                                        {
                                            MessageBox.Show("Тур успешно отредактирован");
                                            if(comboBox5.Text == "Отменен" && statusTour != "Новый")
                                            {
                                                string idTravelPackage = "";
                                                string namePayer = "";
                                                string surnamePayer = "";
                                                string patronymicPayer = "";
                                                string birthdayPayer = "";
                                                string serialPayer = "";
                                                string numberPayer = "";
                                                int summ = 0;
                                                query = $@"SELECT id FROM travel_package WHERE idtour = {tourId}";
                                                cmd = new MySqlCommand(query, con);
                                                idTravelPackage = cmd.ExecuteScalar().ToString();

                                                query = $@"SELECT tourist.name, tourist.surname, tourist.patronymic, tourist.birthday, tourist.serial_passport, tourist.number_passport FROM travel_package JOIN tourist ON travel_package.idpayer = tourist.id WHERE idtour = {tourId}";
                                                cmd = new MySqlCommand(query, con);
                                                MySqlDataReader rdr = cmd.ExecuteReader();
                                                while(rdr.Read())
                                                {
                                                    namePayer = rdr[0].ToString();
                                                    surnamePayer = rdr[1].ToString();
                                                    patronymicPayer = rdr[2].ToString();
                                                    birthdayPayer = rdr[3].ToString();
                                                    serialPayer = rdr[4].ToString();
                                                    numberPayer = rdr[5].ToString();
                                                }
                                                rdr.Close();
                                                query = $@"DELETE FROM tourist_travel WHERE idtravel = {idTravelPackage}";
                                                cmd = new MySqlCommand(query, con);
                                                cmd.ExecuteNonQuery();

                                                query = $@"SELECT amount FROM payment WHERE id_travel = {idTravelPackage}";
                                                cmd = new MySqlCommand(query, con);
                                                rdr = cmd.ExecuteReader();
                                                while(rdr.Read())
                                                {
                                                    summ += Convert.ToInt32(rdr[0].ToString());
                                                }
                                                rdr.Close();

                                                query = $@"DELETE FROM payment WHERE id_travel = {idTravelPackage}";
                                                cmd = new MySqlCommand(query, con);
                                                cmd.ExecuteNonQuery();

                                                query = $@"DELETE FROM travel_package WHERE idtour = {tourId}";
                                                cmd = new MySqlCommand(query, con);
                                                cmd.ExecuteNonQuery();
                                                WordHelper wordHelper = new WordHelper("blank-vosvrata.docx");

                                                var items = new Dictionary<string, string>
                                                {
                                                    { "<DOGOVOR>", $"{tourId}" },
                                                    { "<DATE>", $"{DateTime.Now.ToShortDateString()}" },
                                                    { "<FIO>", $"{surnamePayer} {namePayer} {patronymicPayer}" },
                                                    { "<SERIAL>", $"{serialPayer}" },
                                                    { "<NUMBER>", $"{numberPayer}" },
                                                    { "<BIRTHDAY>", $"{DateTime.Parse(birthdayPayer).ToString("yyyy-MM-dd")}" },
                                                    { "<SUM>", $"{summ}" },
                                                    
                                                };

                                                wordHelper.Process(items, "",tourId, false);
                                                this.Close();
                                            }
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
                                    MessageBox.Show("Страна введена неверно");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Город введен неверно");
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
            var regex = new Regex(@"[^а-яА-Я\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }
        private void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^0-9\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo($"{Application.StartupPath}\\TourImage\\{nameHotel}");
            if (di.GetFiles($"{nameHotel}_*", SearchOption.TopDirectoryOnly).Length > idSelectPhoto)
            {
                button2.Visible = true;
                idSelectPhoto++;
            }
            if (di.GetFiles($"{nameHotel}_*", SearchOption.TopDirectoryOnly).Length == idSelectPhoto)
            {
                button4.Visible = false;
            }
            foreach (var item in di.GetFiles($"{nameHotel}_{idSelectPhoto}*", SearchOption.TopDirectoryOnly))
            {
                using (var stream = new FileStream($"{Application.StartupPath}\\TourImage\\{nameHotel}\\{item}", FileMode.Open))
                {
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo($"{Application.StartupPath}\\TourImage\\{nameHotel}");
            if (di.GetFiles($"{nameHotel}_*", SearchOption.TopDirectoryOnly).Length >= idSelectPhoto)
            {
                button4.Visible = true;
                idSelectPhoto--;
            }
            if (idSelectPhoto == 1)
            {
                button2.Visible = false;
            }
            foreach (var item in di.GetFiles($"{nameHotel}_{idSelectPhoto}*", SearchOption.TopDirectoryOnly))
            {
                using (var stream = new FileStream($"{Application.StartupPath}\\TourImage\\{nameHotel}\\{item}", FileMode.Open))
                {
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if(comboBox8.Text != "")
            {
                if (!Directory.Exists(Application.StartupPath + $"\\TourImage\\{comboBox8.Text.Trim()}"))
                {
                    Directory.CreateDirectory(Application.StartupPath + $"\\TourImage\\{comboBox8.Text.Trim()}");
                }
                DirectoryInfo di = new DirectoryInfo($"{Application.StartupPath}\\TourImage\\{nameHotel}");
                openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
                var havePic = di.GetFiles($"{nameHotel}_*", SearchOption.TopDirectoryOnly).Length;
                OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
                open_dialog.Filter = "Image Files(*.JPG;*.PNG)|*.JPG;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
                if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
                {
                    try
                    {
                        var words = open_dialog.FileName.Split('\\');
                        pictureBox1.Image = Image.FromFile(open_dialog.FileName);
                        File.Copy(open_dialog.FileName, $"{Application.StartupPath}\\TourImage\\{comboBox8.Text.Trim()}\\{nameHotel}_{havePic + 1}.{words[words.Length - 1].Split('.')[1]}", true);
                        if (havePic >= 1)
                        {
                            button4.Visible = false;
                            button2.Visible = true;
                            button6.Visible = true;
                            idSelectPhoto++;
                        }
                        if (havePic == 0)
                        {
                            button2.Visible = false;
                            button4.Visible = false;
                            button6.Visible = true;
                        }
                    }
                    catch
                    {
                        DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Перед добавлением фото выберите отель");
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
                if (c is MaskedTextBox)
                {
                    c.Text = "";
                }
            }
        }

        public void update()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT city.name as 'Город отправления', countries.name as 'Страна тура', start_date as 'Дата начала', end_date as 'Дата окончания',
                                    quantity_night as 'Количество ночей', hotel.name as 'Отель', type_food.description as 'Тип питания', type_room.name as 'Тип номера',
                                    air_travel.name as 'Авиаперелет', tour_operator.name as 'Туроператор', price as 'Цена', status_tour.name as 'Статус тура' FROM tour
                                    JOIN tour_operator ON tour.tour_operator = tour_operator.id
                                    JOIN type_food ON tour.type_food = type_food.id
                                    JOIN type_room ON tour.type_room = type_room.id
                                    JOIN air_travel ON tour.air_travel = air_travel.id
                                    JOIN status_tour ON tour.status = status_tour.id
                                    JOIN hotel ON tour.hotel = hotel.id
                                    JOIN city ON tour.departure_city = city.id
                                    JOIN countries ON tour.country = countries.id
                                    WHERE tour.id = {tourId}";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while(rdr.Read())
                    {
                        comboBox6.Text = rdr[0].ToString();
                        comboBox7.Text = rdr[1].ToString();
                        dateTimePicker1.Text = rdr[2].ToString();
                        dateTimePicker2.Text = rdr[3].ToString();
                        numericUpDown1.Value = Convert.ToDecimal(rdr[4].ToString());
                        comboBox8.Text = rdr[5].ToString();
                        comboBox1.Text = rdr[6].ToString();
                        comboBox2.Text = rdr[7].ToString();
                        comboBox3.Text = rdr[8].ToString();
                        comboBox4.Text = rdr[9].ToString();
                        textBox4.Text = rdr[10].ToString();
                        comboBox5.Text = rdr[11].ToString();
                    }
                }
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
                numericUpDown1.Enabled = false;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                textBox4.Enabled = false;
                comboBox5.Enabled = false;
                comboBox6.Enabled = false;
                comboBox7.Enabled = false;
                comboBox8.Enabled = false;
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

        private void ComboBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^а-яА-Я\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void ComboBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^а-яА-Я\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void ComboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                comboBox8.Items.Clear();
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT name FROM hotel WHERE country = (SELECT id FROM countries WHERE name = '{comboBox7.Text.Trim()}')";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while(rdr.Read())
                    {
                        comboBox8.Items.Add(rdr[0].ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void ComboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT type_food.description as 'Тип питания'
                                    FROM hotel
                                    JOIN type_food ON hotel.type_food = type_food.id
                                    WHERE hotel.name = '{comboBox8.Text}'";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while(rdr.Read())
                    {
                        comboBox1.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();

                    query = $@"SELECT type_room.name as 'Тип номеров'
	                                FROM hotel
	                                LEFT JOIN type_room ON JSON_CONTAINS(hotel.type_room, CAST(type_room.id as JSON),'$')
                                    WHERE hotel.name = '{comboBox8.Text.Trim()}'";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox2.Items.Add(rdr[0].ToString());
                    }
                }
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = Convert.ToDecimal(Convert.ToInt32(dateTimePicker2.Value.AddDays(-Convert.ToInt32(dateTimePicker1.Value.ToString("dd"))).ToString("dd")) - 1);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo($"{Application.StartupPath}\\TourImage\\{nameHotel}");
                openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
                var Pic = di.GetFiles($"{nameHotel}_{idSelectPhoto}*", SearchOption.TopDirectoryOnly)[0];
                OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
                open_dialog.Filter = "Image Files(*.JPG;*.PNG)|*.JPG;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
                if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
                {
                    pictureBox1.Image = Image.FromFile(open_dialog.FileName);
                    var result = Path.ChangeExtension(open_dialog.FileName, $".{Pic.ToString().Split('.')[1]}");
                    var words = result.Split('\\');
                    File.Copy(open_dialog.FileName, $"{Application.StartupPath}\\TourImage\\{comboBox8.Text.Trim()}\\{Pic.ToString().Split('.')[0]}.{words[words.Length - 1].Split('.')[1]}", true);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value.AddDays(2);
            numericUpDown1.Value = Convert.ToDecimal(Convert.ToInt32(dateTimePicker2.Value.AddDays(-Convert.ToInt32(dateTimePicker1.Value.ToString("dd"))).ToString("dd")) - 1);
        }
    }
}
