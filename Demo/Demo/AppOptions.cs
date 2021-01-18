using Microsoft.Extensions.Options;

namespace Demo
{
    public class AppOptions : IOptions<AppOptions>
    {
        public string SignToWhere { get; set; } = null!;

        public string? AppIdOfTCaptchaForSms { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "<Pending>")]
        public string? UrlOfPrivacyAgreement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "<Pending>")]
        public string? UrlOfServiceAgreement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "<Pending>")]
        public string UrlOfAvatar { get; set; } = null!;

        public AppOptions Value => this;

    }
}
