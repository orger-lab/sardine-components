using Sardine.Core;
using Sardine.Core.Views.WPF;
using Sardine.Devices.Hamamatsu.Camera.API;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sardine.Devices.Hamamatsu.Camera.Views.WPF
{
    /// <summary>
    /// Interaction logic for CameraOptionsUI.xaml
    /// </summary>
    public partial class CameraOptionsUI : VesselUserControl<HamamatsuCamera>
    {
        public CameraOptionsUI()
        {
            InitializeComponent();
            ComboBox_Binning.ItemsSource = Enum.GetValues(typeof(SubarraySize)).Cast<SubarraySize>();

            CaptureTriggerBuilder = new CaptureTriggerBuilder();
            OutputTriggerBuilder = new OutputTriggerBuilder();
            DockPanel_CaptureTrigger.DataContext = CaptureTriggerBuilder;
            DockPanel_OutputTriggerBuilder.DataContext = OutputTriggerBuilder;
            ComboBox_Polarity.ItemsSource = Enum.GetValues(typeof(OutputTriggerPolarity)).Cast<OutputTriggerPolarity>();
            ComboBox_Source.ItemsSource = Enum.GetValues(typeof(OutputTriggerSource)).Cast<OutputTriggerSource>();
            ComboBox_TriggerKind.ItemsSource = Enum.GetValues(typeof(OutputTriggerKind)).Cast<OutputTriggerKind>();
            ComboBox_ActiveRegion.ItemsSource = Enum.GetValues(typeof(OutputTriggerActiveRegion)).Cast<OutputTriggerActiveRegion>();

            ComboBox_TriggerActive.ItemsSource = Enum.GetValues(typeof(TriggerActive)).Cast<TriggerActive>();
            ComboBox_TriggerSource.ItemsSource = Enum.GetValues(typeof(TriggerSource)).Cast<TriggerSource>();
            ComboBox_TriggerPolarity.ItemsSource = Enum.GetValues(typeof(TriggerPolarity)).Cast<TriggerPolarity>();
            ComboBox_TriggerMode.ItemsSource = Enum.GetValues(typeof(TriggerMode)).Cast<TriggerMode>();
            ComboBox_TriggerGlobalExposure.ItemsSource = Enum.GetValues(typeof(TriggerGlobalExposure)).Cast<TriggerGlobalExposure>();

            ComboBox_CaptureSetting.ItemsSource = Enum.GetValues(typeof(CaptureSetting)).Cast<CaptureSetting>();
            ComboBox_CaptureSpeed.ItemsSource = Enum.GetValues(typeof(ReadoutSpeed)).Cast<ReadoutSpeed>();
        }


        public CaptureTriggerBuilder CaptureTriggerBuilder { get; }
        public OutputTriggerBuilder OutputTriggerBuilder { get; }

        public override void OnVesselReloadedAction()
        {
            Dispatcher.BeginInvoke(() =>
            {
                SetOutputTriggerSettings();
                UpdateCaptureTriggerBuilderSettings();
                if (IsOnline)
                {
                    ComboBox_OutputTrigger.SelectedIndex = 0;
                }
            });
        }

        private void SetOutputTriggerSettings() => ComboBox_OutputTrigger.ItemsSource = Handle?.OutputTriggers.Select((x, i) => new OutputTriggerWithID(x, i + 1));
        private void Button_UpdateRegion_Click(object sender, RoutedEventArgs e)
        {
            Handle?.UpdateCaptureRegion();
        }

        private void ComboBox_Binning_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_Binning.SelectedItem = Handle?.Binning;
        }

        private void Button_UpdateTiming_Click(object sender, RoutedEventArgs e)
        {
            Handle?.UpdateExposure();
        }


        private void ComboBox_OutputTrigger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OutputTriggerWithID trigger = ((ComboBox)sender).SelectedItem as OutputTriggerWithID;
            if (trigger is not null)
            {
                OutputTriggerBuilder.Source = trigger.Trigger.Source;
                OutputTriggerBuilder.ActiveRegion = trigger.Trigger.ActiveRegion;
                OutputTriggerBuilder.Polarity = trigger.Trigger.Polarity;
                OutputTriggerBuilder.Kind = trigger.Trigger.Kind;
                OutputTriggerBuilder.Delay = trigger.Trigger.Delay;
                OutputTriggerBuilder.Period = trigger.Trigger.Period;
            }
        }


        private void Button_UpdateOutputTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (Handle is not null)
            {
                int id = ComboBox_OutputTrigger.SelectedIndex;
                Handle.OutputTriggers[ComboBox_OutputTrigger.SelectedIndex] = OutputTriggerBuilder.Build();
                
                Dispatcher.Invoke(() => SetOutputTriggerSettings());
                ComboBox_OutputTrigger.SelectedIndex = id;
            }
        }


        private void Button_UpdateCaptureTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (Handle is not null)
            {
                Handle.CaptureTrigger = CaptureTriggerBuilder.Build();
                UpdateCaptureTriggerBuilderSettings();
            }
        }

        private void UpdateCaptureTriggerBuilderSettings()
        {
            if (Handle is not null)
            {
                CaptureTriggerBuilder.TriggerSource = Handle.CaptureTrigger.TriggerSource;
                CaptureTriggerBuilder.TriggerMode = Handle.CaptureTrigger.TriggerMode;
                CaptureTriggerBuilder.TriggerTimes = Handle.CaptureTrigger.TriggerTimes;
                CaptureTriggerBuilder.TriggerActive = Handle.CaptureTrigger.TriggerActive;
                CaptureTriggerBuilder.TriggerPolarity = Handle.CaptureTrigger.TriggerPolarity;
                CaptureTriggerBuilder.TriggerGlobalExposure = Handle.CaptureTrigger.TriggerGlobalExposure;
                CaptureTriggerBuilder.TriggerDelay = Handle.CaptureTrigger.TriggerDelay;
            }
        }

        private void Button_StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (IsOnline)
            {
                if(Handle!.IsCapturing)
                {
                    Handle!.StopCapture();
                }
                else
                {
                    Handle!.StartCapture();
                }
            }
        }

        private void Button_Fire_Click(object sender, RoutedEventArgs e)
        {
            Handle?.CaptureTrigger.Fire();
        }

        private void Button_ValidatePrepare_Click(object sender, RoutedEventArgs e)
        {

            if (IsOnline)
            {
                switch (Handle!.Status)
                {
                    case CaptureStatus.Ready:
                        Handle!.ReleaseResources();
                        return;
                    default:
                        Handle!.CaptureTrigger = CaptureTriggerBuilder.Build();
                        UpdateCaptureTriggerBuilderSettings();

                        var id = ComboBox_OutputTrigger.SelectedIndex;
                        Handle!.OutputTriggers[ComboBox_OutputTrigger.SelectedIndex] = OutputTriggerBuilder.Build();
                        ComboBox_OutputTrigger.SelectedIndex = id;

                        Handle!.UpdateExposure();

                        Handle!.UpdateCaptureRegion();

                        Handle!.PrepareCapture();
                        return;
                }
            }
        }
    }
}
