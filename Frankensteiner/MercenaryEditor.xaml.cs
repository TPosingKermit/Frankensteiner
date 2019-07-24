﻿using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frankensteiner
{
    /// <summary>
    /// Interaction logic for MercenaryEditor.xaml
    /// </summary>
    public partial class MercenaryEditor : MetroWindow
    {
        private ObservableCollection<FaceValueSliderItem> _sliders = new ObservableCollection<FaceValueSliderItem>();
        private Mercenary mercenary;

        public MercenaryEditor(Mercenary selectedMerc)
        {
            InitializeComponent();
            mercenary = selectedMerc;

            Owner = Application.Current.MainWindow; // Set MainWindow as owner, so it'll make proper use of WindowStartupLocation.Owner
            #region Update colours to fit the active theme + alternating colors for readability
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);
            if(appStyle.Item1.Name == "BaseDark")
            {
                gBackground.Background = new SolidColorBrush(Color.FromRgb(69, 69, 69));
                dgValueList.AlternatingRowBackground = new SolidColorBrush(Color.FromRgb(69, 69, 69));
            } else {
                gBackground.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                dgValueList.AlternatingRowBackground = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            }
            #endregion

            // Create 49 sliders, it consists of 3 separate sliders - one for each value
            dgValueList.ItemsSource = _sliders;
            for (int i=0; i < 49; i++)
            {
                var newSlider = new FaceValueSliderItem();
                newSlider.Translation = selectedMerc.FaceValues[i];
                newSlider.Rotation = selectedMerc.FaceValues[i+49];
                newSlider.Scale = selectedMerc.FaceValues[i+98];
                newSlider.UpdateDescription(String.Format("{0}: Unknown Value", (i + 1)));
                _sliders.Add(newSlider);
            }
            tbNewName.IsEnabled = !mercenary.isHordeMercenary;
        }

        private void BOpenRandomizer_Click(object sender, RoutedEventArgs e)
        {
            settingsFlyout.IsOpen = true;
        }

        private void BRandomize_Click(object sender, RoutedEventArgs e)
        {
            int minValue = Convert.ToUInt16(sliderRandomRange.LowerValue);
            int maxValue = Convert.ToUInt16(sliderRandomRange.UpperValue);
            Random rnd = new Random();
            for(int i=0; i < _sliders.Count(); i++)
            {
                uint newTrans = (uint)rnd.Next(minValue, maxValue);
                uint newRot = (uint)rnd.Next(minValue, maxValue);
                uint newScale = (uint)rnd.Next(minValue, maxValue);

                _sliders[i].Translation = newTrans;
                _sliders[i].Rotation = newRot;
                _sliders[i].Scale = newScale;
            }
        }

        private void BMinimizeAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _sliders.Count(); i++)
            {
                _sliders[i].Translation = 0;
                _sliders[i].Rotation = 0;
                _sliders[i].Scale = 0;
            }
        }

        private void BMaximizeAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _sliders.Count(); i++)
            {
                _sliders[i].Translation = 65535;
                _sliders[i].Rotation = 65535;
                _sliders[i].Scale = 65535;
            }
        }

        private void BFrankenstein_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            for (int i = 0; i < _sliders.Count(); i++)
            {
                _sliders[i].Translation = (uint)(rnd.Next(0, 2) == 1 ? 0 : 65535);
                _sliders[i].Rotation = (uint)(rnd.Next(0, 2) == 1 ? 0 : 65535);
                _sliders[i].Scale = (uint)(rnd.Next(0, 2) == 1 ? 0 : 65535);
            }
        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _sliders.Count(); i++)
            {
                var faceSlider = _sliders[i];
                mercenary.FaceValues[i] = faceSlider.Translation;
                mercenary.FaceValues[i+49] = faceSlider.Rotation;
                mercenary.FaceValues[i+98] = faceSlider.Scale;
            }

            mercenary.Name = (!String.IsNullOrWhiteSpace(tbNewName.Text)) ? tbNewName.Text : mercenary.Name;
            if(mercenary.Name == mercenary.OriginalName)
            {
                if (!mercenary.OriginalFaceValues.SequenceEqual(mercenary.FaceValues))
                {
                    mercenary.ItemText = (mercenary.importedMercenary) ? String.Format("[IMPORT] {0} - Unsaved Changes!", mercenary.OriginalName) : String.Format("{0} - Unsaved Changes!", mercenary.OriginalName);
                    mercenary.isOriginal = false;
                    this.DialogResult = true;
                } else {
                    mercenary.ItemText = mercenary.Name;
                    mercenary.isOriginal = true;
                    this.DialogResult = false;
                }
            } else {
                mercenary.ItemText = (mercenary.importedMercenary) ? String.Format("[IMPORT] {0} > {1} - Unsaved Changes!", mercenary.OriginalName, mercenary.Name) : String.Format("{0} > {1} - Unsaved Changes!", mercenary.OriginalName, mercenary.Name);
                mercenary.isOriginal = false;
                this.DialogResult = true;
            }
        }

        private void TbNewName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!String.IsNullOrWhiteSpace(tbNewName.Text) && !bSave.IsEnabled)
            {
                //bSave.IsEnabled = true;
            }
        }
    }
}