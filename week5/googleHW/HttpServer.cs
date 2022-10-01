using System.Net;

namespace googleHW
{
    public class HttpServer
    {
        HttpListener listener;

        public HttpServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8888/");
        }

        public void StartServer()
        {
            listener.Start();
            Console.WriteLine("Server started");
            Work();
        }

        
        private void Work()
        {
            bool working = true;
            while (working)
            {
                Listen();
                var command = Console.ReadLine();
                switch (command)
                {
                    case "stop":
                        StopServer("Server stopped");
                        break;
                    case "exit":
                        Console.WriteLine("Exit? y/n");
                        var inpt = Console.ReadLine();
                        if (inpt == "y")
                        {
                            StopServer("see ya");
                            working = false;
                        }
                        break;
                    case "start":
                        StartServer();
                        break;
                    case "throw":
                        StopServer("Test exception");
                        break;
                    case "ping":
                        {
                            if (listener.IsListening)
                                Console.WriteLine("connected");
                            break;
                        }
                    default:
                        Console.WriteLine("unknown operation");
                        break;
                }
            }
        }

        private async Task Listen() {
            
            while (true)
            {
                byte[] buffer = new byte[] { };
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                switch (request.RawUrl)
                {
                    case "/google":
                        Console.WriteLine("google");
                        if (!File.Exists(@"../../../../google.html"))
                            StopServer("File not found");    
                        else
                            buffer = File.ReadAllBytes(@"../../../../google.html");
                        break;
                }

                
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        public void StopServer(string message)
        {
            listener.Stop();
            Console.WriteLine(message);
            Console.Read();
        }
    }
}
