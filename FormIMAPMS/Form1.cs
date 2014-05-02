using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ActiveUp.Net.Mail;
using Message = ActiveUp.Net.Mail.Message;
namespace FormIMAPMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Client.ConnectSsl("imap.gmail.com", 993);
            Client.Login("cloudifydtest", "cloudifyd123");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IEnumerable<Message> m = GetAllMails("INBOX");
            foreach (var message in m)
            {
                Console.WriteLine(message.BodyText);
            }

        }

        private Imap4Client _client = null;


        /**
         * 
         *
         * this.AddLogEntry("Creating message.");

                    
         */
        public IEnumerable<Message> GetAllMails(string mailBox)
        {
            return GetMails(mailBox, "ALL").Cast<Message>();
        }

        public IEnumerable<Message> GetUnreadMails(string mailBox)
        {
            return GetMails(mailBox, "UNSEEN").Cast<Message>();
        }

        protected Imap4Client Client
        {
            get
            {
                if (_client == null)
                    _client = new Imap4Client();
                return _client;
            }
        }

        private MessageCollection GetMails(string mailBox, string searchPhrase)
        {
            Mailbox mails = Client.SelectMailbox(mailBox);
            MessageCollection messages = mails.SearchParse(searchPhrase);
            return messages;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // We create the message object

            ActiveUp.Net.Mail.SmtpMessage message = new ActiveUp.Net.Mail.SmtpMessage();

            // We assign the sender email
            message.From.Email = "LALA@lala.com";

            // We assign the recipient email
            message.To.Add("cloudifydtest@gmail.com");

            // We assign the subject
            message.Subject = "PUTO";

            // We add the embedded objets.
            message.BodyHtml.Text = "The message doens't contain embedded objects.";


            message.BuildMimePartTree();
            message.Send("smtps://smtp.gmail.com", 587,"cloudifydtest", "cloudifyd123", SaslMechanism.Login);

        }
    }
}
