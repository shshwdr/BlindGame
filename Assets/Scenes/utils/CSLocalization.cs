// using System.Collections;
// using System.Collections.Generic;
// using I2.Loc;
// using TMPro;
// using UnityEngine;
//
// public enum Languages
// {
//     Chinese,English, 
// }
// public static class CSLocalizationManager
// {
//     public static string GetLocalizedString(string stringToLocalize, string defaultString = null)
//     {
//         if (defaultString == null)
//             defaultString = stringToLocalize;
//
//         //if (I2.Loc.LocalizationManager.GetTermsList().Contains(stringToLocalize))
//         {
//
//             string localString = I2.Loc.LocalizationManager.GetTranslation(stringToLocalize);
//
//             if (localString == stringToLocalize)
//
//                 return defaultString;
//             else if (localString == null) return defaultString;
//             else
//
//                 return localString.Replace("\\n", "\n");
//         }
//         // else
//         // {
//         //     return defaultString;
//         // }
//     }
//
//     public static void SetText(TMP_Text text, string stringToLocalize)
//     {
//         var translated = GetLocalizedString(stringToLocalize);
//         if (translated == stringToLocalize)
//         {
//             text.text = translated;
//         }else
//         {
//             text.GetComponent<Localize>().Term = stringToLocalize;
//
//         }
//     }
//     
//     public static void SetLanguage(int languageInt)
//     {
//         //EventPool.Trigger("ChangeLanguage");
//         // foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>(true))
//         // {
//         //     if (text.GetComponentInParent<Dropdown>() == null)
//         //     {
//         //         if (languageInt != 0)
//         //         {
//         //             if (text.font == defaultMenuFont)
//         //             {
//         //                 text.font = GetSafeFont();
//         //             }
//         //         }
//         //         else
//         //         {
//         //             if (text.font == oldFont)
//         //             {
//         //                 text.font = defaultMenuFont;
//         //             }
//         //         }
//         //     }
//         // }
//     }
// }