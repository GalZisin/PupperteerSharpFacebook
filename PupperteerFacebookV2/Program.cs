using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.Json;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PupperteerFacebookV2
{

    class Program
    {
   

        private static NavigationOptions _navigationOptions = new NavigationOptions { WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Networkidle0 } };
        static async Task Main(string[] args)
        {

 
            //var configuration = new ConfigurationBuilder().AddJsonFile("appsetting.json", false, false).Build();

            //Configuration = builder.Build();

            //User = Configuration["Account:User"];

            //Pass = Configuration["Account:Pass"];

            const string url = "https://www.facebook.com/";
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            // Create an instance of the browser and configure launch options
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                DefaultViewport = null
            });


            int delay = 100;
            Page page = await browser.NewPageAsync();
            page.Request += Page_Request;
            page.Response += Page_Response;

            await page.GoToAsync(url);

            CConfiguration settings = new CConfiguration();

            string faceBookEmail = settings.User;
            string faceBookPassword = settings.Pass;
            //string faceBookEmail = "galzisin@walla.co.il";
            //string faceBookPassword = "k28spq1gal";

            //email
            var userNameSelector = "#email";
            await page.WaitForSelectorAsync(userNameSelector);
            await TypeFieldValue(page, userNameSelector, faceBookEmail, delay);

            //password
            var passwordSelector = "#pass";
            await TypeFieldValue(page, passwordSelector, faceBookPassword, delay);

            await page.Keyboard.PressAsync("Tab");

            await page.Keyboard.PressAsync("Enter");


            var SearchInputSelector = "label.rq0escxv.a8c37x1j.a5nuqjux.l9j0dhe7.k4urcfbm input";
            await page.WaitForSelectorAsync(SearchInputSelector);
            await TypeFieldValueSearch(page, SearchInputSelector, "JavaScript, React, and Node.js", delay);

            await page.ClickAsync("div.oajrlxb2.gs1a9yip");

            await page.GoToAsync("https://www.facebook.com/search/top/?q=JavaScript%2C%20React%2C%20and%20Node.js");


            await page.GoToAsync("https://www.facebook.com/groups/javascript.react.node/");

            Thread.Sleep(10000);

            Func<Task> scroll = null;

            scroll = new Func<Task>(async () =>
            {
                await page.Keyboard.DownAsync("ArrowDown");

            });
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(500);
                Console.WriteLine("scroll");
                await scroll();
            }




            //.oajrlxb2.g5ia77u1.qu0x051f.esr5mh6w.e9989ue4.r7d6kgcz.rq0escxv.nhd2j8a9.nc684nl6.p7hjln8o.kvgmc6g5.cxmmr5t8.oygrvhab.hcukyx3x.jb3vyjys.rz4wbd8a.qt6c0cv9.a8nywdso.i1ao9s8h.esuyzwwr.f1sip0of.lzcic4wl.oo9gr5id.gpro0wi8.lrazzd5p
            var jscode = @"() => {
                                  const selectors = Array.from(document.querySelectorAll(`div.du4w35lb.k4urcfbm.l9j0dhe7.sjgh65i0`));                                                                                
                                  return selectors.map( t=> {return {content: t.innerHTML}});
                                  }";



            var results = await page.EvaluateFunctionAsync<Content[]>(jscode);


            List<Data> authorNamesAndText = new List<Data>();

            foreach (var result in results)
            {
                Data data = new Data();
                await page.SetContentAsync(result.content);
                var elementHandle = await page.QuerySelectorAsync("a.oajrlxb2.g5ia77u1.qu0x051f.esr5mh6w.e9989ue4.r7d6kgcz.rq0escxv.nhd2j8a9.nc684nl6.p7hjln8o.kvgmc6g5.cxmmr5t8.oygrvhab.hcukyx3x.jb3vyjys.rz4wbd8a.qt6c0cv9.a8nywdso.i1ao9s8h.esuyzwwr.f1sip0of.lzcic4wl.oo9gr5id.gpro0wi8.lrazzd5p ");
                var content = await elementHandle.QuerySelectorAsync("span").EvaluateFunctionAsync<string>("node => node.innerText");

                var content2 = await page.QuerySelectorAsync("div.kvgmc6g5.cxmmr5t8.oygrvhab.hcukyx3x.c1et5uql.ii04i59q").EvaluateFunctionAsync<string>("node => node.innerText");
                data.posttext = content2;
                data.author = content;
                authorNamesAndText.Add(data);
            }


            //print author and post list
            foreach (Data ap in authorNamesAndText)
            {
                Console.WriteLine(ap);
            }

            await browser.CloseAsync();


        }
        private static void Page_Response(object sender, ResponseCreatedEventArgs e)
        {
            Console.WriteLine(e.Response.Status);
        }

        private static void Page_Request(object sender, RequestEventArgs e)
        {
            Console.WriteLine(e.Request.ResourceType.ToString());
            Console.WriteLine(e.Request.Url);
        }
        private static async Task TypeFieldValue(Page page, string fieldSelector, string value, int delay = 0)
        {
            await page.FocusAsync(fieldSelector);
            await page.TypeAsync(fieldSelector, value, new TypeOptions { Delay = delay });
            await page.Keyboard.PressAsync("Tab");
        }
        private static async Task TypeFieldValueSearch(Page page, string fieldSelector, string value, int delay = 0)
        {
            await page.FocusAsync(fieldSelector);
            await page.TypeAsync(fieldSelector, value, new TypeOptions { Delay = delay });
        }

    }
    public class Data
    {
        public string author { get; set; }
        public string posttext { get; set; }
    }

    public class Content
    {
        public string content { get; set; }

    }
}

