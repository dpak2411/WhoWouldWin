using System;
using System.Collections.Generic;
using AlteryxGalleryAPIWrapper;
using AnalogStoreAnalysis;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace WhoWouldWin
{
    [Binding]
    public class WhoWOuldWInSteps
    {
        public string alteryxurl;
        public string _sessionid;
        private string _appid;
        private string _userid;
        private string _appName;
        private string jobid;
        private string outputid;
        private string validationId;

        // public delegate void DisposeObject();
        //private Client Obj = new Client("https://devgallery.alteryx.com/api/");
        Client Obj = new Client("https://gallery.alteryx.com/api");
        RootObject jsString = new RootObject();


        [Given(@"alteryx running at ""(.*)""")]
        public void GivenAlteryxRunningAt(string url)
        {
            alteryxurl = url;
        }
        
        [Given(@"I am logged in using ""(.*)"" and ""(.*)""")]
        public void GivenIAmLoggedInUsingAnd(string user, string password)
        {
            _sessionid = Obj.Authenticate(user, password).sessionId;
        }
        
        [When(@"I run analog store analysis with ThisMatchUP ""(.*)"" AddChallengers ""(.*)""")]
        public void WhenIRunAnalogStoreAnalysisWithThisMatchUPAddChallengers(string p0, string AddChallenger)
        {
            string response = Obj.SearchApps("Who Would Win");
            var appresponse = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(response);
            int count = appresponse["recordCount"];
            _userid = appresponse["records"][0]["owner"]["id"];
            _appName = appresponse["records"][0]["primaryApplication"]["fileName"];
            _appid = appresponse["records"][0]["id"];
            jsString.appPackage.id = _appid;
            jsString.userId = _userid;
            jsString.appName = _appName;
            string appinterface = Obj.GetAppInterface(_appid);
            dynamic interfaceresp = JsonConvert.DeserializeObject(appinterface);

            List<Jsonpayload.Question> questionAnsls = new List<Jsonpayload.Question>();


            var selectedValue = new List<Jsonpayload.datac>();
            selectedValue.Add(new Jsonpayload.datac() { key = "/Captain America|Captain Ted Striker\"", value = "false" });

            selectedValue.Add(new Jsonpayload.datac() { key = "/Captain Ted Striker|Captain America\"", value = "true" });

            string ValueSelected = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(selectedValue);
            //string AtTrueValue = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(valueTrue);
          //  string NumOfBath = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(Bath);
            for (int i = 0; i < 1; i++)
            {
                if (i == 0)
                {
                    Jsonpayload.Question questionAns = new Jsonpayload.Question();
                    questionAns.name = "Winner";
                    questionAns.answer = ValueSelected;
                    jsString.questions.Add(questionAns);
                }
                //else if (i == 1)
                //{
                //    questionAnsls.Add(new Jsonpayload.Question("Add Challenger", AddChallenger));
                //}
                //else
                //{
                //    //Jsonpayload.Question questionAns = new Jsonpayload.Question();
                //    //questionAns.name = "Add Challenger";
                //    //questionAns.answer = AddChallenger;
                //    //jsString.questions.Add(questionAns);

                //}
            }
            questionAnsls.Add(new Jsonpayload.Question("Add Challenger", AddChallenger));
                jsString.jobName = "Job Name";
                var postData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(jsString);
                string postdata = postData.ToString();
                string resjobqueue = Obj.QueueJob(postdata);
                var jobqueue = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(resjobqueue);
                jobid = jobqueue["id"];

                int counts = 0;
                string status = "";

            CheckValidate:
                System.Threading.Thread.Sleep(1000);
                if (status == "Completed" && counts < 15)
                {
                    //string disposition = validationStatus.disposition;
                }
                else if (counts < 15)
                {
                    string jobstatusresp = Obj.GetJobStatus(jobid);
                    var statusResponse = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(jobstatusresp);
                    status = statusResponse["status"];
                    goto CheckValidate;
                }

                else
                {
                    throw new Exception("Complete Status Not found");

                }
            
        }
        
        [Then(@"I see the WhoWouldWin result ""(.*)""")]
        public void ThenISeeTheWhoWouldWinResult(string result)
        {
            string getmetadata = Obj.GetOutputMetadata(jobid);
            dynamic metadataresp = JsonConvert.DeserializeObject(getmetadata);
            int count = metadataresp.Count;
            for (int j = 0; j <= count - 1; j++)
            {
                outputid = metadataresp[j]["id"];
            }
            string getjoboutput = Obj.GetJobOutput(jobid, outputid, "html");
            string htmlresponse = getjoboutput;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlresponse);
            string output = doc.DocumentNode.SelectSingleNode("//div[@class='DefaultText']").InnerText;
            StringAssert.Contains(result, output);
        }
    }
}
