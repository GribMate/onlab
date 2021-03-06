﻿using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using WinForms = System.Windows.Forms;

using TrackTracker.Services.Interfaces;
using TrackTracker.BLL;
using TrackTracker.BLL.Enums;
using TrackTracker.BLL.Model;
using TrackTracker.BLL.GlobalContexts;
using TrackTracker.GUI.ViewModels.Base;



namespace TrackTracker.GUI.ViewModels
{
    public class DataViewModel : ViewModelBase
    {
        private IEnvironmentService environmentService;
        private IFileService fileService;
        private ISpotifyService spotifyService;
        private IDatabaseService databaseService;

        private Dictionary<string, string> spotifyPlaylists;

        public DataViewModel() : base()
        {
            environmentService = DependencyInjector.GetService<IEnvironmentService>();
            fileService = DependencyInjector.GetService<IFileService>();
            spotifyService = DependencyInjector.GetService<ISpotifyService>();
            databaseService = DependencyInjector.GetService<IDatabaseService>();

            spotifyPlaylists = new Dictionary<string, string>();

            ExternalDriveNames = new ObservableCollection<string>(environmentService.GetExternalDriveNames());
            SupportedFileFormats = GetSupportedFileFormats();
            SpotifyPlaylists = new ObservableCollection<string>();

            BrowseCommand = new RelayCommand<object>(exe => ExecuteBrowse(), can => CanExecuteBrowse);
            AddFilesCommand = new RelayCommand<object>(exe => ExecuteAddFiles(), can => CanExecuteAddFiles);
            LinkSpotifyCommand = new RelayCommand<object>(exe => ExecuteLinkSpotify(), can => CanExecuteLinkSpotify);
            AddSpotifyListCommand = new RelayCommand<object>(exe => ExecuteAddSpotifyList(), can => CanExecuteAddSpotifyList);

            OfflineFolderPath = "Please select your offline music folder...";
            FolderIsChecked = true;
        }

        public ObservableCollection<string> ExternalDriveNames { get; set; }
        public ObservableCollection<string> SupportedFileFormats { get; set; }
        public ObservableCollection<string> SpotifyPlaylists { get; set; }

        public ICommand BrowseCommand { get; }
        public ICommand AddFilesCommand { get; }
        public ICommand LinkSpotifyCommand { get; }
        public ICommand AddSpotifyListCommand { get; }



        private bool folderIsChecked;
        public bool FolderIsChecked
        {
            get => folderIsChecked;
            set
            {
                SetProperty(ref folderIsChecked, value);

                if (value == true)
                {
                    IsEnabledOfflineFolderPath = true;
                    IsEnabledSelectFileExtension = false;
                    IsEnabledSelectDriveName = false;
                }
                else
                {
                    IsEnabledOfflineFolderPath = false;
                    IsEnabledSelectFileExtension = true;
                    IsEnabledSelectDriveName = true;
                }
            }
        }

        // Does not need backing field or set-logic, because it would just be the inverse of FolderIsSelected
        // GUI handles RadioButton GroupName, so the values are always reversed
        public bool DriveIsChecked {get; set; }

        private string selectedExternalDriveName;
        public string SelectedExternalDriveName
        {
            get => selectedExternalDriveName;
            set
            {
                SetProperty(ref selectedExternalDriveName, value);
                NotifyPropertyChanged(nameof(CanExecuteAddFiles));
            }
        }

        private SupportedFileExtension selectedSupportedFileExtension;
        public SupportedFileExtension SelectedSupportedFileExtension
        {
            get => selectedSupportedFileExtension;
            set
            {
                SetProperty(ref selectedSupportedFileExtension, value);
                NotifyPropertyChanged(nameof(CanExecuteAddFiles));
            }
        }

        private string offlineFolderPath;
        public string OfflineFolderPath
        {
            get => offlineFolderPath;
            set
            {
                SetProperty(ref offlineFolderPath, value);
                NotifyPropertyChanged(nameof(CanExecuteAddFiles));
            }
        }

        private string spotifyAccName;
        public string SpotifyAccName
        {
            get => spotifyAccName;
            set { SetProperty(ref spotifyAccName, value); }
        }

        private string spotifyListCount;
        public string SpotifyListCount
        {
            get => spotifyListCount;
            set { SetProperty(ref spotifyListCount, value); }
        }

        private string selectedSpotifyPlaylist;
        public string SelectedSpotifyPlaylist
        {
            get => selectedSpotifyPlaylist;
            set
            {
                SetProperty(ref selectedSpotifyPlaylist, value);
            }
        }



        private bool isEnabledOfflineFolderPath;
        public bool IsEnabledOfflineFolderPath
        {
            get => isEnabledOfflineFolderPath;
            set { SetProperty(ref isEnabledOfflineFolderPath, value); }
        }

        private bool isEnabledSelectFileExtension;
        public bool IsEnabledSelectFileExtension
        {
            get => isEnabledSelectFileExtension;
            set { SetProperty(ref isEnabledSelectFileExtension, value); }
        }

        private bool isEnabledSelectDriveName;
        public bool IsEnabledSelectDriveName
        {
            get => isEnabledSelectDriveName;
            set { SetProperty(ref isEnabledSelectDriveName, value); }
        }



        private bool CanExecuteBrowse
        {
            get => FolderIsChecked;
        }
        private void ExecuteBrowse()
        {            
            WinForms.FolderBrowserDialog fbdMedia = new WinForms.FolderBrowserDialog(); // TODO: add clear WPF folder selector solution instead of WinForms later...
            fbdMedia.ShowNewFolderButton = false; // Folder is supposed to exist already
            fbdMedia.Description = "Select local music library folder:";

            if (fbdMedia.ShowDialog() == WinForms.DialogResult.OK)
            {
                // Just in case, do some sanity check on selected folder to avoid unnecessary exceptions
                if (!String.IsNullOrWhiteSpace(fbdMedia.SelectedPath) && fbdMedia.SelectedPath.Length > 8)
                    OfflineFolderPath = fbdMedia.SelectedPath;
            }
        }

        public bool CanExecuteAddFiles
        {
            get
            {
                if (FolderIsChecked)
                    return fileService.MediaPathIsValid(OfflineFolderPath);
                else
                    return !String.IsNullOrWhiteSpace(SelectedExternalDriveName) && SelectedSupportedFileExtension != SupportedFileExtension.Unknown;
            }
        }
        public void ExecuteAddFiles()
        {
            LocalMediaPack lmp = null;

            if (FolderIsChecked)  // Parsing an offline folder, all supported file formats may appear
            {
                lmp = new LocalMediaPack(OfflineFolderPath, false);
                LoadFilesFromDirectory(lmp, OfflineFolderPath); // Loading up LMP object with file paths
                OfflineFolderPath = "Please select your offline music folder...";
            }
            else // Parsing an external drive, with a given file format
            {
                lmp = new LocalMediaPack(SelectedExternalDriveName, true, SelectedSupportedFileExtension);
                LoadFilesFromDrive(lmp, SelectedExternalDriveName, SelectedSupportedFileExtension); // Loading up LMP object with file paths
            }

            // TODO: rework whole concept of non-redundant LMPs (eg. different pendrives with same drive letter might be a problem...)
            if (DoesLMPAlreadyExist(lmp))
            {
                UtilityHelper.ShowExceptionDialog(
                    "Cannot add files",
                    "Cannot add the selected group of files, since the root drive / folder was already added",
                    null);
            }

            else
            {
                LMPContext.StoredLocalMediaPacks.Add(lmp);
                WriteLMPToDatabase(lmp); // Persisting new LMP object
            }
        }

        public bool CanExecuteLinkSpotify
        {
            get => SpotifyPlaylists.Count == 0; // If the list is empty, the user didn't link account yet
        }
        public async void ExecuteLinkSpotify()
        {
            await spotifyService.Login();

            Dictionary<string, object> data = await spotifyService.GetAccountInformation();
            SpotifyAccName = (string)data.Select(pair => pair).Where(pair => pair.Key.Equals("ProfileName")).First().Value;

            spotifyPlaylists = await spotifyService.GetAllPlaylists();
            SpotifyListCount = spotifyPlaylists.Count.ToString();
            foreach (KeyValuePair<string, string> playlist in spotifyPlaylists)
            {
                SpotifyPlaylists.Add(playlist.Value);
            }
        }

        public bool CanExecuteAddSpotifyList
        {
            get => !String.IsNullOrWhiteSpace(SelectedSpotifyPlaylist);
        }
        public async void ExecuteAddSpotifyList()
        {
            string playlistID = null;

            foreach (KeyValuePair<string, string> playlist in spotifyPlaylists)
            {
                if (playlist.Value == SelectedSpotifyPlaylist)
                {
                    playlistID = playlist.Key;
                    break;
                }
            }

            List<string> playlistTrackIDs = await spotifyService.GetPlaylistTrackIDs(playlistID);

            foreach (string trackID in playlistTrackIDs)
            {
                PlayzoneContext.AddMusicFileSpotify(trackID);
            }

            SpotifyPlaylists.Remove(SelectedSpotifyPlaylist);
        }



        private ObservableCollection<string> GetSupportedFileFormats()
        {
            List<string> formats = new List<string>();

            foreach (SupportedFileExtension ext in Enum.GetValues(typeof(SupportedFileExtension)).Cast<SupportedFileExtension>()) // Casting to get typed iteration, just in case
            {
                if (ext == SupportedFileExtension.Unknown)
                    continue; // Do not offer the selection of "Unknown" format

                formats.Add(ext.ToString());
            }

            return new ObservableCollection<string>(formats);
        }
        private void LoadFilesFromDrive(LocalMediaPack lmp, string driveLetter, SupportedFileExtension type) //loads all the files with the given extension from a given drive into an LMP object, using a service
        {
            List<string> paths = fileService.GetAllFilesFromDrive(driveLetter, type.ToString()); //no typed extensions when calling to DAL
            foreach (string path in paths)
            {
                lmp.AddFilePath(path, type); //loading up the LMP object
            }
        }
        private void LoadFilesFromDirectory(LocalMediaPack lmp, string path) //loads all the files with the given extension from a given directory into an LMP object, using a service
        {
            //when loading from a directory, we want all the supported file types to be read, so we iterate through the extensions
            foreach (SupportedFileExtension ext in Enum.GetValues(typeof(SupportedFileExtension)).Cast<SupportedFileExtension>()) //casting to get typed iteration, just in case
            {
                List<string> paths = fileService.GetAllFilesFromDirectory(path, ext.ToString()); //no typed extensions when calling to DAL
                foreach (string currPath in paths)
                {
                    lmp.AddFilePath(currPath, ext); //loading up the LMP object
                }
            }
        }
        private bool DoesLMPAlreadyExist(LocalMediaPack pack)
        {
            bool result = false;

            // Checking in stored, but inactive packs
            foreach (LocalMediaPack candidate in LMPContext.StoredLocalMediaPacks)
            {
                if (pack.RootPath == candidate.RootPath)
                {
                    result = true;
                    break;
                }
            }

            // Checking in active packs
            foreach (LocalMediaPack candidate in LMPContext.ActiveLocalMediaPacks)
            {
                if (pack.RootPath == candidate.RootPath)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        private void WriteLMPToDatabase(LocalMediaPack pack)
        {
            string[] values = new string[4];
            values[0] = pack.RootPath;
            values[1] = pack.BaseExtension.ToString();
            values[2] = Convert.ToInt32(pack.IsResultOfDriveSearch).ToString(); //0 or 1
            StringBuilder sb = new StringBuilder();
            foreach (string path in pack.GetAllFilePaths().Keys)
            {
                sb.Append(path);
                sb.Append("|");
            }
            sb.Remove(sb.Length - 1, 1); //removing unnecessary closing "|"
            values[3] = sb.ToString();
            databaseService.InsertInto("LocalMediaPacks", values);
        }
    }
}
