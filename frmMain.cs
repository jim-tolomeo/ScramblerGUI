namespace ScramblerGUI
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                txtInputFile.Text = openFileDialog.FileName;
                btnScramble.Enabled = true;
            }
        }

        private void btnScramble_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtInputFile.Text))
            {
                MessageBox.Show($"File not found:\r\n\r\n{txtInputFile.Text}", "Scrambler", MessageBoxButtons.OK);
                return;
            }

            try
            {
                btnScramble.Enabled = false;
                progressBar.Style = ProgressBarStyle.Marquee;
                Application.DoEvents();
                new Scrambler().Scramble(txtInputFile.Text, lblProgress);
            }
            finally
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                lblProgress.Text = "Progress...";
                btnScramble.Enabled = true;
                Application.DoEvents();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}