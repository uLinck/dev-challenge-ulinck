using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Desafio.Umbler.Features.DomainContext.Validators
{
    public static class TldValidator
    {
        private static readonly HashSet<string> ValidTlds;

        static TldValidator()
        {
            try
            {
                var tldFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tlds-alpha-by-domain.txt");
                
                if (!File.Exists(tldFilePath))
                {
                    throw new FileNotFoundException(
                        $"TLD file not found: {tldFilePath}. " +
                        "Download from: https://data.iana.org/TLD/tlds-alpha-by-domain.txt"
                    );
                }

                var lines = File.ReadAllLines(tldFilePath)
                    .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
                    .Select(l => l.ToLowerInvariant());

                ValidTlds = new HashSet<string>(lines, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Error loading valid TLDs list. " +
                    "Make sure the file 'tlds-alpha-by-domain.txt' exists in the application directory.",
                    ex
                );
            }
        }

        public static bool IsValidTld(string tld)
        {
            if (string.IsNullOrWhiteSpace(tld))
                return false;

            return ValidTlds.Contains(tld.ToLowerInvariant());
        }

        public static int Count => ValidTlds.Count;
    }
}
