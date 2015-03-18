﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caffeinated.Beanstalk;

namespace SampleApp
{
    public partial class Form1 : Form
    {
        BeanstalkConnection connection;

        public Form1()
        {
            InitializeComponent();
        }

        void connectButton_Click(object sender, EventArgs e)
        {
            connectButton.Enabled = false;
            disconnectButton.Enabled = true;

            var hostname = hostnameTextBox.Text;
            var port = Convert.ToInt32(portTextBox.Text);

            connection = new BeanstalkConnection(hostname, port);
            connection.Connect();
        }

        void disconnectButton_Click(object sender, EventArgs e)
        {
            connection.Close();
            connection = null;

            connectButton.Enabled = true;
            disconnectButton.Enabled = false;
        }

        async void putJobButton_Click(object sender, EventArgs e)
        {
            var buffer = new byte[4];
            new Random().NextBytes(buffer);
            var id = await connection.PutAsync(buffer, 1, 0, 10);
            putJobsLog.AppendText(string.Format("Put job {0} - {1:X} {2:X} {3:X} {4:X}\n", id, buffer[0], buffer[1], buffer[2], buffer[3]));
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var job = await connection.ReserveAsync();

            var sb = new StringBuilder()
                .AppendFormat("Reserved job {0} - ", job.Id);

            foreach (var b in job.JobData)
                sb.AppendFormat("{0:X} ", b);

            sb.AppendLine();

            reservedJobsLog.AppendText(sb.ToString());
        }
    }
}
