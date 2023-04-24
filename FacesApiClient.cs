using Refit;

internal interface IFacesApi {
    [Multipart]
    [Post("/upload_image")]
    Task<bool> UploadImage([AliasAs("image")] IEnumerable<StreamPart> images);
}