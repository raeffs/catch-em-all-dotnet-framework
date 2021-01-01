using Cirrus.Attributes;
using Cirrus.Engine.ViewModel;
using Cirrus.Module.CatchEmAll.DAL;
using Cirrus.Validation.Attributes;

namespace Cirrus.Module.CatchEmAll.Models
{
    [Name(nameof(DataObjects.CatchEmAll_Settings))]
    internal class Settings
    {
        [Group(nameof(DataObjects.CatchEmAll_SettingsUser), ChildName = nameof(DataObjects.CatchEmAll_SettingsUser))]
        [Choice(ChoiceType.Toggle)]
        [Name(nameof(DataObjects.CatchEmAll_EnableEmailNotification))]
        public bool EnableEmailNotification { get; set; }

        [Group(nameof(DataObjects.CatchEmAll_SettingsUser), ChildName = nameof(DataObjects.CatchEmAll_SettingsUser))]
        [Choice(ChoiceType.Toggle)]
        [Name(nameof(DataObjects.CatchEmAll_EnableIftttNotification))]
        public bool EnableIftttNotification { get; set; }

        [Group(nameof(DataObjects.CatchEmAll_SettingsUser), ChildName = nameof(DataObjects.CatchEmAll_SettingsUser))]
        [EmailAddress]
        [Text(TextType.SingleLine)]
        [Name(nameof(DataObjects.CatchEmAll_Email))]
        public string Email { get; set; }

        [Group(nameof(DataObjects.CatchEmAll_SettingsUser), ChildName = nameof(DataObjects.CatchEmAll_SettingsUser))]
        [EmailAddress]
        [Text(TextType.SingleLine)]
        [Name(nameof(DataObjects.CatchEmAll_AlternativeEmail))]
        public string AlternativeEmail { get; set; }

        [Group(nameof(DataObjects.CatchEmAll_SettingsUser), ChildName = nameof(DataObjects.CatchEmAll_SettingsUser))]
        [Text(TextType.SingleLine)]
        [Name(nameof(DataObjects.CatchEmAll_IftttMakerKey))]
        public string IftttMakerKey { get; set; }

        [Group(nameof(DataObjects.CatchEmAll_SettingsUser), ChildName = nameof(DataObjects.CatchEmAll_SettingsUser))]
        [Text(TextType.SingleLine)]
        [Name(nameof(DataObjects.CatchEmAll_IftttMakerEventName))]
        public string IftttMakerEventName { get; set; }

        [Group(nameof(DataObjects.CatchEmAll_SettingsQuery), ChildName = nameof(DataObjects.CatchEmAll_SettingsQuery))]
        [Choice(ChoiceType.Toggle)]
        [Name(nameof(DataObjects.CatchEmAll_EnableNotificationsDefault))]
        public bool EnableNotificationsDefault { get; set; }

        [Group(nameof(DataObjects.CatchEmAll_SettingsQuery), ChildName = nameof(DataObjects.CatchEmAll_SettingsQuery))]
        [Choice(ChoiceType.Toggle)]
        [Name(nameof(DataObjects.CatchEmAll_AutoFilterDeletedDuplicatesDefault))]
        public bool AutoFilterDeletedDuplicatesDefault { get; set; }
    }
}
