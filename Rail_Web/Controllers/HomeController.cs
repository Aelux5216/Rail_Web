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

        public IActionResult ResultLoad()
        {
            //Get current id
            string s = Request.GetDisplayUrl().Split('(')[1].Trim(')');

            s = "test{" + s;

            //Grab times from middleware
            Send(s);

            Read();

            return RedirectToAction("Result");
        }

        public IActionResult Result()
        {
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
            public byte[] buffer = new byte[8192];
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
                stream.BeginWrite(bytes, 0, bytes.Length,EndSend, bytes); //Begin writing data until the end of the amount of bytes while passing to async callback method.
            }

            catch
            {
                 //If this fails make sure client is connected to the server.
            }
        }

       public void EndSend(IAsyncResult result)
       {
            var bytes = (byte[])result.AsyncState; //Get the info to bytes and finish sending.
       }

        public void Read()
        {
            try
            {
                var stream = client.socket.GetStream(); //Get the socket stream.
                client.stream.BeginRead(client.buffer, 0, client.buffer.Length, EndRead, client.buffer); //Send the data until length of buffer passing the process over to an async thread callback.
            }

            catch
            {

            }
        }

        public void EndRead(IAsyncResult result)
        {
            var stream = client.socket.GetStream(); //Get the stream as it is out of context now.
            int endBytes = stream.EndRead(result); //Find out how much data is left to read.

            var buffer = (byte[])result.AsyncState; //Create buffer based on the previous read method.
            string data = Encoding.UTF8.GetString(buffer, 0, endBytes); //Get the string from the data.

            resultModel.resultValue = data.Split('{').ToList();
        }
    }
}
