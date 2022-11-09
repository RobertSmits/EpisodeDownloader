using System;
using System.Text.Json.Serialization;

namespace EpisodeDownloader.Downloader.Vrt.Models.Api;

public class SearchResponse
{
    [JsonPropertyName("details")]
    public Details Details { get; set; }
}

public class Details
{
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("data")]
    public Data Data { get; set; }
}
public class Data
{
    [JsonPropertyName("program")]
    public Program Program { get; set; }

    [JsonPropertyName("season")]
    public Season Season { get; set; }

    [JsonPropertyName("episode")]
    public Episode Episode { get; set; }
}

public class Program
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("whatsonId")]
    public string WhatsonId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("reference")]
    public Reference Reference { get; set; }
}

public class Season
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("title")]
    public MultiValue<string> Title { get; set; }

    [JsonPropertyName("numberOfEpisodes")]
    public MultiValue<int> NumberOfEpisodes { get; set; }

    [JsonPropertyName("TotalNumberOfEpisodes")]
    public MultiValue<int> totalNumberOfEpisodes { get; set; }
}

public class Episode
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("whatsonId")]
    public string WhatsonId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("available")]
    public bool Available { get; set; }

    [JsonPropertyName("videoId")]
    public string VideoId { get; set; }

    [JsonPropertyName("publicationId")]
    public string PublicationId { get; set; }

    [JsonPropertyName("number")]
    public MultiValue<int> Number { get; set; }

    [JsonPropertyName("onTime")]
    public MultiValue<DateTime> OnTime { get; set; }

    [JsonPropertyName("offTime")]
    public MultiValue<DateTime> OffTime { get; set; }

    [JsonPropertyName("duration")]
    public MultiValue<string> Duration { get; set; }

    [JsonPropertyName("region")]
    public MultiValue<string> Region { get; set; }
}

public class MultiValue<T>
{
    [JsonPropertyName("raw")]
    public T Raw { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("shortValue")]
    public string? ShortValue { get; set; }

    [JsonPropertyName("longValue")]
    public string? LongValue { get; set; }
}

public class Reference
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyName("modelUri")]
    public string ModelUri { get; set; }

    [JsonPropertyName("permalink")]
    public string Permalink { get; set; }

    [JsonPropertyName("passUserIdentity")]
    public bool PpassUserIdentity { get; set; }

    [JsonPropertyName("referenceType")]
    public string ReferenceType { get; set; }
}
