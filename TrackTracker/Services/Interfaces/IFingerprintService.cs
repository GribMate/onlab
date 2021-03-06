﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace TrackTracker.Services.Interfaces
{
    /*
     * Handles getting the fingerprint and AcoustID of a music file (and also decompressing FLAC files into WAV for fingerprinting).
    */
    public interface IFingerprintService
    {
        bool DetectDecompressToolAvailabilty(); //returns true if the external converter tool (EXE) is installed
        string DecompressFile(string sourceFile); //decompresses a FLAC file from sourceLocation to a WAV file at targetLocation

        Task RunFingerprinting(string filePath);
        Dictionary<string, object> GetDataOfLastRun();
        Task<Dictionary<string, Guid>> GetIDsByFingerprint(string fingerprint, int duration);
    }
}
