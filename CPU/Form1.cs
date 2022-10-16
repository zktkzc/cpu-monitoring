using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Management;

namespace CPU
{
    public partial class Form1 : Form
    {

        Process[] MyProcess = Array.Empty<Process>();
        // 百分比
        int mheight = 0;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体的加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Computer myInfo = new Computer();
            // 取内存总量
            var totalPhysicalMemory = myInfo.Info.TotalPhysicalMemory / 1024 / 1024 / 1024;
            this.label5.Text = totalPhysicalMemory.ToString();
            progressBar1.Maximum = Convert.ToInt32(totalPhysicalMemory);
            progressBar1.Value = Convert.ToInt32(totalPhysicalMemory);
            // 取内存可用余额
            var availablePhysicalMemory = myInfo.Info.AvailablePhysicalMemory / 1024 / 1024 / 1024;
            this.label6.Text = availablePhysicalMemory.ToString();
            progressBar2.Maximum = Convert.ToInt32(totalPhysicalMemory);
            progressBar2.Value = Convert.ToInt32(availablePhysicalMemory);

            // 取虚拟地址空间总量
            var totalVirtualMemory = myInfo.Info.TotalVirtualMemory / 1024 / 1024 / 1024;
            this.label7.Text = totalVirtualMemory.ToString();
            progressBar3.Maximum = Convert.ToInt32(totalVirtualMemory);
            progressBar3.Value = Convert.ToInt32(totalVirtualMemory);
            // 取虚拟地址空间可用余额
            var availableVirtualMemory = myInfo.Info.AvailableVirtualMemory / 1024 / 1024 / 1024;
            this.label8.Text = availableVirtualMemory.ToString();
            progressBar4.Maximum = Convert.ToInt32(totalVirtualMemory);
            progressBar4.Value = Convert.ToInt32(availableVirtualMemory);

            // 获取内存占用前10的进程
            loadTop10();
        }

        // 创建图片
        private void createImage()
        {
            int i = panel2.Height / 100;
            Bitmap image = new Bitmap(panel2.Width, panel2.Height);
            // 创建Graphic类对象
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.Green);
            SolidBrush myBrush = new SolidBrush(Color.Lime);
            g.FillRectangle(myBrush, 0, panel2.Height - mheight * i, 30, mheight * i);
            panel2.BackgroundImage = image;
        }

        private void MyUser()
        {
            // CPU百分比计算
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject myObject in searcher.Get())
            {
                mheight = Convert.ToInt32(myObject["LoadPercentage"]?.ToString());
            }
            // CPU动态显示
            createImage();
        }

        /// <summary>
        /// 计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 通过异步的方式执行
            Task.Run(() =>
            {
                MyUser();
            });
        }

        /// <summary>
        /// 刷新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            loadTop10();
        }

        /// <summary>
        /// 关闭指定进程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                // 获取所有进程
                Process[] p = Process.GetProcesses();
                foreach (Process process in p)
                {
                    try
                    {
                        if (process.ProcessName.ToString().Trim() == listBox1.SelectedItem.ToString())
                        {
                            process.Kill();
                            MessageBox.Show("已关闭，请刷新！");
                            return;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("关闭进程失败，请重试！");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择一个要关闭的进程!");
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