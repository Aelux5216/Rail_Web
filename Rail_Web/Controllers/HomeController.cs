using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Rail_Web.Models;
using System.Text;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace Rail_Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            TempData["stations"] = getStations();
            return View();
        }

        public IActionResult Result()
        {
            //Get current id
            string s = Request.GetDisplayUrl().Split('(')[1].Trim(')');

            s = "test{" + s;

            //Grab times from middleware
            Send(s);

            Read();

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public List<string> getStations()
        {
            List<string> stations = new List<string>();

            //Download latest stations

            WebClient client = new System.Net.WebClient();

            Uri url = new Uri("http://www.nationalrail.co.uk/static/documents/content/station_codes.csv", UriKind.Absolute);

            try
            {
                client.DownloadFile(url, "StationList.csv");
            }

            catch
            {

            }
            
            //Setup reader for file
            StreamReader reader = null;
            List<string> stationList = new List<string>();
            bool fileExists = false;

            //Check if the txt file exists before trying to use
            try
            {
                reader = new StreamReader("StationList.csv");
                fileExists = true;
            }
            //If file doesn't exist try other dfs clusters.
            catch
            {

            }

            if (fileExists == true)
            {
                string output;

                reader.ReadLine();

                while ((output = reader.ReadLine()) != null)
                {
                    string output2 = output.Split(',')[0] + " (" + output.Split(',')[1] + ")";
                    stationList.Add(output2);
                }
            }

            return stationList;
        }

        public class ClientInfo //TCP client class.
        {
            public TcpClient socket = null;    //Initalize default tcpclient values.
            public NetworkStream stream = null;
            public byte[] buffer = new byte[1024];
        }

        ClientInfo client = new ClientInfo(); //Create new instance of client class.

        public void Send(string data)
        {
            client.socket = new TcpClient(); //Set socket to null first before reconnecting.

            client.socket.Connect(IPAddress.Parse("127.0.0.1"), 8000);
            client.stream = client.socket.GetStream();

            var bytes = Encoding.UTF8.GetBytes(data); //Get the bytes of the input data.
            try
            {
                var stream = client.socket.GetStream(); //Get the socket stream.
                stream.Write(bytes, 0, bytes.Length); //Begin writing data until the end of the amount of bytes while passing to async callback method.
            }

            catch
            {
                //If this fails make sure client is connected to the server.
            }
        }

        public void Read()
        {
            try
            {
                var stream = client.socket.GetStream(); //Get the socket stream.
                int i = client.stream.Read(client.buffer, 0, 1024);
                StringBuilder sb = new StringBuilder();

                string resultString = Encoding.UTF8.GetString(client.buffer).Trim('\0');

                resultModel.resultValue = resultString.Split('{').ToList();
            }

            catch
            {

            }
        }
    }
}
