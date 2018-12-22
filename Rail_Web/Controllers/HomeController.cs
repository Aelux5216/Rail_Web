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
using Newtonsoft.Json;

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
            //Get depart id
            string raw = Request.GetDisplayUrl();
            string departID = Request.GetDisplayUrl().Split('(')[1].Trim(')');
            string departIDF = departID.Split('/')[0].Trim(')');
            string arrivalID = Request.GetDisplayUrl().Split('(')[2].Trim(')');

            int serviceNo = 0;

            //string builder = "GetOne{" + serviceNo + '{' + departIDF + '{' + arrivalID; //Make sure this still works

            string builder = "GetAll{" + departIDF + "{" + arrivalID;

            //Grab times from middleware
            Send(builder);

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
            public byte[] buffer = new byte[4096];
        }

        ClientInfo client = new ClientInfo(); //Create new instance of client class.

        public void Send(string data)
        {
            client.socket = new TcpClient(); //Set socket to null first before reconnecting.

            try
            {
                client.socket.Connect(IPAddress.Parse("127.0.0.1"), 8001); //Handle no python connection.
            }

            catch
            {

            }

            if (client.socket.Connected == true)
            {
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

            else
            {
                resultModel.error = "Python client not available.";
            }
        }

        public void Read()
        {
            /*try
            {*/
                var stream = client.socket.GetStream(); //Get the socket stream.
                int i = client.stream.Read(client.buffer, 0, 4096);
                StringBuilder sb = new StringBuilder();

                string resultString = Encoding.UTF8.GetString(client.buffer).Trim('\0');

                if (resultString == "NoServices" || resultString == "NoConn")
                {
                    resultModel.error = resultString;
                }

                else
                {

                List<Service> serviceList = new List<Service>();
                
                //Try to deserialize if fails then most likely error message read this and display on screen as appropriate.
                List<string> resultstring1;

                resultstring1 = JsonConvert.DeserializeObject<List<string>>(resultString);

                foreach (string s2 in resultstring1)
                {
                    Service temp = JsonConvert.DeserializeObject<Service>(s2);
                    serviceList.Add(temp);
                }

                List<Service> serviceList1 = serviceList;

                foreach (Service s in serviceList1)
                {
                    List<CallingPoints> temp = new List<CallingPoints>();

                    foreach (string item in s.Calls_at_Temp)
                    {
                        temp.Add(JsonConvert.DeserializeObject<CallingPoints>(item));
                    }

                    s.Calls_at = temp;
                }

                resultModel.resultValue = serviceList1;

            }
        }

            /*catch
            {

            }*/
        }
    }

