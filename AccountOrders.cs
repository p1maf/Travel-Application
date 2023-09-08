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
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
namespace travel
{
    public partial class AccountOrders : Form
    {
        string connectionString = DB.connectionString;
        private Excel.Application _excel;
        private Excel.Worksheet _sheet;
        string role, login;
        public AccountOrders(string userRole, string userLoign)
        {
            InitializeComponent();
            role = userRole;
            login = userLoign;
        }

        void update(string dateFirst, string dateSecond)
        {
            try
            {
                label4.Text = "0";
                label7.Text = "0";
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    string query = $@"SELECT travel_package.id as 'Номер договора', tour.id as 'Номер тура', tour.price as 'Цена ₽', concat(tourist.surname, ' ', LEFT(tourist.name, 1), '.', LEFT(tourist.patronymic, 1)) as 'ФИО клиента', concat(users.surname, ' ', users.name, ' ', users.patronymic) as 'ФИО менеджера',
                                    reg_date as 'Дата оформления', (SELECT sum(amount) FROM payment  WHERE id_travel = travel_package.id group by id_travel) as 'Оплаченная сумма', (SELECT name FROM countries WHERE countries.id = tour.country) as 'Страна тура'
                                    FROM travel_package
                                    JOIN tour ON travel_package.idtour = tour.id
                                    JOIN countries ON tour.country = countries.id
                                    JOIN tourist ON travel_package.idpayer = tourist.id
                                    JOIN users ON travel_package.idmanager = users.id";
                    if(role == "Менеджер")
                    {
                        query = $@"SELECT travel_package.id as 'Номер договора', tour.id as 'Номер тура', tour.price as 'Цена ₽', concat(tourist.surname, ' ', LEFT(tourist.name, 1), '.', LEFT(tourist.patronymic, 1)) as 'ФИО клиента', concat(users.surname, ' ', users.name, ' ', users.patronymic) as 'ФИО менеджера',
                                    reg_date as 'Дата оформления', (SELECT sum(amount) FROM payment  WHERE id_travel = travel_package.id group by id_travel) as 'Оплаченная сумма', (SELECT name FROM countries WHERE countries.id = tour.country) as 'Страна тура'
                                    FROM travel_package
                                    JOIN tour ON travel_package.idtour = tour.id
                                    JOIN tourist ON travel_package.idpayer = tourist.id
                                    JOIN countries ON tour.country = countries.id
                                    JOIN users ON travel_package.idmanager = users.id
                                    WHERE users.login = '{login}'";
                    }
                    if (dateFirst != "" && dateSecond != "" && checkBox1.Checked == false)
                    {
                        if(role != "Менеджер")
                        {
                            query += $" WHERE reg_date BETWEEN '{dateFirst}' AND '{dateSecond}'";
                            if(comboBox1.Text != "Все")
                            {
                                query += $" AND countries.name = '{comboBox1.Text}'";
                                if (comboBox1.Text != "Все")
                                {
                                    query += $" AND countries.name = '{comboBox1.Text}'";
                                }
                            }
                        }
                        else
                        {
                            query += $" AND reg_date BETWEEN '{dateFirst}' AND '{dateSecond}'";
                        }
                    }
                    else
                    {
                        if(comboBox1.Text != "Все" && role != "Менеджер")
                        {
                            query += $" WHERE countries.name = '{comboBox1.Text}'";
                        }
                        else if(comboBox1.Text != "Все" && role == "Менеджер")
                        {
                            query += $" AND countries.name = '{comboBox1.Text}'";
                        }
                    }
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    label5.Text = $"Количество записей: {dataGridView1.Rows.Count}";

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if(row.Cells["Цена ₽"].Value.ToString() != "")
                        {
                            label4.Text = $"{Convert.ToInt32(label4.Text) + Convert.ToInt32(row.Cells["Цена ₽"].Value.ToString())}";
                        }
                        if (row.Cells["Оплаченная сумма"].Value.ToString() != "")
                        {
                            label7.Text = $"{Convert.ToInt32(label7.Text) + Convert.ToInt32(row.Cells["Оплаченная сумма"].Value.ToString())}";
                        }
                    }
                    label9.Text = $"{Convert.ToInt32(label4.Text) - Convert.ToInt32(label7.Text)} р.";
                    label4.Text += " р.";
                    label7.Text += " р.";
                    dataGridView1.Columns[1].Visible = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void AccountOrders_Load(object sender, EventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = connectionString;
                con.Open();
                string query = @"SELECT DISTINCT countries.name FROM travel_package
                                JOIN tour ON travel_package.idtour = tour.id
                                JOIN countries ON tour.country = countries.id";
                MySqlCommand cmd = new MySqlCommand(query, con);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while(rdr.Read())
                {
                    comboBox1.Items.Add(rdr[0].ToString());
                }
            }
            update("", "");
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.MinDate = new DateTime(DateTime.Now.Year, 1, 01);
            dateTimePicker1.MaxDate = new DateTime(DateTime.Now.Year, 12, 31);
            dateTimePicker2.MinDate = new DateTime(DateTime.Now.Year, 1, 01);
            dateTimePicker2.MaxDate = new DateTime(DateTime.Now.Year, 12, 31);
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.MaxDate = dateTimePicker2.Value;
            if (checkBox1.Checked == false)
            {
                update(dateTimePicker1.Value.ToString("yyyy-MM-dd"), dateTimePicker2.Value.ToString("yyyy-MM-dd"));
            }
            dateTimePicker2.MinDate = dateTimePicker1.Value;
        }

        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value;
            if (checkBox1.Checked == false)
            {
                update(dateTimePicker1.Value.ToString("yyyy-MM-dd"), dateTimePicker2.Value.ToString("yyyy-MM-dd"));
            }
            dateTimePicker1.MaxDate = dateTimePicker2.Value;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == false)
            {
                update(dateTimePicker1.Value.ToString("yyyy-MM-dd"), dateTimePicker2.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                update("", "");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                update(dateTimePicker1.Value.ToString("yyyy-MM-dd"), dateTimePicker2.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                update("", "");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            if (checkBox1.Checked == false)
            {
                update(dateTimePicker1.Value.ToString("yyyy-MM-dd"), dateTimePicker2.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                update("", "");
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                _excel = new Excel.Application(); //создаем COM-объект Excel
                _excel.SheetsInNewWorkbook = 1;//количество листов в книге
                _excel.Workbooks.Add(Type.Missing); //добавляем книгу
                Excel.Workbook workbook = _excel.Workbooks[1]; //получам ссылку на первую открытую книгу
                _sheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(1);//получаем ссылку на первый лист
                Excel.Range _excelCells1 = (Excel.Range)_sheet.get_Range("A1", "H1").Cells;
                // Производим объединение
                _excelCells1.Merge(Type.Missing);
                _sheet.Cells[1, 1] = "Отчет дохода туристической компании";
                _sheet.Cells[1, 1].HorizontalAlignment = -4108;
                Excel.Range _excelCells2 = (Excel.Range)_sheet.get_Range("A2", "H2").Cells;
                // Производим объединение
                _excelCells2.Merge(Type.Missing);
                if(checkBox1.Checked == true)
                {
                    _sheet.Cells[2, 1] = $"{dateTimePicker1.Value.ToShortDateString()} - {dateTimePicker2.Value.ToShortDateString()}";
                }
                else
                {
                    _sheet.Cells[2, 1] = $"{dateTimePicker1.MinDate.ToShortDateString()} - {dateTimePicker2.MaxDate.ToShortDateString()}";
                }
                _sheet.Cells[2, 1].HorizontalAlignment = -4108;
                var dgw1 = new DataGridView();
                dgw1 = dataGridView1;
                var cntColl = dgw1.ColumnCount;
                var cntrow = dgw1.RowCount;
                try
                {
                    if (dgw1.RowCount != 0)
                    {

                        //Заполнение заголовков столбцов
                        for (int coll = 1; coll <= cntColl; coll++)
                        {
                            _sheet.Cells[3, coll] = dgw1.Columns[coll - 1].HeaderCell.Value;
                            ((Excel.Range)_sheet.get_Range($"A3:H3")).Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        }
                        //Заполнение ячеек данными
                        for (int row = 0; row <= cntrow - 1; row++)
                        {
                            for (int coll = 1; coll <= cntColl; coll++)
                            {
                                _sheet.Cells[row + 4, coll] = dgw1.Rows[row].Cells[coll - 1].Value;
                                ((Excel.Range)_sheet.get_Range($"A{row + 4}:H{row + 4}")).Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            }
                        }
                        _sheet.Cells[4 + cntrow, 7] = $"Общая сумма туров";
                        _sheet.Cells[4 + cntrow, 8] = $"{label4.Text.Split(' ')[0]} р.";
                        ((Excel.Range)_sheet.Cells[4 + cntrow, 8]).Font.Bold = true;

                        _sheet.Cells[5 + cntrow, 7] = $"Сумма оплат";
                        _sheet.Cells[5 + cntrow, 8] = $"{label7.Text.Split(' ')[0]} р.";
                        ((Excel.Range)_sheet.Cells[5 + cntrow, 8]).Font.Bold = true;

                        _sheet.Cells[6 + cntrow, 7] = $"Остаток к оплате";
                        _sheet.Cells[6 + cntrow, 8] = $"{label9.Text.Split(' ')[0]} р.";
                        ((Excel.Range)_sheet.Cells[6 + cntrow, 8]).Font.Bold = true;
                        _sheet.Columns.EntireColumn.AutoFit();
                    }
                    _excel.Visible = true;
                    Marshal.ReleaseComObject(_excel);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Ошибка");
                    Marshal.ReleaseComObject(_excel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
                Marshal.ReleaseComObject(_excel);
            }
        }
    }
}
