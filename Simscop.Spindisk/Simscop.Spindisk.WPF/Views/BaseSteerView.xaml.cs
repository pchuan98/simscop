using Simscop.Spindisk.Core.ViewModels;
using Simscop.UI.Controls;
using System;
using System.Collections.Generic;
using System.Printing.IndexedProperties;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Simscop.Spindisk.WPF.Views
{
    /// <summary>
    /// Interaction logic for BaseSteerView.xaml
    /// </summary>
    public partial class BaseSteerView : UserControl
    {
        public const int TimerInterval = 100;
        private readonly DispatcherTimer _rightTimer;

        public SteerViewModel? Vm => DataContext as SteerViewModel;

        public BaseSteerView()
        {
            InitializeComponent();
            this.DataContext = null;
            this.DataContextChanged += BaseSteerView_DataContextChanged;

            _rightTimer = new DispatcherTimer(priority: DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(TimerInterval)
            };

            _rightTimer.Tick += (s, e) => Vm?.MoveX(Vm.XyStep);

            this.RightMoveBt.PreviewMouseDown += (s, e) => _rightTimer.Stop(); ;
            this.RightMoveBt.PreviewMouseUp += (s, e) => _rightTimer.Start(); 


        }

        private void BaseSteerView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if (sender is not BaseSteerView view) return;
            //if (view.DataContext is not Core.ViewModels.SteerViewModel vm) return;


            //var btList = new List<IconButton>()
            //{
            //    RightMoveBt,RightBottomMoveBt,
            //    BottomMoveBt,LeftBottomMoveBt,
            //    LeftMoveBt,LeftTopMoveBt,
            //    TopMoveBt,RightTopMoveBt,
            //    UpMoveBt,DownMoveBt
            //};

            //var actions = new List<Action<double>>
            //{
            //    MoveX,

            //    v =>
            //    {
            //        MoveX(v);
            //        MoveY(-v);
            //    },

            //    v=>MoveY(-v),

            //    v =>
            //    {
            //        MoveX(-v);
            //        MoveY(-v);
            //    },

            //    v => MoveX(-v),

            //    v =>
            //    {
            //        MoveX(-v);
            //        MoveY(v);
            //    },

            //    MoveY,

            //    v =>
            //    {
            //        MoveX(v);
            //        MoveY(v);
            //    },
            //};

            //var valList = new List<double>()
            //{
            //    vm.XyStep,vm.XyStep,vm.XyStep,vm.XyStep,vm.XyStep,vm.XyStep,vm.XyStep,vm.XyStep,vm.ZStep,vm.ZStep
            //};

            //for (var i = 0; i < 10; i++)
            //{
            //    var timer = new DispatcherTimer(priority: DispatcherPriority.Render)
            //    {
            //        Interval = TimeSpan.FromMilliseconds(TimerInterval),
            //    };

            //    var index = i;
            //    timer.Tick += (s, args) =>
            //        actions[index](valList[index]);

            //    btList[i].PreviewMouseLeftButtonDown += (s, args) =>
            //        timer.Start();

            //    btList[i].PreviewMouseLeftButtonUp += (s, args) =>
            //        timer.Stop();

            //}
        }

        private void MoveX(double value)
        {
            if (DataContext is not Core.ViewModels.SteerViewModel vm) return;

            vm.MoveX(value);
        }

        private void MoveY(double value)
        {
            if (DataContext is not Core.ViewModels.SteerViewModel vm) return;

            vm.MoveY(value);
        }

        private void MoveZ(double value)
        {
            if (DataContext is not Core.ViewModels.SteerViewModel vm) return;

            vm.MoveZ(value);
        }
    }
}
