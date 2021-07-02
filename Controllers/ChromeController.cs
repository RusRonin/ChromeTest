using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChromeTest.Controllers
{
    [ApiController]
    [Route( "[controller]" )]
    public class ChromeController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cws = new ClientWebSocket();
            CancellationToken token = new CancellationToken();
            await cws.ConnectAsync( new Uri( "ws://localhost:9222/devtools/page/4F2D972E7B6D1A4E3EA19326DA1F3BE5" ), token );
            string ne = @"{""id:"": 1, ""method"": ""'Network.enable'""}";
            byte[] b = Encoding.ASCII.GetBytes( ne );
            byte[] b2 = new byte[ 10000 ];
            ArraySegment<byte> a = new ArraySegment<byte>( b );
            await cws.SendAsync( a, WebSocketMessageType.Text, true, token );
            await cws.ReceiveAsync( b2, token );
            string str = System.Text.Encoding.Default.GetString( b2 );
            return Ok( str );
        }
    }
}
