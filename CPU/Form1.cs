using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Management;

namespace CPU
{
    public partial class Form1 : Form
    {

        Process[] MyProcess = Array.Empty<Process>();
        // �ٷֱ�
        int mheight = 0;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ����ļ����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Computer myInfo = new Computer();
            // ȡ�ڴ�����
            var totalPhysicalMemory = myInfo.Info.TotalPhysicalMemory / 1024 / 1024 / 1024;
            this.label5.Text = totalPhysicalMemory.ToString();
            progressBar1.Maximum = Convert.ToInt32(totalPhysicalMemory);
            progressBar1.Value = Convert.ToInt32(totalPhysicalMemory);
            // ȡ�ڴ�������
            var availablePhysicalMemory = myInfo.Info.AvailablePhysicalMemory / 1024 / 1024 / 1024;
            this.label6.Text = availablePhysicalMemory.ToString();
            progressBar2.Maximum = Convert.ToInt32(totalPhysicalMemory);
            progressBar2.Value = Convert.ToInt32(availablePhysicalMemory);

            // ȡ�����ַ�ռ�����
            var totalVirtualMemory = myInfo.Info.TotalVirtualMemory / 1024 / 1024 / 1024;
            this.label7.Text = totalVirtualMemory.ToString();
            progressBar3.Maximum = Convert.ToInt32(totalVirtualMemory);
            progressBar3.Value = Convert.ToInt32(totalVirtualMemory);
            // ȡ�����ַ�ռ�������
            var availableVirtualMemory = myInfo.Info.AvailableVirtualMemory / 1024 / 1024 / 1024;
            this.label8.Text = availableVirtualMemory.ToString();
            progressBar4.Maximum = Convert.ToInt32(totalVirtualMemory);
            progressBar4.Value = Convert.ToInt32(availableVirtualMemory);

            // ��ȡ�ڴ�ռ��ǰ10�Ľ���
            loadTop10();
        }

        // ����ͼƬ
        private void createImage()
        {
            int i = panel2.Height / 100;
            Bitmap image = new Bitmap(panel2.Width, panel2.Height);
            // ����Graphic�����
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.Green);
            SolidBrush myBrush = new SolidBrush(Color.Lime);
            g.FillRectangle(myBrush, 0, panel2.Height - mheight * i, 30, mheight * i);
            panel2.BackgroundImage = image;
        }

        private void MyUser()
        {
            // CPU�ٷֱȼ���
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject myObject in searcher.Get())
            {
                mheight = Convert.ToInt32(myObject["LoadPercentage"]?.ToString());
            }
            // CPU��̬��ʾ
            createImage();
        }

        /// <summary>
        /// ��ʱ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // ͨ���첽�ķ�ʽִ��
            Task.Run(() =>
            {
                MyUser();
            });
        }

        /// <summary>
        /// ˢ���¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            loadTop10();
        }

        /// <summary>
        /// �ر�ָ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                // ��ȡ���н���
                Process[] p = Process.GetProcesses();
                foreach (Process process in p)
                {
                    try
                    {
                        if (process.ProcessName.ToString().Trim() == listBox1.SelectedItem.ToString())
                        {
                            process.Kill();
                            MessageBox.Show("�ѹرգ���ˢ�£�");
                            return;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("�رս���ʧ�ܣ������ԣ�");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("��ѡ��һ��Ҫ�رյĽ���!");
            }
        }

        private void loadTop10()
        {
            var list = Process.GetProcesses().OrderByDescending(p => p.PagedMemorySize64).Take(10).Select(p => p.ProcessName);
            listBox1.Items.Clear();
            listBox1.Items.AddRange(list.ToArray());
        }
    }
}