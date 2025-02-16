namespace Jobseeker.Application.Common;

public record FileUpload(Stream Content, string FileName, string ContentType, long Length);
