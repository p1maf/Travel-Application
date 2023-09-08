using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace travel
{
    public partial class MainMenu : Form
    {
        string name, role, login;

        string connectionString = DB.connectionString;
        public int timeLeft = 0;
        private void MainMenu_Load(object sender, EventArgs e)
        {
            switch (role)
            {
                case "Менеджер":
                    button1.Visible = false;
                    button5.Visible = false;
                    button2.Visible = false;
                    break;
                case "Администратор":
                    button6.Visible = false;
                    break;
            }
            timer1.Enabled = true;
            GlobalMouseHandler gmh = new GlobalMouseHandler();
            gmh.TheMouseMoved += new MouseMovedEvent(gmh_TheMouseMoved);
            Application.AddMessageFilter(gmh);
            timer1.Interval = 60000;
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        DataTable GetTable(MySqlCommand cmd, string sql)
        {
            //Функция получения таблицы
            cmd.CommandText = sql;
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Guide guide = new Guide(login);
            this.Hide();
            guide.ShowDialog();
            this.Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Tour tour = new Tour(role);
            this.Hide();
            tour.ShowDialog();
            this.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            Special_Features special_Features = new Special_Features();
            this.Hide();
            special_Features.ShowDialog();
            this.Show();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            MakeOrder makeOrder = new MakeOrder("","",role, login);
            this.Hide();
            makeOrder.ShowDialog();
            this.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            AccountOrders accountOrders = new AccountOrders(role, login);
            this.Hide();
            accountOrders.ShowDialog();
            this.Show();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        conn.ConnectionString = connectionString;
                        //Создание подлкючения
                        conn.Open();
                        cmd.Connection = conn;
                        DataTable dt = GetTable(cmd, "show databases LIKE '%tourism';");

                        //Заполнение таблицы
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (!Directory.Exists("Резервная копия базы данных"))
                            {
                                Directory.CreateDirectory("Резервная копия базы данных");
                            }
                            string db = dr[0] + "";
                            string time2 = DateTime.Now.ToString("yyyy.MM.dd_HH.mm");
                            cmd.CommandText = $"use `{db}`";
                            cmd.ExecuteNonQuery();
                            string fileName = $"{db}-{time2}.sql";
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                //Функция импорта
                                mb.ExportInfo.ExportTableStructure = true;
                                mb.ExportInfo.ExportRows = true;
                                mb.ExportToFile("Резервная копия базы данных/" + fileName);
                            }
                        }
                        conn.Close();
                    }
                }
                Application.Exit();
            }
            catch(Exception ex)
            {
                Application.Exit();
            }
        }
        void gmh_TheMouseMoved()
        {
            Point cur_pos = System.Windows.Forms.Cursor.Position;
            timeLeft = 10;
            timer1.Start();
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timeLeft--;

            if (timeLeft == 0)
            {
                //Переход на форму
                timer1.Stop();
                if(Application.OpenForms.OfType<Form1>().Count() > 0)
                {

                }
                else
                {
                    Form1 form1 = new Form1();
                    form1.Show();
                }
                MessageBox.Show("Обнаружена неактивность на форме! Переход на форму авторизации!");
                for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
                {
                    if (Application.OpenForms[i].Name != "Form1")
                    {
                        Application.OpenForms[i].Close();
                    }
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ContractLog contractLog = new ContractLog();
            this.Hide();
            contractLog.ShowDialog();
            this.Show();
        }

        public MainMenu(string userName, string userRole, string loginUser)
        {
            InitializeComponent();
            label1.Text += $"Добро пожаловать!\n{userRole}: {userName}";
            name = userName;
            role = userRole;
            login = loginUser;
        }
    }

    public delegate void MouseMovedEvent();

    public class GlobalMouseHandler : IMessageFilter
    {
        private const int WM_MOUSEMOVE = 0x0200;

        public event MouseMovedEvent TheMouseMoved;

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEMOVE)
            {
                if (TheMouseMoved != null)
                {
                    TheMouseMoved();
                }
            }
            // Always allow message to continue to the next filter control
            return false;
        }

        #endregion
    }
}
