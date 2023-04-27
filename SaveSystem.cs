using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    // Instancja singletona
    private static SaveManager instance;

    // Interfejs strategii dla oszczędzania
    private ISaveStrategy saveStrategy;

    // Lokalna ścieżka zapisu
    private string localSavePath;

    // Ustawienia zapisu w chmurze
    private string cloudSaveKey;
    private bool useCloudSave;

    // Ustawienia serializaton JSON
    private JsonSerializerSettings jsonSettings;

    // Inicjalizacja instancji singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Ustaw strategię oszczędzania
    public void SetSaveStrategy(ISaveStrategy strategy)
    {
        saveStrategy = strategy;
    }

    // Ustawienie lokalnej ścieżki zapisu
    public void SetLocalSavePath(string path)
    {
        localSavePath = path;
    }

    // Skonfiguruj ustawienia zapisu w chmurze
    public void SetCloudSaveSettings(string key, bool useCloud)
    {
        cloudSaveKey = key;
        useCloudSave = useCloud;
    }

    // Ustawienie serializatora JSON
    public void SetJsonSerializerSettings(JsonSerializerSettings settings)
    {
        jsonSettings = settings;
    }

    // Zapisywanie danych
    public void Save(string key, object data)
    {
        // Wytworzenie obiektu zapisu danych z kluczem i danymi
        SaveData saveData = new SaveData(key, data);

        // Serializacja zapisanych danych do JSON
        string saveString = JsonConvert.SerializeObject(saveData, jsonSettings);

        // Jeśli zapisywanie w chmurze jest włączone, użyj strategii zapisywania w chmurze
        if (useCloudSave)
        {
            saveStrategy = new CloudSaveStrategy(cloudSaveKey);
        }
        // Stosować lokalną strategię oszczędzania
        else
        {
            saveStrategy = new LocalSaveStrategy(localSavePath);
        }

        // Zapisanie danych przy użyciu wybranej strategii
        saveStrategy.Save(saveString);
    }

    // Dane dotyczące obciążenia
    public T Load<T>(string key, T defaultValue)
    {
        // Jeśli zapisywanie w chmurze jest włączone, użyj strategii zapisywania w chmurze
        if (useCloudSave)
        {
            saveStrategy = new CloudSaveStrategy(cloudSaveKey);
        }
        // W przeciwnym razie należy użyć lokalnej strategii oszczędzania
        else
        {
            saveStrategy = new LocalSaveStrategy(localSavePath);
        }

        // Wczytanie zapisanych danych przy użyciu wybranej strategii
        string saveString = saveStrategy.Load(key);

        // Jeśli nie ma zapisanych danych, zwróć wartość domyślną
        if (string.IsNullOrEmpty(saveString))
        {
            return defaultValue;
        }

        // Deserializuj zapisane dane z JSON
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(saveString, jsonSettings);

        // Sprawdź, czy zapisane dane są zgodne z aktualną wersją
        if (!IsSaveDataCompatible(saveData))
        {
            Debug.LogWarning("Załadowane dane zapisu nie są zgodne z aktualną wersją.");
            return defaultValue;
        }

        // Zwróć zapisaną wartość danych
        return (T)saveData.Data;
    }

    // Sprawdź, czy zapisane dane są zgodne z aktualną wersją
    private bool IsSaveDataCompatible(SaveData saveData)
    {
        //TODO: Sprawdzenie kompatybilności z aktualną wersją
        // Na przykład sprawdzić, czy zapisane dane mają taki sam format i klucze jak aktualna wersja
        return true;
    }
}
// Interfejs strategii dla oszczędzania
public interface ISaveStrategy
{
    void Save(string data);
    string Load(string key);
}

// Lokalna strategia oszczędzania
public class LocalSaveStrategy : ISaveStrategy
{
    private string savePath; // ścieżka do folderu, w którym będą zapisywane pliki

    public LocalSaveStrategy(string path) // konstruktor przyjmuje ścieżkę do folderu
    {
        savePath = path;
    }

    public void Save(string data) // metoda służąca do zapisywania danych do pliku
    {
        File.WriteAllText(savePath, data); // zapisuje dane do pliku o ścieżce savePath
    }

    public string Load(string key) // metoda służąca do wczytywania danych z pliku o danym kluczu
    {
        string path = savePath + "/" + key; // tworzy ścieżkę do pliku o danym kluczu
        if (File.Exists(path)) // sprawdza, czy plik istnieje
        {
            return File.ReadAllText(path); // jeśli istnieje, wczytuje jego zawartość i zwraca ją
        }
        else
        {
            return null; // jeśli plik nie istnieje, zwraca null
        }
    }
}

// Strategia oszczędzania w chmurze
public class CloudSaveStrategy : ISaveStrategy
{
    private string saveKey;

    public CloudSaveStrategy(string key)
    {
        saveKey = key;
    }

    public void Save(string data)
    {
        // TODO: Wdrożenie funkcji zapisywania w chmurze przy użyciu określonego klucza
        Debug.Log("Zapisywanie do chmury za pomocą klucza: " + saveKey);
    }

    public string Load(string key)
    {
        // TODO: Zaimplementuj funkcjonalność ładowania chmury przy użyciu określonego klucza
        Debug.Log("Ładowanie z chmury za pomocą klucza: " + saveKey);
        return null;
    }
}

// Zapisz obiekt danych
[System.Serializable]
public class SaveData
{
    public string Key;
    public object Data;

    public SaveData(string key, object data)
    {
        Key = key;
        Data = data;
    }
}
