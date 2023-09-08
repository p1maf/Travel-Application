using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace travel
{
    public partial class Guide : Form
    {
        string loginAdmin;
        public Guide(string login)
        {
            InitializeComponent();
            loginAdmin = login;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            User user = new User(loginAdmin);
            this.Hide();
            user.ShowDialog();
            this.Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            TourOperator tourOperator = new TourOperator();
            this.Hide();
            tourOperator.ShowDialog();
            this.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Hotel hotel = new Hotel();
            this.Hide();
            hotel.ShowDialog();
            this.Show();
        }
    }
}
