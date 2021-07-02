using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

namespace ChromeTest.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    public class PuppeteerPoCController : ControllerBase
    {
        [HttpGet( "{base64Url}" )]
        public async Task<TestingResult> Get( [FromRoute] string base64Url, [FromQuery] string launchName )
        {
            byte[] base64EncodedBytes = System.Convert.FromBase64String( base64Url );
            string url = System.Text.Encoding.UTF8.GetString( base64EncodedBytes );

            TestingResult result = new TestingResult
            {
                Url = url,
                LaunchName = launchName
            };

            var browser = await Puppeteer.LaunchAsync( new LaunchOptions
            {
                Headless = true
            } );
            var page = await browser.NewPageAsync();
            await page.GoToAsync( url );

            result.Html = await page.GetContentAsync();

            string jsSelectAllLinks = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
            string[] links = await page.EvaluateExpressionAsync<string[]>( jsSelectAllLinks );
            foreach ( string link in links )
            {
                result.Links.Add( link );
            }

            /*string jsSelectAllLogs = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
            string[] logs = await page.EvaluateExpressionAsync<string[]>( jsSelectAllLogs );
            var logs = chromeDriver.Manage().Logs.GetLog( LogType.Browser );
            foreach ( var log in logs )
            {
                result.Logs.Add( log.Message );
            }*/

            KeyValuePair<int, int>[] screenSizes = new KeyValuePair<int, int>[] {
                new KeyValuePair<int, int>(1920, 1080), new KeyValuePair<int, int>(1600, 900),
                new KeyValuePair<int, int>(1366, 768), new KeyValuePair<int, int>(1280, 1024),
                new KeyValuePair<int, int>(768, 1024), new KeyValuePair<int, int>(375, 667),
                new KeyValuePair<int, int>(375, 720), new KeyValuePair<int, int>(360, 640),
                new KeyValuePair<int, int>(320, 480) };

            /*FirefoxOptions firefoxOptions = new FirefoxOptions() { BrowserExecutableLocation = @"C:\Program Files\Mozilla Firefox\firefox.exe" };
            IWebDriver firefoxDriver = new OpenQA.Selenium.Firefox.FirefoxDriver( @"D:\Autotests\PATH", firefoxOptions );
            firefoxDriver.Navigate().GoToUrl( url );

            IWebDriver edgeDriver = new OpenQA.Selenium.Edge.EdgeDriver();
            edgeDriver.Navigate().GoToUrl( url );

            System.IO.Directory.CreateDirectory( $"D:\\Autotests\\ChromeTest\\screens\\WebDriverPoC\\{launchName}\\chrome" );
            System.IO.Directory.CreateDirectory( $"D:\\Autotests\\ChromeTest\\screens\\WebDriverPoC\\{launchName}\\firefox" );
            System.IO.Directory.CreateDirectory( $"D:\\Autotests\\ChromeTest\\screens\\WebDriverPoC\\{launchName}\\edge" );

            foreach ( var screenSize in screenSizes )
            {
                int width = screenSize.Key;
                int height = screenSize.Value;

                chromeDriver.Manage().Window.Size = new System.Drawing.Size( width, height );
                firefoxDriver.Manage().Window.Size = new System.Drawing.Size( width, height );
                edgeDriver.Manage().Window.Size = new System.Drawing.Size( width, height );

                ITakesScreenshot chromeScreenshotDriver = chromeDriver as ITakesScreenshot;
                Screenshot screenshot = chromeScreenshotDriver.GetScreenshot();
                screenshot.SaveAsFile( $"D:\\Autotests\\ChromeTest\\screens\\WebDriverPoC\\{launchName}\\chrome\\{width}x{height}.png",
                    ScreenshotImageFormat.Png );

                ITakesScreenshot firefoxScreenshotDriver = firefoxDriver as ITakesScreenshot;
                screenshot = firefoxScreenshotDriver.GetScreenshot();
                screenshot.SaveAsFile( $"D:\\Autotests\\ChromeTest\\screens\\WebDriverPoC\\{launchName}\\firefox\\{width}x{height}.png",
                    ScreenshotImageFormat.Png );

                ITakesScreenshot edgeScreenshotDriver = edgeDriver as ITakesScreenshot;
                screenshot = chromeScreenshotDriver.GetScreenshot();
                screenshot.SaveAsFile( $"D:\\Autotests\\ChromeTest\\screens\\WebDriverPoC\\{launchName}\\edge\\{width}x{height}.png",
                    ScreenshotImageFormat.Png );
            }*/

            return result;
        }
    }
}
