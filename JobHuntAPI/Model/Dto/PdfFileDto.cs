namespace JobHuntAPI.Model.Dto
{
	public class PdfFileDto
	{
		public required string FileName { get; set; } = string.Empty;
		public required byte[] Content { get; set; } = Array.Empty<byte>();
		public required string ContentType { get; set; } = string.Empty;
	}
}
