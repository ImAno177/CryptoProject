using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crypto
{
    public partial class UserCheckBoxForm : Form
    {
        public List<string> SelectedOptions { get; private set; } = new List<string>();
        public UserCheckBoxForm(List<string> options)
        {
            InitializeComponent();

            int y = 10;
            foreach (var opt in options)
            {
                CheckBox cb = new CheckBox();
                cb.Text = opt;
                cb.Location = new Point(10, y);
                cb.AutoSize = true;
                this.Controls.Add(cb);
                y += 25;
            }

            Button btnOK = new Button() { Text = "OK", Location = new Point(10, y) };
            btnOK.Click += (s, e) =>
            {
                SelectedOptions = this.Controls
                    .OfType<CheckBox>()
                    .Where(c => c.Checked)
                    .Select(c => c.Text)
                    .ToList();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnOK);

            this.ClientSize = new Size(250, y + 40);
        }
    }
}
