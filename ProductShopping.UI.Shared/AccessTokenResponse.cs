public sealed class AccessTokenResponse
{
    public bool isSuccess { get; set; }
    public string? value { get; set; }
    public string[]? errors { get; set; }
}