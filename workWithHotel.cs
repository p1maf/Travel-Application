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
    public partial class workWithHotel : Form
    {
        string connectionString = DB.connectionString;
        string hotelID, typeFoods, nameHotels, countryHotel;
        string[] typeRooms, typeConv;
        public workWithHotel(string idHotel, string nameBtn,string nameHotel, string typeFood, string typeRoom, string convHotel, string starHotel, string country)
        {
            InitializeComponent();
            button1.Text = nameBtn;
            if(nameBtn != "Добавить")
            {
                textBox1.Text = nameHotel;
                nameHotels = nameHotel;
                typeFoods = typeFood;
                hotelID = idHotel;
                countryHotel = country;
                if (typeRoom != "")
                {
                    typeRooms = typeRoom.Split(',');
                }
                if (convHotel != "")
                {
                    typeConv = convHotel.Split(',');
                }
                textBox4.Text = starHotel;
                textBox1.ForeColor = Color.Black;
                textBox4.ForeColor = Color.Black;
            }
        }

        private void ComboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^а-яА-Я\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text == "Название")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Название";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void TextBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text == "Количество звезд")
            {
                textBox4.Text = "";
                textBox4.ForeColor = Color.Black;
            }
        }

        private void TextBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                textBox4.Text = "Количество звезд";
                textBox4.ForeColor = Color.Gray;
            }
        }

        private void WorkWithHotel_Load(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = "SELECT name FROM type_room";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        checkedListBox1.Items.Add(rdr[0].ToString());
                    }
                    rdr.Close();

                    query = "SELECT name FROM conveniences";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        checkedListBox2.Items.Add(rdr[0].ToString());
                    }

                    rdr.Close();

                    query = "SELECT description FROM type_food";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox1.Items.Add(rdr[0].ToString());
                    }

                    rdr.Close();
                    var values = new AutoCompleteStringCollection();
                    query = "SELECT name FROM countries";
                    cmd = new MySqlCommand(query, con);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox2.Items.Add(rdr[0].ToString());
                        values.Add(rdr[0].ToString());
                    }
                    comboBox2.AutoCompleteCustomSource = values;
                    comboBox2.AutoCompleteMode = AutoCompleteMode.Suggest;
                    comboBox2.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
                comboBox1.Text = typeFoods;
                comboBox2.Text = countryHotel;
                if(typeRooms != null)
                {
                    foreach (var item in typeRooms)
                    {
                        checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(item), true);
                    }
                }
                if(typeConv != null)
                {
                    foreach (var item in typeConv)
                    {
                        checkedListBox2.SetItemChecked(checkedListBox2.Items.IndexOf(item), true);
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
            if(string.IsNullOrWhiteSpace(textBox1.Text) || checkedListBox1.CheckedItems.Count == 0 || string.IsNullOrWhiteSpace(textBox4.Text) || textBox1.Text == "Название" || textBox4.Text == "Количество звезд")
            {
                MessageBox.Show("Не заполнены поля, либо не выбран тип комнат в отеле");
            }
            else
            {
                if(button1.Text == "Добавить")
                {
                    try
                    {
                        string typeRoomHotel = "[";
                        foreach (int item in checkedListBox1.CheckedIndices)
                        {
                            if (checkedListBox1.CheckedIndices.Count == 1)
                            {
                                typeRoomHotel += $"{item + 1},";
                            }
                            else
                            {
                                typeRoomHotel += $"{item + 1},";
                            }
                        }
                        typeRoomHotel = typeRoomHotel.Substring(0, typeRoomHotel.Length - 1) + "]";

                        string typeConvHotel = "[";
                        foreach (int item in checkedListBox2.CheckedIndices)
                        {
                            if (checkedListBox2.CheckedIndices.Count == 1)
                            {
                                typeConvHotel += $"{item + 1},";
                            }
                            else
                            {
                                typeConvHotel += $"{item + 1},";
                            }
                        }
                        if (typeConvHotel.Length == 1)
                        {
                            typeConvHotel += "]";
                        }
                        else
                        {
                            typeConvHotel = typeConvHotel.Substring(0, typeConvHotel.Length - 1) + "]";
                        }

                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $"SELECT name FROM hotel WHERE name = '{textBox1.Text.Trim()}'";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteScalar() == null)
                            {
                                query = $"SELECT name FROM countries WHERE name = '{comboBox2.Text.Trim()}'";
                                cmd = new MySqlCommand(query, con);
                                if(cmd.ExecuteScalar() != null)
                                {
                                    query = $"INSERT INTO hotel VALUES(null,'{textBox1.Text.Trim()}',{comboBox1.SelectedIndex + 1},'{typeRoomHotel}',{textBox4.Text.Trim()},'{typeConvHotel}', (SELECT id FROM countries WHERE name = '{comboBox2.Text}'))";
                                    cmd = new MySqlCommand(query, con);
                                    if (cmd.ExecuteNonQuery() == 1)
                                    {
                                        MessageBox.Show("Информация об отеле успешно добавлена");
                                        this.Close();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Страна введена неверно");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Отель с таким названием уже существует");
                                textBox1.Clear();
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
                        string typeRoomHotel = "[";
                        foreach (int item in checkedListBox1.CheckedIndices)
                        {
                            if (checkedListBox1.CheckedIndices.Count == 1)
                            {
                                typeRoomHotel += $"{item + 1},";
                            }
                            else
                            {
                                typeRoomHotel += $"{item + 1},";
                            }
                        }
                        typeRoomHotel = typeRoomHotel.Substring(0, typeRoomHotel.Length - 1) + "]";

                        string typeConvHotel = "[";
                        foreach (int item in checkedListBox2.CheckedIndices)
                        {
                            if (checkedListBox2.CheckedIndices.Count == 1)
                            {
                                typeConvHotel += $"{item + 1},";
                            }
                            else
                            {
                                typeConvHotel += $"{item + 1},";
                            }
                        }
                        if (typeConvHotel.Length == 1)
                        {
                            typeConvHotel += "]";
                        }
                        else
                        {
                            typeConvHotel = typeConvHotel.Substring(0, typeConvHotel.Length - 1) + "]";
                        }
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $"SELECT name FROM hotel WHERE name = '{textBox1.Text.Trim()}'";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteScalar() == null || cmd.ExecuteScalar().ToString() == nameHotels)
                            {
                                query = $"SELECT name FROM countries WHERE name = '{comboBox2.Text.Trim()}'";
                                cmd = new MySqlCommand(query, con);
                                if (cmd.ExecuteScalar() != null)
                                {
                                    DialogResult result = MessageBox.Show(
                                    "Вы точно хотите отредактировать запись?",
                                    "Сообщение",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information,
                                    MessageBoxDefaultButton.Button1);
                                    if (result == DialogResult.Yes)
                                    {
                                        query = $"UPDATE hotel SET name = '{textBox1.Text.Trim()}', type_food = {comboBox1.SelectedIndex + 1}, type_room = '{typeRoomHotel}', star = {textBox4.Text.Trim()}, conveniences = '{typeConvHotel}', country = (SELECT id FROM countries WHERE name = '{comboBox2.Text}')  WHERE id = {hotelID}";
                                        cmd = new MySqlCommand(query, con);
                                        if (cmd.ExecuteNonQuery() == 1)
                                        {
                                            MessageBox.Show("Информация об отеле успешно обновлена");
                                            this.Close();
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
                                MessageBox.Show("Отель с таким названием уже существует");
                                textBox1.Clear();
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Ошибка");
                    }


                }
            }
        }

        private void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^1-5\b]+");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                            "Вы точно хотите выйти, все изменения будут потеряны?",
                            "Сообщение",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1);
            if(result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
