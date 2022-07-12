namespace Digifinex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.symbol = textBox3.Text;
            form2.key = textBox1.Text;
            form2.secretkey = textBox2.Text;
            form2.Show();
        }
    }
}