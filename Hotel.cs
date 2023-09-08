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
    public partial class Hotel : Form
    {
        string connectionString = DB.connectionString;
        public Hotel()
        {
            InitializeComponent();
        }

        private void Hotel_Load(object sender, EventArgs e)
        {
            update();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            workWithHotel workWithHotel = new workWithHotel("","Добавить","","","","","","");
            this.Hide();
            workWithHotel.ShowDialog();
            this.Show();
            update();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentRow is null)
            {
                MessageBox.Show("Выберите запись");
            }
            else
            {
                string typeRoom = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Тип номеров"].Value.ToString();
                string convHotel = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Удобства"].Value.ToString();
                string nameHotel = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Название отеля"].Value.ToString();
                string typeFood = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Тип питания"].Value.ToString();
                string idHotel = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                string starHotel = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Количество звезд"].Value.ToString();
                string countryHotel = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Страна"].Value.ToString();
                workWithHotel workWithHotel = new workWithHotel(idHotel, "Редактировать", nameHotel, typeFood, typeRoom, convHotel, starHotel, countryHotel);
                this.Hide();
                workWithHotel.ShowDialog();
                this.Show();
                update();
            }
        }

        void update()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT group_concat(DISTINCT hotel.id), hotel.name as 'Название отеля',group_concat(DISTINCT type_room.name) as 'Тип номеров', group_concat(DISTINCT conveniences.name) as 'Удобства',
                                    group_concat(DISTINCT type_food.description) as 'Тип питания', group_concat(DISTINCT star) as 'Количество звезд', group_concat(DISTINCT countries.name) as 'Страна'
                                    FROM hotel
                                    LEFT JOIN type_room ON JSON_CONTAINS(hotel.type_room, CAST(type_room.id as JSON),'$')
                                    LEFT JOIN conveniences ON JSON_CONTAINS(hotel.conveniences, CAST(conveniences.id as JSON),'$')
                                    LEFT JOIN type_food ON hotel.type_food = type_food.id
                                    LEFT JOIN countries ON hotel.country = countries.id
                                    GROUP BY hotel.name";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns[0].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow is null)
            {
                MessageBox.Show("Выберите строку");
            }
            else
            {
                try
                {
                    string idHotel = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();

                        string query = $@"DELETE FROM hotel WHERE id = {idHotel}";
                        DialogResult result = MessageBox.Show(
                            "Вы точно хотите удалить запись",
                            "Сообщение",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1);

                        if (result == DialogResult.Yes)
                        {

                            MySqlCommand cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                MessageBox.Show("Информация об отеле успешно удалена");
                                update();
                            }
                            else
                            {
                                MessageBox.Show("Ошибка");
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
