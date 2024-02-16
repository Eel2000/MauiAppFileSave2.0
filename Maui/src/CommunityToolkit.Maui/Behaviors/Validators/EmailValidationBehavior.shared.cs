﻿using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace CommunityToolkit.Maui.Behaviors;

/// <summary>
/// The <see cref="EmailValidationBehavior"/> is a behavior that allows users to determine whether or not text input is a valid e-mail address. 
/// For example, an <see cref="Entry"/> control can be styled differently depending on whether a valid or an invalid e-mail address is provided.
/// The validation is achieved through a regular expression that is used to verify whether or not the text input is a valid e-mail address.
/// It can be overridden to customize the validation through the properties it inherits from <see cref="ValidationBehavior"/>.
/// </summary>
public partial class EmailValidationBehavior : TextValidationBehavior
{
	/// <summary>
	/// A <see cref="Regex"/> to match an input is a valid email address
	/// Generated by <see cref="GeneratedRegexAttribute"/>
	/// </summary>
	/// <returns>Generated <see cref="Regex"/></returns>
	[GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, 250)]
	protected static partial Regex EmailRegex();

	/// <summary>
	/// A <see cref="Regex"/> to match the domain of an email address
	/// Generated by <see cref="GeneratedRegexAttribute"/>
	/// </summary>
	/// <returns>Generated <see cref="Regex"/></returns>
	[GeneratedRegex(@"(@)(.+)$", RegexOptions.None, 250)]
	protected static partial Regex EmailDomainRegex();

	/// <summary>
	/// Examines the email address domain and normalizes it to ASCII
	/// </summary>
	/// <param name="match"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	private protected static string DomainMapper(Match match)
	{
		var domainName = match.Groups[2].Value;
		if (domainName.StartsWith('-'))
		{
			throw new ArgumentException("Domain name cannot start with hyphen.");
		}

		// Use IdnMapping class to convert Unicode domain names.
		var idn = new IdnMapping();

		// Pull out and process domain name (throws ArgumentException on invalid)
		var asciiDomainName = idn.GetAscii(domainName);

		if (IsIPv4(asciiDomainName) && !IsValidIPv4(asciiDomainName))
		{
			throw new ArgumentException("Invalid IPv4 Address.");
		}

		if (IsIPv6(asciiDomainName) && !IsValidIPv6(asciiDomainName))
		{
			throw new ArgumentException("Invalid IPv6 Address.");
		}

		return match.Groups[1].Value + asciiDomainName;
	}

	/// <inheritdoc /> 
	protected override async ValueTask<bool> ValidateAsync(string? value, CancellationToken token)
	{
		return IsValidEmail(value) && await base.ValidateAsync(value, token);
	}

	/// <inheritdoc /> 
	protected override void OnAttachedTo(VisualElement bindable)
	{
		// Assign Keyboard.Email if the user has not specified a specific Keyboard layout
		if (bindable is InputView inputView && inputView.Keyboard == Keyboard.Default)
		{
			inputView.Keyboard = Keyboard.Email;
		}

		base.OnAttachedTo(bindable);
	}

	/// <inheritdoc /> 
	protected override void OnDetachingFrom(VisualElement bindable)
	{
		// Assign Keyboard.Default if the user has not specified a different Keyboard layout
		if (bindable is InputView inputView && inputView.Keyboard == Keyboard.Email)
		{
			inputView.Keyboard = Keyboard.Default;
		}

		base.OnDetachingFrom(bindable);
	}

	// https://docs.microsoft.com/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
	static bool IsValidEmail(string? email)
	{
		if (string.IsNullOrWhiteSpace(email)
			|| email.StartsWith('.')
			|| email.Contains("..", StringComparison.Ordinal)
			|| email.Contains(".@", StringComparison.Ordinal))
		{
			return false;
		}

		try
		{
			// Normalize the domain
			email = EmailDomainRegex().Replace(email, DomainMapper);
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
		catch (ArgumentException)
		{
			return false;
		}

		try
		{
			var domain = GetDomain(email);

			if (IsValidIPv4(domain) || IsValidIPv6(domain))
			{
				return MailAddress.TryCreate(email, out _);
			}

			return EmailRegex().IsMatch(email);
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
	}

	static string GetDomain(in string emailAddress) => EmailDomainRegex().Match(emailAddress).Groups[2].Value;

	static bool IsIPv4(in string domain)
	{
		return domain.All(x => char.IsDigit(x)
								   || x is '.'
								   || x is '['
								   || x is ']');
	}

	static bool IsIPv6(in string domain)
	{
		return domain.All(x => char.IsAsciiHexDigit(x)
									|| x is ':'
									|| x is '['
									|| x is ']'
									|| x is 'I'
									|| x is 'P'
									|| x is 'v');
	}

	static bool IsValidIPv4(in string domain)
	{
		var normalizedDomain = domain.TrimStart('[').TrimEnd(']');

		return IPAddress.TryParse(normalizedDomain, out var address)
				&& address.AddressFamily is System.Net.Sockets.AddressFamily.InterNetwork;
	}

	static bool IsValidIPv6(in string domain)
	{
		// `[IPv6:..]` is required for IPv6 Address-Literals https://www.rfc-editor.org/rfc/rfc5321#section-4.1.3
		var normalizedDomain = domain.TrimStart('[')
										.TrimStart('I')
										.TrimStart('P')
										.TrimStart('v')
										.TrimStart('6')
										.TrimStart(':')
										.TrimEnd(']');

		return IPAddress.TryParse(normalizedDomain, out var address)
				&& address.AddressFamily is System.Net.Sockets.AddressFamily.InterNetworkV6;
	}
}