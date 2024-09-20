using UnityEditor;
using UnityEngine.UIElements;
using Quiz;
using System.IO;
using UnityEngine;

/// <summary>
// This custom editor window is used for importing questions into the Quiz
// from their original JSON format. Users can select a folder containing JSON files,
// and this script will create corresponding ScriptableObjects with the imported data.
/// </summary>
public class QuestionImporterWindow : EditorWindow
{
    // Constants for window size and file paths
    const int k_WindowSizeX = 300;
    const int k_WindowSizeY = 200;
    const string k_QuestionPath = "Quiz/Data/Questions";
    const string k_DefaultPrefix = "Question";
    const string k_DefaultSuffix = "_Data";

    Button m_ImportButton;

    // Prepend and append strings
    TextField m_CustomPrefixField;
    TextField m_CustomSuffixField;

    // Add a menu item to open the Question Importer window
    [MenuItem("Window/QuizU/Question Importer")]
    public static void ShowWindow()
    {
        GetWindow<QuestionImporterWindow>("Question Importer");
    }

    // Called when the window is opened
    private void OnEnable()
    {
        // Set the window size
        minSize = new Vector2(k_WindowSizeX, k_WindowSizeY);

        // Position the window in the center of the screen
        float screenWidth = Screen.currentResolution.width;
        float screenHeight = Screen.currentResolution.height;
        position = new Rect((screenWidth - k_WindowSizeX) / 2, (k_WindowSizeY - 200) / 2, 300, 200);

        SetupUI();
    }

    // Set up the UI for the editor window
    private void SetupUI()
    {
        VisualElement root = rootVisualElement;

        m_CustomPrefixField = new TextField("Custom Prefix");
        root.Add(m_CustomPrefixField);

        m_CustomSuffixField = new TextField("Custom Suffix");
        root.Add(m_CustomSuffixField);

        m_ImportButton = new Button(ImportJsonFolder)
        {
            text = "Import JSON Folder"
        };
        root.Add(m_ImportButton);

        m_CustomPrefixField.value = k_DefaultPrefix;
        m_CustomSuffixField.value = k_DefaultSuffix;
    }

    // Import all JSON files from a selected folder
    private void ImportJsonFolder()
    {
        string folderPath = EditorUtility.OpenFolderPanel("Select JSON Folder", "", "");

        if (!string.IsNullOrEmpty(folderPath))
        {
            string[] filePaths = Directory.GetFiles(folderPath, "*.json");

            foreach (string path in filePaths)
            {
                string jsonData = File.ReadAllText(path);
                CreateQuestionSOFromJson(jsonData, Path.GetFileNameWithoutExtension(path));
            }
        }
    }

    // Create a QuestionSO ScriptableObject from a JSON string and save it as an asset
    private void CreateQuestionSOFromJson(string jsonData, string assetName)
    {
        // Get the custom prefix and suffix from the TextField elements
        string customPrefix = m_CustomPrefixField.value;
        string customSuffix = m_CustomSuffixField.value;

        // Create a new instance of the QuestionSO ScriptableObject
        QuestionSO questionSO = ScriptableObject.CreateInstance<QuestionSO>();

        // Deserialize the JSON data into a QuestionJsonData object, a temporary
        // struct to hold JSON data for conversion. This makes the JSON file more human-readable.
        QuestionJsonData questionJsonData = JsonUtility.FromJson<QuestionJsonData>(jsonData);

        QuestionSOEditor.PopulateQuestionSOFromJsonData(questionSO, questionJsonData);

        // Add the custom prefix and suffix to the asset name
        string modifiedAssetName = customPrefix + assetName + customSuffix;

        // Define a path and create the asset in the project
        string assetPath = $"Assets/{k_QuestionPath}/{modifiedAssetName}.asset";
        AssetDatabase.CreateAsset(questionSO, assetPath);
    }
}
