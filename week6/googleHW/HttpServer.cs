using System.Net;
using System.Text;
using System.Text.Json;

namespace googleHW
{
    public class HttpServer
    {
        HttpListener listener;
        private ServerSettings _serverSetting;
        private FileInspector _inspector = new FileInspector();
        string PATH = Directory.GetCurrentDirectory() + "/site";

        public HttpServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8888/");
        }

        public void StartServer()
        {
            _serverSetting = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllBytes("./settings.json"));
            listener.Prefixes.Clear();
            listener.Prefixes.Add($"http://localhost:{_serverSetting.Port}/");
            PATH = _serverSetting.Path;
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

                if (Directory.Exists(PATH))
                {
                    buffer = _inspector.getFile(request.RawUrl, PATH);

                    if (buffer == null)
                    {
                        response.Headers.Set("Content-Type", "text/plain");

                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        string err = "404 - not found";
                        buffer = Encoding.UTF8.GetBytes(err);
                    }
                    else
                    {
                        var _type = _inspector.getContentType(request.RawUrl);
                        response.Headers.Set("Content-Type", _type);
                    }
                }
                else
                {
                    string err = $"Directory '{PATH}' not found";
                    buffer = Encoding.UTF8.GetBytes(err);
                }

                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                response.Close();
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
