﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MetaBrainz.MusicBrainz.Interfaces;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Objects.Entities;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaBrainz.MusicBrainz.Objects {

  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal sealed class DiscIdLookupResult : IDiscIdLookupResult {

    internal DiscIdLookupResult(string discid, string json, JsonSerializerSettings jss) {
      // Currently this can return:
      // - a serialized Disc (id + releases)
      // - a serialized CD stub (id + tracks)
      // - a list of releases (as a serialized object containing only a "releases" property)
      // It would be nicer if the first two had a wrapper object with a disc and stub property, respectively.
      var jobj = JsonConvert.DeserializeObject(json, jss) as JObject;
      if (jobj == null)
        throw new ArgumentException($"Disc ID lookup for '{discid}' did not return a usable JSON result.\nContents: {json}");
      var jid = jobj["id"];
      if (jid != null && jobj["releases"] != null)
        this.Disc = JsonConvert.DeserializeObject<Disc>(json, jss);
      else if (jid != null && jobj["tracks"] != null)
        this.Stub = JsonConvert.DeserializeObject<CdStub>(json, jss);
      else {
        var jreleases = jobj["releases"];
        if (jreleases == null)
          throw new ArgumentException($"Disc ID lookup for '{discid}' returned a JSON result that could not be identified as a disc, stub or release list.\nContents: {json}");
        this.Releases = JsonConvert.DeserializeObject<Release[]>(jreleases.ToString(), jss);
      }
      this.Id = discid;
    }

    public string Id { get; }

    public IDisc Disc { get; }

    public IReadOnlyList<IRelease> Releases { get; }

    public ICdStub Stub { get; }

    /// <summary>Gets the textual representation of the disc ID lookup result.</summary>
    /// <returns>A string describing the lookup results.</returns>
    public override string ToString() {
      if (this.Disc != null)
        return "Disc: " + this.Disc;
      if (this.Stub != null)
        return "CD Stub: " + this.Stub;
      if (this.Releases != null)
        return $"{this.Releases.Count} Release(s)";
      return string.Empty; // should be impossible
    }

  }

}
