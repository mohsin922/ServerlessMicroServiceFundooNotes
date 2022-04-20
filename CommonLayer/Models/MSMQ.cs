using Experimental.System.Messaging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CommonLayer.Models
{
    public class MSMQ
    {
        //    private MessageQueue messagequeue = new MessageQueue();

        //    public void Sender(string token)
        //    {
        //        this.messagequeue.Path = @".\pivate$\Tokens";
        //        try
        //        {
        //            if (MessageQueue.Exists(this.messagequeue.Path))
        //            {
        //                MessageQueue.Create(this.messagequeue.Path);

        //            }
        //            this.messagequeue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
        //            //register the Method to the event 
        //            this.messagequeue.ReceiveCompleted += MessageQue_RecivedCompleted;
        //            this.messagequeue.Send(token);
        //            this.messagequeue.BeginReceive();
        //            this.messagequeue.Close();
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }

        //    }

        //    private void MessageQue_RecivedCompleted(object sender, ReceiveCompletedEventArgs e)
        //    {
        //        var message = this.messagequeue.EndReceive(e.AsyncResult);
        //        string token = message.Body.ToString();
        //        try
        //        {
        //            MailMessage mailmessage = new MailMessage();
        //            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        //            {
        //                Port = 587,
        //                Credentials = new NetworkCredential("mohsinzahoor386@gmail.com", "Mohsin@386"),
        //                EnableSsl = true,
        //            };
        //            mailmessage.From = new MailAddress("mohsinzahoor386@gmail.com", "Mohsin@386");

        //            mailmessage.To.Add(new MailAddress("mohsinzahoor386@gmail.com"));
        //            mailmessage.Body = token;
        //            mailmessage.Subject = "Forget Password reset link";
        //            smtpClient.Send(mailmessage);
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        MessageQueue messageQueue = new MessageQueue();
        public void MSMQSender(string token)
        {
            messageQueue.Path = @".\private$\Token";//for windows path

            if (!MessageQueue.Exists(messageQueue.Path))
            {

                MessageQueue.Create(messageQueue.Path);

            }
            messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            messageQueue.ReceiveCompleted += MessageQueue_ReceiveCompleted;
            messageQueue.Send(token);
            messageQueue.BeginReceive();
            messageQueue.Close();
        }

        private void MessageQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var message = messageQueue.EndReceive(e.AsyncResult);
            string token = message.Body.ToString();
            //string Subject = "Fundoo Notes Reset Link";
            //string Body = "Dear Sir/mam! Please copy the token provided for Resetting your Password:" + token;
            MailMessage mailmessage = new MailMessage();
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("mohsinzahoor386@gmail.com", "Mohsin@123"),
                EnableSsl = true,
            };
            //smtpClient.Send("mohsinzahoor386@gmail.com", token, Subject, Body);
            mailmessage.From = new MailAddress("mohsinzahoor386@gmail.com", "Mohsin@123");

            mailmessage.To.Add(new MailAddress("mohsinzahoor386@gmail.com"));
            mailmessage.Body = token;
            mailmessage.Subject = "Dear Sir/mam! Please copy the token provided for Resetting your Password:" + token;
            smtpClient.Send(mailmessage);
            messageQueue.BeginReceive();
        }
    }
}
