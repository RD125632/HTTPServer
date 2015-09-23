using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Server
    {
        private HttpListener listener;
        private List<string> message;

        public static void Main()
        {
            new Server();
        }

        public Server()
        {
            message = new List<string>();

            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add("http://*:80/");
                listener.Start();

                List<string> page0 = new List<string>();
                page0.Add("HTTP/1.0 200 OK\r\n");
                page0.Add("Content-type: text/plain \r\n \r\n");
                page0.Add("Hallo, dit is mijn internet pagina met tekst");

                List<string> page1 = new List<string>();
                page1.Add("HTTP/1.0 200 OK\r\n");
                page1.Add("Content-type: text/html \r\n \r\n");
                page1.Add("Dit is de eerste pagina. <a href='/page2'>Ga naar pagina 2</a>");

                List<string> page2 = new List<string>();
                page2.Add("HTTP/1.0 200 OK\r\n");
                page2.Add("Content-type: text/html \r\n \r\n");
                page2.Add("Dit is pagina 2 <a href='/page1'>terug</a>");



                while (true)
                {
                    Console.WriteLine("waiting for client...");
                    HttpListenerContext context = listener.GetContext();

                    string[] strParams = context.Request.Url.Segments.Skip(1).Select(s => s.Replace("/", "")).ToArray();


                    switch (strParams[0])
                    {
                        case "a":
                            message = page0;
                            break;
                        case "b":
                            if (strParams[1] == "page2")
                            {
                                message = page2;
                            }
                            else
                            {
                                message = page1;
                            }
                            break;

                        case "c":
                            loadPage(strParams[1]);
                            break;
                    }


                    int ByteCount = 0;

                    foreach (string line in message)
                    {
                        ByteCount += Encoding.UTF8.GetByteCount(line);
                    }

                    context.Response.ContentLength64 = ByteCount;
                    context.Response.StatusCode = (int)HttpStatusCode.OK;

                    using (Stream stream = context.Response.OutputStream)
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            foreach (string line in message)
                            {
                                writer.Write(line);
                            }
                        }
                    }
                    Console.WriteLine("page loaded...");
                    context.Response.OutputStream.Close();
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Status);
            }
        }



        private void loadPage(string param)
        {
            message = new List<string>();
            string resource_data = "";
            switch (param)
            {
                case "page1":
                    resource_data = Properties.Resources.page1;
                    break;
                case "page2":
                    resource_data = Properties.Resources.page2;
                    break;
                case "page3":
                    resource_data = Properties.Resources.page3;
                    break;
                case "page4":
                    resource_data = Properties.Resources.page4;
                    break;
            }
            
            string[] words = resource_data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string lines in words)
            {
                message.Add(lines);
            }
        }

    }
}
