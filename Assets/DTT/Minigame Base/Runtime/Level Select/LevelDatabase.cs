using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;

namespace DTT.MinigameBase.LevelSelect
{
    /// <summary>
    /// Stores data about levels that can be used to populate a <see cref="LevelSelect"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "New Level Database", menuName = "DTT/Minigame Base/Level Database")]
    public class LevelDatabase : ScriptableObject
    {
        /// <summary>
        /// The initial amount of levels that will show up when creating the database.
        /// </summary>
        [SerializeField]
        private int _levelCount = 60;

        /// <summary>
        /// The amount of levels that will be unlocked if there is no save file present yet.
        /// </summary>
        [SerializeField]
        private int _initialUnlocks = 1;

        /// <summary>
        /// Unique ID for file saving.
        /// </summary>
        [SerializeField]
        private string _saveFolderName = Guid.NewGuid().ToString();

        /// <summary>
        /// Path to where the persistent file is located.
        /// </summary>
        private string _PersistentFilePath => Path.Combine(Application.persistentDataPath, _saveFolderName);

        /// <summary>
        /// The initial amount of levels that will show up when creating the database.
        /// </summary>
        public int LevelCount => _levelCount;

        /// <summary>
        /// The level data inside of the database.
        /// </summary>
        public ReadOnlyCollection<LevelData> Data => Array.AsReadOnly(_data);

#if UNITY_WEBGL && !UNITY_EDITOR
        /// <summary>
        /// Gives the data to the WebGLDataSaver and passes this on to the browser.
        /// </summary>
        /// <param name="dataToSave">The json string of data to save.</param>
        /// <param name="key">Key of the savefolder name.</param>
        [DllImport("__Internal")]
        public static extern void SaveDataToLocalStorage(string dataToSave, string key);

        /// <summary>
        /// Loads in the data from the browser.
        /// </summary>
        /// <returns>The data in a json string.</returns>
        /// <param name="key">Key of the savefolder name.</param>
        [DllImport("__Internal")]
        public static extern string LoadDataFromBrowser(string key);
#endif

        /// <summary>
        /// The level data inside of the database.
        /// </summary>
        [SerializeField]
        [Tooltip("The level data inside of the database.")]
        private LevelData[] _data;

        /// <summary>
        /// Resets the database based on the settings.
        /// </summary>
        public void UpdateList()
        {
            _data ??= new LevelData[_initialUnlocks];

            for (int i = 0; i < _data.Length; i++)
            {
                _data[i].levelNumber = i + 1;
                _data[i].locked = i >= _initialUnlocks;
            }
        }

        /// <summary>
        /// Sets the score of the index of a level.
        /// </summary>
        /// <param name="index">The index of the level. Generally should be one minus the level number.</param>
        /// <param name="score">The score to set the level to.</param>
        public void SetScore(int index, float score)
        {
            _data[index].score = score;
            Save();
        }
        
        /// <summary>
        /// Sets the locked state of the index of a level.
        /// </summary>
        /// <param name="index">The index of the level. Generally should be one minus the level number.</param>
        /// <param name="isLocked">Whether this level is locked.</param>
        public void SetLocked(int index, bool isLocked)
        {
            _data[index].locked = isLocked;
            Save();
        }

        /// <summary>
        /// Creates the path for the save file.
        /// </summary>
        private void CreatePath() => Directory.CreateDirectory(_PersistentFilePath);

        /// <summary>
        /// Saves data to persistent data path.
        /// </summary>
        private void Save()
        {
            CreatePath();

            string jsonData = JsonUtility.ToJson(new Wrapper<LevelData>(_data));

            string completePath = Path.Combine(_PersistentFilePath, "level_data.json");

#if UNITY_WEBGL && !UNITY_EDITOR
            SaveDataToLocalStorage(jsonData, _saveFolderName);
#else
            string fileLocation = Path.Combine(_PersistentFilePath, "level_data.json");
            File.WriteAllText(fileLocation, jsonData);
#endif
        }

        /// <summary>
        /// Loads in data from persistent data path.
        /// </summary>
        public void Load()
        {
            CreatePath();

#if UNITY_WEBGL && !UNITY_EDITOR
            string jsonData = LoadDataFromBrowser(_saveFolderName);
#else
            // Loading the Json data.

            string jsonData;

            string completePath = Path.Combine(_PersistentFilePath, "level_data.json");

            if (!File.Exists(completePath))
            {
                jsonData = string.Empty;
                return;
            }

            using FileStream fileStream = File.OpenRead(completePath);

            byte[] bytes = new byte[1024];
            int currentByte;

            string fileContent = string.Empty;
            while ((currentByte = fileStream.Read(bytes, 0, bytes.Length)) > 0)
                fileContent += Encoding.UTF8.GetString(bytes, 0, currentByte);

            jsonData = fileContent;
#endif
            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.LogWarning("No data found. Will use default level data.");
                return;
            }

            LevelData[] savedData = JsonUtility.FromJson<Wrapper<LevelData>>(jsonData).array;

            int length = Mathf.Max(savedData.Length, _levelCount);
            LevelData[] tempData = new LevelData[length];

            // Add in saved data.
            for (int i = 0; i < savedData.Length; i++)
                if (!savedData[i].locked)
                    tempData[i] = savedData[i];
                else
                    tempData[i].locked = true;

            // Add in the rest of the data.
            for (int i = 0; i < _levelCount; i++)
            {
                if (tempData[i].locked)
                    tempData[i] = _data[i];

                _data[i].levelNumber = i + 1;
            }

             _data = tempData;
        }

        /// <summary>
        /// Wraps array in class, so arrays can be saved using json.
        /// </summary>
        /// <typeparam name="T">Object type to be wrapped.</typeparam>
        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;

            public Wrapper(T[] array) => this.array = array;
        }
    }
}