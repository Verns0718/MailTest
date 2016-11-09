using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using System.Configuration;
namespace Examples.SmptExamples.Async
{
    public class SimpleAsynchronousExample
    {
        static bool mailSent = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }

        public static void Main(string[] args)
        {
            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient("10.0.2.15",25);
            //client.Credentials = new System.Net.NetworkCredential("e871239", "qtyepjyshnitfbrf");
            //client.EnableSsl = true;
            //client.EnableSsl = false;
            // Specify the e-mail sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            MailAddress from = new MailAddress("pierrot@localhost",
               "Vern_Wu",
            System.Text.Encoding.UTF8);
            // Set destinations for the e-mail message.
            MailAddress to = new MailAddress(Replacedomain("e871239@yahoo.com.tw"));
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            Console.WriteLine("Please type mail data!");
            message.Body = Console.ReadLine(); //"測試信件，此為信件內容";
            // Include some non-ASCII characters in body and subject.
            string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
            message.Body += Environment.NewLine + someArrows;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject ="測試MailHog郵件";
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new
            SendCompletedEventHandler(SendCompletedCallback);
            // The userState can be any object that allows your callback 
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            string userState = "test message1";
            client.SendAsync(message, userState);
            Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
            string answer = Console.ReadLine();
            // If the user canceled the send, and mail hasn't been sent yet,
            // then cancel the pending operation.
            if (answer.StartsWith("c") && mailSent == false)
            {
                client.SendAsyncCancel();
            }
            // Clean up.
            message.Dispose();
            Console.WriteLine("Goodbye.");
        }

        //讀取config Mailinator變數，若為1則自動取代原地址domain
        public static string Replacedomain(string address)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["Mailinator"] == "1")
            {
                address = address.Substring(0, address.IndexOf("@")) + "@mailinator.com";
            }
            return address;
        }
    }
}