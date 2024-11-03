using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Eremex.AvaloniaUI.Charts;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroscope.ViewModels
{
    public partial class SeriesViewModel : ObservableObject
    {
        [ObservableProperty] string axisXKey;
        [ObservableProperty] string axisYKey;
        [ObservableProperty] Color color;
        [ObservableProperty] ISeriesDataAdapter dataAdapter;
    }
    public partial class RealtimeDataGenerator : ObservableObject
    {
        readonly Random random = new(1);
        readonly object sync = new();
        readonly int interval;
        readonly int pointsCount;
        readonly List<(TimeSpan, double)>[] buffers;
        readonly SortedTimeSpanDataAdapter[] adapters;
        readonly double[] yAdditions;
        bool enabled;
        Thread generatingThread;

        [ObservableProperty]
        public Dictionary<int, List<double>> aDcps;

        int AdaptersCount => adapters.Length;
        public SortedTimeSpanDataAdapter[] Adapters => adapters;

        public RealtimeDataGenerator(int adaptersCount, int pointsCount, int interval)
        {
            this.interval = interval;
            this.pointsCount = pointsCount;
            adapters = new SortedTimeSpanDataAdapter[adaptersCount];
            buffers = new List<(TimeSpan, double)>[adaptersCount];
            yAdditions = new double[adaptersCount];
            for (int i = 0; i < AdaptersCount; i++)
            {
                adapters[i] = new SortedTimeSpanDataAdapter();
                buffers[i] = new List<(TimeSpan, double)>();
            }
        }
        (TimeSpan, double) CreatePoint(int index, TimeSpan timeStamp, double value)
        {
            double arg = timeStamp.TotalMilliseconds;
            yAdditions[index] += random.Next(10, 20) * Math.Sign(random.NextDouble() - 0.5);
            if (yAdditions[index] < -30)
                yAdditions[index] += 10;
            if (yAdditions[index] > 30)
                yAdditions[index] -= 10;
            double indication = value + 700000 * index ;
            return (timeStamp, indication);
        }
        void GeneratingLoop()
        {
            var timeStamp = TimeSpan.FromMilliseconds(pointsCount * interval);
            var addition = TimeSpan.FromMilliseconds(interval);
            while (enabled)
            {
                Thread.Sleep((int)addition.TotalMilliseconds);
                timeStamp += addition;
                for (int i = 0; i < AdaptersCount; i++)
                {
                    var point = CreatePoint(i, timeStamp, ADcps[i + 1].LastOrDefault());
                    lock (sync)
                    {
                        buffers[i].Add(point);
                    }
                }
            }
        }
        public void GenerateInitialData(Dictionary<int, List<double>> adcps)
        {
            ADcps = adcps;
            for (int i = 0; i < pointsCount - 1; i++)
            {
                var argument = TimeSpan.FromMilliseconds(i * interval);
                for (int j = 0; j < AdaptersCount; j++)
                    adapters[j].Add(CreatePoint(j, argument, ADcps[j+1].LastOrDefault()));
            }
        }
        public void Start()
        {
            generatingThread ??= new Thread(GeneratingLoop);
            enabled = true;
            generatingThread.Start();
        }
        public void Stop()
        {
            enabled = false;
            generatingThread?.Join();
            generatingThread = null;
        }
        public void UpdateAdapters()
        {
            lock (sync)
            {
                for (int i = 0; i < AdaptersCount; i++)
                {
                    adapters[i].AddRange(buffers[i]);
                    adapters[i].RemoveFromStart(buffers[i].Count);
                    buffers[i].Clear();
                }
            }
        }
    }


    public partial class CartesianChartRealtimePageViewModel : ViewModelBase
    {
        static readonly Color[] colors = new[]
        {
        Color.FromArgb(255, 204, 55, 28),
        Color.FromArgb(255, 255, 106, 0),
        Color.FromArgb(255, 0, 148, 255),
        Color.FromArgb(255, 119, 133, 255),
        Color.FromArgb(255, 0, 127, 128),
        Color.FromArgb(255, 91, 171, 171),
         Color.FromArgb(255, 204, 55, 28),
        Color.FromArgb(255, 255, 106, 0),
        Color.FromArgb(255, 0, 148, 255),
        Color.FromArgb(255, 119, 133, 255),
        Color.FromArgb(255, 0, 127, 128),
        Color.FromArgb(255, 91, 171, 171),
         Color.FromArgb(255, 204, 55, 28),
        Color.FromArgb(255, 255, 106, 0),
        Color.FromArgb(255, 0, 148, 255),
        Color.FromArgb(255, 119, 133, 255),
        Color.FromArgb(255, 0, 127, 128),
        Color.FromArgb(255, 91, 171, 171),
         Color.FromArgb(255, 204, 55, 28),
        Color.FromArgb(255, 255, 106, 0),
        Color.FromArgb(255, 0, 148, 255),
        Color.FromArgb(255, 119, 133, 255),
        Color.FromArgb(255, 0, 127, 128),
        Color.FromArgb(255, 91, 171, 171),
        Color.FromArgb(255, 91, 171, 171)
    };

        readonly DispatcherTimer timer = new(DispatcherPriority.Background);
        readonly RealtimeDataGenerator generator = new(25, 500, 10);

        [ObservableProperty] ObservableCollection<SeriesViewModel> series = new();
        [ObservableProperty]
        public Dictionary<int, List<double>> aDcps;

        public CartesianChartRealtimePageViewModel(Dictionary<int, List<double>> adcps)
        {
            ADcps = adcps;

            generator.GenerateInitialData(ADcps);
            timer.Tick += (_, _) => generator.UpdateAdapters();
            timer.Interval = TimeSpan.FromMilliseconds(2);

            for (int i = 0; i < generator.Adapters.Length; i++)
                series.Add(new SeriesViewModel { Color = colors[i], DataAdapter = generator.Adapters[i] });
        }
        public void Start()
        {
            generator.Start();
            timer.Start();
        }
        public void Stop()
        {
            generator.Stop();
            timer.Stop();
        }
    }
}
