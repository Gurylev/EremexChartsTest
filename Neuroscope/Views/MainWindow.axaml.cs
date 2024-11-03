using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Neuroscope.ViewModels;
using ReactiveUI;
using ScottPlot;
using ScottPlot.Colormaps;
using ScottPlot.Plottables;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Neuroscope.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
       
        public MainWindow()
        {
            InitializeComponent();

            
        }
       
      
    }
}