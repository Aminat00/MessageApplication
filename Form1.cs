using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client;
using System.Configuration;
using RabbitMQ.Client.Events;

namespace MessageApplication
{
    

    public partial class Form1 : Form
    {
        private IModel channel_message;
        private IConnection connection;

        public Form1()
        {
            InitializeComponent();
            
            var ConnectionString = ConfigurationManager.ConnectionStrings["Connection_name"].ConnectionString;

            var ConnectionFactory = new ConnectionFactory() { };
            ConnectionFactory.Uri = new Uri(ConnectionString);
            ConnectionFactory.AutomaticRecoveryEnabled = true;
            ConnectionFactory.DispatchConsumersAsync = true;
            connection = ConnectionFactory.CreateConnection("chatting");
        }
      

        private void button1_Click(object sender, EventArgs e)
        {

            channel_message = connection.CreateModel();
            {
                channel_message.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);
                string message = richTextBox1.Text;
                    var body = Encoding.UTF8.GetBytes(message);
                    channel_message.BasicPublish(exchange: "pubsub",
                    routingKey: "",
                    basicProperties: null,
                    body: body);
                    MessageBox.Show("Message sent successfully.");
             }
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
 
        }

        private async Task channel_messageConsumer_Received(object sender, BasicDeliverEventArgs e)
        {
            
           

            if (String.IsNullOrEmpty(textBox1.Text))
            {
                channel_message = connection.CreateModel();
                channel_message.BasicAck(e.DeliveryTag, false);

                var msg = Encoding.UTF8.GetString(e.Body.ToArray());
                listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(msg)));
                var message1 = Encoding.UTF8.GetString(e.Body.ToArray());
                listBox2.Invoke((MethodInvoker)(() => listBox2.Items.Add(message1)));
               
            }
            else
            {
                channel_message = connection.CreateModel();
                channel_message.BasicAck(e.DeliveryTag, false);
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                listBox2.Invoke((MethodInvoker)(() => listBox2.Items.Add(message)));
            }
            



        }

        private void button2_Click(object sender, EventArgs e)
        {

            channel_message = connection.CreateModel();
            channel_message.BasicQos(0, 1, false);
            var channel_messageConsumer = new AsyncEventingBasicConsumer(channel_message);
            channel_messageConsumer.Received += channel_messageConsumer_Received;

            if (String.IsNullOrEmpty(textBox1.Text))
            {

                var queueName = channel_message.QueueDeclare().QueueName;
                channel_message.QueueBind(queue: queueName,
                                  exchange: "pubsub",
                                  routingKey: "");
                channel_message.BasicConsume(queueName, true, channel_messageConsumer);
            }
            else
            {

                var user = textBox1.Text;

                channel_message.BasicConsume(user, true, channel_messageConsumer);


            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            using (var channel = connection.CreateModel())
            {
                var user = textBox2.Text;
                channel.QueueDeclare(user, true, false, false);
                channel.QueueBind(user, "pubsub", user);
                channel.QueueBind(user, "direct", user);

            }
            MessageBox.Show("User added successfully.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (var channel = connection.CreateModel())
            {
                var user = textBox2.Text;
                channel.QueueDelete(user);
            }
            MessageBox.Show("User deleted successfully.");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (var channel = connection.CreateModel())
            {
                var user = textBox1.Text;
                try
                {
                    channel.ExchangeDeclare(exchange: "direct", type: ExchangeType.Direct);
                    channel.QueueBind(user, "direct", "useronly");

                    string message = richTextBox1.Text;
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "direct",
                        user,
                        basicProperties: null,
                        body: body);
                    MessageBox.Show("Message sent successfully.");
                }
                catch (Exception)
                {

                    MessageBox.Show("User is not added in the group!");
                }
                
            }
          
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}    
