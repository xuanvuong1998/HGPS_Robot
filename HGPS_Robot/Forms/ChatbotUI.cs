using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HGPS_Robot
{
    public partial class ChatbotUI : Form
    {
        public ChatbotUI()
        {
            InitializeComponent();
        }

        private void ChatbotUI_Shown(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            var area = Screen.FromControl(this).WorkingArea;
            picBackground.Location = new Point(0, 0);
            picBackground.Size = new Size(area.Width, area.Height);
            picBackground.Controls.Add(picClose);

            picClose.Location = new Point(300, 820);
            picClose.Size = new Size(170, 200);
            picClose.BackColor = Color.Transparent;

        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
