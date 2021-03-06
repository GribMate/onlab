﻿using System;
using System.Collections.Generic;
using System.Linq;



namespace TrackTracker.BLL.Enums
{
    /*
     * Currently supported extensions of music files by TrackTracker.
     * Note: WAV or APE support might be the next reasonable update.
    */
    public enum SupportedFileExtension
    {
        MP3 = 1,
        FLAC = 2,

        All = 0, // Means that all of the supported formats should be handled

        Unknown = -1
    }

    public static class SupportedFileExtensionConverter  // Provides static methods to manipulate SupportedFileExtensions
    {
        public static SupportedFileExtension ParseExtensionString(string ext)
        {
            if (String.IsNullOrWhiteSpace(ext))
                throw new ArgumentNullException(nameof(ext), $"Cannot convert to {nameof(SupportedFileExtension)}, since parameter is null or empty.");

            try
            {
                return (SupportedFileExtension)Enum.Parse(typeof(SupportedFileExtension), ext.ToUpper());
            }
            catch (Exception) // ArgumentException or OverflowException -> don't need to differentiate
            {
                return SupportedFileExtension.Unknown;
            }
        }
        public static List<SupportedFileExtension> GetAll()
        {
            List<SupportedFileExtension> enumList = new List<SupportedFileExtension>();

            foreach (SupportedFileExtension enumVal in Enum.GetValues(typeof(SupportedFileExtension)).Cast<SupportedFileExtension>())
            {
                if (enumVal == SupportedFileExtension.All || enumVal == SupportedFileExtension.Unknown)
                    continue; // We want to retrieve only the exact file format values

                enumList.Add(enumVal);
            }

            return enumList;
        }
        public static List<string> GetAllString()
        {
            List<string> enumList = new List<string>();

            foreach (SupportedFileExtension enumVal in Enum.GetValues(typeof(SupportedFileExtension)).Cast<SupportedFileExtension>())
            {
                if (enumVal == SupportedFileExtension.All || enumVal == SupportedFileExtension.Unknown)
                    continue; // We want to retrieve only the exact file format values

                enumList.Add(enumVal.ToString());
            }

            return enumList;
        }
    }
}
