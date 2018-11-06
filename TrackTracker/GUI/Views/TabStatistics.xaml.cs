﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;

namespace TrackTracker.GUI.Views
{
    /// <summary>
    /// Interaction logic for TabStatistics.xaml
    /// </summary>
    public partial class TabStatistics : UserControl
    {
        public TabStatistics()
        {
            InitializeComponent();
        }

        private void statistics_GotFocus(object sender, RoutedEventArgs e)
        {
            Statistics statistics = new Statistics();
            statistics.GenerateStatistics(GlobalData.TracklistData.Tracks.ToList<Track>(), true, true, true, true, true, true);

            //--------------------------------------------------PIECHART--------------------------------------
            this.DataContext = new ObservableCollection<StatisticalData>(statistics.GetCountsByArtist());
            //--------------------------------------------------PIECHART--------------------------------------

            if (statistics.TotalCount > 0) labelTotalCount.Content = statistics.TotalCount.ToString();
            else labelTotalCount.Content = "0";
            if (statistics.ProperlyTagged > 0) labelProperlyTagged.Content = statistics.ProperlyTagged.ToString();
            else labelProperlyTagged.Content = "0";

            if (statistics.GetMostFrequentArtist(out string artistName, out uint artistCount))
            {
                labelArtistName.Content = artistName;
                labelArtistCount.Content = artistCount.ToString();
            }
            else
            {
                labelArtistName.Content = "-";
                labelArtistCount.Content = "0";
            }

            if (statistics.GetMostFrequentAlbum(out string albumName, out uint albumCount))
            {
                labelAlbumName.Content = albumName;
                labelAlbumCount.Content = albumCount.ToString();
            }
            else
            {
                labelAlbumName.Content = "-";
                labelAlbumCount.Content = "0";
            }

            if (statistics.GetMostFrequentGenre(out string genreName, out uint genreCount))
            {
                labelGenreName.Content = genreName;
                labelGenreCount.Content = genreCount.ToString();
            }
            else
            {
                labelGenreName.Content = "-";
                labelGenreCount.Content = "0";
            }

            if (statistics.GetMostFrequentDecade(out MusicEra decadeName, out uint decadeCount))
            {
                labelDecadeName.Content = decadeName.ToString(); //TODO: convert properly
                labelDecadeCount.Content = decadeCount.ToString();
            }
            else
            {
                labelDecadeName.Content = "-";
                labelDecadeCount.Content = "0";
            }

            if (labelTotalCount.Content.ToString() != "0")
            {
                string recommendedBand = "";
                if (labelGenreName.Content.ToString() == "Rock") recommendedBand = "Hollywood Undead";
                else if (labelGenreName.Content.ToString() == "Punk") recommendedBand = "Good Charlotte";

                textBlockTip.Text = "Hmm... it seems you rather like " + labelGenreName.Content + "! You might want to check out " + recommendedBand + " for a change!";
            }
            else textBlockTip.Text = "";
        }
    }
}