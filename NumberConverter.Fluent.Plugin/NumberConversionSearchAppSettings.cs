using Blast.API.Settings;
using Blast.Core.Objects;

namespace NumberConverter.Fluent.Plugin
{
    public class NumberConversionSearchAppSettings : SearchApplicationSettingsPage
    {
        private const string BinCategory = "Convert to Binary";
        private const string HexCategory = "Convert to Hex";

        public NumberConversionSearchAppSettings(SearchApplicationInfo searchApp) : base(searchApp)
        {
        }

        [Setting(SettingCategoryName = BinCategory, Name = "Show Binary Prefix", Description = "", DefaultValue = true, CanSyncSetting = true)]
        public bool ShowBinPrefix { get; set; }

        [Setting(SettingCategoryName = BinCategory, Name = "Copy Binary Prefix", Description = "", DefaultValue = true, CanSyncSetting = true)]
        public bool CopyBinPrefix { get; set; }

        [Setting(SettingCategoryName = HexCategory, Name = "Show Hex Prefix", Description = "", DefaultValue = true, CanSyncSetting = true)]
        public bool ShowHexPrefix { get; set; }

        [Setting(SettingCategoryName = HexCategory, Name = "Copy Hex Prefix", Description = "", DefaultValue = true, CanSyncSetting = true)]
        public bool CopyHexPrefix { get; set; }
    }
}
