using System;
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
using Simscop.Fake.Spindisk;
using Simscop.Spindisk.Core.ViewModels;
using CameraViewModel = Simscop.Spindisk.Core.ViewModels.CameraViewModel;

namespace Simscop.Spindisk.WPF.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        private Fake.Spindisk.CameraViewModel cameraVM;
        private SpinViewModel spinVM;
        private Fake.Spindisk.SpinViewModel shellVM;


        public ShellView()
        {
            InitializeComponent();


            Pic1.MouseLeftButtonDown += PicDown;
            Pic2.MouseLeftButtonDown += PicDown;
            Pic3.MouseLeftButtonDown += PicDown;
            Pic4.MouseLeftButtonDown += PicDown;

            

            cameraVM = new ();
            shellVM = new ();
            spinVM = new ();

            SetDataContext();
        }

        private void SetDataContext()
        {
            this.DataContext = shellVM;
            this.BaseCameraControl.DataContext = cameraVM;
            this.SpinControl.DataContext = spinVM;
        }

        private bool IsFull { get; set; } = false;

        private void PicDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount==2)
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new CameraView
            {
                DataContext = cameraVM
            };

            window.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
