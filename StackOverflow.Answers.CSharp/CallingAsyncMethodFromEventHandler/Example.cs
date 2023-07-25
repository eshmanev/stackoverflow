using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflow.Answers.CSharp.CallingAsyncMethodFromEventHandler
{
    /// <summary>
    /// <see href="https://stackoverflow.com/questions/75982605/how-to-call-async-method-from-an-event#75982605"/>
    /// </summary>
    internal class Example
    {
        public event EventHandler<EventArgs> Banned;

        public Example()
        {
            Banned += async (s, e) => await btnCheck_Click(s, e);
        }

        async Task<bool> IsIPBannedAsync(string ip)
        {
            try
            {
                bool isBanned = false;

                await foreach (var line in File.ReadLinesAsync("file.txt"))
                {
                    if (line.Contains(ip.Trim())) { isBanned = true; break; }
                }
                return isBanned;
            }
            catch { return false; }
        }

        private async Task btnCheck_Click(object sender, EventArgs e)
        {
            bool banned = await IsIPBannedAsync("192.168.1.1");
        }
    }
}
