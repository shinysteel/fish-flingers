using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FishFlingers.Localisation
{
    public interface ILocalisationManagerListener
    { }

    public class LocalisationManager : GameSystem<ILocalisationManagerListener>
    {
        private LocalisationManagerConfig _config;

        private Dictionary<LocalisationTerm, StringTableEntry> _termEntryMap = new();

        public override void Initialise(GameManagerConfig config)
        {
            _config = config.LocalisationManagerConfig;

            AsyncOperationHandle<IList<StringTable>> op = LocalizationSettings.StringDatabase.GetAllTables();

            op.Completed += completed =>
            {
                foreach (StringTable table in completed.Result)
                {
                    foreach (StringTableEntry entry in table.Values)
                    {
                        _termEntryMap.Add((LocalisationTerm)Utils.Math.HashLongToInt(entry.KeyId), entry);
                    }
                }
            };

            base.Initialise(config);
        }

        public string GetString(LocalisationTerm term)
        {
            return _termEntryMap[term].GetLocalizedString();
        }
    }
}