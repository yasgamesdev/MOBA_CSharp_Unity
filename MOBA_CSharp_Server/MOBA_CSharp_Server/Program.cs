using MOBA_CSharp_Server.Game;
using System.Diagnostics;
using System.Threading;

namespace MOBA_CSharp_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEntity root = new GameEntity();

            int frameRate = root.GetChild<ConfigEntity>().GetInt("FrameRate");
            int frameMilliseconds = 1000 / frameRate;

            Stopwatch stopwatch = new Stopwatch();
            int overTime = 0;
            while (true)
            {
                stopwatch.Restart();

                root.Step((frameMilliseconds + overTime) * 0.001f);

                stopwatch.Stop();
                int stepTime = (int)stopwatch.ElapsedMilliseconds;

                if (stepTime <= frameMilliseconds)
                {
                    Thread.Sleep(frameMilliseconds - stepTime);
                }
                else
                {
                    overTime = stepTime - frameMilliseconds;
                }
            }
        }
    }
}
