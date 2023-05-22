using System;
using System.Collections.Generic;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EmailClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Aplicație de gestionare a email-urilor");

            while (true)
            {
                Console.WriteLine("1. Afiseaza email-urile din cutia postala (POP3)");
                Console.WriteLine("2. Afiseaza email-urile din cutia postala (IMAP)");
                Console.WriteLine("3. Descarca un email cu atasamente");
                Console.WriteLine("4. Trimite un email doar cu text");
                Console.WriteLine("5. Trimite un email cu atasament");
                Console.WriteLine("6. Ieșire");
                Console.Write("Alege o opțiune: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ShowEmailsPOP3();
                        break;
                    case "2":
                        ShowEmailsIMAP();
                        break;
                    case "3":
                        DownloadEmailWithAttachments();
                        break;
                    case "4":
                        SendTextEmail();
                        break;
                    case "5":
                        SendEmailWithAttachment();
                        break;
                    case "6":
                        Console.WriteLine("La revedere!");
                        return;
                    default:
                        Console.WriteLine("Opțiune invalidă. Vă rugăm să încercați din nou.");
                        break;
                }

                Console.WriteLine();
            }
        }

        static void ShowEmailsPOP3()
        {
            using (var client = new Pop3Client())
            {
                // Conectare la serverul POP3
                client.Connect("pop.gmail.com", 995, SecureSocketOptions.SslOnConnect);

                // Autentificare
                client.Authenticate("negaraalex25@gmail.com", "tgcissgheqbhcztg");

                // Obținerea numărului total de email-uri din cutia poștală
                int count = client.GetMessageCount();
                Console.WriteLine($"Numărul total de email-uri: {count}");

                // Afișarea subiectului fiecărui email
                for (int i = 0; i < 10; i++)
                {
                    var message = client.GetMessage(i);
                    Console.WriteLine($"Subiect: {message.Subject}");
                }

                // Deconectare de la server
                client.Disconnect(true);
            }
        }

        static void ShowEmailsIMAP()
        {
            using (var client = new ImapClient())
            {
                // Conectare la serverul IMAP
                client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);

                // Autentificare
                client.Authenticate("negaraalex25@gmail.com", "tgcissgheqbhcztg");

                // Deschiderea folderului Inbox
                client.Inbox.Open(FolderAccess.ReadOnly);

                // Obținerea numărului total de email-uri din Inbox
                int count = client.Inbox.Count;
                Console.WriteLine($"Numărul total de email-uri: {count}");

                // Afișarea subiectului fiecărui email
                for (int i = 0; i < 10; i++)
                {
                    var message = client.Inbox.GetMessage(i);
                    Console.WriteLine($"Subiect: {message.Subject}");
                }

                // Deconectare de la server
                client.Disconnect(true);
            }
        }

        static void DownloadEmailWithAttachments()
        {
            using (var client = new Pop3Client())
            {
                // Conectare la serverul POP3
                client.Connect("pop.gmail.com", 995, SecureSocketOptions.SslOnConnect);

                // Autentificare
                client.Authenticate("negaraalex25@gmail.com", "tgcissgheqbhcztg");

                // Descărcarea primului email cu atașamente
                var message = client.GetMessage(0);
                Console.WriteLine($"Subiect: {message.Subject}");

                // Căutăm atașamentele în corpul mesajului
                var multipart = message.Body as Multipart;

                if (multipart != null)
                {
                    foreach (var part in multipart)
                    {
                        if (part is MimePart attachment)
                        {
                            var fileName = attachment.FileName;

                            using (var stream = System.IO.File.Create($"C:\\FolderDestinatie\\{fileName}"))
                            {
                                attachment.Content.DecodeTo(stream);
                            }
                        }
                    }
                }

                // Deconectare de la server
                client.Disconnect(true);
            }
        }



        static void SendTextEmail()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Nume Expeditor", "adresa@gmail.com"));
            message.To.Add(new MailboxAddress("Nume Destinatar", "destinatar@example.com"));
            message.Subject = "Subiectul email-ului";

            message.Body = new TextPart("plain")
            {
                Text = "Acesta este un email de test."
            };

            using (var client = new SmtpClient())
            {
                // Conectare la serverul SMTP
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // Autentificare
                client.Authenticate("negaraalex25@gmail.com", "tgcissgheqbhcztg");

                // Trimiterea email-ului
                client.Send(message);

                // Deconectare de la server
                client.Disconnect(true);
            }

            Console.WriteLine("Email trimis cu succes!");
        }

        static void SendEmailWithAttachment()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Nume Expeditor", "adresa@gmail.com"));
            message.To.Add(new MailboxAddress("Nume Destinatar", "destinatar@example.com"));
            message.Subject = "Subiectul email-ului cu atașament";

            var builder = new BodyBuilder();
            builder.TextBody = "Acesta este un email cu atașament.";

            // Adăugarea unui atașament
            builder.Attachments.Add("cale/catre/fisier.pdf");

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Conectare la serverul SMTP
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // Autentificare
                client.Authenticate("negaraalex25@gmail.com", "tgcissgheqbhcztg");

                // Trimiterea email-ului
                client.Send(message);

                // Deconectare de la server
                client.Disconnect(true);
            }

            Console.WriteLine("Email trimis cu succes!");
        }
    }
}
