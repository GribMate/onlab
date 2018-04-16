﻿using System;
using System.Collections.Generic;



namespace Onlab.BLL
{
    public class AppConfig
    {
        private Dictionary<MediaPlayerType, string> mediaPlayerPaths;

        public AppConfig()
        {
            mediaPlayerPaths = new Dictionary<MediaPlayerType, string>();
        }

        public bool AddMediaPlayerPath(MediaPlayerType type, string path) //adds a new media player location
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
        public bool ModifyMediaPlayerPath(MediaPlayerType type, string newPath) //changes an already existing media player location
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
        public bool TryDeleteMediaPlayerPath(MediaPlayerType type)
        {
            throw new NotImplementedException();
        }
        public bool TryGetMediaPlayerPath(MediaPlayerType type, out string path)
        {
            throw new NotImplementedException();
        }
    }
}
