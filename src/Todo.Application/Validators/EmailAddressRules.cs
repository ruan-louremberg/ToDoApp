using System.Net.Mail;

namespace ToDoApp.Application.Validators;

internal static class EmailAddressRules
{
    public static bool HasValidDomain(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            !MailAddress.TryCreate(email, out var address) ||
            !string.Equals(address.Address, email, StringComparison.Ordinal))
        {
            return false;
        }

        var labels = address.Host.Split('.');
        return labels.Length > 1 &&
               labels.All(label => label.Length > 0) &&
               labels[^1].Length > 1;
    }
}
