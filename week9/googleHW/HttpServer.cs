using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using googleHW.Attributes;

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
        
        private byte[]? MethodHandler(HttpListenerContext _httpContext, HttpListenerResponse response)
        {
            // // объект запроса
            // HttpListenerRequest request = _httpContext.Request;
            //
            // // объект ответа
            // HttpListenerResponse response = _httpContext.Response;

            if (_httpContext.Request.Url.Segments.Length < 2) return null;

            string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");
            

            var assembly = Assembly.GetExecutingAssembly();

            // походу здесь проверку на наличие названия Controller в атрибуте
            var controller = assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                .FirstOrDefault(c => c.Name.ToLower() == controllerName.ToLower());

            if (controller == null) return null;

            var test = typeof(HttpController).Name;
            

            var methods = controller.GetMethods().Where(t => t.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"));
            
            var method = methods.FirstOrDefault();

            var segments = _httpContext.Request.Url.Segments;
            if (segments.Length == 3)
            {
                if (segments[1] == "accounts/" && int.TryParse(segments[2], out int _))
                    method = methods.Where(m => m.Name == "GetAccountById").First();
            }
            
            if (method == null) return null;
            
            var values = _httpContext.Request.InputStream;

            var queryParams = GetQuery(_httpContext, method);
            
            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);
            response.ContentType = "Application/json";

            byte[] buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
            return buffer;
        }

        private object[] GetQuery(HttpListenerContext listener, MethodInfo method)
        {
            string[] strParams = listener.Request.Url
                .Segments
                .Skip(2)
                .Select(s => s.Replace("/", ""))
                .ToArray();
            foreach (var parameter in method.GetParameters())
            {
                if(parameter.ParameterType == typeof(HttpListenerContext))
                    return new object[] {listener};
            }
            if (listener.Request.HttpMethod == "GET")
            {
                
                return method.GetParameters()
                    .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                    .ToArray();
            }
            return new object[] {listener};
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
                byte[]? buffer = new byte[] { };
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                if (Directory.Exists(PATH))
                {
                    buffer = _inspector.getFile(request.RawUrl, PATH);

                    if (buffer == null)
                    {
                        buffer = MethodHandler(context, response);
                        if (buffer == null)
                        {
                            response.Headers.Set("Content-Type", "text/plain");
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            string err = "404 - not found";
                            buffer = Encoding.UTF8.GetBytes(err);
                        }
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
