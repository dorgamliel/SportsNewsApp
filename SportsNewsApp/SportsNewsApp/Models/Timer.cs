
using System.Diagnostics;

namespace SportsNewsApp.Models
{
    class Timer
    {
        private Stopwatch stopwatch;
        public Timer()
        {
            stopwatch = new Stopwatch();
        }
        public void ElapseStart()
        {
            stopwatch.Start();
        }
        public void ElapseEnd(string funcName)
        {
            stopwatch.Stop();
            Debug.WriteLine(funcName + ", " + stopwatch.ElapsedMilliseconds + "ms.\n");
        }
        public string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            return sf.GetMethod().Name;
        }
    }
}
