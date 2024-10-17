using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _123123Client
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private string localIp;
        private string remoteIp;
        private bool isClientBusy = false;
        private StringBuilder clientBuffer = new StringBuilder();

        public Form1()
        {
            InitializeComponent();
            localIp = GetLocalIPAddress();
        }

        private void ReceiveMessages()
        {
            while (client.Connected)
            {
                try
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Invoke((MethodInvoker)delegate
                        {
                            textBox2.Text += $"{message}" + Environment.NewLine;
                        });
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Произошла ошибка при чтении данных: {ex.Message}");
                    break;
                }
            }
        }

        private string GetLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    
        private void button1_Click(object sender, EventArgs e)
        {
           

            remoteIp = textBox4.Text;
            if (client != null && client.Connected && isClientBusy)
            {
                textBox2.Clear();
                textBox2.Text = clientBuffer.ToString();
                clientBuffer.Clear();
                isClientBusy = false;
            }
            if (client != null && client.Connected)
            {
                client.Close();
            }

            client = new TcpClient(remoteIp, 12345);
            stream = client.GetStream();
            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string message = textBox1.Text;
                string data = textBox5.Text;
                if (client.Connected)
                {
                    byte[] mes = Encoding.UTF8.GetBytes(message);
                    byte[] dat = Encoding.UTF8.GetBytes(textBox5.Text);
                    stream.Write(dat, 0, dat.Length);
                    stream.Write(mes, 0, mes.Length);
                    textBox2.Text += $"{localIp}: {message}" + Environment.NewLine;
                }
                else
                {
                    MessageBox.Show("Ошибка: Соединение разорвано");
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Произошла ошибка при записи данных: {ex.Message}");
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
