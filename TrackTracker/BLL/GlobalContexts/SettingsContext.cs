﻿using System;
using System.Collections.Generic;

using TrackTracker.BLL.Enums;



namespace TrackTracker.BLL.GlobalContexts
{
    /*
     * Holds the configuration of the application (every setting that can be altered by the user).
    */
    public static class SettingsContext //TODO: CleanCode
    {
        // Holds exactly one path per MediaPlayerType (or nothing)
        private static Dictionary<SupportedMediaPlayers, string> mediaPlayerPaths = new Dictionary<SupportedMediaPlayers, string>();


        public static bool AddMediaPlayerPath(SupportedMediaPlayers type, string path) //adds a new media player location
        {
            if (path is null) throw new ArgumentNullException();
            if (path.Length < 4) throw new ArgumentException(); //"C:\x" is 4 chars

            if (mediaPlayerPaths.ContainsKey(type)) return false; //cannot add, already added, use ModifyMediaPlayerPath() instead
            else
            {
                mediaPlayerPaths.Add(type, path);
                return true; //added successfully
            }
        }
        public static bool ModifyMediaPlayerPath(SupportedMediaPlayers type, string newPath) //changes an already existing media player location
        {
            if (newPath is null) throw new ArgumentNullException();
            if (newPath.Length < 4) throw new ArgumentException(); //"C:\x" is 4 chars

            if (!mediaPlayerPaths.ContainsKey(type)) return false; //cannot modify non existent entry, use AddMediaPlayerPath() instead
            else
            {
                mediaPlayerPaths.Remove(type);
                mediaPlayerPaths.Add(type, newPath);
                return true; //added successfully
            }
        }
        public static bool DeleteMediaPlayerPath(SupportedMediaPlayers type) //deletes an already existing media player location
        {
            //we don't have to check arguments for exceptions, since "type" is always a valid value of MediaPlayerType
            if (!mediaPlayerPaths.ContainsKey(type)) return false; //cannot delete non existent entry
            else
            {
                mediaPlayerPaths.Remove(type);
                return true; //deleted successfully
            }
        }
        public static bool GetMediaPlayerPath(SupportedMediaPlayers type, out string path) //returns an already existing media player location or null
        {
            //we don't have to re-write "get value" logic, since Dictionary already provides it
            //however, we still want to hide the internal data structure
            return mediaPlayerPaths.TryGetValue(type, out path);
        }
        public static Dictionary<SupportedMediaPlayers, string> GetAllMediaPlayerPaths() //returns all of the paths if any
        {
            return new Dictionary<SupportedMediaPlayers, string>(mediaPlayerPaths); //copying a new Dictionary, since we don't want the original to be modified by reference
        }
    }
}
