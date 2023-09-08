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
    public partial class TourOperator : Form
    {
        string connectionString = DB.connectionString;
        public TourOperator()
        {
            InitializeComponent();
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
                    string idOperator = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();

                        string query = $@"DELETE FROM tour_operator WHERE id = {idOperator}";
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
                                MessageBox.Show("Туроператор успешно удален");
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
        void update()
        {
            try
            {
                string query = @"SELECT tour_operator.id, tour_operator.name as 'Название', legal_name as 'Юридическое название', INN as 'ИНН', OGRN as 'ОГРН',
                                address as 'Адрес', contract as 'Номер договора' FROM tour_operator;";
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

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string INN = row.Cells["ИНН"].Value.ToString();
                        string OGRN = row.Cells["ОГРН"].Value.ToString();
                        row.Cells["ИНН"].Value = INN.Replace(INN.Substring(1, INN.Length - 2), new string('*', INN.Length - 1));
                        row.Cells["ОГРН"].Value = OGRN.Replace(OGRN.Substring(1, OGRN.Length - 2), new string('*', OGRN.Length - 1));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void TourOperator_Load(object sender, EventArgs e)
        {
            update();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            workWithTourOperator workWithTourOperator = new workWithTourOperator("Добавить", "");
            this.Hide();
            workWithTourOperator.ShowDialog();
            this.Show();
            update();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentRow is null)
            {
                MessageBox.Show("Выберите строку");
            }
            else
            {
                string idOperator = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                workWithTourOperator workWithTourOperator = new workWithTourOperator("Редактировать", idOperator);
                this.Hide();
                workWithTourOperator.ShowDialog();
                this.Show();
                update();
            }

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
