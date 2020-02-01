using System;
using EpisodeDownloader.Contracts;

namespace EpisodeDownloader.Contracts.Downloader
{
    public interface IEpisodeProvider
    {
        /// <summary>
        /// Check whether the provided url can be handled by the current provider
        /// </summary>
        /// <param name="showUrl">The uri to test.</param>
        /// <returns>true if the provided url can be handled; otherwise, false.</returns>
        bool CanHandleUrl(Uri showUrl);

        /// <summary>
        /// Get the season urls for a given show url.
        /// </summary>
        /// <param name="showUrl"></param>
        /// <returns>
        /// An array whose elements contain the season urls for the given show.
        /// </returns>
        Uri[] GetShowSeasons(Uri showUrl);

        /// <summary>
        /// Get the episode urls for a given season url.
        /// </summary>
        /// <param name="seasonUrl"></param>
        /// <returns>
        /// An array whose elements contain the episode urls for the given seaso.
        /// </returns>
        Uri[] GetShowSeasonEpisodes(Uri seasonUrl);

        /// <summary>
        /// Get the episode info urls for a given show url.
        /// </summary>
        /// <param name="episodeUrl"></param>
        /// <returns>
        /// The EpisodeInfo for the given episode.
        /// </returns>
        EpisodeInfo GetEpisodeInfo(Uri episodeUrl);
    }
}
