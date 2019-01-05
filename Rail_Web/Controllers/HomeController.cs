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

        public IActionResult Result(string dep, string arr)
        {
            string builder = "GetAll{" + dep + "{" + arr;

            //Grab times from middleware
            Send(builder);

            Read();

            return View(resultModel.modelInstance);
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
            public byte[] buffer = new byte[204800]; //Accomidate for big amount of departures.
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
                var bytes = Encoding.UTF8.GetBytes(data); //Get the bytes of the input data.

                try
                {
                    client.stream = client.socket.GetStream();
                    client.stream.Write(bytes, 0, bytes.Length); //Begin writing data until the end of the amount of bytes while passing to async callback method.
                }

                catch
                {
                    //If this fails make sure client is connected to the server.
                }
            }

            else
            {
                resultModel r = new resultModel { error = "Python client not available." };
                resultModel.modelInstance = r;
            }
        }

        public void Read()
        {
            try
            {
                client.stream = client.socket.GetStream(); //Get the socket stream.
                client.stream.Read(client.buffer, 0, client.buffer.Length);

                string dataRecieved = Encoding.UTF8.GetString(client.buffer).Trim('\0');

                resultModel r = new resultModel();

                if (dataRecieved == "NoServices" || dataRecieved == "NoConn")
                {
                    r.error = dataRecieved;
                    resultModel.modelInstance = r;
                }

                else
                {
                    List<Service> serviceList = new List<Service>();

                    //Try to deserialize if fails then most likely error message read this and display on screen as appropriate.
                    List<string> resultstring1;

                    resultstring1 = JsonConvert.DeserializeObject<List<string>>(dataRecieved);

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

                    r.resultValue = serviceList1;
                    resultModel.modelInstance = r;
                }
            }

            catch
            {
                resultModel r = new resultModel();

                r.error = "TechIssues"; //Can't connect to python client. 

                resultModel.modelInstance = r;
            }
        }

        public IActionResult Ticket(string refer, string arrCode, bool std, bool fst)
        {
            Send("GetOne" + "{" + refer + "{" + arrCode);

            Read();

            string classType = null;

            string cost = null;

            if (std == true && fst == false)
            {
                classType = "Standard";
                cost = "£15.00";
            }

            else if (std == false && fst == true)
            {
                classType = "First Class";
                cost = "£20.00";
            }

            DateTime timestamp = DateTime.UtcNow;

            Random rnd = new Random();

            string randomList = "abcdefghijklmnopqrstuvwxyz0123456789";

            string reference = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", randomList[rnd.Next(0, 35)], 
                randomList[rnd.Next(0, 35)], randomList[rnd.Next(0, 35)], randomList[rnd.Next(0, 35)], 
                randomList[rnd.Next(0, 35)], randomList[rnd.Next(0, 35)], randomList[rnd.Next(0, 35)], 
                randomList[rnd.Next(0, 35)], randomList[rnd.Next(0, 35)]);

            Service r = resultModel.modelInstance.resultValue[0];

            Ticket returnedTicket = new Ticket { reference = reference, arrCode = arrCode, classType = classType, date = timestamp.Date,
                time = timestamp.ToShortTimeString(), depCode = r.dep_code, totalCost = cost };

            resultModel.modelInstance.ticketInstance = returnedTicket;

            return View(resultModel.modelInstance);
        }

    }
}

