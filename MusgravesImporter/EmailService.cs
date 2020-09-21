using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MusgravesImporter
{
 
        public class EmailCredential
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }

            public string DisplayName { get; set; }
            public string FromAddress { get; set; }
            public bool EnableSsl { get; set; }
            public int Port { get; set; }
            public string SmptAddress { get; set; }
        }
        public class EmailMessage 
        {
            public EmailMessage()
            {
                CarbonCopy = new List<MailAddress>();
                Attachments = new List<Attachment>();
                DestinationList = new List<string>();
            }
            public virtual MailAddress From { get; set; }
            public virtual string ReceiverName { get; set; }
            public virtual string FromFriendlyName { get; set; }
            public List<string> DestinationList { get; set; }
            public List<Attachment> Attachments { get; set; }
            public List<MailAddress> CarbonCopy { get; set; }
            public bool IsHtml { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }
        public class Email
        {
            public bool SendEmail(EmailMessage message)
            {

                var subject = message.Subject;
                var htmlBody = message.Body;
                var plainTextBody = message.Body;

                var mailMsg = new MailMessage();
                if (message.DestinationList.Count > 0)
                {
                    foreach (var email in message.DestinationList)
                    {
                        mailMsg.To.Add(new MailAddress(email));
                    }

                }



                if (message.CarbonCopy != null && message.CarbonCopy.Count > 0)
                {
                    foreach (var copy in message.CarbonCopy)
                    {
                        mailMsg.CC.Add(copy);
                    }
                }

                // From
                mailMsg.From = new MailAddress("reporting@interlincgroup.com");
            mailMsg.Body = message.Body;
                mailMsg.IsBodyHtml = true;


                // Subject and multi-part/alternative Body
                mailMsg.Subject = subject;
                mailMsg.Sender = new MailAddress("reporting@interlincgroup.com");
                if (message.Attachments != null && message.Attachments.Count > 0)
                {
                    foreach (var att in message.Attachments)
                    {
                        mailMsg.Attachments.Add(att);
                    }
                }

                var cred = GetEmailCredentials() ?? new EmailCredential();
                // Initialize SmtpClient and send
                using var smtpClient = new SmtpClient(cred.SmptAddress, cred.Port);
                var credentials = new NetworkCredential(cred.UserName, cred.Password);
                smtpClient.Credentials = credentials;
                smtpClient.Port = cred.Port;
                smtpClient.EnableSsl = cred.EnableSsl;
                try
                {
                    smtpClient.Send(mailMsg);
                    return true;
                }
                catch (Exception ex)
                {

                    return false;
                }
            }


            private EmailCredential GetEmailCredentials()
            {
                return new EmailCredential
                {
                    DisplayName ="Interlinc Reporting",
                    EnableSsl = true,
                    FromAddress = "blackwood0110@gmail.com",
                    Password = "hckzguhpdgsrhlgu",
                    Port = 587,
                    SmptAddress = "smtp.gmail.com",
                    UserName = "blackwood0110@gmail.com"


                };
            }
        }
    
}
