﻿using Smartstore.Collections;
using Smartstore.Core.Catalog.Products;
using Smartstore.Core.Content.Media;
using Smartstore.Data;
using Smartstore.IO;
using Smartstore.Net.Http;

namespace Smartstore.Core.DataExchange.Import
{
    /// <summary>
    /// Helper to import images (like product and category images) in a performant way.
    /// Images are downloaded if <see cref="DownloadManagerItem.Url"/> is specified and
    /// they will not be imported if they already exist (duplicate check).
    /// </summary>
    public interface IMediaImporter
    {
        /// <summary>
        /// A handler that is called when reportable events such as errors occur.
        /// </summary>
        Action<ImportMessage, DownloadManagerItem> MessageHandler { get; set; }

        /// <summary>
        /// Creates a download manager item.
        /// </summary>
        /// <param name="imageDirectory">
        /// Directory with images to be imported. 
        /// In that case, the images in the import file are referenced by file path (absolute or relative).
        /// </param>
        /// <param name="downloadDirectory">Directory in which the downloaded images will be saved.</param>
        /// <param name="entity">The entity to which the file belongs. E.g. <see cref="Product"/>, <see cref="Category"/> etc.</param>
        /// <param name="urlOrPath">URL or path to download.</param>
        /// <param name="state">Any state to identify the source later after batch save. E.g. <see cref="ImportRow{T}"/> etc.</param>
        /// <param name="displayOrder">Display order of the item.</param>
        /// <param name="fileNameLookup">Lookup for existing file names to avoid name duplicates.</param>
        /// <returns>Download manager item or <c>null</c> if none could be created.</returns>
        DownloadManagerItem CreateDownloadItem(
            IDirectory imageDirectory,
            IDirectory downloadDirectory,
            BaseEntity entity,
            string urlOrPath,
            object state = null,
            int displayOrder = 0,
            HashSet<string> fileNameLookup = null);

        /// <summary>
        /// Imports a batch of images assigned directly to one entity.
        /// </summary>
        /// <typeparam name="T">The type of entity for which the file are imported.</typeparam>
        /// <param name="downloadManagerItems">Collection of product images to be imported. Images are downloaded if <see cref="DownloadManagerItem.Url"/> is specified.</param>
        /// <param name="album">Media album to be assigned to the imported images.</param>
        /// <param name="addMediaFile">Callback function to assign the imported file to the designated entity.</param>
        /// <param name="checkAssignedFile">Callback function to check if the file to be imported is already assigned to the designated entity.</param>
        /// <param name="checkExistingFile">Defines whether to check for existing files in the same media folder.</param>
        /// <param name="duplicateFileHandling">A value indicating how to handle duplicate images.</param>
        /// <returns>Number of new images.</returns>
        Task<int> ImportMediaFileAsync<T>(
            DbContextScope scope,
            ICollection<DownloadManagerItem> downloadManagerItems,
            TreeNode<MediaFolderNode> album,
            Action<T, int> addMediaFile,
            Func<T, FileStream, Task<bool>> checkAssignedFile,
            bool checkExistingFile,
            DuplicateFileHandling duplicateFileHandling = DuplicateFileHandling.Rename,
            CancellationToken cancelToken = default) where T : BaseEntity;

        /// <summary>
        /// Imports a batch of images assigned to parent entities via IMediaFile (e.g. <see cref="ProductMediaFile"/>).
        /// </summary>
        /// <param name="items">Collection of product images to be imported. Images are downloaded if <see cref="DownloadManagerItem.Url"/> is specified.</param>
        /// <param name="existingFiles">Multimap of already assigned media files.</param>
        /// <param name="album">Media album to be assigned to the imported images.</param>
        /// <param name="addMediaFile">
        ///     Callback function to assign imported files to designated entities which derive from IMediaFile (e.g. <see cref="ProductMediaFile"/>).
        ///     Returns the assigned mediafile so it can be added to existing files and won't be imported again.
        /// </param>
        /// <param name="duplicateFileHandling">A value indicating how to handle duplicate images.</param>
        /// <returns>Number of new images.</returns>
        Task<int> ImportMediaFilesAsync(
            DbContextScope scope,
            ICollection<DownloadManagerItem> items,
            Multimap<int, IMediaFile> existingFiles,
            TreeNode<MediaFolderNode> album,
            Func<MediaFile, DownloadManagerItem, IMediaFile> addMediaFile,
            DuplicateFileHandling duplicateFileHandling = DuplicateFileHandling.Rename,
            CancellationToken cancelToken = default);

        /// <summary>
        /// Imports a batch of product images.
        /// </summary>
        /// <param name="items">Collection of product images to be imported. Images are downloaded if <see cref="DownloadManagerItem.Url"/> is specified.</param>
        /// <param name="duplicateFileHandling">A value indicating how to handle duplicate images.</param>
        /// <returns>Number of new images.</returns>
        Task<int> ImportProductImagesAsync(
            DbContextScope scope,
            ICollection<DownloadManagerItem> items,
            DuplicateFileHandling duplicateFileHandling = DuplicateFileHandling.Rename,
            CancellationToken cancelToken = default);

        /// <summary>
        /// Imports a batch of category images.
        /// </summary>
        /// <param name="items">Collection of category images to be imported. Images are downloaded if <see cref="DownloadManagerItem.Url"/> is specified.</param>
        /// <param name="duplicateFileHandling">A value indicating how to handle duplicate images.</param>
        /// <returns>Number of new images.</returns>
        Task<int> ImportCategoryImagesAsync(
            DbContextScope scope,
            ICollection<DownloadManagerItem> items,
            DuplicateFileHandling duplicateFileHandling = DuplicateFileHandling.Rename,
            CancellationToken cancelToken = default);

        /// <summary>
        /// Imports a batch of customer avatars.
        /// </summary>
        /// <param name="items">Collection of customer avatars to be imported. Images are downloaded if <see cref="DownloadManagerItem.Url"/> is specified.</param>
        /// <param name="duplicateFileHandling">A value indicating how to handle duplicate images.</param>
        /// <returns>Number of new images.</returns>
        Task<int> ImportCustomerAvatarsAsync(
            DbContextScope scope,
            ICollection<DownloadManagerItem> items,
            DuplicateFileHandling duplicateFileHandling = DuplicateFileHandling.Rename,
            CancellationToken cancelToken = default);
    }
}
