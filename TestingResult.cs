using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChromeTest
{
    public class TestingResult
    {
        public string LaunchName { get; set; }
        public string Url { get; set; }
        public string Html { get; set; }
        public List<string> Links { get; set; } = new List<string>();
        public List<string> Logs { get; set; } = new List<string>();
    }
}
