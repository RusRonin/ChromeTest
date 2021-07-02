using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;

namespace ChromeTest.Controllers
{
    internal class ChromeRemoteWebDriver : RemoteWebDriver, ISupportsLogs
    {
        public ChromeRemoteWebDriver( Uri uri, ChromeOptions options )
            : base( uri, options )
        {
        }
    }

    [Route( "api/[controller]" )]
    [ApiController]
    public class SelenoidPoCController : ControllerBase
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

            //var capabilities = new DesiredCapabilities();
            //capabilities.SetCapability( CapabilityType.BrowserName, "UNKNOWN" );
            //capabilities.SetCapability( CapabilityType.BrowserVersion, "" );
            ChromeOptions options = new ChromeOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal
            };
            options.SetLoggingPreference( LogType.Browser, LogLevel.All );
            /*options.SetLoggingPreference( LogType.Driver, LogLevel.All );*/

            IWebDriver chromeDriver = new ChromeRemoteWebDriver( new Uri( "http://localhost:4444/wd/hub" ), options );
            //IWebDriver chromeDriver = new OpenQA.Selenium.Chrome.ChromeDriver( options );
            chromeDriver.Navigate().GoToUrl( url );

            result.Html = chromeDriver.PageSource;

            var links = chromeDriver.FindElements( By.TagName( "a" ) );
            foreach ( var link in links )
            {
                result.Links.Add( link.GetAttribute( "href" ) );
            }

            var logs = chromeDriver.Manage().Logs.GetLog( LogType.Browser );
            foreach ( var log in logs )
            {
                result.Logs.Add( log.Message );
            }
            /*logs = chromeDriver.Manage().Logs.GetLog( LogType.Driver );
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

            FirefoxOptions firefoxOptions = new FirefoxOptions();
            IWebDriver firefoxDriver = new RemoteWebDriver( new Uri( "http://localhost:4444/wd/hub" ), firefoxOptions );
            firefoxDriver.Navigate().GoToUrl( url );

            EdgeOptions edgeOptions = new EdgeOptions();
            IWebDriver edgeDriver = new RemoteWebDriver( new Uri( "http://localhost:4444/wd/hub" ), edgeOptions );
            edgeDriver.Navigate().GoToUrl( url );

            System.IO.Directory.CreateDirectory( $"D:\\Autotests\\ChromeTest\\screens\\SelenoidPoC\\{launchName}\\chrome" );
            System.IO.Directory.CreateDirectory( $"D:\\Autotests\\ChromeTest\\screens\\SelenoidPoC\\{launchName}\\firefox" );
            System.IO.Directory.CreateDirectory( $"D:\\Autotests\\ChromeTest\\screens\\SelenoidPoC\\{launchName}\\edge" );

            foreach ( var screenSize in screenSizes )
            {
                int width = screenSize.Key;
                int height = screenSize.Value;

                chromeDriver.Manage().Window.Size = new System.Drawing.Size( width, height );
                firefoxDriver.Manage().Window.Size = new System.Drawing.Size( width, height );
                edgeDriver.Manage().Window.Size = new System.Drawing.Size( width, height );

                ITakesScreenshot chromeScreenshotDriver = chromeDriver as ITakesScreenshot;
                Screenshot screenshot = chromeScreenshotDriver.GetScreenshot();
                screenshot.SaveAsFile( $"D:\\Autotests\\ChromeTest\\screens\\SelenoidPoC\\{launchName}\\chrome\\{width}x{height}.png",
                    ScreenshotImageFormat.Png );

                ITakesScreenshot firefoxScreenshotDriver = firefoxDriver as ITakesScreenshot;
                screenshot = firefoxScreenshotDriver.GetScreenshot();
                screenshot.SaveAsFile( $"D:\\Autotests\\ChromeTest\\screens\\SelenoidPoC\\{launchName}\\firefox\\{width}x{height}.png",
                    ScreenshotImageFormat.Png );

                ITakesScreenshot edgeScreenshotDriver = edgeDriver as ITakesScreenshot;
                screenshot = chromeScreenshotDriver.GetScreenshot();
                screenshot.SaveAsFile( $"D:\\Autotests\\ChromeTest\\screens\\SelenoidPoC\\{launchName}\\edge\\{width}x{height}.png",
                    ScreenshotImageFormat.Png );
            }

            return result;
        }
    }
}
