﻿using System;
using Onlab.DAL;

namespace Onlab.BLL
{
    /*
    Enum: ExtensionType
    Description:
        Extension of music files for which proper tagging and meta-data is supported.
    */
    public enum ExtensionType : ushort
    {
        MP3 = 0, //default
        FLAC = 1
    }

    /*
    Enum: MediaPlayerType
    Description:
        Identifier of media players that are supported for tracklist mixing.
    */
    public enum MediaPlayerType : ushort
    {
        Foobar2000 = 0 //default
    }

    /*
    Enum: MusicGenre
    Description:
        Currently handled genre of tracks.
    */
    [Flags]
    public enum MusicGenre : ushort //TODO: may need to be deleted after MusicBrainz integration
    {
        Rock = 1,
        Pop = 2,
        Rap = 4,
        Metal = 8,
        Dubstep = 16,
        Classical = 32,
        Xtreme = 64
    }

    /*
    Enum: MusicLanguage
    Description:
        Currently handled language of tracks.
    */
    [Flags]
    public enum MusicLanguage : ushort //TODO: may need to be deleted after MusicBrainz integration
    {
        ENG = 1,
        HUN = 2,
        GER = 4,
        FRE = 8,
        SPA = 16
    }

    /*
    Class: GlobalVariables
    Description:
        Provides static and global properties and variables for the application.
        Used mainly for utility data during running.
        Provides data for GlobalAlgorithms class.
    */
    public static class GlobalVariables
    {
        public static IDatabaseProvider DatabaseProvider;
        public static IFileProvider FileProvider;
        public static IEnvironmentProvider EnvironmentProvider;
        public static IMusicBrainzProvider MusicBrainzProvider;

        public static AppConfig AppConfig;
        public static LocalMediaPackContainer LocalMediaPackContainer; //persistent settings through the whole application
        //TODO: make persisent loading from config file stored locally on file system
        public static TracklistData TracklistData; //dynamic wrapper of data currently represented @ Tracklist tab table
        public static PlayzoneData PlayzoneData; //dynamic wrapper of data currently represented @ Playzone tab table
        //TODO: generalize data classes via base class or interface

        public static void Initialize()
        {
            DatabaseProvider = new DatabaseProvider();
            FileProvider = new FileProvider();
            EnvironmentProvider = new EnvironmentProvider();
            MusicBrainzProvider = new MusicBrainzProvider();

            AppConfig = new AppConfig();
            LocalMediaPackContainer = new LocalMediaPackContainer();
            TracklistData = new TracklistData();
            PlayzoneData = new PlayzoneData();
        }
    }
}