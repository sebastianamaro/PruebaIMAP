using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using Org.BouncyCastle.OpenSsl;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace FormIMAP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var client = new ImapClient())
            {
                var credentials = new NetworkCredential("", "");
                var uri = new Uri("imaps://imap.gmail.com");

                using (var cancel = new CancellationTokenSource())
                {
                    client.Connect(uri, cancel.Token);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH");

                    client.Authenticate(credentials, cancel.Token);

                    // The Inbox folder is always available on all IMAP servers...
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly, cancel.Token);

                    Console.WriteLine("Total messages: {0}", inbox.Count);
                    Console.WriteLine("Recent messages: {0}", inbox.Recent);
                    string txt = "";
                    for (int i = 0; i < inbox.Count; i++)
                    {
                        var message = inbox.GetMessage(i, cancel.Token);
                        txt+=message.Subject+Environment.NewLine;
                    }
                    MessageBox.Show(txt);
                    client.Disconnect(true, cancel.Token);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MailMessage m= new MailMessage();

            m.From = new MailAddress("yy@gmail.com", "Joey Tribbiani");
            m.To.Add(new MailAddress("xx@gmail.com", "Mrs. Chanandler Bong"));
            m.Subject = "How you doin'?";
            m.Body =  @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"
            ;
            m.Attachments.Add(new Attachment(@"D:\Cosas Seba\Facu\CalendarioAcademico2011.pdf"));
            var message = MimeMessage.CreateFromMailMessage(m);
            using (var client = new SmtpClient())
            {
                var credentials = new NetworkCredential("", "");

                // Note: if the server requires SSL-on-connect, use the "smtps" protocol instead
                var uri = new Uri("smtp://in-v3.mailjet.com:587");

                using (var cancel = new CancellationTokenSource())
                {
                    client.Connect(uri, cancel.Token);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(credentials, cancel.Token);

                   // client.Send(message, new MailboxAddress("matias.dumrauf@gmail.com", "Joey Tribbiani"), message.To.Mailboxes, cancel.Token);
                  client.Send(message, cancel.Token);
                    client.Disconnect(true, cancel.Token);
                    client.Dispose();
                   
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            using (var client = new ImapClient())
            {
                var credentials = new NetworkCredential("", "");
                var uri = new Uri("imaps://imap.gmail.com");

                using (var cancel = new CancellationTokenSource())
                {
                    client.Connect(uri, cancel.Token);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH");

                    client.Authenticate(credentials, cancel.Token);

                    // The Inbox folder is always available on all IMAP servers...
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly, cancel.Token);


                    var query = SearchQuery.DeliveredAfter(DateTime.Parse("2013-01-12")).And(SearchQuery.SubjectContains("Dropbox")).And(SearchQuery.Seen);
                    string txt = "";
                    foreach (var uid in inbox.Search(query, cancel.Token))
                    {
                        var message = inbox.GetMessage(uid, cancel.Token);
                        txt+= uid+"-" +message.Subject+ Environment.NewLine;
                    }
                    MessageBox.Show(txt);
                    client.Disconnect(true, cancel.Token);
                }
            }

            
        }
    }
}
