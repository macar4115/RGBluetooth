using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;//to use I/O ports
using System.Runtime.InteropServices;

namespace RGBluetooth
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn
         (
              int nLeftRect,
              int nTopRect,
              int nRightRect,
              int nBottomRect,
              int nWidthEllipse,
                 int nHeightEllipse

          );
        public Form1()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
        }
        int red = 255, green = 255, blue = 255;
        string send = "R255G255B255";
        SerialPort serialPort1 = new SerialPort();
        Bitmap myBitmap = new Bitmap(Properties.Resources.RGBwheel2);
        private bool dragging = false;
        private Point startPoint = new Point(0, 0);


        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();//Retrieving  I/O ports
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);//Adding ports to combobox
            }
            pictureBox1.BackColor = Color.FromArgb(red, green, blue);//Retrieving backcolor of picturebox
            hScrollBar1.Value = red;//Setting red bar grabed red value
            hScrollBar2.Value = green;//Setting green bar grabed green value
            hScrollBar3.Value = blue;//Setting blue bar grabed blue value
            label5.Text = pictureBox1.BackColor.ToString();//Writing color by color code
            this.Location = Properties.Settings.Default.Form1Location;//Location data from settings
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedText == null)
            {
                MessageBox.Show("Please select a port");//Error message for unselected port
            }
            else
            {
                //Saving selected port values
                string x = comboBox1.SelectedItem.ToString();
                serialPort1.PortName = x;
                serialPort1.BaudRate = 9600;//equal to Arduino serial baund rate

                // Trying open and connect to port
                try
                {
                    if (!serialPort1.IsOpen)
                    {
                        serialPort1.Open();
                        button2.Enabled = true;
                        button1.Enabled = false;
                    }
                }
                catch
                {
                    MessageBox.Show("Could not open serial port.Please select another port or control your connection.");//Error message for  connetion open failure.
                }
            }
        }


        //Clearing and Retrieving I/O ports=Refeshing ports
        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();//Getting I/O ports
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);//Adding ports to combobox
            }
        }

        //Writing data to port
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Checking port
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                }
            }
            catch
            {
                MessageBox.Show("Could not open serial port");
            }
            serialPort1.DiscardOutBuffer();//Clearing previous data from port
            serialPort1.Write(send);//Writing data to port
        }

        //Clearing port and saving form location
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.WriteLine("R000G000B000");
                timer1.Enabled = false;
                serialPort1.Close();
            }
            if (this.WindowState == FormWindowState.Normal)
            {

                Properties.Settings.Default.Form1Location = this.Location;

            }
            Properties.Settings.Default.Save();
        }

        //Changing red value of color by bar
        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            red = hScrollBar1.Value;
            pictureBox1.BackColor = Color.FromArgb(red, green, blue);//Showing changed color
            send = string.Concat("R", red.ToString(), "G", green.ToString(), "B", blue.ToString());
            label5.Text = pictureBox1.BackColor.ToString();
        }

        //Changing green value of color by bar
        private void hScrollBar2_ValueChanged(object sender, EventArgs e)
        {
            green = hScrollBar2.Value;
            pictureBox1.BackColor = Color.FromArgb(red, green, blue);//Showing changed color
            send = string.Concat("R", red.ToString(), "G", green.ToString(), "B", blue.ToString());
            label5.Text = pictureBox1.BackColor.ToString();
        }

        //Changing blue value of color by bar
        private void hScrollBar3_ValueChanged(object sender, EventArgs e)
        {
            blue = hScrollBar3.Value;
            pictureBox1.BackColor = Color.FromArgb(red, green, blue);//Showing changed color
            send = string.Concat("R", red.ToString(), "G", green.ToString(), "B", blue.ToString());
            label5.Text = pictureBox1.BackColor.ToString();
        }

        //Changing color by color wheel
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Color pixelColor = myBitmap.GetPixel(e.X, e.Y);
            if (pixelColor.R > 0 || pixelColor.G > 0 || pixelColor.B > 0)
            {
                red = pixelColor.R;
                green = pixelColor.G;
                blue = pixelColor.B;
            }
            pictureBox1.BackColor = Color.FromArgb(red, green, blue);//Showing changed color
            //Changing bar values to color value
            hScrollBar1.Value = red;
            hScrollBar2.Value = green;
            hScrollBar3.Value = blue;
        }

        //Closing app by nocification icon
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Closing port by nocification icon
        private void exeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Write("R000G000B000");
                timer1.Enabled = false;
                button2.Enabled = true;
                button1.Enabled = true;
                button3.Enabled = false;
                serialPort1.Close();
            }
        }

        //Getting app to normal size by nocification icon
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        //Close app button
        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Minimize app button
        private void button5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //Changing form location
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this.startPoint.X, p.Y - this.startPoint.Y);
            }
        }

        //Start button
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                }
            }
            catch
            {
                MessageBox.Show("Could not open serial port");
            }
            if (serialPort1.IsOpen == true)
            {
                timer1.Enabled = true;
                button2.Enabled = false;
            }
        }

        //Stop button
        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Write("R000G000B000");
                timer1.Enabled = false;
                button2.Enabled = true;
                button1.Enabled = true;
                serialPort1.Close();//seri portu kapatır
            }
        }
    }
}
