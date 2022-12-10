using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Monry.StringBasedLocalization.Editor
{
    public static class LocalizationHelper
    {
        private static IDictionary<TableReference, SharedTableData> SharedTableDataCaches { get; } = new Dictionary<TableReference, SharedTableData>();
        private static Locale EditorLocale { get; set; }

        [MenuItem("CONTEXT/" + nameof(LocalizableMonoBehaviour) + "/Convert String Reference")]
        [MenuItem("CONTEXT/" + nameof(LocalizableScriptableObject) + "/Convert String Reference")]
        private static void ConvertStringReference(MenuCommand menuCommand)
        {
            EditorLocale ??= LocalizationSettings.Instance.GetAvailableLocales().Locales.First();
            if (EditorLocale == default)
            {
                Debug.LogWarning("No Locales configured.");
                return;
            }
            var instance = menuCommand.context;
            var type = instance.GetType();
            var fields = type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => typeof(LocalizedString).IsAssignableFrom(x.FieldType));
            foreach (var field in fields)
            {
                var localizedString = (LocalizedString)field.GetValue(instance);
                if (localizedString == default)
                {
                    continue;
                }
                if (!SharedTableDataCaches.TryGetValue(localizedString.TableReference, out var sharedTableData))
                {
                    SharedTableDataCaches[localizedString.TableReference] = sharedTableData = LocalizationSettings.Instance.GetStringDatabase().GetTable(localizedString.TableReference, EditorLocale).SharedData;
                }
                // string → TableReference, string → TableEntryReference の暗黙キャストを用いる
                localizedString.TableReference = localizedString.TableReference.TableCollectionName;
                localizedString.TableEntryReference = localizedString.TableEntryReference.ResolveKeyName(sharedTableData);
            }
        }
    }
}
