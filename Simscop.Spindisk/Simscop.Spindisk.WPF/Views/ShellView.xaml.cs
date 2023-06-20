﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Simscop.Spindisk.Core.ViewModels;
using CameraViewModel = Simscop.Spindisk.Core.ViewModels.CameraViewModel;

namespace Simscop.Spindisk.WPF.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        private CameraViewModel cameraVM;
        private SpinViewModel spinVM;
        private ShellViewModel shellVM;
        private SteerViewModel steerVM;
        private LaserViewModel laserVM;

        private int frameCount = 0;
        private DateTime lastTime = DateTime.Now;

        public ShellView()
        {
            InitializeComponent();


            Pic1.MouseLeftButtonDown += PicDown;
            Pic2.MouseLeftButtonDown += PicDown;
            Pic3.MouseLeftButtonDown += PicDown;
            Pic4.MouseLeftButtonDown += PicDown;



            cameraVM = new CameraViewModel();
            shellVM = new ShellViewModel();
            spinVM = new SpinViewModel();
            steerVM = new SteerViewModel();
            laserVM = new LaserViewModel();

            SetDataContext();

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // 计算帧率
            frameCount++;
            var now = DateTime.Now;
            var elapsed = now - lastTime;
            if (elapsed >= TimeSpan.FromSeconds(1))
            {
                var fps = frameCount / elapsed.TotalSeconds;
                // 更新界面显示
                FpsLabel.Text = $"FPS: {fps:F0}";
                // 重置计数器
                frameCount = 0;
                lastTime = now;
            }
        }

        private void SetDataContext()
        {
            this.DataContext = shellVM;
            this.BaseCameraControl.DataContext = cameraVM;
            this.SpinControl.DataContext = spinVM;
            this.SteerControl.DataContext = steerVM;
            this.LaserControl.DataContext = laserVM;
        }

        private bool IsFull { get; set; } = false;

        private void PicDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var grid = sender as Border;

                if (!IsFull)
                {
                    foreach (Border child in Client.Children)
                        if (child != grid) child.Visibility = Visibility.Collapsed;

                    Client.Columns = 1;
                    Client.Rows = 1;
                    IsFull = true;
                }
                else
                {
                    foreach (Border child in Client.Children)
                        if (child != grid) child.Visibility = Visibility.Visible;

                    Client.Columns = 2;
                    Client.Rows = 2;
                    IsFull = false;
                }
            }



        }

        // TODO 这里的卡顿问题已经定位了，原因就是在给datacontext的时候数据变化和赋值原因，解决办法挺简单的，单个窗口重复利用就行，但是这里目前就卡着吧，有空再改
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CameraView view = new();
            view.Show();
            view.DataContext = cameraVM;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void AboutBtClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("请联系");
        }

        private void HelpBtClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("请联系");
        }

        private void OpenBtClick(object sender, RoutedEventArgs e)
        {
            var vm = new ScanViewModel();
            var view = new ScanView
            {
                DataContext = vm
            };

            view.Show();
        }
    }
}
