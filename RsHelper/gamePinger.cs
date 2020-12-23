using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Collections.Generic;

namespace RsHelper
{
    public class gamePinger
    {

        IPAddress address;
        public Action<string, int[]> onPingResult;
        public Action finishedPinging;
        Object pingLock = new object();
        List<Thread> pings = new List<Thread>();

        public static gamePinger findOnPort(int port)
        {
            TcpConnectionInformation[] tcpConnInfoArray = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
            IPAddress rsServer = null;

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.RemoteEndPoint.Port == port)
                {
                    rsServer = tcpi.RemoteEndPoint.Address;
                    break;
                }
            }

            return (rsServer == null) ? null : new gamePinger(rsServer);
        }

        public gamePinger(string ip)
        {
            address = IPAddress.Parse(ip);
        }

        public gamePinger()
        {

        }

        private gamePinger(IPAddress address)
        {
            this.address = address;
        }

        public string getAddress()
        {
            return address.ToString();
        }

        public bool isPinging()
        {
            lock(pingLock)
            {
                return pings.Count != 0;
            }
        }

        public void waitOnPings()
        {
            while(true)
            {
                bool wait = false;
                lock (pingLock)
                {
                    wait = pings.Count != 0;
                }
                if(wait)
                {
                    Thread.Sleep(1000);
                } else
                {
                    break;
                }
            }
        }

        public void asyncPing(int times, int delay, string ip = null)
        {
            string ipToUse = (ip != null) ? ip : address.ToString();
            Thread t = new Thread(() =>
            {
                int[] latency = new int[times];
                Ping p = new Ping();
                for (int i = 0; i < times; i++)
                {
                    int ms = (int)p.Send(ipToUse).RoundtripTime;
                    ms = (ms == 0) ? 2000 : ms;
                    latency[i] = ms;
                    Thread.Sleep(delay);
                }
                onPingResult.Invoke(ipToUse, latency);
                lock (pingLock)
                {
                    pings.Remove(Thread.CurrentThread);
                    if (pings.Count == 0)
                        finishedPinging.Invoke();
                }
            });
            lock(pingLock)
            {
                pings.Add(t);
            }
            t.Start();
        }

        public void pingGroup(List<string> ips, int times, int delay, int stagger)
        {
            new Thread(() =>
            {
                foreach (string ip in ips)
                {
                    asyncPing(times, delay, ip);
                    Thread.Sleep(stagger);
                }
            }).Start();
        }

        public static int averageLatency(int[] latencyResult)
        {
            double total = 0;
            for(int i=0; i<latencyResult.Length; i++)
            {
                total += latencyResult[i];
            }
            return (int)Math.Round(total / latencyResult.Length);
        }

        public static double standardDeviation(int[] latencyResult)
        {
            int average = averageLatency(latencyResult);
            double result = 0;
            for(int i=0; i<latencyResult.Length; i++)
            {
                result += (int)Math.Pow((latencyResult[i] - average), 2);
            }
            result = (double)decimal.Divide((decimal)result, latencyResult.Length - 1);
            result = Math.Sqrt(result);
            return Math.Round(result, 2);
        }

    }
}
