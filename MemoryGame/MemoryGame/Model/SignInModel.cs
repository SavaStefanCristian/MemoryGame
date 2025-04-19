using MemoryGame.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace MemoryGame.Model
{
    public class SignInModel
    {
        public SignInModel()
        {
            ImageList = new List<BitmapImage>();
            LoadImages();
            LoadPlayers();
        }

        public List<Player> Players { get; set; }
        public List<BitmapImage> ImageList { get; private set; }

        private string[] imageFiles;

        public void LoadImages()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\UserImages");

            imageFiles = Directory.GetFiles(folderPath, "*.*")
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string file in imageFiles)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                ImageList.Add(bitmap);
            }

        }

        public void LoadPlayers()
        {
            string playersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Users\players.json");

            Players = new();

            if (File.Exists(playersFilePath))
            {
                string jsonFromFile = File.ReadAllText(playersFilePath);

                if (jsonFromFile == "") return;

                List<Player>? loadedPeople = JsonSerializer.Deserialize<List<Player>>(jsonFromFile);

                if (loadedPeople == null) return;

                Players = loadedPeople;
            }
        }

        public BitmapImage? RetrievePlayerImage(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex > Players.Count) return null;

            int imageIndex = imageFiles.ToList().FindIndex(imagePath => Path.GetFileName(imagePath) == Players[playerIndex].Photo);

            if (imageIndex < 0 || imageIndex >= ImageList.Count) return null;

            return ImageList[imageIndex];
        }

        public void DeletePlayer(int playerIndex)
        {
            string savesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Users\Saves\" + Players[playerIndex].Name + ".json");
            if (File.Exists(savesFilePath))
            {
                File.Delete(savesFilePath);
            }

            string playersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Users\players.json");

            Players.RemoveAt(playerIndex);
            string json = JsonSerializer.Serialize(Players);
            File.WriteAllText(playersFilePath, json);
        }
    }
}
