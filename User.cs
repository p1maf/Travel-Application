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
    public partial class User : Form
    {
        string connectionString = DB.connectionString;
        string nameUser, surnameUser, patronymicUser, loginUser, passwordUser,roleUser, idUser;
        string loginAdmin;

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow is null)
            {
                MessageBox.Show("Выберите строку");
            }
            else
            {
                try
                {
                    idUser = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                    string loginAdm = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Логин"].Value.ToString();
                    if(loginAdm == loginAdmin)
                    {
                        MessageBox.Show("Вы не можете удалить себя");
                    }
                    else
                    {
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();

                            string query = $@"UPDATE users SET delUser = 1 WHERE id = {idUser}";
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
                                    MessageBox.Show("Пользователь успешно удален");
                                    update();
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка");
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


        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow is null)
                {
                    MessageBox.Show("Выберите пользователя для редактирования");
                }
                else
                {
                    idUser = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                    nameUser = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Имя"].Value.ToString();
                    surnameUser = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Фамилия"].Value.ToString();
                    patronymicUser = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Отчество"].Value.ToString();
                    loginUser = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Логин"].Value.ToString();
                    roleUser = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Роль"].Value.ToString();
                    addUser add = new addUser(nameUser, surnameUser, patronymicUser, loginUser, "", roleUser, "Редактировать", idUser);
                    this.Hide();
                    add.ShowDialog();
                    this.Show();
                    update();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }

        }

        public User(string login)
        {
            InitializeComponent();
            loginAdmin = login;
        }

        private void User_Load(object sender, EventArgs e)
        {
            update();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            addUser add = new addUser("", "", "", "", "", "", "Добавить", "");
            this.Hide();
            add.ShowDialog();
            this.Show();
            update();
        }

        void update()
        {
            try
            {
                string query = "SELECT users.id, surname as 'Фамилия', users.name as 'Имя', patronymic as 'Отчество', login as 'Логин', role.name as 'Роль' FROM users INNER JOIN role ON users.role = role.id WHERE delUser = '0';";
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
                        string patrUser = "";
                        string nameUser = row.Cells["Имя"].Value.ToString();
                        if(row.Cells["Отчество"].Value.ToString() != "")
                        {
                            patrUser = row.Cells["Отчество"].Value.ToString();
                        }
                        row.Cells["Имя"].Value = nameUser.Replace(nameUser.Substring(1, nameUser.Length - 2), new string('*', nameUser.Length - 1));
                        if(patrUser != "")
                        {
                            row.Cells["Отчество"].Value = patrUser.Replace(patrUser.Substring(1, patrUser.Length - 2), new string('*', patrUser.Length - 1));
                        }
                    }
                    dataGridView1.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}
