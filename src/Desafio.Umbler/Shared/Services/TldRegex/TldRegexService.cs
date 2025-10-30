using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Desafio.Umbler.Shared.Services.TldRegex
{
    public class TldRegexService : ITldRegexService
    {
        private readonly Lazy<Regex> _tldRemovalRegex;
        private readonly Lazy<HashSet<string>> _validTlds;

        public Regex TldRemovalRegex => _tldRemovalRegex.Value;
        public HashSet<string> ValidTlds => _validTlds.Value;
        public int TldCount => _validTlds.Value.Count;

        public TldRegexService()
        {
            _tldRemovalRegex = new Lazy<Regex>(BuildTldRemovalRegex);
            _validTlds = new Lazy<HashSet<string>>(LoadValidTlds);
        }

        public bool IsValidTld(string tld)
        {
            if (string.IsNullOrWhiteSpace(tld))
                return false;

            return ValidTlds.Contains(tld.ToLowerInvariant());
        }

        private string GetTldFilePath()
        {
            var tldFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tlds-alpha-by-domain.txt");

            if (!File.Exists(tldFilePath))
            {
                throw new FileNotFoundException(
                    $"CRITICAL: TLD file not found at '{tldFilePath}'. " +
                    "This file is required for proper domain validation. " +
                    "Download the official IANA TLD list from: https://data.iana.org/TLD/tlds-alpha-by-domain.txt " +
                    "and place it in the application root directory."
                );
            }

            return tldFilePath;
        }

        private HashSet<string> LoadValidTlds()
        {
            try
            {
                var tldFilePath = GetTldFilePath();

                var lines = File.ReadAllLines(tldFilePath)
                    .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
                    .Select(l => l.ToLowerInvariant());

                var tldSet = new HashSet<string>(lines, StringComparer.OrdinalIgnoreCase);

                if (tldSet.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"TLD file '{tldFilePath}' is empty or contains no valid TLDs. " +
                        "Please download the latest version from: https://data.iana.org/TLD/tlds-alpha-by-domain.txt"
                    );
                }

                return tldSet;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Error loading valid TLDs list. " +
                    "Make sure the file 'tlds-alpha-by-domain.txt' exists and is properly formatted.",
                    ex
                );
            }
        }

        private Regex BuildTldRemovalRegex()
        {
            try
            {
                var tldFilePath = GetTldFilePath();

                var tlds = File.ReadAllLines(tldFilePath)
                    .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
                    .Select(l => l.ToLowerInvariant())
                    .Select(tld => Regex.Escape(tld))
                    .ToList();

                var compositeTlds = new[] { "com.br", "co.uk", "co.jp", "com.au", "gov.br", "net.br", "org.br" }
                    .Select(tld => Regex.Escape(tld));

                tlds.AddRange(compositeTlds);

                tlds = tlds.OrderByDescending(t => t.Length).ToList();

                if (tlds.Count == 0)
                {
                    throw new InvalidOperationException(
                        "No TLDs loaded from file. Cannot build regex pattern."
                    );
                }

                var pattern = @"\.(" + string.Join("|", tlds) + @")$";

                return new Regex(
                    pattern,
                    RegexOptions.IgnoreCase | RegexOptions.Compiled
                );
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Error building TLD removal regex pattern.",
                    ex
                );
            }
        }
    }

    public interface ITldRegexService
    {
        Regex TldRemovalRegex { get; }
        HashSet<string> ValidTlds { get; }
        bool IsValidTld(string tld);
        int TldCount { get; }
    }
}
