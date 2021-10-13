using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Blast.Core;
using Blast.Core.Interfaces;
using Blast.Core.Objects;
using Blast.Core.Results;

namespace NumberConverter.Fluent.Plugin
{
    public class NumberConversionSearchApp : ISearchApplication
    {
        private const string SearchAppName = "NumberConvertor";
        private readonly List<SearchTag> _searchTags;
        private readonly SearchApplicationInfo _applicationInfo;
        private readonly List<ISearchOperation> _supportedOperations;
        private readonly NumberConversionSearchAppSettings _appSettings;

        public NumberConversionSearchApp()
        {
            // For icon glyphs look at https://docs.microsoft.com/en-us/windows/uwp/design/style/segoe-ui-symbol-font
            _searchTags = new List<SearchTag>
            {
                new() { Name = ConversionType.Hex.ToString(), IconGlyph = "\uE8EF", Description = "Convert to hex" },
                new() { Name = ConversionType.Binary.ToString(), IconGlyph = "\uE8EF", Description = "Convert to binary" }
            };

            _supportedOperations = new List<ISearchOperation>
            {
                new ResultCopyOperation()
            };
            
            _applicationInfo = new SearchApplicationInfo(SearchAppName, "This apps converts hex to decimal", _supportedOperations)
            {
                MinimumSearchLength = 1,
                IsProcessSearchEnabled = false,
                IsProcessSearchOffline = false,
                ApplicationIconGlyph = "\uE8EF",
                SearchAllTime = ApplicationSearchTime.Fast,
                DefaultSearchTags = _searchTags
            };
            _applicationInfo.SettingsPage = _appSettings = new NumberConversionSearchAppSettings(_applicationInfo);
        }

        public ValueTask LoadSearchApplicationAsync()
        {
            // This is used if you need to load anything asynchronously on Fluent Search startup
            return ValueTask.CompletedTask;
        }

        public SearchApplicationInfo GetApplicationInfo()
        {
            return _applicationInfo;
        }

        public async IAsyncEnumerable<ISearchResult> SearchAsync(SearchRequest searchRequest, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested || searchRequest.SearchType == SearchType.SearchProcess)
            {
                yield break;
            }

            string searchedTag = searchRequest.SearchedTag;
            string searchedText = searchRequest.SearchedText.Trim();
            ConversionType conversionType = ConversionType.Any;

            // Check that the search tag is something this app can handle
            if (!string.IsNullOrWhiteSpace(searchedTag))
            {
                if (!searchedTag.Equals(SearchAppName, StringComparison.OrdinalIgnoreCase) && !Enum.TryParse(searchedTag, true, out conversionType))
                {
                    yield break;
                }
            }

            // Check that the searched text is a number
            if (!ParseNumber(searchedText, out int number, ref conversionType))
            {
                yield break;
            }

            // Convert number to hex
            if (conversionType is ConversionType.Any or ConversionType.Hex)
            {
                string convertedNumber = $"{number:X}";
                yield return CreateResult("0x", convertedNumber, _appSettings.CopyHexPrefix, _appSettings.ShowHexPrefix, ConversionType.Hex);
            }

            // Convert number to binary
            if (conversionType is ConversionType.Any or ConversionType.Binary)
            {
                string convertedNumber = Convert.ToString(number, 2);
                yield return CreateResult("0b", convertedNumber, _appSettings.CopyBinPrefix, _appSettings.ShowBinPrefix, ConversionType.Binary);
            }

            // Local helper function for result creation
            NumberConversionSearchResult CreateResult(string prefix, string converted, bool copyPrefix, bool showPrefix, ConversionType resultType)
            {
                return new NumberConversionSearchResult(number, SearchAppName, $"{(copyPrefix ? prefix : "")}{converted}", 
                    $"{searchedText} = {(showPrefix ? prefix : "")}{converted}", searchedText, resultType.ToString(), 2,
                    _supportedOperations, _searchTags);
            }
        }

        private bool ParseNumber(string input, out int result, ref ConversionType conversionType)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                result = -1;
                return false;
            }

            if (int.TryParse(input, out int number))
            {
                result = number;
                return true;
            }
            
            try
            {
                // hex number
                if (input.StartsWith("0x") && input.Length > 2)
                {
                    result = Convert.ToInt32(input, 16);
                    conversionType = conversionType == ConversionType.Any ? ConversionType.Binary : conversionType;
                    return true;
                }

                // binary number
                if (input.StartsWith("0b") && input.Length > 2)
                {
                    result = Convert.ToInt32(input[2..], 2);
                    conversionType = conversionType == ConversionType.Any ? ConversionType.Hex : conversionType;
                    return true;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            result = -1;
            return false;
        }

        public ValueTask<ISearchResult> GetSearchResultForId(object searchObjectId)
        {
            // This is used to calculate a search result after Fluent Search has been restarted
            // This is only used by the custom search tag feature
            return new();
        }

        public ValueTask<IHandleResult> HandleSearchResult(ISearchResult searchResult)
        {
            if (searchResult is not NumberConversionSearchResult numberSearchResult)
            {
                throw new InvalidCastException(nameof(NumberConversionSearchResult));
            }

            // Copy converted number to clipboard
            if (numberSearchResult.SelectedOperation is ResultCopyOperation)
            {
                TextCopy.Clipboard.SetText(numberSearchResult.ConvertedNumber);
            }
            
            return new(new HandleResult(true, false));
        }
    }
}