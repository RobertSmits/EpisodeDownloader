using System;
using System.Threading;
using System.Threading.Tasks;

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
        Task<Uri[]> GetShowSeasonsAsync(Uri showUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the episode urls for a given season url.
        /// </summary>
        /// <param name="seasonUrl"></param>
        /// <returns>
        /// An array whose elements contain the episode urls for the given seaso.
        /// </returns>
        Task<Uri[]> GetShowSeasonEpisodesAsync(Uri seasonUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the episode info urls for a given show url.
        /// </summary>
        /// <param name="episodeUrl"></param>
        /// <returns>
        /// The EpisodeInfo for the given episode.
        /// </returns>
        Task<EpisodeInfo> GetEpisodeInfoAsync(Uri episodeUrl, CancellationToken cancellationToken = default);
    }
}
