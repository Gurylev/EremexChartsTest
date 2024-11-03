using Avalonia.Threading;
using DynamicData;
using HidSharp;
using MathNet.Filtering.FIR;
using MathNet.Filtering;
using ReactiveUI.Fody.Helpers;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Neuroscope.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public CartesianChartRealtimePageViewModel CartesianVM { get; set; }
        
        [Reactive]
        public Dictionary<int, List<double>> ADcps { get; set; }


        public MainWindowViewModel()
        {
            ADcps = new Dictionary<int, List<double>>()
            {
                {1, [] },
                {2, []},
                {3, []},
                {4, []},
                {5, []},
                {6, []},
                {7, []},
                {8, []},
                {9, []},
                {10, []},
                {11, []},
                {12, []},
                {13, []},
                {14, []},
                {15, []},
                {16, []},
                {17, []},
                {18, []},
                {19, []},
                {20, []},
                {21, []},
                {22, []},
                {23, []},
                {24, []},
                {25, []}
            };

            CartesianVM = new CartesianChartRealtimePageViewModel(ADcps);
            CartesianVM.Start();


           
        }

     
    }
}
