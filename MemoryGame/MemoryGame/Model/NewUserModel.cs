using MemoryGame.Entities;
using MemoryGame.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MemoryGame.Model
{
    public class NewUserModel : BaseVM
    {
        public NewUserModel()
        {
            ImageList = new List<BitmapImage>();
            LoadImages();
        }

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

        public bool CreatePlayer(string name, int imageIndex)
        {
            string playersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Users\players.json");

            List<Player> allPlayers = new ();

            if (File.Exists(playersFilePath))
            {
                string jsonFromFile = File.ReadAllText(playersFilePath);

                if (jsonFromFile != "")
                {

                    List<Player>? loadedPeople = JsonSerializer.Deserialize<List<Player>>(jsonFromFile);
                    if (loadedPeople != null)
                    {
                        allPlayers = loadedPeople;
                        foreach (Player player in loadedPeople)
                        {
                            if (player.Name == name) return false;
                        }
                    }
                }
            }

            allPlayers.Add(new Player { Name = name, Photo = Path.GetFileName(imageFiles[imageIndex]) });

            string json = JsonSerializer.Serialize(allPlayers);
            File.WriteAllText(playersFilePath, json);


            return true;
        }
    }
}
