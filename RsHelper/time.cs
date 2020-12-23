using System;
using System.Collections.Generic;
using System.Threading;

namespace RsHelper
{
    public class time
    {
        public int hours;
        public int minutes;
        public int seconds;
        public string message;
        public Action<string> onFinish;
        public static Action<string> onEveryFinish;
        static Object timeSync = new object();
        static List<time> times = new List<time>();
        static Thread timer = new Thread(tick);

        public time(int hours, int minutes, int seconds, string message) : this()
        {
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.message = message;
        }

        public time() {
            lock(timeSync)
            {
                if (timer.ThreadState == ThreadState.Unstarted)
                    timer.Start();
            }
        }

        public static bool addTime(time t)
        {
            lock(timeSync)
            {
                if (checkDupe(t))
                    return false;
                times.Add(t);
                return true;
            }
        }

        public static bool removeTime(string message)
        {
            lock(timeSync)
            {
                for(int i=0; i<times.Count; i++)
                {
                    if (times[i].message.Equals(message))
                    {
                        times.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }

        }

        public static bool checkDupe(time t)
        {
            lock(timeSync)
            {
                foreach (time a in times)
                {
                    if (a.message == t.message)
                        return true;
                }
                return false;
            }
        }

        public static void tick()
        {
            while (true)
            {
                lock (timeSync)
                {
                    for (int i = 0; i < times.Count; i++)
                    {
                        if (times[i].seconds == 0)
                        {
                            if (times[i].minutes == 0)
                            {
                                if (times[i].hours == 0)
                                {
                                    if(onEveryFinish != null)
                                        onEveryFinish.Invoke(times[i].message);
                                    if(times[i].onFinish!=null)
                                        times[i].onFinish.Invoke(times[i].message);
                                    times.RemoveAt(i);
                                    i--;
                                }
                                else if (times[i].hours > 0)
                                {
                                    times[i].hours--;
                                    times[i].minutes = 59;
                                    times[i].seconds = 59;
                                }
                            }
                            else if (times[i].minutes > 0)
                            {
                                times[i].minutes--;
                                times[i].seconds = 59;
                            }
                        }
                        else if (times[i].seconds > 0)
                        {
                            times[i].seconds--;
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public static time timeFromString(string s)
        {
            string[] time = s.Split(':');

            time timeLeft = new time(0, 0, 0, "");

            if (time.Length == 3)
            {
                timeLeft.hours = Int32.Parse(time[0]);
                timeLeft.minutes = Int32.Parse(time[1]);
                timeLeft.seconds = Int32.Parse(time[2]);
            }
            if (time.Length == 2)
            {
                timeLeft.minutes = Int32.Parse(time[0]);
                timeLeft.seconds = Int32.Parse(time[1]);
            }
            if (time.Length == 1)
            {
                timeLeft.seconds = Int32.Parse(time[0]);
            }

            return timeLeft;
        }

        public static time addToCurrentTime(time a)
        {
            DateTime endTime = DateTime.Now.AddHours(a.hours).AddMinutes(a.minutes).AddSeconds(a.seconds);
            return new time(endTime.Hour, endTime.Minute, endTime.Second, a.message);
        }
    }
}
